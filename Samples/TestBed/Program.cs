////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using LanguageExt;
using System.Threading.Tasks;
using LanguageExt.Traits;
using TestBed;
using static LanguageExt.Prelude;


public class Program
{
    static async Task<int> t1()
    {
        await Task.Delay(1000);
        Console.WriteLine("t1");
        return 1;
    }

    static async Task<int> t2()
    {
        await Task.Delay(10);
        Console.WriteLine("t2");
        return 2;
    }

    public static async Task Issue1426()
    {
        // Changing 1st IO.lift to IO.liftAsync(() => Task.FromResult(1)) will print out t2 first.
        var s1         = IO.lift(() => 1).Bind(_ => IO.lift(() => 1)).Bind(_ => IO.liftAsync(() => t1()));
        var s2         = IO.lift(() => 1).Bind(_ => IO.lift(() => 2)).Bind(_ => IO.liftAsync(() => t2()));
        var add        = static (int x, int y) => x + y;
        var runTest    = add.Map(s1).Apply(s2);
        var resultTest = await runTest.RunAsync();
    }    
    
    static void Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ///////////////////////////////////////////v////////////////////////////////////////////////////////////

        //var my = IO.pure(true);
        //var mz = mx >> my;
        
        var fx = IO.pure(100);
        var fy = IO.pure(200);
        var fz = IO.pure(300);
        var fr = ((int x, int y, int z) => x + y + z) * fx * fy * fz;

        var af = IO.pure((int x, int y, int z) => x + y + z);
        var ax = IO.pure(100);
        var ay = IO.pure(200);
        var az = IO.pure(300);
        var ar = af * ax * ay * az;
        
        var mx = IO.pure(100);
        var mr = mx >> (x => IO.pure(x + 1));
        
        var fr1 = fr.Run();
        var ar1 = ar.Run();
        var mr1 = mr.Run();

        /*Issue1497.AppPrelude.Test();

        var mx = Validation.Fail<StringM, int>("fail 1");
        var my = Validation.Fail<StringM, int>("fail 2");
        var mz = mx & my;

        Seq<K<Either, string, int>> m1 =
        [
            (Either<string, int>)Right(100),
            (Either<string, int>)Left("Hello"),
            (Either<string, int>)Left("World"),
            (Either<string, int>)Right(200)
        ];
        IEnumerable<Either<string, int>> m2 =
        [
            (Either<string, int>)Right(100),
            (Either<string, int>)Left("Hello"),
            (Either<string, int>)Left("World"),
            (Either<string, int>)Right(200)
        ];
        Iterable<Either<string, int>>     m3 = [Right(100), Left("Hello"), Left("World"), Right(200)];
        Lst<Either<string, int>>          m4 = [Right(100), Left("Hello"), Left("World"), Right(200)];
        Seq<ChronicleT<StringM, IO, int>> m5 = [];

        var r1 = m1.Partition();
        var r2 = m2.PartitionSequence();
        var r3 = m3.PartitionSequence();
        var r4 = m4.PartitionSequence();
        var r5 = m5.PartitionSequence();*/

        //TestBed.StateStuff.StateForkIO.forkTest.Run(4).Run().Ignore();
        //Issue1453.Test();
        //UseTest.Main().GetAwaiter().GetResult();
        //Issue1426().GetAwaiter().GetResult();
        //SeqConstructTests.Test();
        //ResourcesDiscussion1366.Run();
        //StateTTest();
        //AtomHashMapTests.Test();
        //await AtomHashMapPerf.Test();
        // await PipesTest();
        // await ObsAffTests.Test();
        // await AsyncTests();
        //testing.Run(Runtime.New());
        //var _ = QueueExample<Runtime>.Issue1065().RunUnit(new Runtime()).Result;
        //ScheduleTests.Run();
        //ApplicativeTest.Test().GetAwaiter().GetResult();
        //Console.WriteLine(PipesTestBed.effect.RunEffect().Run(Runtime.New()).Result);
        //Issue1230.Run();
        //Issue1234.Test();
        //SequenceParallelTest.Run();
        //FreeTests.Test();
    }

    public record Item;
    public record ItemId;
    public record ItemRepo
    {
        public Task<Item> GetById(ItemId id) => 
            throw new NotImplementedException();
    }

    public static ReaderT<ItemRepo, Eff, Item> GetItem(ItemId itemId) =>
        from repo in ReaderT.ask<Eff, ItemRepo>()
        from item in liftIO(async () => await repo.GetById(itemId))
        select item;

    public static ReaderT<ItemRepo, Eff, Unit> ProcessItem(Item item) =>
        throw new NotImplementedException();

    public static ReaderT<ItemRepo, Eff, Unit> ReaderTEffExample(ItemId id) =>
        from item in GetItem(id)
        from _    in ProcessItem(item)
        select unit;

    public static void StateTTest()
    {
        var mx = StateT<string, OptionT<IO>, int>.Pure(100);
        var my = from s in StateT.get<OptionT<IO>, string>()
                 from x in IO.lift(() => Console.WriteLine(s))
                 from _ in StateT.put<OptionT<IO>, string>($"Hello: {s}")
                 from t in StateT.get<OptionT<IO>, string>()
                 from y in IO.lift(() => Console.WriteLine(t))
                 select unit;

        var mr = my.Run("Paul").Run().Run();
    }

    /*
    public static void PipesTest()
    {
        // Create two queues.  Queues are Producers that have an Enqueue function
        var queue1 = Queue<Eff<Runtime>, string>();
        var queue2 = Queue<Eff<Runtime>, string>();

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
        Consumer<string, Eff<Runtime>, Unit> writeToQueue() =>
            from x in awaiting<string>()
            from _ in x switch
                      {
                          "1" => queue1.EnqueueM(x.Substring(1)),
                          "2" => queue2.EnqueueM(x.Substring(1)),
                          _   => FailEff<Runtime, Unit>(Errors.Cancelled)
                      }
            select unit;

        //var clientServer = incrementer | oneTwoThree;

        var file1 = File<Runtime>.openRead("i:\\defaults.xml")
                  | Stream<Eff<Runtime>>.read(240)
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

        var foldTest2 = Producer.lift<string, Eff<Runtime>, char>(Console<Runtime>.readKey.Map(k => k.KeyChar))
                                .FoldUntil("", (word, ch) => word + ch, char.IsWhiteSpace)
                      | repeat(writeLine);

        var effect1  = yieldAll(Seq("Paul", "James", "Gavin")) | sayHello | writeLine;
        var effect1a = yieldAll(Some("Paul")) | sayHello | writeLine;

        var time    = Observable.Interval(TimeSpan.FromSeconds(1));
        var effect2 = yieldAll(time) | now | toLongTimeString | writeLine;

        var echo = readLine | writeLine;

        var timeOneStep  = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => "whole");
        var timeHalfStep = Observable.Interval(TimeSpan.FromSeconds(.5)).Select(_ => "half");
        var channel1     = Producer.yieldAll<Eff<Runtime>, string>(timeOneStep);
        var channel2     = Producer.yieldAll<Eff<Runtime>, string>(timeHalfStep);
        var channel      = Producer.merge(channel1, channel2) | writeLine;

        var result = queueing.As().RunEffect()
                             .As().Run(Runtime.New(), EnvIO.New())
                             .Match(Succ: x => Console.WriteLine($"Success: {x}"),
                                    Fail: Console.WriteLine);
    }

    static Effect<Eff<Runtime>, Unit> fizzBuzz =>
        yieldAll(Range(1, 20)) | process | writeLine;

    static Pipe<int, string, Eff<Runtime>, Unit> process =>
        from n in Pipe.awaiting<Eff<Runtime>, int, string>()
        from t in collect(n | fizz, n | buzz, n | number)
        from _ in Pipe.yield<int, string, Eff<Runtime>>($"{t.Item1}{t.Item2}{t.Item3}")
        select unit;

    static Consumer<int, Eff<Runtime>, string> fizz =>
        from n in awaiting<int>()
        select n % 3 == 0
                   ? "Fizz"
                   : "";

    static Consumer<int, Eff<Runtime>, string> buzz =>
        from n in awaiting<int>()
        select n % 5 == 0
                   ? "Buzz"
                   : "";

    static Consumer<int, Eff<Runtime>, string> number =>
        from n in awaiting<int>()
        select n % 3 != 0 && n % 5 != 0
                   ? $"{n}"
                   : "";

    static Pipe<ConsoleKeyInfo, char, Eff<Runtime>, Unit> keyChar =>
        map<ConsoleKeyInfo, char>(k => k.KeyChar);

    static Pipe<char, string, Eff<Runtime>, Unit> words =>
        foldUntil("", (word, ch) => word + ch, (char x) => char.IsWhiteSpace(x));

    static Pipe<string, string, Eff<Runtime>, Unit> filterEmpty =>
        filter<string>(notEmpty);

    static Pipe<DateTime, string, Eff<Runtime>, Unit> toLongTimeString =>
        from n in awaiting<DateTime>()
        from _ in yield(n.ToLongTimeString())
        select unit;

    static Pipe<long, DateTime, Eff<Runtime>, Unit> now =>
        from t in Pipe.awaiting<Eff<Runtime>, long, DateTime>()
        from n in Time<Eff<Runtime>, Runtime>.now
        from _ in Pipe.yield<long, DateTime, Eff<Runtime>>(n)
        select unit;

    static Producer<string, Eff<Runtime>, Unit> readLine =>
        from nm in Console<Eff<Runtime>, Runtime>.readLine
        from _2 in yield(nm)
        select unit;

    static Consumer<string, Eff<Runtime>, Unit> writeLine =>
        from l in awaiting<string>()
        from _ in Console<Eff<Runtime>, Runtime>.writeLine(l)
        select unit;

    static Consumer<string, Eff<Runtime>, Unit> write =>
        from l in awaiting<string>()
        from a in Console<Eff<Runtime>, Runtime>.write(l)
        select unit;

    static Pipe<string, string, Eff<Runtime>, Unit> sayHello =>
        from l in awaiting<string>()
        from _ in yield($"Hello {l}")
        select unit;

    static Pipe<SeqLoan<byte>, string, Eff<Runtime>, Unit> decodeUtf8 =>
        from c in awaiting<SeqLoan<byte>>()
        from _ in yield(Encoding.UTF8.GetString(c.ToReadOnlySpan()))
        select unit;

    static Pipe<A, string, Eff<Runtime>, Unit> toString<A>() =>
        from l in awaiting<A>()
        from _ in yield($"{l} ")
        select unit;

    static Pipe<int, string, Eff<Runtime>, Unit> times10 =>
        from n in awaiting<int>()
        from _ in yield($"{n * 10}")
        select unit;

    static Pipe<string, string, Eff<Runtime>, Unit> prepend(string x) =>
        from l in awaiting<string>()
        from _ in yield($"{x}{l}")
        select unit;

    static Pipe<string, string, Eff<Runtime>, Unit> append(string x) =>
        from l in awaiting<string>()
        from _ in yield($"{l}{x}")
        select unit;


    // Old way, using lots of generics

    static Producer<string, Eff<Runtime>, Unit> readLine2 =>
        from w in Console<Eff<Runtime>, Runtime>.writeLine("Enter your name")
        from l in Console<Eff<Runtime>, Runtime>.readLine
        from _ in Producer.yield<string, Eff<Runtime>>(l)
        from n in readLine2
        select unit;

    static Pipe<string, string, Eff<Runtime>, Unit> sayHello2 =>
        from l in Pipe.awaiting<Eff<Runtime>, string, string>()
        from _ in Pipe.yield<string, string, Eff<Runtime>>($"Hello {l}")
        from n in sayHello2
        select unit;

    static Consumer<string, Eff<Runtime>, Unit> writeLine2 =>
        from l in Consumer.awaiting<Eff<Runtime>, string>()
        from a in Console<Eff<Runtime>, Runtime>.writeLine(l)
        from n in writeLine2
        select unit;
        */


    /*static Server<Runtime, int, int, Unit> incrementer(int question) =>
        from _1 in Server.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Server received: {question}"))
        from _2 in Server.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Server responded: {question + 1}"))
        from nq in Server.respond<Runtime, int, int>(question + 1)
        select unit;

    static Client<Runtime, int, int, Unit> oneTwoThree =>
        from qn in Client.yieldAll<Runtime, int, int, int>(Seq(1, 2, 3))
        from _1 in Client.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Client requested: {qn}"))
        from an in Client.request<Runtime, int, int>(qn)
        from _2 in Client.lift<Runtime, int, int, Unit>(Console<Runtime>.writeLine($"Client received: {an}"))
        select unit;*/


    /*
    static Pipe<string, string, Eff<Runtime>, Unit> pipeMap =>
        Pipe.map((string x) => $"Hello {x}");*/
}

public static class Issue1453erwr
{
    public static Eff<bool> isEmpty<E>(IteratorAsync<E> iter) =>
        liftEff(async _ => await iter.IsEmpty);

    public static Eff<E> head<E>(IteratorAsync<E> iter) =>
        liftEff(async _ => await iter.Head);

    public static Eff<IteratorAsync<E>> tail<E>(IteratorAsync<E> iter) =>
        liftEff(async _ => await iter.Split().Tail);
}
