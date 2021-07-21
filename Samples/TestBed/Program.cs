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
using static LanguageExt.Pipes.Proxy;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using LanguageExt.Sys.IO;
using TestBed;

public class Program
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

        await PipesTest();

        // await ObsAffTests.Test();
        // await AsyncTests();
    }

    public static async Task PipesTest()
    {
        var items1 = from x in enumerate<int, string>(Seq("Paul", "James", "Gavin"))
                     from _ in liftIO(Console<Runtime>.writeLine($"Enter your name ------------- {x}"))
                     from n in yield(x.Length)
                     select unit;

        var effect = items1 | times10 | writeLine;

        var effect1 = enumerate(Seq("Paul", "James", "Gavin")) | sayHello | writeLine;

        var time = Observable.Interval(TimeSpan.FromSeconds(1));

        var effect2 = observe2(time) | now | toLongTimeString | writeLine;

        var result = await effect2.RunEffect<Runtime, Unit>()
                                  .Run(Runtime.New());
    }

    static Pipe<Runtime, long, DateTime, Unit> now =>
        from t in awaiting<long>()         
        from n in liftIO(Time<Runtime>.now)
        from _ in yield(n)
        select unit;

    static Pipe<Runtime, DateTime, string, Unit> toLongTimeString =>
        from n in awaiting<DateTime>()         
        from _ in yield(n.ToLongTimeString())
        select unit;

    static Consumer<Runtime, string, Unit> writeLine =>
        from l in awaiting<string>()
        from a in liftIO(Console<Runtime>.writeLine(l))
        select unit;
    
    static Producer<Runtime, string, Unit> readLine =>
        repeat(from _1 in liftIO(Console<Runtime>.writeLine("Enter your name"))
               from nw in liftIO(Time<Runtime>.now)
               from _2 in yield(nw.ToLongTimeString())
               select unit);

    static Pipe<Runtime, string, string, Unit> sayHello =>
        from l in awaiting<string>()         
        from _ in yield($"Hello {l}")
        select unit;


    static Pipe<Runtime, int, string, Unit> times10 =>
        from n in awaiting<int>()         
        from _ in yield($"{n * 10}")
        select unit;
    
    
    static Producer<Runtime, string, Unit> readLine2 =>
        from w in Producer.liftIO<Runtime, string, Unit>(Console<Runtime>.writeLine("Enter your name"))
        from l in Producer.liftIO<Runtime, string, string>(Console<Runtime>.readLine)
        from _ in Producer.yield<Runtime, string>(l)
        from n in readLine2
        select unit;

    static Pipe<Runtime, string, string, Unit> sayHello2 =>
        from l in Pipe.await<Runtime, string, string>()         
        from _ in Pipe.yield<Runtime, string, string>($"Hello {l}")
        from n in sayHello2
        select unit;
    
    static Consumer<Runtime, string, Unit> writeLine2 =>
        from l in Consumer.await<Runtime, string>()
        from a in Consumer.liftIO<Runtime, string>(Console<Runtime>.writeLine(l))
        from n in writeLine2
        select unit;
    
    
    static Pipe<Runtime, string, string, Unit> pipeMap =>
        Pipe.map((string x) => $"Hello {x}");
    
}
