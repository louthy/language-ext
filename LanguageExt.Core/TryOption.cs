using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// TryOption delegate
    /// </summary>
    public delegate TryOptionResult<T> TryOption<T>();

    /// <summary>
    /// Holds the state of the TryOption post invocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TryOptionResult<T>
    {
        internal readonly Option<T> Value;
        internal Exception Exception;

        public TryOptionResult(Option<T> value)
        {
            Value = value;
            Exception = null;
        }

        public TryOptionResult(Exception e)
        {
            Exception = e;
            Value = default(T);
        }

        public static implicit operator TryOptionResult<T>(Option<T> value) =>
            new TryOptionResult<T>(value);

        public static implicit operator TryOptionResult<T>(T value) =>
            new TryOptionResult<T>(OptionCast.Cast(value));

        public static implicit operator TryOptionResult<T>(OptionNone value) =>
            new TryOptionResult<T>(Option<T>.None);

        internal bool IsFaulted => Exception != null;

        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }


    public struct TryOptionSomeContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;

        internal TryOptionSomeContext(TryOption<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public TryOptionNoneContext<T, R> None(Func<R> noneHandler) =>
            new TryOptionNoneContext<T, R>(option, someHandler, noneHandler);

        public TryOptionNoneContext<T, R> None(R noneValue) =>
            new TryOptionNoneContext<T, R>(option, someHandler, () => noneValue);
    }

    public struct TryOptionSomeUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;

        internal TryOptionSomeUnitContext(TryOption<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public TryOptionNoneUnitContext<T> None(Action noneHandler) =>
            new TryOptionNoneUnitContext<T>(option, someHandler, noneHandler);
    }

    public struct TryOptionNoneContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;
        readonly Func<R> noneHandler;

        internal TryOptionNoneContext(TryOption<T> option, Func<T, R> someHandler, Func<R> noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        public R Fail(Func<Exception, R> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);

        public R Fail(R failValue) =>
            option.Match(someHandler, noneHandler, _ => failValue);
    }

    public struct TryOptionNoneUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;
        readonly Action noneHandler;

        internal TryOptionNoneUnitContext(TryOption<T> option, Action<T> someHandler, Action noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        public Unit Fail(Action<Exception> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);
    }
}

/// <summary>
/// Extension methods for the TryOption monad
/// </summary>
public static class __TryOptionExt
{
    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    [Obsolete("'Failure' has been deprecated.  Please use 'IfNone|IfNoneOrFail' instead")]
    public static T Failure<T>(this TryOption<T> self, T defaultValue)
    {
        if (defaultValue == null) throw new ArgumentNullException("defaultValue");

        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultValue;
        else
            return res.Value.Value;
    }

    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    [Obsolete("'Failure' has been deprecated.  Please use 'IfNone|IfNoneOrFail' instead")]
    public static T Failure<T>(this TryOption<T> self, Func<T> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultAction();
        else
            return res.Value.Value;
    }

    /// <summary>
    /// Invokes the someHandler if TryOption is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<T>(this TryOption<T> self, Action<T> someHandler)
    {
        var res = self.Try();
        if (res.Value.IsSome)
        {
            someHandler(res.Value.Value);
        }
        return unit;
    }

    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    public static T IfNone<T>(this TryOption<T> self, T defaultValue)
    {
        if (defaultValue == null) throw new ArgumentNullException("defaultValue");

        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultValue;
        else
            return res.Value.Value;
    }

    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    public static T IfNone<T>(this TryOption<T> self, Func<T> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultAction();
        else
            return res.Value.Value;
    }

    public static T IfNoneOrFail<T>(
        this TryOption<T> self,
        Func<T> None,
        Func<Exception, T> Fail)
    {
        var res = self.Try();
        if (res.Value.IsNone)
            return None();
        else if (res.IsFaulted)
            return Fail(res.Exception);
        else
            return res.Value.Value;
    }

    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : match(res.Value, Some, None);
    }

    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : match(res.Value, Some, () => None);
    }

    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, R Fail)
    {
        if (Fail == null) throw new ArgumentNullException("Fail");

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : match(res.Value, Some, None);
    }

    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, R Fail)
    {
        if (Fail == null) throw new ArgumentNullException("Fail");

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : match(res.Value, Some, () => None);
    }

    public static Unit Match<T>(this TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail)
    {
        var res = self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            match(res.Value, Some, None);

        return Unit.Default;
    }

    public static Option<T> ToOption<T>(this TryOption<T> self) =>
        self.Try().Value;

    public static TryOptionResult<T> Try<T>(this TryOption<T> self)
    {
        try
        {
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryOptionResult<T>(e);
        }
    }


    public static Option<T> IfFailThrow<T>(this TryOption<T> self)
    {
        try
        {
            var res = self();
            if (res.IsFaulted)
            {
                throw res.Exception;
            }
            return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            throw;
        }
    }

    public static TryOption<U> Select<T, U>(this TryOption<T> self, Func<T, U> select)
    {
        return new TryOption<U>(() =>
        {
            TryOptionResult<T> resT;
            try
            {
                resT = self();
                if (resT.IsFaulted)
                    return new TryOptionResult<U>(resT.Exception);
                if (resT.Value.IsNone)
                    return new TryOptionResult<U>(None);
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                return new TryOptionResult<U>(e);
            }

            Option<U> resU;
            try
            {
                resU = select(resT.Value.Value);
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                return new TryOptionResult<U>(e);
            }

            return new TryOptionResult<U>(resU);
        });
    }

    public static TryOption<V> SelectMany<T, U, V>(
        this TryOption<T> self,
        Func<T, TryOption<U>> bind,
        Func<T, U, V> project
        )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryOptionResult<U> resU;
                try
                {
                    resU = bind(resT.Value.Value)();
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                    if (resU.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static Unit Iter<T>(this TryOption<T> self, Action<T> action) =>
        self.IfSome(action);

    public static int Count<T>(this TryOption<T> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? 0
            : res.Value.Count();
    }

    public static bool ForAll<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.ForAll(pred);
    }

    public static S Fold<S, T>(this TryOption<T> self, S state, Func<S, T, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : res.Value.Fold(state, folder);
    }

    public static bool Exists<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.Exists(pred);
    }

    public static TryOption<T> Filter<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted || res.Value.IsNone
            ? () => res
            : pred(res.Value.Value)
                ? self
                : () => None;
    }

    public static TryOption<R> Map<T, R>(this TryOption<T> self, Func<T, R> mapper) =>
        self.Select(mapper);

    public static TryOption<R> Bind<T, R>(this TryOption<T> self, Func<T, TryOption<R>> binder) =>
        from x in self
        from y in binder(x)
        select y;

    public static TryOption<T> Where<T>(this TryOption<T> self, Func<T, bool> pred) =>
        self.Filter(pred);

    public static IEnumerable<Either<Exception, T>> AsEnumerable<T>(this TryOption<T> self)
    {
        var res = self.Try();

        if (res.IsFaulted)
        {
            yield return res.Exception;
        }
        else if (res.Value.IsSome)
        {
            yield return res.Value.Value;
        }
    }

    public static Lst<Either<Exception, T>> ToList<T>(this TryOption<T> self) =>
        toList(self.AsEnumerable());

    public static ImmutableArray<Either<Exception, T>> ToArray<T>(this TryOption<T> self) =>
        toArray(self.AsEnumerable());

    public static TryOptionSomeContext<T, R> Some<T, R>(this TryOption<T> self, Func<T, R> someHandler) =>
        new TryOptionSomeContext<T, R>(self, someHandler);

    public static TryOptionSomeUnitContext<T> Some<T>(this TryOption<T> self, Action<T> someHandler) =>
        new TryOptionSomeUnitContext<T>(self, someHandler);

    public static int Sum(this TryOption<int> self) =>
        self.Try().Value.Value;

    public static string AsString<T>(this TryOption<T> self) =>
        match(self,
            Some: v => v == null
                        ? "Some(null)"
                        : String.Format("Some({0})", v),
            None: () => "None",
            Fail: ex => "Fail(" + ex.Message + ")"
        );

    public static IEnumerable<V> SelectMany<T, U, V>(
        this TryOption<T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        TryOptionResult<T> resT;
        try
        {
            resT = self();
            if (resT.IsFaulted)
                return new V[0];
            if (resT.Value.IsNone)
                return new V[0];
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new V[0];
        }

        IEnumerable<U> resU;
        try
        {
            resU = bind(resT.Value.Value);
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new V[0];
        }

        try
        {
            return resU.Select(x => project(resT.Value.Value, x));
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new V[0];
        }
    }

    public static IEnumerable<V> SelectMany<T, U, V>(
        this TryOption<T> self,
        Func<T, Lst<U>> bind,
        Func<T, U, V> project
        )
    {
        TryOptionResult<T> resT;
        try
        {
            resT = self();
            if (resT.IsFaulted)
                return List<V>();
            if (resT.Value.IsNone)
                return List<V>();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return List<V>();
        }

        IEnumerable<U> resU;
        try
        {
            resU = bind(resT.Value.Value);
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return List<V>();
        }

        try
        {
            return LanguageExt.List.createRange(resU.Select(x => project(resT.Value.Value, x)));
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return List<V>();
        }
    }

    public static Map<K, V> SelectMany<K, T, U, V>(
        this TryOption<T> self,
        Func<T, Map<K, U>> bind,
        Func<T, U, V> project
        )
    {
        TryOptionResult<T> resT;
        try
        {
            resT = self();
            if (resT.IsFaulted)
                return Prelude.Map<K, V>();
            if (resT.Value.IsNone)
                return Prelude.Map<K, V>();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return Prelude.Map<K, V>();
        }

        Map<K, U> resU;
        try
        {
            resU = bind(resT.Value.Value);
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return Prelude.Map<K, V>();
        }

        try
        {
            return resU.Select(x => project(resT.Value.Value, x));
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return Prelude.Map<K, V>();
        }
    }

    public static TryOption<V> SelectMany<T, U, V>(
          this TryOption<T> self,
          Func<T, Option<U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryOptionResult<U> resU;
                try
                {
                    resU = bind(resT.Value.Value);
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                    if (resU.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static TryOption<V> SelectMany<T, U, V>(
          this TryOption<T> self,
          Func<T, OptionUnsafe<U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryOptionResult<U> resU;
                try
                {
                    var u = bind(resT.Value.Value);
                    resU = u.IsSome
                               ? new TryOptionResult<U>(u.Value)
                               : new TryOptionResult<U>(None);
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                    if (resU.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }


    public static TryOption<V> SelectMany<T, U, V>(
          this TryOption<T> self,
          Func<T, Try<U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryResult<U> resU;
                try
                {
                    resU = bind(resT.Value.Value).Try();
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static TryOption<V> SelectMany<L, T, U, V>(
          this TryOption<T> self,
          Func<T, Either<L,U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryOptionResult<U> resU;
                try
                {
                    var u = bind(resT.Value.Value);
                    resU = u.IsRight
                               ? new TryOptionResult<U>(u.RightValue)
                               : new TryOptionResult<U>(None);
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                    if (resU.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static TryOption<V> SelectMany<L, T, U, V>(
          this TryOption<T> self,
          Func<T, EitherUnsafe<L, U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                TryOptionResult<U> resU;
                try
                {
                    var u = bind(resT.Value.Value);
                    resU = u.IsRight
                               ? new TryOptionResult<U>(u.RightValue)
                               : new TryOptionResult<U>(None);
                    if (resU.IsFaulted)
                        return new TryOptionResult<V>(resU.Exception);
                    if (resU.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }


    public static TryOption<V> SelectMany<L, T, U, V>(
          this TryOption<T> self,
          Func<T, Reader<T, U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                ReaderResult<U> resU;
                try
                {
                    var u = bind(resT.Value.Value);
                    resU = u(resT.Value.Value);
                    if (resU.IsBottom)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static TryOption<V> SelectMany<Out, L, T, U, V>(
          this TryOption<T> self,
          Func<T, Writer<Out, U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                WriterResult<Out, U> resU;
                try
                {
                    resU = bind(resT.Value.Value)();
                    if (resU.IsBottom)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }

    public static TryOption<V> SelectMany<L, T, U, V>(
          this TryOption<T> self,
          Func<T, State<T, U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<V>(resT.Exception);
                    if (resT.Value.IsNone)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                StateResult<T, U> resU;
                try
                {
                    var u = bind(resT.Value.Value);
                    resU = u(resT.Value.Value);
                    if (resU.IsBottom)
                        return new TryOptionResult<V>(None);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                Option<V> resV;
                try
                {
                    resV = project(resT.Value.Value, resU.Value);
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }

                return new TryOptionResult<V>(resV);
            }
        );
    }
}