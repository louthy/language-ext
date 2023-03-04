#nullable enable

using System;
using System.Text;
using System.Threading;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live
{
    /// <summary>
    /// Live IO runtime
    /// </summary>
    public readonly struct Runtime : 
        HasCancel<Runtime>,
        HasConsole<Runtime>,
        HasFile<Runtime>,
        HasEncoding<Runtime>,
        HasTextRead<Runtime>,
        HasTime<Runtime>,
        HasEnvironment<Runtime>,
        HasDirectory<Runtime>,
        HasRandom<Runtime>
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
        /// <param name="seed">seed to used for the random generator</param>
        public static Runtime New(int? seed = default) =>
            new Runtime(new RuntimeEnv(new CancellationTokenSource(), Encoding.Default, seed));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="source">Cancellation token source</param>
        /// <param name="seed">seed to used for the random generator</param>
        public static Runtime New(CancellationTokenSource source, int? seed = default) =>
            new Runtime(new RuntimeEnv(source, Encoding.Default, seed));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="seed">seed to used for the random generator</param>
        public static Runtime New(Encoding encoding, int? seed = default) =>
            new Runtime(new RuntimeEnv(new CancellationTokenSource(), encoding, seed));

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="source">Cancellation token source</param>
        /// <param name="seed">seed to used for the random generator</param>
        public static Runtime New(Encoding encoding, CancellationTokenSource source, int? seed = default) =>
            new Runtime(new RuntimeEnv(source, encoding, seed));

        /// <summary>
        /// Create a new Runtime with a fresh cancellation token
        /// </summary>
        /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
        /// <returns>New runtime</returns>
        public Runtime LocalCancel =>
            new Runtime(new RuntimeEnv(new CancellationTokenSource(), Env.Encoding));

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
            SuccessEff(ConsoleIO.Default);

        /// <summary>
        /// Access the file environment
        /// </summary>
        /// <returns>File environment</returns>
        public Eff<Runtime, Traits.FileIO> FileEff =>
            SuccessEff(FileIO.Default);

        /// <summary>
        /// Access the directory environment
        /// </summary>
        /// <returns>Directory environment</returns>
        public Eff<Runtime, Traits.DirectoryIO> DirectoryEff =>
            SuccessEff(DirectoryIO.Default);

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
        public Eff<Runtime, Traits.TimeIO> TimeEff  =>
            SuccessEff(TimeIO.Default);

        /// <summary>
        /// Access the operating-system environment
        /// </summary>
        /// <returns>Operating-system environment environment</returns>
        public Eff<Runtime, Traits.EnvironmentIO> EnvironmentEff =>
            SuccessEff(EnvironmentIO.Default);

        /// <summary>
        /// Creates a new runtime from this with a new Random IO and optional seed
        /// </summary>
        /// <remarks>This is for sub-systems to run in their own local random/controlled random contexts</remarks>
        /// <returns>New runtime</returns>
        public Runtime LocalRandom(int? seed = default) =>
            new Runtime(env.LocalRandom(seed));

        /// <summary>
        /// Access the random synchronous effect environment
        /// </summary>
        /// <returns>Random synchronous effect environment</returns>
        public Eff<Runtime, Traits.RandomIO> RandomEff =>
            Eff<Runtime, Traits.RandomIO>(static rt => rt.env.Random);
    }
    
    public class RuntimeEnv
    {
        public readonly CancellationTokenSource Source;
        public readonly CancellationToken Token;
        public readonly Encoding Encoding;
        public readonly Traits.RandomIO Random;

        public RuntimeEnv(CancellationTokenSource source, CancellationToken token, Encoding encoding, int? seed = default)
        {
            Source   = source;
            Token    = token;
            Encoding = encoding;
            Random = RandomIO.New(seed);
        }

        public RuntimeEnv(CancellationTokenSource source, Encoding encoding, int? seed = default) : this(source, source.Token, encoding, seed)
        {
        }

        public RuntimeEnv LocalRandom(int? seed = default) =>
            new RuntimeEnv(Source, Token, Encoding, seed); 
    }
}
