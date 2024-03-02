using System;
using LanguageExt.Sys.Live;
using LanguageExt;
using LanguageExt.Common;

namespace EffectsExamples;

class Program
{
    static void Main(string[] args)
    {
        Test();
        return;
        
        Menu<Runtime>
           .menu
           .Run(Runtime.New(), EnvIO.New())
           .ThrowIfFail();
    }

    static void Test()
    {
        var mx = OptionT.lift(IO.lift<int>(() => throw new Exception("Fail")));
        
        //var my = mx.MapFail(e => Error.New("Replacement error"));
        //var my = mx.IfNone(100).As();
        var my = mx.MapT(io => io.As().MapFail(e => Error.New("Replacement error")));
        
        var r  = my.Run().As().Run();
        Console.WriteLine(r);
    }
}
