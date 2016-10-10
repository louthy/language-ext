﻿namespace LanguageExt

open LanguageExt
open LanguageExt.UnitsOfMeasure
open System
open ProcessFs

module RouterFs =

    let DefaultRouterOption              = LanguageExt.RouterOption.Default
    let RemoveLocalWorkerWhenTerminated  = LanguageExt.RouterOption.RemoveLocalWorkerWhenTerminated
    let RemoveRemoteWorkerWhenTerminated = LanguageExt.RouterOption.RemoveRemoteWorkerWhenTerminated
    let RemoveWorkerWhenTerminated       = LanguageExt.RouterOption.RemoveWorkerWhenTerminated

    /// Spawns a router based on the settings in process.conf
    /// Uses existing processes as the routees
    let fromConfigName (name:string) =
        Router.fromConfig(new ProcessName(name))

    /// Spawns a router based on the settings in process.conf
    /// Creates child worker processes as the routees
    let fromConfig (name:string) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) =
        Router.fromConfig(new ProcessName(name), new Func<'state>(setup), new Func<'state,'msg,'state>(inbox))

    /// Spawns a new process with Count worker processes, each message is sent to 
    /// all worker processes
    let broadcast (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) =
        Router.broadcast(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'msg,'state>(inbox), flags)

    /// Spawns a new process with that routes each message to the Workers
    /// in a round robin fashion.
    let broadcastWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) =
        Router.broadcast(new ProcessName(name), workers, option, flags)

    /// Spawns a new process with Count worker processes, each message is sent to 
    /// all worker processes
    let broadcastMap (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg) =
        Router.broadcastMap(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg>(mapper), flags)

    /// Spawns a new process with that routes each message to all workers
    /// Each message is mapped before being broadcast.
    let broadcastMapWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg) =
        Router.broadcastMap(new ProcessName(name), workers, new Func<'tmsg,'umsg>(mapper), option, flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting Us are passed all of the 
    /// worker processes.
    let broadcastMapMany (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg seq) =
        Router.broadcastMapMany(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg seq>(mapper), flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting Us are passed all of the 
    /// worker processes.
    let broadcastMapManyWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg seq) =
        Router.broadcastMapMany(new ProcessName(name), workers, new Func<'tmsg,'umsg seq>(mapper), option, flags)

    /// Spawns a new process with Count worker processes, each message is 
    /// sent to the least busy worker.
    let leastBusy (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) =
        Router.leastBusy(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'msg,'state>(inbox), flags)

    /// Spawns a new process with that routes each message to the 
    /// least busy worker.
    let leastBusyWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) =
        Router.leastBusy(new ProcessName(name), workers, option, flags)

    /// Spawns a new process with Count worker processes, each message is mapped
    /// and sent to the least busy worker
    let leastBusyMap (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg) =
        Router.leastBusyMap(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg>(mapper), flags)

    /// Spawns a new process with worker processes, each message is mapped
    /// and sent to the least busy worker
    let leastBusyMapWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg) =
        Router.broadcastMap(new ProcessName(name), workers, new Func<'tmsg,'umsg>(mapper), option, flags)

    /// Spawns a new process with Count worker processes, each message is sent to one worker
    /// process randomly.
    let random (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) =
        Router.random(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'msg,'state>(inbox), flags)

    /// Spawns a new process with that routes each message to the Workers
    /// randomly.
    let randomWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) =
        Router.random(new ProcessName(name), workers, option, flags)

    /// Spawns a new process with Count worker processes, each message is sent to one worker
    /// process randomly.
    let randomMap (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg) =
        Router.randomMap(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg>(mapper), flags)

    /// Spawns a new process with that routes each message to the Workers
    /// randomly.
    let randomMapWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg) =
        Router.randomMap(new ProcessName(name), workers, new Func<'tmsg,'umsg>(mapper), option, flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes randomly.
    let randomMapMany (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg seq) =
        Router.randomMapMany(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg seq>(mapper), flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes randomly.
    let randomMapManyWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg seq) =
        Router.randomMapMany(new ProcessName(name), workers, new Func<'tmsg,'umsg seq>(mapper), option, flags)

    /// Spawns a new process with Count worker processes, each message is sent to one worker
    /// process in a round robin fashion.
    let roundRobin (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) =
        Router.roundRobin(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'msg,'state>(inbox), flags)

    /// Spawns a new process with that routes each message to the Workers
    /// in a round robin fashion.
    let roundRobinWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) =
        Router.roundRobin(new ProcessName(name), workers, option, flags)

    /// Spawns a new process with Count worker processes, each message is mapped
    /// and sent to one worker process in a round robin fashion.
    let roundRobinMap (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg) =
        Router.roundRobinMap(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg>(mapper), flags)

    /// Spawns a new process with that routes each message is mapped and 
    /// sent to the Workers in a round robin fashion.
    let roundRobinMapWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg) =
        Router.roundRobinMap(new ProcessName(name), workers, new Func<'tmsg,'umsg>(mapper), option, flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes in a round robin fashion.
    let roundRobinMapMany (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg seq) =
        Router.roundRobinMapMany(new ProcessName(name), count, new Func<'state>(setup), new Func<'state,'umsg,'state>(inbox), new Func<'tmsg,'umsg seq>(mapper), flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes in a round robin fashion.
    let roundRobinMapManyWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg seq) =
        Router.roundRobinMapMany(new ProcessName(name), workers, new Func<'tmsg,'umsg seq>(mapper), option, flags)

    /// Spawns a new process with Count worker processes, each message is sent to one worker
    /// process in a round robin fashion.
    let hash (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'msg -> 'state) (hashfn:int -> 'msg -> int) =
        Router.hash(new ProcessName(name), count, setup, inbox, hashfn, flags)

    /// Spawns a new process with that routes each message to the Workers
    /// in a round robin fashion.
    let hashWith (name:string) (workers:ProcessId list) (hashfn:int -> 'msg -> int) (option:RouterOption) (flags:ProcessFlags) =
        Router.hash(new ProcessName(name), workers, hashfn, option, flags)

    /// Spawns a new process with Count worker processes, each message is mapped
    /// and sent to one worker process in a round robin fashion.
    let hashMap (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg) (hashfn:int -> 'tmsg -> int) =
        Router.hashMap(new ProcessName(name), count, setup, inbox, mapper, hashfn, flags)

    /// Spawns a new process with that routes each message is mapped and 
    /// sent to the Workers in a round robin fashion.
    let hashMapWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg) (hashfn:int -> 'umsg -> int) =
        Router.hashMap(new ProcessName(name), workers, mapper, hashfn, option, flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes in a round robin fashion.
    let hashMapMany (name:string) (count:int) (flags:ProcessFlags) (setup:unit -> 'state) (inbox:'state -> 'umsg -> 'state) (mapper:'tmsg -> 'umsg seq) (hashfn:int -> 'umsg -> int) =
        Router.hashMapMany(new ProcessName(name), count, setup, inbox, mapper, hashfn, flags)

    /// Spawns a new process with N worker processes, each message is mapped 
    /// from T to IEnumerable U before each resulting U is passed to the worker
    /// processes in a round robin fashion.
    let hashMapManyWith (name:string) (workers:ProcessId list) (option:RouterOption) (flags:ProcessFlags) (mapper:'tmsg -> 'umsg seq) (hashfn:int -> 'umsg -> int) =
        Router.hashMapMany(new ProcessName(name), workers, mapper, hashfn, option, flags)
