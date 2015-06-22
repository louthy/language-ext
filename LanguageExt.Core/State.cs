using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    public delegate StateResult<S, T> State<S, T>(S state);

    /// <summary>
    /// State result.
    /// </summary>
    public struct StateResult<S, T>
    {
        public readonly T Value;
        public readonly S State;

        internal StateResult(S state, T value)
        {
            Value = value;
            State = state;
        }
    }

    public static class StateExt
    {
        public static State<S, IEnumerable<T>> AsEnumerable<S, T>(this State<S, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        public static IEnumerable<T> AsEnumerable<S, T>(this State<S, T> self, S state)
        {
            yield return self(state).Value;
        }

        public static State<S, Unit> Iter<S, T>(this State<S, T> self, Action<T> action)
        {
            return s =>
            {
                action(self(s).Value);
                return new StateResult<S, Unit>(s,unit);
            };
        }

        public static int Count<S, T>(this State<S, T> self) =>
            1;

        public static State<S, bool> ForAll<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static State<S, bool> Exists<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static State<S, FState> Fold<S, T, FState>(this State<S, T> self, FState state, Func<FState, T, FState> folder) =>
            s => new StateResult<S, FState>(s,folder(state, self(s).Value));

        public static State<S, Unit> Fold<S, T>(this State<S, T> self, Func<S, T, S> folder) =>
            s => new StateResult<S, Unit>(folder(s, self(s).Value), unit);

        public static State<S, R> Map<S, T, R>(this State<S, T> self, Func<T, R> mapper) =>
            self.Select(mapper);

        public static State<S, R> Bind<S, T, R>(this State<S, T> self, Func<T, State<S, R>> binder) =>
            from x in self
            from y in binder(x)
            select y;

        public static State<S, U> Select<S, T, U>(this State<S, T> self, Func<T, U> map)
        {
            if (map == null) throw new ArgumentNullException("map");
            return (S state) =>
            {
                var resT = self(state);
                return new StateResult<S, U>(resT.State, map(resT.Value));
            };
        }

        public static State<S, V> SelectMany<S, T, U, V>(
            this State<S, T> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return (S state) =>
            {
                var resT = self(state);
                var resU = bind(resT.Value)(resT.State);
                var resV = project(resT.Value, resU.Value);
                return new StateResult<S, V>(resU.State, resV);
            };
        }

        public static State<S, T> Filter<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            env => failwith<StateResult<S, T>>("State doesn't support Where or Filter");

        public static State<S, T> Where<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            env => failwith<StateResult<S, T>>("State doesn't support Where or Filter");

    }
}
