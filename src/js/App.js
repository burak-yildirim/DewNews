import * as main from "../styles/main.css";
import { Union, Record } from "./.fable/fable-library.3.2.9/Types.js";
import { union_type, record_type, list_type, int32_type } from "./.fable/fable-library.3.2.9/Reflection.js";
import { Deferred$1, Deferred$1$reflection } from "./Deferred.js";
import { Cmd_none, Cmd_OfFunc_result } from "./.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { createElement } from "react";
import { Interop_reactApi } from "./.fable/Feliz.1.58.1/Interop.fs.js";
import { singleton, append, delay, toList } from "./.fable/fable-library.3.2.9/Seq.js";
import { join } from "./.fable/fable-library.3.2.9/String.js";
import { singleton as singleton_1 } from "./.fable/fable-library.3.2.9/List.js";
import { equals } from "./.fable/fable-library.3.2.9/Util.js";
import { ProgramModule_mkProgram, ProgramModule_run } from "./.fable/Fable.Elmish.3.1.0/program.fs.js";
import { Program_withReactSynchronous } from "./.fable/Fable.Elmish.React.3.0.1/react.fs.js";


export class State extends Record {
    constructor(StoryIds) {
        super();
        this.StoryIds = StoryIds;
    }
}

export function State$reflection() {
    return record_type("App.State", [], State, () => [["StoryIds", Deferred$1$reflection(list_type(int32_type))]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["LoadStoryIds"];
    }
}

export function Msg$reflection() {
    return union_type("App.Msg", [], Msg, () => [[]]);
}

export function init() {
    return [new State(new Deferred$1(1)), Cmd_OfFunc_result(new Msg(0))];
}

export function update(msg, state) {
    return [state, Cmd_none()];
}

export const loadingWidget = createElement("button", {
    className: "btn btn-circle btn-ghost loading place-self-auto",
});

export function render(state, dispatch) {
    return createElement("div", {
        className: "flex flex-col gap-3 place-items-center",
        children: Interop_reactApi.Children.toArray(Array.from(toList(delay(() => {
            let children_2, children_6, children_10;
            return append(singleton(createElement("ul", {
                className: join(" ", ["menu bg-base-100 menu-horizontal rounded-box place-self-auto"]),
                children: Interop_reactApi.Children.toArray([(children_2 = singleton_1(createElement("a", {
                    children: Interop_reactApi.Children.toArray(["Item 1"]),
                })), createElement("li", {
                    children: Interop_reactApi.Children.toArray(Array.from(children_2)),
                })), (children_6 = singleton_1(createElement("a", {
                    children: Interop_reactApi.Children.toArray(["Item 2"]),
                })), createElement("li", {
                    children: Interop_reactApi.Children.toArray(Array.from(children_6)),
                })), (children_10 = singleton_1(createElement("a", {
                    children: Interop_reactApi.Children.toArray(["Item 3"]),
                })), createElement("li", {
                    children: Interop_reactApi.Children.toArray(Array.from(children_10)),
                }))]),
            })), delay(() => append(singleton(createElement("div", {
                className: "tabs tabs-boxed place-self-auto",
                children: Interop_reactApi.Children.toArray([createElement("a", {
                    className: "tab tab-lg",
                    children: "Tab 1",
                }), createElement("a", {
                    className: "tab tab-lg",
                    children: "Tab 2",
                }), createElement("a", {
                    className: "tab tab-lg",
                    children: "Tab 3",
                })]),
            })), delay(() => (equals(state.StoryIds, new Deferred$1(1)) ? singleton(loadingWidget) : singleton(loadingWidget))))));
        })))),
    });
}

ProgramModule_run(Program_withReactSynchronous("elmish-app", ProgramModule_mkProgram(init, (msg, state) => update(msg, state), (state_1, dispatch) => render(state_1, dispatch))));

