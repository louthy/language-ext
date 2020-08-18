/*
using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Pipes;
using static LanguageExt.IO;
using static LanguageExt.SIO;
using static LanguageExt.Pipe;
using static LanguageExt.Prelude;
using static LanguageExt.IO.File;
using static LanguageExt.IO.Time;
using static LanguageExt.IO.Console;
using static System.ConsoleColor;

namespace LanguageExt
{
    public static partial class Prelude
    {
        public static Producer<Runtime, string, Unit> readLine =>
            from a in Producer.liftIO(IO.Console.readLine)
            from _ in Producer.yield(a)
            from n in readLine
            select unit;

        public static Consumer<Runtime, string, Unit> writeLine =>
            from l in Consumer.awaiting<string>()
            from a in Consumer.liftIO<string>(IO.Console.writeLine(l))
            from n in writeLine 
            select unit;
    }
}
*/
