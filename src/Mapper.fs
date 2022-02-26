module Mapper

open Thoth.Json
open Types

let hnItemTypeDecoder: Decoder<HNItemType> =
    Decode.string
    |> Decode.andThen 
        (function
        | "job" -> Decode.succeed HNJob
        | "story" -> Decode.succeed HNStory
        | "comment" -> Decode.succeed HNComment
        | "poll" -> Decode.succeed HNPoll
        | "pollopt" -> Decode.succeed HNPollopt
        | invalid -> Decode.fail (sprintf "Failed to decode %s. It is an invalid case for HNItemType" invalid))

let hnItemDecoder: Decoder<HNItem> =
    Decode.object (fun get ->
        { Id = get.Required.Field "id" Decode.bigint
          Deleted = get.Optional.Field "deleted" Decode.bool
          Type = get.Required.Field "type" hnItemTypeDecoder
          By = get.Required.Field "by" Decode.string
          Time = get.Required.Field "time" Decode.bigint
          Text = get.Required.Field "text" Decode.string
          Dead = get.Optional.Field "dead" Decode.bool
          Parent = get.Optional.Field "parent" Decode.bigint
          Poll = get.Optional.Field "poll" Decode.bigint
          Kids = get.Optional.Field "kids" (Decode.list Decode.bigint)
          Url = get.Optional.Field "url" Decode.string
          Score = get.Optional.Field "score" Decode.bigint
          Title = get.Optional.Field "title" Decode.string
          Parts = get.Optional.Field "parts" (Decode.list Decode.bigint)
          Descendants = get.Optional.Field "descendants" Decode.bigint })
