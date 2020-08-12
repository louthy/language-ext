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
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, Unit> copy<RT>(string fromPath, string toPath, bool overwrite = false)
                where RT : struct, HasFile<RT> =>
                default(RT).FileEff.Map(e => e.Copy(fromPath, toPath, overwrite));

            /// <summary>
            /// Append lines to the end of the file provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, Unit> appendAllLines<RT>(string path, IEnumerable<string> contents)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileAff.MapAsync(e => e.AppendAllLines(path, contents, en, ct))
                select rs;

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, Seq<string>> readAllLines<RT>(string path)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileAff.MapAsync(e => e.ReadAllLines(path, en, ct))
                select rs;

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, Unit> writeAllLines<RT>(string path, Seq<string> lines)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileAff.MapAsync(e => e.WriteAllLines(path, lines, en, ct))
                select rs;

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, string> readAllText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileAff.MapAsync(e => e.ReadAllText(path, en, ct))
                select rs;

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, Unit> writeAllText<RT>(string path, string text)
                where RT : struct, HasFile<RT> =>
                from ct in cancelToken<RT>()
                from en in encoding<RT>()
                from rs in default(RT).FileAff.MapAsync(e => e.WriteAllText(path, text, en, ct))
                select rs;

            /// <summary>
            /// Delete the file provided
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, Unit> delete<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileEff.Map(e => e.Delete(path));

            /// <summary>
            /// Open a text file
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, TextReader> openText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileEff.Map(e => e.OpenText(path));

            /// <summary>
            /// Create a new text file to stream to
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, TextWriter> createText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileEff.Map(e => e.CreateText(path));

            /// <summary>
            /// Return a stream to append text to
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, TextWriter> appendText<RT>(string path)
                where RT : struct, HasFile<RT> =>
                default(RT).FileEff.Map(e => e.AppendText(path));
        }
    }
}
