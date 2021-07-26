using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// The pipes library decouples stream processing stages from each other so
    /// that you can mix and match diverse stages to produce useful streaming
    /// programs.  If you are a library writer, pipes lets you package up streaming
    /// components into a reusable interface.  If you are an application writer,
    /// pipes lets you connect pre-made streaming components with minimal effort to
    /// produce a highly-efficient program that streams data in constant memory.
    ///
    /// To enforce loose coupling, components can only communicate using two commands:
    /// 
    /// * `yield`: Send output data
    /// * `await`: Receive input data
    ///
    /// Pipes has four types of components built around these two commands:
    /// 
    /// * `Producer` can only `yield` values and they model streaming sources
    /// * `Consumer` can only `await` values and they model streaming sinks
    /// * `Pipe` can both `yield` and `await` values and they model stream transformations
    /// * `Effect` can neither `yield` nor `await` and they model non-streaming components
    /// 
    /// Pipes uses parametric polymorphism (i.e. generics) to overload all operations.
    ///
    /// You've probably noticed this overloading already:
    /// 
    ///  * `yield` works within both `Producer` and a `Pipe`
    ///  * `await` works within both `Consumer` and `Pipe`
    ///  * `|` connects `Producer`, `Consumer`, and `Pipe` in varying ways
    /// 
    /// This overloading is great when it works, but when connections fail they
    /// produce type errors that appear intimidating at first.  This section
    /// explains the underlying types so that you can work through type errors
    /// intelligently.
    /// 
    /// `Producer`, `Consumer`, `Pipe`, and `Effect` are all special cases of a
    /// single underlying type: `Proxy`.  This overarching type permits fully
    /// bidirectional communication on both an upstream and downstream interface.
    /// 
    /// You can think of it as having the following shape:
    /// 
    ///      Proxy a' a b' b m r
    ///
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        a' ◄--       ◄-- b'  -- Information flowing upstream
    ///            |         |
    ///        a  --►       --► b   -- Information flowing downstream
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  The four core types do not use the upstream flow of information.  This means
    ///  that the `a'` and `b'` in the above diagram go unused unless you use the
    ///  more advanced features.
    /// 
    ///  Pipes uses type synonyms to hide unused inputs or outputs and clean up
    ///  type signatures.  These type synonyms come in two flavors:
    /// 
    ///  * Concrete type synonyms that explicitly close unused inputs and outputs of
    ///    the 'Proxy' type
    /// 
    ///  * Polymorphic type synonyms that don't explicitly close unused inputs or
    ///    outputs
    /// 
    ///  The concrete type synonyms use `()` to close unused inputs and `Void` (the
    ///  uninhabited type) to close unused outputs:
    /// 
    ///  * `Effect`: explicitly closes both ends, forbidding `await` and `yield`
    /// 
    ///      type Effect = Proxy X () () X
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///      Void ◄--       ◄-- ()
    ///            |         |
    ///        () --►       --► Void
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  * `Producer`: explicitly closes the upstream end, forbidding `await`
    /// 
    ///      type Producer b = Proxy X () () b
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///      Void ◄--       ◄-- ()
    ///            |         |
    ///        () --►       --► b
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  * `Consumer`: explicitly closes the downstream end, forbidding `yield`
    /// 
    ///      type Consumer a = Proxy () a () X
    ///     
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        () ◄--       ◄-- ()
    ///            |         |
    ///        a  --►       --► Void
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    ///  Pipe: marks both ends open, allowing both `await` and `yield`
    /// 
    ///        type Pipe a b = Proxy () a () b
    ///       
    ///        Upstream | Downstream
    ///            +---------+
    ///            |         |
    ///        () ◄--       ◄-- ()
    ///            |         |
    ///        a  --►       --► b
    ///            |    |    |
    ///            +----|----+
    ///                 v
    ///                 r
    /// 
    /// When you compose `Proxy` using `|` all you are doing is placing them
    /// side by side and fusing them laterally.  For example, when you compose a
    /// `Producer`, `Pipe`, and a `Consumer`, you can think of information flowing
    /// like this:
    /// 
    ///             Producer                Pipe                 Consumer
    ///          +-----------+          +----------+          +------------+
    ///          |           |          |          |          |            |
    ///    Void ◄--         ◄--   ()   ◄--        ◄--   ()   ◄--          ◄-- ()
    ///          |  stdinLn  |          |  take 3  |          |  stdoutLn  |
    ///      () --►         --► String --►        --► String --►          --► Void
    ///          |     |     |          |    |     |          |      |     |
    ///          +-----|-----+          +----|-----+          +------|-----+
    ///                v                     v                       v
    ///                ()                    ()                      ()
    /// 
    ///  Composition fuses away the intermediate interfaces, leaving behind an `Effect`:
    /// 
    ///                         Effect
    ///          +-----------------------------------+
    ///          |                                   |
    ///    Void ◄--                                 ◄-- ()
    ///          |  stdinLn >-> take 3 >-> stdoutLn  |
    ///      () --►                                 --► Void
    ///          |                                   |
    ///          +----------------|------------------+
    ///                           v
    ///                           () 
    /// </summary>
    public abstract class Proxy<RT, UOut, UIn, DIn, DOut, A>  where RT : struct, HasCancel<RT>
    {
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy();
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f);
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f);
        public abstract Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f);
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r);
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs);
        public abstract Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs);
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs);
        public abstract Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT,  UOut, UIn, DInC, DOutC, DIn>> rhs);
        public abstract Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect();
        public abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Observe();
    }
    
    public partial class Pure<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly A Value;

        public Pure(A value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            f(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Pure<RT, UOut, UIn, DIn, DOut, B>(f(Value));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Pure<RT, UOut, UIn, C1, C, A>(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> r) =>
            r;

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Pure<RT, UOutA, AUInA, DIn, DOut, A>(Value);
                
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Pure<RT, UOut, UIn, DInC, DOutC, A>(Value);

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Pure<RT, DOut, DIn, UIn, UOut, A>(Value);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        public void Deconstruct(out A value) =>
            value = Value;
    }

    public partial class Repeat<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, UOut, UIn, DIn, DOut, A> Inner;

        public Repeat(Proxy<RT, UOut, UIn, DIn, DOut, A> inner) =>
            Inner = inner;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Repeat<RT, UOut, UIn, C1, C, A>(Inner.For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Repeat<RT, UOutA, AUInA, DIn, DOut, A>(Inner.ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Repeat<RT, UOutA, AUInA, DIn, DOut, A>(Inner.ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Repeat<RT, UOut, UIn, DInC, DOutC, A>(Inner.ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Repeat<RT, UOut, UIn, DInC, DOutC, A>(Inner.ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Repeat<RT, DOut, DIn, UIn, UOut, A>(Inner.Reflect());
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        public void Deconstruct(out Proxy<RT, UOut, UIn, DIn, DOut, A> inner) =>
            inner = Inner;
    }
    
    public abstract class Use<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps);
    }

    public partial class Use<RT, UOut, UIn, DIn, DOut, X, A> : Use<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly Func<Aff<RT, X>> Acquire;
        public readonly Func<X, Unit> Release; 
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        public Use(Func<Aff<RT, X>> acquire, Func<X, Unit> release, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Acquire, Release, Next) = (acquire, release, next);
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Map(f));
        
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Use<RT, UOut, UIn, C1, C, X, A>(Acquire, Release, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> rhs) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Action(rhs));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs) =>
            new Use<RT, UOutA, AUInA, DIn, DOut, X, A>(Acquire, Release, x => Next(x).ComposeRight(lhs));
        
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Use<RT, UOutA, AUInA, DIn, DOut, X, A>(Acquire, Release, x => Next(x).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Use<RT, UOut, UIn, DInC, DOutC, X, A>(Acquire, Release, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Use<RT, UOut, UIn, DInC, DOutC, X, A>(Acquire, Release, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Use<RT, DOut, DIn, UIn, UOut, X, A>(Acquire, Release, x => Next(x).Reflect());
             
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new Use<RT, UOut, UIn, DIn, DOut, X, A>(Acquire, Release, x => Next(x).Observe());

        [Pure]
        public void Deconstruct(out Func<Aff<RT, X>> acquire, out Func<X, Unit> release, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (acquire, release, next) = (Acquire, Release, Next);

        [Pure]
        internal override Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps) =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Acquire().Map(x =>
                              {
                                  disps.TryAdd(x, new Disp(() => Release(x)));
                                  return x;
                              }).Map(Next));
    }    
        
    internal class Disp : IDisposable
    {
        readonly Action dispose;
        int disposed = 0;

        public Disp(Action dispose) =>
            this.dispose = dispose; 
            
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                dispose?.Invoke();
            }
        }
    }
    
    public abstract class Release<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps);
    }

    public partial class Release<RT, UOut, UIn, DIn, DOut, X, A> : Release<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly X Value;
        public readonly Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        public Release(X value, Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Value, Next) = (value, next);
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Map(f));
        
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Release<RT, UOut, UIn, C1, C, X, A>(Value, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> rhs) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Action(rhs));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs) =>
            new Release<RT, UOutA, AUInA, DIn, DOut, X, A>(Value, x => Next(x).ComposeRight(lhs));
        
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Release<RT, UOutA, AUInA, DIn, DOut, X, A>(Value, x => Next(x).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Release<RT, UOut, UIn, DInC, DOutC, X, A>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Release<RT, UOut, UIn, DInC, DOutC, X, A>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Release<RT, DOut, DIn, UIn, UOut, X, A>(Value, x => Next(x).Reflect());
             
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new Release<RT, UOut, UIn, DIn, DOut, X, A>(Value, x => Next(x).Observe());

        [Pure]
        public void Deconstruct(out X value, out Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (value, next) = (Value, Next);

        [Pure]
        internal override Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps) =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Prelude.Eff<RT, Unit>(_ =>
                                      {
                                          if (disps.TryRemove(Value, out var d))
                                          {
                                              d.Dispose();
                                          }
                                          return Prelude.unit;
                                      }).Map(Next));
    }    

    public abstract class Enumerate<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract EnumerateDataType Type { get; }
        internal abstract IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffects();
        internal abstract IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffectsAsync();
        internal abstract IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, A>> onNext, 
            Action<Error> onError = null, 
            Action onCompleted = null);
    }

    public partial class Enumerate<RT, UOut, UIn, DIn, DOut, X, A> : Enumerate<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal readonly EnumerateData<X> Items;
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        internal Enumerate(EnumerateData<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (items, next);

        public Enumerate(IEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateEnumerable<X>(items), next);

        public Enumerate(IAsyncEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateAsyncEnumerable<X>(items), next);

        public Enumerate(IObservable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateObservable<X>(items), next);

        [Pure]
        internal override EnumerateDataType Type =>
            Items.Type;
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Enumerate<RT, UOut, UIn, C1, C, X, A>(Items, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(Items, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(Items, c1 => Next(c1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Enumerate<RT, DOut, DIn, UIn, UOut, X, A>(Items, x => Next(x).Reflect());
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        internal void Deconstruct(out EnumerateData<X> items, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (items, next) = (Items, Next);

        [Pure]
        internal override IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffects()
        {
            if (Type == EnumerateDataType.Enumerable)
            {
                foreach (var item in ((EnumerateEnumerable<X>)Items).Values)
                {
                    yield return Next(item);
                }
            }
        }

        [Pure]
        internal override async IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffectsAsync()
        {
            if (Type == EnumerateDataType.AsyncEnumerable)
            {
                await foreach (var item in ((EnumerateAsyncEnumerable<X>)Items).Values)
                {
                    yield return Next(item);
                }
            }
        }

        internal override IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, A>> onNext,
            Action<Error> onError = null,
            Action onCompleted = null) =>
            Type == EnumerateDataType.Observable
                ? ((EnumerateObservable<X>)Items).Values.Subscribe(new Observerable(onCompleted, onError, x => onNext(Next(x))))
                : null;

        class Observerable : IObserver<X>
        {
            readonly Action onCompleted;
            readonly Action<Error> onError;
            readonly Action<X> onNext;
            
            public Observerable(Action onCompleted, Action<Error> onError, Action<X> onNext)
            {
                this.onCompleted = onCompleted;
                this.onError     = onError;
                this.onNext      = onNext;
            }

            public void OnCompleted() =>
                onCompleted?.Invoke();

            public void OnError(Exception error) =>
                onError?.Invoke(error);

            public void OnNext(X value) =>
                onNext?.Invoke(value);
        }
    }

    public partial class Request<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly UOut Value;
        public readonly Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;
        
        public Request(UOut value, Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (Value, Next) = (value, fun);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public void Deconstruct(out UOut value, out Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (value, fun) = (Value, Next);

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Request<RT, UOut, UIn, C1, C, A>(Value, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            fb1(Value).ComposeLeft(Next);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            lhs(Value).Bind(b => Next(b).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, A>(Value, a => Next(a).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, A>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Respond<RT, DOut, DIn, UIn, UOut, A>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(new Request<RT, UOut, UIn, DIn, DOut, A>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
                );
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));
    }

    public partial class Respond<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly DOut Value;
        public readonly Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;
        
        public Respond(DOut value, Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (Value, Next) = (value, fun);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> fb) =>
            fb(Value).Bind(b1 => Next(b1).For(fb));
            
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, A>(Value, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, A>(Value, x1 => Next(x1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            rhs(Value).ComposeRight(Next);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            rhs(Value).Bind(b1 => Next(b1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Request<RT, DOut, DIn, UIn, UOut, A>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(new Respond<RT, UOut, UIn, DIn, DOut, A>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
            );

        [Pure]
        public void Deconstruct(out DOut value, out Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (value, fun) = (Value, Next);
    }

    public partial class M<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> Value;
        
        public M(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Bind(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Map(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new M<RT, UOut, UIn, C1, C, A>(Value.Map(mx => mx.For(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Action(r)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(p1 => p1.ComposeRight(fb1)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new M<RT, UOutA, AUInA, DIn, DOut, A>(Value.Map(x => x.ComposeRight(lhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(p1 => p1.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, A>(Value.Map(x => x.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new M<RT, DOut, DIn, UIn, UOut, A>(Value.Map(x => x.Reflect()));
         
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Value.Bind(x => ((M<RT, UOut, UIn, DIn, DOut, A>)x.Observe()).Value));
        
        [Pure]
        public void Deconstruct(out Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>> value) =>
            value = Value;
    }
    
    /// <summary>
    /// Producers are effectful streams of input.  Specifically, a `Producer` is a
    /// monad transformer that extends the base IO monad with a new `yield` command.
    /// This `yield` command lets you send output downstream to an anonymous handler,
    /// decoupling how you generate values from how you consume them.
    /// </summary>
    public partial class Producer<RT, OUT, A> : Proxy<RT, Void, Unit, Unit, OUT, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, OUT, A> Value;
        
        public Producer(Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, OUT, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Void, Unit, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Action<S>(Proxy<RT, Void, Unit, Unit, OUT, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, OUT, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, A> Observe() =>
            Value.Observe();

        [Pure]
        public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<RT, OUT, A> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Effect<RT, A> operator |(Producer<RT, OUT, A> p1, Consumer<OUT, A> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, OUT, A> value) =>
            value = Value;

        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Producer<OUT, A> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Producer<RT, OUT, A>(Pure<A> p) =>
            Producer.Pure<RT, OUT, A>(p.Value);

        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretProducer<RT, OUT>()).ToProducer();

        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Producer<RT, OUT, B> SelectMany<B>(Func<A, Producer<OUT, B>> bind) =>
            Value.Bind(a => bind(a).Interpret<RT>()).ToProducer();

        [Pure]
        public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<OUT, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Consumers only await
    /// </summary>
    public partial class Consumer<RT, IN, A> : Proxy<RT, Unit, IN, Unit, Void, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, Void, A> Value;
        
        public Consumer(Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, Void, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Unit, IN, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Action<S>(Proxy<RT, Unit, IN, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, Void, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);
        
        [Pure]
        public override Proxy<RT, Void, Unit, IN, Unit, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, Void, A> value) =>
            value = Value;

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Consumer<IN, A> c) =>
            c.Interpret<RT>();

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(ConsumerLift<RT, IN, A> c) =>
            c.Interpret();

        [Pure]
        public static implicit operator Consumer<RT, IN, A>(Pure<A> p) =>
            Consumer.Pure<RT, IN, A>(p.Value);

        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretConsumer<RT, IN>()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
        
        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, Consumer<IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret<RT>()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Consumer<RT, IN, B> SelectMany<B>(Func<A, ConsumerLift<RT, IN, B>> bind) =>
            Value.Bind(a =>  bind(a).Interpret()).ToConsumer();

        [Pure]
        public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, ConsumerLift<RT, IN, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Pipes both await and yield
    /// </summary>
    /// <remarks>
    ///     Upstream | Downstream
    ///         +---------+
    ///         |         |
    ///     () <==       <== ()
    ///         |         |
    ///     A  ==>       ==> B
    ///         |    |    |
    ///         +----|----+
    ///              R
    /// </remarks>
    public partial class Pipe<RT, IN, OUT, A> : Proxy<RT, Unit, IN, Unit, OUT, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, OUT, A> Value;
        
        public Pipe(Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Bind<S>(Func<A, Proxy<RT, Unit, IN, Unit, OUT, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, A> For<C1, C>(Func<OUT, Proxy<RT, Unit, IN, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Action<S>(Proxy<RT, Unit, IN, Unit, OUT, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, A> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, OUT, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, IN, Unit, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, OUT, A> value) =>
            value = Value;
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<RT, IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<IN, A> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, A> operator |(Pipe<RT, IN, OUT, A> p1, Consumer<RT, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, A> operator |(Producer<OUT, IN> p1, Pipe<RT, IN, OUT, A> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public Pipe<RT, IN, C, A> Then<C>(Pipe<RT, OUT, C, A> p2) =>
            Proxy.compose(this, p2);

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pipe<IN, OUT, A> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, A>(Pure<A> p) =>
            Pipe.Pure<RT, IN, OUT, A>(p.Value);

        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretPipe<RT, IN, OUT>()).ToPipe();

        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));

        [Pure]
        public Pipe<RT, IN, OUT, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> bind) =>
            Value.Bind(a => bind(a).Interpret<RT>()).ToPipe();

        [Pure]
        public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Clients only request and never respond
    /// </summary>
    public partial class Client<RT, REQ, RES, A> : Proxy<RT, REQ, RES, Unit, Void, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, REQ, RES, Unit, Void, A> Value;

        public Client(Proxy<RT, REQ, RES, Unit, Void, A> value) =>
            Value = value;
 
        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, REQ, RES, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, REQ, RES, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, REQ, RES, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Action<S>(Proxy<RT, REQ, RES, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<REQ, Proxy<RT, UOutA, AUInA, REQ, RES, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<REQ, Proxy<RT, UOutA, AUInA, Unit, Void, RES>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, REQ, RES, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, REQ, RES, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, REQ, RES, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, RES, REQ, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, REQ, RES, Unit, Void, A> value) =>
            value = Value;

        [Pure]
        public static Effect<RT, A> operator |(Func<REQ, Server<RT, REQ, RES, A>> x, Client<RT, REQ, RES, A> y) =>
            y.ComposeRight(x).ToEffect();
        
        [Pure]
        public Client<RT, REQ, RES, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretClient<RT, REQ, RES>()).ToClient();

        [Pure]
        public Client<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Servers only respond and never request
    /// </summary>
    public partial class Server<RT, REQ, RES, A> : Proxy<RT, Void, Unit, REQ, RES, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, REQ, RES, A> Value;
    
        public Server(Proxy<RT, Void, Unit, REQ, RES, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, REQ, RES, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<RES, Proxy<RT, Void, Unit, C1, C, REQ>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Action<S>(Proxy<RT, Void, Unit, REQ, RES, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, REQ, RES, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, REQ, RES, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, REQ, RES, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<RES, Proxy<RT, REQ, RES, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<RES, Proxy<RT, Void, Unit, DInC, DOutC, REQ>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, RES, REQ, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, REQ, RES, A> value) =>
            value = Value;
        
        public Server<RT, REQ, RES, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretServer<RT, REQ, RES>()).ToServer();

        public Server<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type
    /// It neither yields nor awaits, but represents an entire effect system
    /// </summary>
    public partial class Effect<RT, A> : Proxy<RT, Void, Unit, Unit, Void, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, Void, A> Value;

        public Effect(Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> ToProxy() =>
            Value.ToProxy();
        [Pure]

        public override Proxy<RT, Void, Unit, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Void, Unit, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Action<S>(Proxy<RT, Void, Unit, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, Void, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            value = Value;
    }
}
