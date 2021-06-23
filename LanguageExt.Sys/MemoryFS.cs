using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LanguageExt.Sys
{
    /// <summary>
    /// Encapsulated in-memory file-system
    /// No public API exists for this.  Use Sys.IO.File.* to interact with the file-system
    /// </summary>
    /// <remarks>
    /// Primarily used for testing (for use with TestRuntime or your own testing runtime)
    /// 
    /// There is no public API for this.  If you want to leverage this in other types then
    /// you can access its internal functionality via:
    ///
    ///     new Sys.Test.FileIO(new MemoryFS())
    /// 
    /// </remarks>
    public class MemoryFS
    {
        readonly ConcurrentDictionary<string, byte[]> files = new();

        internal Unit Delete(string path)
        {
            files.TryRemove(path, out var _);
            return default;
        }

        internal bool Exists(string path) =>
            files.ContainsKey(path);

        internal byte[] GetFile(string path) =>
            files.TryGetValue(path, out var data)
                ? data
                : throw new FileNotFoundException("File not found", path);

        internal string GetText(string path) =>
            Encoding.UTF8.GetString(GetFile(path));

        internal string[] GetLines(string path) =>
            Encoding.UTF8.GetString(GetFile(path)).Split('\n');

        internal Unit PutFile(string path, byte[] data, bool overwrite = false)
        {
            files.AddOrUpdate(path, data, (_, _) => overwrite ? data : throw new IOException("Destination file already exists"));
            return default;
        }

        internal Unit PutText(string path, string text, bool overwrite = false) =>
            PutFile(path, Encoding.UTF8.GetBytes(text), overwrite);

        internal Unit PutLines(string path, IEnumerable<string> text, bool overwrite = false) =>
            PutText(path, string.Join("\n", text), overwrite);

        internal Unit Append(string path, byte[] data)
        {
            var pre = GetFile(path);
            var fin = new byte[pre.Length + data.Length];
            Array.Copy(pre, fin, pre.Length);
            Array.Copy(data, pre.Length, fin, 0, data.Length);
            return PutFile(path, fin, true);
        }

        internal Unit AppendText(string path, string text) =>
            PutText(path, GetText(path) + text, true);

        internal Unit AppendLines(string path, IEnumerable<string> lines) =>
            PutLines(path, GetLines(path).Concat(lines).ToArray(), true);

        internal Unit CopyFile(string src, string dest, bool overwrite = false) =>
            PutFile(dest, GetFile(src), overwrite);
    }
}
