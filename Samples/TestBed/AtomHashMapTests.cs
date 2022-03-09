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
            var xs = AtomHashMap<string, int>();
            xs.Add("Hello", 123);

            xs.OnChange().Subscribe(patch =>
            {
                Console.WriteLine(patch.Changes);
            });

            xs.OnEntryChange().Subscribe(pair =>
            {
                Console.WriteLine(pair);
            });
            
            xs.Add("World", 456);
            xs.SetItem("World", 123);
            xs.Remove("World");
            xs.Remove("World");

            var rx = Ref("Hello");
            var ry = Ref("World");

            Observable.Merge(rx.OnChange(), ry.OnChange()).Subscribe(v =>
            {
                Console.WriteLine(v);
            });

            atomic(() =>
            {
                swap(rx, _ => "Helloy");
                swap(ry, _ => "Worldy");
            });
        }
    }
}
