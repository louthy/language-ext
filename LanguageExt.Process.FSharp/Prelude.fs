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

//    let internal roundRobinInbox map (s:unit) msg = 
//        tellNextChild (map msg) Sender
//
//    let internal roundRobinSetup count flags inbox = 
//        spawnMany count "worker" flags NoSetup (fun (_:unit) msg -> inbox msg) |> ignore
//
//    let spawnRoundRobin name count flags inbox =
//        spawn name flags (fun () -> roundRobinSetup count flags inbox) (roundRobinInbox id)
//
//    let spawnRoundRobinMap name map count flags inbox =
//        spawn name flags (fun () -> roundRobinSetup count flags inbox) (roundRobinInbox map)
//
//    let spawnRoundRobinMany name manyMap count flags inbox =
//        spawn name flags (fun () -> roundRobinSetup count flags inbox) (roundRobinInboxMany manyMap id)
//
//    let spawnRoundRobinManyMap name manyMap map count flags inbox =
//        spawn name flags (fun () -> roundRobinSetup count flags inbox) (roundRobinInboxMany manyMap map)

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

    /// <summary>
    /// Starts a new session in the Process system
    /// </summary>
    /// <param name="timeoutSeconds">Session timeout is seconds</param>
    /// <returns>Session ID of the newly created session</returns>
    let sessionStart (timeoutSeconds:float<second>) =
        Process.sessionStart((timeoutSeconds/1.0<second>) * LanguageExt.Prelude.seconds)

    /// <summary>
    /// Ends a session in the Process system with the specified
    /// session ID
    /// </summary>
    /// <param name="sid">Session ID</param>
    let sessionStop (sid:string) =
        Process.sessionStop(sid) |> ignore

    /// <summary>
    /// Touch a session
    /// Time-stamps the session so that its time-to-expiry is reset
    /// </summary>
    /// <param name="sid">Session ID</param>
    let sessionTouch (sid:string) =
        Process.sessionTouch(sid) |> ignore

    /// <summary>
    /// Gets the current session ID
    /// </summary>
    /// <remarks>Also touches the session so that its time-to-expiry 
    /// is reset</remarks>
    /// <returns>Optional session ID</returns>
    let sessionId() =
        Process.sessionId() |> LanguageExt.FSharp.fs

    /// <summary>
    /// Set the meta-data to store with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    /// </summary>
    /// <param name="sid">Session ID</param>
    /// <param name="data">Data to store</param>
    let sessionSetData (sid:string) (data:obj) =
        Process.sessionSetData(sid,data) |> ignore

    /// <summary>
    /// Clear the meta-data stored with the session
    /// </summary>
    /// <param name="sid">Session ID</param>
    let sessionClearData (sid:string) =
        Process.sessionClearData(sid) |> ignore

    /// <summary>
    /// Get the meta-data stored with the session, this is typically
    /// user credentials when they've logged in.  But can be anything.
    /// </summary>
    /// <param name="sid">Session ID</param>
    let sessionGetData (sid:string) =
        Process.sessionGetData(sid) |> LanguageExt.FSharp.fs

    /// <summary>
    /// Returns True if there is an active session
    /// </summary>
    /// <returns></returns>
    let hasSession() =
        Process.hasSession()

    /// <summary>
    /// Acquires a session for the duration of invocation of the 
    /// provided function
    /// </summary>
    /// <param name="sid">Session ID</param>
    /// <param name="f">Function to invoke</param>
    /// <returns>Result of the function</returns>
    let withSession (sid:string) (f : unit -> 'r) =
        Process.withSession(sid, f)
