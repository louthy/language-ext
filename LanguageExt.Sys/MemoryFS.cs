using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using LanguageExt.ClassInstances;
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
        readonly Atom<Entry> machine = Atom<Entry>(new FolderEntry("[machine]", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, default));
        readonly static char[] invalidPath = Path.GetInvalidPathChars();
        readonly static char[] invalidFile = Path.GetInvalidFileNameChars();
        public string CurrentDir = "C:\\";

        public MemoryFS() =>
            AddLogicalDrive("C");


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

        Seq<string> ParsePath(string path) =>
            System.IO.Path.IsPathRooted(path)
                ? ParsePath1(path)
                :throw new IOException($"Path not rooted: {path}");

        static Seq<string> ParsePath1(string path) =>
            ValidatePathNames(path.Trim()
                                  .TrimEnd(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                                  .Split(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                                  .ToSeq());

        static Seq<string> ValidatePathNames(Seq<string> path)
        {
            if (path.IsEmpty) 
                throw new IOException($"Invalid path: {string.Join(Path.DirectorySeparatorChar.ToString(), path)}");
            
            if (path.Head.Exists(invalidPath.Contains)) 
                throw new IOException($"Invalid path: {string.Join(Path.DirectorySeparatorChar.ToString(), path)}");
            
            foreach (var name in path.Tail)
            {
                if (name.Exists(invalidFile.Contains)) 
                    throw new IOException($"Invalid path: {string.Join(Path.DirectorySeparatorChar.ToString(), path)}");
            }
            return path;
        }
        
        Entry? FindEntry(string path) =>
            FindEntry(machine, ParsePath(path));

        Entry? FindEntry(Entry entry, Seq<string> path) =>
            path.IsEmpty
                ? entry
                : entry.GetChild(path.Head) switch
                  {
                      null  => null,
                      var e => FindEntry(e, path.Tail)
                  };

        internal IEnumerable<string> EnumerateFolders(string path, string searchPattern, SearchOption option)
        {
            var regex  = MakePathSearchRegex(searchPattern);
            var entry = FindEntry(path);
            return entry == null || entry is FileEntry
                       ? throw new DirectoryNotFoundException($"Directory not found: {path}")
                       : entry.EnumerateFolders(Empty, regex, option, false)
                              .Map(e => string.Join(Path.DirectorySeparatorChar.ToString(), e.Path));
        }

        internal IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption option)
        {
            var regex = MakePathSearchRegex(searchPattern);
            var entry = FindEntry(path);
            return entry == null || entry is FileEntry
                       ? throw new DirectoryNotFoundException($"Directory not found: {path}")
                       : entry.EnumerateFiles(Empty, regex, option)
                              .Map(e => string.Join(Path.DirectorySeparatorChar.ToString(), e.Path));
        }

        internal IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption option)
        {
            var regex = MakePathSearchRegex(searchPattern);
            var entry = FindEntry(path);
            return entry == null || entry is FileEntry
                       ? throw new DirectoryNotFoundException($"Directory not found: {path}")
                       : entry.EnumerateEntries(Empty, regex, option, false)
                              .Map(e => string.Join(Path.DirectorySeparatorChar.ToString(), e.Path));
        }

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
            var path1 = ParsePath(path);
            machine.Swap(m => path1.NonEmptyInits.Fold(m, go));  
            return default;
            
            Entry go(Entry m, Seq<string> path1)
            {
                var folder = new FolderEntry(path1.Last, now, now, now, default);
                return FindEntry(m, path1) switch
                       {
                           null => m.Add(path1, folder, now).IfLeft(e => throw e),
                           var e => e is FolderEntry
                                        ? m
                                        : throw new IOException($"File with same name already exists: {path}")
                       };
            }
        }

        internal bool FolderExists(string path) =>
            FindEntry(path) is FolderEntry;

        internal Unit DeleteFolder(string path, bool recursive, DateTime now)
        {
            machine.Swap(m => m.Delete(ParsePath(path), recursive, now).Case switch
                              {
                                  Exception ex => throw ex,
                                  Entry m1     => m1,
                                  _            => throw new InvalidOperationException()
                              });
            return default;
        }

        internal Unit SetFolderCreationTime(string path, DateTime dt, DateTime now)
        {
            machine.Swap(
                m =>
                    m.Update(
                        ParsePath(path),
                        e => e.SetCreationTime(dt),
                        _ => throw new DirectoryNotFoundException($"Directory not found: {path}"),
                        now).Case switch
                    {
                        Exception ex => throw ex,
                        Entry e      => e,
                        _            => throw new InvalidOperationException()
                    }
            );
            return default;

        }

        internal Unit SetFolderLastAccessTime(string path, DateTime dt, DateTime now)
        {
            machine.Swap(
                m =>
                    m.Update(
                        ParsePath(path),
                        e => e.SetLastAccessTime(dt),
                        _ => throw new DirectoryNotFoundException($"Directory not found: {path}"),
                        now).Case switch
                    {
                        Exception ex => throw ex,
                        Entry e      => e,
                        _            => throw new InvalidOperationException()
                    }
            );
            return default;
        }

        internal Unit SetFolderLastWriteTime(string path, DateTime dt, DateTime now)
        {
            machine.Swap(
                m =>
                    m.Update(
                        ParsePath(path),
                        e => e.SetLastWriteTime(dt),
                        _ => throw new DirectoryNotFoundException($"Directory not found: {path}"),
                        now).Case switch
                    {
                        Exception ex => throw ex,
                        Entry e      => e,
                        _            => throw new InvalidOperationException()
                    }
            );
            return default;
        }

        internal DateTime GetFolderCreationTime(string path) =>
            FindEntry(path) switch
            {
                null  =>  throw new DirectoryNotFoundException($"Directory not found: {path}"),
                var e => e.CreationTime
            };

        internal DateTime GetFolderLastAccessTime(string path) =>
            FindEntry(path) switch
            {
                null  =>  throw new DirectoryNotFoundException($"Directory not found: {path}"),
                var e => e.LastAccessTime
            };

        internal DateTime GetFolderLastWriteTime(string path) =>
            FindEntry(path) switch
            {
                null  =>  throw new DirectoryNotFoundException($"Directory not found: {path}"),
                var e => e.LastWriteTime
            };
        
        internal Unit Delete(string path, DateTime now)
        {
            machine.Swap(m => FindEntry(m, ParsePath(path)) is FileEntry f
                                  ? m.Delete(ParsePath(path), false, now).Case switch
                                    {
                                        Exception => throw new FileNotFoundException("File not found", path),
                                        Entry m1  => m1,
                                        _         => throw new InvalidOperationException()
                                    }
                                  : throw new FileNotFoundException("File not found", path));
            return default;
        }

        internal bool Exists(string path) =>
            FindEntry(path) is FileEntry;

        internal byte[] GetFile(string path) =>
            FindEntry(path) is FileEntry f
                ? f.Data
                : throw new FileNotFoundException("File not found", path);

        internal string GetText(string path) =>
            Encoding.UTF8.GetString(GetFile(path));

        internal string[] GetLines(string path) =>
            Encoding.UTF8.GetString(GetFile(path)).Split('\n');

        internal Unit PutFile(string path, byte[] data, bool overwrite, DateTime now)
        {
            var path1 = ParsePath(path);
            var file  = new FileEntry(path1.Last, now, now, now, data);
            
            machine.Swap(
                m => FindEntry(m, path1) switch
                     {
                        null  => m.Add(path1, file, now).IfLeft(e => throw e),
                        var e => overwrite
                                    ? m.Update(path1, _ => file, _ => new FileNotFoundException(), now).IfLeft(e => throw e)
                                    : throw new IOException($"File-system entry already exists: {path}")
                     });
            return default;
        }

        internal Unit PutText(string path, string text, bool overwrite, DateTime now) =>
            PutFile(path, Encoding.UTF8.GetBytes(text), overwrite, now);

        internal Unit PutLines(string path, IEnumerable<string> text, bool overwrite, DateTime now) =>
            PutText(path, string.Join("\n", text), overwrite, now);

        internal Unit Append(string path, byte[] data, DateTime now)
        {
            var pre = GetFile(path);
            var fin = new byte[pre.Length + data.Length];
            System.Array.Copy(pre, fin, pre.Length);
            System.Array.Copy(data, pre.Length, fin, 0, data.Length);
            return PutFile(path, fin, true, now);
        }

        internal Unit AppendText(string path, string text, DateTime now) =>
            PutText(path, GetText(path) + text, true, now);

        internal Unit AppendLines(string path, IEnumerable<string> lines, DateTime now) =>
            PutLines(path, GetLines(path).Concat(lines).ToArray(), true, now);

        internal Unit CopyFile(string src, string dest, bool overwrite, DateTime now) =>
            PutFile(dest, GetFile(src), overwrite, now);

        internal Unit Move(string src, string dest, DateTime now)
        {
            var srcp   = ParsePath(src); 
            var destp  = ParsePath(dest);
            var parent = Path.GetDirectoryName(dest);
            if (parent == null) throw new DirectoryNotFoundException($"Parent directory not found: {dest}");
            
            machine.Swap(
                m =>
                {
                    // Get the source file or folder (it should exist)
                    var esrc = FindEntry(m, srcp);
                    if (esrc == null) throw new IOException($"Source doesn't exist: {src}");
                    
                    // Get the destination file or folder (it shouldn't exist)
                    var edest = FindEntry(m, destp);
                    if (edest != null) throw new IOException($"Destination already exists: {dest}");

                    // Create the destination folder
                    var parent1      = ParsePath(parent);
                    var parentFolder = new FolderEntry(parent1.Last, now, now, now, default);
                    var eparent      = FindEntry(m, parent1);
                    if(eparent == null) m = m.Add(parent1, parentFolder, now).IfLeft(e => throw e);

                    // Delete the original
                    m = m.Delete(srcp, true, now).IfLeft(e => throw e);
                    
                    // Write the entry in the new location and update the path and name
                    m = m.Add(destp, esrc.UpdateName(destp.Last), now).IfLeft(e => throw e);

                    return m;
                });

            return default;
        }

        abstract class Entry
        {
            public readonly string Name;
            public readonly DateTime CreationTime;
            public readonly DateTime LastAccessTime;
            public readonly DateTime LastWriteTime;
 
            protected Entry(string name, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
            {
                Name           = name;
                CreationTime   = creationTime;
                LastAccessTime = lastAccessTime;
                LastWriteTime  = lastWriteTime;
            }

            public abstract IEnumerable<(Seq<string> Path, Entry Entry)> EnumerateEntries(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf);
            public abstract IEnumerable<(Seq<string> Path, FolderEntry Entry)> EnumerateFolders(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf);
            public abstract IEnumerable<(Seq<string> Path, FileEntry Entry)> EnumerateFiles(Seq<string> parent, Regex searchPattern, SearchOption option);
            public abstract Entry? GetChild(string name);
            public abstract Either<Exception, Entry> Add(Seq<string> path, Entry entry, DateTime now);
            public abstract Either<Exception, Entry> Update(Seq<string> path, Func<Entry, Either<Exception, Entry>> update, Func<Entry, Either<Exception, Entry>> notFound, DateTime now);
            public abstract Either<Exception, Entry> Delete(Seq<string> path, bool recursive, DateTime now);
            public abstract bool IsEmpty { get; }
            public abstract Entry SetCreationTime(DateTime dt);
            public abstract Entry SetLastAccessTime(DateTime dt);
            public abstract Entry SetLastWriteTime(DateTime dt);
            public abstract Entry UpdateName(string name);
        }
        
        class FileEntry : Entry
        {
            public readonly byte[] Data;

            public FileEntry(string name, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, byte[] data) 
                : base(name, creationTime, lastAccessTime, lastWriteTime) =>
                    Data = data;

            public override IEnumerable<(Seq<string> Path, FolderEntry Entry)> EnumerateFolders(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf) =>
                new (Seq<string>, FolderEntry)[0];

            public override IEnumerable<(Seq<string> Path, FileEntry Entry)> EnumerateFiles(Seq<string> parent, Regex searchPattern, SearchOption option) =>
                searchPattern.IsMatch(Name)
                    ? new[] {(parent.Add(Name), this)}
                    : new (Seq<string>, FileEntry)[0];

            public override IEnumerable<(Seq<string> Path, Entry Entry)> EnumerateEntries(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf) =>
                includeSelf && searchPattern.IsMatch(Name)
                    ? new[] {(parent.Add(Name), (Entry)this)}
                    : new (Seq<string>, Entry)[0];

            public override Entry? GetChild(string name) =>
                null;

            public override Either<Exception, Entry> Add(Seq<string> path, Entry entry, DateTime now) =>
                new DirectoryNotFoundException();

            public override Either<Exception, Entry> Update(Seq<string> path, Func<Entry, Either<Exception, Entry>> update, Func<Entry, Either<Exception, Entry>> notFound, DateTime now) =>
                path.IsEmpty
                    ? update(this)
                    : notFound(this);

            public override Either<Exception, Entry> Delete(Seq<string> path, bool recursive, DateTime now) =>
                new DirectoryNotFoundException();

            public override bool IsEmpty =>
                true;

            public override Entry SetCreationTime(DateTime dt) =>
                new FileEntry(Name, dt, LastAccessTime, LastWriteTime, Data);

            public override Entry SetLastAccessTime(DateTime dt) =>
                new FileEntry(Name, CreationTime, dt, LastWriteTime, Data);

            public override Entry SetLastWriteTime(DateTime dt) =>
                new FileEntry(Name, CreationTime, LastAccessTime, dt, Data);

            public override Entry UpdateName(string name) =>
                new FileEntry(name, CreationTime, LastAccessTime, LastWriteTime, Data);
        }

        class FolderEntry : Entry
        {
            readonly Map<OrdStringOrdinalIgnoreCase, string, Entry> Entries;

            public FolderEntry(string name, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, Map<OrdStringOrdinalIgnoreCase, string, Entry> entries)
                : base(name, creationTime, lastAccessTime, lastWriteTime) =>
                    Entries = entries;

            IEnumerable<FileEntry> Files =>
                Entries.Values.Choose(e => e is FileEntry f ? Some(f) : None);

            public override IEnumerable<(Seq<string> Path, FolderEntry Entry)> EnumerateFolders(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf)
            {
                var self = includeSelf && searchPattern.IsMatch(Name)
                               ? new [] {(parent.Add(Name), this)}
                               : new (Seq<string>, FolderEntry)[0];

                var children = option == SearchOption.AllDirectories
                                   ? Entries.Values.Choose(e => e is FolderEntry f ? Some(f.EnumerateFolders(parent.Add(Name), searchPattern, option, true)) : None).Bind(identity)
                                   : new (Seq<string>, FolderEntry)[0];

                return self.Concat(children);
            }

            public override IEnumerable<(Seq<string> Path, FileEntry Entry)> EnumerateFiles(Seq<string> parent, Regex searchPattern, SearchOption option)
            {
                var files = Files.Bind(f => f.EnumerateFiles(parent.Add(Name), searchPattern, option));

                var children = option == SearchOption.AllDirectories
                                   ? Entries.Values.Choose(e => e is FolderEntry f ? Some(f.EnumerateFiles(parent.Add(Name), searchPattern, option)) : None).Bind(identity)
                                   : new (Seq<string>, FileEntry) [0];

                return files.Concat(children);
            }

            public override IEnumerable<(Seq<string> Path, Entry Entry)> EnumerateEntries(Seq<string> parent, Regex searchPattern, SearchOption option, bool includeSelf)
            {
                var self = includeSelf && searchPattern.IsMatch(Name)
                               ? new [] {(parent.Add(Name), (Entry)this)}
                               : new (Seq<string>, Entry)[0];

                var files = Files.Bind(f => f.EnumerateEntries(parent.Add(Name), searchPattern, option, true));

                var children = option == SearchOption.AllDirectories
                                   ? Entries.Values.Choose(e => e is FolderEntry f ? Some(f.EnumerateEntries(parent.Add(Name), searchPattern, option, true)) : None).Bind(identity)
                                   : new (Seq<string>, Entry) [0];

                return self.Concat(files).Concat(children);
            }

            public override Either<Exception, Entry> Add(Seq<string> path, Entry entry, DateTime now) =>
                path.Length switch
                {
                    0 => entry,
                    1 => new FolderEntry(Name, CreationTime, now, now, Entries.AddOrUpdate(entry.Name, entry)),
                    _ => Entries.Find(path.Head).Case switch
                         {
                             Entry e => e.Add(path.Tail, entry, now).Case switch
                                        {
                                            Exception ex => ex,
                                            Entry     ne => new FolderEntry(Name, CreationTime, now, now, Entries.SetItem(ne.Name, ne)), 
                                            _            => new InvalidOperationException(),
                                        },
                             _       => new DirectoryNotFoundException()
                         }
                };

            public override Either<Exception, Entry> Update(Seq<string> path, Func<Entry, Either<Exception, Entry>> update, Func<Entry, Either<Exception, Entry>> notFound, DateTime now) =>
                path.IsEmpty
                    ? update(this)
                    : Entries.Find(path.Head).Case switch
                      {
                          Entry e => e.Update(path.Tail, update, notFound, now).Case switch
                                     {
                                         Exception ex => ex,
                                         Entry ne     => new FolderEntry(Name, CreationTime, now, now, Entries.SetItem(ne.Name, ne)),
                                         _            => new InvalidOperationException(),
                                     },
                          _       => notFound(this),
                      };
            
            public override Either<Exception, Entry> Delete(Seq<string> path, bool recursive, DateTime now) =>
                path.Length switch
                {
                    0 => new DirectoryNotFoundException(),
                    1 => Entries.Find(path.Head).Case switch
                         {
                             Entry e when recursive || e.IsEmpty => new FolderEntry(Name, CreationTime, now, now, Entries.Remove(e.Name)),
                             Entry e                             => new IOException("Directory not empty"),
                             _                                   => new IOException("Invalid path")
                         },
                    _ => Entries.Find(path.Head).Case switch
                         {
                             Entry e => e.Delete(path.Tail, recursive, now).Case switch
                                        {
                                            Exception ex => ex,
                                            Entry     ne => new FolderEntry(Name, CreationTime, now, now, Entries.SetItem(ne.Name, ne)), 
                                            _            => new InvalidOperationException(),
                                        },
                             _       => new DirectoryNotFoundException()
                         }
                };        

            public override Entry? GetChild(string name) =>
                Entries.Find(name).Case switch
                {
                    Entry e => e,
                    _       => null
                };

            public override bool IsEmpty =>
                Entries.IsEmpty;

            public override Entry SetCreationTime(DateTime dt) =>
                new FolderEntry(Name, dt, LastAccessTime, LastWriteTime, Entries);

            public override Entry SetLastAccessTime(DateTime dt) =>
                new FolderEntry(Name, CreationTime, dt, LastWriteTime, Entries);

            public override Entry SetLastWriteTime(DateTime dt) =>
                new FolderEntry(Name, CreationTime, LastAccessTime, dt, Entries);
    
            public override Entry UpdateName(string name) =>
                new FolderEntry(name, CreationTime, LastAccessTime, LastWriteTime, Entries);
        }
    }
}
