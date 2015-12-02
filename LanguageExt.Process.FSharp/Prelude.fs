namespace LanguageExt

open LanguageExt
open LanguageExt.UnitsOfMeasure
open System
open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

module ProcessFs = 

    type ProcessId = LanguageExt.ProcessId
    type PID = unit -> ProcessId

    let MakePID (pid:ProcessId) : PID = fun () -> pid

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

    let Self() = 
        Process.Self

    let Parent() = 
        Process.Parent

    let User() = 
        Process.User

    let DeadLetters() = 
        Process.DeadLetters

    let Registered() = 
        Process.Registered

    let Sender() = 
        Process.Sender

    let NoSender() = 
        ProcessId.NoSender

    let childrenSelf() : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.Children)

    let children pid : FSharp.Collections.Map<string,ProcessId> = 
        FSharp.fs(Process.children(pid))

    let NoSetup() = 
        ()
    
    let findProcess name = 
        Process.find(new ProcessName(name))

    let registerSelf name = 
        let pid = Process.register(new ProcessName(name))
        fun () -> pid

    let register name processId flags = 
        let pid = Process.register(new ProcessName(name),processId(),flags,LanguageExt.ProcessSetting.DefaultMailboxSize)
        fun () -> pid

    let deregister name = 
        Process.deregister(new ProcessName(name)) |> ignore

    let killSelf() = 
        Process.kill |> ignore

    let kill pid = 
        Process.kill(pid) |> ignore

    let shutdownAll() = 
        Process.shutdownAll() |> ignore

    let reply msg = 
        Process.reply msg |> ignore

    let replyIfAsked msg = 
        Process.replyIfAsked msg |> ignore

    let ask pid (message : 'a) : 'b = 
        Process.ask<'b>(pid(), message)

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
        Process.tell(pid(),message,sender()) |> ignore

    let tellDelay pid message (delay:TimeSpan) sender = 
        Process.tell(pid(),message,delay,sender())

    let tellChildren message sender = 
        Process.tellChildren(message,sender()) |> ignore

    let tellChildrenDelay message (delay:TimeSpan) sender = 
        Process.tellChildren(message,delay,sender())

    let tellChild name message sender = 
        Process.tellChild(new ProcessName(name), message, sender()) |> ignore

    let tellChildByIndex (index:int) message sender = 
        Process.tellChild(index, message, sender()) |> ignore

    let tellParent message sender = 
        Process.tellParent (message, sender()) |> ignore
    
    let tellParentDelay message (delay:TimeSpan) sender = 
        Process.tellParent(message,delay, sender())

    let tellSelf message = 
        Process.tellSelf(message,Self()) |> ignore
    
    let tellSelfDelay message (delay:TimeSpan) = 
        Process.tellSelf(message,delay,Self())

    let publish message = 
        Process.publish message |> ignore
    
    let publishDelay message (delay:TimeSpan) = 
        Process.publish(message,delay) |> ignore
    
    let subscribe pid = 
        Process.subscribe(pid()) |> ignore
    
    let observe pid = 
        Process.observe(pid());

    let spawn name flags setup messageHandler = 
        let pid = Process.spawn(new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)
        fun () -> pid

    let spawnMany count name flags setup messageHandler = 
        let pids = Process.spawnMany(count, new ProcessName(name), new Func<'state>(setup), new Func<'state, 'msg, 'state>(messageHandler), flags)
        pids |> Seq.map(fun pid -> fun () -> pid )

    //
    // Connects to a cluster.  At the moment we only support Redis, so open
    // LanguageExt.Process.Redis and call:
    //
    //      RedisCluster.register()
    //      clusterConnect "redis" "unique-name-for-this-service" "localhost" "0"
    //
    let clusterConnect clusterProvider nodeName connectionString catalogueString = 
        Cluster.disconnect() |> ignore
        Cluster.connect(clusterProvider,new ProcessName(nodeName),connectionString,catalogueString) |> ignore

    let clusterDisconnect () =
        Cluster.disconnect() |> ignore

    /// Starts a new session in the Process system
    let sessionStart (timeoutSeconds:float<second>) =
        Process.sessionStart((timeoutSeconds/1.0<second>) * LanguageExt.Prelude.seconds)

    /// Ends a session in the Process system with the specified
    /// session ID
    let sessionStop (sid:string) =
        Process.sessionStop(sid) |> ignore

    /// Touch a session
    /// Time-stamps the session so that its time-to-expiry is reset
    let sessionTouch (sid:string) =
        Process.sessionTouch(sid) |> ignore

    /// Gets the current session ID
    /// Also touches the session so that its time-to-expiry 
    /// is reset
    let sessionId() =
        Process.sessionId() |> LanguageExt.FSharp.fs

    /// Set the meta-data to store with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionSetData (sid:string) (data:obj) =
        Process.sessionSetData(sid,data) |> ignore

    /// Clear the meta-data stored with the session
    let sessionClearData (sid:string) =
        Process.sessionClearData(sid) |> ignore

    /// Get the meta-data stored with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    let sessionGetData (sid:string) =
        Process.sessionGetData(sid) |> LanguageExt.FSharp.fs

    /// Returns True if there is an active session
    let hasSession() =
        Process.hasSession()

    /// Acquires a session for the duration of invocation of the 
    /// provided function
    let withSession (sid:string) (f : unit -> 'r) =
        Process.withSession(sid, f)
