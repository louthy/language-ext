using LanguageExtTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Process;

namespace LanguageExt.Tests
{
    public interface IMyProcess
    {
        void Hello(string msg);
        int World(string msg);
        Return<int> NoReply(string msg);
        Return<int> NoReply2(string msg);
        Return<int> MsgLength(string msg);
        Option<int> TestOption(Option<int> msg);
    }

    public class MyProcess : IMyProcess
    {
        List<string> state = new List<string>();

        public void Hello(string msg)
        {
            state.Add(msg);
        }

        public int World(string msg)
        {
            state.Add(msg);
            return state.Count;
        }

        public Return<int> NoReply(string msg)
        {
            return noreply;
        }

        public Return<int> NoReply2(string msg)
        {
            return null;
        }

        public Return<int> MsgLength(string msg)
        {
            if (msg.Length == 0)
                return noreply;
            else
                return msg.Length;
        }

        public Option<int> TestOption(Option<int> value) =>
            value;
    }

    public class ProcessProxies
    {
        [Fact]
        public void ProxyTest1()
        {
            lock (ProcessTests.sync)
            {
                shutdownAll();
                ProcessConfig.initialise();

                var pid = spawn<MyProcess>("proxy-test1");

                var proxy = proxy<IMyProcess>(pid);

                proxy.Hello("Hello");
                int res = proxy.World("World");

                Assert.True(res == 2);
            }
        }

        [Fact]
        public void ProxyTest2()
        {
            lock (ProcessTests.sync)
            {
                shutdownAll();
                ProcessConfig.initialise();

                var proxy = spawn<IMyProcess>("proxy-test2", () => new MyProcess());

                proxy.Hello("Hello");
                int res = proxy.World("World");

                Assert.True(res == 2);
            }
        }

        [Fact]
        public void ProxyTest3()
        {
            lock (ProcessTests.sync)
            {
                shutdownAll();
                ProcessConfig.initialise();

                var proxy = spawn<IMyProcess>("proxy-test3", () => new MyProcess());

                Assert.True(proxy.MsgLength("Hello") == 5);
            }
        }

        [Fact]
        public void ProxyOptionTest1()
        {
            lock (ProcessTests.sync)
            {
                shutdownAll();
                ProcessConfig.initialise();

                var proxy = spawn<IMyProcess>("proxy-test4", () => new MyProcess());

                Assert.True(proxy.TestOption(123) == 123);
            }
        }
    }
}
