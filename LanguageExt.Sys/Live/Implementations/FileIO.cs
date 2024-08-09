using System.Collections.Generic;
using System.IO;
using System.Text;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live.Implementations;

/// <summary>
/// Real world interaction with the file-system
/// </summary>
public record FileIO : Sys.Traits.FileIO
{
    public static readonly Sys.Traits.FileIO Default =
        new FileIO();

    /// <summary>
    /// Copy file from one place to another
    /// </summary>
    public IO<Unit> Copy(string fromPath, string toPath, bool overwrite = false) =>
        lift(() => File.Copy(fromPath, toPath, overwrite));

    /// <summary>
    /// Append lines to the end of a file
    /// </summary>
    public IO<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        liftIO(async env =>
               {
                   await File.AppendAllLinesAsync(path, lines, encoding, env.Token).ConfigureAwait(false);
                   return unit;
               });

    /// <summary>
    /// Read all lines from a file
    /// </summary>
    public IO<Seq<string>> ReadAllLines(string path, Encoding encoding) =>
        liftIO(async env => (await File.ReadAllLinesAsync(path, encoding, env.Token).ConfigureAwait(false)).AsIterable().ToSeq());

    /// <summary>
    /// Write all lines to a file
    /// </summary>
    public IO<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding) =>
        liftIO(async env =>
               {
                   await File.WriteAllLinesAsync(path, lines, encoding, env.Token).ConfigureAwait(false);
                   return unit;
               });
    
    /// <summary>
    /// Read text from a file
    /// </summary>
    public IO<string> ReadAllText(string path, Encoding encoding) =>
        liftIO(env => File.ReadAllTextAsync(path, encoding, env.Token));

    /// <summary>
    /// Read data from a file
    /// </summary>
    public IO<byte[]> ReadAllBytes(string path) =>
        liftIO(env => File.ReadAllBytesAsync(path, env.Token));

    /// <summary>
    /// Write text to a file
    /// </summary>
    public IO<Unit> WriteAllText(string path, string text, Encoding encoding) =>
        liftIO(async env =>
               {
                   await File.WriteAllTextAsync(path, text, encoding, env.Token).ConfigureAwait(false);
                   return unit;
               });

    /// <summary>
    /// Write data to a file
    /// </summary>
    public IO<Unit> WriteAllBytes(string path, byte[] data) =>
        liftIO(async env =>
               {
                   await File.WriteAllBytesAsync(path, data, env.Token).ConfigureAwait(false);
                   return unit;
               });

    /// <summary>
    /// Delete a file
    /// </summary>
    public IO<Unit> Delete(string path) =>
        lift(() => File.Delete(path));

    /// <summary>
    /// True if a file at the path exists
    /// </summary>
    public IO<bool> Exists(string path) =>
        lift(() => File.Exists(path));

    /// <summary>
    /// Open a text file
    /// </summary>
    public IO<TextReader> OpenText(string path) =>
        lift<TextReader>(() =>File.OpenText(path));

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    public IO<TextWriter> CreateText(string path) =>
        lift<TextWriter>(() =>File.CreateText(path));

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    public IO<TextWriter> AppendText(string path) =>
        lift<TextWriter>(() => File.AppendText(path));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<System.IO.Stream> OpenRead(string path) =>
        lift<System.IO.Stream>(() =>File.OpenRead(path));

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<System.IO.Stream> Open(string path, FileMode mode) =>
        lift<System.IO.Stream>(() => File.Open(path, mode));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<System.IO.Stream> Open(string path, FileMode mode, FileAccess access) =>
        lift<System.IO.Stream>(() =>File.Open(path, mode, access));
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    public IO<System.IO.Stream> OpenWrite(string path) =>
        lift<System.IO.Stream>(() =>File.OpenWrite(path));
}
