using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    /// <summary>
    /// IO prelude
    /// </summary>
    public static partial class IO
    {
        /// <summary>
        /// File IO 
        /// </summary>
        public static class File
        {
            /// <summary>
            /// Copy file 
            /// </summary>
            /// <param name="fromPath">Source path</param>
            /// <param name="toPath">Destination path</param>
            /// <param name="overwrite">Overwrite if the file already exists at the destination</param>
            /// <typeparam name="RT">Runtime</typeparam>
            /// <returns>Unit</returns>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, Unit> copy<RT>(string fromPath, string toPath, bool overwrite = false)
                where RT : struct, HasFile<RT> =>
                default(RT).FileSIO.Map(e => e.Copy(fromPath, toPath, overwrite));

            /// <summary>
            /// Append lines to the end of the file provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Unit> appendAllLines<RT>(string path, IEnumerable<string> contents)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileIO.MapAsync(e => e.AppendAllLines(path, contents, en, ct))
                select rs;

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Seq<string>> readAllLines<RT>(string path)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileIO.MapAsync(e => e.ReadAllLines(path, en, ct))
                select rs;

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Unit> writeAllLines<RT>(string path, Seq<string> lines)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileIO.MapAsync(e => e.WriteAllLines(path, lines, en, ct))
                select rs;

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, string> readAllText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileIO.MapAsync(e => e.ReadAllText(path, en, ct))
                select rs;

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Unit> writeAllText<RT>(string path, string text)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileIO.MapAsync(e => e.WriteAllText(path, text, en, ct))
                select rs;

            /// <summary>
            /// Delete the file provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, Unit> delete<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileSIO.Map(e => e.Delete(path));

            /// <summary>
            /// Open a text file
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, TextReader> openText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileSIO.Map(e => e.OpenText(path));

            /// <summary>
            /// Create a new text file to stream to
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, TextWriter> createText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileSIO.Map(e => e.CreateText(path));

            /// <summary>
            /// Return a stream to append text to
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, TextWriter> appendText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileSIO.Map(e => e.AppendText(path));
        }
    }
}
