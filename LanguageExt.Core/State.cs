using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
