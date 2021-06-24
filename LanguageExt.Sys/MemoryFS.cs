using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LanguageExt.UnsafeValueAccess;

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
        readonly Folder drive = new Folder("C:");
        
        Seq<string> ParsePath(string path) =>
            System.IO.Path.IsPathRooted(path)
                ? path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                      .ToSeq()
                :throw new IOException($"Path not rooted: {path}");

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

        internal Unit CreateFolder(string path)
        {
            path = AssertDrive(path);
            return Folder.PutFolder(drive, path);
        }

        internal bool FolderExists(string path)
        {
            path = AssertDrive(path);
            return Folder.FolderExists(drive, path);
        }

        internal Unit DeleteFolder(string path, bool recursive)
        {
            path = AssertDrive(path);
            return Folder.DeleteFolder(drive, path, recursive);
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
        
        class Folder
        {
            string Name;
            readonly ConcurrentDictionary<string, byte[]> Files = new (StringComparer.InvariantCultureIgnoreCase);
            readonly ConcurrentDictionary<string, Folder> Folders = new (StringComparer.InvariantCultureIgnoreCase);
            
            public Folder(string name)
            {
                Name    = name;
            }

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

            public static Unit PutFolder(Folder root, string path) =>
                PutFolder(root, ParsePath(path), path);

            static Unit PutFolder(Folder f, Seq<string> path, string fullPath) =>
                path.Length switch
                {
                    0 => throw new DirectoryNotFoundException($"Invalid path: {fullPath}"),
                    1 => AddFolder(f, path.Head),
                    _ => f.Folders.TryGetValue(path[0], out var child)
                             ? PutFolder(child, path.Tail, fullPath)
                             : throw new DirectoryNotFoundException($"Invalid path: {fullPath}")
                };

            static Unit AddFolder(Folder f, string name)
            {
                if (f.Files.ContainsKey(name)) throw new IOException($"File already exists with the same name: {name}");
                f.Folders.AddOrUpdate(name, new Folder(name), (_, current) => current);
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
                // TODO: Not thread-safe
                
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
                        if(fremove.Folders.Count > 0)
                        {
                            throw new IOException("Directory not empty");
                        }
                        else
                        {
                            f.Folders.TryRemove(name, out var _);
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
                    0 => false,
                    1 => FolderExists1(f, path.Head, fullPath),
                    _ => f.Folders.TryGetValue(path[0], out var child) && FolderExists(child, path.Tail, fullPath)
                };

            static bool FolderExists1(Folder f, string name, string fullPath) =>
                f.Folders.ContainsKey(name);

            
            static Seq<string> ParsePath(string path) =>
                System.IO.Path.IsPathRooted(path)
                    ? path.Split(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                          .ToSeq()
                    : throw new IOException($"Path not rooted: {path}"); 
        }        
    }
}
