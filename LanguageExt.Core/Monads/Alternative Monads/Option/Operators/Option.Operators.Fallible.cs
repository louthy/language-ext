using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class OptionExtensions
{
    extension<A>(Option<A> self)
    {
        public static Option<A> operator |(Option<A> lhs, CatchM<Unit, Option, A> rhs) =>
            +lhs.Catch(rhs);

        public static Option<A> operator |(Option<A> lhs, Fail<Unit> rhs) =>
            +lhs.Catch(rhs);

        public static Option<A> operator |(Option<A> lhs, Unit rhs) =>
            +lhs.Catch(rhs);
    }
    
    extension<A>(K<Option, A> self)
    {
        public static Option<A> operator |(K<Option, A> lhs, CatchM<Unit, Option, A> rhs) =>
            +lhs.Catch(rhs);

        public static Option<A> operator |(K<Option, A> lhs, Fail<Unit> rhs) =>
            +lhs.Catch(rhs);

        public static Option<A> operator |(K<Option, A> lhs, Unit rhs) =>
            +lhs.Catch(rhs);
    }
}
