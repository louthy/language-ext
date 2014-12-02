using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using LanguageExt;
using LanguageExt.List;
using LanguageExt.Prelude;
using LanguageExt.Process;

namespace LanguageExtTests
{
    [TestFixture]
    public class ProcessTests
    {
        [Test]
        public void RegisterTest()
        {
            restart();

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register("woooo amazing", pid);

            Assert.IsTrue(registered().Count() == 1);
            Assert.IsTrue(registered().First().Value == "/root/system/registered/woooo amazing");

            tell(regid, "hello");

            Thread.Sleep(100);

            Assert.IsTrue(value == "hello");

            Thread.Sleep(100);

            unregister("woooo amazing");

            Thread.Sleep(100);

            Assert.IsTrue(registered().Count() == 0);
        }

        [Test]
        public void SpawnProcess()
        {
            restart();

            string value = null;
            var pid = spawn<string>("SpawnProcess", msg => value = msg );

            tell(pid, "hello, world");

            Thread.Sleep(200);
            Assert.IsTrue(value == "hello, world");

            kill(pid);
        }

        [Test]
        public void SpawnErrorSurviveProcess()
        {
            restart();

            int value = 0;
            int count = 0;

            var pid = spawn<string>("SpawnAnErrorProcess", _ =>
                {
                    if (count++ == 0)
                        throw new Exception("fail");
                    else
                        value = count;
                });

            tell(pid, "msg");
            tell(pid, "msg");
            tell(pid, "msg");

            Thread.Sleep(400);
            Assert.IsTrue(value == 3);

            kill(pid);
        }

        [Test]
        public void SpawnAndKillProcess()
        {
            restart();

            string value = null;
            var pid = spawn<string>("SpawnAndKillProcess", msg => value = msg);
            tell(pid, "1");

            Thread.Sleep(200);

            kill(pid);

            Thread.Sleep(200);

            tell(pid, "2");

            Thread.Sleep(200);

            Assert.IsTrue(value == "1");
            Assert.IsTrue(length(children()) == 0);
        }

        [Test]
        public void SpawnAndKillHierarchy()
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

            Thread.Sleep(200);

            kill(pid);

            Thread.Sleep(200);

            tell(pid, "2");

            Thread.Sleep(200);

            Assert.IsTrue(value == "1");
            Assert.IsTrue(length(children()) == 0);
        }

        public static int DepthMax(int depth) =>
            depth == 0
                ? 1
                : (int)Math.Pow(5, (double)depth) + DepthMax(depth - 1);

        [Test]
        public void MassiveSpawnAndKillHierarchy()
        {
            Func<Unit> setup = null;
            int count = 0;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            restart();

            var actor = fun((Unit s, string msg) =>
            {
                Interlocked.Increment(ref count);
                iter(children(), child => tell(child, msg));
            });

            setup = fun(() =>
            {
                Interlocked.Increment(ref count);

                int level = Int32.Parse(self().Name.Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(range(0, nodes), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            while (count < max) Thread.Sleep(50);
            count = 0;

            tell(zero, "Hello");

            // crude, but whatever
            while (count < max) Thread.Sleep(50);
            count = 0;

            shutdown(zero);

            Thread.Sleep(3000);

            Assert.IsTrue(children(user()).Count() == 0);
        }
    }
}
