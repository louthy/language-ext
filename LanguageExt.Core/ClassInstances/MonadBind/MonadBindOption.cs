using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static  LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MonadReturnOption<A> : Typeclass
    {
        public Option<A> Return(A value) => value;
    }

    public struct MonadFailOption<A> : Typeclass
    {
        public Option<A> Fail(Error error) => None;
    }

    public struct MonadBindOption<A, B> : Typeclass
    {
        public Option<B> Bind(Option<A> ma, Func<A, Option<B>> f) =>
            ma.IsSome
                ? f(ma.Value)
                : None;
    }
}
