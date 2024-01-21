using System;
using System.Text;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live
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
            env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");
        
        /// <summary>
        /// Constructor function
        /// </summary>
        public static Runtime New() =>
            new (new RuntimeEnv(SynchronizationContext.Current, new CancellationTokenSource(), Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="source">Cancellation token source</param>
        public static Runtime New(CancellationTokenSource source) =>
            new (new RuntimeEnv(SynchronizationContext.Current, source, Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        public static Runtime New(Encoding encoding) =>
            new (new RuntimeEnv(SynchronizationContext.Current, new CancellationTokenSource(), encoding));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="syncContext">Thread synchronisation context</param>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        public static Runtime New(SynchronizationContext syncContext, Encoding encoding, CancellationTokenSource source) =>
            new (new RuntimeEnv(syncContext, source, encoding));
        
        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="syncContext">Thread synchronisation context</param>
        public static Runtime New(SynchronizationContext syncContext) =>
            new (new RuntimeEnv(syncContext, new CancellationTokenSource(), Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="syncContext">Thread synchronisation context</param>
        /// <param name="source">Cancellation token source</param>
        public static Runtime New(SynchronizationContext syncContext, CancellationTokenSource source) =>
            new (new RuntimeEnv(syncContext, source, Encoding.Default));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="syncContext">Thread synchronisation context</param>
        /// <param name="encoding">Text encoding</param>
        public static Runtime New(SynchronizationContext syncContext, Encoding encoding) =>
            new (new RuntimeEnv(syncContext, new CancellationTokenSource(), encoding));

        /// <summary>
        /// Create a new Runtime with a fresh cancellation token
        /// </summary>
        /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
        /// <returns>New runtime</returns>
        public Runtime LocalCancel =>
            new (new RuntimeEnv(Env.SyncContext, new CancellationTokenSource(), Env.Encoding));

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
            Pure(ConsoleIO.Default);

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, Traits.FileIO> FileEff =>
            Pure(FileIO.Default);

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, Traits.DirectoryIO> DirectoryEff =>
            Pure(DirectoryIO.Default);

        /// <summary>
        /// Access the TextReader environment
        /// </summary>
        /// <returns>TextReader environment</returns>
        public Eff<Runtime, Traits.TextReadIO> TextReadEff =>
            Pure(TextReadIO.Default);

        /// <summary>
        /// Access the time environment
        /// </summary>
        /// <returns>Time environment</returns>
        public Eff<Runtime, Traits.TimeIO> TimeEff  =>
            Pure(TimeIO.Default);

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, Traits.EnvironmentIO> EnvironmentEff =>
            Pure(EnvironmentIO.Default);
        
        /// <summary>
        /// Error mapper
        /// </summary>
        public Error FromError(Error error) =>
            error;

        /// <summary>
        /// Set the `SynchronizationContext`
        /// </summary>
        public Runtime WithSyncContext(SynchronizationContext syncContext) =>
            new (Env with { SyncContext = syncContext });

        /// <summary>
        /// Get the `SynchronizationContext`
        /// </summary>
        public SynchronizationContext SynchronizationContext =>
            Env.SyncContext;
    }
    
    public record RuntimeEnv(
        SynchronizationContext SyncContext,
        CancellationTokenSource Source,
        CancellationToken Token,
        Encoding Encoding)
    {
        public RuntimeEnv(SynchronizationContext syncContext, CancellationTokenSource source, Encoding encoding) : 
            this(syncContext, source, source.Token, encoding)
        { }
    }
}
