using System;
using System.IO;

namespace LanguageExt.Sys.Live
{
    public class DirectoryIO : Traits.DirectoryIO
    {
        public static readonly Traits.DirectoryIO Default = new DirectoryIO();
        
        public Unit Create(string path)
        {
            System.IO.Directory.CreateDirectory(path);
            return default;
        }

        public Unit Delete(string path, bool recursive = true)
        {
            System.IO.Directory.Delete(path, recursive);
            return default;
        }

        public Option<DirectoryInfo> GetParent(string path) =>
            System.IO.Directory.GetParent(path);

        public bool Exists(string path) =>
            System.IO.Directory.Exists(path);

        public Unit SetCreationTime(string path, DateTime creationTime)
        {
            System.IO.Directory.SetCreationTime(path, creationTime);
            return default;
        }

        public Unit SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            System.IO.Directory.SetCreationTimeUtc(path, creationTimeUtc);
            return default;
        }

        public DateTime GetCreationTime(string path) =>
            System.IO.Directory.GetCreationTime(path);

        public DateTime GetCreationTimeUtc(string path) =>
            System.IO.Directory.GetCreationTimeUtc(path);

        public Unit SetLastWriteTime(string path, DateTime lastWriteTime) 
        {
            System.IO.Directory.SetLastWriteTime(path, lastWriteTime);
            return default;
        }

        public Unit SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {    
            System.IO.Directory.SetLastWriteTime(path, lastWriteTimeUtc);
            return default;
        }

        public DateTime GetLastWriteTime(string path) =>
            System.IO.Directory.GetLastWriteTime(path);

        public DateTime GetLastWriteTimeUtc(string path) =>
            System.IO.Directory.GetLastWriteTimeUtc(path);

        public Unit SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            System.IO.Directory.SetLastAccessTime(path, lastAccessTime);
            return default;
        }

        public Unit SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            System.IO.Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
            return default;
        }

        public DateTime GetLastAccessTime(string path) =>
            System.IO.Directory.GetLastAccessTime(path);

        public DateTime GetLastAccessTimeUtc(string path) =>
            System.IO.Directory.GetLastAccessTimeUtc(path);

        public Seq<string> EnumerateDirectories(string path) =>
            System.IO.Directory.EnumerateDirectories(path).ToSeq();

        public Seq<string> EnumerateDirectories(string path, string searchPattern) =>
            System.IO.Directory.EnumerateDirectories(path, searchPattern).ToSeq();

        public Seq<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
            System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption).ToSeq();

        public Seq<string> EnumerateFiles(string path) =>
            System.IO.Directory.EnumerateFiles(path).ToSeq();

        public Seq<string> EnumerateFiles(string path, string searchPattern) =>
            System.IO.Directory.EnumerateFiles(path, searchPattern).ToSeq();

        public Seq<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
            System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption).ToSeq();

        public Seq<string> EnumerateFileSystemEntries(string path) =>
            System.IO.Directory.EnumerateFileSystemEntries(path).ToSeq();

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern) =>
            System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern).ToSeq();

        public Seq<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
            System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption).ToSeq();

        public string GetDirectoryRoot(string path) =>
            System.IO.Directory.GetDirectoryRoot(path);

        public string GetCurrentDirectory() =>
            System.IO.Directory.GetCurrentDirectory();

        public Unit SetCurrentDirectory(string path)
        {
            System.IO.Directory.SetCurrentDirectory(path);
            return default;
        }

        public Unit Move(string sourceDirName, string destDirName)
        {
            System.IO.Directory.Move(sourceDirName, destDirName);
            return default;
        }

        public Seq<string> GetLogicalDrives() =>
            System.IO.Directory.GetLogicalDrives().ToSeq();
    }
}
