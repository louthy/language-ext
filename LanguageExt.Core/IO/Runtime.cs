using System;
using System.Text;
using System.Threading;
using LanguageExt.Interfaces;

using static LanguageExt.Prelude;

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
        HasTime<Runtime>,
        HasAtom<Runtime>
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
        /// Run the provided Aff in a local context that has a new CancellationTokenSource and token
        /// </summary>
        /// <remarks>This is for sub-systems to run in their own local cancellation contexts</remarks>
        /// <param name="ma">Operation to run in context</param>
        /// <returns>New runtime</returns>
        public Runtime LocalCancel =>
            new Runtime(encoding, new CancellationTokenSource());
        
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
        public Eff<Runtime, CancellationTokenSource> CancellationTokenSource =>
            Eff<Runtime, CancellationTokenSource>(env => env.source);

#if !NETSTANDARD21
        /// <summary>
        /// Get the cancellation token
        /// </summary>
        /// <returns>CancellationToken</returns>
        public Eff<Runtime, CancellationToken> Token =>
            Eff<Runtime, CancellationToken>(env => env.CancellationToken);
#endif
        
        /// <summary>
        /// Access the console IO environment
        /// </summary>
        /// <returns>Console IO environment</returns>
        public Aff<Runtime, ConsoleIO> ConsoleAff =>
            SuccessAff(LiveIO.ConsoleIO.Default);

        /// <summary>
        /// Access the console SIO environment
        /// </summary>
        /// <returns>Console SIO environment</returns>
        public Eff<Runtime, ConsoleIO> ConsoleEff =>
            SuccessEff(LiveIO.ConsoleIO.Default);

        /// <summary>
        /// Access the file add environment
        /// </summary>
        /// <returns>File aff environment</returns>
        public Aff<Runtime, FileIO> FileAff =>
            SuccessAff(LiveIO.FileIO.Default);
        
        /// <summary>
        /// Access the file eff environment
        /// </summary>
        /// <returns>File eff environment</returns>
        public Eff<Runtime, FileIO> FileEff =>
            SuccessEff(LiveIO.FileIO.Default);

        /// <summary>
        /// Access the atom aff environment
        /// </summary>
        /// <returns>File IO environment</returns>
        public Aff<Runtime, AtomIO> AtomAff =>
            SuccessAff(LiveIO.AtomIO.Default);
        
        /// <summary>
        /// Access the atom eff environment
        /// </summary>
        /// <returns>File eff environment</returns>
        public Eff<Runtime, AtomIO> AtomEff =>
            SuccessEff(LiveIO.AtomIO.Default);

        /// <summary>
        /// Access the TextReader IO environment
        /// </summary>
        /// <returns>TextReader IO environment</returns>
        public Aff<Runtime, TextReadIO> TextReadAff =>
            SuccessAff(LiveIO.TextReadIO.Default);
        
        /// <summary>
        /// Access the TextReader SIO environment
        /// </summary>
        /// <returns>TextReader SIO environment</returns>
        public Eff<Runtime, TextReadIO> TextReadEff =>
            SuccessEff(LiveIO.TextReadIO.Default);

        /// <summary>
        /// Access the time IO environment
        /// </summary>
        /// <returns>Time IO environment</returns>
        public Aff<Runtime, TimeIO> TimeAff =>
            SuccessAff(LiveIO.TimeIO.Default);

        /// <summary>
        /// Access the time SIO environment
        /// </summary>
        /// <returns>Time SIO environment</returns>
        public Eff<Runtime, TimeIO> TimeEff  =>
            SuccessEff(LiveIO.TimeIO.Default);

        /// <summary>
        /// Get encoding
        /// </summary>
        /// <returns></returns>
        public Eff<Runtime, Encoding> Encoding =>
            Eff<Runtime, Encoding>(env => env.encoding);
    }
}
