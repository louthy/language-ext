using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;

using LanguageExt;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExtTests
{
    [TestFixture]
    public class ProcessTests
    {
        [Test]
        public static void AskReplyError()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-test", "localhost", "0");

            var world = spawn<ProcessId, string>("world",
                () => spawn<string>("hello", msg => failwith<Unit>("Failed!"), ProcessFlags.PersistInbox),
                (pid, msg) =>
                {
                    Assert.Throws<ProcessException>(() =>
                    {
                        ask<string>(pid, msg);
                    });
                    return pid;
                },
                ProcessFlags.PersistInbox
            );

            tell(world, "error throwing test");
            Thread.Sleep(1000);
        }

        [Test]
        public void AskReply()
        {
            shutdownAll();

            var helloServer = spawn<string>("hello-server", msg =>
            {
                reply("Hello, " + msg);
            });

            var response = ask<string>(helloServer, "Paul");

            Assert.IsTrue(response == "Hello, Paul");
        }

        [Test]
        public void PubSubTest()
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

            Assert.IsTrue(value == "hello");
        }

        [Test]
        public void LocalRegisterTest()
        {
            shutdownAll();

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register<string>("woooo amazing", pid);

            Thread.Sleep(100);

            var kids = children(Registered);

            Assert.IsTrue(kids.Count() == 1);
            Assert.IsTrue(kids["woooo amazing"].Path == "/root/registered/woooo amazing");

            tell(regid, "hello");

            Thread.Sleep(2000);

            Assert.IsTrue(value == "hello");

            tell(find("woooo amazing"), "world");

            Thread.Sleep(2000);

            Assert.IsTrue(value == "world");

            Thread.Sleep(100);

            deregister("woooo amazing");

            Thread.Sleep(100);

            kids = children(Registered);
            Assert.IsTrue(kids.Count() == 0);
        }

        [Test]
        public void RedisRegisterTest()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-test", "localhost", "0");

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register<string>("woooo amazing", pid);

            Thread.Sleep(100);

            var kids = children(Registered);

            Assert.IsTrue(kids.Count() == 1);
            Assert.IsTrue(kids["woooo amazing"].Path == "/redis-test/registered/woooo amazing");

            tell(regid, "hello");

            Thread.Sleep(2000);  

            Assert.IsTrue(value == "hello");

            tell(find("woooo amazing"), "world");

            Thread.Sleep(2000);

            Assert.IsTrue(value == "world");

            Thread.Sleep(100);

            deregister("woooo amazing");

            Thread.Sleep(100);

            kids = children(Registered);
            Assert.IsTrue(kids.Count() == 0);
        }


        [Test]
        public void SpawnProcess()
        {
            shutdownAll();

            string value = null;
            var pid = spawn<string>("SpawnProcess", msg => value = msg);

            tell(pid, "hello, world");

            Thread.Sleep(200);
            Assert.IsTrue(value == "hello, world");

            kill(pid);
        }

        [Test]
        public void SpawnErrorSurviveProcess()
        {
            shutdownAll();

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
            shutdownAll();

            string value = null;
            var pid = spawn<string>("SpawnAndKillProcess", msg => value = msg);
            tell(pid, "1");

            Thread.Sleep(200);

            kill(pid);

            Thread.Sleep(200);

            tell(pid, "2");

            Thread.Sleep(200);

            Assert.IsTrue(value == "1");
            Assert.IsTrue(children(User).Length == 0);
        }

        [Test]
        public void SpawnAndKillHierarchy()
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

            Thread.Sleep(200);

            kill(pid);

            Thread.Sleep(200);

            tell(pid, "2");

            Thread.Sleep(200);

            Assert.IsTrue(value == "1", "Expected 1, actually equals: "+ value);
            Assert.IsTrue(children(User).Length == 0);
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

            shutdownAll();

            var actor = fun((Unit s, string msg) =>
            {
                Interlocked.Increment(ref count);
                iter(Children.Values, child => tell(child, msg));
            });

            setup = fun(() =>
            {
                Interlocked.Increment(ref count);

                int level = Int32.Parse(Self.GetName().Value.Split('_').First()) + 1;
                if (level <= depth)
                {
                    iter(Range(0, nodes), i => spawn(level + "_" + i, setup, actor));
                }
            });

            var zero = spawn("0", setup, actor);

            while (count < max) Thread.Sleep(50);
            count = 0;

            tell(zero, "Hello");

            // crude, but whatever
            while (count < max) Thread.Sleep(50);
            count = 0;

            kill(zero);

            Thread.Sleep(3000);

            Assert.IsTrue(children(User).Count() == 0);
        }

        [Test]
        public void ScheduledMsgTest()
        {
            shutdownAll();

            string v = "";

            var p = spawn<string>("test-sch", x => v = x);

            var future = DateTime.Now.AddSeconds(3);

            tell(p, "hello", future);

            while (DateTime.Now < future)
            {
                Assert.IsTrue(v == "");
                Thread.Sleep(10);
            }

            while (DateTime.Now < future.AddMilliseconds(100))
            {
                Thread.Sleep(10);
            }
            Assert.IsTrue(v == "hello");
        }
    }
}
