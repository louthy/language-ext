using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOption<A, B> : 
        Functor<Option<A>, Option<B>, A, B>,
        BiFunctor<Option<A>, Option<B>, Unit, A, B>,
        Applicative<Option<Func<A, B>>, Option<A>, Option<B>, A, B>
    {
        public static readonly FOption<A, B> Inst = default(FOption<A, B>);

        [Pure]
        public Option<B> BiMap(Option<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MOption<A>, MOption<B>, Option<A>, Option<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Option<B> Map(Option<A> ma, Func<A, B> f) =>
            FOptional<MOption<A>, MOption<B>, Option<A>, Option<B>, A, B>.Inst.Map(ma, f);

        [Pure]
        public Option<B> Apply(Option<Func<A, B>> fab, Option<A> fa) =>
            ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>
            .Inst.Apply(fab, fa);

        [Pure]
        public Option<A> Pure(A x) =>
            ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>
            .Inst.Pure(x);

        [Pure]
        public Option<B> Action(Option<A> fa, Option<B> fb) =>
            ApplOptional<
                MOption<Func<A, B>>, MOption<A>, MOption<B>, 
                Option<Func<A, B>>, Option<A>, Option<B>, 
                A, B>
            .Inst.Action(fa, fb);
    }

    public struct FOption<A, B, C> :
        Applicative<Option<Func<A, Func<B, C>>>, Option<Func<B, C>>, Option<A>, Option<B>, Option<C>, A, B, C>
    {
        public static readonly FOption<A, B, C> Inst = default(FOption<A, B, C>);

        [Pure]
        public Option<Func<B, C>> Apply(Option<Func<A, Func<B, C>>> fab, Option<A> fa) =>
            ApplOptional<
                MOption<Func<A, Func<B, C>>>, MOption<Func<B, C>>, MOption<A>, MOption<B>, MOption<C>, 
                Option<Func<A, Func<B, C>>>, Option<Func<B, C>>, Option<A>, Option<B>, Option<C>, 
                A, B, C>
            .Inst.Apply(fab, fa);

        [Pure]
        public Option<C> Apply(Option<Func<A, Func<B, C>>> fab, Option<A> fa, Option<B> fb) =>
            ApplOptional <
                MOption<Func<A, Func<B, C>>>, MOption<Func<B, C>>, MOption<A>, MOption<B>, MOption<C>, 
                Option<Func<A, Func<B, C>>>, Option<Func<B, C>>, Option<A>, Option<B>, Option<C>, 
                A, B, C>
            .Inst.Apply(fab, fa, fb);

        [Pure]
        public Option<A> Pure(A x) =>
            ApplOptional<
                MOption<Func<A, Func<B, C>>>, MOption<Func<B, C>>, MOption<A>, MOption<B>, MOption<C>,
                Option<Func<A, Func<B, C>>>, Option<Func<B, C>>, Option<A>, Option<B>, Option<C>,
                A, B, C>
            .Inst.Pure(x);
    }
}
