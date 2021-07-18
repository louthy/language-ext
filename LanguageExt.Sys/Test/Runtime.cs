using System;
using System.Text;
using System.Threading;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    /// <summary>
    /// Test IO runtime
    /// </summary>
    public readonly struct Runtime : 
        HasCancel<Runtime>,
        HasConsole<Runtime>,
        HasFile<Runtime>,
        HasEncoding<Runtime>,
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
            new Runtime(new RuntimeEnv(new CancellationTokenSource(),
                                       System.Text.Encoding.Default,
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
            new Runtime(new RuntimeEnv(source, 
                                       System.Text.Encoding.Default, 
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
            new Runtime(new RuntimeEnv(new CancellationTokenSource(), 
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
            new Runtime(new RuntimeEnv(source, 
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
        public Eff<Runtime, Traits.ConsoleIO> ConsoleEff =>
            Eff<Runtime, Traits.ConsoleIO>(rt => new Test.ConsoleIO(rt.Env.Console));

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, Traits.FileIO> FileEff =>
            from n in Time<Runtime>.now
            from r in Eff<Runtime, Traits.FileIO>(rt => new Test.FileIO(rt.Env.FileSystem, n))
            select r;

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, Traits.DirectoryIO> DirectoryEff =>
            from n in Time<Runtime>.now
            from r in Eff<Runtime, Traits.DirectoryIO>(rt => new Test.DirectoryIO(rt.Env.FileSystem, n))
            select r;
        
        /// <summary>
        /// Access the TextReader environment
        /// </summary>
        /// <returns>TextReader environment</returns>
        public Eff<Runtime, Traits.TextReadIO> TextReadEff =>
            SuccessEff(Test.TextReadIO.Default);

        /// <summary>
        /// Access the time environment
        /// </summary>
        /// <returns>Time environment</returns>
        public Eff<Runtime, Traits.TimeIO> TimeEff  =>
            Eff<Runtime, Traits.TimeIO>(rt => new Test.TimeIO(rt.Env.TimeSpec));

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, Traits.EnvironmentIO> EnvironmentEff =>
            Eff<Runtime, Traits.EnvironmentIO>(rt => new Test.EnvironmentIO(rt.Env.SysEnv));
    }
    
    public class RuntimeEnv
    {
        public readonly CancellationTokenSource Source;
        public readonly CancellationToken Token;
        public readonly Encoding Encoding;
        public readonly MemoryConsole Console;
        public readonly MemoryFS FileSystem;
        public readonly TestTimeSpec TimeSpec;
        public readonly MemorySystemEnvironment SysEnv;

        public RuntimeEnv(
            CancellationTokenSource source, 
            CancellationToken token, 
            Encoding encoding, 
            MemoryConsole console, 
            MemoryFS fileSystem, 
            TestTimeSpec? timeSpec,
            MemorySystemEnvironment sysEnv)
        {
            Source     = source;
            Token      = token;
            Encoding   = encoding;
            Console    = console;
            FileSystem = fileSystem;
            TimeSpec   = timeSpec ?? TestTimeSpec.RunningFromNow();
            SysEnv     = sysEnv;
        }

        public RuntimeEnv(
            CancellationTokenSource source, 
            Encoding encoding, 
            MemoryConsole console,
            MemoryFS fileSystem, 
            TestTimeSpec? timeSpec,
            MemorySystemEnvironment sysEnv) : 
            this(source, source.Token, encoding, console, fileSystem, timeSpec, sysEnv)
        {
        }

        public RuntimeEnv LocalCancel =>
            new RuntimeEnv(new CancellationTokenSource(), Encoding, Console, FileSystem, TimeSpec, SysEnv); 
    }
}
