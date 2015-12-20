
## LanguageExt.Process

Another issue with working with C# is that no matter how much of this library you take on-board, you will always end up bumping into mutable state or side-effecting systems.  A way around that is to package up the side-effects into atoms of functional computation that are attached to the mutable state (in whatever form it may take).

The [Actor model](https://en.wikipedia.org/wiki/Actor_model) + functional message handling expressions are the perfect programming model for that.  Concurrent programming in C# isn't a huge amount of fun.  Yes the TPL gets you lots of stuff for free, but it doesn't magically protect you from race conditions or accessing shared state, and definitely doesn't help with accessing shared external state like a database.

Documention | Source
--------|-------------| ---------
[Process system overview](https://github.com/louthy/language-ext/wiki/Process-system) | [Prelude.cs](https://github.com/louthy/language-ext/edit/master/LanguageExt.Process/Prelude.cs)
[Message dispatch](https://github.com/louthy/language-ext/wiki/Process-system-message-dispatch) | 

### Getting started

Make sure you have the `LanguageExt.Process` DLL included in your project.  If you're using F# then you will also need to include `LanguageExt.Process.FSharp`.

In C# you should be `using static LanguageExt.Process`, if you're not using C# 6, just prefix all functions in the examples below with `Process.`

In F# you should:
```
open LanguageExt.ProcessFs
```

If you want to use it with Redis, include `LanguageExt.Process.Redis.dll`.  To connect to Redis use:

```C#
    // C#
    RedisCluster.register();
    Cluster.connect("redis", "my-redis-node", "localhost", "0", "my-redis-role");
```
```F#
    // F#
    RedisCluster.register()
    connect "redis" "my-redis-test" "localhost" "0" "my-redis-role"
```

* Argument 1 is fixed for Redis
* Argument 2 is your app's node name to make it uniquely addressable in the cluster
* Argument 3 is a comma separated list of Redis nodes to connect to
* Argument 4 is the Redis database number to connect to
* Argument 5 is the role for the node in the cluster - it's just a name that allows grouping of nodes for message dispatch

Note, neither of those lines are needed if you're doing in-app messaging only.

### Nuget

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'.  This is all that's needed to get started.
[LanguageExt.Process](https://www.nuget.org/packages/LanguageExt.Process) | 'Erlang like' actor system for in-app messaging and massive concurrency
[LanguageExt.Process.Redis](https://www.nuget.org/packages/LanguageExt.Process.Redis) | Cluster support for the `LangaugeExt.Process` system for cluster aware processes using Redis for queue and state persistence
[LanguageExt.Process.FSharp](https://www.nuget.org/packages/LanguageExt.Process.FSharp) | F# API to the `LangaugeExt.Process` system
[LanguageExt.ProcessJS](https://www.nuget.org/packages/LanguageExt.ProcessJS) | Javascript API to the `LangaugeExt.Process` system.  Supports running of Processes in a client browser, with hooks for two-way UI binding

### What's the Actor model?

* An actor is a single threaded process
* It has its own blob of state that only it can see and update
* It has a message queue (inbox)
* It processes the messages one at a time (single threaded remember)
* When a message comes in, it can change its state; when the next message arrives it gets that modified state.
* It has a parent Actor
* It can `spawn` child Actors
* It can `tell` messages to other Actors
* It can `ask` for replies from other Actors
* They're very lightweight, you can create 10,000s of them no problem

So you have a little bundle of self contained computation, attached to a blob of private state, that can get messages telling it to do stuff with its private state.  Sounds like OO right?  Well, it is, but as Alan Kay envisioned it.  The slight difference with this is that it enforces execution order, and therefore there's no shared state access, and no race conditions (within the actor).  
