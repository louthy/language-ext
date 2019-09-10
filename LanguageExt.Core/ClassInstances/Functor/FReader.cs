using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FReader<Env, A, B> :
        Functor<Reader<Env, A>, Reader<Env, B>, A, B>
    {
        public static readonly FReader<Env, A, B> Inst = default;

        [Pure]
        public Reader<Env, B> Map(Reader<Env, A> ma, Func<A, B> f) =>
            MReader<Env, A>.Inst.Bind<MReader<Env, B>, Reader<Env, B>, B>(ma, a =>
                MReader<Env, B>.Inst.Return(_ => f(a)));
    }
}
