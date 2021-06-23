using System.IO;
using LanguageExt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Test
{
    /// <summary>
    /// Test world interaction with the file-system
    /// </summary>
    /// <remarks>
    /// Primarily used for testing (for use with TestRuntime or your own testing runtime)
    /// </remarks>
    public readonly struct FileIO : Sys.Traits.FileIO
    {
        readonly MemoryFS fs;

        public FileIO(MemoryFS fs) =>
            this.fs = fs;
        
        /// <summary>
        /// Copy file from one place to another
        /// </summary>
        public Unit Copy(string fromPath, string toPath, bool overwrite = false) =>
            fs.CopyFile(fromPath, toPath, overwrite);

        /// <summary>
        /// Append lines to the end of a file
        /// </summary>
        public ValueTask<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token) =>
            fs.AppendLines(path, lines).AsValueTask();
        
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        public ValueTask<Seq<string>> ReadAllLines(string path, Encoding encoding, CancellationToken token) => 
            fs.GetLines(path).ToSeq().AsValueTask();

        /// <summary>
        /// Write all lines to a file
        /// </summary>
        public ValueTask<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token) =>
            fs.PutLines(path, lines).AsValueTask();

        /// <summary>
        /// Read text from a file
        /// </summary>
        public ValueTask<string> ReadAllText(string path, Encoding encoding, CancellationToken token) =>
            fs.GetText(path).AsValueTask();
        
        /// <summary>
        /// Write text to a file
        /// </summary>
        public ValueTask<Unit> WriteAllText(string path, string text, Encoding encoding, CancellationToken token) =>
            fs.PutText(path, text).AsValueTask();

        /// <summary>
        /// Delete a file
        /// </summary>
        public Unit Delete(string path) =>
            fs.Delete(path);

        /// <summary>
        /// True if a file at the path exists
        /// </summary>
        public bool Exists(string path) =>
            fs.Exists(path);

        /// <summary>
        /// Open a text file
        /// </summary>
        public TextReader OpenText(string path) =>
            new StringReader(fs.GetText(path));

        /// <summary>
        /// Create a new text file to stream to
        /// </summary>
        public TextWriter CreateText(string path) =>
            new SimpleTextWriter(path, fs);

        /// <summary>
        /// Return a stream to append text to
        /// </summary>
        public TextWriter AppendText(string path)
        {
            var w = new SimpleTextWriter(path, fs);
            w.Write(fs.GetText(path));
            return w;
        }
    }
    
    internal class SimpleTextWriter : StringWriter
    {
        readonly string path;
        readonly MemoryFS fs;

        public SimpleTextWriter(string path, MemoryFS fs) =>
            (this.path, this.fs) = (path, fs);

        protected override void Dispose(bool disposing) =>
            fs.PutText(path, ToString());
    }
}
