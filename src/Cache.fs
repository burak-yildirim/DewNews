module Cache

open System

let private scheduleCallback<'T> (dueTime: TimeSpan option) (callback: 'T -> unit) (t: 'T) =
    match dueTime with
    | None -> ()
    | Some dt -> 
        async {
            do! Async.Sleep dt
            callback t
        }
        |> Async.StartImmediate


let cached<'T, 'R when 'T : comparison> (expire: TimeSpan option) (loader: 'T -> 'R) =
    let mutable cache: Map<'T, (DateTimeOffset * 'R)> = Map.empty
    let clear key = cache <- Map.remove key cache
    (fun (t: 'T) -> 
        let now = DateTimeOffset.Now
        match Map.tryFind t cache with
        | Some _ -> ()
        | _ ->
            let newValue = loader t
            cache <- Map.add t (now, newValue) cache
            scheduleCallback expire clear t
        
        Map.find t cache |> snd)

let cached2<'T, 'U, 'R when 'T : comparison and 'U : comparison>
    (expire: TimeSpan option) (loader: 'T -> 'U -> 'R) =
    let mutable cache: Map<('T * 'U), (DateTimeOffset * 'R)> = Map.empty
    let clear key = cache <- Map.remove key cache
    (fun (t: 'T) (u: 'U) -> 
        let now = DateTimeOffset.Now
        match Map.tryFind (t, u) cache with
        | Some _ -> ()
        | _ ->
            let newValue = loader t u
            cache <- Map.add (t, u) (now, newValue) cache
            scheduleCallback expire clear (t, u)
        
        Map.find (t, u) cache |> snd)

let cached3<'T, 'U, 'K, 'R when 'T : comparison and 'U : comparison and 'K : comparison>
    (expire: TimeSpan option) (loader: 'T -> 'U -> 'K -> 'R) =
    let mutable cache: Map<('T * 'U * 'K), (DateTimeOffset * 'R)> = Map.empty
    let clear key = cache <- Map.remove key cache
    (fun (t: 'T) (u: 'U) (k: 'K) -> 
        let now = DateTimeOffset.Now
        match Map.tryFind (t, u, k) cache with
        | Some _ -> ()
        | _ ->
            let newValue = loader t u k
            cache <- Map.add (t, u, k) (now, newValue) cache
            scheduleCallback expire clear (t, u, k)
        
        Map.find (t, u, k) cache |> snd)
