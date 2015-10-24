using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LanguageExt;
using LanguageExt.Trans;
using LanguageExt.Trans.Linq;
using System.Reactive.Linq;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Diagnostics;
using System.Threading.Tasks;

// ************************************************************************************
// 
//  This is just a dumping ground I use for debugging the library, you can ignore this
// 
// ************************************************************************************

namespace TestBed
{
    class Tests
    {
        public static void MemoTest3()
        {
            GC.Collect();

            var fix = 0;
            var count = 1000;

            Func<int, int> fn = x => x + fix;

            var m = fn.memo();

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

        public static void LocalRegisterTest()
        {
            shutdownAll();

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register<string>("woooo amazing", pid);

            //Thread.Sleep(100);

            var kids = children(Registered);

            Debug.Assert(kids.Count() == 1);
            Debug.Assert(kids["woooo amazing"].Path == "/root/registered/woooo amazing");

            tell(regid, "hello");

            Thread.Sleep(100);

            Debug.Assert(value == "hello");

            tell(find("woooo amazing"), "world");

            Thread.Sleep(100);

            Debug.Assert(value == "world");

            deregister("woooo amazing");

            Thread.Sleep(10);

            kids = children(Registered);
            Debug.Assert(kids.Count() == 0);
        }

        public static void KillChildTest()
        {
            var pid = spawn<ProcessId, bool>(
                        "hello", 
                        ()   => spawn<string>("world", msg => reply(msg) ),
                        (cpid, flag) =>
                        {
                            if (flag)
                            {
                                kill(cpid);
                            }
                            else
                            {
                                if (Children.ContainsKey(cpid.GetName().Value))
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

        public static void RegisterTest()
        {
            try
            {
                shutdownAll();

                // Let Language Ext know that Redis exists
                RedisCluster.register();

                // Connect to the Redis cluster
                Cluster.connect("redis", "redis-test", "localhost", "0");

                string value = null;
                var pid = spawn<string>("reg-proc", msg => value = msg);

                var regid = register<string>("woooo amazing", pid);

                Thread.Sleep(10);

                var kids = children(Registered);

                Debug.Assert(kids.Count() == 1);
                Debug.Assert(kids["woooo amazing"].Path == "/registered/woooo amazing");

                tell(regid, "hello");

                Thread.Sleep(10);

                Debug.Assert(value == "hello");

                tell(find("woooo amazing"), "world");

                Thread.Sleep(10);

                Debug.Assert(value == "world");

                Thread.Sleep(10);

                deregister("woooo amazing");

                Thread.Sleep(10);

                kids = children(Registered);
                Debug.Assert(kids.Count() == 0);
            }
            finally
            {
                Cluster.disconnect();
            }
        }


        class Log : IProcess<string>
        {
            public void OnMessage(string msg)
            {
                Console.WriteLine(msg);
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



        public static void MapOptionTest()
        {
            var m = Map<Option<int>, Map<Option<int>, string>>();

            m = m.AddOrUpdate(Some(1), Some(1), "Some Some");
            m = m.AddOrUpdate(None, Some(1), "None Some");
            m = m.AddOrUpdate(Some(1), None, "Some None");
            m = m.AddOrUpdate(None, None, "None None");

            Debug.Assert(m[Some(1)][Some(1)] == "Some Some");
            Debug.Assert(m[None][Some(1)] == "None Some");
            Debug.Assert(m[Some(1)][None] == "Some None");
            Debug.Assert(m[None][None] == "None None");

            Debug.Assert(m.CountT() == 4);

            m = m.FilterT(v => v.EndsWith("None", StringComparison.Ordinal));

            Debug.Assert(m.CountT() == 2);
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

            try
            {
                var pid = spawn<Unit, string>("world",
                    ()      => failwith<Unit>("Failed!"),
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
            Cluster.connect("redis", "redis-test", "localhost", "0");

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

        public static void RegisteredAskReply()
        {
            shutdownAll();

            var helloServer = spawn<string>("hello-server", msg =>
            {
                reply("Hello, " + msg);
            });

            var hi = register<string>("hi", helloServer);

            var response = ask<string>(find("hi"), "Paul");

            Debug.Assert(response == "Hello, Paul");
        }

        public static void AskReply()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-test", "localhost", "0");

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

            Console.WriteLine("*** ABOUT TO SHUTDOWN ***");

            shutdownAll();

            Console.WriteLine("*** SHUTDOWN COMPLETE ***");

            var pid = spawn<string, string>("SpawnProcess", () => "", (_, msg) => msg);

            string value = null;
            observeState<string>(pid).Subscribe(x => value = x);

            tell(pid, "hello, world");

            Thread.Sleep(100);

            if (value != "hello, world") Console.WriteLine(" Value actually is: " + value);

            Debug.Assert(value == "hello, world");

            kill(pid);

            Debug.Assert(children(User).Count == 0);

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

        public static void LiftTest()
        {
            var x = List(None, Some(1), Some(2), Some(3), Some(4));

            Debug.Assert(x.Lift() == None);
            Debug.Assert(x.LiftT() == 0);

            var y = List(Some(1), Some(2), Some(3), Some(4));

            Debug.Assert(y.Lift() == Some(1));
            Debug.Assert(y.LiftT() == 1);

            var z = Some(Some(Some(Some(1))));

            Debug.Assert(z.LiftT().Lift() == Some(1));
            Debug.Assert(z.LiftT().LiftT() == 1);
        }

        public static void BindingTest()
        {
            var x = from a in ask<string>()
                    from b in tell("Hello " + a)
                    select b;

            var res = x("everyone").Value().Output.Head();
        }

        public static void WrappedOptionOptionLinqTest()
        {
            var opt = Some(Some(Some(100)));

            var res = from x in opt
                      from y in x
                      select y * 2;

            Debug.Assert(res.IfNone(0).IfNone(0) == 200);

            opt = Some(Some<Option<int>>(None));

            res = from x in opt
                  from y in x
                  select y * 2;

            Debug.Assert(res.IfNone(0).IfNone(1) == 1);
        }

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
                      select x * 2;

            match(res,
                Some: x =>
                {
                    Debug.Assert(x[0] == 2);
                    Debug.Assert(x[1] == 4);
                    Debug.Assert(x[2] == 6);
                    Debug.Assert(x[3] == 8);
                    Debug.Assert(x[4] == 10);
                },
                None: () => Debug.Assert(false)
            );
        }

        public static TryOption<int> GetTryOptionValue(bool select) => () =>
            select
                ? Some(10)
                : None;

        public static void LinqTest()
        {
            var res = (from v in match(
                                     GetTryOptionValue(true).AsEnumerable(),
                                     Right: r => List(r),
                                     Left: l => List<int>()
                                 )
                       from r in Range(1, 10)
                       select v * r)
                      .ToList();
        }

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

            var m = fn.memo();

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

            var pid = spawn<int, string>("SpawnAnErrorProcess", () => 0, (count,_) =>
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

            ProcessId pid = spawn<string, string>("SpawnAndKillProcess", () => "", (_, msg) => msg);
            tell(pid, "1");
            kill(pid);
            tell(pid, "2");

            var kids = children(User);
            var len = kids.Length;
            Debug.Assert(len == 0);
        }

        public static void SpawnAndKillHierarchy()
        {
            shutdownAll();

            int value = 0;

            var pid = spawn<int, string>("SpawnAndKillHierarchy.TopLevel",
                () => { spawn<string>("SpawnAndKillHierarchy.ChildLevel", Console.WriteLine); return 0; },
                (_, msg) => value = Int32.Parse(msg)
            );

            tell(pid, "1");
            kill(pid);
            tell(pid, "2");

            Thread.Sleep(200);

            Debug.Assert(value == 1,"Expected 1, got "+value);
            Debug.Assert(children(User).Length == 0);
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

            var actor = fun((Unit s, string msg) =>
            {
                iter(Children.Values, child => tell(child, msg));
            });

            setup = fun(() =>
            {
                int level = Int32.Parse(Self.GetName().Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            tell(zero, "Hello");
            kill(zero);

            Debug.Assert(children(User).Count() == 0);
        }

        public static void MassiveSpawnAndKillHierarchy()
        {
            shutdownAll();

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

                int level = Int32.Parse(Self.GetName().Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i => {
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

            Debug.Assert(children(User).Count() == 0);
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
    }
}
