using System;
using System.IO;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.IO;

public class Directory<M, RT>
    where RT : Has<M, DirectoryIO>
    where M : State<M, RT>, Monad<M>
{
    static readonly K<M, DirectoryIO> trait = 
        State.getsM<M, RT, DirectoryIO>(e => e.Trait); 
    
    /// <summary>
    /// Create a directory
    /// </summary>
    public static K<M, DirectoryInfo> create(string path) =>
        trait.Bind(rt => rt.Create(path));

    /// <summary>
    /// Delete a directory
    /// </summary>
    public static K<M, Unit> delete(string path, bool recursive = true) =>
        trait.Bind(rt => rt.Delete(path, recursive));
        
    /// <summary>
    /// Get parent directory
    /// </summary>
    public static K<M, Option<DirectoryInfo>> getParent(string path) =>
        trait.Bind(rt => rt.GetParent(path));

    /// <summary>
    /// Check if directory exists
    /// </summary>
    public static K<M, bool> exists(string path) =>
        trait.Bind(rt => rt.Exists(path));
     
    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static K<M, Unit> setCreationTime(string path, DateTime creationTime) =>
        trait.Bind(rt => rt.SetCreationTime(path, creationTime));

    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static K<M, Unit> setCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        trait.Bind(rt => rt.SetCreationTimeUtc(path, creationTimeUtc));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static K<M, DateTime> getCreationTime(string path) =>
        trait.Bind(rt => rt.GetCreationTime(path));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static K<M, DateTime> getCreationTimeUtc(string path) =>
        trait.Bind(rt => rt.GetCreationTimeUtc(path));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static K<M, Unit> setLastWriteTime(string path, DateTime lastWriteTime) =>
        trait.Bind(rt => rt.SetLastWriteTime(path, lastWriteTime));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static K<M, Unit> setLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        trait.Bind(rt => rt.SetLastWriteTimeUtc(path, lastWriteTimeUtc));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static K<M, DateTime> getLastWriteTime(string path) =>
        trait.Bind(rt => rt.GetLastWriteTime(path));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static K<M, DateTime> getLastWriteTimeUtc(string path) =>
        trait.Bind(rt => rt.GetLastWriteTimeUtc(path));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static K<M, Unit> setLastAccessTime(string path, DateTime lastAccessTime) =>
        trait.Bind(rt => rt.SetLastAccessTime(path, lastAccessTime));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static K<M, Unit> setLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        trait.Bind(rt => rt.SetLastAccessTimeUtc(path, lastAccessTimeUtc));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static K<M, DateTime> getLastAccessTime(string path) =>
        trait.Bind(rt => rt.GetLastAccessTime(path));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static K<M, DateTime> getLastAccessTimeUtc(string path) =>
        trait.Bind(rt => rt.GetLastAccessTimeUtc(path));

    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path) =>
        trait.Bind(rt => rt.EnumerateDirectories(path));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path, string searchPattern) =>
        trait.Bind(rt => rt.EnumerateDirectories(path, searchPattern));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        trait.Bind(rt => rt.EnumerateDirectories(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path) =>
        trait.Bind(rt => rt.EnumerateFiles(path));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path, string searchPattern) =>
        trait.Bind(rt => rt.EnumerateFiles(path, searchPattern));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        trait.Bind(rt => rt.EnumerateFiles(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path) =>
        trait.Bind(rt => rt.EnumerateFileSystemEntries(path));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern) =>
        trait.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        trait.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern, searchOption));

    /// <summary>
    /// Get the root of the path provided
    /// </summary>
    public static K<M, string> getRoot(string path) =>
        trait.Bind(rt => rt.GetDirectoryRoot(path));

    /// <summary>
    /// Get the current directory
    /// </summary>
    public static K<M, string> current =>
        trait.Bind(rt => rt.GetCurrentDirectory());

    /// <summary>
    /// Set the current directory
    /// </summary>
    /// <param name="path"></param>
    public static K<M, Unit> setCurrent(string path) =>
        trait.Bind(rt => rt.SetCurrentDirectory(path));

    /// <summary>
    /// Move a directory
    /// </summary>
    public static K<M, Unit> move(string sourceDirName, string destDirName) =>
        trait.Bind(rt => rt.Move(sourceDirName, destDirName));

    /// <summary>
    /// Get the logical drives
    /// </summary>
    public static K<M, Seq<string>> logicalDrives =>
        trait.Bind(rt => rt.GetLogicalDrives());
}
