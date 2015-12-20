
## LanguageExt.Process

An issue with working with C# is that no matter how much of this library you take on-board, you will always end up bumping into mutable state or side-effecting systems.  A way around that is to package up the side-effects into atoms of functional computation that are attached to the mutable state (in whatever form it may take).

The [Actor model](https://en.wikipedia.org/wiki/Actor_model) + functional message handling expressions are the perfect programming model for that.  Concurrent programming in C# isn't a huge amount of fun.  Yes the TPL gets you lots of stuff for free, but it doesn't magically protect you from race conditions or accessing shared state, and definitely doesn't help with accessing shared external state like a database.

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

### Documentation

Documention | Description
--------|-------------| ---------
[Process system overview](https://github.com/louthy/language-ext/wiki/Process-system) | A quick guide to the core features of the Process system
[Message dispatch](https://github.com/louthy/language-ext/wiki/Process-system-message-dispatch) | The power of any actor system, especially when it comes to a changing network topology is in its message routing and dispatching
[ProcessId](https://github.com/louthy/language-ext/wiki/ProcessId) |  Process address/location mechansim
[Routers](https://github.com/louthy/language-ext/wiki/Routers) | Processes that manage sets of 'worker' processes by routing their received messages by following pre-defined behaviours, e.g. Round-robin, broadcast, etc.
[Dispatchers](https://github.com/louthy/language-ext/wiki/Dispatchers) | Similar to routers but without the need for a router process, all routing is done by the sender
[Registered processes](https://github.com/louthy/language-ext/wiki/Registered-processes) | A sort of DNS for Processes, can also register dispatchers
[Roles](https://github.com/louthy/language-ext/wiki/Roles) | A special type of dispatcher that's aware of the aliveness of cluster nodes and what their roles are

### Nuget

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'.  This is all that's needed to get started.
[LanguageExt.Process](https://www.nuget.org/packages/LanguageExt.Process) | 'Erlang like' actor system for in-app messaging and massive concurrency
[LanguageExt.Process.Redis](https://www.nuget.org/packages/LanguageExt.Process.Redis) | Cluster support for the `LangaugeExt.Process` system for cluster aware processes using Redis for queue and state persistence
[LanguageExt.Process.FSharp](https://www.nuget.org/packages/LanguageExt.Process.FSharp) | F# API to the `LangaugeExt.Process` system
[LanguageExt.ProcessJS](https://www.nuget.org/packages/LanguageExt.ProcessJS) | Javascript API to the `LangaugeExt.Process` system.  Supports running of Processes in a client browser, with hooks for two-way UI binding
