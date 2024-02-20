using System;
using System.IO;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test.Implementations;

public record DirectoryIO(MemoryFS fs, DateTime now) : Traits.DirectoryIO
{
    public IO<DirectoryInfo> Create(string path) =>
        lift(() => fs.CreateFolder(path, now));

    public IO<Unit> Delete(string path, bool recursive = true) =>
        lift(() => fs.DeleteFolder(path, recursive, now));

    public IO<Option<DirectoryInfo>> GetParent(string path) =>
        Live.Implementations.DirectoryIO.Default.GetParent(path);

    public IO<bool> Exists(string path) =>
        lift(() => fs.FolderExists(path));

    public IO<Unit> SetCreationTime(string path, DateTime creationTime) =>
        lift(() => fs.SetFolderCreationTime(path, creationTime.ToLocalTime(), now));

    public IO<Unit> SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        lift(() => fs.SetFolderCreationTime(path, creationTimeUtc.ToUniversalTime(), now));

    public IO<DateTime> GetCreationTime(string path) =>
        lift(() => fs.GetFolderCreationTime(path).ToLocalTime());

    public IO<DateTime> GetCreationTimeUtc(string path) =>
        lift(() => fs.GetFolderCreationTime(path).ToUniversalTime());

    public IO<Unit> SetLastWriteTime(string path, DateTime lastWriteTime) =>
        lift(() => fs.SetFolderLastWriteTime(path, lastWriteTime.ToLocalTime(), now));

    public IO<Unit> SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        lift(() => fs.SetFolderLastWriteTime(path, lastWriteTimeUtc.ToUniversalTime(), now));

    public IO<DateTime> GetLastWriteTime(string path) =>
        lift(() => fs.GetFolderLastWriteTime(path).ToLocalTime());

    public IO<DateTime> GetLastWriteTimeUtc(string path) =>
        lift(() => fs.GetFolderLastWriteTime(path).ToUniversalTime());

    public IO<Unit> SetLastAccessTime(string path, DateTime lastAccessTime) =>
        lift(() => fs.SetFolderLastAccessTime(path, lastAccessTime.ToLocalTime(), now));

    public IO<Unit> SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        lift(() => fs.SetFolderLastAccessTime(path, lastAccessTimeUtc.ToUniversalTime(), now));

    public IO<DateTime> GetLastAccessTime(string path) =>
        lift(() => fs.GetFolderLastAccessTime(path).ToLocalTime());

    public IO<DateTime> GetLastAccessTimeUtc(string path) =>
        lift(() => fs.GetFolderLastAccessTime(path).ToUniversalTime());

    public IO<Seq<string>> EnumerateDirectories(string path) =>
        lift(() => fs.EnumerateFolders(path, "*", SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern) =>
        lift(() => fs.EnumerateFolders(path, searchPattern, SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        lift(() => fs.EnumerateFolders(path, searchPattern, searchOption).ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path) =>
        lift(() => fs.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern) =>
        lift(() => fs.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        lift(() => fs.EnumerateFiles(path, searchPattern, searchOption).ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path) =>
        lift(() => fs.EnumerateEntries(path, "*", SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern) =>
        lift(() => fs.EnumerateEntries(path, searchPattern, SearchOption.TopDirectoryOnly).ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        lift(() => fs.EnumerateEntries(path, searchPattern, searchOption).ToSeq());

    public IO<string> GetDirectoryRoot(string path) =>
        Live.Implementations.DirectoryIO.Default.GetDirectoryRoot(path);

    public IO<string> GetCurrentDirectory() =>
        lift(() => fs.CurrentDir);

    public IO<Unit> SetCurrentDirectory(string path) =>
        lift(() => ignore(fs.CurrentDir = path));

    public IO<Unit> Move(string sourceDirName, string destDirName) =>
        lift(() => fs.Move(sourceDirName, destDirName, now));

    public IO<Seq<string>> GetLogicalDrives() =>
        lift(() => fs.GetLogicalDrives());
}
