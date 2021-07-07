using System;
using System.IO;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys.IO
{
    public static class Directory<RT>
        where RT : struct, HasDirectory<RT>
    {
        /// <summary>
        /// Create a directory
        /// </summary>
        public static Eff<RT, Unit> create(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.Create(path));

        /// <summary>
        /// Delete a directory
        /// </summary>
        public static Eff<RT, Unit> delete(string path, bool recursive = true) =>
            default(RT).DirectoryEff.Map(rt => rt.Delete(path, recursive));
        
        /// <summary>
        /// Get parent directory
        /// </summary>
        public static Eff<RT, Option<DirectoryInfo>> getParent(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetParent(path));

        /// <summary>
        /// Check if directory exists
        /// </summary>
        public static Eff<RT, bool> exists(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.Exists(path));
     
        /// <summary>
        /// Set the directory creation time
        /// </summary>
        public static Eff<RT, Unit> setCreationTime(string path, DateTime creationTime) =>
            default(RT).DirectoryEff.Map(rt => rt.SetCreationTime(path, creationTime));

        /// <summary>
        /// Set the directory creation time
        /// </summary>
        public static Eff<RT, Unit> setCreationTimeUtc(string path, DateTime creationTimeUtc) =>
            default(RT).DirectoryEff.Map(rt => rt.SetCreationTimeUtc(path, creationTimeUtc));

        /// <summary>
        /// Get the directory creation time
        /// </summary>
        public static Eff<RT, DateTime> getCreationTime(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetCreationTime(path));

        /// <summary>
        /// Get the directory creation time
        /// </summary>
        public static Eff<RT, DateTime> getCreationTimeUtc(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetCreationTimeUtc(path));

        /// <summary>
        /// Set the directory last write time
        /// </summary>
        public static Eff<RT, Unit> setLastWriteTime(string path, DateTime lastWriteTime) =>
            default(RT).DirectoryEff.Map(rt => rt.SetLastWriteTime(path, lastWriteTime));

        /// <summary>
        /// Set the directory last write time
        /// </summary>
        public static Eff<RT, Unit> setLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
            default(RT).DirectoryEff.Map(rt => rt.SetLastWriteTimeUtc(path, lastWriteTimeUtc));

        /// <summary>
        /// Get the directory last write time
        /// </summary>
        public static Eff<RT, DateTime> getLastWriteTime(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetLastWriteTime(path));

        /// <summary>
        /// Get the directory last write time
        /// </summary>
        public static Eff<RT, DateTime> getLastWriteTimeUtc(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetLastWriteTimeUtc(path));

        /// <summary>
        /// Set the directory last access time
        /// </summary>
        public static Eff<RT, Unit> setLastAccessTime(string path, DateTime lastAccessTime) =>
            default(RT).DirectoryEff.Map(rt => rt.SetLastAccessTime(path, lastAccessTime));

        /// <summary>
        /// Set the directory last access time
        /// </summary>
        public static Eff<RT, Unit> setLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
            default(RT).DirectoryEff.Map(rt => rt.SetLastAccessTimeUtc(path, lastAccessTimeUtc));
        
        /// <summary>
        /// Get the directory last access time
        /// </summary>
        public static Eff<RT, DateTime> getLastAccessTime(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetLastAccessTime(path));
        
        /// <summary>
        /// Get the directory last access time
        /// </summary>
        public static Eff<RT, DateTime> getLastAccessTimeUtc(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetLastAccessTimeUtc(path));

        /// <summary>
        /// Enumerate directories
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateDirectories(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateDirectories(path));
        
        /// <summary>
        /// Enumerate directories
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateDirectories(path, searchPattern));
        
        /// <summary>
        /// Enumerate directories
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateDirectories(path, searchPattern, searchOption));
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFiles(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFiles(path));
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFiles(path, searchPattern));
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFiles(path, searchPattern, searchOption));
        
        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFileSystemEntries(path));

        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFileSystemEntries(path, searchPattern));

        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
            default(RT).DirectoryEff.Map(rt => rt.EnumerateFileSystemEntries(path, searchPattern, searchOption));

        /// <summary>
        /// Get the root of the path provided
        /// </summary>
        public static Eff<RT, string> getRoot(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.GetDirectoryRoot(path));

        /// <summary>
        /// Get the current directory
        /// </summary>
        public static Eff<RT, string> current =>
            default(RT).DirectoryEff.Map(static rt => rt.GetCurrentDirectory());

        /// <summary>
        /// Set the current directory
        /// </summary>
        /// <param name="path"></param>
        public static Eff<RT, Unit> setCurrent(string path) =>
            default(RT).DirectoryEff.Map(rt => rt.SetCurrentDirectory(path));

        /// <summary>
        /// Move a directory
        /// </summary>
        public static Eff<RT, Unit> move(string sourceDirName, string destDirName) =>
            default(RT).DirectoryEff.Map(rt => rt.Move(sourceDirName, destDirName));

        /// <summary>
        /// Get the logical drives
        /// </summary>
        public static Eff<RT, Seq<string>> logicalDrives =>
            default(RT).DirectoryEff.Map(static rt => rt.GetLogicalDrives());
    }
}
