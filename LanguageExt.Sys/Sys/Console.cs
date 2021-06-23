using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys
{
    /// <summary>
    /// Time IO 
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// Read a key from the console
        /// </summary>
        public static Eff<RT, ConsoleKeyInfo> readKey<RT>() 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.ReadKey());
        
        /// <summary>
        /// Clear the console
        /// </summary>
        public static Eff<RT, Unit> clear<RT>()
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.Clear());

        /// <summary>
        /// Read from the console
        /// </summary>
        public static Eff<RT, int> read<RT>() 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.Read());

        /// <summary>
        /// Read from the console
        /// </summary>
        public static Eff<RT, string> readLine<RT>() 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.ReadLine());

        /// <summary>
        /// Write an empty line to the console
        /// </summary>
        public static Eff<RT, Unit> writeEmptyLine<RT>() 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.WriteLine());

        /// <summary>
        /// Write a line to the console
        /// </summary>
        public static Eff<RT, Unit> writeLine<RT>(string line) 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.WriteLine(line));

        /// <summary>
        /// Write a line to the console
        /// </summary>
        public static Eff<RT, string> writeLine2<RT>(string line) 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.WriteLine(line)).Map(_ => line);
        
        public static Eff<RT, Unit> setBgColor<RT>(ConsoleColor color) 
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.SetBgColor(color));

        public static Eff<RT, Unit> setColor<RT>(ConsoleColor color)
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.SetColor(color));

        public static Eff<RT, ConsoleColor> bgColor<RT>()
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.BgColor);
    
        public static Eff<RT, ConsoleColor> color<RT>()
            where RT : struct, HasConsole<RT> =>
            default(RT).ConsoleEff.Map(e => e.Color);
    }
}
