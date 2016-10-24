using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Instances
{
    public struct FOption<A, B> : 
        Functor<Option<A>, Option<B>, A, B>,
        BiFunctor<Option<A>, Option<B>, Unit, A, B>
    {
        public static readonly FOption<A, B> Inst = default(FOption<A, B>);

        public Option<B> BiMap(Option<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            ma.IsNone
                ? fa == null
                    ? Option<B>.None
                    : fa(unit)
                : fb == null
                    ? Option<B>.None
                    : fb(ma.Value);

        public Option<B> Map(Option<A> ma, Func<A, B> f) =>
            ma.IsSome && f != null
                ? Optional(f(ma.Value))
                : None;
    }
}
