using System;
using System.IO;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.IO;

public static class Directory<RT>
    where RT : HasDirectory<RT>
{
    static readonly Eff<RT, DirectoryIO> self =
        runtime<RT>().Bind(rt => rt.DirectoryEff);
    
    /// <summary>
    /// Create a directory
    /// </summary>
    public static Eff<RT, DirectoryInfo> create(string path) =>
        self.Bind(rt => rt.Create(path));

    /// <summary>
    /// Delete a directory
    /// </summary>
    public static Eff<RT, Unit> delete(string path, bool recursive = true) =>
        self.Bind(rt => rt.Delete(path, recursive));
        
    /// <summary>
    /// Get parent directory
    /// </summary>
    public static Eff<RT, Option<DirectoryInfo>> getParent(string path) =>
        self.Bind(rt => rt.GetParent(path));

    /// <summary>
    /// Check if directory exists
    /// </summary>
    public static Eff<RT, bool> exists(string path) =>
        self.Bind(rt => rt.Exists(path));
     
    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static Eff<RT, Unit> setCreationTime(string path, DateTime creationTime) =>
        self.Bind(rt => rt.SetCreationTime(path, creationTime));

    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static Eff<RT, Unit> setCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        self.Bind(rt => rt.SetCreationTimeUtc(path, creationTimeUtc));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static Eff<RT, DateTime> getCreationTime(string path) =>
        self.Bind(rt => rt.GetCreationTime(path));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static Eff<RT, DateTime> getCreationTimeUtc(string path) =>
        self.Bind(rt => rt.GetCreationTimeUtc(path));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static Eff<RT, Unit> setLastWriteTime(string path, DateTime lastWriteTime) =>
        self.Bind(rt => rt.SetLastWriteTime(path, lastWriteTime));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static Eff<RT, Unit> setLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        self.Bind(rt => rt.SetLastWriteTimeUtc(path, lastWriteTimeUtc));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static Eff<RT, DateTime> getLastWriteTime(string path) =>
        self.Bind(rt => rt.GetLastWriteTime(path));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static Eff<RT, DateTime> getLastWriteTimeUtc(string path) =>
        self.Bind(rt => rt.GetLastWriteTimeUtc(path));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static Eff<RT, Unit> setLastAccessTime(string path, DateTime lastAccessTime) =>
        self.Bind(rt => rt.SetLastAccessTime(path, lastAccessTime));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static Eff<RT, Unit> setLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        self.Bind(rt => rt.SetLastAccessTimeUtc(path, lastAccessTimeUtc));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static Eff<RT, DateTime> getLastAccessTime(string path) =>
        self.Bind(rt => rt.GetLastAccessTime(path));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static Eff<RT, DateTime> getLastAccessTimeUtc(string path) =>
        self.Bind(rt => rt.GetLastAccessTimeUtc(path));

    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path) =>
        self.Bind(rt => rt.EnumerateDirectories(path));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern) =>
        self.Bind(rt => rt.EnumerateDirectories(path, searchPattern));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        self.Bind(rt => rt.EnumerateDirectories(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path) =>
        self.Bind(rt => rt.EnumerateFiles(path));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern) =>
        self.Bind(rt => rt.EnumerateFiles(path, searchPattern));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        self.Bind(rt => rt.EnumerateFiles(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path) =>
        self.Bind(rt => rt.EnumerateFileSystemEntries(path));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern) =>
        self.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        self.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern, searchOption));

    /// <summary>
    /// Get the root of the path provided
    /// </summary>
    public static Eff<RT, string> getRoot(string path) =>
        self.Bind(rt => rt.GetDirectoryRoot(path));

    /// <summary>
    /// Get the current directory
    /// </summary>
    public static Eff<RT, string> current =>
        self.Bind(rt => rt.GetCurrentDirectory());

    /// <summary>
    /// Set the current directory
    /// </summary>
    /// <param name="path"></param>
    public static Eff<RT, Unit> setCurrent(string path) =>
        self.Bind(rt => rt.SetCurrentDirectory(path));

    /// <summary>
    /// Move a directory
    /// </summary>
    public static Eff<RT, Unit> move(string sourceDirName, string destDirName) =>
        self.Bind(rt => rt.Move(sourceDirName, destDirName));

    /// <summary>
    /// Get the logical drives
    /// </summary>
    public static Eff<RT, Seq<string>> logicalDrives =>
        self.Bind(rt => rt.GetLogicalDrives());
}
