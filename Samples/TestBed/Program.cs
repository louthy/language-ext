////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LanguageExt;
using System.Text;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Sys.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using LanguageExt.Sys.Live;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using TestBed;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

public interface IAsyncQueue<A>
{
    Task<A> DequeueAsync();
}

public static class Ext
{
    static Producer<RT, A, Unit> ToProducer<RT, A>(this IAsyncQueue<A> q) 
        where RT : struct, HasCancel<RT>
    {
        return Proxy.enumerate(go());

        async IAsyncEnumerable<A> go()
        {
            while (true)
            {
                yield return await q.DequeueAsync();
            }
        }
    }

    public static Producer<RT, A, Unit> ToProducer<RT, A>(this IAsyncQueue<A>[] qs)
        where RT : struct, HasCancel<RT> =>
        Producer.merge(qs.Map(q => q.ToProducer<RT, A>()).ToSeq());
}

public class Program
{
    static void Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ///////////////////////////////////////////v////////////////////////////////////////////////////////////

        //AtomHashMapTests.Test();
        //await AtomHashMapPerf.Test();
        // await PipesTest();
        // await ObsAffTests.Test();
        // await AsyncTests();
        //testing.Run(Runtime.New());
        //var _ = QueueExample<Runtime>.Issue1065().RunUnit(new Runtime()).Result;
        //ScheduleTests.Run();
        ApplicativeTest.Test().GetAwaiter().GetResult();
    }

    public static async Task PipesTest()
    {
        // Create two queues.  Queues are Producers that have an Enqueue function
        var queue1 = Proxy.Queue<Runtime, string>();
        var queue2 = Proxy.Queue<Runtime, string>();

        // Compose the queues with a pipe that prepends some text to what they produce
        var queues = Seq(queue1 | prepend("Queue 1: "), queue2 | prepend("Queue 2: "));

        // Run the queues in a forked task
        // Repeatedly read from the console and write to one of the two queues depending on
        // whether the first char is 1 or 2
        var queueing = from _ in fork(Producer.merge(queues) | writeLine)
                       from x in repeat(Console<Runtime>.readLines) | writeToQueue()
                       select unit;

        // Consumer of the console.  It enqueues the item to queue1 or queue2 depending
        // on the first char of the string it awaits
        Consumer<Runtime, string, Unit> writeToQueue() =>
            from x in awaiting<string>()
            from _ in x.HeadOrNone().Case switch
                      {
                          '1' => queue1.EnqueueEff(x.Substring(1)),
                          '2' => queue2.EnqueueEff(x.Substring(1)),
                          _   => FailEff<Unit>(Errors.CancelledText)
                      }
            select unit;

        var clientServer = incrementer | oneTwoThree;

        var file1 = File<Runtime>.openRead("i:\\defaults.xml")
                  | Stream<Runtime>.read(240)
                  | decodeUtf8
                  | writeLine;

        var file2 = File<Runtime>.openText("i:\\defaults.xml")
                  | TextRead<Runtime>.readLine
                  | writeLine;

        var file3 = File<Runtime>.openText("i:\\defaults.xml")
                  | TextRead<Runtime>.readChar
                  | words
                  | filterEmpty
                  | writeLine;

        var foldTest1 = repeat(Console<Runtime>.readKeys)
                      | keyChar
                      | words
                      | filterEmpty
                      | writeLine;

        var foldTest2 = Producer.lift<Runtime, string, char>(Console<Runtime>.readKey.Map(k => k.KeyChar))
                                .FoldUntil("", (word, ch) => word + ch, char.IsWhiteSpace)
                      | repeat(writeLine);

        var effect1  = enumerate(Seq("Paul", "James", "Gavin")) | sayHello | writeLine;
        var effect1a = enumerate(Some("Paul")) | sayHello | writeLine;

        var time    = Observable.Interval(TimeSpan.FromSeconds(1));
        var effect2 = observe(time) | now | toLongTimeString | writeLine;

        var echo = readLine | writeLine;

        var timeOneStep  = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => "whole");
        var timeHalfStep = Observable.Interval(TimeSpan.FromSeconds(.5)).Select(_ => "half");
        var channel1     = Producer.observe<Runtime, string>(timeOneStep);
        var channel2     = Producer.observe<Runtime, string>(timeHalfStep);
        var channel      = (channel1 + channel2) | writeLine;

        var result = (await queueing //.RunEffect()
                         .Run(Runtime.New()))
           .Match(Succ: x => Console.WriteLine($"Success: {x}"),
                  Fail: e => Console.WriteLine(e));
    }

    static Effect<Runtime, Unit> fizzBuzz =>
        enumerate(Range(1, 20)) | process | writeLine;

    static Pipe<Runtime, int, string, Unit> process =>
        from n in awaiting<int>()
        from t in collect(n | fizz, n | buzz, n | number)
        from _ in yield($"{t.Item1}{t.Item2}{t.Item3}")
        select unit;

    static Consumer<Runtime, int, string> fizz =>
        from n in awaiting<int>()
        select n % 3 == 0
                   ? "Fizz"
                   : "";

    static Consumer<Runtime, int, string> buzz =>
        from n in awaiting<int>()
        select n % 5 == 0
                   ? "Buzz"
                   : "";

    static Consumer<Runtime, int, string> number =>
        from n in awaiting<int>()
        select n % 3 != 0 && n % 5 != 0
                   ? $"{n}"
                   : "";

    static Pipe<Runtime, ConsoleKeyInfo, char, Unit> keyChar =>
        map<ConsoleKeyInfo, char>(k => k.KeyChar);

    static Pipe<Runtime, char, string, Unit> words =>
        foldUntil("", (word, ch) => word + ch, (char x) => char.IsWhiteSpace(x));

    static Pipe<Runtime, string, string, Unit> filterEmpty =>
        filter<string>(notEmpty);

    static Pipe<Runtime, DateTime, string, Unit> toLongTimeString =>
        from n in awaiting<DateTime>()
        from _ in yield(n.ToLongTimeString())
        select unit;

    static Pipe<Runtime, long, DateTime, Unit> now =>
        from t in awaiting<long>()
        from n in Time<Runtime>.now
        from _ in yield(n)
        select unit;

    static Producer<Runtime, string, Unit> readLine =>
        from nm in Console<Runtime>.readLine
        from _2 in yield(nm)
        select unit;

    static Consumer<Runtime, string, Unit> writeLine =>
        from l in awaiting<string>()
        from _ in Console<Runtime>.writeLine(l)
        select unit;

    static Consumer<Runtime, string, Unit> write =>
        from l in awaiting<string>()
        from a in Console<Runtime>.write(l)
        select unit;

    static Pipe<Runtime, string, string, Unit> sayHello =>
        from l in awaiting<string>()
        from _ in yield($"Hello {l}")
        select unit;

    static Pipe<Runtime, SeqLoan<byte>, string, Unit> decodeUtf8 =>
        from c in awaiting<SeqLoan<byte>>()
        from _ in yield(Encoding.UTF8.GetString(c.ToReadOnlySpan()))
        select unit;

    static Pipe<Runtime, A, string, Unit> toString<A>() =>
        from l in awaiting<A>()
        from _ in yield($"{l} ")
        select unit;

    static Pipe<Runtime, int, string, Unit> times10 =>
        from n in awaiting<int>()
        from _ in yield($"{n * 10}")
        select unit;

    static Pipe<Runtime, string, string, Unit> prepend(string x) =>
        from l in awaiting<string>()
        from _ in yield($"{x}{l}")
        select unit;

    static Pipe<Runtime, string, string, Unit> append(string x) =>
        from l in awaiting<string>()
        from _ in yield($"{l}{x}")
        select unit;


    // Old way, using lots of generics

    static Producer<Runtime, string, Unit> readLine2 =>
        from w in Producer.lift<Runtime, string, Unit>(Console<Runtime>.writeLine("Enter your name"))
        from l in Producer.lift<Runtime, string, string>(Console<Runtime>.readLine)
        from _ in Producer.yield<Runtime, string>(l)
        from n in readLine2
        select unit;

    static Pipe<Runtime, string, string, Unit> sayHello2 =>
        from l in Pipe.awaiting<Runtime, string, string>()
        from _ in Pipe.yield<Runtime, string, string>($"Hello {l}")
        from n in sayHello2
        select unit;

    static Consumer<Runtime, string, Unit> writeLine2 =>
        from l in Consumer.awaiting<Runtime, string>()
        from a in Consumer.lift<Runtime, string>(Console<Runtime>.writeLine(l))
        from n in writeLine2
        select unit;


    static Server<Runtime, int, int, Unit> incrementer(int question) =>
        from _1 in Server.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Server received: {question}"))
        from _2 in Server.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Server responded: {question + 1}"))
        from nq in Server.respond<Runtime, int, int>(question + 1)
        select unit;

    static Client<Runtime, int, int, Unit> oneTwoThree =>
        from qn in Client.enumerate<Runtime, int, int, int>(Seq(1, 2, 3))
        from _1 in Client.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Client requested: {qn}"))
        from an in Client.request<Runtime, int, int>(qn)
        from _2 in Client.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Client received: {an}"))
        select unit;


    static Pipe<Runtime, string, string, Unit> pipeMap =>
        Pipe.map((string x) => $"Hello {x}");
}
