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
    /// NOTE: This isn't anywhere near as strict as the real file-system, and so it shouldn't really be used to test
    ///       file-operations.  It should be used to test simple access to files without having to create them for
    ///       real, or worry about what drives exist.
    /// </remarks>
    public class MemoryFS
    {
        readonly Folder drive = new Folder("C:", DateTime.Now);
        public string CurrentDir = "C:\\";
        
        Seq<string> ParsePath(string path) =>
            System.IO.Path.IsPathRooted(path)
                ? path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                      .ToSeq()
                :throw new IOException($"Path not rooted: {path}");

        public Seq<string> GetLogicalDrives() =>
            Seq1("C:\\");
        
        string AssertDrive(string? path)
        {
            if (path != null &&
                (path.StartsWith("C:\\") ||
                 path.StartsWith("C:\\") ||
                 path.StartsWith("c:/") ||
                 path.StartsWith("c:/")))
            {
                return path.Substring(3);
            }
            else
            {
                throw new IOException("Only drive available in MemoryFS is C:");
            }
        }

        internal IEnumerable<string> EnumerateFolders(string path, string searchPattern, SearchOption option)
        {
            path = AssertDrive(path);
            var regex = MakePathSearchRegex(searchPattern);
            return Folder.EnumerateFolders(drive, path, regex, option);
        }

        internal IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption option)
        {
            path = AssertDrive(path);
            var regex = MakePathSearchRegex(searchPattern);
            return Folder.EnumerateFiles(drive, path, regex, option);
        }

        // TODO: This will probably need some finessing
        static Regex MakePathSearchRegex(string searchPattern) =>
            new Regex(searchPattern.Replace(".", "\\.")
                                   .Replace("$", "\\$")
                                   .Replace("^", "\\^")
                                   .Replace("\\", "\\\\")
                                   .Replace("+", "\\+")
                                   .Replace("{", "\\{")
                                   .Replace("}", "\\}")
                                   .Replace("[", "\\[")
                                   .Replace("]", "\\]")
                                   .Replace("(", "\\(")
                                   .Replace(")", "\\)")
                                   .Replace("|", "\\|")
                                   .Replace("?", ".")
                                   .Replace("*", ".+?"));  // Match the preceding character or subexpression 1 or more times (as few as possible).

        internal Unit CreateFolder(string path, DateTime now)
        {
            path = AssertDrive(path);
            return Folder.PutFolder(drive, path, now);
        }

        internal bool FolderExists(string path)
        {
            path = AssertDrive(path);
            return Folder.FolderExists(drive, path);
        }

        internal string AssertFolderExists(string path)
        {
            path = AssertDrive(path);
            var e = Folder.FolderExists(drive, path);
            return e
                       ? path
                       : throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        internal Unit DeleteFolder(string path, bool recursive)
        {
            path = AssertDrive(path);
            return Folder.DeleteFolder(drive, path, recursive);
        }

        internal Unit SetFolderCreationTime(string path, DateTime dt)
        {
            path = AssertDrive(path);
            return ignore(Folder.Map(drive, path, f => f.CreationTime = dt));
        }

        internal Unit SetFolderLastAccessTime(string path, DateTime dt)
        {
            path = AssertDrive(path);
            return ignore(Folder.Map(drive, path, f => f.LastAccessTime = dt));
        }

        internal Unit SetFolderLastWriteTime(string path, DateTime dt)
        {
            path = AssertDrive(path);
            return ignore(Folder.Map(drive, path, f => f.LastWriteTime = dt));
        }

        internal DateTime GetFolderCreationTime(string path)
        {
            path = AssertDrive(path);
            return Folder.Map(drive, path, f => f.CreationTime);
        }

        internal DateTime GetFolderLastAccessTime(string path)
        {
            path = AssertDrive(path);
            return Folder.Map(drive, path, f => f.LastAccessTime);
        }

        internal DateTime GetFolderLastWriteTime(string path)
        {
            path = AssertDrive(path);
            return Folder.Map(drive, path, f => f.LastWriteTime);
        }
        
        internal Unit Delete(string path)
        {
            path = AssertDrive(path);
            return Folder.DeleteFile(drive, path);
        }

        internal bool Exists(string path)
        {
            path = AssertDrive(path);
            return Folder.FileExists(drive, path);
        }

        internal byte[] GetFile(string path)
        {
            path = AssertDrive(path);
            return Folder.GetFile(drive, path);
        }

        internal string GetText(string path) =>
            Encoding.UTF8.GetString(GetFile(path));

        internal string[] GetLines(string path) =>
            Encoding.UTF8.GetString(GetFile(path)).Split('\n');

        internal Unit PutFile(string path, byte[] data, bool overwrite = false)
        {
            path = AssertDrive(path);
            return Folder.PutFile(drive, path, data, overwrite);
        }

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

            public static A Map<A>(Folder root, string path, Func<Folder, A> map) =>
                Map1(root, ParsePath(path), path, map);

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
                PutFile(root, ParsePath(path), path, data, overwrite);

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
                DeleteFile(root, ParsePath(path), path);

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
                GetFile(root, ParsePath(path), path);

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
                FileExists(root, ParsePath(path), path);

            static bool FileExists(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => false,
                    1 => FileExists1(f, path.Head, fullPath),
                    _ => f.Folders.TryGetValue(path[0], out var child) && FileExists(child, path.Tail, fullPath)
                };

            static bool FileExists1(Folder f, string name, string fullPath) =>
                f.Files.ContainsKey(name);

            public static Unit PutFolder(Folder root, string path, DateTime now) =>
                PutFolder(root, ParsePath(path), path, now);

            static Unit PutFolder(Folder f, Seq<string> path, string fullPath, DateTime now) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Invalid path: {fullPath}"),
                    1 => AddFolder(f, path.Head, now),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? PutFolder(child, path.Tail, fullPath, now)
                             : throw new DirectoryNotFoundException($"Invalid path: {fullPath}")
                };

            static Unit AddFolder(Folder f, string name, DateTime now)
            {
                if (f.Files.ContainsKey(name)) throw new IOException($"File already exists with the same name: {name}");
                f.Folders.AddOrUpdate(name, new Folder(name, now), (_, current) => current);
                return default;
            }
            
            public static Unit DeleteFolder(Folder root, string path, bool recursive) =>
                DeleteFolder(root, ParsePath(path), path, recursive);

            static Unit DeleteFolder(Folder f, Seq<string> path, string fullPath, bool recursive) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Directory not found: {fullPath}"),
                    1 => DeleteFolder1(f, path.Head, recursive),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? DeleteFolder(child, path.Tail, fullPath, recursive)
                             : throw new DirectoryNotFoundException($"Directory not found: {fullPath}")
                };

            static Unit DeleteFolder1(Folder f, string name, bool recursive)
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
                                DeleteFolder1(fremove, child.Key, recursive);
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
                }

                return default;
            }
            
            public static bool FolderExists(Folder root, string path) =>
                FolderExists(root, ParsePath(path), path);

            static bool FolderExists(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => true,
                    _ => f.Folders.TryGetValue(path[0], out var child) && FolderExists(child, path.Tail, fullPath)
                };

            static Seq<string> ParsePath(string path) =>
                System.IO.Path.IsPathRooted(path)
                    ? path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                          .ToSeq()
                    : throw new IOException($"Path not rooted: {path}");

            public static IEnumerable<string> EnumerateFolders(Folder f, string path, Regex regex, SearchOption option) =>
                EnumerateFolders(f, ParsePath(path), path, regex, option);
            
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
                EnumerateFiles(f, ParsePath(path), path, regex, option);
            
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
