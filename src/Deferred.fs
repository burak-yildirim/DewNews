module Deferred

type Deferred<'T> =
    | NotStartedYet
    | InProgress
    | Resolved of  'T

type DeferredResult<'T> = Deferred<Result<'T, string>>

[<RequireQualifiedAccess>]
module Deferred =
    let isResolved = function
        | Resolved _ -> true
        | _ -> false
    
    let isInProgress = function
        | InProgress -> true
        | _ -> false
    
    let map (transform: 'T -> 'U) (deferred: Deferred<'T>): Deferred<'U> =
        match deferred with
        | NotStartedYet -> NotStartedYet
        | InProgress -> InProgress
        | Resolved t -> Resolved (transform t)
    
    let hasExists (predicate: 'T -> bool) = function
        | Resolved t -> predicate t
        | _ -> false
    
    let bind (transform: 'T -> Deferred<'U>) (deferred: Deferred<'T>): Deferred<'U> =
        match deferred with
        | NotStartedYet -> NotStartedYet
        | InProgress -> InProgress
        | Resolved t -> transform t
