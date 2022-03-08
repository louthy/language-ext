using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class AtomHashMapTests
    {
        public static void Test()
        {
            var xs = AtomHashMap<string, int>();
            xs.Add("Hello", 123);

            xs.Change += (prev, curr, change) =>
            {
                Console.WriteLine(change);
            };
            
            xs.Add("World", 456);
            xs.SetItem("World", 123);
            xs.Remove("World");
            xs.Remove("World");
        }
    }
}
