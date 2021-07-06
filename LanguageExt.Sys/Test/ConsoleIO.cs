using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    /// <summary>
    /// Encapsulated in-memory console
    /// No public API exists for this.  Use Sys.IO.Console.* to interact with the console
    /// </summary>
    /// <remarks>
    /// Primarily used for testing (for use with TestRuntime or your own testing runtime)
    /// </remarks>
    public readonly struct ConsoleIO : Sys.Traits.ConsoleIO
    {
        readonly MemoryConsole mem;

        public ConsoleIO(MemoryConsole mem) =>
            this.mem = mem;
        
        public Option<ConsoleKeyInfo> ReadKey() =>
            mem.ReadKey();

        public Unit Clear() =>
            mem.Clear();

        public Unit SetBgColor(ConsoleColor color) =>
            mem.SetBgColor(color);

        public Unit SetColor(ConsoleColor color) =>
            mem.SetColor(color);

        public ConsoleColor BgColor => 
            mem.BgColor;
        
        public ConsoleColor Color => 
            mem.Color;
        
        public Option<int> Read() =>
            mem.Read();
        
        public Option<string> ReadLine() =>
            mem.ReadLine();

        public Unit WriteLine() =>
            mem.WriteLine();

        public Unit WriteLine(string value) =>
            mem.WriteLine(value);

        public Unit Write(string value) =>
            mem.Write(value);
    }
}
