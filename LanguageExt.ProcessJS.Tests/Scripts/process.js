// LanguageExt.Process.js
// The MIT License (MIT)
// Copyright (c) 2014-2015 Paul Louth
// https://github.com/louthy/language-ext/blob/master/LICENSE.md

var Process = (function () {

    var withContext = function (ctx, f) {
        var savedContext = context;

        if (!ctx) {
            ctx = {
                self:       User,
                sender:     NoSender,
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

    var actor      = { "/": { children: {} } };
    var ignore     = function () { };
    var id         = function (id) { return id };
    var publish    = null;
    var inboxStart = function (pid, parent, setup, inbox, stateless) {

        ctx = {
            self:       pid,
            parent:     parent,
            sender:     NoSender,
            currentMsg: null,
            currentReq: null
        };

        var process = withContext(ctx, function () {
            return {
                pid:        pid,
                state:      setup(),
                setup:      setup,
                inbox:      inbox,
                stateless:  stateless,
                children:   {},
                subs: {}
            }
        });
        actor[parent].children[pid] = process;
        return process;
    }
    var Root        = "/root-js";
    var System      = "/root-js/system";
    var DeadLetters = "/root-js/system/dead-letters";
    var Errors      = "/root-js/system/errors";
    var User        = "/root-js/user";
    var NoSender    = "/no-sender";
    var root        = actor[Root]        = inboxStart(Root, "/", ignore, function (msg) { publish(msg); }, true);
    var system      = actor[System]      = inboxStart(System, Root, ignore, function (msg) { publish(msg); }, true);
    var user        = actor[User]        = inboxStart(User, Root, ignore, function (msg) { publish(msg); }, true);
    var noSender    = actor[NoSender]    = inboxStart(NoSender, "/", ignore, function (msg) { publish(msg); }, true);
    var deadLetters = actor[DeadLetters] = inboxStart(DeadLetters, System, ignore, function (msg) { publish(msg); }, true);
    var errors      = actor[Errors]      = inboxStart(Errors, System, ignore, function (msg) { publish(msg); }, true);
    var subscribeId = 1;
    var context     = null;

    var inloop = function() {
        return context != null;
    }

    var isLocal = function (pid) {
        return pid.indexOf(Root) == 0;
    }

    var getSender = function (sender) {
        return sender || (inloop() ? context.self : NoSender);
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
            actor[pid] = inboxStart(pid, context.self, setup, inbox, stateless);
            return pid;
        });
    }

    var tell = function (pid, msg, sender) {
        var ctx = {
            isAsk: false,
            self: pid,
            sender: getSender(sender),
            currentMsg: msg,
            currentReq: null
        };
        window.postMessage({ processjs: "tell", pid: pid, msg: msg, ctx: ctx }, window.location.origin);
    }

    var tellDelay = function (pid, msg, delay) {
        var self = context.self;
        return setTimeout(function () { tell(pid, msg, self); }, delay);
    }

    var ask = function (pid, msg) {
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
                throw new "Process doesn't exist " + pid;
            }
            else {
                throw "'ask' is only available for intra-JS process calls.";
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
                    p.state = p.inbox(msg);
                }
                else {
                    p.state = p.inbox(p.state, event.data.msg);
                }
                return context.reply;
            });
        }
    }

    var reply = function (msg) {
        context.reply = msg;
    }

    var subscribeAsync = function (pid) {
        var p = actor[pid];
        if (!p || !p.inbox) {
            if (isLocal(pid)) {
                throw new "Process doesn't exist " + pid;
            }
            else {
                throw "'subscribe' is currently only available for intra-JS process calls.";
            }
        }

        var id = subscribeId;
        var onNext = function (msg) { };
        var onComplete = function () { };

        var subctx = {
            pid:    function()   { return pid; },
            id:     function ()  { return id; },
            forall: function (f) { onNext = f; return this; },
            done:   function (f) { onComplete = f; return this; }
        };

        p.subs[subscribeId] = {
            next: function (msg) { onNext(msg); },
            done: function ()    { onComplete(); },
            ctx: subctx
        };

        subscribeId++;
        return subctx;
    }

    var subscribeSync = function (pid) {
        var self = context.self;
        return subscribeAsync(pid).forall(function (msg) {
            tell(self, msg, pid);
        });
    }

    var subscribe = function (pid) {
        if (inloop()) 
            subscribeSync(pid)
        else
            subscribeAsync(pid);
    }

    var unsubscribe = function (ctx) {
        if (!ctx || !ctx.id || !ctx.id() || !ctx.pid()) return;
        var p = actor[ctx.pid()];
        if (!p || !p.inbox) return;
        var sub = p.subs[ctx.id()];
        if (sub && sub.done && typeof sub.done === "function") {
            try {
                sub.done();
            }
            catch (e) {
                // TODO: Report error
            }
        }
        delete p.subs[ctx.id()];
    }

    publish = function (msg) {
        if (inloop()) {
            window.postMessage(
                { pid: context.self, msg: msg, processjs: "pub" },
                window.location.origin
            );
        }
        else {
            throw "'publish' can only be called from within a process";
        }
    }

    var kill = function (pid) {
        if (arguments.length == 0) {
            if (inloop()) {
                pid = context.pid;
            }
            else {
                throw "'kill' can only be called without arguments from within a process";
            }
        }

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
        delete actor[pid];
    }

    var deadLetter = function (to, msg, sender) {
        tell(DeadLetters, { to: to, msg: msg, sender: getSender(sender) });
    }

    var error = function (e, to, msg, sender) {
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
                    p.state = p.inbox(data.msg);
                }
                else {
                    p.state = p.inbox(p.state, data.msg);
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

        for(var x in p.subs) {
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
            !event.data ||
            !event.data.processjs, 
            !event.data.pid ||
            !event.data.msg) {
            return;
        }
        switch( event.data.processjs )
        {
            case "tell": receiveTell(event.data); break;
            case "pub":  receivePub(event.data); break;
        }
    }

    var processSystem = {
        connect:        null,
        ask:            ask,
        publish:        publish,
        reply:          reply,
        receive:        receive,
        kill:           kill,
        spawn:          spawn,
        subscribe:      subscribe,
        tell:           tell,
        tellDelay:      tellDelay,
        tellDeadLetter: deadLetter,
        tellError:      error,
        unsubscribe:    unsubscribe,
        Root:           Root,
        NoSender:       NoSender,
        System:         System,
        DeadLetters:    DeadLetters,
        Errors:         Errors,
        User:           User,
        isAsk:          function () { return context ? context.isAsk : false }
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
                { pid: data.To, msg: ctx.currentMsg, ctx: ctx, processjs: "tell" },
                window.location.origin
            );
        }
    }

    return processSystem;
})();

window.addEventListener("message", Process.receive, false);