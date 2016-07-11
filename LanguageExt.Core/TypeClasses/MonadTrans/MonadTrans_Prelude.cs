using LanguageExt.TypeClasses;
using System;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        public static TMA lift<TMA, A>(Option<A> ma) 
            where TMA : struct, MonadT<Option<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

        public static TMA lift<TMA, A>(OptionUnsafe<A> ma)
            where TMA : struct, MonadT<OptionUnsafe<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

        public static TMA lift<TMA, A>(Lst<A> ma)
            where TMA : struct, MonadT<Lst<A>, A>
        {
            return (TMA)default(TMA).Return(ma);
        }

    }
}
