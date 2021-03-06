using System;
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

    public partial class Repeat<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, UOut, UIn, DIn, DOut, R> Inner;

        public Repeat(Proxy<RT, UOut, UIn, DIn, DOut, R> inner) =>
            Inner = inner;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Repeat<RT, UOut, UIn, C1, C, R>(Inner.For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Repeat<RT, UOut, UIn, DIn, DOut, S>(Inner.Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            new Repeat<RT, UOutA, AUInA, DIn, DOut, R>(Inner.ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Repeat<RT, UOutA, AUInA, DIn, DOut, R>(Inner.ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            new Repeat<RT, UOut, UIn, DInC, DOutC, R>(Inner.ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Repeat<RT, UOut, UIn, DInC, DOutC, R>(Inner.ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new Repeat<RT, DOut, DIn, UIn, UOut, R>(Inner.Reflect());
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>>.Success(this));

        [Pure]
        public void Deconstruct(out Proxy<RT, UOut, UIn, DIn, DOut, R> inner) =>
            inner = Inner;
    }

    public abstract class Enumerate<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public abstract IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, R>> MakeEffects();
        public abstract IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, R>> MakeEffectsAsync();
        public abstract bool IsAsync { get; }
    }

    public partial class Enumerate<RT, UOut, UIn, DIn, DOut, X, R> : Enumerate<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public readonly IEnumerable<X> Items;
        public readonly IAsyncEnumerable<X> ItemsA;
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> Next;

        public Enumerate(IEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (Items, Next) = (items, next);

        public Enumerate(IAsyncEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (ItemsA, Next) = (items, next);

        internal Enumerate(IEnumerable<X> items, IAsyncEnumerable<X> itemsA, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (Items, ItemsA, Next) = (items, itemsA, next);

        [Pure]
        public override bool IsAsync =>
            Items == null;
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, ItemsA, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, ItemsA, x => Next(x).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Enumerate<RT, UOut, UIn, C1, C, X, R>(Items, ItemsA, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, ItemsA, x => Next(x).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, R>(Items, ItemsA, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, R>(Items, ItemsA, c1 => Next(c1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, R>(Items, ItemsA, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, R>(Items, ItemsA, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new Enumerate<RT, DOut, DIn, UIn, UOut, X, R>(Items, ItemsA, x => Next(x).Reflect());
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>>.Success(this));

        [Pure]
        public void Deconstruct(out IEnumerable<X> items, out IAsyncEnumerable<X> itemsA, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (items, itemsA, next) = (Items, ItemsA, Next);

        [Pure]
        public override IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, R>> MakeEffects()
        {
            foreach (var item in Items)
            {
                yield return Next(item);
            }
        }

        [Pure]
        public override async IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, R>> MakeEffectsAsync()
        {
            await foreach (var item in ItemsA)
            {
                yield return Next(item);
            }
        }
    }
    
    
    public abstract class Observer<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public abstract IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, R>> onNext, 
            Action<Error> onError = null, 
            Action onCompleted = null);
    }

    public partial class Observer<RT, UOut, UIn, DIn, DOut, X, R> : Observer<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public readonly IObservable<X> Items;
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> Next;

        public Observer(IObservable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (Items, Next) = (items, next);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Observer<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new Observer<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Observer<RT, UOut, UIn, C1, C, X, R>(Items, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Observer<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            new Observer<RT, UOutA, AUInA, DIn, DOut, X, R>(Items, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Observer<RT, UOutA, AUInA, DIn, DOut, X, R>(Items, c1 => Next(c1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            new Observer<RT, UOut, UIn, DInC, DOutC, X, R>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Observer<RT, UOut, UIn, DInC, DOutC, X, R>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new Observer<RT, DOut, DIn, UIn, UOut, X, R>(Items, n => Next(n).Reflect()); 

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>>.Success(this));

        [Pure]
        public void Deconstruct(out IObservable<X> items, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, R>> next) =>
            (items, next) = (Items, Next);

        [Pure]
        public override IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, R>> onNext, 
            Action<Error> onError = null, 
            Action onCompleted = null) =>
            Items.Subscribe(new Observerable(onCompleted, onError, x => onNext(Next(x))));

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

    public partial class Request<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public readonly UOut Value;
        public readonly Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> Next;
        
        public Request(UOut value, Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> fun) =>
            (Value, Next) = (value, fun);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public void Deconstruct(out UOut value, out Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> fun) =>
            (value, fun) = (Value, Next);

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new Request<RT, UOut, UIn, C1, C, R>(Value, x => Next(x).For(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            fb1(Value).ComposeLeft(Next);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            lhs(Value).Bind(b => Next(b).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, R>(Value, a => Next(a).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, R>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new Respond<RT, DOut, DIn, UIn, UOut, R>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>>.Success(new Request<RT, UOut, UIn, DIn, DOut, R>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
                );
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));
    }

    public partial class Respond<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R> where RT : struct, HasCancel<RT>
    {
        public readonly DOut Value;
        public readonly Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> Next;
        
        public Respond(DOut value, Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> fun) =>
            (Value, Next) = (value, fun);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> fb) =>
            fb(Value).Bind(b1 => Next(b1).For(fb));
            
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, R>(Value, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, R>(Value, x1 => Next(x1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            rhs(Value).ComposeRight(Next);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            rhs(Value).Bind(b1 => Next(b1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new Request<RT, DOut, DIn, UIn, UOut, R>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>>.Success(new Respond<RT, UOut, UIn, DIn, DOut, R>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
            );

        [Pure]
        public void Deconstruct(out DOut value, out Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, R>> fun) =>
            (value, fun) = (Value, Next);
    }

    public partial class M<RT, UOut, UIn, DIn, DOut, R> : Proxy<RT, UOut, UIn, DIn, DOut, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>> Value;
        
        public M(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Bind(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Map(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> f) =>
            new M<RT, UOut, UIn, C1, C, R>(Value.Map(mx => mx.For(f)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new M<RT, UOut, UIn, DIn, DOut, S>(Value.Map(mx => mx.Action(r)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> fb1) =>
            new M<RT, UOutA, AUInA, DIn, DOut, R>(Value.Map(p1 => p1.ComposeRight(fb1)));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new M<RT, UOutA, AUInA, DIn, DOut, R>(Value.Map(x => x.ComposeRight(lhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, R>(Value.Map(p1 => p1.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new M<RT, UOut, UIn, DInC, DOutC, R>(Value.Map(x => x.ComposeLeft(rhs)));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, R> Reflect() =>
            new M<RT, DOut, DIn, UIn, UOut, R>(Value.Map(x => x.Reflect()));
         
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, R> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, R>(
                Value.Bind(x => ((M<RT, UOut, UIn, DIn, DOut, R>)x.Observe()).Value));
        
        [Pure]
        public void Deconstruct(out Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, R>> value) =>
            value = Value;
    }
    
    /// <summary>
    /// Producers are effectful streams of input.  Specifically, a `Producer` is a
    /// monad transformer that extends the base IO monad with a new `yield` command.
    /// This `yield` command lets you send output downstream to an anonymous handler,
    /// decoupling how you generate values from how you consume them.
    /// </summary>
    public partial class Producer<RT, OUT, R> : Proxy<RT, Void, Unit, Unit, OUT, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, OUT, R> Value;
        
        public Producer(Proxy<RT, Void, Unit, Unit, OUT, R> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, R> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Bind<S>(Func<R, Proxy<RT, Void, Unit, Unit, OUT, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, R> For<C1, C>(Func<OUT, Proxy<RT, Void, Unit, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, S> Action<S>(Proxy<RT, Void, Unit, Unit, OUT, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, R> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, R> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, OUT, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, Unit, Void, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, OUT, R> Observe() =>
            Value.Observe();

        [Pure]
        public static Effect<RT, R> operator |(Producer<RT, OUT, R> p1, Consumer<RT, OUT, R> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Effect<RT, R> operator |(Producer<RT, OUT, R> p1, Consumer<OUT, R> p2) => 
            Proxy.compose(p1, p2);
        
        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, OUT, R> value) =>
            value = Value;

        [Pure]
        public static implicit operator Producer<RT, OUT, R>(Producer<OUT, R> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Producer<RT, OUT, R>(Pure<R> p) =>
            Producer.Pure<RT, OUT, R>(p.Value);
    }

    /// <summary>
    /// Consumers only await
    /// </summary>
    public partial class Consumer<RT, IN, R> : Proxy<RT, Unit, IN, Unit, Void, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, Void, R> Value;
        
        public Consumer(Proxy<RT, Unit, IN, Unit, Void, R> value) =>
            Value = value;

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, R> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Bind<S>(Func<R, Proxy<RT, Unit, IN, Unit, Void, S>> f) =>
            Value.Bind(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, R> For<C1, C>(Func<Void, Proxy<RT, Unit, IN, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, S> Action<S>(Proxy<RT, Unit, IN, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, Void, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);
        
        [Pure]
        public override Proxy<RT, Void, Unit, IN, Unit, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, Void, R> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, Void, R> value) =>
            value = Value;

        [Pure]
        public static implicit operator Consumer<RT, IN, R>(Consumer<IN, R> c) =>
            c.Interpret<RT>();

        [Pure]
        public static implicit operator Consumer<RT, IN, R>(ConsumerLift<RT, IN, R> c) =>
            c.Interpret();

        [Pure]
        public static implicit operator Consumer<RT, IN, R>(Pure<R> p) =>
            Consumer.Pure<RT, IN, R>(p.Value);
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
    public partial class Pipe<RT, IN, OUT, R> : Proxy<RT, Unit, IN, Unit, OUT, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, IN, Unit, OUT, R> Value;
        
        public Pipe(Proxy<RT, Unit, IN, Unit, OUT, R> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, R> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Bind<S>(Func<R, Proxy<RT, Unit, IN, Unit, OUT, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Unit, IN, C1, C, R> For<C1, C>(Func<OUT, Proxy<RT, Unit, IN, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, S> Action<S>(Proxy<RT, Unit, IN, Unit, OUT, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, IN, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, OUT, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, OUT, IN>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, OUT, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, IN, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<OUT, Proxy<RT, Unit, IN, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, OUT, Unit, IN, Unit, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, IN, Unit, OUT, R> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, IN, Unit, OUT, R> value) =>
            value = Value;
        
        [Pure]
        public static Producer<RT, OUT, R> operator |(Producer<RT, IN, R> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, R> operator |(Producer<IN, R> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, R> operator |(Pipe<RT, IN, OUT, R> p1, Consumer<OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Consumer<RT, IN, R> operator |(Pipe<RT, IN, OUT, R> p1, Consumer<RT, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public static Producer<RT, OUT, R> operator |(Producer<OUT, IN> p1, Pipe<RT, IN, OUT, R> p2) =>
            Proxy.compose(p1, p2);
        
        [Pure]
        public Pipe<RT, IN, C, R> Then<C>(Pipe<RT, OUT, C, R> p2) =>
            Proxy.compose(this, p2);

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, R>(Pipe<IN, OUT, R> p) =>
            p.Interpret<RT>();

        [Pure]
        public static implicit operator Pipe<RT, IN, OUT, R>(Pure<R> p) =>
            Pipe.Pure<RT, IN, OUT, R>(p.Value);
    }

    /// <summary>
    /// Clients only request and never respond
    /// </summary>
    public partial class Client<RT, UOut, UIn, R> : Proxy<RT, UOut, UIn, Unit, Unit, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, UOut, UIn, Unit, Unit, R> Value;

        public Client(Proxy<RT, UOut, UIn, Unit, Unit, R> value) =>
            Value = value;
 
        [Pure]
        public override Proxy<RT, UOut, UIn, Unit, Unit, R> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, UOut, UIn, Unit, Unit, S> Bind<S>(Func<R, Proxy<RT, UOut, UIn, Unit, Unit, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, UOut, UIn, Unit, Unit, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, R> For<C1, C>(Func<Unit, Proxy<RT, UOut, UIn, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, UOut, UIn, Unit, Unit, S> Action<S>(Proxy<RT, UOut, UIn, Unit, Unit, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Unit, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Unit, R> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, Unit, Unit, UIn>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Unit, Proxy<RT, Unit, Unit, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Unit, Proxy<RT, UOut, UIn, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, Unit, UIn, UOut, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, UOut, UIn, Unit, Unit, R> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, UOut, UIn, Unit, Unit, R> value) =>
            value = Value;
    }

    /// <summary>
    /// Servers only respond and never request
    /// </summary>
    public partial class Server<RT, DIn, DOut, R> : Proxy<RT, Unit, Unit, DIn, DOut, R>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Unit, Unit, DIn, DOut, R> Value;
    
        public Server(Proxy<RT, Unit, Unit, DIn, DOut, R> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Unit, Unit, DIn, DOut, R> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Unit, Unit, DIn, DOut, S> Bind<S>(Func<R, Proxy<RT, Unit, Unit, DIn, DOut, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Unit, Unit, DIn, DOut, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Unit, Unit, C1, C, R> For<C1, C>(Func<DOut, Proxy<RT, Unit, Unit, C1, C, DIn>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Unit, Unit, DIn, DOut, S> Action<S>(Proxy<RT, Unit, Unit, DIn, DOut, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, Unit, Unit, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, R> ComposeRight<UOutA, AUInA>(Func<Unit, Proxy<RT, UOutA, AUInA, DIn, DOut, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Unit, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Unit, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, Unit, Unit, DInC, DOutC, DIn>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, DOut, DIn, Unit, Unit, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Unit, Unit, DIn, DOut, R> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Unit, Unit, DIn, DOut, R> value) =>
            value = Value;
    }

    /// <summary>
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type
    /// It neither yields nor awaits, but represents an entire effect system
    /// </summary>
    public partial class Effect<RT, R> : Proxy<RT, Void, Unit, Unit, Void, R> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, Void, R> Value;

        public Effect(Proxy<RT, Void, Unit, Unit, Void, R> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, R> ToProxy() =>
            Value.ToProxy();
        [Pure]

        public override Proxy<RT, Void, Unit, Unit, Void, S> Bind<S>(Func<R, Proxy<RT, Void, Unit, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Map<S>(Func<R, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, R> For<C1, C>(Func<Void, Proxy<RT, Void, Unit, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Action<S>(Proxy<RT, Void, Unit, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, R> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, R>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, R> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, Void, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, R>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, R> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, R> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, R> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, Void, R> value) =>
            value = Value;
    }
}
