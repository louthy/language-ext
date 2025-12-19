using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed;

public class BracketTest
{
    class Test : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Disposing Test");
        }
    }

    public static void Run()
    {
        var workflow = bracketIO(
            from d in use(() => new Test())
            select d
        );
        ignore(workflow.RunSafe());
    }
}
