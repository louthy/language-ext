////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Reflection;
using LanguageExt;
using LanguageExt.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LanguageExt.Interfaces;
using static LanguageExt.IO.File;
using TestBed;

class Program
{
    static async Task Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ///////////////////////////////////////////v////////////////////////////////////////////////////////////

        await AsyncTests();
    }

    public interface ILogging
    {
        ValueTask<Unit> Log(string text);
    }

    public interface HasLogging<RT> : HasCancel<RT>
        where RT : struct, HasCancel<RT>
    {
        Aff<RT, ILogging> Logging { get; }
    }
    
    public struct LiveLogging : ILogging
    {
        public static ILogging Default = new LiveLogging();
        
        public ValueTask<Unit> Log(string text)
        {
            Console.WriteLine(text);
            return default;
        }
    }
    
    public struct MyRuntime : HasCancel<MyRuntime>, HasLogging<MyRuntime>
    {
        readonly CancellationTokenSource source;

        public MyRuntime(CancellationTokenSource source) =>
            this.source = source;

        public MyRuntime LocalCancel =>
            new MyRuntime(new CancellationTokenSource());

        public CancellationToken CancellationToken =>
            source.Token;

        public Eff<MyRuntime, CancellationTokenSource> CancellationTokenSource =>
            Eff<MyRuntime, CancellationTokenSource>(env => env.source);

        public Aff<MyRuntime, ILogging> Logging =>
            SuccessAff(LiveLogging.Default);
    }

    public static class LogIO
    {
        public static Aff<RT, Unit> log<RT>(string text) where RT : struct, HasLogging<RT> =>
            default(RT).Logging.MapAsync(e => e.Log(text));

    }

    static Aff<RT, Unit> Foo<RT>() where RT : struct, HasLogging<RT> =>
        from _1 in LogIO.log<RT>("Hello")
        from _2 in LogIO.log<RT>("World")
        select unit;

    static Aff<RT, Unit> Bar<RT>() where RT : struct, HasLogging<RT> =>
        Foo<RT>();

    static async Task<Unit> AsyncTests()
    {
        var res = await Bar<MyRuntime>().RunIO(new MyRuntime(new CancellationTokenSource()));
        
        // Setup
        MkIO.Setup(Runtime.New());
        var tmp1 = Path.GetTempFileName();
        var tmp2 = Path.GetTempFileName();
        var tmp3 = "//**--";
        File.WriteAllLines(tmp1, new[] {"Hello", "World"});
        File.WriteAllLines(tmp2, new[] {"Hello", "World", "Again"});

        // Run with environment
        var res1 = await AddLines<Runtime>(tmp1, tmp2).RunIO(Runtime.New());
        
        // Run with wrapped environment
        var res2 = await AddLines(tmp1, tmp2);

        // Run with environment
        var fail1 = await AddLines<Runtime>(tmp1, tmp3).RunIO(Runtime.New());
        
        // Run with wrapped environment
        var fail2 = await AddLines(tmp1, tmp3);

        await OptionAsyncTest();

        return unit;
    }

    static Aff<RT, int> AddLines<RT>(string path1, string path2) where RT : struct, HasFile<RT> =>
        from lines1 in readAllLines<RT>(path1)
        from lines2 in readAllLines<RT>(path2)
        select lines1.Count + lines2.Count;

    static async AffPure<int> AddLines(string path1, string path2)
    {
        var lines1 = await MkIO.readAllLines(path1);
        var lines2 = await MkIO.readAllLines(path2);
        return lines1.Count + lines2.Count;
    }

    static async AffPure<int> Add(AffPure<int> ma, AffPure<int> mb)
    {
        var a = await ma;
        Console.WriteLine("HEll");
        var b = await mb;
        return a + b;
    }

    static async Task OptionAsyncTest()
    {
        var r1 = await Add(SomeAsync(100), SomeAsync(200)).IfNone(-1);
        Console.WriteLine(r1);

        var r2 = await Add(SomeAsync(100), None).IfNone(-1);
        Console.WriteLine(r2);

        var r3 = await Add(SomeAsync<int>(async _ => (await System.IO.File.ReadAllTextAsync("")).Length), SomeAsync(100)).IfNone(-1);
        Console.WriteLine(r2);
    }

    static async OptionAsync<int> Add(OptionAsync<int> ma, OptionAsync<int> mb)
    {
        var a = await ma;
        var b = await mb;
        return a + b;
    }

    static class MkIO
    {
        public static Func<string, AffPure<Seq<string>>> readAllLines;

        public static void Setup<RT>(RT runtime) where RT : struct, HasFile<RT>
        {
            readAllLines = path => AffMaybe<Seq<string>>(async () => await IO.File.readAllLines<RT>(path).RunIO(runtime));
        }
    }
}
