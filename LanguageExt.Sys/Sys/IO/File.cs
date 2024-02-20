using System.IO;
using LanguageExt.Pipes;
using LanguageExt.Sys.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.Sys.IO;

/// <summary>
/// File IO 
/// </summary>
public class File<M, RT>
    where M : Reader<M, RT>, Resource<M>, Monad<M>
    where RT : Has<M, FileIO>, HasEncoding
{
    static readonly K<M, FileIO> trait = 
        Reader.asksM<M, RT, FileIO>(e => e.Trait); 
    
    /// <summary>
    /// Copy file 
    /// </summary>
    /// <param name="fromPath">Source path</param>
    /// <param name="toPath">Destination path</param>
    /// <param name="overwrite">Overwrite if the file already exists at the destination</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <returns>Unit</returns>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> copy(string fromPath, string toPath, bool overwrite = false) =>
        trait.Bind(e => e.Copy(fromPath, toPath, overwrite));

    /// <summary>
    /// Append lines to the end of the file provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> appendAllLines(string path, IEnumerable<string> contents) =>
        from en in Enc<M, RT>.encoding
        from rs in trait.Bind(e => e.AppendAllLines(path, contents, en))
        select rs;

    /// <summary>
    /// Read all of the lines from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Seq<string>> readAllLines(string path) =>
        from en in Enc<M, RT>.encoding
        from rs in trait.Bind(e => e.ReadAllLines(path, en))
        select rs;

    /// <summary>
    /// Write all of the lines to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> writeAllLines(string path, Seq<string> lines) =>
        from en in Enc<M, RT>.encoding
        from rs in trait.Bind(e => e.WriteAllLines(path, lines, en))
        select rs;

    /// <summary>
    /// Read all of the text from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, string> readAllText(string path) =>
        from en in Enc<M, RT>.encoding
        from rs in trait.Bind(e => e.ReadAllText(path, en))
        select rs;

    /// <summary>
    /// Read all of the data from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, byte[]> readAllBytes(string path) =>
        trait.Bind(e => e.ReadAllBytes(path));

    /// <summary>
    /// Write all of the text to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> writeAllText(string path, string text) =>
        from en in Enc<M, RT>.encoding
        from rs in trait.Bind(e => e.WriteAllText(path, text, en))
        select rs;

    /// <summary>
    /// Write all of the data to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> writeAllBytes(string path, byte[] data) =>
        trait.Bind(e => e.WriteAllBytes(path, data));

    /// <summary>
    /// Delete the file provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> delete(string path) =>
        trait.Bind(e => e.Delete(path));

    /// <summary>
    /// True if a file exists at the path
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, bool> exists(string path) =>
        trait.Bind(e => e.Exists(path));

    /// <summary>
    /// Open a text file
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<TextReader, M, Unit> openText(string path) => 
        from t in openTextInternal(path)
        from _ in Proxy.yield(t)
        select unit;

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, TextWriter> createText(string path) =>
        trait.Bind(e => e.CreateText(path));

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, TextWriter> appendText(string path) =>
        trait.Bind(e => e.AppendText(path));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public static Producer<Stream, M, Unit> openRead(string path) =>
        from s in openReadInternal(path)
        from _ in Proxy.yield(s)
        select unit;

    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<Stream, M, Unit> open(string path, FileMode mode) =>
        from s in openInternal(path, mode)
        from _ in Proxy.yield(s)
        select unit;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<Stream, M, Unit> open(string path, FileMode mode, FileAccess access) =>
        from s in openInternal(path, mode, access)
        from _ in Proxy.yield(s)
        select unit;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<Stream, M, Unit> openWrite(string path) =>
        from s in openWriteInternal(path)
        from _ in Proxy.yield(s)
        select unit;

    // -- Internal ------------------------------------------------------------------------------------------------- 

    /// <summary>
    /// Open a file-stream
    /// </summary>
    static K<M, Stream> openReadInternal(string path) =>
        from io in trait.Map(e => e.OpenRead(path))
        from rs in Resource.use<M, Stream>(io)
        select rs;

    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    static K<M, Stream> openInternal(string path, FileMode mode) =>
        from io in trait.Map(e => e.Open(path, mode))
        from rs in Resource.use<M, Stream>(io)
        select rs;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    static K<M, Stream> openInternal(string path, FileMode mode, FileAccess access) =>
        from io in trait.Map(e => e.Open(path, mode, access))
        from rs in Resource.use<M, Stream>(io)
        select rs;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    static K<M, Stream> openWriteInternal(string path) =>
        from io in trait.Map(e => e.OpenWrite(path))
        from rs in Resource.use<M, Stream>(io)
        select rs;

    /// <summary>
    /// Open a text file
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    static K<M, TextReader> openTextInternal(string path) =>
        from io in trait.Map(e => e.OpenText(path))
        from rs in Resource.use<M, TextReader>(io)
        select rs;
}
