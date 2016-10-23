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
    /// Some, None, or Failure</returns>
    public struct TryOption<A>
    {
        static readonly Func<Option<A>> defaultA = () => default(A);

        readonly Func<Option<A>> value;

        /// <summary>
        /// Constructor a Try of A
        /// </summary>
        /// <param name="value">Delegate value</param>
        internal TryOption(Func<Option<A>> value)
        {
            this.value = value;
        }

        /// <summary>
        /// Safe accessor to this.value
        /// </summary>
        Func<Option<A>> Value => value ?? defaultA;

        /// <summary>
        /// Run the wrapped delegate without try/catch protection
        /// </summary>
        internal Option<A> Run() =>
            Value();

        /// <summary>
        /// Invoke a delegate if the Try returns a value successfully
        /// </summary>
        /// <param name="Some">Delegate to invoke if successful</param>
        public Unit IfSome(Action<A> Some)
        {
            var res = TryOptionExtensions.Try(this);
            if (!res.IsFaulted && res.Value.IsSome)
            {
                Some(res.Value.Value);
            }
            return unit;
        }

        /// <summary>
        /// Invoke a delegate if the Try is in a Fail or None state
        /// </summary>
        /// <param name="None">Delegate to invoke if successful</param>
        public Unit IfNone(Action None)
        {
            var res = TryOptionExtensions.Try(this);
            if (res.IsFaulted || res.Value.IsNone)
            {
                None();
            }
            return unit;
        }

        /// <summary>
        /// Return a default value if the Try fails
        /// </summary>
        /// <param name="defaultValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the Try otherwise</returns>
        [Pure]
        public A IfNone(A defaultValue)
        {
            if (isnull(defaultValue)) throw new ArgumentNullException(nameof(defaultValue));

            var res = TryOptionExtensions.Try(this);
            if (res.IsFaulted || res.Value.IsNone)
                return defaultValue;
            else
                return res.Value.Value;
        }

        /// <summary>
        /// Invoke a delegate if the Try fails
        /// </summary>
        /// <param name="None">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
        [Pure]
        public A IfNone(Func<A> None)
        {
            var res = TryOptionExtensions.Try(this);
            if (res.IsFaulted || res.Value.IsNone)
                return None();
            else
                return res.Value.Value;
        }

        /// <summary>
        /// Invoke delegates based on None or Failed stateds
        /// </summary>
        /// <typeparam name="T">Bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="None">Delegate to invoke if the result is None</param>
        /// <param name="Fail">Delegate to invoke if the result is Fail</param>
        /// <returns>Success value, or the result of the None or Failed delegate</returns>
        [Pure]
        public A IfNoneOrFail(
            Func<A> None,
            Func<Exception, A> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            if (res.Value.IsNone)
                return None();
            else if (res.IsFaulted)
                return Fail(res.Exception);
            else
                return res.Value.Value;
        }

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public ExceptionMatch<Option<A>> IfFail()
        {
            var res = TryOptionExtensions.Try(this);
            if (res.IsFaulted)
                return res.Exception.Match<Option<A>>();
            else
                return new ExceptionMatch<Option<A>>(res.Value);
        }

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ or Fail delegates</returns>
        [Pure]
        public R Match<R>(Func<A, R> Some, Func<R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value);
        }

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ or Fail delegates</returns>
        [Pure]
        public R Match<R>(Func<A, R> Some, Func<R> None, Func<Exception, R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : None();
        }

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Default value to use if the Try computation fails</param>
        /// <returns>The result of either the Succ delegate or the Fail value</returns>
        [Pure]
        public R Match<R>(Func<A, R> Some, R Fail)
        {
            if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Fail
                : Some(res.Value.Value);
        }

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public Unit Match(Action<A> Some, Action Fail)
        {
            var res = TryOptionExtensions.Try(this);

            if (res.IsFaulted || res.Value.IsNone)
                Fail();
            else
                Some(res.Value.Value);

            return Unit.Default;
        }

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public Unit Match(Action<A> Some, Action None, Action<Exception> Fail)
        {
            var res = TryOptionExtensions.Try(this);

            if (res.IsFaulted)
                Fail(res.Exception);
            else if(res.Value.IsNone)
                None();
            else 
                Some(res.Value.Value);

            return Unit.Default;
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Some, Func<R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted || res.Value.IsNone
                ? Task.FromResult(Fail())
                : Some(res.Value.Value));
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Some, Func<R> None, Func<Exception, R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : Task.FromResult(None()));
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Some, Func<Task<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value));
        }

        public async Task<R> MatchAsync<R>(Func<A, Task<R>> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : None());
        }

        public async Task<R> MatchAsync<R>(Func<A, R> Some, Func<Task<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Task.FromResult(Some(res.Value.Value)));
        }

        public async Task<R> MatchAsync<R>(Func<A, R> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return await (res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome 
                    ? Task.FromResult(Some(res.Value.Value))
                    : None());
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Some, Func<R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Observable.Return(Fail())
                : Some(res.Value.Value);
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Some, Func<R> None, Func<Exception, R> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : Observable.Return(None());
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Some, Func<IObservable<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value);
        }

        public IObservable<R> MatchObservable<R>(Func<A, IObservable<R>> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : None();
        }

        public IObservable<R> MatchObservable<R>(Func<A, R> Some, Func<IObservable<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Observable.Return(Some(res.Value.Value));
        }

        public IObservable<R> MatchObservable<R>(Func<A, R> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Observable.Return(Some(res.Value.Value))
                    : None();
        }

        /// <summary>
        /// Memoise the try
        /// </summary>
        public TryOption<A> Memo()
        {
            var res = TryOptionExtensions.Try(this);
            return TryOption(() =>
            {
                if (res.IsFaulted) throw new InnerException(res.Exception);
                return res.Value;
            });
        }

        [Pure]
        public Option<A> ToOption()
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? None
                : res.Value;
        }

        [Pure]
        public Try<A> ToTry()
        {
            var self = this;
            return Try(() => self.IfFailThrow());
        }

        [Pure]
        public A IfFailThrow()
        {
            try
            {
                var res = Run();
                if (res.IsNone) throw new ValueIsNoneException();
                return res.Value;
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                throw;
            }
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TryOption<U> Select<U>(Func<A, U> select)
        {
            var self = this;
            return TryOption(() => self.Run().Map(select));
        }

        /// <summary>
        /// Apply Try values to a Try function of arity 2
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg1">Try argument</param>
        /// <param name="arg2">Try argument</param>
        /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
        public Unit Iter(Action<A> action) =>
            IfSome(action);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TrTry computation</param>
        /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
        [Pure]
        public int Count()
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? 0
                : 1;
        }

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the Try computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public bool ForAll(Func<A, bool> pred)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? false
                : pred(res.Value.Value);
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
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? state
                : folder(state, res.Value.Value);
        }

        /// <summary>
        /// Folds Try value into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function for Success</param>
        /// <param name="Fail">Fold function for Failure</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S BiFold<S>(S state, Func<S, A, S> Some, Func<S, S> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? Fail(state)
                : Some(state, res.Value.Value);
        }

        /// <summary>
        /// Folds Try value into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function for Success</param>
        /// <param name="Fail">Fold function for Failure</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S TriFold<S>(S state, Func<S, A, S> Some, Func<S, S> None, Func<S, Exception, S> Fail)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted
                ? Fail(state, res.Exception)
                : res.Value.IsSome
                    ? Some(state, res.Value.Value)
                    : None(state);
        }

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public bool Exists(Func<A, bool> pred)
        {
            var res = TryOptionExtensions.Try(this);
            return res.IsFaulted || res.Value.IsNone
                ? false
                : pred(res.Value.Value);
        }

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="mapper">Delegate to map the bound value</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public TryOption<R> Map<R>(Func<A, R> mapper)
        {
            var self = this;
            return TryOption(() => self.Run().Map(mapper));
        }

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public TryOption<R> BiMap<R>(Func<A, R> Some, Func<R> Fail)
        {
            var self = this;
            return TryOption(() =>
            {
                var res = self.Try();
                return res.IsFaulted || res.Value.IsNone
                    ? Fail()
                    : Some(res.Value.Value);
            });
        }

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public TryOption<R> TriMap<R>(Func<A, R> Some, Func<R> None, Func<Exception, R> Fail)
        {
            var self = this;
            return TryOption(() =>
            {
                var res = self.Try();
                return res.IsFaulted
                    ? Fail(res.Exception)
                    : res.Value.IsSome
                        ? Some(res.Value.Value)
                        : None();
            });
        }

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public TryOption<Func<B, R>> ParMap<B, R>(Func<A, B, R> func) =>
            Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public TryOption<Func<B, Func<C, R>>> ParMap<B, C, R>(Func<A, B, C, R> func) =>
            Map(curry(func));

        [Pure]
        public TryOption<A> Filter(Func<A, bool> pred)
        {
            var self = this;
            return TryOption(() =>
            {
                var res = self.Run();
                return res.IsSome && pred(res.Value)
                    ? res
                    : Option<A>.None;
            });
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TryOption<A> Where(Func<A, bool> pred) =>
            Filter(pred);

        [Pure]
        public TryOption<R> Bind<R>(Func<A, TryOption<R>> binder)
        {
            var self = this;
            return TryOption(() =>
            {
                var opt = self.Run();
                return opt.IsSome
                    ? binder(opt.Value).Run()
                    : Option<R>.None;
            });
        }

        [Pure]
        public TryOption<R> BiBind<R>(Func<A, TryOption<R>> Some, Func<TryOption<R>> Fail)
        {
            var self = this;
            return TryOption(() =>
            {
                var res = self.Try();
                return res.IsFaulted || res.Value.IsNone
                    ? Fail().Run()
                    : Some(res.Value.Value).Run();
            });
        }

        [Pure]
        public TryOption<R> TriBind<R>(Func<A, TryOption<R>> Some, Func<TryOption<R>> None, Func<Exception, TryOption<R>> Fail)
        {
            var self = this;
            return TryOption(() =>
            {
                var res = self.Try();
                return res.IsFaulted
                    ? Fail(res.Exception).Run()
                    : res.Value.IsSome
                        ? Some(res.Value.Value).Run()
                        : None().Run();
            });
        }

        [Pure]
        public IEnumerable<Either<Exception, A>> AsEnumerable()
        {
            var res = TryOptionExtensions.Try(this);

            if (res.IsFaulted)
            {
                yield return res.Exception;
            }
            else if(res.Value.IsSome)
            {
                yield return res.Value.Value;
            }
        }

        [Pure]
        public Lst<Either<Exception, A>> ToList() =>
            toList(AsEnumerable());

        [Pure]
        public Either<Exception, A>[] ToArray() =>
            toArray(AsEnumerable());

        [Pure]
        public TryOptionSomeContext<A, R> Some<R>(Func<A, R> Some) =>
            new TryOptionSomeContext<A, R>(this, Some);

        [Pure]
        public TryOptionSomeUnitContext<A> Some(Action<A> Some) =>
            new TryOptionSomeUnitContext<A>(this, Some);

        [Pure]
        public override string ToString() =>
            match(this,
                Some: v => isnull(v)
                          ? "Some(null)"
                          : $"Some({v})",
                None: () => "None",
                Fail: ex => $"Fail({ex.Message})"
            );

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TryOption<V> SelectMany<U, V>(
            Func<A, TryOption<U>> bind,
            Func<A, U, V> project)
        {
            var self = this;
            return TryOption(() =>
            {
                var resT = self.Run();
                if (resT.IsNone) return Option<V>.None;
                var resU = bind(resT.Value).Run();
                if (resU.IsNone) return Option<V>.None;
                return project(resT.Value, resU.Value);
            });
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<V> SelectMany<U, V>(
            Func<A, IEnumerable<U>> bind,
            Func<A, U, V> project
            )
        {
            var resT = TryOptionExtensions.Try(this);
            if (resT.IsFaulted || resT.Value.IsNone) return new V[0];
            return bind(resT.Value.Value).Map(resU => project(resT.Value.Value, resU));
        }

        public TryOption<V> Join<U, K, V>(
            TryOption<U> inner,
            Func<A, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<A, U, V> project)
        {
            var self = this;
            return TryOption(() =>
            {
                var selfRes = self.Run();
                var innerRes = inner.Run();
                return selfRes.IsSome && innerRes.IsSome && EqualityComparer<K>.Default.Equals(outerKeyMap(selfRes.Value), innerKeyMap(innerRes.Value))
                    ? project(selfRes.Value, innerRes.Value)
                    : raise<V>(new BottomException());
            });
        }

        /// <summary>
        /// Implicit conversion operator from TryOption delegate to Try
        /// </summary>
        /// <param name="value">TryOption delegate to wrap</param>
        public static implicit operator TryOption<A>(Func<Option<A>> value) =>
            new TryOption<A>(value);
    }
}
