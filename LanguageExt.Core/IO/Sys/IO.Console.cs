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
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;
 
        /// <summary>
        /// Time IO 
        /// </summary>
        public static class Console
        {
            /// <summary>
            /// Read a key from the console
            /// </summary>
            public static SIO<Runtime, ConsoleKeyInfo> readKey =>
                Runtime.senv.Map(e => e.Console.ReadKey());
            
            /// <summary>
            /// Clear the console
            /// </summary>
            public static SIO<Runtime, Unit> clear =>
                Runtime.senv.Map(e => e.Console.Clear());

            /// <summary>
            /// Read from the console
            /// </summary>
            public static SIO<Runtime, int> read =>
                Runtime.senv.Map(e => e.Console.Read());

            /// <summary>
            /// Read from the console
            /// </summary>
            public static SIO<Runtime, string> readLine =>
                Runtime.senv.Map(e => e.Console.ReadLine());

            /// <summary>
            /// Write an empty line to the console
            /// </summary>
            public static SIO<Runtime, Unit> writeEmptyLine =>
                Runtime.senv.Map(e => e.Console.WriteLine());

            /// <summary>
            /// Write a line to the console
            /// </summary>
            public static SIO<Runtime, Unit> writeLine(string line) =>
                Runtime.senv.Map(e => e.Console.WriteLine(line));

            /// <summary>
            /// Write a line to the console
            /// </summary>
            public static SIO<Runtime, string> writeLine2(string line) =>
                Runtime.senv.Map(e => e.Console.WriteLine(line)).Map(_ => line);
            
            public static SIO<Runtime, Unit> setBgColor(ConsoleColor color) =>
                Runtime.senv.Map(e => e.Console.SetBgColor(color));

            public static SIO<Runtime, Unit> setColor(ConsoleColor color) =>
                Runtime.senv.Map(e => e.Console.SetColor(color));

            public static SIO<Runtime, ConsoleColor> bgColor =>
                Runtime.senv.Map(e => e.Console.BgColor);
        
            public static SIO<Runtime, ConsoleColor> color =>
                Runtime.senv.Map(e => e.Console.Color);
        }
    }
}
