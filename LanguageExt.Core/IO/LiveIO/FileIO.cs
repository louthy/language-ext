using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Threading;
using LanguageExt.Interfaces;
using LanguageExt;

namespace LanguageExt.LiveIO
{
    /// <summary>
    /// Real world interaction with the file-system
    /// </summary>
    public struct FileIO : Interfaces.FileIO
    {
        public static Interfaces.FileIO Default =
            new FileIO();
 
        /// <summary>
        /// Copy file from one place to another
        /// </summary>
        public Unit Copy(string fromPath, string toPath, bool overwrite = false)
        {
            File.Copy(fromPath, toPath, overwrite);
            return default;
        }

#if NETSTANDARD21
        /// <summary>
        /// Append lines to the end of a file
        /// </summary>
        public async ValueTask<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token)
        {
            await File.AppendAllLinesAsync(path, lines, encoding, token).ConfigureAwait(false);
            return default;
        }
#else
        /// <summary>
        /// Append lines to the end of a file
        /// </summary>
        public ValueTask<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token)
        {
            File.AppendAllLines(path, lines, encoding);
            return default;
        }
#endif

#if NETSTANDARD21
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        public async ValueTask<Seq<string>> ReadAllLines(string path, Encoding encoding, CancellationToken token) => 
            (await File.ReadAllLinesAsync(path, encoding, token).ConfigureAwait(false)).ToSeq();
#else
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        public ValueTask<Seq<string>> ReadAllLines(string path, Encoding encoding, CancellationToken token) => 
            File.ReadAllLines(path, encoding).ToSeq().AsValueTask();
#endif

#if NETSTANDARD21
        /// <summary>
        /// Write all lines to a file
        /// </summary>
        public async ValueTask<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token)
        {
            await File.WriteAllLinesAsync(path, lines, encoding, token).ConfigureAwait(false);
            return default;
        }
#else
        /// <summary>
        /// Write all lines to a file
        /// </summary>
        public ValueTask<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token)
        {
            File.WriteAllLines(path, lines, encoding);
            return default;
        }
#endif

#if NETSTANDARD21
        /// <summary>
        /// Read text from a file
        /// </summary>
        public async ValueTask<string> ReadAllText(string path, Encoding encoding, CancellationToken token) =>
            await File.ReadAllTextAsync(path, encoding, token).ConfigureAwait(false);
#else
        /// <summary>
        /// Read text from a file
        /// </summary>
        public ValueTask<string> ReadAllText(string path, Encoding encoding, CancellationToken token) =>
            File.ReadAllText(path, encoding).AsValueTask();
#endif

#if NETSTANDARD21
        /// <summary>
        /// Write text to a file
        /// </summary>
        public async ValueTask<Unit> WriteAllText(string path, string text, Encoding encoding, CancellationToken token)
        {
            await File.WriteAllTextAsync(path, text, encoding, token).ConfigureAwait(false);
            return default;
        }
#else
        /// <summary>
        /// Write text to a file
        /// </summary>
        public ValueTask<Unit> WriteAllText(string path, string text, Encoding encoding, CancellationToken token)
        {
            File.WriteAllText(path, text, encoding);
            return default;
        }
#endif

        /// <summary>
        /// Delete a file
        /// </summary>
        public Unit Delete(string path)
        {
            File.Delete(path);
            return default;
        }

        /// <summary>
        /// True if a file at the path exists
        /// </summary>
        public bool Exists(string path) =>
            File.Exists(path);

        /// <summary>
        /// Open a text file
        /// </summary>
        public TextReader OpenText(string path) =>
            File.OpenText(path);

        /// <summary>
        /// Create a new text file to stream to
        /// </summary>
        public TextWriter CreateText(string path) =>
            File.CreateText(path);

        /// <summary>
        /// Return a stream to append text to
        /// </summary>
        public TextWriter AppendText(string path) =>
            File.AppendText(path);
    }
}
