import { printf, toText } from "./.fable/fable-library.3.2.9/String.js";
import { singleton } from "./.fable/fable-library.3.2.9/AsyncBuilder.js";
import { Http_get } from "./.fable/Fable.SimpleHttp.3.1.0/Http.fs.js";
import { int, list, fromString } from "./.fable/Thoth.Json.7.0.0/Decode.fs.js";
import { uncurry } from "./.fable/fable-library.3.2.9/Util.js";
import { FSharpResult$2 } from "./.fable/fable-library.3.2.9/Choice.js";

export const getStoryUrl = (() => {
    const clo1 = toText(printf("https://hacker-news.firebaseio.com/v0/%sstories.json?print=pretty"));
    return (arg10) => clo1(arg10);
})();

const storyUrl = (() => {
    const New = getStoryUrl("new");
    const Top = getStoryUrl("top");
    const Best = getStoryUrl("best");
    const Ask = getStoryUrl("ask");
    const Show = getStoryUrl("show");
    return {
        Ask: Ask,
        Best: Best,
        Job: getStoryUrl("job"),
        New: New,
        Show: Show,
        Top: Top,
    };
})();

export function getTopStories() {
    return singleton.Delay(() => singleton.Bind(Http_get(storyUrl.Top), (_arg1) => {
        const statusCode = _arg1[0] | 0;
        const responseText = _arg1[1];
        if (statusCode === 200) {
            const storyIds = fromString((path, value) => list(uncurry(2, int), path, value), responseText);
            return singleton.Return(storyIds);
        }
        else {
            const code = statusCode | 0;
            return singleton.Return(new FSharpResult$2(1, toText(printf("Error! Status code: %d"))(code)));
        }
    }));
}

export function getStory(storyId) {
    return singleton.Delay(() => singleton.Return(new FSharpResult$2(0, 5)));
}

