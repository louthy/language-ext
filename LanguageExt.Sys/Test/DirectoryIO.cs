using System;
using System.IO;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    public readonly struct DirectoryIO : Traits.DirectoryIO
    {
        readonly MemoryFS fs;
        readonly DateTime now;
        
        public DirectoryIO(MemoryFS fs, DateTime now) =>
            (this.fs, this.now) = (fs, now);

        public Unit Create(string path) =>
            fs.CreateFolder(path, now);

        public Unit Delete(string path, bool recursive = true) =>
            fs.DeleteFolder(path, recursive);

        public Option<DirectoryInfo> GetParent(string path) =>
            Live.DirectoryIO.Default.GetParent(path);

        public bool Exists(string path) =>
            fs.FolderExists(path);

        public Unit SetCreationTime(string path, DateTime creationTime) =>
            fs.SetFolderCreationTime(path, creationTime.ToLocalTime());

        public Unit SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
            fs.SetFolderCreationTime(path, creationTimeUtc.ToUniversalTime());

        public DateTime GetCreationTime(string path) =>
            fs.GetFolderCreationTime(path).ToLocalTime();

        public DateTime GetCreationTimeUtc(string path) =>
            fs.GetFolderCreationTime(path).ToUniversalTime();

        public Unit SetLastWriteTime(string path, DateTime lastWriteTime) =>
            fs.SetFolderLastWriteTime(path, lastWriteTime.ToLocalTime());

        public Unit SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
            fs.SetFolderLastWriteTime(path, lastWriteTimeUtc.ToUniversalTime());

        public DateTime GetLastWriteTime(string path) =>
            fs.GetFolderLastWriteTime(path).ToLocalTime();

        public DateTime GetLastWriteTimeUtc(string path) =>
            fs.GetFolderLastWriteTime(path).ToUniversalTime();

        public Unit SetLastAccessTime(string path, DateTime lastAccessTime) =>
            fs.SetFolderLastAccessTime(path, lastAccessTime.ToLocalTime());

        public Unit SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
            fs.SetFolderLastAccessTime(path, lastAccessTimeUtc.ToUniversalTime());

        public DateTime GetLastAccessTime(string path) =>
            fs.GetFolderLastAccessTime(path).ToLocalTime();

        public DateTime GetLastAccessTimeUtc(string path) =>
            fs.GetFolderLastAccessTime(path).ToUniversalTime();

        public Seq<string> EnumerateDirectories(string path) =>
            fs.EnumerateFolders(path, "*", SearchOption.TopDirectoryOnly).ToSeq();

        public Seq<string> EnumerateDirectories(string path, string searchPattern) =>
            fs.EnumerateFolders(path, searchPattern, SearchOption.TopDirectoryOnly).ToSeq();

        public Seq<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
            fs.EnumerateFolders(path, searchPattern, searchOption).ToSeq();

        public Seq<string> EnumerateFiles(string path) =>
            fs.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly).ToSeq();

        public Seq<string> EnumerateFiles(string path, string searchPattern) =>
            fs.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToSeq();

        public Seq<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
            fs.EnumerateFiles(path, searchPattern, searchOption).ToSeq();

        public Seq<string> EnumerateFileSystemEntries(string path) =>
            EnumerateDirectories(path) + EnumerateFiles(path);

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern) =>
            EnumerateDirectories(path, searchPattern) + EnumerateFiles(path, searchPattern);

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
            EnumerateDirectories(path, searchPattern, searchOption) + EnumerateFiles(path, searchPattern, searchOption);

        public string GetDirectoryRoot(string path) =>
            Live.DirectoryIO.Default.GetDirectoryRoot(path);

        public string GetCurrentDirectory() =>
            fs.CurrentDir;

        public Unit SetCurrentDirectory(string path) =>
            ignore(fs.CurrentDir = path);

        public Unit Move(string sourceDirName, string destDirName) =>
            throw new NotImplementedException();

        public Seq<string> GetLogicalDrives() =>
            fs.GetLogicalDrives();
    }
}
