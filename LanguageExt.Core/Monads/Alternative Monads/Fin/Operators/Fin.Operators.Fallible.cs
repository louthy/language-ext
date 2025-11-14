using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinExtensions
{
    extension<A>(Fin<A> self)
    {
        public static Fin<A> operator |(Fin<A> lhs, CatchM<Error, Fin, A> rhs) =>
            +lhs.Catch(rhs);

        public static Fin<A> operator |(Fin<A> lhs, Fail<Error> rhs) =>
            +lhs.Catch(rhs);

        public static Fin<A> operator |(Fin<A> lhs, Error rhs) =>
            +lhs.Catch(rhs);
    }    
    
    extension<A>(K<Fin, A> self)
    {
        public static Fin<A> operator |(K<Fin, A> lhs, CatchM<Error, Fin, A> rhs) =>
            +lhs.Catch(rhs);

        public static Fin<A> operator |(K<Fin, A> lhs, Fail<Error> rhs) =>
            +lhs.Catch(rhs);

        public static Fin<A> operator |(K<Fin, A> lhs, Error rhs) =>
            +lhs.Catch(rhs);
    }
}
