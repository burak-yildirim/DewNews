module App

open Elmish
open Elmish.React
open Feliz
open Deferred
open Types
open StoryService
open Microsoft.FSharp.Reflection
Fable.Core.JsInterop.importAll "./styles/main.css"
// Fable.Core.JsInterop.importAll "@fortawesome/fontawesome-free/js/regular.js"

type StoryKind = NewStories | TopStories | BestStories | AskStories | ShowStories | JobStories

type State = 
    { StoryIds: DeferredResult<bigint list>
      StoryMap: Map<bigint, DeferredResult<HNItem>>
      StoryKind: StoryKind }

type Msg = 
    | SetStoryIds of DeferredResult<bigint list>
    | SetStory of bigint * DeferredResult<HNItem>
    | SetStoryKind of StoryKind

let init () =
    { StoryIds = InProgress; StoryMap = Map.empty; StoryKind = NewStories }, Cmd.ofMsg (SetStoryIds InProgress)

let getStoryIds = function
    | NewStories -> getNewStoryIds ()
    | TopStories -> getTopStoryIds ()
    | BestStories -> getBestStoryIds ()
    | AskStories -> getAskStoryIds ()
    | ShowStories -> getShowStoryIds ()
    | JobStories -> getJobStoryIds ()

let update (msg: Msg) (state: State): (State * Cmd<Msg>) =
    match msg with
    | SetStoryIds NotStartedYet
    | SetStoryIds InProgress->
        let cmdAsync = async {
            let! storyIdsResult = getStoryIds state.StoryKind
            return (storyIdsResult |> Resolved |> SetStoryIds)
        }

        { state with StoryIds = InProgress }, Cmd.OfAsync.result cmdAsync
    | SetStoryIds (Resolved (Ok ids) as resolved) ->
        let storyTuples: (bigint * DeferredResult<HNItem>) list =
            ids
            |> List.map (fun id -> id, InProgress)
        let cmd =
            storyTuples
            |> List.map (fun tuple -> Cmd.ofMsg (SetStory tuple))
            |> Cmd.batch
        
        { state with StoryIds = resolved; StoryMap = (Map.ofList storyTuples) }, cmd
    | SetStoryIds (Resolved (Error msg) as resolved) ->
        { state with StoryIds = resolved }, Cmd.none
    | SetStory (storyId, deferred) ->
        let newMap =
            Map.change
                storyId
                (function
                | Some _ -> Some deferred
                | _ -> None)
                state.StoryMap
        
        let cmd =
            match deferred with
            | NotStartedYet
            | InProgress ->
                let cmdAsync = async {
                    let! storyResult = getStory storyId
                    return ((storyId, Resolved storyResult) |> SetStory)
                }
                Cmd.OfAsync.result cmdAsync
            | Resolved _ -> Cmd.none

        { state with StoryMap = newMap }, cmd
    | SetStoryKind storyKind when state.StoryKind = storyKind -> state, Cmd.none
    | SetStoryKind storyKind ->
        { state with StoryKind = storyKind }, Cmd.ofMsg (SetStoryIds InProgress)

let loadingWidget =
    Html.button [
        prop.className "btn btn-circle btn-ghost loading place-self-auto"
    ]

let tabItemWidget (storyKind: StoryKind) (state: State) (dispatch: Msg -> unit) =
    let activeTabClass = if storyKind = state.StoryKind then "tab-active" else ""
    let tabName =
        storyKind.ToString().Replace("Stories", "")
    
    Html.a [
        prop.className (sprintf "tab tab-lg %s" activeTabClass)
        prop.text tabName
        prop.onClick (fun _ -> storyKind |> SetStoryKind |> dispatch)
    ]

let storyCardWidget (deferredStory: DeferredResult<HNItem>) =
    match deferredStory with
    | NotStartedYet
    | InProgress ->
        Html.div [
            prop.className "card bg-base-100 shadow-xl my-4 flex w-full place-content-center"
            prop.children [loadingWidget]
        ]
    | Resolved (Ok story) ->
        Html.a [
            prop.className "btn btn-ghost normal-case bg-base-900 shadow-lg my-4 flex w-full content-center"
            prop.href (getItemUrl story)
            prop.target "_blank"
            prop.rel "noopener noreferrer"
            prop.children [
                Html.div [
                    prop.className "flex-none w-14 h-14 grid grid-flow-col justify-around content-center mr-6"
                    prop.children [ 
                        Html.div [
                            prop.text (getItemScoreText story)
                        ]
                        Html.div [
                            prop.children [
                                Html.i [prop.className "fa-regular fa-chart-bar fa-xl"]
                            ]
                        ]
                    ]
                ]
                
                Html.div [
                    prop.className "flex-1 grid justify-start content-center"
                    prop.text (getItemTitleText story)
                ]
            ]
        ]
    | Resolved (Error msg) ->
        Html.div [
            prop.className "card bg-base-100 shadow-xl my-4 flex"
            prop.children [
                Html.div [
                    prop.className "flex-none justify-self-center self-center text-red-600"
                    prop.text msg
                ]
            ]
        ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    Html.div [
        prop.className "flex flex-col gap-3 justify-center items-center"
        prop.children [
            Html.button [
                prop.className "btn btn-primary"
                prop.text "RELOAD"
                prop.onClick (fun _ -> InProgress |> SetStoryIds |> dispatch)
            ]

            Html.div [
                prop.className "tabs tabs-boxed"
                prop.children [
                    for kindInfo in FSharpType.GetUnionCases typeof<StoryKind> ->
                        tabItemWidget (FSharpValue.MakeUnion(kindInfo, [||]) :?> StoryKind) state dispatch
                ]
            ]

            match state.StoryIds with
            | NotStartedYet
            | InProgress -> loadingWidget
            | Resolved storyIdsResult ->
                match storyIdsResult with
                | Error msg -> Html.text (sprintf "ERROR!! %s" msg)
                | Ok storyIds ->
                    Html.div [
                        prop.children [
                            for (id, defStory) in (Map.toSeq state.StoryMap) -> storyCardWidget defStory
                        ]
                    ]
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
