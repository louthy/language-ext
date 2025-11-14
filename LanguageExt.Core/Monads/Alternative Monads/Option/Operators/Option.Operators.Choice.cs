using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class OptionExtensions
{
    extension<A>(Option<A> self)
    {
        public static Option<A> operator |(Option<A> lhs, Option<A> rhs) =>
            +lhs.Choose(rhs);

        public static Option<A> operator |(Option<A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToOption());
    }

    extension<A>(K<Option, A> self)
    {
        public static Option<A> operator |(K<Option, A> lhs, K<Option, A> rhs) =>
            +lhs.Choose(rhs);

        public static Option<A> operator |(K<Option, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(rhs.ToOption());
    }
}
