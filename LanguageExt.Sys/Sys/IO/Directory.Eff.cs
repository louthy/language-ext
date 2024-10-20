using System;
using System.IO;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.IO;

public class Directory<RT>
    where RT : 
        Has<Eff<RT>, DirectoryIO>
{
    /// <summary>
    /// Create a directory
    /// </summary>
    public static Eff<RT, DirectoryInfo> create(string path) =>
        Directory<Eff<RT>, RT>.create(path).As();

    /// <summary>
    /// Delete a directory
    /// </summary>
    public static Eff<RT, Unit> delete(string path, bool recursive = true) =>
        Directory<Eff<RT>, RT>.delete(path, recursive).As();
        
    /// <summary>
    /// Get parent directory
    /// </summary>
    public static Eff<RT, Option<DirectoryInfo>> getParent(string path) =>
        Directory<Eff<RT>, RT>.getParent(path).As();

    /// <summary>
    /// Check if directory exists
    /// </summary>
    public static Eff<RT, bool> exists(string path) =>
        Directory<Eff<RT>, RT>.exists(path).As();
     
    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static Eff<RT, Unit> setCreationTime(string path, DateTime creationTime) =>
        Directory<Eff<RT>, RT>.setCreationTime(path, creationTime).As();

    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static Eff<RT, Unit> setCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        Directory<Eff<RT>, RT>.setCreationTimeUtc(path, creationTimeUtc).As();

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static Eff<RT, DateTime> getCreationTime(string path) =>
        Directory<Eff<RT>, RT>.getCreationTime(path).As();

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static Eff<RT, DateTime> getCreationTimeUtc(string path) =>
        Directory<Eff<RT>, RT>.getCreationTimeUtc(path).As();

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static Eff<RT, Unit> setLastWriteTime(string path, DateTime lastWriteTime) =>
        Directory<Eff<RT>, RT>.setLastWriteTime(path, lastWriteTime).As();

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static Eff<RT, Unit> setLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        Directory<Eff<RT>, RT>.setLastWriteTimeUtc(path, lastWriteTimeUtc).As();

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static Eff<RT, DateTime> getLastWriteTime(string path) =>
        Directory<Eff<RT>, RT>.getLastWriteTime(path).As();

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static Eff<RT, DateTime> getLastWriteTimeUtc(string path) =>
        Directory<Eff<RT>, RT>.getLastWriteTimeUtc(path).As();

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static Eff<RT, Unit> setLastAccessTime(string path, DateTime lastAccessTime) =>
        Directory<Eff<RT>, RT>.setLastAccessTime(path, lastAccessTime).As();

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static Eff<RT, Unit> setLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        Directory<Eff<RT>, RT>.setLastAccessTimeUtc(path, lastAccessTimeUtc).As();
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static Eff<RT, DateTime> getLastAccessTime(string path) =>
        Directory<Eff<RT>, RT>.getLastAccessTime(path).As();
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static Eff<RT, DateTime> getLastAccessTimeUtc(string path) =>
        Directory<Eff<RT>, RT>.getLastAccessTimeUtc(path).As();

    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path) =>
        Directory<Eff<RT>, RT>.enumerateDirectories(path).As();
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern) =>
        Directory<Eff<RT>, RT>.enumerateDirectories(path, searchPattern).As();
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Directory<Eff<RT>, RT>.enumerateDirectories(path, searchPattern, searchOption).As();
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path) =>
        Directory<Eff<RT>, RT>.enumerateFiles(path).As();
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern) =>
        Directory<Eff<RT>, RT>.enumerateFiles(path, searchPattern).As();
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory<Eff<RT>, RT>.enumerateFiles(path, searchPattern, searchOption).As();
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path) =>
        Directory<Eff<RT>, RT>.enumerateFileSystemEntries(path).As();

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern) =>
        Directory<Eff<RT>, RT>.enumerateFileSystemEntries(path, searchPattern).As();

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static Eff<RT, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        Directory<Eff<RT>, RT>.enumerateFileSystemEntries(path, searchPattern, searchOption).As();

    /// <summary>
    /// Get the root of the path provided
    /// </summary>
    public static Eff<RT, string> getRoot(string path) =>
        Directory<Eff<RT>, RT>.getRoot(path).As();

    /// <summary>
    /// Get the current directory
    /// </summary>
    public static Eff<RT, string> current =>
        Directory<Eff<RT>, RT>.current.As();

    /// <summary>
    /// Set the current directory
    /// </summary>
    /// <param name="path"></param>
    public static Eff<RT, Unit> setCurrent(string path) =>
        Directory<Eff<RT>, RT>.setCurrent(path).As();

    /// <summary>
    /// Move a directory
    /// </summary>
    public static Eff<RT, Unit> move(string sourceDirName, string destDirName) =>
        Directory<Eff<RT>, RT>.move(sourceDirName, destDirName).As();

    /// <summary>
    /// Get the logical drives
    /// </summary>
    public static Eff<RT, Seq<string>> logicalDrives =>
        Directory<Eff<RT>, RT>.logicalDrives.As();
}
