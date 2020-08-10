using System;
using System.Text;
using System.Threading;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    /// <summary>
    /// Live IO runtime
    /// </summary>
    public struct Runtime : 
        HasCancel<Runtime>,
        HasConsole<Runtime>,
        HasFile<Runtime>,
        HasEncoding<Runtime>,
        HasTextRead<Runtime>,
        HasTime<Runtime>
    {
        readonly CancellationTokenSource source;
        readonly CancellationToken token;
        readonly Encoding encoding;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        Runtime(Encoding encoding, CancellationTokenSource source) =>
            (this.source, this.token, this.encoding) = (source, source.Token, encoding);

        /// <summary>
        /// Ctor function
        /// </summary>
        public static Runtime New() =>
            new Runtime(System.Text.Encoding.Default, new CancellationTokenSource());

        /// <summary>
        /// Ctor function
        /// </summary>
        /// <param name="token">Cancellation token source</param>
        public static Runtime New(CancellationTokenSource source) =>
            new Runtime(System.Text.Encoding.Default, source);

        /// <summary>
        /// Ctor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        public static Runtime New(Encoding encoding) =>
            new Runtime(encoding, new CancellationTokenSource());

        /// <summary>
        /// Ctor function
        /// </summary>
        /// <param name="encoding">Text encoding</param>
        /// <param name="token">Cancellation token source</param>
        public static Runtime New(Encoding encoding, CancellationTokenSource source) =>
            new Runtime(encoding, source);

        /// <summary>
        /// Direct access to cancellation token
        /// </summary>
        public CancellationToken CancellationToken =>
            token;
        
        /// <summary>
        /// Directly access the cancellation token source
        /// </summary>
        /// <param name="runtime">Runtime</param>
        /// <returns>CancellationTokenSource</returns>
        public SIO<Runtime, CancellationTokenSource> CancellationTokenSource =>
            SIO<Runtime, CancellationTokenSource>.Effect(env => env.source);

#if !NETSTANDARD21
        /// <summary>
        /// Get the cancellation token
        /// </summary>
        /// <returns>CancellationToken</returns>
        public SIO<Runtime, CancellationToken> Token =>
            SIO<Runtime, CancellationToken>.Effect(env => env.CancellationToken);
#endif
        
        /// <summary>
        /// Access the console IO environment
        /// </summary>
        /// <returns>Console IO environment</returns>
        public IO<Runtime, ConsoleIO> ConsoleIO =>
            IO<Runtime, ConsoleIO>.Success(new LiveIO.ConsoleIO());

        /// <summary>
        /// Access the console SIO environment
        /// </summary>
        /// <returns>Console SIO environment</returns>
        public SIO<Runtime, ConsoleIO> ConsoleSIO =>
            SIO<Runtime, ConsoleIO>.Success(new LiveIO.ConsoleIO());

        /// <summary>
        /// Access the file IO environment
        /// </summary>
        /// <returns>File IO environment</returns>
        public IO<Runtime, FileIO> FileIO =>
            IO<Runtime, FileIO>.Success(new LiveIO.FileIO());
        
        /// <summary>
        /// Access the file SIO environment
        /// </summary>
        /// <returns>File SIO environment</returns>
        public SIO<Runtime, FileIO> FileSIO =>
            SIO<Runtime, FileIO>.Success(new LiveIO.FileIO());

        /// <summary>
        /// Access the TextReader IO environment
        /// </summary>
        /// <returns>TextReader IO environment</returns>
        public IO<Runtime, TextReadIO> TextReadIO =>
            IO<Runtime, TextReadIO>.Success(new LiveIO.TextReadIO());
        
        /// <summary>
        /// Access the TextReader SIO environment
        /// </summary>
        /// <returns>TextReader SIO environment</returns>
        public SIO<Runtime, TextReadIO> TextReadSIO =>
            SIO<Runtime, TextReadIO>.Success(new LiveIO.TextReadIO());

        /// <summary>
        /// Access the time IO environment
        /// </summary>
        /// <returns>Time IO environment</returns>
        public IO<Runtime, TimeIO> TimeIO =>
            IO<Runtime, TimeIO>.Success(new LiveIO.TimeIO());

        /// <summary>
        /// Access the time SIO environment
        /// </summary>
        /// <returns>Time SIO environment</returns>
        public SIO<Runtime, TimeIO> TimeSIO  =>
            SIO<Runtime, TimeIO>.Success(new LiveIO.TimeIO());

        /// <summary>
        /// Get encoding
        /// </summary>
        /// <returns></returns>
        public SIO<Runtime, Encoding> Encoding =>
            SIO<Runtime, Encoding>.Effect(env => env.encoding);
    }
}
