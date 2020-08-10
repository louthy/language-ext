using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.Interfaces;
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
            public static SIO<RT, ConsoleKeyInfo> readKey<RT>() 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.ReadKey());
            
            /// <summary>
            /// Clear the console
            /// </summary>
            public static SIO<RT, Unit> clear<RT>()
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.Clear());

            /// <summary>
            /// Read from the console
            /// </summary>
            public static SIO<RT, int> read<RT>() 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.Read());

            /// <summary>
            /// Read from the console
            /// </summary>
            public static SIO<RT, string> readLine<RT>() 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.ReadLine());

            /// <summary>
            /// Write an empty line to the console
            /// </summary>
            public static SIO<RT, Unit> writeEmptyLine<RT>() 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.WriteLine());

            /// <summary>
            /// Write a line to the console
            /// </summary>
            public static SIO<RT, Unit> writeLine<RT>(string line) 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.WriteLine(line));

            /// <summary>
            /// Write a line to the console
            /// </summary>
            public static SIO<RT, string> writeLine2<RT>(string line) 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.WriteLine(line)).Map(_ => line);
            
            public static SIO<RT, Unit> setBgColor<RT>(ConsoleColor color) 
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.SetBgColor(color));

            public static SIO<RT, Unit> setColor<RT>(ConsoleColor color)
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.SetColor(color));

            public static SIO<RT, ConsoleColor> bgColor<RT>()
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.BgColor);
        
            public static SIO<RT, ConsoleColor> color<RT>()
                where RT : struct, HasConsole<RT> =>
                default(RT).ConsoleSIO.Map(e => e.Color);
        }
    }
}
