using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    public class CompositionTests
    {
        private readonly Func<string> _f;
        private readonly Func<string, string> _g;
        private readonly Func<string, int> _h;

        public CompositionTests()
        {
            _f = fun(() => "Bob");
            _g = fun((string name) => $"Hello, {name}");
            _h = (string s) => s.Length;
        }

        [Fact]
        public void Sanity()
        {
            string expected = "Hello, Bob";
            Assert.Equal(_g(_f()), expected);
            Assert.Equal(_h(_g(_f())), expected.Length);
        }

        [Fact]
        public void ComposeFuncWithNoArgFunc()
        {
            Assert.Equal(_g.Compose(_f)(), _g(_f()));
        }

        [Fact]
        public void BackComposeNoArgFuncWithFunc()
        {
            Assert.Equal(_f.BackCompose(_g)(), _g(_f()));
        }

        [Fact]
        public void ComposeActionWithNoArgFunc()
        {
            string result;
            var g = act((string name) => { result = _g(name); });

            result = null;
            Assert.Equal(g.Compose(_f)(), Unit.Default);
            Assert.Equal(result, _g(_f()));
        }

        [Fact]
        public void BackComposeNoArgFuncWithAction()
        {
            string result;
            var g = act((string name) => { result = _g(name); });

            result = null;
            Assert.Equal(_f.BackCompose(g)(), Unit.Default);
            Assert.Equal(result, _g(_f()));
        }

        [Fact]
        public void ComposeFuncWithFunc()
        {
            Assert.Equal(_h.Compose(_g)(_f()), _h(_g(_f())));
        }

        [Fact]
        public void BackComposeFuncWithFunc()
        {
            Assert.Equal(_g.BackCompose(_h)(_f()), _h(_g(_f())));
        }

        [Fact]
        public void ComposeActionWithFunc()
        {
            int? result;
            var h = act((string s) => { result = _h(s); });

            result = null;
            Assert.Equal(h.Compose(_g)(_f()), Unit.Default);
            Assert.Equal(result, _h(_g(_f())));
        }

        [Fact]
        public void BackComposeFuncWithAction()
        {
            int? result;
            var h = act((string s) => { result = _h(s); });

            result = null;
            Assert.Equal(_g.BackCompose(h)(_f()), Unit.Default);
            Assert.Equal(result, _h(_g(_f())));
        }
    }
}
