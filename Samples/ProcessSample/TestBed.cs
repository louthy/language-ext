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

        public static void MassiveSpawnAndKillHierarchy()
        {
            restart();

            Func<Unit> setup = null;

            var actor = fun((Unit s, string msg) =>
            {
                iter(children(), child => tell(child, msg));
                Console.WriteLine(self() + " : " + msg);
            });

            int count = 0;

            setup = fun(() =>
            {
                Console.WriteLine("spawn: " + self() + ".  "+ (count++));

                int level = Int32.Parse(self().Name.Value.Split('_').First()) + 1;
                if (level < 7)
                {
                    iter(range(0, 5), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            tell(zero, "Hello");

            Console.ReadKey();
        }
    }
}
