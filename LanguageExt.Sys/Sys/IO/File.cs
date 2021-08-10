using System;
using System.IO;
using System.Buffers;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.TypeClasses;
using LanguageExt.Effects.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using Void = LanguageExt.Pipes.Void;
using LanguageExt.UnsafeValueAccess;
using System.Runtime.CompilerServices;

namespace LanguageExt.Sys.IO
{
    /// <summary>
    /// File IO 
    /// </summary>
    public static class File<RT>
        where RT : struct, HasFile<RT>
    {
        /// <summary>
        /// Copy file 
        /// </summary>
        /// <param name="fromPath">Source path</param>
        /// <param name="toPath">Destination path</param>
        /// <param name="overwrite">Overwrite if the file already exists at the destination</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <returns>Unit</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, Unit> copy(string fromPath, string toPath, bool overwrite = false) =>
            default(RT).FileEff.Map(e => e.Copy(fromPath, toPath, overwrite));

        /// <summary>
        /// Append lines to the end of the file provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> appendAllLines(string path, IEnumerable<string> contents) =>
            from ct in cancelToken<RT>()
            from en in Enc<RT>.encoding
            from rs in default(RT).FileEff.MapAsync(e => e.AppendAllLines(path, contents, en, ct))
            select rs;

        /// <summary>
        /// Read all of the lines from the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Seq<string>> readAllLines(string path) =>
            from ct in cancelToken<RT>()
            from en in Enc<RT>.encoding
            from rs in default(RT).FileEff.MapAsync(e => e.ReadAllLines(path, en, ct))
            select rs;

        /// <summary>
        /// Write all of the lines to the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> writeAllLines(string path, Seq<string> lines) =>
            from ct in cancelToken<RT>()
            from en in Enc<RT>.encoding
            from rs in default(RT).FileEff.MapAsync(e => e.WriteAllLines(path, lines, en, ct))
            select rs;

        /// <summary>
        /// Read all of the text from the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, string> readAllText(string path) =>
            from ct in cancelToken<RT>()
            from en in Enc<RT>.encoding
            from rs in default(RT).FileEff.MapAsync(e => e.ReadAllText(path, en, ct))
            select rs;

        /// <summary>
        /// Read all of the data from the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, byte[]> readAllBytes(string path) =>
            from ct in cancelToken<RT>()
            from rs in default(RT).FileEff.MapAsync(e => e.ReadAllBytes(path, ct))
            select rs;

        /// <summary>
        /// Write all of the text to the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> writeAllText(string path, string text) =>
            from ct in cancelToken<RT>()
            from en in Enc<RT>.encoding
            from rs in default(RT).FileEff.MapAsync(e => e.WriteAllText(path, text, en, ct))
            select rs;

        /// <summary>
        /// Write all of the data to the path provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> writeAllBytes(string path, byte[] data) =>
            from ct in cancelToken<RT>()
            from rs in default(RT).FileEff.MapAsync(e => e.WriteAllBytes(path, data, ct))
            select rs;

        /// <summary>
        /// Delete the file provided
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, Unit> delete(string path) =>
            default(RT).FileEff.Map(e => e.Delete(path));

        /// <summary>
        /// True if a file exists at the path
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, bool> exists(string path) =>
            default(RT).FileEff.Map(e => e.Exists(path));

        /// <summary>
        /// Open a text file
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Producer<RT, TextReader, Unit> openText(string path) =>
            from t in Proxy.use(openTextInternal(path))
            from _ in Proxy.yield(t)
            select unit;

        /// <summary>
        /// Create a new text file to stream to
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, TextWriter> createText(string path) =>
            default(RT).FileEff.Map(e => e.CreateText(path));

        /// <summary>
        /// Return a stream to append text to
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, TextWriter> appendText(string path) =>
            default(RT).FileEff.Map(e => e.AppendText(path));

        /// <summary>
        /// Open a file-stream
        /// </summary>
        public static Producer<RT, Stream, Unit> openRead(string path) =>
            from s in Proxy.use(openReadInternal(path))
            from _ in Proxy.yield(s as Stream)
            select unit;

        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Producer<RT, Stream, Unit> open(string path, FileMode mode) =>
            from s in Proxy.use(openInternal(path, mode))
            from _ in Proxy.yield(s as Stream)
            select unit;
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Producer<RT, Stream, Unit> open(string path, FileMode mode, FileAccess access) =>
            from s in Proxy.use(openInternal(path, mode, access))
            from _ in Proxy.yield(s as Stream)
            select unit;
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Producer<RT, Stream, Unit> open(string path, FileMode mode, FileAccess access, FileShare share) =>
            from s in Proxy.use(openInternal(path, mode, access, share))
            from _ in Proxy.yield(s as Stream)
            select unit;
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Producer<RT, Stream, Unit> openWrite(string path) =>
            from s in Proxy.use(openWriteInternal(path))
            from _ in Proxy.yield(s as Stream)
            select unit;

        // -- Internal ------------------------------------------------------------------------------------------------- 

        /// <summary>
        /// Open a file-stream
        /// </summary>
        static Eff<RT, FileStream> openReadInternal(string path) =>
            default(RT).FileEff.Map(e => e.OpenRead(path));

        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        static Eff<RT, FileStream> openInternal(string path, FileMode mode) =>
            default(RT).FileEff.Map(e => e.Open(path, mode));
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        static Eff<RT, FileStream> openInternal(string path, FileMode mode, FileAccess access) =>
            default(RT).FileEff.Map(e => e.Open(path, mode, access));

        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        static Eff<RT, FileStream> openInternal(string path, FileMode mode, FileAccess access, FileShare share) =>
            default(RT).FileEff.Map(e => e.Open(path, mode, access));
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        static Eff<RT, FileStream> openWriteInternal(string path) =>
            default(RT).FileEff.Map(e => e.OpenWrite(path));    

        /// <summary>
        /// Open a text file
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        static Eff<RT, TextReader> openTextInternal(string path) =>
            default(RT).FileEff.Map(e => e.OpenText(path));
    }
}
