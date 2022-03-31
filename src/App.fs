module App

open Elmish
open Elmish.React
open Feliz
open Deferred
open Types
open StoryService
Fable.Core.JsInterop.importAll "./styles/main.css"
// Fable.Core.JsInterop.importAll "@fortawesome/fontawesome-free/js/regular.js"


type State = 
    { StoryIds: DeferredResult<bigint list>
      StoryMap: Map<bigint, DeferredResult<HNItem>> }

type Msg = 
    | LoadStoryIds of DeferredResult<bigint list>
    | LoadStory of bigint * DeferredResult<HNItem>

let init () =
    { StoryIds = InProgress; StoryMap = Map.empty }, Cmd.ofMsg (LoadStoryIds InProgress)

let update (msg: Msg) (state: State): (State * Cmd<Msg>) =
    match msg with
    | LoadStoryIds NotStartedYet
    | LoadStoryIds InProgress->
        let cmdAsync = async {
            let! storyIdsResult = getTopStories ()
            return (storyIdsResult |> Resolved |> LoadStoryIds)
        }

        { state with StoryIds = InProgress }, Cmd.OfAsync.result cmdAsync
    | LoadStoryIds (Resolved (Ok ids) as resolved) ->
        let storyTuples: (bigint * DeferredResult<HNItem>) list =
            ids
            |> List.map (fun id -> id, InProgress)
        let cmd =
            storyTuples
            |> List.map (fun tuple -> Cmd.ofMsg (LoadStory tuple))
            |> Cmd.batch
        
        { state with StoryIds = resolved; StoryMap = (Map.ofList storyTuples) }, cmd
    | LoadStoryIds (Resolved (Error msg) as resolved) ->
        { state with StoryIds = resolved }, Cmd.none
    | LoadStory (storyId, deferred) ->
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
                    return ((storyId, Resolved storyResult) |> LoadStory)
                }
                Cmd.OfAsync.result cmdAsync
            | Resolved _ -> Cmd.none

        { state with StoryMap = newMap }, cmd

let loadingWidget =
    Html.button [
        prop.className "btn btn-circle btn-ghost loading place-self-auto"
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
        prop.className "flex flex-col gap-3 place-items-center"
        prop.children [
            Html.ul [
                prop.classes [ "menu bg-base-100 menu-horizontal rounded-box place-self-auto" ]
                prop.children [
                    Html.li [ Html.a [ Html.text "Item 1" ] ]
                    Html.li [ Html.a [ Html.text "Item 2" ] ]
                    Html.li [ Html.a [ Html.text "Item 3" ] ]
                ]
            ]

            Html.div [
                prop.className "tabs tabs-boxed place-self-auto"
                prop.children [
                    Html.a [
                        prop.className "tab tab-lg"
                        prop.text "Tab 1"
                    ]
                    Html.a [
                        prop.className "tab tab-lg"
                        prop.text "Tab 2"
                    ]
                    Html.a [
                        prop.className "tab tab-lg"
                        prop.text "Tab 3"
                    ]
                ]
            ]

            Html.button [
                prop.className "btn btn-primary"
                prop.text "RELOAD"
                prop.onClick (fun _ -> InProgress |> LoadStoryIds |> dispatch)
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
