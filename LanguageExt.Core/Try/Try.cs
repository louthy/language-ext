using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.Instances;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    /// <summary>
    /// The Try monad captures exceptions and uses them to cancel the
    /// computation.  Primarily useful for expression based processing
    /// of errors.
    /// </summary>
    /// <remarks>To invoke directly, call x.Try()</remarks>
    /// <returns>A value that represents the outcome of the computation, either
    /// Success or Failure</returns>
    public struct Try<A> : Monad<A>, Optional<A>
    {
        static readonly Func<A> defaultA = () => default(A);

        readonly Func<A> value;

        /// <summary>
        /// Constructor a Try of A
        /// </summary>
        /// <param name="value">Delegate value</param>
        Try(Func<A> value)
        {
            this.value = value;
        }

        /// <summary>
        /// Safe accessor to this.value
        /// </summary>
        Func<A> Value => value ?? defaultA;

        /// <summary>
        /// Run the wrapped delegate without try/catch protection
        /// </summary>
        internal A Run() =>
            Value();

        /// <summary>
        /// Append the Try(x) to Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to sum the Trys (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public Try<A> Append(Try<A> rhs)
        {
            var self = this;
            return Try(() => TypeDesc.Append(self.Run(), rhs.Run(), TypeDesc<A>.Default));
        }

        /// <summary>
        /// Subtract the Try(x) from Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to find the difference between the Trys (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public Try<A> Subtract(Try<A> rhs)
        {
            var self = this;
            return Try(() => TypeDesc.Subtract(self.Run(), rhs.Run(), TypeDesc<A>.Default));
        }

        /// <summary>
        /// Find the product of Try(x) and Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to multiply the Trys (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public Try<A> Multiply(Try<A> rhs)
        {
            var self = this;
            return Try(() => TypeDesc.Multiply(self.Run(), rhs.Run(), TypeDesc<A>.Default));
        }

        /// <summary>
        /// Divide Try(x) by Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to divide the Trys (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public Try<A> Divide(Try<A> rhs)
        {
            var self = this;
            return Try(() => TypeDesc.Divide(self.Run(), rhs.Run(), TypeDesc<A>.Default));
        }

        /// <summary>
        /// Invokes the succHandler if Try is in the Success state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSucc(Action<A> succHandler)
        {
            var res = TryExtensions.Try(this);
            if (!res.IsFaulted)
            {
                succHandler(res.Value);
            }
            return unit;
        }

        /// <summary>
        /// Returns the Succ(value) of the Try or a default if it's Fail
        /// </summary>
        [Pure]
        public A IfFail(A defaultValue)
        {
            if (isnull(defaultValue)) throw new ArgumentNullException(nameof(defaultValue));

            var res = TryExtensions.Try(this);
            if (res.IsFaulted)
                return defaultValue;
            else
                return res.Value;
        }

        /// <summary>
        /// Returns the Succ(value) of the Try or a default if it's Fail
        /// </summary>
        [Pure]
        public A IfFail(Func<A> defaultAction)
        {
            var res = TryExtensions.Try(this);
            if (res.IsFaulted)
                return defaultAction();
            else
                return res.Value;
        }

        /// <summary>
        /// Returns the Succ(value) of the Try or a default if it's Fail
        /// </summary>
        [Pure]
        public A IfFail(Func<Exception, A> defaultAction)
        {
            var res = TryExtensions.Try(this);
            if (res.IsFaulted)
                return defaultAction(res.Exception);
            else
                return res.Value;
        }

        /// <summary>
        /// Returns an exception matching context.  Call a chain of With<ExceptionType>() to handle specific
        /// exceptions, followed by Otherwise or OtherwiseThrow()
        /// </summary>
        [Pure]
        public ExceptionMatch<A> IfFail()
        {
            var res = TryExtensions.Try(this);
            if (res.IsFaulted)
                return res.Exception.Match<A>();
            else
                return new ExceptionMatch<A>(res.Value);
        }

        [Pure]
        public R Match<R>(Func<A, R> Succ, Func<Exception, R> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        }

        [Pure]
        public R Match<R>(Func<A, R> Succ, R Fail)
        {
            if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail
                : Succ(res.Value);
        }

        public Unit Match(Action<A> Succ, Action<Exception> Fail)
        {
            var res = TryExtensions.Try(this);

            if (res.IsFaulted)
                Fail(res.Exception);
            else
                Succ(res.Value);

            return Unit.Default;
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Succ, Func<Exception, R> Fail)
        {
            var res = TryExtensions.Try(this);
            return await (res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : Succ(res.Value));
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail)
        {
            var res = TryExtensions.Try(this);
            return await (res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value));
        }

        public async Task<R> MatchAsync<R>(Func<A, R> Succ, Func<Exception, Task<R>> Fail)
        {
            var res = TryExtensions.Try(this);
            return await (res.IsFaulted
                ? Fail(res.Exception)
                : Task.FromResult(Succ(res.Value)));
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Succ, Func<Exception, R> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : Succ(res.Value);
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        }

        public IObservable<R> MatchObservable<R>(Func<A, R> Succ, Func<Exception, IObservable<R>> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : Observable.Return(Succ(res.Value));
        }

        /// <summary>
        /// Memoise the try
        /// </summary>
        public Try<A> Memo()
        {
            var res = TryExtensions.Try(this);
            return Try(() =>
            {
                if (res.IsFaulted) throw new InnerException(res.Exception);
                return res.Value;
            });
        }

        [Pure]
        public Option<A> ToOption()
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? None
                : Optional(res.Value);
        }

        [Pure]
        public TryOption<A> ToTryOption()
        {
            var self = this;
            return () =>
            {
               var res = TryExtensions.Try(self);
               return res.IsFaulted
                   ? None
                   : Optional(res.Value);
            };
        }

        [Pure]
        public A IfFailThrow()
        {
            try
            {
                return Run();
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                throw;
            }
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Try<U> Select<U>(Func<A, U> select)
        {
            var self = this;
            return Try(() => select(self.Run()));
        }

        public Unit Iter(Action<A> action) =>
            IfSucc(action);

        public Unit Iter(Action<A> Succ, Action<Exception> Fail)
        {
            var res = TryExtensions.Try(this);
            if (res.IsFaulted)
            {
                Fail(res.Exception);
            }
            else
            {
                Succ(res.Value);
            }
            return unit;
        }

        [Pure]
        public int Count<T>()
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? 0
                : 1;
        }

        [Pure]
        public bool ForAll(Func<A, bool> pred)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? false
                : pred(res.Value);
        }

        [Pure]
        public bool ForAll(Func<A, bool> Succ, Func<Exception, bool> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        }

        /// <summary>
        /// Folds Try value into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? state
                : folder(state, res.Value);
        }

        /// <summary>
        /// Folds Try value into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Succ">Fold function for Success</param>
        /// <param name="Fail">Fold function for Failure</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(state, res.Exception)
                : Succ(state, res.Value);
        }

        [Pure]
        public bool Exists(Func<A, bool> pred)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? false
                : pred(res.Value);
        }

        [Pure]
        public bool Exists(Func<A, bool> Succ, Func<Exception, bool> Fail)
        {
            var res = TryExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        }

        [Pure]
        public Try<R> Map<R>(Func<A, R> mapper)
        {
            var self = this;
            return Try(() => mapper(self.Run()));
        }

        [Pure]
        public Try<R> BiMap<R>(Func<A, R> Succ, Func<Exception, R> Fail)
        {
            var self = this;
            return Try(() =>
            {
                var res = self.Try();
                return res.IsFaulted
                    ? Fail(res.Exception)
                    : Succ(res.Value);
            });
        }

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public Try<Func<B, R>> ParMap<B, R>(Func<A, B, R> func) =>
            Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public Try<Func<B, Func<C, R>>> ParMap<B, C, R>(Func<A, B, C, R> func) =>
            Map(curry(func));

        [Pure]
        public Try<A> Filter(Func<A, bool> pred)
        {
            var self = this;
            return Try(() =>
            {
                var res = self.Run();
                return pred(res)
                    ? res
                    : raise<A>(new BottomException());
            });
        }

        [Pure]
        public Try<A> BiFilter(Func<A, bool> Succ, Func<Exception, bool> Fail)
        {
            var self = this;
            return Try(() =>
            {
                var res = self.Try();
                return res.IsFaulted
                    ? Fail(res.Exception)
                        ? res.Value
                        : raise<A>(new BottomException())
                    : Succ(res.Value)
                        ? res.Value
                        : raise<A>(new BottomException());
            });
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Try<A> Where(Func<A, bool> pred) =>
            Filter(pred);

        [Pure]
        public Try<R> Bind<R>(Func<A, Try<R>> binder)
        {
            var self = this;
            return Try(() => binder(self.Run()).Run());
        }

        [Pure]
        public Try<R> BiBind<R>(Func<A, Try<R>> Succ, Func<Exception, Try<R>> Fail)
        {
            var self = this;
            return Try(() =>
            {
                var res = self.Try();
                return res.IsFaulted
                    ? Fail(res.Exception).Run()
                    : Succ(res.Value).Run();
            });
        }

        [Pure]
        public IEnumerable<Either<Exception, A>> AsEnumerable()
        {
            var res = TryExtensions.Try(this);

            if (res.IsFaulted)
            {
                yield return res.Exception;
            }
            else
            {
                yield return res.Value;
            }
        }

        [Pure]
        public Lst<Either<Exception, A>> ToList() =>
            toList(AsEnumerable());

        [Pure]
        public Either<Exception, A>[] ToArray() =>
            toArray(AsEnumerable());

        [Pure]
        public TrySuccContext<A, R> Succ<R>(Func<A, R> succHandler) =>
            new TrySuccContext<A, R>(this, succHandler);

        [Pure]
        public TrySuccUnitContext<A> Succ(Action<A> succHandler) =>
            new TrySuccUnitContext<A>(this, succHandler);

        [Pure]
        public override string ToString() =>
            match(this,
                Succ: v => isnull(v)
                          ? "Succ(null)"
                          : $"Succ({v})",
                Fail: ex => $"Fail({ex.Message})"
            );

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Try<V> SelectMany<U, V>(
            Func<A, Try<U>> bind,
            Func<A, U, V> project)
        {
            var self = this;
            return Try(() =>
            {
                var resT = self.Run();
                return project(resT, bind(resT).Run());
            });
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<V> SelectMany<U, V>(
            Func<A, IEnumerable<U>> bind,
            Func<A, U, V> project
            )
        {
            var resT = TryExtensions.Try(this);
            if (resT.IsFaulted) return new V[0];
            return bind(resT.Value).Map(resU => project(resT.Value, resU));
        }

        public Try<V> Join<U, K, V>(
            Try<U> inner,
            Func<A, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<A, U, V> project)
        {
            var self = this;
            return Try(() =>
            {
                var selfRes = self.Run();
                var innerRes = inner.Run();
                return EqualityComparer<K>.Default.Equals(outerKeyMap(selfRes), innerKeyMap(innerRes))
                    ? project(selfRes, innerRes)
                    : raise<V>(new BottomException());
            });
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var ta = AsTry(ma).Try();
            return ta.IsFaulted
                ? default(Try<B>).Fail(ta.Exception)
                : f(ta.Value);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B>
        {
            var ta = AsTry(ma).Try();
            return ta.IsFaulted
                ? (MB)default(MB).Fail(ta.Exception)
                : f(ta.Value);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<A> Fail(Exception err) =>
            Try<A>(() => { throw new InnerException(err); });

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public S Fold<S>(Foldable<A> fa, S state, Func<S, A, S> f)
        {
            var ta = AsTry(fa).Run();
            return f(state, ta);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public S FoldBack<S>(Foldable<A> fa, S state, Func<S, A, S> f)
        {
            var ta = AsTry(fa).Run();
            return f(state, ta);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNoneA(Optional<A> a) =>
            AsTry(a).Try().IsFaulted;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSomeA(Optional<A> a) =>
            !IsNoneA(a);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsUnsafe(Optional<A> a) =>
            true;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            Try(() => f(AsTry(fa).Run()));

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            AsTry(a).ToOption().Match(Some, None);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            AsTry(a).ToOption().MatchUnsafe(Some, None);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<A> Return(A x, params A[] xs) =>
            Try(() => x);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<A> Return(IEnumerable<A> xs)
        {
            var x = xs.Take(1).ToArray();
            return x.Length == 0
                ? Try(() => raise<A>(new BottomException()))
                : Try(() => x[0]);
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static Try<A> AsTry(Optional<A> a) => (Try<A>)a;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static Try<A> AsTry(Monad<A> a) => (Try<A>)a;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static Try<A> AsTry(Functor<A> a) => (Try<A>)a;

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static Try<A> AsTry(Foldable<A> a) => (Try<A>)a;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<A> Zero() =>
            Try(() => default(A));

        /// <summary>
        /// Implicit conversion operator from TryDelegate to Try
        /// </summary>
        /// <param name="value">TryDelegate to wrap</param>
        public static implicit operator Try<A>(Func<A> value) =>
            new Try<A>(value);
    }

    public static class TryTest
    {
        public static Try<int> Test() => 
            Try(() => 0);
    }
}
