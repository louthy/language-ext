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
public static class File<RT>
    where RT : HasFile<RT>
{
    static readonly Eff<RT, FileIO> self =
        runtime<RT>().Bind(rt => rt.FileEff);
    
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
        self.Bind(e => e.Copy(fromPath, toPath, overwrite));

    /// <summary>
    /// Append lines to the end of the file provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Unit> appendAllLines(string path, IEnumerable<string> contents) =>
        from en in Enc<RT>.encoding
        from rs in self.Bind(e => e.AppendAllLines(path, contents, en))
        select rs;

    /// <summary>
    /// Read all of the lines from the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Seq<string>> readAllLines(string path) =>
        from en in Enc<RT>.encoding
        from rs in self.Bind(e => e.ReadAllLines(path, en))
        select rs;

    /// <summary>
    /// Write all of the lines to the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Unit> writeAllLines(string path, Seq<string> lines) =>
        from en in Enc<RT>.encoding
        from rs in self.Bind(e => e.WriteAllLines(path, lines, en))
        select rs;

    /// <summary>
    /// Read all of the text from the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, string> readAllText(string path) =>
        from en in Enc<RT>.encoding
        from rs in self.Bind(e => e.ReadAllText(path, en))
        select rs;

    /// <summary>
    /// Read all of the data from the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, byte[]> readAllBytes(string path) =>
        self.Bind(e => e.ReadAllBytes(path));

    /// <summary>
    /// Write all of the text to the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Unit> writeAllText(string path, string text) =>
        from en in Enc<RT>.encoding
        from rs in self.Bind(e => e.WriteAllText(path, text, en))
        select rs;

    /// <summary>
    /// Write all of the data to the path provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Unit> writeAllBytes(string path, byte[] data) =>
        self.Bind(e => e.WriteAllBytes(path, data));

    /// <summary>
    /// Delete the file provided
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, Unit> delete(string path) =>
        self.Bind(e => e.Delete(path));

    /// <summary>
    /// True if a file exists at the path
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, bool> exists(string path) =>
        self.Bind(e => e.Exists(path));

    /// <summary>
    /// Open a text file
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Producer<TextReader, Eff.R<RT>, Unit> openText(string path) => 
        from t in openTextInternal(path)
        from _ in Proxy.yield(t)
        select unit;

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, TextWriter> createText(string path) =>
        self.Bind(e => e.CreateText(path));

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Eff<RT, TextWriter> appendText(string path) =>
        self.Bind(e => e.AppendText(path));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public static Producer<Stream, Eff.R<RT>, Unit> openRead(string path) =>
        from s in openReadInternal(path)
        from _ in Proxy.yield(s)
        select unit;

    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Producer<Stream, Eff.R<RT>, Unit> open(string path, FileMode mode) =>
        from s in openInternal(path, mode)
        from _ in Proxy.yield(s)
        select unit;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Producer<Stream, Eff.R<RT>, Unit> open(string path, FileMode mode, FileAccess access) =>
        from s in openInternal(path, mode, access)
        from _ in Proxy.yield(s)
        select unit;
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    public static Producer<Stream, Eff.R<RT>, Unit> openWrite(string path) =>
        from s in openWriteInternal(path)
        from _ in Proxy.yield(s)
        select unit;

    // -- Internal ------------------------------------------------------------------------------------------------- 

    /// <summary>
    /// Open a file-stream
    /// </summary>
    static Eff<RT, Stream> openReadInternal(string path) =>
        self.Bind(e => Eff.use(e.OpenRead(path)));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    static Eff<RT, Stream> openInternal(string path, FileMode mode) =>
        self.Bind(e => Eff.use(e.Open(path, mode)));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    static Eff<RT, Stream> openInternal(string path, FileMode mode, FileAccess access) =>
        self.Bind(e => Eff.use(e.Open(path, mode, access)));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    static Eff<RT, Stream> openWriteInternal(string path) =>
        self.Bind(e => Eff.use(e.OpenWrite(path)));    

    /// <summary>
    /// Open a text file
    /// </summary>
    [Pure, MethodImpl(AffOpt.mops)]
    static Eff<RT, TextReader> openTextInternal(string path) =>
        self.Bind(e => Eff.use(e.OpenText(path)));
}
