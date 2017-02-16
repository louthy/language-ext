using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LanguageExt.Tests
{
    public class ReflectTests
    {
        public class TestClass
        {
            public readonly string W;
            public readonly string X;
            public readonly string Y;
            public readonly string Z;

            public TestClass()
            {
            }

            public TestClass(string w)
            {
                W = w;
            }

            public TestClass(string w, string x)
            {
                W = w;
                X = x;
            }

            public TestClass(string w, bool x)
            {
                W = w;
                X = x.ToString();
            }

            public TestClass(string w, string x, string y)
            {
                W = w;
                X = x;
                Y = y;
            }

            public TestClass(string w, string x, string y, string z)
            {
                W = w;
                X = x;
                Y = y;
                Z = z;
            }
        }

        [Fact]
        public void CtorOfArity1Test()
        {
            var ctor = Reflect.Util.CtorInvoke<string, TestClass>();

            var res = ctor("Hello");

            Assert.True(res.W == "Hello");
        }

        [Fact]
        public void CtorOfArity2Test()
        {
            var ctor = Reflect.Util.CtorInvoke<string, string, TestClass>();

            var res = ctor("Hello", "World");

            Assert.True(res.W == "Hello");
            Assert.True(res.X == "World");
        }

        [Fact]
        public void CtorOfArity2Test2()
        {
            var ctor = Reflect.Util.CtorInvoke<string, bool, TestClass>();

            var res = ctor("Hello", true);

            Assert.True(res.W == "Hello");
            Assert.True(res.X == "True");
        }

        [Fact]
        public void CtorOfArity3Test()
        {
            var ctor = Reflect.Util.CtorInvoke<string, string, string, TestClass>();

            var res = ctor("Roland","TR", "909");

            Assert.True(res.W == "Roland");
            Assert.True(res.X == "TR");
            Assert.True(res.Y == "909");
        }

        [Fact]
        public void CtorOfArity4Test()
        {
            var ctor = Reflect.Util.CtorInvoke<string, string, string, string, TestClass>();

            var res = ctor("Chandler", "Curve", "Bender", "EQ");

            Assert.True(res.W == "Chandler");
            Assert.True(res.X == "Curve");
            Assert.True(res.Y == "Bender");
            Assert.True(res.Z == "EQ");
        }
    }
}
