namespace LanguageExt

open LanguageExt
open LanguageExt.UnitsOfMeasure
open System
open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

module ProcessFs = 

    type SessionId   = LanguageExt.SessionId
    type ProcessId   = LanguageExt.ProcessId
    type ProcessName = LanguageExt.ProcessName

    let DefaultFlags = 
        ProcessFlags.Default

    let PersistInbox = 
        ProcessFlags.PersistInbox

    let PersistState = 
        ProcessFlags.PersistState

    let RemotePublish = 
        ProcessFlags.RemotePublish

    let PersistAll = 
        ProcessFlags.PersistAll

    let Self = 
        new ProcessId("/__special__/self")

    let Parent = 
        new ProcessId("/__special__/parent")

    let User = 
        new ProcessId("/__special__/user")

    let DeadLetters = 
        new ProcessId("/__special__/dead-letters")

    let Registered = 
        new ProcessId("/__special__/registered")

    let Sender = 
        new ProcessId("/__special__/sender")

    let Root = 
        new ProcessId("/__special__/root")

    let Errors = 
        new ProcessId("/__special__/errors")

    let NoSender = 
        ProcessId.NoSender

    let isSenderValid () =
        Process.Sender.IsValid

    let isSelfValid () =
        Process.Self.IsValid

    let resolvePID (pid:ProcessId) : ProcessId = 
        Process.resolvePID(pid)

    let childrenSelf() : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.Children)

    let children pid : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.children(pid |> resolvePID))

    // Used to represent the lack of a setup function for a Process
    let NoSetup() = ()

    // Used to coerce a stateless inbox function into one that takes a unit state
    let NoState (inbox:'msg -> unit) = 
        (fun (_:unit) (msg:'msg) -> inbox msg |> ignore)
    
    /// Find a process by its *registered* name (a kind of DNS for Processes).
    /// 
    /// Multiple Processes can register under the same name.  You may use 
    /// a dispatcher to work on them collectively (wherever they are in the 
    /// cluster).  i.e. 
    /// 
    ///     var regd = register "proc" pid
    ///     tell DispatchFs.Broadcast.[regd] "Hello" Self
    ///     tell DispatchFs.First.[regd] "Hello" Self
    ///     tell DispatchFs.LeastBusy.[regd] "Hello" Self
    ///     tell DispatchFs.Random.[regd] "Hello" Self
    ///     tell DispatchFs.RoundRobin.[regd] "Hello" Self
    let findProcess name = 
        Process.find(new ProcessName(name))

    /// Register a named process (a kind of DNS for Processes).  
    /// 
    /// If the Process is visible to the cluster (PersistInbox) then the 
    /// registration becomes a permanent named look-up until Process.deregister 
    /// is called.
    /// 
    /// Multiple Processes can register under the same name.  You may use 
    /// a dispatcher to work on them collectively (wherever they are in the 
    /// cluster).  i.e. 
    /// 
    ///     var regd = registerSelf proc
    ///     tell DispatchFs.Broadcast.[regd] "Hello" Self
    ///     tell DispatchFs.First.[regd] "Hello" Self
    ///     tell DispatchFs.LeastBusy.[regd] "Hello" Self
    ///     tell DispatchFs.Random.[regd] "Hello" Self
    ///     tell DispatchFs.RoundRobin.[regd] "Hello" Self
    /// 
    ///     This should be used from within a process' message loop only
    let registerSelf name = 
        Process.register(new ProcessName(name))

    /// Register a named process (a kind of DNS for Processes).  
    /// 
    /// If the Process is visible to the cluster (PersistInbox) then the 
    /// registration becomes a permanent named look-up until Process.deregister 
    /// is called.
    /// 
    /// Multiple Processes can register under the same name.  You may use 
    /// a dispatcher to work on them collectively (wherever they are in the 
    /// cluster).  i.e. 
    /// 
    ///     var regd = register "proc" pid
    ///     tell DispatchFs.Broadcast.[regd] "Hello" Self
    ///     tell DispatchFs.First.[regd] "Hello" Self
    ///     tell DispatchFs.LeastBusy.[regd] "Hello" Self
    ///     tell DispatchFs.Random.[regd] "Hello" Self
    ///     tell DispatchFs.RoundRobin.[regd] "Hello" Self
    /// 
    ///     This should be used from within a process' message loop only
    let register name processId system = 
        Process.register(new ProcessName(name), resolvePID processId)

    /// Deregister all Processes associated with a name. NOTE: Be very careful
    /// with usage of this function if you didn't handle the registration you
    /// are potentially disconnecting many Processes from their registered name.
    /// 
    /// Any Process (or dispatcher, or role, etc.) can be registered by a name - 
    /// a kind of DNS for ProcessIds.  There can be multiple names associated
    /// with a single ProcessId and multiple ProcessIds associated with a name.
    /// 
    /// This function removes all registered ProcessIds for a specific name.
    /// If you wish to deregister all names registered for specific Process then
    /// use deregisterById pid
    let deregisterByName name = 
        Process.deregisterByName(new ProcessName(name)) |> ignore

    /// Deregister a Process from any names it's been registered as.
    /// 
    /// Any Process (or dispatcher, or role, etc.) can be registered by a name - 
    /// a kind of DNS for ProcessIds.  There can be multiple names associated
    /// with a single ProcessId.  
    /// 
    /// This function removes all registered names for a specific ProcessId.
    /// If you wish to deregister all ProcessIds registered under a name then
    /// use deregisterByName name
    let deregisterById pid = 
        Process.deregisterById(resolvePID pid) |> ignore

    let killSelf() = 
        Process.kill |> ignore

    let kill pid = 
        Process.kill(resolvePID pid) |> ignore

    let shutdownAll() = 
        Process.shutdownAll() |> ignore

    let isAsk () =
        Process.isAsk

    /// Reply to an ask
    let reply msg : unit = 
        Process.reply msg |> ignore

    /// Reply if asked
    let replyIfAsked msg : unit = 
        Process.replyIfAsked msg |> ignore

    /// Reply to the asker, or if it's not an ask then tell the sender
    /// via a message to their inbox.
    let replyOrTellSender msg : unit = 
        Process.replyOrTellSender msg |> ignore

    let ask pid (message : 'a) : 'b = 
        Process.ask<'b>(resolvePID pid, message)

    let askChildren (message : 'a) : 'b seq = 
        Process.askChildren<'b>(message,Int32.MaxValue)

    let askChildrenTake (message : 'a) count : 'b seq = 
        Process.askChildren<'b>(message,count)

    let askChildrenFirst (message : 'a) = 
        askChildrenTake message 1

    let askParent (message : 'a) : 'b = 
        Process.askParent message 

    let askChild name (message : 'a) : 'b = 
        Process.askChild(new ProcessName(name), message)

    let askChildByIndex (index:int) (message : 'a) : 'b = 
        Process.askChild(index, message)

    let tell pid message sender = 
        Process.tell(resolvePID pid,message,resolvePID sender) |> ignore

    let tellDelay pid message (delay:TimeSpan) sender = 
        Process.tell(resolvePID pid,message,delay,resolvePID sender)

    let tellChildren message sender = 
        Process.tellChildren(message,resolvePID sender) |> ignore

    let tellChildrenDelay message (delay:TimeSpan) sender = 
        Process.tellChildren(message,delay,resolvePID sender)

    let tellChild name message sender = 
        Process.tellChild(new ProcessName(name), message, resolvePID sender) |> ignore

    let tellChildByIndex (index:int) message sender = 
        Process.tellChild(index, message, resolvePID sender) |> ignore

    let tellParent message sender = 
        Process.tellParent (message, resolvePID sender) |> ignore
    
    let tellParentDelay message (delay:TimeSpan) sender = 
        Process.tellParent(message,delay, resolvePID sender)

    let tellSelf message = 
        Process.tellSelf(message,Process.Self) |> ignore
    
    let tellSelfDelay message (delay:TimeSpan) = 
        Process.tellSelf(message,delay,Process.Self)

    let publish message = 
        Process.publish message |> ignore
    
    let publishDelay message (delay:TimeSpan) = 
        Process.publish(message, delay) |> ignore
    
    let subscribe pid = 
        Process.subscribe(resolvePID pid) |> ignore
    
    let observe pid = 
        Process.observe(resolvePID pid);

    let spawn name flags setup messageHandler = 
        Process.spawn(new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)

    let spawnMany count name flags setup messageHandler = 
        Process.spawnMany(count, new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)
        |> Seq.map(fun pid -> pid)

    /// Starts a new session in the Process system
    let sessionStart (timeoutSeconds:float<second>) : SessionId =
        Process.sessionStart((timeoutSeconds/1.0<second>) * LanguageExt.Prelude.seconds)

    /// Ends a session in the Process system with the specified
    /// session ID
    let sessionStop () =
        Process.sessionStop() |> ignore

    /// Touch the current session
    /// Time-stamps the session so that its time-to-expiry is reset
    let sessionTouch () =
        Process.sessionTouch() |> ignore

    /// Touch a provided session
    /// Time-stamps the session so that its time-to-expiry is reset
    let sessionUser (sid:SessionId) =
        Process.sessionTouch(sid) |> ignore

    /// Gets the current session ID
    /// Also touches the session so that its time-to-expiry 
    /// is reset
    let sessionId() : SessionId option =
        Process.sessionId() 
        |> LanguageExt.FSharp.fs 

    /// Set the meta-data to store with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionSetData (key:string) (value:obj) =
        Process.sessionSetData(key, value) |> ignore

    /// Clear the meta-data stored with the session
    let sessionClearData (key:string) =
        Process.sessionClearData(key) |> ignore

    /// Get the meta-data stored with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionGetData (key:string) =
        Process.sessionGetData(key) |> LanguageExt.FSharp.fs

    /// Returns True if there is an active session
    let hasSession() =
        Process.hasSession()

    /// Acquires a session for the duration of invocation of the 
    /// provided function
    let withSession (sid:SessionId) (f : unit -> 'r) =
        Process.withSession(sid, f)
