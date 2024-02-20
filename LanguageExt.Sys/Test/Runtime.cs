using System;
using System.Text;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    /// <summary>
    /// Test IO runtime
    /// </summary>
    public readonly struct Runtime : 
        HasActivitySource<Runtime>,
        HasConsole<Runtime>,
        HasFile<Runtime>,
        HasTextRead<Runtime>,
        HasTime<Runtime>,
        HasEnvironment<Runtime>,
        HasDirectory<Runtime>
    {
        public readonly RuntimeEnv env;

        /// <summary>
        /// Constructor
        /// </summary>
        Runtime(RuntimeEnv env) =>
            this.env = env;

        /// <summary>
        /// Configuration environment accessor
        /// </summary>
        public RuntimeEnv Env =>
            env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(SynchronizationContext.Current,
                                       new CancellationTokenSource(),
                                       Encoding.Default,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(CancellationTokenSource source, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(SynchronizationContext.Current,
                                       source,
                                       Encoding.Default,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(Encoding encoding, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(SynchronizationContext.Current,
                                       new CancellationTokenSource(),
                                       encoding,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(Encoding encoding, CancellationTokenSource source, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(SynchronizationContext.Current,
                                       source,
                                       encoding,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));


        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(SynchronizationContext syncContext, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(syncContext,
                                       new CancellationTokenSource(),
                                       Encoding.Default,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(SynchronizationContext syncContext, CancellationTokenSource source, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(syncContext,
                                       source,
                                       Encoding.Default,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(SynchronizationContext syncContext, Encoding encoding, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(syncContext,
                                       new CancellationTokenSource(),
                                       encoding,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(SynchronizationContext syncContext, Encoding encoding, CancellationTokenSource source,
            TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(syncContext,
                                       source,
                                       encoding,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec,
                                       MemorySystemEnvironment.InitFromSystem()));        
        
        /// <summary>
        /// Create a new Runtime with a fresh cancellation token
        /// </summary>
        /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
        /// <returns>New runtime</returns>
        public Runtime LocalCancel =>
            new Runtime(Env.LocalCancel);

        /// <summary>
        /// Direct access to cancellation token
        /// </summary>
        public CancellationToken CancellationToken =>
            Env.Token;

        /// <summary>
        /// Directly access the cancellation token source
        /// </summary>
        /// <returns>CancellationTokenSource</returns>
        public CancellationTokenSource CancellationTokenSource =>
            Env.Source;

        /// <summary>
        /// Get encoding
        /// </summary>
        /// <returns></returns>
        public Encoding Encoding =>
            Env.Encoding;

        /// <summary>
        /// Access the console environment
        /// </summary>
        /// <returns>Console environment</returns>
        public Eff<Runtime, Traits.ConsoleIO> ConsoleIO =>
            lift<Runtime, Traits.ConsoleIO>(rt => new ConsoleIO(rt.Env.Console));

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, Traits.FileIO> FileEff =>
            from n in Time<Runtime>.now
            from r in lift<Runtime, Traits.FileIO>(rt => new FileIO(rt.Env.FileSystem, n))
            select r;

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, Traits.DirectoryIO> DirectoryIO =>
            from n in Time<Runtime>.now
            from r in lift<Runtime, Traits.DirectoryIO>(rt => new DirectoryIO(rt.Env.FileSystem, n))
            select r;
        
        /// <summary>
        /// Access the TextReader environment
        /// </summary>
        /// <returns>TextReader environment</returns>
        public Eff<Runtime, Traits.TextReadIO> TextReadEff =>
            SuccessEff(TextReadIO.Default);

        /// <summary>
        /// Access the time environment
        /// </summary>
        /// <returns>Time environment</returns>
        public Eff<Runtime, Traits.TimeIO> TimeIO  =>
            Eff<Runtime, Traits.TimeIO>(rt => new TimeIO(rt.Env.TimeSpec));

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, Traits.EnvironmentIO> EnvironmentIO =>
            Eff<Runtime, Traits.EnvironmentIO>(rt => new EnvironmentIO(rt.Env.SysEnv));

        public Runtime WithSyncContext(SynchronizationContext syncContext) =>
            new(Env with { SyncContext = syncContext });

        public SynchronizationContext SynchronizationContext =>
            Env.SyncContext;
        
        public Error FromError(Error error) =>
            error;
    }
    
    public record RuntimeEnv(
        SynchronizationContext SyncContext,
        CancellationTokenSource Source,
        CancellationToken Token,
        Encoding Encoding,
        MemoryConsole Console,
        MemoryFS FileSystem,
        TestTimeSpec? TimeSpec,
        MemorySystemEnvironment SysEnv)
    {
        public RuntimeEnv(
            SynchronizationContext syncContext,
            CancellationTokenSource source, 
            Encoding encoding, 
            MemoryConsole console,
            MemoryFS fileSystem, 
            TestTimeSpec? timeSpec,
            MemorySystemEnvironment sysEnv) : 
            this(syncContext, source, source.Token, encoding, console, fileSystem, timeSpec, sysEnv)
        {
        }

        public RuntimeEnv LocalCancel =>
            new (SyncContext, new CancellationTokenSource(), Encoding, Console, FileSystem, TimeSpec, SysEnv); 
    }
}
