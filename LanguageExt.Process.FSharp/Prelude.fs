namespace LanguageExt

open LanguageExt
open LanguageExt.UnitsOfMeasure
open System
open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

module ProcessFs = 

    type SessionId = string
    type ProcessId = LanguageExt.ProcessId
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

    let Parent() = 
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
        match pid.Path with
        | "/__special__/self"         -> Process.Self
        | "/__special__/sender"       -> Process.Sender
        | "/__special__/parent"       -> Process.Parent
        | "/__special__/user"         -> Process.User
        | "/__special__/dead-letters" -> Process.DeadLetters
        | "/__special__/registered"   -> Process.Registered
        | "/__special__/root"         -> Process.Root
        | "/__special__/errors"       -> Process.Errors
        | _                           -> pid

    let childrenSelf() : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.Children)

    let children pid : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.children(pid |> resolvePID))

    // Used to represent the lack of a setup function for a Process
    let NoSetup() = ()

    // Used to coerce a stateless inbox function into one that takes a unit state
    let NoState (inbox:'msg -> unit) = 
        (fun (_:unit) (msg:'msg) -> inbox msg |> ignore)
    
    let findProcess name = 
        Process.find(new ProcessName(name))

    let registerSelf name = 
        Process.register(new ProcessName(name))

    let register name processId flags = 
        Process.register(new ProcessName(name),processId,flags,LanguageExt.ProcessSetting.DefaultMailboxSize)

    let deregister name = 
        Process.deregister(new ProcessName(name)) |> ignore

    let killSelf() = 
        Process.kill |> ignore

    let kill pid = 
        Process.kill(pid |> resolvePID) |> ignore

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
        Process.ask<'b>(pid |> resolvePID, message)

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
        Process.tell(pid |> resolvePID,message,sender) |> ignore

    let tellDelay pid message (delay:TimeSpan) sender = 
        Process.tell(pid |> resolvePID,message,delay,sender)

    let tellChildren message sender = 
        Process.tellChildren(message,sender) |> ignore

    let tellChildrenDelay message (delay:TimeSpan) sender = 
        Process.tellChildren(message,delay,sender)

    let tellChild name message sender = 
        Process.tellChild(new ProcessName(name), message, sender) |> ignore

    let tellChildByIndex (index:int) message sender = 
        Process.tellChild(index, message, sender) |> ignore

    let tellParent message sender = 
        Process.tellParent (message, sender) |> ignore
    
    let tellParentDelay message (delay:TimeSpan) sender = 
        Process.tellParent(message,delay, sender)

    let tellSelf message = 
        Process.tellSelf(message,Process.Self) |> ignore
    
    let tellSelfDelay message (delay:TimeSpan) = 
        Process.tellSelf(message,delay,Process.Self)

    let publish message = 
        Process.publish message |> ignore
    
    let publishDelay message (delay:TimeSpan) = 
        Process.publish(message,delay) |> ignore
    
    let subscribe pid = 
        Process.subscribe(pid |> resolvePID) |> ignore
    
    let observe pid = 
        Process.observe(pid |> resolvePID);

    let spawn name flags setup messageHandler = 
        Process.spawn(new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)

    let spawnMany count name flags setup messageHandler = 
        Process.spawnMany(count, new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)
        |> Seq.map(fun pid -> pid)

    // Connects to a cluster.  At the moment we only support Redis, so open
    // LanguageExt.Process.Redis and call:
    //
    //      RedisCluster.register()
    //      clusterConnect "redis" "unique-name-for-this-service" "localhost" "0"
    let clusterConnect clusterProvider nodeName connectionString catalogueString role = 
        Cluster.disconnect() |> ignore
        Cluster.connect(clusterProvider,new ProcessName(nodeName),connectionString,catalogueString,role) |> ignore

    let clusterDisconnect () =
        Cluster.disconnect() |> ignore

    /// Starts a new session in the Process system
    let sessionStart (timeoutSeconds:float<second>) : SessionId =
        Process.sessionStart((timeoutSeconds/1.0<second>) * LanguageExt.Prelude.seconds)

    /// Ends a session in the Process system with the specified
    /// session ID
    let sessionStop (sid:SessionId) =
        Process.sessionStop(sid) |> ignore

    /// Touch a session
    /// Time-stamps the session so that its time-to-expiry is reset
    let sessionTouch (sid:SessionId) =
        Process.sessionTouch(sid) |> ignore

    /// Gets the current session ID
    /// Also touches the session so that its time-to-expiry 
    /// is reset
    let sessionId() : SessionId option =
        Process.sessionId() 
        |> LanguageExt.FSharp.fs 

    /// Set the meta-data to store with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionSetData (sid:SessionId) (data:obj) =
        Process.sessionSetData(sid,data) |> ignore

    /// Clear the meta-data stored with the session
    let sessionClearData (sid:SessionId) =
        Process.sessionClearData(sid) |> ignore

    /// Get the meta-data stored with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionGetData (sid:SessionId) =
        Process.sessionGetData(sid) |> LanguageExt.FSharp.fs

    /// Returns True if there is an active session
    let hasSession() =
        Process.hasSession()

    /// Acquires a session for the duration of invocation of the 
    /// provided function
    let withSession (sid:SessionId) (f : unit -> 'r) =
        Process.withSession(sid, f)
