using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExt.Tests
{
    public interface IMyProcess
    {
        void Hello(string msg);
        int World(string msg);
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
    }

    public class ProcessProxies
    {
        [Fact]
        public void ProxyTest1()
        {
            Process.shutdownAll();

            var pid = Process.spawn<MyProcess>("proxy-test1");

            var proxy = Process.proxy<IMyProcess>(pid);

            proxy.Hello("Hello");
            int res = proxy.World("World");

            Assert.True(res == 2);
        }

        [Fact]
        public void ProxyTest2()
        {
            Process.shutdownAll();

            var proxy = Process.spawn<IMyProcess>("proxy-test2", () => new MyProcess());

            proxy.Hello("Hello");
            int res = proxy.World("World");

            Assert.True(res == 2);
        }

    }
}
