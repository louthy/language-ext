using System.Collections.Generic;
using System.IO;
using System.Text;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits;

public interface FileIO
{
    /// <summary>
    /// Copy file from one place to another
    /// </summary>
    IO<Unit> Copy(string fromPath, string toPath, bool overwrite = false);

    /// <summary>
    /// Append lines to the end of a file
    /// </summary>
    IO<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding);
        
    /// <summary>
    /// Read all lines from a file
    /// </summary>
    IO<Seq<string>> ReadAllLines(string path, Encoding encoding);
        
    /// <summary>
    /// Read text from a file
    /// </summary>
    IO<string> ReadAllText(string path, Encoding encoding);
        
    /// <summary>
    /// Read text from a file
    /// </summary>
    IO<byte[]> ReadAllBytes(string path);

    /// <summary>
    /// Write text to a file
    /// </summary>
    IO<Unit> WriteAllText(string path, string lines, Encoding encoding);

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    IO<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding);

    /// <summary>
    /// Write text to a file
    /// </summary>
    IO<Unit> WriteAllBytes(string path, byte[] data);
        
    /// <summary>
    /// Delete a file
    /// </summary>
    IO<Unit> Delete(string path);
        
    /// <summary>
    /// True if a file at the path exists
    /// </summary>
    IO<bool> Exists(string path);

    /// <summary>
    /// Open a text file
    /// </summary>
    IO<TextReader> OpenText(string path);

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    IO<TextWriter> CreateText(string path);

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    IO<TextWriter> AppendText(string path);

    /// <summary>
    /// Open a file-stream
    /// </summary>
    IO<Stream> OpenRead(string path);

    /// <summary>
    /// Open a file-stream
    /// </summary>
    IO<Stream> Open(string path, FileMode mode);

    /// <summary>
    /// Open a file-stream
    /// </summary>
    IO<Stream> Open(string path, FileMode mode, FileAccess access);

    /// <summary>
    /// Open a file-stream
    /// </summary>
    IO<Stream> OpenWrite(string path);
}
    
/// <summary>
/// Type-class giving a struct the trait of supporting File IO
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasFile<RT> : HasIO<RT>, HasEncoding<RT>
    where RT : HasIO<RT>, HasEncoding<RT>
{
    /// <summary>
    /// Access the file synchronous effect environment
    /// </summary>
    /// <returns>File synchronous effect environment</returns>
    IO<FileIO> FileIO { get; }
}
