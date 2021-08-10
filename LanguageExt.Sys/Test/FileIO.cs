using System;
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
        readonly DateTime now;

        public FileIO(MemoryFS fs, DateTime now) =>
            (this.fs, this.now) = (fs, now);
        
        /// <summary>
        /// Copy file from one place to another
        /// </summary>
        public Unit Copy(string fromPath, string toPath, bool overwrite = false) =>
            fs.CopyFile(fromPath, toPath, overwrite, now);

        /// <summary>
        /// Append lines to the end of a file
        /// </summary>
        public ValueTask<Unit> AppendAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token) =>
            fs.AppendLines(path, lines, now).AsValueTask();
        
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        public ValueTask<Seq<string>> ReadAllLines(string path, Encoding encoding, CancellationToken token) => 
            fs.GetLines(path).ToSeq().AsValueTask();
        
        /// <summary>
        /// Read all lines from a file
        /// </summary>
        public ValueTask<byte[]> ReadAllBytes(string path, CancellationToken token) => 
            fs.GetFile(path).AsValueTask();

        /// <summary>
        /// Write all lines to a file
        /// </summary>
        public ValueTask<Unit> WriteAllLines(string path, IEnumerable<string> lines, Encoding encoding, CancellationToken token) =>
            fs.PutLines(path, lines, true, now).AsValueTask();

        /// <summary>
        /// Write all lines to a file
        /// </summary>
        public ValueTask<Unit> WriteAllBytes(string path, byte[] data, CancellationToken token) =>
            fs.PutFile(path, data, true, now).AsValueTask();

        /// <summary>
        /// Read text from a file
        /// </summary>
        public ValueTask<string> ReadAllText(string path, Encoding encoding, CancellationToken token) =>
            fs.GetText(path).AsValueTask();
        
        /// <summary>
        /// Write text to a file
        /// </summary>
        public ValueTask<Unit> WriteAllText(string path, string text, Encoding encoding, CancellationToken token) =>
            fs.PutText(path, text, true, now).AsValueTask();

        /// <summary>
        /// Delete a file
        /// </summary>
        public Unit Delete(string path) =>
            fs.Delete(path, now);

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
            new SimpleTextWriter(path, fs, now);

        /// <summary>
        /// Return a stream to append text to
        /// </summary>
        public TextWriter AppendText(string path)
        {
            var w = new SimpleTextWriter(path, fs, now);
            w.Write(fs.GetText(path));
            return w;
        }

        /// <summary>
        /// Open a file-stream
        /// </summary>
        public FileStream OpenRead(string path) =>
            throw new NotImplementedException();
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        public FileStream Open(string path, FileMode mode) =>
            throw new NotImplementedException();
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        public FileStream Open(string path, FileMode mode, FileAccess access) =>
            throw new NotImplementedException();
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) =>
            throw new NotImplementedException();
        
        /// <summary>
        /// Open a file-stream
        /// </summary>
        public FileStream OpenWrite(string path) =>
            throw new NotImplementedException();
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
}
