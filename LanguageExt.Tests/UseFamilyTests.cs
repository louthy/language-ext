using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
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
            
        #region Disposable Constructor Exception Tests
        [Fact]
        public void tryuseFuncMap_TryException_IsFaulted()
        {
            Func<IDisposable> m = () => throw new Exception();
            Func<IDisposable, Unit> f = _ => unit;

            var actual = tryuse(m, f)();

            Assert.True(actual.IsFaulted);
        }

        [Fact]
        public void useTryMap_TryException_IsFaulted()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            Func<IDisposable, Unit> f = _ => unit;

            var actual = use(m, f)();

            Assert.True(actual.IsFaulted);
        }

        [Fact]
        public void useTryBind_TryException_IsFaulted()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            var actual = use(m, f)();

            Assert.True(actual.IsFaulted);
        }

        [Fact]
        public void UseTryMap_TryException_IsFaulted()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            Func<IDisposable, Unit> f = _ => unit;

            var actual = m.Use(f)();

            Assert.True(actual.IsFaulted);
        }

        [Fact]
        public void UseTryBind_TryException_IsFaulted()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            Func<IDisposable, Try<Unit>> f = _ => Try(unit);

            var actual = m.Use(f)();

            Assert.True(actual.IsFaulted);
        }

        [Fact]
        public void tryuseFuncMap_TryException_SelectNotInvoked()
        {
            Func<IDisposable> m =() => throw new Exception();
            var selectInvoked = false;
            Func<IDisposable, Unit> f = _ =>
            {
                selectInvoked = true;
                return unit;
            };

            tryuse(m, f)();

            Assert.False(selectInvoked);
        }

        [Fact]
        public void useTryMap_TryException_SelectNotInvoked()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            var selectInvoked = false;
            Func<IDisposable, Unit> f = _ =>
            {
                selectInvoked = true;
                return unit;
            };

            use(m, f)();

            Assert.False(selectInvoked);
        }

        [Fact]
        public void useTryBind_TryException_SelectNotInvoked()
        {
            var m = Try<IDisposable>(() => throw new Exception());
            var selectInvoked = false;
            Func<IDisposable, Try<Unit>> f = _ =>
            {
                selectInvoked = true;
                return Try(unit);
            };

            use(m, f)();

            Assert.False(selectInvoked);
        }

        [Fact]
        public void UseTryMap_TryException_SelectNotInvoked()
        {
            var m = Try<IDisposable>(() => throw new Exception());

            var selectInvoked = false;
            Func<IDisposable, Unit> f = _ =>
            {
                selectInvoked = true;
                return unit;
            };

            m.Use(f)();

            Assert.False(selectInvoked);
        }

        [Fact]
        public void UseTryBind_TryException_SelectNotInvoked()
        {
            var m = Try<IDisposable>(() => throw new Exception());

            var selectInvoked = false;
            Func<IDisposable, Try<Unit>> f = _ =>
            {
                selectInvoked = true;
                return Try(unit);
            };

            m.Use(f)();

            Assert.False(selectInvoked);
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

        #region Disposable not Disposed too Early Tests
        [Fact]
        public async Task useDisposableMapToTask_CheckForEarlyDisposeCall_DisposeNotCalled()
        {
            var d = new FakeDisposable();
            Func<FakeDisposable, Task<bool>> f = async fd => {
                await Task.Yield();
                return fd.DisposedCalled;
            };

            var disposeAlreadyCalled = await use(d, f);

            Assert.False(disposeAlreadyCalled);
        }

        [Fact]
        public async Task useFuncMapToTask_CheckForEarlyDisposeCall_DisposeNotCalled()
        {
            Func<FakeDisposable> m = () => new FakeDisposable();
            Func<FakeDisposable, Task<bool>> f = async fd => {
                await Task.Yield();
                return fd.DisposedCalled;
            };

            var disposeAlreadyCalled = await use(m, f);

            Assert.False(disposeAlreadyCalled);
        }
        #endregion

        private class FakeDisposable : IDisposable
        {
            public bool DisposedCalled { get; private set; }
            public string Label { get; }

            public FakeDisposable(string label = "") => Label = label;
            public void Dispose() =>
                DisposedCalled = DisposedCalled ? failwith<bool>($"Can't dispose {Label}; already disposed") : true;
            public string UseDisposable() =>
                DisposedCalled ? failwith<string>($"Can't use {Label}; already disposed") : $"Now it's {DateTime.Now}";
            public IEnumerable<string> Read()
            {
                yield return UseDisposable();
            }
        }

        [Fact]
        public void ResourceTryWorks()
        {
            Use(() => new FakeDisposable())
                .Select(r => r.Read())
                .Select(s => s.First())
                .ToTry()
                .IfFailThrow();
        }

        [Fact]
        public void ResourceWorksWithDeferred()
        {
            Lst<FakeDisposable> disposables;
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            var read = Use(getFake).Select(d => d.Read());
            var getFirst = from r in read select r.First();
            
            var first = getFirst.Run();
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceComposes()
        {
            Lst<FakeDisposable> disposables;
            var fake2 = new FakeDisposable("2");
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            FakeDisposable getFake2()
            {
                disposables = disposables.Add(fake2);
                return fake2;
            }

            var reader =
                from a in Use(getFake2)
                select a.Read();
            var getFirst =
                from a in reader
                from b in Use(getFake)
                let combined = a.First() + b.Read().First()
                from _ in release(b)
                select $"{fake2.DisposedCalled}|{b.Label}";
            var str = getFirst.Run();
            
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceTransforms()
        {
            var resources = List(Use(() => new FakeDisposable("1")), Use(() => new FakeDisposable("2")));
            var resource = resources.Traverse(r => r.Read().First());

            resource.ToTry().IfFailThrow();
        }

        [Fact]
        public void ResourceMapDisposesAll()
        {
            Lst<FakeDisposable> disposables;
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }

            var map =
                from a in Use(getFake)
                select 1;
            
            Assert.False(disposables.Any());
            map.Run();
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAll()
        {
            Lst<FakeDisposable> disposables;
            var fake2 = new FakeDisposable("2");
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            FakeDisposable getFake2()
            {
                disposables = disposables.Add(fake2);
                return fake2;
            }

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select 1;
            
            Assert.False(disposables.Any());
            useBoth.Run();
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceMapDisposesAllWhenProjectionFails()
        {
            Lst<FakeDisposable> disposables;
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }

            var map =
                from a in Use(getFake)
                select failwith<int>("I was setup to fail");
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => map.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAllWhenProjectionFails()
        {
            Lst<FakeDisposable> disposables;
            var fake2 = new FakeDisposable("2");
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            FakeDisposable getFake2()
            {
                disposables = disposables.Add(fake2);
                return fake2;
            }

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select failwith<int>("I was setup to fail");
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => useBoth.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAllWhenFirstSourceFails()
        {
            Lst<FakeDisposable> disposables;
            var fake2 = new FakeDisposable("2");
            FakeDisposable getFake() => throw new Exception("I was setup to fail");
            FakeDisposable getFake2()
            {
                disposables = disposables.Add(fake2);
                return fake2;
            }

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select 1;
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => useBoth.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAllWhenSecondSourceFails()
        {
            Lst<FakeDisposable> disposables;
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            FakeDisposable getFake2() => throw new Exception("I was setup to fail");

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select 1;
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => useBoth.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAllWhenFirstIsNull()
        {
            Lst<FakeDisposable> disposables;
            var fake2 = new FakeDisposable("2");
            FakeDisposable getFake() => null;
            FakeDisposable getFake2()
            {
                disposables = disposables.Add(fake2);
                return fake2;
            }

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select 1;
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => useBoth.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

        [Fact]
        public void ResourceBindDisposesAllWhenSecondIsNull()
        {
            Lst<FakeDisposable> disposables;
            FakeDisposable getFake()
            {
                var d = new FakeDisposable("1");
                disposables = disposables.Add(d);
                return d;
            }
            FakeDisposable getFake2() => null;

            var useBoth =
                from a in Use(getFake)
                from b in Use(getFake2)
                select 1;
            
            Assert.False(disposables.Any());
            Assert.ThrowsAny<Exception>(() => useBoth.Run());
            Assert.True(disposables.All(d => d.DisposedCalled));
        }

    }
}