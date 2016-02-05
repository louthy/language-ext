using LanguageExt;
using LanguageExt.Trans;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace LanguageExtTests
{
    public class BaseMsg
    {
        public string Test;
    }

    public class SubMsg : BaseMsg
    {
        public string SubTest;
    }

    public interface ITestProcess
    {
    }

    public class TestProcess : ITestProcess
    {
    }

    public class TestProcess2
    {
    }

    public class ProcessTypeChecks
    {
        [Fact]
        public void MessageTypeTest1()
        {
            Assert.True(TypeHelper.IsMessageValidForProcess(typeof(SubMsg), new[] { typeof(BaseMsg) }).IsRight);
        }

        [Fact]
        public void MessageTypeTest2()
        {
            Assert.True(TypeHelper.IsMessageValidForProcess(new SubMsg(), new[] { typeof(BaseMsg) }).IsRight);
        }

        [Fact]
        public void MessageTypeTest3()
        {
            Assert.False(TypeHelper.IsMessageValidForProcess(new BaseMsg(), new[] { typeof(SubMsg) }).IsRight);
        }

        [Fact]
        public void MessageTypeTest4()
        {
            var msg = new SubMsg { Test = "Hello", SubTest = "World" };
            string json = JsonConvert.SerializeObject(msg);

            var res = TypeHelper.IsMessageValidForProcess(json, new[] { typeof(BaseMsg) }).LiftUnsafe();

            Assert.True(res is BaseMsg);
            Assert.False(res is SubMsg);
        }

        [Fact]
        public void StateTypeTest1()
        {
            Assert.True(TypeHelper.HasStateTypeOf(typeof(ITestProcess), typeof(TestProcess).GetTypeInfo().ImplementedInterfaces.ToArray()).IsRight);
        }

        [Fact]
        public void StateTypeTest2()
        {
            Assert.False(TypeHelper.HasStateTypeOf(typeof(TestProcess2), typeof(TestProcess).GetTypeInfo().ImplementedInterfaces.ToArray()).IsRight);
        }
    }
}
