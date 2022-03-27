module StoryService

open System
open Fable.SimpleHttp
open Thoth.Json
open Deferred
open Cache
open Types
open Mapper

let private getStoryUrl = sprintf "https://hacker-news.firebaseio.com/v0/%sstories.json?print=pretty"
let private getItemUrl = sprintf "https://hacker-news.firebaseio.com/v0/item/%A.json?print=pretty"
let private storyUrl =
    {| New = getStoryUrl "new"
       Top = getStoryUrl "top"
       Best = getStoryUrl "best"
       Ask = getStoryUrl "ask"
       Show = getStoryUrl "show"
       Job = getStoryUrl "job" 
       Item = getItemUrl |}

let private expirationSpan = new TimeSpan(hours = 0, minutes = 0, seconds = 10)

let private getTopStoriesInternal (): Async<Result<bigint list, string>> = async {
    let! (statusCode, responseText) = Http.get storyUrl.Top
    match statusCode with
    | 200 ->
        let crowdedStoryIds = Decode.fromString (Decode.list Decode.bigint) responseText
        let storyIds = Result.map (List.truncate 10) crowdedStoryIds
        return storyIds
    | code -> return Error (sprintf "Error! Status code: %d" code)
}

let getTopStories: (unit -> Async<Result<bigint list, string>>) =
    cached (Some expirationSpan) getTopStoriesInternal

let private getStoryInternal (storyId: bigint): Async<Result<HNItem, string>> = async {
    let! (statusCode, responseText) = Http.get (storyUrl.Item storyId)
    match statusCode with
    | 200 ->
        let story = Decode.fromString hnItemDecoder responseText
        return story
    | code -> return Error (sprintf "Error! Status code: %d" code)
}

let getStory: (bigint -> Async<Result<HNItem, string>>) =
    cached (Some expirationSpan) getStoryInternal
