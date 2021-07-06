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
    public static class Console<RT>
        where RT : struct, HasConsole<RT>
    {
        /// <summary>
        /// Read a key from the console
        /// </summary>
        public static Eff<RT, ConsoleKeyInfo> readKey =>
            default(RT).ConsoleEff.Bind(e => e.ReadKey()
                                              .Match(Some: SuccessEff<ConsoleKeyInfo>, 
                                                     None: () => FailEff<ConsoleKeyInfo>(Error.New("end of stream"))));

        /// <summary>
        /// Clear the console
        /// </summary>
        public static Eff<RT, Unit> clear =>
            default(RT).ConsoleEff.Map(e => e.Clear());

        /// <summary>
        /// Read from the console
        /// </summary>
        public static Eff<RT, int> read =>
            default(RT).ConsoleEff.Bind(e => e.Read()
                                             .Match(Some: SuccessEff<int>, 
                                                    None: () => FailEff<int>(Error.New("end of stream"))));

        /// <summary>
        /// Read from the console
        /// </summary>
        public static Eff<RT, string> readLine =>
            default(RT).ConsoleEff.Bind(e => e.ReadLine()
                                              .Match(Some: SuccessEff<string>, 
                                                     None: () => FailEff<string>(Error.New("end of stream"))));

        /// <summary>
        /// Write an empty line to the console
        /// </summary>
        public static Eff<RT, Unit> writeEmptyLine =>
            default(RT).ConsoleEff.Map(e => e.WriteLine());

        /// <summary>
        /// Write a line to the console
        /// </summary>
        public static Eff<RT, Unit> writeLine(string line) =>
            default(RT).ConsoleEff.Map(e => e.WriteLine(line));

        /// <summary>
        /// Write a line to the console
        /// </summary>
        public static Eff<RT, string> writeLine2(string line) =>
            default(RT).ConsoleEff.Map(e => e.WriteLine(line)).Map(_ => line);

        public static Eff<RT, Unit> setBgColor(ConsoleColor color) =>
            default(RT).ConsoleEff.Map(e => e.SetBgColor(color));

        public static Eff<RT, Unit> setColor(ConsoleColor color) =>
            default(RT).ConsoleEff.Map(e => e.SetColor(color));

        public static Eff<RT, ConsoleColor> bgColor =>
            default(RT).ConsoleEff.Map(e => e.BgColor);

        public static Eff<RT, ConsoleColor> color =>
            default(RT).ConsoleEff.Map(e => e.Color);
    }
}
