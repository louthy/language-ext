using System.IO;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Sys.IO;

/// <summary>
/// File IO 
/// </summary>
public class File<RT>
    where RT : 
        Has<Eff<RT>, FileIO>, 
        Has<Eff<RT>, EncodingIO>
{
    /// <summary>
    /// Copy file 
    /// </summary>
    /// <param name="fromPath">Source path</param>
    /// <param name="toPath">Destination path</param>
    /// <param name="overwrite">Overwrite if the file already exists at the destination</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <returns>Unit</returns>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> copy(string fromPath, string toPath, bool overwrite = false) =>
        File<Eff<RT>, RT>.copy(fromPath, toPath, overwrite).As();

    /// <summary>
    /// Append lines to the end of the file provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> appendAllLines(string path, IEnumerable<string> contents) =>
        File<Eff<RT>, RT>.appendAllLines(path, contents).As();

    /// <summary>
    /// Read all of the lines from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Seq<string>> readAllLines(string path) =>
        File<Eff<RT>, RT>.readAllLines(path).As();

    /// <summary>
    /// Write all of the lines to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> writeAllLines(string path, Seq<string> lines) =>
        File<Eff<RT>, RT>.writeAllLines(path, lines).As();

    /// <summary>
    /// Read all of the text from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, string> readAllText(string path) =>
        File<Eff<RT>, RT>.readAllText(path).As();

    /// <summary>
    /// Read all of the data from the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, byte[]> readAllBytes(string path) =>
        File<Eff<RT>, RT>.readAllBytes(path).As();

    /// <summary>
    /// Write all of the text to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> writeAllText(string path, string text) =>
        File<Eff<RT>, RT>.writeAllText(path, text).As();

    /// <summary>
    /// Write all of the data to the path provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> writeAllBytes(string path, byte[] data) =>
        File<Eff<RT>, RT>.writeAllBytes(path, data).As();

    /// <summary>
    /// Delete the file provided
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> delete(string path) =>
        File<Eff<RT>, RT>.delete(path).As();

    /// <summary>
    /// True if a file exists at the path
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, bool> exists(string path) =>
        File<Eff<RT>, RT>.exists(path).As();

    /// <summary>
    /// Open a text file
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<RT, TextReader, Unit> openText(string path) => 
        File<Eff<RT>, RT>.openText(path);

    /// <summary>
    /// Create a new text file to stream to
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, TextWriter> createText(string path) =>
        File<Eff<RT>, RT>.createText(path).As();

    /// <summary>
    /// Return a stream to append text to
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, TextWriter> appendText(string path) =>
        File<Eff<RT>, RT>.appendText(path).As();

    /// <summary>
    /// Open a file-stream
    /// </summary>
    public static Producer<RT, Stream, Unit> openRead(string path) =>
        File<Eff<RT>, RT>.openRead(path);

    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<RT, Stream, Unit> open(string path, FileMode mode) =>
        File<Eff<RT>, RT>.open(path, mode);
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<RT, Stream, Unit> open(string path, FileMode mode, FileAccess access) =>
        File<Eff<RT>, RT>.open(path, mode, access);
        
    /// <summary>
    /// Open a file-stream
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Producer<RT, Stream, Unit> openWrite(string path) =>
        File<Eff<RT>, RT>.openWrite(path);
}
