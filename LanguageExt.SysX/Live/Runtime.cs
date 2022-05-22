using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using LanguageExt.Sys.Traits;
using LanguageExt.SysX.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.SysX.Live
{
    /// <summary>
    /// Live IO runtime
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
        readonly RuntimeEnv env;

        /// <summary>
        /// Constructor
        /// </summary>
        Runtime(RuntimeEnv env) =>
            this.env = env;

        /// <summary>
        /// Configuration environment accessor
        /// </summary>
        public RuntimeEnv Env =>
            env ?? throw new InvalidOperationException(
                "Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

        /// <summary>
        /// Constructor function
        /// </summary>
        public static Runtime New() =>
            new Runtime(new RuntimeEnv(ActivityEnv.Default, new CancellationTokenSource(), Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="source">Cancellation token source</param>
        public static Runtime New(ActivityEnv activity, CancellationTokenSource source) =>
            new Runtime(new RuntimeEnv(activity, source, Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="encoding">Text encoding</param>
        public static Runtime New(ActivityEnv activity, Encoding encoding) =>
            new Runtime(new RuntimeEnv(activity, new CancellationTokenSource(), encoding));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="activity">Tracing activity</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        public static Runtime New(ActivityEnv activity, Encoding encoding, CancellationTokenSource source) =>
            new Runtime(new RuntimeEnv(activity, source, encoding));

        /// <summary>
        /// Create a new Runtime with a fresh cancellation token
        /// </summary>
        /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
        /// <returns>New runtime</returns>
        public Runtime LocalCancel =>
            new Runtime(new RuntimeEnv(Env.Activity, new CancellationTokenSource(), Env.Encoding));

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
            new Runtime(Env 
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
        public Eff<Runtime, LanguageExt.SysX.Traits.ActivitySourceIO> ActivitySourceEff =>
            Eff<Runtime, LanguageExt.SysX.Traits.ActivitySourceIO>(rt => new ActivitySourceIO(rt.Env.Activity.ActivitySource));

        /// <summary>
        /// Access the console environment
        /// </summary>
        /// <returns>Console environment</returns>
        public Eff<Runtime, ConsoleIO> ConsoleEff =>
            SuccessEff(Sys.Live.ConsoleIO.Default);

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, FileIO> FileEff =>
            SuccessEff(Sys.Live.FileIO.Default);

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, DirectoryIO> DirectoryEff =>
            SuccessEff(Sys.Live.DirectoryIO.Default);

        /// <summary>
        /// Access the TextReader environment
        /// </summary>
        /// <returns>TextReader environment</returns>
        public Eff<Runtime, TextReadIO> TextReadEff =>
            SuccessEff(Sys.Live.TextReadIO.Default);

        /// <summary>
        /// Access the time environment
        /// </summary>
        /// <returns>Time environment</returns>
        public Eff<Runtime, TimeIO> TimeEff  =>
            SuccessEff(Sys.Live.TimeIO.Default);

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, EnvironmentIO> EnvironmentEff =>
            SuccessEff(Sys.Live.EnvironmentIO.Default);
    }
    
    public record RuntimeEnv(
        ActivityEnv Activity,
        CancellationTokenSource Source, 
        CancellationToken Token, 
        Encoding Encoding) : IDisposable
    {
        public RuntimeEnv(ActivityEnv activity, CancellationTokenSource source, Encoding encoding) : 
            this(activity,
                 source, 
                 source.Token, 
                 encoding)
        {
        }

        public void Dispose()
        {
            Activity.Dispose();
            Source.Dispose();
        }
    }
}
