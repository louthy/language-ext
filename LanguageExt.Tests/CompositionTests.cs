using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class CompositionTests
    {
        private readonly Func<string> f;
        private readonly Func<string, string> g;
        private readonly Func<string, int> h;

        public CompositionTests()
        {
            f = () => "Bob";
            g = (string name) => $"Hello, {name}";
            h = (string s) => s.Length;
        }

        [Fact]
        public void Sanity()
        {
            string expected = "Hello, Bob";
            Assert.Equal(g(f()), expected);
            Assert.Equal(h(g(f())), expected.Length);
        }

        [Fact]
        public void BackComposeFuncWithNoArgFunc()
        {
            Assert.Equal(g.BackCompose(f)(), g(f()));
        }

        [Fact]
        public void ComposeNoArgFuncWithFunc()
        {
            Assert.Equal(f.Compose(g)(), g(f()));
        }

        [Fact]
        public void BackComposeActionWithNoArgFunc()
        {
            string result;
            var g = act((string name) => { result = this.g(name); });

            result = null;
            g.BackCompose(f)();
            Assert.Equal(result, this.g(f()));
        }

        [Fact]
        public void ComposeNoArgFuncWithAction()
        {
            string result;
            var g = act((string name) => { result = this.g(name); });

            result = null;
            f.Compose(g)();
            Assert.Equal(result, this.g(f()));
        }

        [Fact]
        public void BackComposeFuncWithFunc()
        {
            Assert.Equal(h.BackCompose(g)(f()), h(g(f())));
        }

        [Fact]
        public void ComposeFuncWithFunc()
        {
            Assert.Equal(g.Compose(h)(f()), h(g(f())));
        }

        [Fact]
        public void BackComposeActionWithFunc()
        {
            int? result;
            var h = act((string s) => { result = this.h(s); });

            result = null;
            h.BackCompose(g)(f());
            Assert.Equal(result, this.h(g(f())));
        }

        [Fact]
        public void ComposeFuncWithAction()
        {
            int? result;
            var h = act((string s) => { result = this.h(s); });

            result = null;
            g.Compose(h)(f());
            Assert.Equal(result, this.h(g(f())));
        }
    }
}
