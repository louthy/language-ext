using System;
using System.IO;

namespace LanguageExt.Sys.Test
{
    public readonly struct DirectoryIO : Traits.DirectoryIO
    {
        readonly MemoryFS fs;
        
        public DirectoryIO(MemoryFS fs) =>
            this.fs = fs;
        
        public Unit Create(string path) =>
            throw new NotImplementedException();

        public Unit Delete(string path, bool recursive = true) =>
            throw new NotImplementedException();

        public Option<DirectoryInfo> GetParent(string path) =>
            throw new NotImplementedException();

        public bool Exists(string path) =>
            throw new NotImplementedException();

        public Unit SetCreationTime(string path, DateTime creationTime) =>
            throw new NotImplementedException();

        public Unit SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
            throw new NotImplementedException();

        public DateTime GetCreationTime(string path) =>
            throw new NotImplementedException();

        public DateTime GetCreationTimeUtc(string path) =>
            throw new NotImplementedException();

        public Unit SetLastWriteTime(string path, DateTime lastWriteTime) =>
            throw new NotImplementedException();

        public Unit SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
            throw new NotImplementedException();

        public DateTime GetLastWriteTime(string path) =>
            throw new NotImplementedException();

        public DateTime GetLastWriteTimeUtc(string path) =>
            throw new NotImplementedException();

        public Unit SetLastAccessTime(string path, DateTime lastAccessTime) =>
            throw new NotImplementedException();

        public Unit SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
            throw new NotImplementedException();

        public DateTime GetLastAccessTime(string path) =>
            throw new NotImplementedException();

        public DateTime GetLastAccessTimeUtc(string path) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateDirectories(string path) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateDirectories(string path, string searchPattern) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFiles(string path) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFiles(string path, string searchPattern) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFileSystemEntries(string path) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern) =>
            throw new NotImplementedException();

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
            throw new NotImplementedException();

        public string GetDirectoryRoot(string path) =>
            throw new NotImplementedException();

        public string GetCurrentDirectory() =>
            throw new NotImplementedException();

        public Unit SetCurrentDirectory(string path) =>
            throw new NotImplementedException();

        public Unit Move(string sourceDirName, string destDirName) =>
            throw new NotImplementedException();

        public Seq<string> GetLogicalDrives() =>
            throw new NotImplementedException();
    }
}
