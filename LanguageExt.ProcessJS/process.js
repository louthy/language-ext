﻿// LanguageExt.Process.js
// The MIT License (MIT)
// Copyright (c) 2014-2015 Paul Louth
// https://github.com/louthy/language-ext/blob/master/LICENSE.md

var unit = "(unit)";

// set the origin if it isn't supported by the browser
if (!window.location.origin) {
    window.location.origin = window.location.protocol + "//"
        + window.location.hostname
        + (window.location.port ? ':' + window.location.port : '');
}

var failwith = function (err) {
    console.error(err);
    throw err;
}

var Process = (function () {

    var withContext = function (ctx, f) {
        var savedContext = context;

        if (!ctx) {
            ctx = {
                parent: User,
                self: User,
                sender: NoSender,
                currentMsg: null,
                currentReq: null
            };
        }
        context = ctx;
        var res = null;
        try {
            res = f(context);
        }
        catch (e) {
            error(e + "(" + context.self + ")", ctx.self, ctx.currentMsg, ctx.sender);
        }
        context = savedContext;
        return res;
    }

    var actor = { "/": { children: {} } };
    var ignore = function () { };
    var id = function (id) { return id };
    var publish = null;
    var inboxStart = function (pid, parent, setup, inbox, stateless, shutdown) {

        ctx = {
            self: pid,
            parent: parent,
            sender: NoSender,
            currentMsg: null,
            currentReq: null
        };

        var process = {
            pid: pid,
            state: null,
            setup: setup,
            inbox: inbox,
            stateless: stateless,
            children: {},
            subs: {},
            obs: {},
            shutdown: shutdown
        };

        actor[parent].children[pid] = process;
        actor[pid] = process;

        process.state = withContext(ctx, function () {
            return setup();
        });

        return process;
    }
    var Root = "/root-js";
    var System = "/root-js/system";
    var DeadLetters = "/root-js/system/dead-letters";
    var Errors = "/root-js/system/errors";
    var User = "/root-js/user";
    var NoSender = "/no-sender";
    var root = inboxStart(Root, "/", ignore, function (msg) { publish(msg); }, true);
    var system = inboxStart(System, Root, ignore, function (msg) { publish(msg); }, true);
    var user = inboxStart(User, Root, ignore, function (msg) { publish(msg); }, true);
    var noSender = inboxStart(NoSender, "/", ignore, function (msg) { publish(msg); }, true);
    var deadLetters = inboxStart(DeadLetters, System, ignore, function (msg) { publish(msg); }, true);
    var errors = inboxStart(Errors, System, ignore, function (msg) { publish(msg); }, true);
    var subscribeId = 1;
    var context = null;

    var inloop = function () {
        return context != null;
    }

    var isLocal = function (pid) {
        if (typeof pid === "undefined") failwith("isLocal: 'pid' not defined");
        return pid.indexOf(Root) == 0;
    }

    var getSender = function (sender) {
        return sender || (inloop() ? context.self : NoSender);
    }

    var spawn = function (name, setup, inbox, shutdown) {
        if (typeof name === "undefined") failwith("spawn: 'name' not defined");
        var stateless = false;
        if (arguments.length == 2) {
            inbox = setup;
            setup = function () { return {} };
            stateless = true;
        }
        else {
            if (typeof setup !== "function") {
                var val = setup;
                setup = function () { return val };
            }
        }

        return withContext(context, function () {
            var pid = context.self + "/" + name;
            inboxStart(pid, context.self, setup, inbox, stateless, shutdown);
            return pid;
        });
    }

    var tell = function (pid, msg, sender) {
        if (typeof pid === "undefined") failwith("tell: 'pid' not defined");
        if (typeof msg === "undefined") failwith("tell: 'msg' not defined");

        var ctx = {
            isAsk: false,
            self: pid,
            sender: getSender(sender),
            currentMsg: msg,
            currentReq: null
        };
        postMessage(
            JSON.stringify({ processjs: "tell", pid: pid, msg: msg, ctx: ctx }),
            window.location.origin
        );
    }

    var tellDelay = function (pid, msg, delay) {
        if (typeof pid === "undefined") failwith("tellDelay: 'pid' not defined");
        if (typeof msg === "undefined") failwith("tellDelay: 'msg' not defined");
        if (typeof delay === "undefined") failwith("tellDelay: 'delay' not defined");
        var self = context.self;
        return setTimeout(function () { tell(pid, msg, self); }, delay);
    }

    var ask = function (pid, msg) {
        if (typeof pid === "undefined") failwith("ask: 'pid' not defined");
        if (typeof msg === "undefined") failwith("ask: 'msg' not defined");
        var ctx = {
            isAsk: true,
            self: pid,
            sender: getSender(),
            currentMsg: msg,
            currentReq: null
        };
        var p = actor[pid];
        if (!p || !p.inbox) {
            if (isLocal(pid)) {
                failwith("Process doesn't exist " + pid);
            }
            else {
                failwith("'ask' is only available for intra-JS process calls.");
            }
        }
        else {
            // TODO: This isn't ideal, you could 'jump ahead' of the queue where an ask arrives 
            //       before a postMessaged 'tell'.  In reality this may not be a huge issue because
            //       posting to an actor is always supposed to be asynchronous, so you don't know 
            //       what order a message arrives at the inbox (and therefore shouldn't rely on 
            //       the send order).  Also, this allows for blocking 'ask', which is difficult to
            //       achieve by other means.
            return withContext(ctx, function () {
                context.reply = null;
                if (p.stateless) {
                    var state = p.inbox(msg);
                    if (typeof state !== "undefined") p.state = state;
                }
                else {
                    var state = p.inbox(p.state, msg);
                    if (typeof state !== "undefined") p.state = state;
                }
                return context.reply;
            });
        }
    }

    var reply = function (msg) {
        if (typeof msg === "undefined") failwith("reply: 'msg' not defined");
        context.reply = msg;
    }

    var subscribeAsync = function (pid) {
        if (typeof pid === "undefined") failwith("subscribeAsync: 'pid' not defined");
        var p = actor[pid];
        if (!p || !p.inbox) {
            if (isLocal(pid)) {
                failwith("Process doesn't exist " + pid);
            }
            else {
                failwith("'subscribe' is currently only available for intra-JS process calls.");
            }
        }

        var id = subscribeId;
        var onNext = function (msg) { };
        var onComplete = function () { };

        p.subs[id] = {
            next: function (msg) { onNext(msg); },
            done: function () { onComplete(); }
        };

        subscribeId++;

        return {
            unsubscribe: function () {
                onComplete();
                delete p.subs[id];
            },
            forall: function (f) { onNext = f; return this; },
            done: function (f) { onComplete = f; return this; }
        };
    }

    var subscribeSync = function (pid) {
        if (typeof pid === "undefined") failwith("subscribeSync: 'pid' not defined");
        var self = context.self;
        if (isLocal(pid)) {
            return subscribeAsync(pid).forall(function (msg) {
                tell(self, msg, pid);
            });
        }
        else {
            var id = subscribeId;
            var ctx = {
                unsubscribe: function () {
                    $.connection.processHub.server.unsubscribe(pid, self);
                    delete actor[self].obs[id];
                },
                next: function () { },
                done: function () { }
            }
            actor[self].obs[id] = ctx;
            subscribeId++;
            $.connection.processHub.server.subscribe(pid, self);
            return ctx;
        }
    }

    var subscribe = function (pid) {
        if (typeof pid === "undefined") failwith("subscribe: 'pid' not defined");
        return inloop()
            ? subscribeSync(pid)
            : subscribeAsync(pid);
    }

    var unsubscribe = function (ctx) {
        if (typeof ctx === "undefined") failwith("unsubscribe: 'ctx' not defined");
        if (!ctx || ctx.unsubscribe) return;
        ctx.unsubscribe();
    }

    publish = function (msg) {
        if (typeof msg === "undefined") failwith("publish: 'msg' not defined");
        if (inloop()) {
            postMessage(
                JSON.stringify({ pid: context.self, msg: msg, processjs: "pub" }),
                window.location.origin
            );
        }
        else {
            failwith("'publish' can only be called from within a process");
        }
    }

    var kill = function (pid) {
        if (typeof pid === "undefined") failwith("kill: 'pid' not defined");
        if (arguments.length == 0) {
            if (inloop()) {
                pid = context.pid;
            }
            else {
                failwith("'kill' can only be called without arguments from within a process");
            }
        }

        var p = actor[pid];
        if ("undefined" === typeof p) {
            return;
        }
        var children = p.children;
        for (var i = 0; i < children.length; i++) {
            kill(children[i]);
        }

        if (typeof p.shutdown === "function") {
            p.shutdown(p.state);
        }

        for (var x in p.obs) {
            p.obs[x].unsubscribe();
        }
        p.obs = {};

        for (var x in p.subs) {
            var sub = p.subs[x];
            if (typeof sub.done === "function") {
                try {
                    sub.done();
                }
                catch (e) {
                    // Ignore.  We just want to stop other 
                    // subscribers being hurt by one bad egg.
                    error(e, pid + "/sub/done", null, null);
                }
            }
        }
        p.subs = {};

        delete actor[extractParent(pid)].children[pid];
        delete actor[pid];
    }

    var deadLetter = function (to, msg, sender) {
        tell(DeadLetters, { to: to, msg: msg, sender: getSender(sender) });
    }

    var error = function (e, to, msg, sender) {
        console.error(e);
        tell(Errors, { error: e, to: to, msg: msg, sender: getSender(sender) });
    }

    var receiveTell = function (data) {
        var p = actor[data.pid];
        if (!p || !p.inbox) {
            if (isLocal(data.pid)) {
                deadLetter(data.pid, data.msg, data.sender);
            }
            else {
                var msg = typeof data.msg === "string"
                    ? data.msg
                    : JSON.stringify(data.msg);
                $.connection.processHub.server.tell(data.pid, msg, data.ctx.sender);
            }
            return;
        }

        try {
            withContext(data.ctx, function () {
                if (p.stateless) {
                    var state = p.inbox(data.msg);
                    if (typeof state !== "undefined") p.state = state;
                }
                else {
                    var state = p.inbox(p.state, data.msg);
                    if (typeof state !== "undefined") p.state = state;
                }
            });
        }
        catch (e) {
            error(e, data.pid, data.msg, data.sender);
            deadLetter(data.pid, data.msg, data.sender);
            p.state = p.setup();
        }
    }

    var receivePub = function (data) {
        var p = actor[data.pid];
        if (!p || !p.subs) {
            return;
        }

        for (var x in p.subs) {
            var sub = p.subs[x];
            if (typeof sub.next === "function") {
                try {
                    sub.next(data.msg);
                }
                catch (e) {
                    // Ignore.  We just want to stop other subscribers
                    // being hurt by one bad egg.
                    error(e, data.pid + "/pub", data.msg, null);
                }
            }
        }
    }

    var receive = function (event) {
        if (event.origin !== window.location.origin ||
            typeof event.data !== "string") {
            return;
        }
        var data = JSON.parse(event.data);

        if (!data.processjs,
            !data.pid ||
            !data.msg) {
            return;
        }
        switch (data.processjs) {
            case "tell": receiveTell(data); break;
            case "pub": receivePub(data); break;
        }
    }

    var connect = function () {
        var proxy = $.connection.processHub;
        proxy.client.onMessage = function (data) {
            var ctx = {
                self: data.To,
                sender: data.Sender,
                replyTo: data.ReplyTo,
                currentMsg: JSON.parse(data.Content),
                currentReq: data.RequestId
            };
            postMessage(
                JSON.stringify({ pid: data.To, msg: ctx.currentMsg, ctx: ctx, processjs: "tell" }),
                window.location.origin
            );
        }
        return {
            done: function (f) {
                $.connection.hub.start().done(f);
            }
        };
    }

    var isAsk = function () {
        return context
            ? context.isAsk
            : false
    }

    var isTell = function () {
        return !isAsk
    }

    var formatItem = function (msg) {
        return "<div class='process-log-msg-row'>" +
                "<div class='process-log-row process-log-row" + msg.TypeDisplay + "'>" +
                "<div class='log-time'>" + msg.DateDisplay + "</div>" +
                "<div class='log-type'>" + msg.TypeDisplay + "</div>" +
                (msg.Message == null
                    ? ""
                    : "<div class='log-msg'>" + msg.Message + "</div>") +
                "</div>" +
                (msg.Exception == null || msg.Exception == ""
                    ? ""
                    : "<div class='process-log-row testbed-log-rowError'><div id='log-ex-msg'>" + msg.Exception + "</div></div>") +
                "</div>";
    };

    var log = {
        tell: function (type, msg) {
            if (typeof type === "undefined") failwith("Log.tell: 'type' not defined");
            if (typeof msg === "undefined") failwith("Log.tell: 'msg' not defined");
            tell("/root/user/process-log", {
                Type: type,
                Message: msg
            });
        },
        tellInfo: function (msg) { this.tell(1, msg); },
        tellWarn: function (msg) { this.tell(2, msg); },
        tellError: function (msg) { this.tell(12, msg); },
        tellDebug: function (msg) { this.tell(16, msg); },
        view: function (id, viewSize) {
            if (!id) failwith("Log.view: 'id' not defined");
            return Process.spawn("process-log",
                    function () {
                        Process.subscribe("/root/user/process-log");
                        return [];
                    },
                    function (state, msg) {
                        state.unshift(msg);
                        $("#" + id).prepend(formatItem(msg));
                        if (state.length > (viewSize || 50)) {
                            state.pop();
                            $('.process-log-msg-row:last').remove();
                        }
                        return state;
                    }
                );
        }
    }

    var extractParent = function (pid) {
        var i = pid.lastIndexOf('/');
        if (i == -1) return User;
        return pid.substr(0, i);
    }

    return {
        ask: ask,
        connect: connect,
        isAsk: isAsk,
        isTell: isTell,
        kill: kill,
        publish: publish,
        reply: reply,
        receive: receive,
        spawn: spawn,
        subscribe: subscribe,
        tell: tell,
        tellDelay: tellDelay,
        tellDeadLetter: deadLetter,
        tellError: error,
        unsubscribe: unsubscribe,
        DeadLetters: DeadLetters,
        Errors: Errors,
        NoSender: NoSender,
        Log: log,
        Root: Root,
        System: System,
        User: User,
        Parent: function () { return context ? extractParent(context.self) : User },
        Self: function () { return context ? context.self : User; },
        Sender: function () { return context ? context.sender : NoSender; },
    };
})();

window.addEventListener("message", Process.receive, false);

// Knockout.js Process view 
if (typeof ko !== "undefined" && typeof ko.observable !== "undefined") {

    Process.computed = function (fn) {
        return { tag: "Computed", fn: fn };
    };

    Process.clone = function (obj) {
        if (null == obj || "object" != typeof obj) return obj;
        var copy = obj.constructor();
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = obj[attr];
        }
        return copy;
    }

    Process.cloneMap = function (obj, fn) {
        if (null == obj || "object" != typeof obj) return obj;
        var copy = obj.constructor();
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = fn(obj[attr]);
        }
        return copy;
    }

    Process.spawnView = function (name, containerId, templateId, setup, inbox, shutdown) {

        if (typeof setup !== "function") {
            var val = setup;
            setup = function () { return val };
        }

        return Process.spawn(name,
            // Setup
            function () {

                var view = function (state) {
                    this.render = function (el) {
                        ko.cleanNode($("#" + containerId)[0]);
                        ko.applyBindings(state, el)
                    }
                };

                var state = setup();

                var makeObs = function (value) {
                    if (value != null && typeof value !== "undefined" && typeof value !== "function") {
                        if (value != null && typeof value.tag === "string" && value.tag == "Computed") {
                            return ko.computed(state[key].fn, state);
                        }
                        if (Object.prototype.toString.call(value) === Object.prototype.toString.call([])) {
                            return ko.observableArray(value);
                        }
                        else {
                            return ko.observable(value);
                        }
                    }
                    else {
                        return value;
                    }
                }

                state.with = function (patch) {
                    var clone = Process.clone(this);
                    for (var key in patch) {
                        if (patch.hasOwnProperty(key)) {
                            var patchValue = patch[key];
                            if (clone.hasOwnProperty(key)) {
                                var clonedValue = clone[key];
                                if (ko.isObservable(clonedValue)) {
                                    clonedValue(patchValue);
                                }
                                else {
                                    clone[key] = makeObs(patchValue);
                                }
                            }
                            else {
                                clone[key] = makeObs(patchValue);
                            }
                        }
                    }
                    return clone;
                }

                if (typeof state === "object") {
                    for (var key in state) {
                        state[key] = makeObs(state[key]);
                    }
                }

                var refresh = function (s) {
                    var el = document.createElement("div");
                    el.innerHTML = $("#" + templateId).html();
                    (new view(s)).render(el);
                    $("#" + containerId).empty();
                    $("#" + containerId).append(el);
                };

                refresh(state);

                return {
                    state: state,
                    refresh: refresh
                };
            },
            // Inbox
            function (state, msg) {
                var newState = inbox(state.state, msg);
                if (newState != state.state) {
                    state.refresh(newState);
                    return {
                        state: newState,
                        refresh: state.refresh
                    }
                }
                else {
                    return state;
                };
            },
            // Shutdown
            function (state) {
                ko.cleanNode($("#" + containerId)[0]);
                $("#" + containerId).empty();
                if ("function" === typeof shutdown) {
                    shutdown(state);
                }
            });
    }
}
