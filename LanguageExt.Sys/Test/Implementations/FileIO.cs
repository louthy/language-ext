using System.IO;
using System.Text;
using System.Collections.Generic;

namespace LanguageExt.Sys.Test.Implementations;

/// <summary>
/// Test world interaction with the file-system
/// </summary>
/// <remarks>
/// Primarily used for testing (for use with TestRuntime or your own testing runtime)
/// </remarks>
public record FileIO(string root) : Sys.Traits.FileIO
{
    string FixPath(string path) =>
        Path.Combine(root, path.Replace(":", "_drive"));
    
    /// <summary>
    /// Copy file from one place to another
    /// </summary>
    public IO<Unit> Copy(string fromPath, string toPath, bool overwrite = false) =>
        Live.Implementations.FileIO.Default.Copy(FixPath(fromPath), FixPath(toPath), overwrite);

    /// <summary>
    /// Append lines to the end of a file
    /// </summary>
    public IO<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        Live.Implementations.FileIO.Default.AppendAllLines(FixPath(path), lines, encoding);
        
    /// <summary>
    /// Read all lines from a file
    /// </summary>
    public IO<Seq<string>> ReadAllLines(string path, Encoding encoding) => 
        Live.Implementations.FileIO.Default.ReadAllLines(FixPath(path), encoding);
        
    /// <summary>
    /// Read all lines from a file
    /// </summary>
    public IO<byte[]> ReadAllBytes(string path) => 
        Live.Implementations.FileIO.Default.ReadAllBytes(FixPath(path));

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    public IO<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        Live.Implementations.FileIO.Default.WriteAllLines(FixPath(path), lines, encoding);

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    public IO<Unit> WriteAllBytes(string path, byte[] data) =>
        Live.Implementations.FileIO.Default.WriteAllBytes(FixPath(path), data);

    /// <summary>
    /// Read text from a file
    /// </summary>
    public IO<string> ReadAllText(string path, Encoding encoding) =>
        Live.Implementations.FileIO.Default.ReadAllText(FixPath(path), encoding);
        
    /// <summary>
    /// Write text to a file
    /// </summary>
    public IO<Unit> WriteAllText(string path, string text, Encoding encoding) =>
        Live.Implementations.FileIO.Default.WriteAllText(FixPath(path), text, encoding);

    /// <summary>
    /// Delete a file
    /// </summary>
    public IO<Unit> Delete(string path) =>
        Live.Implementations.FileIO.Default.Delete(FixPath(path));

    /// <summary>
    /// True if a file at the path exists
    /// </summary>
    public IO<bool> Exists(string path) =>
        Live.Implementations.FileIO.Default.Exists(FixPath(path));

    /// <summary>
    /// Open a text file
    /// </summary>
    public IO<TextReader> OpenText(string path) =>
        Live.Implementations.FileIO.Default.OpenText(FixPath(path));

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    public IO<TextWriter> CreateText(string path) =>
        Live.Implementations.FileIO.Default.CreateText(FixPath(path));

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    public IO<TextWriter> AppendText(string path) =>
        Live.Implementations.FileIO.Default.AppendText(FixPath(path));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> OpenRead(string path) =>
        Live.Implementations.FileIO.Default.OpenRead(FixPath(path));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> Open(string path, FileMode mode) =>
        Live.Implementations.FileIO.Default.Open(FixPath(path), mode);
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> Open(string path, FileMode mode, FileAccess access) =>
        Live.Implementations.FileIO.Default.Open(FixPath(path), mode, access);
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<Stream> OpenWrite(string path) =>
        Live.Implementations.FileIO.Default.OpenWrite(FixPath(path));
}
