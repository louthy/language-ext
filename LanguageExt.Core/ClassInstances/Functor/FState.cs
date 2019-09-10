using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FState<S, A, B> :
        Functor<State<S, A>, State<S, B>, A, B>
    {
        public static readonly FState<S, A, B> Inst = default;

        [Pure]
        public State<S, B> Map(State<S, A> ma, Func<A, B> f) =>
            MState<S, A>.Inst.Bind<MState<S, B>, State<S, B>, B>(ma, a =>
                MState<S, B>.Inst.Return(_ => f(a)));
    } 
}