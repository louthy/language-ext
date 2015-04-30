using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LanguageExt;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Diagnostics;

namespace ProcessSample
{
    class TestBed
    {
        public static void RunTests()
        {
            UnsafeOptionTest();
            MassiveSpawnAndKillHierarchy();
            SpawnAndKillProcess();
            SpawnAndKillHierarchy();
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
                    iter(range(0, nodes), i => spawn(level + "_" + i, setup, actor));
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
