using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    public interface FileIO
    {
        /// <summary>
        /// Copy file from one place to another
        /// </summary>
        Unit Copy(string fromPath, string toPath, bool overwrite = false);

        /// <summary>
        /// Append lines to the end of a file
        /// </summary>
        ValueTask<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token);
        
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        ValueTask<Seq<string>> ReadAllLines(string path, Encoding encoding, CancellationToken token);
        
        /// <summary>
        /// Read text from a file
        /// </summary>
        ValueTask<string> ReadAllText(string path, Encoding encoding, CancellationToken token);
        
        /// <summary>
        /// Read text from a file
        /// </summary>
        ValueTask<byte[]> ReadAllBytes(string path, CancellationToken token);

        /// <summary>
        /// Write text to a file
        /// </summary>
        ValueTask<Unit> WriteAllText(string path, string lines, Encoding encoding, CancellationToken token);

        /// <summary>
        /// Write all lines to a file
        /// </summary>
        ValueTask<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token);

        /// <summary>
        /// Write text to a file
        /// </summary>
        ValueTask<Unit> WriteAllBytes(string path, byte[] data, CancellationToken token);
        
        /// <summary>
        /// Delete a file
        /// </summary>
        Unit Delete(string path);
        
        /// <summary>
        /// True if a file at the path exists
        /// </summary>
        bool Exists(string path);

        /// <summary>
        /// Open a text file
        /// </summary>
        TextReader OpenText(string path);

        /// <summary>
        /// Create a new text file to stream to
        /// </summary>
        TextWriter CreateText(string path);

        /// <summary>
        /// Return a stream to append text to
        /// </summary>
        TextWriter AppendText(string path);

        /// <summary>
        /// Open a file-stream
        /// </summary>
        FileStream OpenRead(string path);

        /// <summary>
        /// Open a file-stream
        /// </summary>
        FileStream Open(string path, FileMode mode);

        /// <summary>
        /// Open a file-stream
        /// </summary>
        FileStream Open(string path, FileMode mode, FileAccess access);

        /// <summary>
        /// Open a file-stream
        /// </summary>
        FileStream Open(string path, FileMode mode, FileAccess access, FileShare share);

        /// <summary>
        /// Open a file-stream
        /// </summary>
        FileStream OpenWrite(string path);
    }
    
    /// <summary>
    /// Type-class giving a struct the trait of supporting File IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasFile<RT> : HasCancel<RT>, HasEncoding<RT>
        where RT : struct, HasCancel<RT>, HasEncoding<RT>
    {
        /// <summary>
        /// Access the file synchronous effect environment
        /// </summary>
        /// <returns>File synchronous effect environment</returns>
        Eff<RT, FileIO> FileEff { get; }
    }
}
