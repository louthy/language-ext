﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Combines all the behaviours of the Reader, Writer and State monads into a single
    /// monadic computation.  
    /// </summary>
    /// <typeparam name="R">Reader type</typeparam>
    /// <typeparam name="W">Writer type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    /// <param name="rws">A tuple (environment, state) that represents the shared immutable
    /// environment and shared mutable state.</param>
    /// <returns>A value that represents the bound value as well as the shared environment,
    /// shared state, and shared outputs</returns>
    public delegate RwsResult<W, S, T> Rws<R, W, S, T>(Tuple<R, S> rws);

    /// <summary>
    /// RWS result.
    /// </summary>
    public struct RwsResult<W, S, T>
    {
        readonly T value;
        public readonly IEnumerable<W> Output;
        public readonly S State;
        public readonly bool IsBottom;

        internal RwsResult(IEnumerable<W> output, S state, T value, bool isBottom = false)
        {
            this.value = value;
            Output = output;
            State = state;
            IsBottom = isBottom;
        }

        [Pure]
        public T Value =>
            IsBottom
                ? default(T)
                : value;

        [Pure]
        public static implicit operator RwsResult<W, S, T>(T value) =>
           new RwsResult<W, S, T>(new W[0], default(S), value);        // TODO:  Not a good idea

        [Pure]
        public static implicit operator T(RwsResult<W, S, T> value) =>
           value.Value;

        public static readonly RwsResult<W, S, T> Bottom =
            new RwsResult<W, S, T>(new W[0], default(S), default(T), true);
    }
}

public static class RwsExtensions
{
    internal static Rws<R, W, S, T> Valid<R, W, S, T>(this Rws<R, W, S, T> self) =>
        self ?? (rs => RwsResult<W, S, T>.Bottom);

    [Pure]
    public static Rws<R, W, S, IEnumerable<T>> AsEnumerable<R, W, S, T>(this Rws<R, W, S, T> self) =>
        self.Select(x => (new T[1] { x }).AsEnumerable());

    [Pure]
    public static IEnumerable<T> AsEnumerable<R, W, S, T>(this Rws<R, W, S, T> self, R env, S state)
    {
        var res = self.Valid()(Tuple(env, state));
        if (!res.IsBottom)
        {
            yield return self(Tuple(env, res.State)).Value;
        }
    }

    public static Rws<R, W, S, Unit> Iter<R, W, S, T>(this Rws<R, W, S, T> self, Action<T> action) =>
        s => bmap(self.Valid()(s), action);

    [Pure]
    public static Rws<R, W, S, int> Count<R, W, S, T>(this Rws<R, W, S, T> self) =>
        s => self.Valid()(s).IsBottom
            ? 0
            : 1;

    [Pure]
    public static Rws<R, W, S, bool> ForAll<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
        self.Select(pred);

    [Pure]
    public static Rws<R, W, S, bool> Exists<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
        self.Select(pred);

    [Pure]
    public static Rws<R, W, S, FState> Fold<R, W, S, T, FState>(this Rws<R, W, S, T> self, FState state, Func<FState, T, FState> folder) =>
        s => bmap(self.Valid()(s), x => folder(state, x));

    [Pure]
    public static Rws<R, W, S, S> Fold<R, W, S, T>(this Rws<R, W, S, T> self, Func<S, T, S> folder) =>
        s => bmap(self.Valid()(s), x => folder(s.Item2, x));

    [Pure]
    public static Rws<R, W, S, Ret> Map<R, W, S, T, Ret>(this Rws<R, W, S, T> self, Func<T, Ret> mapper) =>
        self.Select(mapper);

    [Pure]
    public static Rws<R, W, S, Ret> Bind<R, W, S, T, Ret>(this Rws<R, W, S, T> self, Func<T, Rws<R, W, S, Ret>> binder)
    {
        if (binder == null) throw new ArgumentNullException(nameof(binder));
        return state =>
        {
            var resT = self.Valid()(state);
            if (resT.IsBottom)
            {
                return new RwsResult<W, S, Ret>(resT.Output, resT.State, default(Ret), true);
            }
            return binder(resT.Value).Valid()(Tuple(state.Item1, resT.State));
        };
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Rws<R, W, S, U> Select<R, W, S, T, U>(this Rws<R, W, S, T> self, Func<T, U> map)
    {
        if (map == null) throw new ArgumentNullException(nameof(map));
        return state =>
        {
            var resT = self.Valid()(state);
            return resT.IsBottom
                ? new RwsResult<W, S, U>(resT.Output, resT.State, default(U), true)
                : new RwsResult<W, S, U>(resT.Output, resT.State, map(resT.Value));
        };
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Rws<R, W, S, V> SelectMany<R, W, S, T, U, V>(
        this Rws<R, W, S, T> self,
        Func<T, Rws<R, W, S, U>> bind,
        Func<T, U, V> project
        )
    {
        if (bind == null) throw new ArgumentNullException(nameof(bind));
        if (project == null) throw new ArgumentNullException(nameof(project));

        return state =>
        {
            var resT = self.Valid()(state);
            if (resT.IsBottom) return new RwsResult<W, S, V>(resT.Output, resT.State, default(V), true);
            var resU = bind(resT.Value).Valid()(Tuple(state.Item1, resT.State));
            if (resU.IsBottom) return new RwsResult<W, S, V>(resU.Output, resU.State, default(V), true);
            var resV = project(resT.Value, resU.Value);
            return new RwsResult<W, S, V>(resT.Output.Concat(resU.Output), resU.State, resV);
        };
    }

    [Pure]
    public static Rws<R, W, S, T> Filter<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
        self.Where(pred);

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Rws<R, W, S, T> Where<R, W, S, T>(this Rws<R, W, S, T> self, Func<T, bool> pred) =>
        state =>
        {
            var res = self.Valid()(state);
            return new RwsResult<W, S, T>(res.Output, res.State, res.Value, !pred(res.Value));
        };

    [Pure]
    public static Rws<R, W, S, int> Sum<R, W, S>(this Rws<R, W, S, int> self) =>
        state => bmap(self.Valid()(state), x => x);

    [Pure]
    private static RwsResult<W, S, Ret> bmap<W, S, T, Ret>(RwsResult<W, S, T> r, Func<T, Ret> f) =>
        r.IsBottom
            ? new RwsResult<W, S, Ret>(r.Output, r.State, default(Ret), true)
            : new RwsResult<W, S, Ret>(r.Output, r.State, f(r.Value), false);

    [Pure]
    private static RwsResult<W, S, Unit> bmap<W, S, T>(RwsResult<W, S, T> r, Action<T> f)
    {
        if (r.IsBottom)
        {
            return new RwsResult<W, S, Unit>(r.Output, r.State, unit, true);
        }
        else
        {
            f(r.Value);
            return new RwsResult<W, S, Unit>(r.Output, r.State, unit, false);
        }
    }
}