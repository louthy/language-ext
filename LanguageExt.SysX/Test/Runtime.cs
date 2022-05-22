using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using LanguageExt.Sys.Traits;
using LanguageExt.Sys.Test;
using static LanguageExt.Prelude;
using LanguageExt.Sys;
using LanguageExt.SysX.Traits;

namespace LanguageExt.SysX.Test
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
            new Runtime(new RuntimeEnv(ActivityEnv.Default,
                                        new CancellationTokenSource(),
                                        Encoding.Default,
                                        new MemoryConsole(),
                                        new MemoryFS(),
                                        timeSpec ?? TestTimeSpec.RunningFromNow(),
                                        MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(ActivityEnv activity, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(activity,
                                       new CancellationTokenSource(),
                                       Encoding.Default,
                                       new MemoryConsole(),
                                       new MemoryFS(),
                                       timeSpec ?? TestTimeSpec.RunningFromNow(),
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(ActivityEnv activity, CancellationTokenSource source, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(activity,
                                       source, 
                                       Encoding.Default, 
                                       new MemoryConsole(), 
                                       new MemoryFS(),
                                       timeSpec ?? TestTimeSpec.RunningFromNow(),
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(ActivityEnv activity, Encoding encoding, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(activity, 
                                       new CancellationTokenSource(), 
                                       encoding, 
                                       new MemoryConsole(), 
                                       new MemoryFS(),
                                       timeSpec ?? TestTimeSpec.RunningFromNow(),
                                       MemorySystemEnvironment.InitFromSystem()));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        /// <param name="timeSpec">Defines how time works in the runtime</param>
        public static Runtime New(ActivityEnv activity, Encoding encoding, CancellationTokenSource source, TestTimeSpec? timeSpec = default) =>
            new Runtime(new RuntimeEnv(activity,
                                       source, 
                                       encoding, 
                                       new MemoryConsole(), 
                                       new MemoryFS(),
                                       timeSpec ?? TestTimeSpec.RunningFromNow(),
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
        /// Set the current activity and update the ParentId automatically
        /// </summary>
        /// <param name="activity">Activity to set</param>
        /// <returns>Updated runtime</returns>
        public Runtime SetActivity(Activity? activity) =>
            new (Env 
                with {Activity = Env.Activity 
                    with {Activity = activity, ParentId = Env.Activity.Activity?.Id ?? ""}});

        /// <summary>
        /// Get the current activity 
        /// </summary>
        public Activity? CurrentActivity =>
            Env.Activity.Activity;

        /// <summary>
        /// Activity source environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, ActivitySourceIO> ActivitySourceEff =>
            Eff<Runtime, ActivitySourceIO>(rt => new LanguageExt.SysX.Live.ActivitySourceIO(rt.Env.Activity.ActivitySource));
        
        /// <summary>
        /// Access the console environment
        /// </summary>
        /// <returns>Console environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.ConsoleIO> ConsoleEff =>
            Eff<Runtime, LanguageExt.Sys.Traits.ConsoleIO>(rt => new LanguageExt.Sys.Test.ConsoleIO(rt.Env.Console));

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.FileIO> FileEff =>
            from n in Time<Runtime>.now
            from r in Eff<Runtime, LanguageExt.Sys.Traits.FileIO>(rt => new LanguageExt.Sys.Test.FileIO(rt.Env.FileSystem, n))
            select r;

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.DirectoryIO> DirectoryEff =>
            from n in Time<Runtime>.now
            from r in Eff<Runtime, LanguageExt.Sys.Traits.DirectoryIO>(rt => new LanguageExt.Sys.Test.DirectoryIO(rt.Env.FileSystem, n))
            select r;
        
        /// <summary>
        /// Access the TextReader environment
        /// </summary>
        /// <returns>TextReader environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.TextReadIO> TextReadEff =>
            SuccessEff(LanguageExt.Sys.Test.TextReadIO.Default);

        /// <summary>
        /// Access the time environment
        /// </summary>
        /// <returns>Time environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.TimeIO> TimeEff  =>
            Eff<Runtime, LanguageExt.Sys.Traits.TimeIO>(rt => new LanguageExt.Sys.Test.TimeIO(rt.Env.TimeSpec));

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, LanguageExt.Sys.Traits.EnvironmentIO> EnvironmentEff =>
            Eff<Runtime, LanguageExt.Sys.Traits.EnvironmentIO>(rt => new LanguageExt.Sys.Test.EnvironmentIO(rt.Env.SysEnv));
    }
    
    public record RuntimeEnv(
        ActivityEnv Activity,
        CancellationTokenSource Source,
        CancellationToken Token,
        Encoding Encoding,
        MemoryConsole Console,
        MemoryFS FileSystem,
        TestTimeSpec TimeSpec,
        MemorySystemEnvironment SysEnv) 
    {
        public RuntimeEnv(
            ActivityEnv activity,
            CancellationTokenSource source, 
            Encoding encoding, 
            MemoryConsole console,
            MemoryFS fileSystem, 
            TestTimeSpec? timeSpec,
            MemorySystemEnvironment sysEnv) : 
            this(activity, source, source.Token, encoding, console, fileSystem, timeSpec ?? TestTimeSpec.RunningFromNow(), sysEnv)
        {
        }

        public RuntimeEnv LocalCancel =>
            new RuntimeEnv(Activity, new CancellationTokenSource(), Encoding, Console, FileSystem, TimeSpec, SysEnv); 
    }
}
