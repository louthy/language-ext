﻿using System;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class UseFamilyTests
    {
        #region Null Disposable Tests
        [Fact]
        public void useTryMap_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            var m = Try(d);
            Func<IDisposable, Unit> f = _ => unit;

            var actual = use(m, f)();

            Assert.False(actual.IsFaulted);
        }

        [Fact]
        public void useTryBind_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            var m = Try(d);
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            var actual = use(m, f)();

            Assert.False(actual.IsFaulted);
        }

        [Fact]
        public async Task useTaskMap_NullDisposable_NoExceptionThrown()
        {
            IDisposable d = null;
            var m = d.AsTask();
            Func<IDisposable, Unit> f = _ => unit;

            await use(m, f);
        }

        [Fact]
        public async Task useTaskBind_NullDisposable_NoExceptionThrown()
        {
            IDisposable d = null;
            var m = d.AsTask();
            Func<IDisposable, Task<Unit>> f = _ => unit.AsTask();

            await use(m, f);
        }

        [Fact]
        public void useFuncMap_NullDisposable_NoExceptionThrown()
        {
            IDisposable d = null;
            Func<IDisposable> m = () => d;
            Func<IDisposable, Unit> f = _ => unit;

            use(m, f);
        }

        [Fact]
        public void useDisposableMap_NullDisposable_NoExceptionThrown()
        {
            IDisposable d = null;
            Func<IDisposable, Unit> f = _ => unit;

            use(d, f);
        }

        [Fact]
        public void tryuseFuncMap_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            Func<IDisposable> m = () => d;
            Func<IDisposable, Unit> f = _ => unit;

            var actual = tryuse(m, f)();

            Assert.False(actual.IsFaulted);
        }

        [Fact]
        public void tryuseDisposableMap_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            Func<IDisposable, Unit> f = _ => unit;

            var actual = tryuse(d, f)();

            Assert.False(actual.IsFaulted);
        }

        [Fact]
        public void UseTryMap_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            var m = Try(d);
            Func<IDisposable, Unit> f = _ => unit;

            var actual = m.Use(f)();

            Assert.False(actual.IsFaulted);
        }

        [Fact]
        public void UseTryBind_NullDisposable_IsNotFaulted()
        {
            IDisposable d = null;
            var m = Try(d);
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            var actual = m.Use(f)();

            Assert.False(actual.IsFaulted);
        }
        #endregion

        #region Disposable Disposed Tests
        [Fact]
        public void useTryMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = Try(d);
            Func<IDisposable, Unit> f = _ => unit;

            use(m, f)();

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void useTryBind_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = Try(d);
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            var actual = use(m, f)();

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public async Task useTaskMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = identity<IDisposable>(d).AsTask();
            Func<IDisposable, Unit> f = _ => unit;

            await use(m, f);

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public async Task useTaskBind_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = identity<IDisposable>(d).AsTask();
            Func<IDisposable, Task<Unit>> f = _ => unit.AsTask();

            await use(m, f);

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void useFuncMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            Func<IDisposable> m = () => d;
            Func<IDisposable, Unit> f = _ => unit;

            use(m, f);

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void useDisposableMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            Func<IDisposable, Unit> f = _ => unit;

            use(d, f);

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void tryuseFuncMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            Func<IDisposable> m = () => d;
            Func<IDisposable, Unit> f = _ => unit;

            tryuse(m, f)();

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void tryuseDisposableMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            Func<IDisposable, Unit> f = _ => unit;

            tryuse(d, f)();

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void UseTryMap_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = Try(d);
            Func<IDisposable, Unit> f = _ => unit;

            m.Use(f)();

            Assert.True(d.DisposedCalled);
        }

        [Fact]
        public void UseTryBind_MockDisposable_DisposeCalled()
        {
            var d = new FakeDisposable();
            var m = Try(d);
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            m.Use(f)();

            Assert.True(d.DisposedCalled);
        }
        #endregion

        private class FakeDisposable : IDisposable
        {
            public bool DisposedCalled { get; private set; }

            public void Dispose() => DisposedCalled = true;
        }

    }
}
