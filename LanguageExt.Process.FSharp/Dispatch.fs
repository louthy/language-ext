namespace LanguageExt

open System
open ProcessFs

module DispatchFs =
  
    /// Registers a dispatcher for a role
    /// Dispatchers take in a 'leaf' ProcessId (i.e. /user/my-process) and return an enumerable
    /// of real ProcessIds that the Process system will use to deliver all of the standard functions
    /// like tell, ask, subscribe, etc.
    let register (name:string) (selector:ProcessId -> ProcessId seq) =
        Dispatch.register(new ProcessName(name), new Func<ProcessId,System.Collections.Generic.IEnumerable<ProcessId>>(selector))

    /// Removes the dispatcher registration for the named dispatcher
    let deregister (name:string) : unit =
        Dispatch.deregister(new ProcessName(name)) |> ignore

    /// Generates a ProcessId that can be used to dispatch to multiple Processes at once
    let broadcast (processIds:ProcessId seq) : ProcessId =
        Dispatch.broadcast processIds

    /// Generates a ProcessId that can be used to dispatch to the least busy Process
    let leastBusy (processIds:ProcessId seq) : ProcessId =
        Dispatch.leastBusy processIds

    /// Generates a ProcessId that can be used to dispatch to a random Process
    let random (processIds:ProcessId seq) : ProcessId =
        Dispatch.random processIds

    /// Generates a ProcessId that can be used to dispatch to a set of Processes in a 
    /// round-robin fashion
    let roundRobin (processIds:ProcessId seq) : ProcessId =
        Dispatch.roundRobin processIds
