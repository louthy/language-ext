using System;
using System.IO;

namespace LanguageExt.Sys.Test.Implementations;

public record DirectoryIO(string root) : Traits.DirectoryIO
{
    string FixPath(string path) =>
        Path.Combine(root, path.Replace(":", "_drive"));
    
    public IO<DirectoryInfo> Create(string path) =>
        Live.Implementations.DirectoryIO.Default.Create(FixPath(path));

    public IO<Unit> Delete(string path, bool recursive = true) =>
        Live.Implementations.DirectoryIO.Default.Delete(FixPath(path), recursive);

    public IO<Option<DirectoryInfo>> GetParent(string path) =>
        Live.Implementations.DirectoryIO.Default.GetParent(FixPath(path));

    public IO<bool> Exists(string path) =>
        Live.Implementations.DirectoryIO.Default.Exists(FixPath(path));

    public IO<Unit> SetCreationTime(string path, DateTime creationTime) =>
        Live.Implementations.DirectoryIO.Default.SetCreationTime(FixPath(path), creationTime);

    public IO<Unit> SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        Live.Implementations.DirectoryIO.Default.SetCreationTimeUtc(FixPath(path), creationTimeUtc);

    public IO<DateTime> GetCreationTime(string path) =>
        Live.Implementations.DirectoryIO.Default.GetCreationTime(FixPath(path));

    public IO<DateTime> GetCreationTimeUtc(string path) =>
        Live.Implementations.DirectoryIO.Default.GetCreationTimeUtc(FixPath(path));

    public IO<Unit> SetLastWriteTime(string path, DateTime lastWriteTime) =>
        Live.Implementations.DirectoryIO.Default.SetLastWriteTime(FixPath(path), lastWriteTime);

    public IO<Unit> SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        Live.Implementations.DirectoryIO.Default.SetLastWriteTimeUtc(FixPath(path), lastWriteTimeUtc);

    public IO<DateTime> GetLastWriteTime(string path) =>
        Live.Implementations.DirectoryIO.Default.GetLastWriteTime(FixPath(path));

    public IO<DateTime> GetLastWriteTimeUtc(string path) =>
        Live.Implementations.DirectoryIO.Default.GetLastWriteTimeUtc(FixPath(path));

    public IO<Unit> SetLastAccessTime(string path, DateTime lastAccessTime) =>
        Live.Implementations.DirectoryIO.Default.SetLastAccessTime(FixPath(path), lastAccessTime);

    public IO<Unit> SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        Live.Implementations.DirectoryIO.Default.SetLastAccessTimeUtc(FixPath(path), lastAccessTimeUtc);

    public IO<DateTime> GetLastAccessTime(string path) =>
        Live.Implementations.DirectoryIO.Default.GetLastAccessTime(FixPath(path));

    public IO<DateTime> GetLastAccessTimeUtc(string path) =>
        Live.Implementations.DirectoryIO.Default.GetLastAccessTimeUtc(FixPath(path));

    public IO<Seq<string>> EnumerateDirectories(string path) =>
        Live.Implementations.DirectoryIO.Default.EnumerateDirectories(FixPath(path));

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern) =>
        Live.Implementations.DirectoryIO.Default.EnumerateDirectories(FixPath(path), searchPattern);

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Live.Implementations.DirectoryIO.Default.EnumerateDirectories(FixPath(path), searchPattern, searchOption);

    public IO<Seq<string>> EnumerateFiles(string path) =>
        Live.Implementations.DirectoryIO.Default.EnumerateFiles(FixPath(path));

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern) =>
        Live.Implementations.DirectoryIO.Default.EnumerateFiles(FixPath(path), searchPattern);

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Live.Implementations.DirectoryIO.Default.EnumerateFiles(FixPath(path), searchPattern, searchOption);

    public IO<Seq<string>> EnumerateFileSystemEntries(string path) =>
        Live.Implementations.DirectoryIO.Default.EnumerateFileSystemEntries(FixPath(path));

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern) =>
        Live.Implementations.DirectoryIO.Default.EnumerateFileSystemEntries(FixPath(path), searchPattern);

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)  =>
        Live.Implementations.DirectoryIO.Default.EnumerateFileSystemEntries(FixPath(path), searchPattern, searchOption);

    public IO<string> GetDirectoryRoot(string path) =>
        Live.Implementations.DirectoryIO.Default.GetDirectoryRoot(FixPath(path));

    public IO<string> GetCurrentDirectory()  =>
        Live.Implementations.DirectoryIO.Default.GetCurrentDirectory();

    public IO<Unit> SetCurrentDirectory(string path) =>
        Live.Implementations.DirectoryIO.Default.SetCurrentDirectory(FixPath(path));

    public IO<Unit> Move(string sourceDirName, string destDirName) =>
        Live.Implementations.DirectoryIO.Default.Move(FixPath(sourceDirName), FixPath(destDirName));

    public IO<Seq<string>> GetLogicalDrives() =>
        Live.Implementations.DirectoryIO.Default.GetLogicalDrives();
}
