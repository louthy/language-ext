using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Token;

// ************************************************************************************
// 
//  This is just a dumping ground I use for debugging the library, you can ignore this
// 
// ************************************************************************************

namespace TestBed
{
    class Tests
    {
        public class Version : IComparable<Version>, IEquatable<Version>
        {
            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Build { get; private set; }
            public string Name { get; private set; }

            public Version(Option<string> name, int major, int minor, int build)
            {
                Major = major;
                Minor = minor;
                Build = build;
                Name = name.IfNone("");
            }

            public Version(IEnumerable<int> numbers, Option<string> name)
            {
                match(
                    numbers,
                    () => failwith<Version>("No numbers specified in the version string"),
                    major => New(name, major),
                    (major, minor) => New(name, major, minor),
                    (major, minor, build) => New(name, major, minor, build),
                    (major, minor, build, _) => failwith<Version>("More than 3 numbers in the version string")
                );
            }

            public static Version New(Option<string> name, int major, int minor = 0, int build = 0) =>
                new Version(name, major, minor, build);

            public int CompareTo(Version other)
            {
                var res = Major.CompareTo(other.Major);
                if (res != 0) return res;
                res = Minor.CompareTo(other.Minor);
                if (res != 0) return res;
                res = Build.CompareTo(other.Build);
                if (res != 0) return res;
                return Name.CompareTo(other.Name);
            }

            public bool Equals(Version other) =>
                !ReferenceEquals(other, null) &&
                 Major == other.Major &&
                 Minor == other.Minor &&
                 Build == other.Build &&
                 Name == other.Name;

            public override bool Equals(object obj) =>
                obj is Version && Equals((Version)obj);

            public override int GetHashCode() =>
                (Major.GetHashCode() * 13) +
                (Minor.GetHashCode() * 13) +
                (Build.GetHashCode() * 13) +
                Name.GetHashCode();

            public static bool operator ==(Version lhs, Version rhs) =>
                lhs.Equals(rhs);

            public static bool operator !=(Version lhs, Version rhs) =>
                !lhs.Equals(rhs);

            public static bool operator >(Version lhs, Version rhs) =>
                lhs.CompareTo(rhs) > 0;

            public static bool operator >=(Version lhs, Version rhs) =>
                lhs.CompareTo(rhs) >= 0;

            public static bool operator <(Version lhs, Version rhs) =>
                lhs.CompareTo(rhs) < 0;

            public static bool operator <=(Version lhs, Version rhs) =>
                lhs.CompareTo(rhs) <= 0;

            public override string ToString() =>
                Name.Length == 0
                    ? $"{Major}.{Minor}.{Build}"
                    : $"{Major}.{Minor}.{Build}-{Name}";

            static readonly Parser<int> Integer =
                Token(
                    from x in many1(digit)
                    let v = parseInt(new string(x.ToArray()))
                    from n in v.Match(
                        Some: d => result(d),
                        None: () => failure<int>("Not a valid decimal value"))
                    select n);

            static readonly Parser<char> Dot =
                Token(ch('.'));

            static readonly Parser<char> Dash =
                Token(ch('-'));

            static Parser<T> Token<T>(Parser<T> p) =>
                from v in p
                from _ in skipMany(space)
                select v;

            static readonly Parser<string> Word =
                Token(asString(many1(alphaNum)));

            static readonly Parser<Version> Parser =
                from numbers in sepBy1(Integer, Dot)
                from _ in optional(Dash)
                from name in optional(Word)
                select new Version(numbers, name);

            public static Option<Version> Parse(string version) =>
                Parser(version.ToPString()).Reply.Result;
        }

        public static void VersionTest()
        {
            var versions = new[] { "xdsd.efrer", "3 .  67.  6 - bler", "9.0", "4.5-beta", "4.5-alpha", "1.0", "1.2.3", "1.20.300", "1.200.3sometext" };

            var results = from v in versions
                          from pv in Version.Parse(v).AsEnumerable()
                          orderby pv
                          select pv;
        }

        public static void StopStart()
        {
            StopStartImpl();
            StopStartImpl();
            StopStartImpl();
            StopStartImpl();
            StopStartImpl();
            StopStartImpl();
            StopStartImpl();
        }

        public static readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

        class Hours : NewType<int> { public Hours(int value) : base(value) { } }

        internal static void SerialiseDeserialiseCoreTypes()
        {
            var h1 = JsonConvert.SerializeObject(new Hours(2), JsonSerializerSettings);

            var method = typeof(JsonConvert).GetMethods()
                                            .Filter(m => m.IsGenericMethod)
                                            .Filter(m => m.Name == "DeserializeObject")
                                            .Filter(m => m.GetParameters().Length == 1)
                                            .Head();

            var h2 = method.MakeGenericMethod(typeof(Hours)).Invoke(null, new[] { h1 });

            var x = Some(123);
            var y = Option<int>.None;

            var str = JsonConvert.SerializeObject(x, JsonSerializerSettings);
            var z = method.MakeGenericMethod(typeof(Option<int>)).Invoke(null, new[] { str });
        }

        public static void StopStartImpl()
        {
            shutdownAll();
            ProcessConfig.initialiseFileSystem();

            ProcessId pid = spawn<string, string>("start-stop", () => "", (_, msg) => msg);
            tell(pid, "1");
            kill(pid);

            var kids = children(User());
            var len = kids.Length;
            Debug.Assert(len == 0);
        }

        public static void MemoTest3()
        {
            GC.Collect();

            var fix = 0;
            var count = 1000;

            Func<int, int> fn = x => x + fix;

            var m = fn.Memo();

            var nums1 = freeze(map(Range(0, count), i => m(i)));

            fix = 1000;

            var nums2 = freeze(map(Range(0, count), i => m(i)));

            var zipped = zip(nums1, nums2, (a, b) => a == b);

            var filtered = filter(zipped, v => v);

            var matches = length(filtered);
        }

        public static void LstRevLastIndexTest()
        {
            var list = List(1, 1, 2, 2, 2);
            var rev = list.Rev();

            Debug.Assert(rev.LastIndexOf(1) == 4, "Should have been 4, actually is: " + rev.LastIndexOf(1));
            Debug.Assert(rev.LastIndexOf(2) == 2, "Should have been 2, actually is: " + rev.LastIndexOf(2));
        }

        //public static void LocalRegisterTest()
        //{
        //    shutdownAll();
        //    ProcessConfig.initialise();

        //    string value = null;
        //    var pid = spawn<string>("reg-proc", msg => value = msg);

        //    var regid = register("woooo amazing", pid);

        //    //Thread.Sleep(100);

        //    var kids = children(Registered);

        //    Debug.Assert(kids.Count() == 1);
        //    Debug.Assert(kids["woooo amazing"].Path == "/root/registered/woooo amazing");

        //    tell(regid, "hello");

        //    Thread.Sleep(100);

        //    Debug.Assert(value == "hello");

        //    tell(find("woooo amazing"), "world");

        //    Thread.Sleep(100);

        //    Debug.Assert(value == "world");

        //    deregisterById(pid);

        //    Thread.Sleep(10);

        //    kids = children(Registered);
        //    Debug.Assert(kids.Count() == 0);
        //}

        public static void KillChildTest()
        {
            var pid = spawn<ProcessId, bool>(
                        "hello",
                        () => spawn<string>("world", msg => reply(msg)),
                        (cpid, flag) =>
                        {
                            if (flag)
                            {
                                kill(cpid);
                            }
                            else
                            {
                                if (Children.ContainsKey(cpid.Name.Value))
                                {
                                    reply(ask<string>(cpid, "echo") == "echo");
                                }
                                else
                                {
                                    throw new Exception("Child doesn't exist");
                                }
                            }
                            return cpid;
                        });

            Debug.Assert(ask<bool>(pid, false));

            tell(pid, true);


            ask<bool>(pid, false);

            try
            {
                Debug.Assert(ask<bool>(pid, false) == false);
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message.Contains("Child doesn't exist"));
            }

            Console.WriteLine("Works");
            Console.ReadKey();
        }

        //public static void RegisterTest()
        //{
        //    try
        //    {
        //        shutdownAll();
        //        ProcessConfig.initialise();

        //        // Let Language Ext know that Redis exists
        //        RedisCluster.register();

        //        // Connect to the Redis cluster
        //        Cluster.connect("redis", "redis-test", "localhost", "0", "global");

        //        string value = null;
        //        var pid = spawn<string>("reg-proc", msg => value = msg);

        //        var regid = register<string>("woooo amazing", pid);

        //        Thread.Sleep(10);

        //        var kids = children(Registered);

        //        Debug.Assert(kids.Count() == 1);
        //        Debug.Assert(kids["woooo amazing"].Path == "/registered/woooo amazing");

        //        tell(regid, "hello");

        //        Thread.Sleep(10);

        //        Debug.Assert(value == "hello");

        //        tell(find("woooo amazing"), "world");

        //        Thread.Sleep(10);

        //        Debug.Assert(value == "world");

        //        Thread.Sleep(10);

        //        deregister("woooo amazing");

        //        Thread.Sleep(10);

        //        kids = children(Registered);
        //        Debug.Assert(kids.Count() == 0);
        //    }
        //    finally
        //    {
        //        Cluster.disconnect();
        //    }
        //}


        class Log : IProcess<string>
        {
            public void OnMessage(string msg)
            {
                Console.WriteLine(msg);
            }

            public void OnTerminated(ProcessId pid)
            {
            }
        }

        public static void ClassBasedProcess()
        {
            var log = spawn<Log, string>("log");

            tell(log, "Hello, World");
        }

        public static Task<int> GetTheNumber()
        {
            return Task.Run(() => 123);
        }

        public static async void AsyncOption()
        {
            var x = Some(true);
            Option<bool> y = None;

            var res1 = await x.MatchAsync(
                            Some: v => GetTheNumber(),
                            None: () => 0
                        );

            Console.WriteLine(res1);

            var res2 = await y.MatchAsync(
                            Some: v => GetTheNumber(),
                            None: () => 0
                        );

            Console.WriteLine(res2);

        }


        public static void MassAddRemoveTest()
        {
            int max = 1000000;

            var items = LanguageExt.List.map(Range(1, max), _ => Tuple(Guid.NewGuid(), Guid.NewGuid()))
                                        .ToDictionary(kv => kv.Item1, kv => kv.Item2);

            var m = Map<Guid, Guid>().AddRange(items);
            Debug.Assert(m.Count == max);

            foreach (var item in items)
            {
                Debug.Assert(m.ContainsKey(item.Key));
                m = m.Remove(item.Key);
                Debug.Assert(!m.ContainsKey(item.Key));
                max--;
                Debug.Assert(m.Count == max);
            }
        }

        public static void ProcessStartupError()
        {
            shutdownAll();
            ProcessConfig.initialise();

            try
            {
                var pid = spawn<Unit, string>("world",
                    () => failwith<Unit>("Failed!"),
                    (_, __) => _, ProcessFlags.PersistInbox
                    );

                ask<Unit>(pid, unit);

                throw new Exception("Not here");
            }
            catch (ProcessException e)
            {
                Debug.Assert(e.Message == "Process issue: Invalid message type for ask (expected System.String)");
            }
        }

        public static void AskReplyError()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-test", "localhost", "0");

            try
            {
                var world = spawn<ProcessId, string>("world",
                    () => spawn<string>("hello", msg => failwith<Unit>("Failed!"), ProcessFlags.PersistInbox),
                    (pid, msg) =>
                    {
                        ask<string>(pid, msg);
                        return pid;
                    },
                    ProcessFlags.PersistInbox
                );
                tell(world, "error throwing test");
            }
            catch (Exception e)
            {
                Debug.Assert(e.Message == "Process issue: Failed!");
            }

            Thread.Sleep(1000);
        }

        //public static void RegisteredAskReply()
        //{
        //    shutdownAll();
        //    ProcessConfig.initialise();

        //    var helloServer = spawn<string>("hello-server", msg =>
        //    {
        //        reply("Hello, " + msg);
        //    });

        //    var hi = register<string>("hi", helloServer);

        //    var response = ask<string>(find("hi"), "Paul");

        //    Debug.Assert(response == "Hello, Paul");
        //}

        public static void AskReply()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-test", "localhost", "0");

            var helloServer = spawn<string>("hello-server", msg =>
                {
                    reply("Hello, " + msg);
                },
                ProcessFlags.PersistInbox);

            var response = ask<string>(helloServer, "Paul");

            Debug.Assert(response == "Hello, Paul");
        }

        public static void PubSubTest()
        {
            shutdownAll();
            ProcessConfig.initialise();

            // Spawn a process
            var pid = spawn<string>("pubsub", msg =>
            {
                // Publish anything we're sent
                publish(msg);
            });

            string value = null;

            // Subscribe to the 'string' publications
            var sub = subscribe(pid, (string v) => value = v);

            // Send string message to the process
            tell(pid, "hello");

            Thread.Sleep(500);

            Debug.Assert(value == "hello");
        }

        public static void SpawnProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

            Console.WriteLine("*** ABOUT TO SHUTDOWN ***");

            shutdownAll();

            Console.WriteLine("*** SHUTDOWN COMPLETE ***");
            ProcessConfig.initialise();

            var pid = spawn<string, string>("SpawnProcess", () => "", (_, msg) => msg);

            string value = null;
            observeState<string>(pid).Subscribe(x => value = x);

            tell(pid, "hello, world");

            Thread.Sleep(100);

            if (value != "hello, world") Console.WriteLine(" Value actually is: " + value);

            Debug.Assert(value == "hello, world");

            kill(pid);

            Debug.Assert(children(User()).Count == 0);

            Console.WriteLine("*** END OF TEST ***");
        }

        public class Bindings
        {
            public readonly Map<string, int> Map;
            public Bindings(params Tuple<string, int>[] items)
            {
                Map = Map(items);
            }

            public static Bindings New(params Tuple<string, int>[] items)
            {
                return new Bindings(items);
            }
        }
        public static void ReaderAskTest()
        {
            var lookupVar = fun((string name, Bindings bindings) => LanguageExt.Map.find(bindings.Map, name).IfNone(0));

            var calcIsCountCorrect = from count in asks((Bindings env) => lookupVar("count", env))
                                     from bindings in ask<Bindings>()
                                     select count == LanguageExt.Map.length(bindings.Map);

            var sampleBindings = Bindings.New(
                                    Tuple("count", 3),
                                    Tuple("1", 1),
                                    Tuple("b", 2)
                                    );

            bool res = calcIsCountCorrect(sampleBindings).Value;

            Debug.Assert(res);
        }

        // TODO: Drop for a better test
        //public static void LiftTest()
        //{
        //    var x = List(None, Some(1), Some(2), Some(3), Some(4));

        //    Debug.Assert(x.Lift() == None);
        //    Debug.Assert(x.LiftT() == 0);

        //    var y = List(Some(1), Some(2), Some(3), Some(4));

        //    Debug.Assert(y.Lift() == Some(1));
        //    Debug.Assert(y.LiftT() == 1);

        //    var z = Some(Some(Some(Some(1))));

        //    Debug.Assert(z.LiftT().Lift() == Some(1));
        //    Debug.Assert(z.LiftT().LiftT() == 1);
        //}

        public static void BindingTest()
        {
            var x = from a in ask<string>()
                    from b in tell("Hello " + a)
                    select b;

            var res = x("everyone").Value().Output.Head();
        }

        // TODO: Restore when refactor of type-classes complete
        //public static void WrappedOptionOptionLinqTest()
        //{
        //    var opt = Some(Some(Some(100)));

        //    var res = from x in opt
        //              from y in x
        //              select y * 2;

        //    Debug.Assert(res.IfNone(0).IfNone(0) == 200);

        //    opt = Some(Some<Option<int>>(None));

        //    res = from x in opt
        //          from y in x
        //          select y * 2;

        //    Debug.Assert(res.IfNone(0).IfNone(1) == 1);
        //}

        public static void WrappedListTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));
            var res = opt.FoldT(0, (s, v) => s + v);
            var mopt = opt.MapT(x => x * 2);
            var mres = mopt.FoldT(0, (s, v) => s + v);

            Debug.Assert(res == 15, "Expected 15, but got " + res);
            Debug.Assert(mres == 30, "Expected 30, but got " + mres);
            Debug.Assert(opt.CountT() == 5, "opt != 5, (" + opt.CountT() + ")");
            Debug.Assert(mopt.CountT() == 5, "mopt != 5, (" + mopt.CountT() + ")");

            opt = None;
            res = opt.FoldT(0, (s, v) => s + v);

            Debug.Assert(res == 0, "res != 0, got " + res);
            Debug.Assert(opt.CountT() == 0, "opt.Count() != 0, got " + opt.CountT());
        }

        public static void WrappedListLinqTest()
        {
            var opt = Some(List(1, 2, 3, 4, 5));

            var res = from x in opt
                      from y in x
                      select y * 2;

            match(res,
                () =>
                {
                    Debug.Assert(false);
                    return false;
                },
                (x,xs) =>
                {
                    var vs = x.Cons(xs).ToArray();

                    Debug.Assert(vs[0] == 2);
                    Debug.Assert(vs[1] == 4);
                    Debug.Assert(vs[2] == 6);
                    Debug.Assert(vs[3] == 8);
                    Debug.Assert(vs[4] == 10);
                    return true;
                }
            );
        }

        public static TryOption<int> GetTryOptionValue(bool select) => () =>
            select
                ? Some(10)
                : None;

        //public static void LinqTest()
        //{
        //    var res = (from v in GetTryOptionValue(true).Match(
        //                             Some: r  => List(r),
        //                             None: () => List<int>(),
        //                             Fail: e  => List<int>()
        //                         )
        //               from r in Range(1, 10)
        //               select v * r)
        //              .ToList();
        //}

        public static void ExTest4()
        {
            string x = match(Number<Exception>(9),
                              Succ: v => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<SystemException>(e => "It's a system exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled"));

            if (x == "Not handled")
            {
                Console.WriteLine("Eq");
            }
        }

        public static Try<int> Number<T>(int x) where T : Exception, new() => () =>
            x % 2 == 0
                ? x
                : raise<int>(new T());


        public static void MemoTest()
        {
            var fix = 0;
            var count = 10000;

            Func<int, int> fn = x => x + fix;

            var m = fn.Memo();

            var nums1 = map(Range(0, count), i => m(i)).ToList();

            fix = 1000;

            var nums2 = map(Range(0, count), i => m(i)).ToList();


            var res = length(
                filter(
                    zip(
                        nums1,
                        nums2,
                        (a, b) => a == b
                        ),
                        v => v
                    )
                ) == count;
        }

        public static void UnsafeOptionTest()
        {
            string empty = null;
            OptionUnsafe<string> optional1 = None;
            OptionUnsafe<string> optional2 = None;

            var value = optional1 || optional2 || SomeUnsafe(empty);
            if (value == "fred")
            {
                Console.WriteLine("Yay");
            }
            else
            {
                Console.WriteLine("Nay");
            }
        }

        public static void SpawnErrorSurviveProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

            var pid = spawn<int, string>("SpawnAnErrorProcess", () => 0, (count, _) =>
            {
                count++;
                if (count == 3) throw new Exception("fail");
                return count;
            });

            int value = 0;
            observeState<int>(pid).Subscribe(x => value = x);

            tell(pid, "msg");
            tell(pid, "msg");

            Thread.Sleep(20);

            Debug.Assert(value == 2);

            Thread.Sleep(20);

            tell(pid, "msg");

            Thread.Sleep(100);

            Debug.Assert(value == 0, "Expected 0, got " + value);

            kill(pid);
        }

        public static void SpawnAndKillProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

            ProcessId pid = spawn<string, string>("SpawnAndKillProcess", () => "", (_, msg) => msg);
            tell(pid, "1");
            kill(pid);
            tell(pid, "2");

            var kids = children(User());
            var len = kids.Length;
            Debug.Assert(len == 0);
        }

        public static void SpawnAndKillHierarchy()
        {
            shutdownAll();
            ProcessConfig.initialise();

            int value = 0;

            var pid = spawn<int, string>("SpawnAndKillHierarchy.TopLevel",
                () => { spawn<string>("SpawnAndKillHierarchy.ChildLevel", x => Console.WriteLine(x)); return 0; },
                (_, msg) => value = Int32.Parse(msg)
            );

            tell(pid, "1");
            kill(pid);
            tell(pid, "2");

            Thread.Sleep(200);

            Debug.Assert(value == 1, "Expected 1, got " + value);
            Debug.Assert(children(User()).Length == 0);
        }

        public static int DepthMax(int depth) =>
            depth == 0
                ? 1
                : (int)Math.Pow(5, (double)depth) + DepthMax(depth - 1);

        public static void MassiveSpawnAndKillHierarchy2()
        {
            Func<Unit> setup = null;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            shutdownAll();
            ProcessConfig.initialise();

            var actor = fun((Unit s, string msg) =>
            {
                iter(Children.Values, child => tell(child, msg));
            });

            setup = fun(() =>
            {
                int level = Int32.Parse(Self.Name.Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            tell(zero, "Hello");
            kill(zero);

            Debug.Assert(children(User()).Count() == 0);
        }

        public static void MassiveSpawnAndKillHierarchy()
        {
            shutdownAll();
            ProcessConfig.initialise();

            Func<Unit> setup = null;
            int count = 0;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            var actor = fun((Unit s, string msg) =>
            {
                Console.WriteLine(msg);
                Interlocked.Increment(ref count);
                iter(Children.Values, child => tell(child, msg));
            });

            setup = fun(() =>
            {
                Interlocked.Increment(ref count);

                int level = Int32.Parse(Self.Name.Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i =>
                    {
                        Console.WriteLine("Spawning: " + level + "_" + i);
                        spawn(level + "_" + i, setup, actor);
                        Console.WriteLine("Done spawning: " + level + "_" + i);
                    });
                }
            });

            var zero = spawn("0", setup, actor);
            tell(zero, "Hello");

            // crude, but whatever
            while (count < max) Thread.Sleep(50);
            count = 0;

            kill(zero);

            Thread.Sleep(3000);

            Debug.Assert(children(User()).Count() == 0);
        }

        public static void ScheduledMsgTest()
        {
            string v = "";

            var p = spawn<string>("test-sch", x => v = x);

            var future = DateTime.Now.AddSeconds(3);

            tell(p, "hello", future);

            while (DateTime.Now < future.AddMilliseconds(1000))
            {
                Thread.Sleep(10);
            }
            Debug.Assert(v == "hello");
        }

        public static void ShowChildren(ProcessId pid)
        {
            Console.WriteLine(pid);
            children(pid).Iter(ShowChildren);
        }

        public static void PStringCasting()
        {
            string str = "Hello";
            var pstr = str.ToCharArray().ToPString();
            var ostr = pstr.Cast<object>();
            pstr = ostr.Cast<char>();
        }

        public static void BlockingQueueTest()
        {
            var queue = new BlockingQueue<int>();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            long current = watch.ElapsedMilliseconds;
            int count = 0;

            Task.Run(() => queue.Receive(_ =>
            {
                count++;
                if (watch.ElapsedMilliseconds - current > 1000)
                {
                    Console.WriteLine($"Messages per second {count}");
                    current = watch.ElapsedMilliseconds;
                    count = 0;
                }
            }));

            for(int i =0; ; i++)
            {
                queue.Send(i);
            }
        }
    }
}