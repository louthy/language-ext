using System;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class TryExtensions
{
    /// <summary>
    /// Memoize the computation so that it's only run once
    /// </summary>
    public static Try<A> Memo<A>(this Try<A> ma)
    {
        bool run = false;
        Result<A> result = new Result<A>();
        return (() =>
        {
            if (run) return result;
            result = ma.Try();
            run = true;
            return result;
        });
    }

    /// <summary>
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    [Pure]
    public static Unit IfSucc<A>(this Try<A> self, Action<A> Succ)
    {
        var res = self.Try();
        if (!res.IsFaulted)
        {
            Succ(res.Value);
        }
        return unit;
    }

    /// <summary>
    /// Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static A IfFail<A>(this Try<A> self, A failValue)
    {
        if (isnull(failValue)) throw new ArgumentNullException(nameof(failValue));

        var res = self.Try();
        if (res.IsFaulted)
            return failValue;
        else
            return res.Value;
    }

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static A IfFail<A>(this Try<A> self, Func<A> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return Fail();
        else
            return res.Value;
    }

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static A IfFail<A>(this Try<A> self, Func<Exception, A> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return Fail(res.Exception);
        else
            return res.Value;
    }

    /// <summary>
    /// Provides a fluent exception matching interface which is invoked
    /// when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatch<A> IfFail<A>(this Try<A> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return res.Exception.Match<A>();
        else
            return new ExceptionMatch<A>(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static R Match<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Default value to use if the Try computation fails</param>
    /// <returns>The result of either the Succ delegate or the Fail value</returns>
    [Pure]
    public static R Match<A, R>(this Try<A> self, Func<A, R> Succ, R Fail)
    {
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : Succ(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static Unit Match<A>(this Try<A> self, Action<A> Succ, Action<Exception> Fail)
    {
        var res = self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            Succ(res.Value);

        return Unit.Default;
    }

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    [Pure]
    public static Option<A> ToOption<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    }

    [Pure]
    public static TryOption<A> ToTryOption<A>(this Try<A> self) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    };

    [Pure]
    public static OptionUnsafe<A> ToOptionUnsafe<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : OptionUnsafe<A>.Some(res.Value);
    }

    [Pure]
    public static Validation<Exception, A> ToValidation<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail<Exception, A>(res.Exception)
            : Success<Exception, A>(res.Value);
    }

    [Pure]
    public static Either<Exception, A> ToEither<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Either<Exception, A>.Left(res.Exception)
            : Either<Exception, A>.Right(res.Value);
    }

    [Pure]
    public static EitherUnsafe<Exception, A> ToEitherUnsafe<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? EitherUnsafe<Exception, A>.Left(res.Exception)
            : EitherUnsafe<Exception, A>.Right(res.Value);
    }

    [Pure]
    public static A IfFailThrow<A>(this Try<A> self)
    {
        try
        {
            return self().Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            throw;
        }
    }

    /// <summary>
    /// Map the bound value from A to B
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Returned bound value type</typeparam>
    /// <param name="self">This</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped Try</returns>
    [Pure]
    public static Try<B> Select<A, B>(this Try<A> self, Func<A, B> f) => 
        Map(self, f);
    
    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Unit Iter<A>(this Try<A> self, Action<A> action) =>
        self.IfSucc(action);

    /// <summary>
    /// Counts the number of bound values.  
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">TrTry computation</param>
    /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
    [Pure]
    public static int Count<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
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
    public static bool ForAll<A>(this Try<A> self, Func<A, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
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
    public static S Fold<A, S>(this Try<A> self, S state, Func<S, A, S> folder)
    {
        var res = self.Try();
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
    public static S BiFold<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(state, res.Exception)
            : Succ(state, res.Value);
    }

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static bool Exists<A>(this Try<A> self, Func<A, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static Try<B> Map<A, B>(this Try<A> self, Func<A, B> f) =>
        Memo(() => self.Try().Map(f));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static Try<R> BiMap<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail) => Memo<R>(() =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    });

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Try<Func<B, R>> ParMap<A, B, R>(this Try<A> self, Func<A, B, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Try<Func<B, Func<C, R>>> ParMap<A, B, C, R>(this Try<A> self, Func<A, B, C, R> func) =>
        self.Map(curry(func));

    [Pure]
    public static Try<A> Filter<A>(this Try<A> self, Func<A, bool> pred) => Memo(() =>
    {
        var res = self();
        return pred(res.Value)
            ? res
            : raise<A>(new BottomException());
    });

    [Pure]
    public static Try<A> BiFilter<A>(this Try<A> self, Func<A, bool> Succ, Func<Exception, bool> Fail) => Memo<A>(() =>
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

    [Pure]
    public static Try<A> Where<A>(this Try<A> self, Func<A, bool> pred) =>
        self.Filter(pred);

    [Pure]
    public static Try<B> Bind<A, B>(this Try<A> self, Func<A, Try<B>> binder) =>
        MTry<A>.Inst.Bind<MTry<B>, Try<B>, B>(self, binder);

    [Pure]
    public static Try<R> BiBind<A, R>(this Try<A> self, Func<A, Try<R>> Succ, Func<Exception, Try<R>> Fail) => Memo(() =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception).Try()
            : Succ(res.Value).Try();
    });

    [Pure]
    public static Seq<A> AsEnumerable<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Empty
            : res.Value.Cons(Empty);
    }

    [Pure]
    public static Lst<A> ToList<A>(this Try<A> self) =>
        toList(self.AsEnumerable());

    [Pure]
    public static Arr<A> ToArray<A>(this Try<A> self) =>
        toArray(self.AsEnumerable());

    [Pure]
    public static TrySuccContext<A, R> Succ<A,R>(this Try<A> self, Func<A, R> succHandler) =>
        new TrySuccContext<A, R>(self, succHandler);

    [Pure]
    public static TrySuccUnitContext<A> Succ<A>(this Try<A> self, Action<A> succHandler) =>
        new TrySuccUnitContext<A>(self, succHandler);

    [Pure]
    public static string AsString<A>(this Try<A> self) =>
        self.Match(
            Succ: v => isnull(v)
                      ? "Succ(null)"
                      : $"Succ({v})",
            Fail: ex => $"Fail({ex.Message})");

    [Pure]
    public static Try<C> SelectMany<A, B, C>(
        this Try<A> self,
        Func<A, Try<B>> bind,
        Func<A, B, C> project) =>
            MTry<A>.Inst.Bind<MTry<C>, Try<C>, C>(self,    a =>
            MTry<B>.Inst.Bind<MTry<C>, Try<C>, C>(bind(a), b =>
            MTry<C>.Inst.Return(project(a, b))));

    [Pure]
    public static Try<V> Join<A, U, K, V>(
        this Try<A> self, 
        Try<U> inner,
        Func<A, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<A, U, V> project) => Memo<V>(() =>
        {
            var selfRes = self();
            var innerRes = inner();
            return EqualityComparer<K>.Default.Equals(outerKeyMap(selfRes.Value), innerKeyMap(innerRes.Value))
                ? project(selfRes.Value, innerRes.Value)
                : throw new BottomException();
        });

    [Pure]
    public static Result<T> Try<T>(this Try<T> self)
    {
        try
        {
            if (self == null)
            {
                throw new ArgumentNullException("this is null");
            }
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new Result<T>(e);
        }
    }

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, U> select)
        where T : IDisposable => () =>
            {
                var t = default(T);
                try
                {
                    return select(self().Value);
                }
                finally
                {
                    t?.Dispose();
                }
            };

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, Try<U>> select)
        where T : IDisposable => () =>
        {
            var t = default(T);
            try
            {
                t = self().Value;
                return select(t)().Value;
            }
            finally
            {
                t?.Dispose();
            }
        };

    [Pure]
    public static int Sum(this Try<int> self) =>
        self.Try().Value;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
        self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        });

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : Succ(res.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Observable.Return(Succ(res.Value));
        })
        from t in tt
        select t;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<T>> self) =>
        from x in self
        from y in x
        select y;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<T>>> self) =>
        from x in self
        from y in x
        from z in y
        select z;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<Try<T>>>> self) =>
        from w in self
        from x in w
        from y in x
        from z in y
        select z;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Try<B> Apply<A, B>(this Try<Func<A, B>> fab, Try<A> fa) =>
        ApplTry<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Try<B> Apply<A, B>(this Func<A, B> fab, Try<A> fa) =>
        ApplTry<A, B>.Inst.Apply(Prelude.Try(fab), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Try<C> Apply<A, B, C>(this Try<Func<A, B, C>> fabc, Try<A> fa, Try<B> fb) =>
        fabc.Bind(f => ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa, fb));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Try<C> Apply<A, B, C>(this Func<A, B, C> fabc, Try<A> fa, Try<B> fb) =>
        ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, B, C>> fabc, Try<A> fa) =>
        fabc.Bind(f => ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, Try<A> fa) =>
        ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, Func<B, C>>> fabc, Try<A> fa) =>
        ApplTry<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, Try<A> fa) =>
        ApplTry<A, B, C>.Inst.Apply(Prelude.Try(fabc), fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static Try<B> Action<A, B>(this Try<A> fa, Try<B> fb) =>
        ApplTry<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Compare the bound value of Try(x) to Try(y).  If either of the
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>1 if lhs > rhs, 0 if lhs == rhs, -1 if lhs < rhs</returns>
    [Pure]
    public static int Compare<ORD, A>(this Try<A> lhs, Try<A> rhs) where ORD : struct, Ord<A>
    {
        var x = lhs.Try();
        var y = lhs.Try();
        if (x.IsFaulted && y.IsFaulted) return 0;
        if (x.IsFaulted && !y.IsFaulted) return -1;
        if (!x.IsFaulted && y.IsFaulted) return 1;
        return default(ORD).Compare(x.Value, y.Value);
    }

    /// <summary>
    /// Append the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs ++ rhs</returns>
    [Pure]
    public static Try<A> Append<SEMI, A>(this Try<A> lhs, Try<A> rhs) where SEMI : struct, Semigroup<A> => 
        from x in lhs
        from y in rhs
        select append<SEMI, A>(x, y);

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Add<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select plus<NUM, A>(x, y);

    /// <summary>
    /// Find the subtract of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Subtract<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select subtract<NUM, A>(x, y);

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Product<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> => 
        from x in lhs
        from y in rhs
        select product<NUM, A>(x, y);

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Divide<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select divide<NUM, A>(x, y);

    /// <summary>
    /// Convert the Try type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Try to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static A? ToNullable<A>(this Try<A> ma) where A : struct
    {
        var x = ma.Try();
        return x.IsFaulted
            ? (A?)null
            : x.Value;
    }
}