using System;
using System.IO;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    public interface DirectoryIO
    {
        /// <summary>
        /// Create a directory
        /// </summary>
        Unit Create(string path);

        /// <summary>
        /// Delete a directory
        /// </summary>
        Unit Delete(string path, bool recursive = true);
        
        /// <summary>
        /// Get parent directory
        /// </summary>
        Option<DirectoryInfo> GetParent(string path);

        /// <summary>
        /// Check if directory exists
        /// </summary>
        bool Exists(string path);
        
        /// <summary>
        /// Set the directory creation time
        /// </summary>
        Unit SetCreationTime(string path, DateTime creationTime);

        /// <summary>
        /// Set the directory creation time
        /// </summary>
        Unit SetCreationTimeUtc(string path, DateTime creationTimeUtc);

        /// <summary>
        /// Get the directory creation time
        /// </summary>
        DateTime GetCreationTime(string path);

        /// <summary>
        /// Get the directory creation time
        /// </summary>
        DateTime GetCreationTimeUtc(string path);
        
        /// <summary>
        /// Set the directory last write time
        /// </summary>
        Unit SetLastWriteTime(string path, DateTime lastWriteTime);

        /// <summary>
        /// Set the directory last write time
        /// </summary>
        Unit SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
        
        /// <summary>
        /// Get the directory last write time
        /// </summary>
        DateTime GetLastWriteTime(string path);
        
        /// <summary>
        /// Get the directory last write time
        /// </summary>
        DateTime GetLastWriteTimeUtc(string path);

        /// <summary>
        /// Set the directory last access time
        /// </summary>
        Unit SetLastAccessTime(string path, DateTime lastAccessTime);

        /// <summary>
        /// Set the directory last access time
        /// </summary>
        Unit SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);
        
        /// <summary>
        /// Get the directory last access time
        /// </summary>
        DateTime GetLastAccessTime(string path);
        
        /// <summary>
        /// Get the directory last access time
        /// </summary>
        DateTime GetLastAccessTimeUtc(string path);

        /// <summary>
        /// Enumerate directories
        /// </summary>
        Seq<string> EnumerateDirectories(string path);
        
        /// <summary>
        /// Enumerate directories
        /// </summary>
        Seq<string> EnumerateDirectories(string path, string searchPattern);
        
        /// <summary>
        /// Enumerate directories
        /// </summary>
        Seq<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        Seq<string> EnumerateFiles(string path);
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        Seq<string> EnumerateFiles(string path, string searchPattern);
        
        /// <summary>
        /// Enumerate files
        /// </summary>
        Seq<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
        
        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        Seq<string> EnumerateFileSystemEntries(string path);
        
        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        Seq<string> EnumerateFileSystemEntries(string path, string searchPattern);
        
        /// <summary>
        /// Enumerate file system entries
        /// </summary>
        Seq<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption);
        
        /// <summary>
        /// Get the root of the path provided
        /// </summary>
        string GetDirectoryRoot(string path);
        
        /// <summary>
        /// Get the current directory
        /// </summary>
        string GetCurrentDirectory();

        /// <summary>
        /// Set the current directory
        /// </summary>
        /// <param name="path"></param>
        Unit SetCurrentDirectory(string path);
        
        /// <summary>
        /// Move a directory
        /// </summary>
        Unit Move(string sourceDirName, string destDirName);
        
        /// <summary>
        /// Get the logical drives
        /// </summary>
        Seq<string> GetLogicalDrives();
    }
    
    /// <summary>
    /// Type-class giving a struct the trait of supporting File IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasDirectory<RT>
        where RT : struct, HasDirectory<RT>
    {
        /// <summary>
        /// Access the directory synchronous effect environment
        /// </summary>
        /// <returns>Directory synchronous effect environment</returns>
        Eff<RT, DirectoryIO> DirectoryEff { get; }
    }
}
