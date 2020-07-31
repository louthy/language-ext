using System.Text;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Interfaces;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Contains the common IO runtime.
    /// Derive from this type to extend the runtime and then pass that to your
    /// IO operations.
    /// </summary>
    public partial class Runtime : Cancellable
    {
        public static readonly Runtime Live = 
            new Runtime(
                CancellationToken.None, 
                Encoding.Default, 
                default(LiveIO.FileIO),
                default(LiveIO.TimeIO),
                default(LiveIO.ConsoleIO),
                default(LiveIO.TextReadIO));

        public CancellationToken CancelToken { get; }
        public readonly Encoding Encoding;
        public readonly FileIO File;
        public readonly TimeIO Time;
        public readonly ConsoleIO Console;
        public readonly TextReadIO TextRead;

        public Runtime(
            CancellationToken cancelToken,
            Encoding encoding,
            FileIO file,
            TimeIO time,
            ConsoleIO console,
            TextReadIO textRead
            )
        {
            CancelToken = cancelToken;
            Encoding = encoding;
            File = file;
            Time = time;
            Console = console;
            TextRead = textRead;
        }

        /// <summary>
        /// File environment from the runtime into the bound value
        /// </summary>
        public static readonly IO<Runtime, Runtime> env =
            IO.env<Runtime>();

        /// <summary>
        /// File environment from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, Runtime> senv =
            SIO.env<Runtime>();

        /// <summary>
        /// File environment from the runtime into the bound value
        /// </summary>
        public static readonly IO<Runtime, FileIO> fileIO =
            IO.env<Runtime>().Map(e => e.File);

        /// <summary>
        /// File environment from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, FileIO> fileSIO =
            SIO.env<Runtime>().Map(e => e.File);

       
        /// <summary>
        /// Time environment from the runtime into the bound value
        /// </summary>
        public static readonly IO<Runtime, TimeIO> timeIO =
            IO.env<Runtime>().Map(e => e.Time);

        /// <summary>
        /// Time environment from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, TimeIO> timeSIO =
            SIO.env<Runtime>().Map(e => e.Time);
        
        
        /// <summary>
        /// Console environment from the runtime into the bound value
        /// </summary>
        public static readonly IO<Runtime, ConsoleIO> consoleIO =
            IO.env<Runtime>().Map(e => e.Console);

        /// <summary>
        /// Console environment from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, ConsoleIO> consoleSIO =
            SIO.env<Runtime>().Map(e => e.Console);
        
        
        /// <summary>
        /// Stream reader 
        /// </summary>
        public static readonly IO<Runtime, TextReadIO> streamReadIO =
            IO.env<Runtime>().Map(e => e.TextRead);

        /// <summary>
        /// Stream reader 
        /// </summary>
        public static readonly SIO<Runtime, TextReadIO> streamReadSIO =
            SIO.env<Runtime>().Map(e => e.TextRead);
        
        /// <summary>
        /// Cancel token from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, CancellationToken> cancelEnv =
            SIO.env<Runtime>().Map(e => e.CancelToken);

        /// <summary>
        /// Text encoding from the runtime into the bound value
        /// </summary>
        public static readonly SIO<Runtime, Encoding> encodingEnv =
            SIO.env<Runtime>().Map(e => e.Encoding);
    }
}
