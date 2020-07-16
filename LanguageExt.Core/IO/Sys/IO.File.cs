using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
            [Pure, MethodImpl(IO.mops)]
            public static SIO<Runtime, Unit> copy(string fromPath, string toPath, bool overwrite = false) =>
                Runtime.senv.Map(e => e.File.Copy(fromPath, toPath, overwrite));

            /// <summary>
            /// Append lines to the end of the file provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Unit> appendAllLines(string path, IEnumerable<string> contents) =>
                Runtime.env.MapAsync(e => e.File.AppendAllLines(path, contents, e.Encoding, e.CancelToken));

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Seq<string>> readAllLines(string path) =>
                Runtime.env.MapAsync(e => e.File.ReadAllLines(path, e.Encoding, e.CancelToken));

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Unit> writeAllLines(string path, Seq<string> lines) =>
                Runtime.env.MapAsync(e => e.File.WriteAllLines(path, lines, e.Encoding, e.CancelToken));

            /// <summary>
            /// Read all of the lines from the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, string> readAllText(string path) =>
                Runtime.env.MapAsync(e => e.File.ReadAllText(path, e.Encoding, e.CancelToken));

            /// <summary>
            /// Write all of the lines to the path provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Unit> writeAllText(string path, string text) =>
                Runtime.env.MapAsync(e => e.File.WriteAllText(path, text, e.Encoding, e.CancelToken));

            /// <summary>
            /// Delete the file provided
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<Runtime, Unit> delete(string path) =>
                Runtime.senv.Map(e => e.File.Delete(path));

            /// <summary>
            /// Open a text file
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<Runtime, TextReader> openText(string path) =>
                Runtime.senv.Map(e => e.File.OpenText(path));

            /// <summary>
            /// Create a new text file to stream to
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<Runtime, TextWriter> createText(string path) =>
                Runtime.senv.Map(e => e.File.CreateText(path));

            /// <summary>
            /// Return a stream to append text to
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<Runtime, TextWriter> appendText(string path) =>
                Runtime.senv.Map(e => e.File.AppendText(path));
        }
    }
}