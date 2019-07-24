using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplValidation<FAIL, SUCCESS, SUCCESS2> : 
        Functor<Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, SUCCESS, SUCCESS2>,
        BiFunctor<Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, Seq<FAIL>, SUCCESS, SUCCESS2>,
        Applicative<Validation<FAIL, Func<SUCCESS, SUCCESS2>>, Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, SUCCESS, SUCCESS2>
    {
        public static readonly ApplValidation<FAIL, SUCCESS, SUCCESS2> Inst = default(ApplValidation<FAIL, SUCCESS, SUCCESS2>);

        [Pure]
        public Validation<FAIL, SUCCESS2> BiMap(Validation<FAIL, SUCCESS> ma, Func<Seq<FAIL>, SUCCESS2> fa, Func<SUCCESS, SUCCESS2> fb) =>
            FValidation<FAIL, SUCCESS, SUCCESS2>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Validation<FAIL, SUCCESS2> Map(Validation<FAIL, SUCCESS> ma, Func<SUCCESS, SUCCESS2> f) =>
            FValidation<FAIL, SUCCESS, SUCCESS2>.Inst.Map(ma, f);

        [Pure]
        public Validation<FAIL, SUCCESS2> Apply(Validation<FAIL, Func<SUCCESS, SUCCESS2>> fab, Validation<FAIL, SUCCESS> fa) =>
            (fab, fa).Apply((f, a) => f(a));

        [Pure]
        public Validation<FAIL, SUCCESS2> Action(Validation<FAIL, SUCCESS> fa, Validation<FAIL, SUCCESS2> fb) =>
            (fa, fb).Apply((a, b) => b);

        [Pure]
        public Validation<FAIL, SUCCESS> Pure(SUCCESS x) =>
            Success<FAIL, SUCCESS>(x);
    }

    public struct ApplValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2> :
        BiFunctor<Validation<FAIL, SUCCESS>, Validation<FAIL2, SUCCESS2>, FAIL, SUCCESS, FAIL2, SUCCESS2>,
        Applicative<Validation<FAIL, Func<SUCCESS, SUCCESS2>>, Validation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS2>, SUCCESS, SUCCESS2>
    {
        public static readonly ApplValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2> Inst = default(ApplValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2>);

        [Pure]
        public Validation<FAIL2, SUCCESS2> BiMap(Validation<FAIL, SUCCESS> ma, Func<FAIL, FAIL2> fa, Func<SUCCESS, SUCCESS2> fb) =>
            ma.Match(
                Succ: b => Validation<FAIL2, SUCCESS2>.Success(Check.NullReturn(fb(b))),
                Fail: a => Validation<FAIL2, SUCCESS2>.Fail(Check.NullReturn(a.Map(fa))));

        [Pure]
        public Validation<FAIL, SUCCESS2> Apply(Validation<FAIL, Func<SUCCESS, SUCCESS2>> fab, Validation<FAIL, SUCCESS> fa) =>
            (fab, fa).Apply((f, a) => f(a));

        [Pure]
        public Validation<FAIL, SUCCESS2> Action(Validation<FAIL, SUCCESS> fa, Validation<FAIL, SUCCESS2> fb) =>
            (fa, fb).Apply((a, b) => b);

        [Pure]
        public Validation<FAIL, SUCCESS> Pure(SUCCESS x) =>
            Success<FAIL, SUCCESS>(x);
    }

    public struct ApplValidation<FAIL, A, B, C> :
        Applicative<Validation<FAIL, Func<A, Func<B, C>>>, Validation<FAIL, Func<B, C>>, Validation<FAIL, A>, Validation<FAIL, B>, Validation<FAIL, C>, A, B, C>
    {
        public static readonly ApplValidation<FAIL, A, B, C> Inst = default(ApplValidation<FAIL, A, B, C>);

        [Pure]
        public Validation<FAIL, Func<B, C>> Apply(Validation<FAIL, Func<A, Func<B, C>>> fab, Validation<FAIL, A> fa) =>
            (fab, fa).Apply((f, a) => f(a));

        [Pure]
        public Validation<FAIL, C> Apply(Validation<FAIL, Func<A, Func<B, C>>> fab, Validation<FAIL, A> fa, Validation<FAIL, B> fb) =>
            (fab, fa, fb).Apply((f, a, b) => f(a)(b));

        [Pure]
        public Validation<FAIL, A> Pure(A x) =>
            Success<FAIL, A>(x);
    }
}
