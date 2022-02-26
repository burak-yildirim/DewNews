module App

open Elmish
open Elmish.React
open Feliz
open Feliz.DaisyUI
open Deferred
open Types
Fable.Core.JsInterop.importAll "./styles/main.css"


type State = 
    { StoryIds: Deferred<Result<bigint list, string>>
      StoriesMap: Map<bigint, Deferred<Result<HNItem, string>>> }

type Msg = 
    | LoadStoryIds of Deferred<Result<bigint list, string>>
    | SetStory of Map<bigint, Deferred<Result<HNItem, string>>>

let init () =
    { StoryIds = InProgress; StoriesMap = Map.empty }, Cmd.ofMsg (LoadStoryIds InProgress)

let update (msg: Msg) (state: State): (State * Cmd<Msg>) =
    match msg with
    | LoadStoryIds NotStartedYet
    | LoadStoryIds InProgress-> state, Cmd.none
    | LoadStoryIds (Resolved (Ok ids)) -> state, Cmd.none
    | LoadStoryIds (Resolved (Error errMsg)) -> state, Cmd.none
    | SetStory storyMap -> state, Cmd.none

let loadingWidget =
    Html.button [
        prop.className "btn btn-circle btn-ghost loading place-self-auto"
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

            if state.StoryIds = InProgress
            then loadingWidget
            else
                loadingWidget
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
