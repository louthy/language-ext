using System;
using System.Linq.Expressions;

namespace LanguageExt
{
    public static partial class Prelude
    {
        public static Func<R> fun<R>(Func<R> f) => f;
        public static Func<T1, R> fun<T1, R>(Func<T1, R> f) => f;
        public static Func<T1, T2, R> fun<T1, T2, R>(Func<T1, T2, R> f) => f;
        public static Func<T1, T2, T3, R> fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;
        public static Func<T1, T2, T3, T4, R> fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, R> fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, R> fun<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, R> fun<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => f;

        public static Func<Unit> fun(Action f) => () => { f(); return unit; };
        public static Func<T1, Unit> fun<T1>(Action<T1> f) => (a1) => { f(a1); return unit; };
        public static Func<T1, T2, Unit> fun<T1, T2>(Action<T1, T2> f) => (a1, a2) => { f(a1, a2); return unit; };
        public static Func<T1, T2, T3, Unit> fun<T1, T2, T3>(Action<T1, T2, T3> f) => (a1, a2, a3) => { f(a1, a2, a3); return unit; };
        public static Func<T1, T2, T3, T4, Unit> fun<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => (a1, a2, a3, a4) => { f(a1, a2, a3, a4); return unit; };
        public static Func<T1, T2, T3, T4, T5, Unit> fun<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => (a1, a2, a3, a4, a5) => { f(a1, a2, a3, a4, a5); return unit; };
        public static Func<T1, T2, T3, T4, T5, T6, Unit> fun<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => (a1, a2, a3, a4, a5, a6) => { f(a1, a2, a3, a4, a5, a6); return unit; };
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> fun<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => (a1, a2, a3, a4, a5, a6, a7) => { f(a1, a2, a3, a4, a5, a6, a7); return unit; };

        public static Action act(Action f) => f;
        public static Action<T1> act<T1>(Action<T1> f) => f;
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> f) => f;
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> f) => f;
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => f;
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => f;
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => f;
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => f;

        public static Action act<R>(Func<R> f) => () => f();
        public static Action<T1> act<T1, R>(Func<T1, R> f) => a1 => f(a1);
        public static Action<T1, T2> act<T1, T2, R>(Func<T1, T2, R> f) => (a1, a2) => f(a1, a2);
        public static Action<T1, T2, T3> act<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => (a1, a2, a3) => f(a1, a2, a3);
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => (a1, a2, a3, a4) => f(a1, a2, a3, a4);
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => (a1, a2, a3, a4, a5) => f(a1, a2, a3, a4, a5);
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => (a1, a2, a3, a4, a5, a6) => f(a1, a2, a3, a4, a5, a6);
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => (a1, a2, a3, a4, a5, a6, a7) => f(a1, a2, a3, a4, a5, a6, a7);

        public static Expression<Func<R>> expr<R>(Expression<Func<R>> f) => f;
        public static Expression<Func<T1, R>> expr<T1, R>(Expression<Func<T1, R>> f) => f;
        public static Expression<Func<T1, T2, R>> expr<T1, T2, R>(Expression<Func<T1, T2, R>> f) => f;
        public static Expression<Func<T1, T2, T3, R>> expr<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, R>> expr<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, R>> expr<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> expr<T1, T2, T3, T4, T5, T6, R>(Expression<Func<T1, T2, T3, T4, T5, T6, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> expr<T1, T2, T3, T4, T5, T6, T7, R>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> f) => f;
        public static Expression<Action> expr(Expression<Action> f) => f;
        public static Expression<Action<T1>> expr<T1>(Expression<Action<T1>> f) => f;
        public static Expression<Action<T1, T2>> expr<T1, T2>(Expression<Action<T1, T2>> f) => f;
        public static Expression<Action<T1, T2, T3>> expr<T1, T2, T3>(Expression<Action<T1, T2, T3>> f) => f;
        public static Expression<Action<T1, T2, T3, T4>> expr<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5>> expr<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> expr<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expr<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> f) => f;
    }
}
