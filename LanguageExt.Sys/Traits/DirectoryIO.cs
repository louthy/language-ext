using System;
using System.IO;

namespace LanguageExt.Sys.Traits;

public interface DirectoryIO
{
    /// <summary>
    /// Create a directory
    /// </summary>
    IO<DirectoryInfo> Create(string path);

    /// <summary>
    /// Delete a directory
    /// </summary>
    IO<Unit> Delete(string path, bool recursive = true);
        
    /// <summary>
    /// Get parent directory
    /// </summary>
    IO<Option<DirectoryInfo>> GetParent(string path);

    /// <summary>
    /// Check if directory exists
    /// </summary>
    IO<bool> Exists(string path);
        
    /// <summary>
    /// Set the directory creation time
    /// </summary>
    IO<Unit> SetCreationTime(string path, DateTime creationTime);

    /// <summary>
    /// Set the directory creation time
    /// </summary>
    IO<Unit> SetCreationTimeUtc(string path, DateTime creationTimeUtc);

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    IO<DateTime> GetCreationTime(string path);

    /// <summary>
    /// Get the directory creation time
    /// </summary>
    IO<DateTime> GetCreationTimeUtc(string path);
        
    /// <summary>
    /// Set the directory last write time
    /// </summary>
    IO<Unit> SetLastWriteTime(string path, DateTime lastWriteTime);

    /// <summary>
    /// Set the directory last write time
    /// </summary>
    IO<Unit> SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
        
    /// <summary>
    /// Get the directory last write time
    /// </summary>
    IO<DateTime> GetLastWriteTime(string path);
        
    /// <summary>
    /// Get the directory last write time
    /// </summary>
    IO<DateTime> GetLastWriteTimeUtc(string path);

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    IO<Unit> SetLastAccessTime(string path, DateTime lastAccessTime);

    /// <summary>
    /// Set the directory last access time
    /// </summary>
    IO<Unit> SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    IO<DateTime> GetLastAccessTime(string path);
        
    /// <summary>
    /// Get the directory last access time
    /// </summary>
    IO<DateTime> GetLastAccessTimeUtc(string path);

    /// <summary>
    /// Enumerate directories
    /// </summary>
    IO<Seq<string>> EnumerateDirectories(string path);
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    IO<Seq<string>> EnumerateDirectories(string path, string searchPattern);
        
    /// <summary>
    /// Enumerate directories
    /// </summary>
    IO<Seq<string>> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    IO<Seq<string>> EnumerateFiles(string path);
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    IO<Seq<string>> EnumerateFiles(string path, string searchPattern);
        
    /// <summary>
    /// Enumerate files
    /// </summary>
    IO<Seq<string>> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    IO<Seq<string>> EnumerateFileSystemEntries(string path);
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern);
        
    /// <summary>
    /// Enumerate file system entries
    /// </summary>
    IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption);
        
    /// <summary>
    /// Get the root of the path provided
    /// </summary>
    IO<string> GetDirectoryRoot(string path);
        
    /// <summary>
    /// Get the current directory
    /// </summary>
    IO<string> GetCurrentDirectory();

    /// <summary>
    /// Set the current directory
    /// </summary>
    /// <param name="path"></param>
    IO<Unit> SetCurrentDirectory(string path);
        
    /// <summary>
    /// Move a directory
    /// </summary>
    IO<Unit> Move(string sourceDirName, string destDirName);
        
    /// <summary>
    /// Get the logical drives
    /// </summary>
    IO<Seq<string>> GetLogicalDrives();
}
