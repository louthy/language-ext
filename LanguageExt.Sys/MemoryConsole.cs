using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Sys
{
    /// <summary>
    /// An in-memory console emulation
    /// </summary>
    /// <remarks>
    /// Mostly for use in test runtimes.  This simulates the real System.IO.Console, with additional functionality
    /// for pre-building a keyboard buffer (WriteKey, WriteKeyChar, WriteKeyString) and committing them as though
    /// they were typed.  ReadKey and the other Read* methods will read the entries from the pre-built keyboard
    /// buffer.
    ///
    /// There is no public API for this, other than the prepping of the keyboard buffer, which is outside of the
    /// normal console system.  
    /// </remarks>
    public class MemoryConsole : IEnumerable<string>
    {
        readonly ConcurrentQueue<ConsoleKeyInfo> KeyboardBuffer = new();
        readonly ConcurrentStack<string> Console = new();

        public MemoryConsole() =>
            Console.Push("");

        /// <summary>
        /// Write a key into the keyboard buffer
        /// </summary>
        /// <remarks>
        /// Won't show in the console until Commit or Read* is called 
        /// </remarks>
        public Unit WriteKey(ConsoleKeyInfo key)
        {
            KeyboardBuffer.Enqueue(key);
            return default;
        }

        /// <summary>
        /// Write a character into the keyboard buffer
        /// </summary>
        /// <remarks>
        /// Won't show in the console until Commit or Read* is called 
        /// </remarks>
        public Unit WriteKeyChar(char ch)
        {
            KeyboardBuffer.Enqueue(new ConsoleKeyInfo(ch, CharToConsoleKey(ch), false, false, false));
            return default;
        }
        
        /// <summary>
        /// Write a character into the keyboard buffer
        /// </summary>
        /// <remarks>
        /// Won't show in the console until Commit or Read* is called 
        /// </remarks>
        public Unit WriteKeyString(string str)
        {
            foreach (var ch in str)
            {
                WriteKeyChar(ch);
            }
            return default;
        }

        /// <summary>
        /// Commit what's in the keyboard buffer (as though they've been typed in)
        /// </summary>
        /// <returns></returns>
        public Unit Commit()
        {
            while(KeyboardBuffer.Count > 0)
            {
                Read();
            }
            return default;
        }
        
        internal ConsoleKeyInfo ReadKey()
        {
            while (true)
            {
                if (KeyboardBuffer.TryDequeue(out var key))
                {
                    return key;
                }
                else
                {
                    // Ugly, but should only be needed for testing
                    Thread.Sleep(10);
                    continue;
                }
            }
        }

        internal ConsoleKey CharToConsoleKey(char ch) =>
            Enum.TryParse<ConsoleKey>(ch.ToString(), out var ck) ? ck : default;
        
        internal Unit Clear()
        {
            Console.Clear();
            return default;
        }

        internal Unit SetBgColor(ConsoleColor color)
        {
            BgColor = color;
            return default;
        }

        internal Unit SetColor(ConsoleColor color)
        {
            Color = color;
            return default;
        }

        internal ConsoleColor BgColor { get; private set; } = ConsoleColor.Black;
        internal ConsoleColor Color { get; private set; } = ConsoleColor.White;

        internal int Read()
        {
            var kc = ReadKey().KeyChar;
            var ch = Convert.ToChar(kc);
            if (ch == '\n')
            {
                Console.Push("");
            }
            else
            {
                lock (Console)
                {
                    if (Console.TryPop(out var line))
                    {
                        Console.Push(line + Convert.ToChar(kc));
                    }
                }
            }

            return kc;
        }

        internal string ReadLine()
        {
            List<char> chs = new();
            while (true)
            {
                var ch = ReadKey().KeyChar;
                if (ch == '\n') return new string(chs.ToArray());
                chs.Add(ch);
            }
        }
        
        internal Unit WriteLine()
        {
            Console.Push("");
            return unit;
        }

        internal Unit WriteLine(string value)
        {
            Console.Push(value);
            return unit;
        }

        internal Unit Write(string value)
        {
            lock (Console)
            {
                if (Console.TryPop(out var line))
                {
                    Console.Push(line + value);
                }
            }
            return unit;
        }

        public IEnumerator<string> GetEnumerator() =>
            Console.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Console.GetEnumerator();
    }
}
