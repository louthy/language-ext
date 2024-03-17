using System;
using System.IO;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live.Implementations;

public record DirectoryIO : Traits.DirectoryIO
{
    public static readonly Traits.DirectoryIO Default = 
        new DirectoryIO();

    public IO<DirectoryInfo> Create(string path) =>
        lift(() => Directory.CreateDirectory(path));

    public IO<Unit> Delete(string path, bool recursive = true) =>
        lift(() => Directory.Delete(path, recursive));

    public IO<Option<DirectoryInfo>> GetParent(string path) =>
        lift(() => Optional(Directory.GetParent(path)));

    public IO<bool> Exists(string path) =>
        lift(() => Directory.Exists(path));

    public IO<Unit> SetCreationTime(string path, DateTime creationTime) =>
        lift(() => Directory.SetCreationTime(path, creationTime));

    public IO<Unit> SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        lift(() => Directory.SetCreationTimeUtc(path, creationTimeUtc));

    public IO<DateTime> GetCreationTime(string path) =>
        lift(() => Directory.GetCreationTime(path));

    public IO<DateTime> GetCreationTimeUtc(string path) =>
        lift(() => Directory.GetCreationTimeUtc(path));

    public IO<Unit> SetLastWriteTime(string path, DateTime lastWriteTime) =>
        lift(() => Directory.SetLastWriteTime(path, lastWriteTime));

    public IO<Unit> SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        lift(() => Directory.SetLastWriteTime(path, lastWriteTimeUtc));

    public IO<DateTime> GetLastWriteTime(string path) =>
        lift(() => Directory.GetLastWriteTime(path));

    public IO<DateTime> GetLastWriteTimeUtc(string path) =>
        lift(() => Directory.GetLastWriteTimeUtc(path));

    public IO<Unit> SetLastAccessTime(string path, DateTime lastAccessTime) =>
        lift(() => Directory.SetLastAccessTime(path, lastAccessTime));

    public IO<Unit> SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        lift(() => Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc));

    public IO<DateTime> GetLastAccessTime(string path) =>
        lift(() => Directory.GetLastAccessTime(path));

    public IO<DateTime> GetLastAccessTimeUtc(string path) =>
        lift(() => Directory.GetLastAccessTimeUtc(path));

    public IO<Seq<string>> EnumerateDirectories(string path) =>
        lift(() => Directory.EnumerateDirectories(path).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern) =>
        lift(() => Directory.EnumerateDirectories(path, searchPattern).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        lift(() => Directory.EnumerateDirectories(path, searchPattern, searchOption).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path) =>
        lift(() => Directory.EnumerateFiles(path).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern) =>
        lift(() => Directory.EnumerateFiles(path, searchPattern).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        lift(() => Directory.EnumerateFiles(path, searchPattern, searchOption).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path) =>
        lift(() => Directory.EnumerateFileSystemEntries(path).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern) =>
        lift(() => Directory.EnumerateFileSystemEntries(path, searchPattern).AsEnumerableM().ToSeq());

    public IO<Seq<string>> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)  =>
        lift(() => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption).AsEnumerableM().ToSeq());

    public IO<string> GetDirectoryRoot(string path) =>
        lift(() => Directory.GetDirectoryRoot(path));

    public IO<string> GetCurrentDirectory()  =>
        lift(Directory.GetCurrentDirectory);

    public IO<Unit> SetCurrentDirectory(string path) =>
        lift(() => Directory.SetCurrentDirectory(path));

    public IO<Unit> Move(string sourceDirName, string destDirName) =>
        lift(() => Directory.Move(sourceDirName, destDirName));

    public IO<Seq<string>> GetLogicalDrives() =>
        lift(() => Directory.GetLogicalDrives().ToSeq());
}
