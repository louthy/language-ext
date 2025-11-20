using System;
using System.IO;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.IO;

public class Directory<M, RT>
    where M : 
        MonadIO<M>  
    where RT : 
        Has<M, DirectoryIO>
{
    static K<M, DirectoryIO> directoryIO => 
        Has<M, RT, DirectoryIO>.ask;
    
    /// <summary>
    /// Create a directory
    /// </summary>
    public static K<M, DirectoryInfo> create(string path) =>
        directoryIO.Bind(rt => rt.Create(path));

    /// <summary>
    /// Delete a directory
    /// </summary>
    public static K<M, Unit> delete(string path, bool recursive = true) =>
        directoryIO.Bind(rt => rt.Delete(path, recursive));
        
    /// <summary>
    /// Get parent directory
    /// </summary>
    public static K<M, Option<DirectoryInfo>> getParent(string path) =>
        directoryIO.Bind(rt => rt.GetParent(path));

    /// <summary>
    /// Check if directory exists
    /// </summary>
    public static K<M, bool> exists(string path) =>
        directoryIO.Bind(rt => rt.Exists(path));
     
    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static K<M, Unit> setCreationTime(string path, DateTime creationTime) =>
        directoryIO.Bind(rt => rt.SetCreationTime(path, creationTime));

    /// <summary>
    /// Set the directory creation time
    /// </summary>
    public static K<M, Unit> setCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        directoryIO.Bind(rt => rt.SetCreationTimeUtc(path, creationTimeUtc));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static K<M, DateTime> getCreationTime(string path) =>
        directoryIO.Bind(rt => rt.GetCreationTime(path));

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    public static K<M, DateTime> getCreationTimeUtc(string path) =>
        directoryIO.Bind(rt => rt.GetCreationTimeUtc(path));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static K<M, Unit> setLastWriteTime(string path, DateTime lastWriteTime) =>
        directoryIO.Bind(rt => rt.SetLastWriteTime(path, lastWriteTime));

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    public static K<M, Unit> setLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        directoryIO.Bind(rt => rt.SetLastWriteTimeUtc(path, lastWriteTimeUtc));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static K<M, DateTime> getLastWriteTime(string path) =>
        directoryIO.Bind(rt => rt.GetLastWriteTime(path));

    /// <summary>
    /// Get the directory last write time
    /// </summary>
    public static K<M, DateTime> getLastWriteTimeUtc(string path) =>
        directoryIO.Bind(rt => rt.GetLastWriteTimeUtc(path));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static K<M, Unit> setLastAccessTime(string path, DateTime lastAccessTime) =>
        directoryIO.Bind(rt => rt.SetLastAccessTime(path, lastAccessTime));

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    public static K<M, Unit> setLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        directoryIO.Bind(rt => rt.SetLastAccessTimeUtc(path, lastAccessTimeUtc));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static K<M, DateTime> getLastAccessTime(string path) =>
        directoryIO.Bind(rt => rt.GetLastAccessTime(path));
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    public static K<M, DateTime> getLastAccessTimeUtc(string path) =>
        directoryIO.Bind(rt => rt.GetLastAccessTimeUtc(path));

    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path) =>
        directoryIO.Bind(rt => rt.EnumerateDirectories(path));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path, string searchPattern) =>
        directoryIO.Bind(rt => rt.EnumerateDirectories(path, searchPattern));
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    public static K<M, Seq<string>> enumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        directoryIO.Bind(rt => rt.EnumerateDirectories(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path) =>
        directoryIO.Bind(rt => rt.EnumerateFiles(path));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path, string searchPattern) =>
        directoryIO.Bind(rt => rt.EnumerateFiles(path, searchPattern));
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    public static K<M, Seq<string>> enumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        directoryIO.Bind(rt => rt.EnumerateFiles(path, searchPattern, searchOption));
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path) =>
        directoryIO.Bind(rt => rt.EnumerateFileSystemEntries(path));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern) =>
        directoryIO.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern));

    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    public static K<M, Seq<string>> enumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        directoryIO.Bind(rt => rt.EnumerateFileSystemEntries(path, searchPattern, searchOption));

    /// <summary>
    /// Get the root of the path provided
    /// </summary>
    public static K<M, string> getRoot(string path) =>
        directoryIO.Bind(rt => rt.GetDirectoryRoot(path));

    /// <summary>
    /// Get the current directory
    /// </summary>
    public static K<M, string> current =>
        directoryIO.Bind(rt => rt.GetCurrentDirectory());

    /// <summary>
    /// Set the current directory
    /// </summary>
    /// <param name="path"></param>
    public static K<M, Unit> setCurrent(string path) =>
        directoryIO.Bind(rt => rt.SetCurrentDirectory(path));

    /// <summary>
    /// Move a directory
    /// </summary>
    public static K<M, Unit> move(string sourceDirName, string destDirName) =>
        directoryIO.Bind(rt => rt.Move(sourceDirName, destDirName));

    /// <summary>
    /// Get the logical drives
    /// </summary>
    public static K<M, Seq<string>> logicalDrives =>
        directoryIO.Bind(rt => rt.GetLogicalDrives());
}
