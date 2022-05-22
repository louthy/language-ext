using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class AtomHashMapTests
    {
        public static void Test()
        {
            var thm = TrackingHashMap<int, string>();

            Console.WriteLine(thm);

            thm = thm.Add(100, "World");
            thm = thm.Snapshot();
            thm = thm.SetItem(100, "Hello");

            Console.WriteLine(thm.Changes);
 
            var xs = AtomHashMap<string, int>();

            xs.OnEntryChange().Subscribe(pair => Console.WriteLine(pair));
            
            xs.Add("Hello", 456);
            xs.SetItem("Hello", 123);
            xs.Remove("Hello");
            xs.Remove("Hello");

            var rx = Ref("Hello");
            var ry = Ref("World");

            Observable.Merge(rx.OnChange(), 
                             ry.OnChange())
                      .Subscribe(Console.WriteLine);

            atomic(() =>
            {
                swap(rx, x => $"1. {x}");
                swap(ry, y => $"2. {y}");
            });
        }
    }
}
