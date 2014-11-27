using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.List;
using LanguageExt.Prelude;
using LanguageExt.Process;
using System.Diagnostics;

namespace ProcessSample
{
    class TestBed
    {
        public static void RunTests()
        {
            MassiveSpawnAndKillHierarchy();
            SpawnAndKillProcess();
            SpawnAndKillHierarchy();
        }

        public static void SpawnAndKillProcess()
        {
            restart();

            string value = null;
            ProcessId pid = spawn<string>("SpawnAndKillProcess", msg => value = msg);
            tell(pid, "1");

            Thread.Sleep(100);

            kill(pid);

            Thread.Sleep(100);

            tell(pid, "2");

            Thread.Sleep(100);

            Debug.Assert(value == "1");

            var kids = children();
            var len = length(kids);
            Debug.Assert(len == 0);
        }

        public static void SpawnAndKillHierarchy()
        {
            restart();

            string value = null;
            ProcessId parentId;

            var pid = spawn<Unit, string>("SpawnAndKillHierarchy.TopLevel",
                () =>
                {
                    parentId = parent();

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
            Debug.Assert(length(children()) == 0);
        }

        public static int DepthMax(int depth) =>
            depth == 0
                ? 1
                : (int)Math.Pow(5, (double)depth) + DepthMax(depth - 1);

        public static void MassiveSpawnAndKillHierarchy()
        {
            restart();

            int count = 0;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            Console.WriteLine("Max: " + max);

            Func<Unit> setup = null;

            var actor = fun((Unit s, string msg) =>
            {
                Interlocked.Increment(ref count);
                iter(children(), child => tell(child, msg));
                Console.WriteLine(self() + " : " + msg);
            });

            setup = fun(() =>
            {
                Interlocked.Increment(ref count);

                Console.WriteLine("spawn: " + self() + "\t"+ count);

                int level = Int32.Parse(self().Name.Value.Split('_').First()) + 1;
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

            ShowChildren(user());

            shutdown(zero);

            Thread.Sleep(3000);

            ShowChildren(user());

            Console.ReadKey();
        }

        public static void ShowChildren(ProcessId pid)
        {
            Console.WriteLine(pid);
            iter(children(pid), ShowChildren);
        }
    }
}
