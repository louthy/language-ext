using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Diagnostics;

namespace ProcessSample
{
    /// <summary>
    /// This is just a dumping ground I use for debugging the library, you can ignore this.
    /// </summary>
    class TestBed
    {
        public static void RunTests()
        {
            WrappedOptionOptionLinqTest();
            WrappedListLinqTest();
            WrappedListTest();
            LinqTest();
            ExTest4();
            MemoTest();
            UnsafeOptionTest();
            MassiveSpawnAndKillHierarchy();
            SpawnAndKillProcess();
            SpawnAndKillHierarchy();
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

        private static TryOption<int> GetTryOptionValue(bool select) => () =>
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

        private static Try<int> Number<T>(int x) where T : Exception, new() => () =>
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

        public static void SpawnAndKillProcess()
        {
            shutdownAll();

            string value = null;
            ProcessId pid = spawn<string>("SpawnAndKillProcess", msg => value = msg);
            tell(pid, "1");

            Thread.Sleep(100);

            kill(pid);

            Thread.Sleep(100);

            tell(pid, "2");

            Thread.Sleep(100);

            Debug.Assert(value == "1");

            var kids = Children;
            var len = length(kids);
            Debug.Assert(len == 0);
        }

        public static void SpawnAndKillHierarchy()
        {
            shutdownAll();

            string value = null;
            ProcessId parentId;

            var pid = spawn<Unit, string>("SpawnAndKillHierarchy.TopLevel",
                () =>
                {
                    parentId = Parent;

                    spawn<string>("SpawnAndKillHierarchy.ChildLevel", msg => value = msg);
                    return unit;
                },
                (state, msg) =>
                {
                    value = msg;
                    return state;
                }
            );

            tell(pid, "1");
            Thread.Sleep(100);
            kill(pid);
            Thread.Sleep(100);
            tell(pid, "2");
            Thread.Sleep(100);

            Debug.Assert(value == "1");
            Debug.Assert(length(Children) == 0);
        }

        public static int DepthMax(int depth) =>
            depth == 0
                ? 1
                : (int)Math.Pow(5, (double)depth) + DepthMax(depth - 1);

        public static void MassiveSpawnAndKillHierarchy()
        {
            shutdownAll();

            int count = 0;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            Console.WriteLine("Max: " + max);

            Func<Unit> setup = null;

            var actor = fun((Action<Unit, string>)((Unit s, string msg) =>
            {
                Interlocked.Increment(ref count);
                iter((IEnumerable<ProcessId>)LanguageExt.Process.Children, child => tell(child, msg));
                Console.WriteLine(Self + " : " + msg);
            }));

            setup = fun(() =>
            {
                Interlocked.Increment(ref count);

                Console.WriteLine("spawn: " + Self + "\t"+ count);

                int level = Int32.Parse(Self.Name.Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            while (count < max) Thread.Sleep(10);
            count = 0;


            tell(zero, "Hello");


            while (count < max) Thread.Sleep(10);
            count = 0;

            ShowChildren(User);

            shutdown(zero);

            Thread.Sleep(3000);

            ShowChildren(User);

            Console.ReadKey();
        }

        public static void ShowChildren(ProcessId pid)
        {
            Console.WriteLine(pid);
            iter(children(pid), ShowChildren);
        }
    }
}
