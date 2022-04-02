module StoryService

open System
open Fable.SimpleHttp
open Thoth.Json
open Deferred
open Cache
open Types
open Mapper

let private getStoryIdsUrl = sprintf "https://hacker-news.firebaseio.com/v0/%sstories.json?print=pretty"
let private getItemUrl = sprintf "https://hacker-news.firebaseio.com/v0/item/%A.json?print=pretty"
let private storyUrl =
    {| New = getStoryIdsUrl "new"
       Top = getStoryIdsUrl "top"
       Best = getStoryIdsUrl "best"
       Ask = getStoryIdsUrl "ask"
       Show = getStoryIdsUrl "show"
       Job = getStoryIdsUrl "job" |}

let private expirationSpan = new TimeSpan(hours = 0, minutes = 0, seconds = 10)

let private getStoryIdsInternal url (): Async<Result<bigint list, string>> = async {
    let! (statusCode, responseText) = Http.get url
    match statusCode with
    | 200 ->
        let crowdedStoryIds = Decode.fromString (Decode.list Decode.bigint) responseText
        let storyIds = Result.map (List.truncate 10) crowdedStoryIds
        return storyIds
    | code -> return Error (sprintf "Error! Status code: %d" code)
}

let getNewStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.New)

let getTopStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.Top)

let getBestStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.Best)

let getAskStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.Ask)

let getShowStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.Show)

let getJobStoryIds: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) (getStoryIdsInternal storyUrl.Job)

let private getStoryInternal (storyId: bigint): Async<Result<HNItem, string>> = async {
    let! (statusCode, responseText) = Http.get (getItemUrl storyId)
    match statusCode with
    | 200 ->
        let story = Decode.fromString hnItemDecoder responseText
        return story
    | code -> return Error (sprintf "Error! Status code: %d" code)
}

let getStory: (bigint -> Async<Result<HNItem, string>>) =
    cached (Some expirationSpan) getStoryInternal
