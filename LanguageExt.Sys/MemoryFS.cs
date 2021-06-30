using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys
{
    /// <summary>
    /// Encapsulated in-memory file-system
    /// No public API exists for this other than for adding and getting the logical in-memory drives
    /// </summary>
    /// <remarks>
    /// Primarily used for testing (for use with Sys.Test.Runtime or your own runtimes)
    ///
    /// This isn't anywhere near as strict as the real file-system, and so it shouldn't really be used to test
    /// file-operations.  It should be used to test simple access to files without having to create them for
    /// real, or worry about what drives exist.  Error messages shouldn't be relied on, only success and failure.
    /// </remarks>
    public class MemoryFS
    {
        readonly Folder machine = new Folder("[root]", DateTime.MinValue);
        readonly static char[] invalidPath = Path.GetInvalidPathChars();
        readonly static char[] invalidFile = Path.GetInvalidFileNameChars();
        public string CurrentDir = "C:\\";

        public MemoryFS() =>
            AddLogicalDrive("C");

        Seq<string> ParsePath(string path) =>
            System.IO.Path.IsPathRooted(path)
                ? path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                      .ToSeq()
                :throw new IOException($"Path not rooted: {path}");

        /// <summary>
        /// Get the logical in-memory drives 
        /// </summary>
        /// <returns>Sequence of drive names</returns>
        public Seq<string> GetLogicalDrives() =>
            EnumerateFolders("[root]", "*", SearchOption.TopDirectoryOnly).ToSeq();
        
        /// <summary>
        /// Add a logical in-memory drive
        /// </summary>
        public Unit AddLogicalDrive(string name) =>
            CreateFolder($"{name.TrimEnd(':')}:", DateTime.MinValue);

        internal IEnumerable<string> EnumerateFolders(string path, string searchPattern, SearchOption option)
        {
            var regex = MakePathSearchRegex(searchPattern);
            return Folder.EnumerateFolders(machine, path, regex, option);
        }

        internal IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption option)
        {
            var regex = MakePathSearchRegex(searchPattern);
            return Folder.EnumerateFiles(machine, path, regex, option);
        }

        // TODO: This will probably need some finessing
        static Regex MakePathSearchRegex(string searchPattern) =>
            new Regex(MakeAnchor(
                          searchPattern.Replace("\\", "\\\\")
                                       .Replace(".", "\\.")
                                       .Replace("$", "\\$")
                                       .Replace("^", "\\^")
                                       .Replace("+", "\\+")
                                       .Replace("{", "\\{")
                                       .Replace("}", "\\}")
                                       .Replace("[", "\\[")
                                       .Replace("]", "\\]")
                                       .Replace("(", "\\(")
                                       .Replace(")", "\\)")
                                       .Replace("|", "\\|")
                                       .Replace("?", ".??")
                                       .Replace("*", ".*?")));

        static string MakeAnchor(string p) =>
            $"^{p}$";

        internal Unit CreateFolder(string path, DateTime now)
        {
            Folder.PutFolder(machine, path, now);
            return default;
        }

        internal bool FolderExists(string path) =>
            Folder.FolderExists(machine, path);

        internal string AssertFolderExists(string path)
        {
            var e = Folder.FolderExists(machine, path);
            return e
                       ? path
                       : throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        internal Unit DeleteFolder(string path, bool recursive) =>
            Folder.DeleteFolder(machine, path, recursive);

        internal Unit SetFolderCreationTime(string path, DateTime dt) =>
            ignore(Folder.Map(machine, path, f => f.CreationTime = dt));

        internal Unit SetFolderLastAccessTime(string path, DateTime dt) =>
            ignore(Folder.Map(machine, path, f => f.LastAccessTime = dt));

        internal Unit SetFolderLastWriteTime(string path, DateTime dt) =>
            ignore(Folder.Map(machine, path, f => f.LastWriteTime = dt));

        internal DateTime GetFolderCreationTime(string path) =>
            Folder.Map(machine, path, f => f.CreationTime);

        internal DateTime GetFolderLastAccessTime(string path) =>
            Folder.Map(machine, path, f => f.LastAccessTime);

        internal DateTime GetFolderLastWriteTime(string path) =>
            Folder.Map(machine, path, f => f.LastWriteTime);
        
        internal Unit Delete(string path) =>
            Folder.DeleteFile(machine, path);

        internal bool Exists(string path) =>
            Folder.FileExists(machine, path);

        internal byte[] GetFile(string path) =>
            Folder.GetFile(machine, path);

        internal string GetText(string path) =>
            Encoding.UTF8.GetString(GetFile(path));

        internal string[] GetLines(string path) =>
            Encoding.UTF8.GetString(GetFile(path)).Split('\n');

        internal Unit PutFile(string path, byte[] data, bool overwrite = false) =>
            Folder.PutFile(machine, path, data, overwrite);

        internal Unit PutText(string path, string text, bool overwrite = false) =>
            PutFile(path, Encoding.UTF8.GetBytes(text), overwrite);

        internal Unit PutLines(string path, IEnumerable<string> text, bool overwrite = false) =>
            PutText(path, string.Join("\n", text), overwrite);

        internal Unit Append(string path, byte[] data)
        {
            var pre = GetFile(path);
            var fin = new byte[pre.Length + data.Length];
            System.Array.Copy(pre, fin, pre.Length);
            System.Array.Copy(data, pre.Length, fin, 0, data.Length);
            return PutFile(path, fin, true);
        }

        internal Unit AppendText(string path, string text) =>
            PutText(path, GetText(path) + text, true);

        internal Unit AppendLines(string path, IEnumerable<string> lines) =>
            PutLines(path, GetLines(path).Concat(lines).ToArray(), true);

        internal Unit CopyFile(string src, string dest, bool overwrite = false) =>
            PutFile(dest, GetFile(src), overwrite);
        
        class Folder
        {
            public string Name;
            public DateTime CreationTime;
            public DateTime LastAccessTime;
            public DateTime LastWriteTime;
            readonly object sync = new();

            readonly ConcurrentDictionary<string, byte[]> Files = new (StringComparer.InvariantCultureIgnoreCase);
            readonly ConcurrentDictionary<string, Folder> Folders = new (StringComparer.InvariantCultureIgnoreCase);
            
            public Folder(string name, DateTime now)
            {
                Name           = name;
                CreationTime   = now;
                LastAccessTime = now;
                LastWriteTime  = now;
            }

            public override string ToString() =>
                Name;

            public static A Map<A>(Folder root, string path, Func<Folder, A> map) =>
                Map1(root, ParseFolderPath(path), path, map);

            static A Map1<A>(Folder f, Seq<string> path, string fullPath, Func<Folder, A> map) =>
                path.Length switch
                {
                    0 => Map2(f, map),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? Map1(child, path.Tail, fullPath, map)
                             : throw new DirectoryNotFoundException($"Invalid path: {fullPath}")
                };

            static A Map2<A>(Folder f, Func<Folder, A> map) =>
                map(f);

            public static Unit PutFile(Folder root, string path, byte[] data, bool overwrite) =>
                PutFile(root, ParseFilePath(path), path, data, overwrite);

            static Unit PutFile(Folder f, Seq<string> path, string fullPath, byte[] data, bool overwrite) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Invalid path: {fullPath}"),
                    1 => AddFile(f, path.Head, data, overwrite),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                            ? PutFile(child, path.Tail, fullPath, data, overwrite)
                            : throw new DirectoryNotFoundException($"Invalid path: {fullPath}")
                };

            static Unit AddFile(Folder f, string name, byte[] data, bool overwrite)
            {
                if (f.Folders.ContainsKey(name)) throw new IOException($"Directory already exists with the same name: {name}");
                f.Files.AddOrUpdate(name, data, (_,_) => overwrite ? data : throw new IOException("Destination file already exists"));
                return default;
            }

            public static Unit DeleteFile(Folder root, string path) =>
                DeleteFile(root, ParseFilePath(path), path);

            static Unit DeleteFile(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => throw new FileNotFoundException($"File not found: {fullPath}"),
                    1 => DeleteFile1(f, path.Head),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? DeleteFile(child, path.Tail, fullPath)
                             : throw new FileNotFoundException($"File not found: {fullPath}")
                };

            static Unit DeleteFile1(Folder f, string name)
            {
                f.Files.TryRemove(name, out var _);
                return default;
            }

            public static byte[] GetFile(Folder root, string path) =>
                GetFile(root, ParseFilePath(path), path);

            static byte[] GetFile(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => throw new FileNotFoundException($"File not found: {fullPath}"),
                    1 => GetFile1(f, path.Head, fullPath),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? GetFile(child, path.Tail, fullPath)
                             : throw new FileNotFoundException($"File not found: {fullPath}")
                };

            static byte[] GetFile1(Folder f, string name, string fullPath) =>
                f.Files.TryGetValue(name, out var d)
                    ? d
                    : throw new FileNotFoundException($"File not found: {fullPath}");
            
            public static bool FileExists(Folder root, string path) =>
                FileExists(root, ParseFilePath(path), path);

            static bool FileExists(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => false,
                    1 => FileExists1(f, path.Head, fullPath),
                    _ => f.Folders.TryGetValue(path[0], out var child) && FileExists(child, path.Tail, fullPath)
                };

            static bool FileExists1(Folder f, string name, string fullPath) =>
                f.Files.ContainsKey(name);

            public static Folder PutFolder(Folder root, string path, DateTime now) =>
                PutFolder(root, ParseFolderPath(path), path, now);

            static Folder PutFolder(Folder f, Seq<string> path, string fullPath, DateTime now) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Invalid path: {fullPath}"),
                    1 => AddFolder(f, path.Head, now),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? PutFolder(child, path.Tail, fullPath, now)
                             : PutFolder(AddFolder(f, path.Head, now), path.Tail, fullPath, now)
                };

            static Folder AddFolder(Folder f, string name, DateTime now)
            {
                if (f.Files.ContainsKey(name)) throw new IOException($"File already exists with the same name: {name}");
                return f.Folders.AddOrUpdate(name, new Folder(name, now), (_, current) => current);
            }
            
            public static Unit DeleteFolder(Folder root, string path, bool recursive) =>
                DeleteFolder(root, ParseFolderPath(path), path, recursive);

            static Unit DeleteFolder(Folder f, Seq<string> path, string fullPath, bool recursive) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Directory not found: {fullPath}"),
                    1 => DeleteFolder1(f, path.Head, fullPath, recursive),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? DeleteFolder(child, path.Tail, fullPath, recursive)
                             : throw new DirectoryNotFoundException($"Directory not found: {fullPath}")
                };

            static Unit DeleteFolder1(Folder f, string name, string fullPath, bool recursive)
            {
                lock (f.sync)
                {
                    if (f.Folders.TryGetValue(name, out var fremove))
                    {
                        if (recursive)
                        {
                            f.Folders.TryRemove(name, out var _);
                            foreach (var child in fremove.Folders)
                            {
                                DeleteFolder1(fremove, child.Key, fullPath, recursive);
                            }
                        }
                        else
                        {
                            if (fremove.Folders.Count > 0)
                            {
                                throw new IOException("Directory not empty");
                            }
                            else
                            {
                                f.Folders.TryRemove(name, out var _);
                            }
                        }
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
                    }
                }

                return default;
            }
            
            public static bool FolderExists(Folder root, string path) =>
                FolderExists(root, ParseFolderPath(path), path);

            static bool FolderExists(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => true,
                    _ => f.Folders.TryGetValue(path[0], out var child) && FolderExists(child, path.Tail, fullPath)
                };

            static Seq<string> ParseFolderPath(string path) =>
                ValidatePathNames(path.TrimEnd(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                                      .Split(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                                      .ToSeq());
            
            static Seq<string> ParseFilePath(string path) =>
                ValidatePathNames(path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                                       .ToSeq());

            static Seq<string> ValidatePathNames(Seq<string> path)
            {
                if (path.IsEmpty) throw new IOException($"Invalid path: {string.Join("\\", path)}");
                if (path.Head.Exists(invalidPath.Contains)) throw new IOException($"Invalid path: {string.Join("\\", path)}");
                foreach (var name in path.Tail)
                {
                    if (name.Exists(invalidFile.Contains)) throw new IOException($"Invalid path: {string.Join("\\", path)}");
                }
                return path;
            }

            public static IEnumerable<string> EnumerateFolders(Folder f, string path, Regex regex, SearchOption option) =>
                EnumerateFolders(f, ParseFolderPath(path), path, regex, option);
            
            static IEnumerable<string> EnumerateFolders(Folder f, Seq<string> path, string fullPath, Regex regex, SearchOption option) =>
                path.Length switch
                {
                    0 => EnumerateFolders1(f, fullPath, regex, option),
                    _ => f.Folders.TryGetValue(path[0], out var child) 
                            ? EnumerateFolders(child, path.Tail, fullPath, regex, option)
                            : throw new DirectoryNotFoundException($"Directory not found: {fullPath}")
                };

            static IEnumerable<string> EnumerateFolders1(Folder f, string fullPath, Regex regex, SearchOption option)
            {
                return Yield(f, fullPath);

                IEnumerable<string> Yield(Folder f, string p)
                {
                    foreach (var c in f.Folders)
                    {
                        var np = Path.Combine(p, c.Value.Name);
                        if (regex.IsMatch(c.Value.Name))
                        {
                            yield return np;
                        }

                        if (option == SearchOption.AllDirectories)
                        {
                            foreach (var ic in Yield(c.Value, np))
                            {
                                yield return ic;
                            }
                        }
                    }
                }
            }
            
            public static IEnumerable<string> EnumerateFiles(Folder f, string path, Regex regex, SearchOption option) =>
                EnumerateFiles(f, ParseFolderPath(path), path, regex, option);
            
            static IEnumerable<string> EnumerateFiles(Folder f, Seq<string> path, string fullPath, Regex regex, SearchOption option) =>
                path.Length switch
                {
                    0 => EnumerateFiles1(f, fullPath, regex, option),
                    _ => f.Folders.TryGetValue(path[0], out var child) 
                             ? EnumerateFiles(child, path.Tail, fullPath, regex, option)
                             : throw new DirectoryNotFoundException($"Directory not found: {fullPath}")
                };

            static IEnumerable<string> EnumerateFiles1(Folder f, string fullPath, Regex regex, SearchOption option)
            {
                return Yield(f, fullPath);

                IEnumerable<string> Yield(Folder f, string p)
                {
                    foreach (var c in f.Files)
                    {
                        var np = Path.Combine(p, c.Key);
                        if (regex.IsMatch(c.Key))
                        {
                            yield return np;
                        }
                    }
                    
                    foreach (var c in f.Folders)
                    {
                        var np = Path.Combine(p, c.Value.Name);
                        if (option == SearchOption.AllDirectories)
                        {
                            foreach (var ic in Yield(c.Value, np))
                            {
                                yield return ic;
                            }
                        }
                    }
                }
            }
        }        
    }
}
