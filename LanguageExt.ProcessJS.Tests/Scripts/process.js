// LanguageExt.Process.js
// The MIT License (MIT)
// Copyright (c) 2014-2015 Paul Louth
// https://github.com/louthy/language-ext/blob/master/LICENSE.md

var Process = (function () {

    var actor = {};
    var ignore = function () { };
    var id = function (id) { return id };

    var inboxStart = function (pid, setup, inbox, stateless) {
        return {
            pid: pid,
            state: setup(),
            setup: setup,
            inbox: inbox,
            stateless: stateless
        };
    }

    var Root = "/root-js";
    var User = "/root-js/user";
    var NoSender = "/no-sender";

    var root = actor[Root] = inboxStart(ignore, ignore);
    var user = actor[User] = inboxStart(ignore, ignore);
    var noSender = actor[NoSender] = inboxStart(ignore, ignore);

    var context = null;

    var withContext = function (ctx, f) {
        var savedContext = context;

        if (!ctx) {
            ctx = {
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
            // TODO: Log error
        }
        context = savedContext;
        return res;
    }

    var spawn = function (name, setup, inbox) {

        var stateless = false;
        if (arguments.length == 2)
        {
            inbox = setup;
            setup = function () { return {} };
            stateless = true;
        }
        else
        {
            if( typeof setup !== "function" )
            {
                var val = setup;
                setup = function () { return val };
            }
        }

        return withContext(null, function () {
            var pid = context.self + "/" + name;
            actor[pid] = inboxStart(pid, setup, inbox, stateless);
            return pid;
        });
    }

    var tell = function (pid, msg, sender) {
        var ctx = {
            self: pid,
            sender: sender || (context ? context.self : NoSender),
            currentMsg: msg,
            currentReq: null
        };
        window.postMessage({ pid: pid, msg: msg, ctx: ctx, isAsk: false }, window.location.origin);
    }

    var tellDelay = function (pid, msg, delay) {
        var self = context.self;
        return setTimeout(function () { tell(pid, msg, self); }, delay);
    }

    var ask = function (pid, msg) {
        var ctx = {
            self: pid,
            sender: sender || (context ? context.self : NoSender),
            currentMsg: msg,
            currentReq: null
        };
        window.postMessage({ pid: pid, msg: msg, ctx: ctx, isAsk: true }, window.location.origin);
    }

    var receive = function (event) {
        if (event.origin !== window.location.origin ||
            !event.data ||
            !event.data.pid ||
            !event.data.ctx ||
            !event.data.msg) {
            return;
        }
        var p = actor[event.data.pid];
        if (!p || !p.inbox) {


            var msg = typeof event.data.msg === "string"
                ? event.data.msg
                : JSON.stringify(event.data.msg);

            if (event.isAsk) {
                // TODO: Not sure how to get the reply just yet.
                $.connection.processHub.server.ask(event.data.pid, msg, event.data.ctx.sender);
            }
            else {
                $.connection.processHub.server.tell(event.data.pid, msg, event.data.ctx.sender);
            }
            return;
        }

        try {
            withContext(event.data.ctx, function () {
                if (p.stateless) {
                    p.state = p.inbox(event.data.msg);
                }
                else {
                    p.state = p.inbox(p.state, event.data.msg);
                }
            });
        }
        catch (e) {
            // TODO: Error letter
            // TODO: Dead letter
            p.state = p.setup();
        }
    }

    var processSystem = {
        connect: null,
        spawn: spawn,
        tell: tell,
        tellDelay: tellDelay,
        ask: ask,
        receive: receive,
        Root: Root,
        NoSender: NoSender,
        User: User
    };

    processSystem.connect = function () {
        var proxy = $.connection.processHub;
        proxy.client.onMessage = function (data) {

            var ctx = {
                self: data.To,
                sender: data.Sender,
                replyTo: data.ReplyTo,
                currentMsg: JSON.parse(data.Content),
                currentReq: data.RequestId
            };

            window.postMessage(
                { pid: data.To, msg: ctx.currentMsg, ctx: ctx },
                window.location.origin
            );
        }
    }

    return processSystem;
})();

window.addEventListener("message", Process.receive, false);