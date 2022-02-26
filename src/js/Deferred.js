import { Union } from "./.fable/fable-library.3.2.9/Types.js";
import { union_type } from "./.fable/fable-library.3.2.9/Reflection.js";

export class Deferred$1 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["NotStartedYet", "InProgress", "Resolved"];
    }
}

export function Deferred$1$reflection(gen0) {
    return union_type("Deferred.Deferred`1", [gen0], Deferred$1, () => [[], [], [["Item", gen0]]]);
}

export function Deferred_isResolved(_arg1) {
    if (_arg1.tag === 2) {
        return true;
    }
    else {
        return false;
    }
}

export function Deferred_isInProgress(_arg1) {
    if (_arg1.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

export function Deferred_map(transform, deferred) {
    switch (deferred.tag) {
        case 1: {
            return new Deferred$1(1);
        }
        case 2: {
            const t = deferred.fields[0];
            return new Deferred$1(2, transform(t));
        }
        default: {
            return new Deferred$1(0);
        }
    }
}

export function Deferred_hasExists(predicate, _arg1) {
    if (_arg1.tag === 2) {
        const t = _arg1.fields[0];
        return predicate(t);
    }
    else {
        return false;
    }
}

export function Deferred_bind(transform, deferred) {
    switch (deferred.tag) {
        case 1: {
            return new Deferred$1(1);
        }
        case 2: {
            const t = deferred.fields[0];
            return transform(t);
        }
        default: {
            return new Deferred$1(0);
        }
    }
}

