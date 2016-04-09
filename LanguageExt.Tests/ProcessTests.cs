using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;

using LanguageExt;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExtTests
{
    
    public class ProcessTests
    {
#if !CI
        [Fact]
        public static void AskReplyError()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-ask-reply-test", "localhost", "0");

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
        }

        [Fact]
        public void AskReply()
        {
            shutdownAll();
            ProcessConfig.initialise();

            var helloServer = spawn<string>("hello-server", msg =>
            {
                reply("Hello, " + msg);
            });

            var response = ask<string>(helloServer, "Paul");

            Assert.True(response == "Hello, Paul");
        }

        [Fact]
        public void PubSubTest()
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

            Thread.Sleep(50);

            Assert.True(value == "hello");
        }

        [Fact]
        public void LocalRegisterTest()
        {
            shutdownAll();
            ProcessConfig.initialise();

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register("woooo amazing", pid);

            Assert.True(regid == "/disp/reg/local-woooo amazing");

            tell(regid, "hello");

            Thread.Sleep(100);

            Assert.True(value == "hello");

            tell(find("woooo amazing"), "world");

            Thread.Sleep(100);

            Assert.True(value == "world");

            deregisterById(pid);

            Thread.Sleep(100);

            try
            {
                tell(find("woooo amazing"), "noooo");
            }
            catch(Exception e)
            {
                Assert.True(e.Message.Contains("No processes in group"));
            }

            Thread.Sleep(100);

            Assert.True(value != "noooo");
        }

        [Fact]
        public void RedisRegisterTest()
        {
            shutdownAll();

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            ProcessConfig.initialise("sys", "global", "redis-test", "localhost", "0");

            string value = null;
            var pid = spawn<string>("reg-proc", msg => value = msg);

            var regid = register("woooo amazing", pid);

            Assert.True(regid == "/disp/reg/global-woooo amazing");

            Thread.Sleep(200);

            tell(regid, "hello");

            Thread.Sleep(200);

            Assert.True(value == "hello");

            tell(find("woooo amazing"), "world");

            Thread.Sleep(200);

            Assert.True(value == "world");

            deregisterById(pid);

            Thread.Sleep(200);

            try
            {
                tell(find("woooo amazing"), "noooo");
            }
            catch(Exception e)
            {
                Assert.True(e.Message.Contains("No processes in group"));
            }

            Thread.Sleep(200);

            Assert.True(value != "noooo");
        }

        [Fact]
        public void SpawnProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

            string value = null;
            var pid = spawn<string>("SpawnProcess", msg => value = msg);

            tell(pid, "hello, world");

            Thread.Sleep(200);
            Assert.True(value == "hello, world");

            kill(pid);
        }

        [Fact]
        public void SpawnErrorSurviveProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

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
            Assert.True(value == 3);

            kill(pid);
        }

        [Fact]
        public void SpawnAndKillProcess()
        {
            shutdownAll();
            ProcessConfig.initialise();

            string value = null;
            var pid = spawn<string>("SpawnAndKillProcess", msg => value = msg);
            tell(pid, "1");

            Thread.Sleep(10);

            kill(pid);

            Thread.Sleep(10);

            Assert.Throws<ProcessException>(() =>
            {
                tell(pid, "2");
            });

            Thread.Sleep(10);

            Assert.True(value == "1");
            Assert.True(children(User()).Length == 0);
        }

        [Fact]
        public void SpawnAndKillHierarchy()
        {
            shutdownAll();
            ProcessConfig.initialise();

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

            Thread.Sleep(10);

            kill(pid);

            Thread.Sleep(10);

            Assert.Throws<ProcessException>(() =>
            {
                tell(pid, "2");
            });

            Thread.Sleep(10);

            Assert.True(value == "1", "Expected 1, actually equals: "+ value);
            Assert.True(children(User()).Length == 0);
        }

        [Fact]
        public static void ProcessStartupInvalidTypeError()
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

                throw new Exception("Shouldn't get here");
            }
            catch (ProcessException e)
            {
                Assert.True(e.Message == "Process issue: Invalid message type for ask (expected System.String)");
            }
        }

        //  Startup works differently now, it's runs asynchronously, but not
        //  on the first message. This will need a new test.
        //
        //[Fact]
        //public static void ProcessStartupError()
        //{
        //    shutdownAll();

        //    try
        //    {
        //        var pid = spawn<Unit, string>("world",
        //            () => failwith<Unit>("Failed!"),
        //            (_, __) => _, ProcessFlags.PersistInbox
        //            );

        //        ask<Unit>(pid, "test");

        //        throw new Exception("Shouldn't get here");
        //    }
        //    catch (ProcessException e)
        //    {
        //        Assert.True(e.Message == "Process issue: Process failed to start: /root/user/world");
        //    }
        //}

        public static int DepthMax(int depth) =>
            depth == 0
                ? 1
                : (int)Math.Pow(5, (double)depth) + DepthMax(depth - 1);

        [Fact]
        public void MassiveSpawnAndKillHierarchy()
        {
            Func<Unit> setup = null;
            int count = 0;
            int depth = 6;
            int nodes = 5;
            int max = DepthMax(depth);

            shutdownAll();
            ProcessConfig.initialise();

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
            tell(zero, "Hello");

            // crude, but whatever
            while (count < max) Thread.Sleep(20);
            count = 0;

            kill(zero);

            Thread.Sleep(500);

            Assert.True(children(User()).Count() == 0);
        }

        [Fact]
        public void ScheduledMsgTest()
        {
            shutdownAll();
            ProcessConfig.initialise();

            string v = "";

            var p = spawn<string>("test-sch", x => v = x);

            var future = DateTime.UtcNow.AddSeconds(3);

            tell(p, "hello", future);

            while (DateTime.UtcNow < future)
            {
                Assert.True(v == "");
                Thread.Sleep(10);
            }

            while (DateTime.UtcNow < future.AddMilliseconds(1000))
            {
                Thread.Sleep(10);
            }
            Assert.True(v == "hello");
        }
#endif
    }
}
