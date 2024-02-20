using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test.Implementations;

/// <summary>
/// Test world interaction with the file-system
/// </summary>
/// <remarks>
/// Primarily used for testing (for use with TestRuntime or your own testing runtime)
/// </remarks>
public record FileIO(MemoryFS fs, DateTime now) : Sys.Traits.FileIO
{
    /// <summary>
    /// Copy file from one place to another
    /// </summary>
    public IO<Unit> Copy(string fromPath, string toPath, bool overwrite = false) =>
        lift(() => fs.CopyFile(fromPath, toPath, overwrite, now));

    /// <summary>
    /// Append lines to the end of a file
    /// </summary>
    public IO<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        lift(() => fs.AppendLines(path, lines, now));
        
    /// <summary>
    /// Read all lines from a file
    /// </summary>
    public IO<Seq<string>> ReadAllLines(string path, Encoding encoding) => 
        lift(() => fs.GetLines(path).ToSeq());
        
    /// <summary>
    /// Read all lines from a file
    /// </summary>
    public IO<byte[]> ReadAllBytes(string path) => 
        lift(() => fs.GetFile(path));

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    public IO<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        lift(() => fs.PutLines(path, lines, true, now));

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    public IO<Unit> WriteAllBytes(string path, byte[] data) =>
        lift(() => fs.PutFile(path, data, true, now));

    /// <summary>
    /// Read text from a file
    /// </summary>
    public IO<string> ReadAllText(string path, Encoding encoding) =>
        lift(() => fs.GetText(path));
        
    /// <summary>
    /// Write text to a file
    /// </summary>
    public IO<Unit> WriteAllText(string path, string text, Encoding encoding) =>
        lift(() => fs.PutText(path, text, true, now));

    /// <summary>
    /// Delete a file
    /// </summary>
    public IO<Unit> Delete(string path) =>
        lift(() => fs.Delete(path, now));

    /// <summary>
    /// True if a file at the path exists
    /// </summary>
    public IO<bool> Exists(string path) =>
        lift(() => fs.Exists(path));

    /// <summary>
    /// Open a text file
    /// </summary>
    public IO<TextReader> OpenText(string path) =>
        lift(() => new StringReader(fs.GetText(path)) as TextReader);

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    public IO<TextWriter> CreateText(string path) =>
        lift(() => new SimpleTextWriter(path, fs, now) as TextWriter);

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    public IO<TextWriter> AppendText(string path) =>
        lift(() =>
             {
                 var w = new SimpleTextWriter(path, fs, now);
                 w.Write(fs.GetText(path));
                 return w as TextWriter;
             });

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> OpenRead(string path) =>
        Open(path, FileMode.Open, FileAccess.Read);
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> Open(string path, FileMode mode) =>
        lift(() => fs.Open(path, mode, FileAccess.ReadWrite));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> Open(string path, FileMode mode, FileAccess access) =>
        lift(() => fs.Open(path, mode, access));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> OpenWrite(string path) =>
        Open(path, FileMode.Open, FileAccess.Write);
}
    
internal class SimpleTextWriter : StringWriter
{
    readonly string path;
    readonly MemoryFS fs;
    readonly DateTime now;

    public SimpleTextWriter(string path, MemoryFS fs, DateTime now) =>
        (this.path, this.fs, this.now) = (path, fs, now);

    protected override void Dispose(bool disposing) =>
        fs.PutText(path, ToString(), true, now);
}
