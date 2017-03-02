using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ArrArr
    {
        public static A SumT<NumA,  A>(this Arr<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Arr<A>> ma) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Arr<Arr<B>> BindT< A, B>(this Arr<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MArr<Arr<B>>, Arr<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Arr<Arr<B>> MapT< A, B>(this Arr<Arr<A>> ma, Func<A, B> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MArr<Arr<B>>, Arr<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Arr<A>> ma, Action<A> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MArr<Arr<A>>).Return(ma);

        public static Arr<Arr<A>> Lift< A>(A a) =>
            default(MArr<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Arr<Arr<A>> FilterT< A>(this Arr<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Arr<Arr<A>> Where< A>(this Arr<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Arr<Arr<B>> Select< A, B>(this Arr<Arr<A>> ma, Func<A, B> f) =>
            Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MArr<Arr<B>>, Arr<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Arr<Arr<C>> SelectMany< A, B, C>(
            this Arr<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Arr<A>>, Arr<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MArr<Arr<C>>, Arr<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrHashSet
    {
        public static A SumT<NumA,  A>(this Arr<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<HashSet<A>> ma) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Arr<HashSet<B>> BindT< A, B>(this Arr<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MArr<HashSet<B>>, Arr<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Arr<HashSet<B>> MapT< A, B>(this Arr<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MArr<HashSet<B>>, Arr<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<HashSet<A>> ma, Action<A> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MArr<HashSet<A>>).Return(ma);

        public static Arr<HashSet<A>> Lift< A>(A a) =>
            default(MArr<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Arr<HashSet<A>> FilterT< A>(this Arr<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Arr<HashSet<A>> Where< A>(this Arr<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Arr<HashSet<B>> Select< A, B>(this Arr<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MArr<HashSet<B>>, Arr<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Arr<HashSet<C>> SelectMany< A, B, C>(
            this Arr<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<HashSet<A>>, Arr<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MArr<HashSet<C>>, Arr<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrLst
    {
        public static A SumT<NumA,  A>(this Arr<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Lst<A>> ma) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Arr<Lst<B>> BindT< A, B>(this Arr<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MArr<Lst<B>>, Arr<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Arr<Lst<B>> MapT< A, B>(this Arr<Lst<A>> ma, Func<A, B> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MArr<Lst<B>>, Arr<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Lst<A>> ma, Action<A> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MArr<Lst<A>>).Return(ma);

        public static Arr<Lst<A>> Lift< A>(A a) =>
            default(MArr<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Arr<Lst<A>> FilterT< A>(this Arr<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Arr<Lst<A>> Where< A>(this Arr<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Arr<Lst<B>> Select< A, B>(this Arr<Lst<A>> ma, Func<A, B> f) =>
            Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MArr<Lst<B>>, Arr<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Arr<Lst<C>> SelectMany< A, B, C>(
            this Arr<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Lst<A>>, Arr<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MArr<Lst<C>>, Arr<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrOption
    {
        public static A SumT<NumA,  A>(this Arr<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Option<A>> ma) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Arr<Option<B>> BindT< A, B>(this Arr<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MArr<Option<B>>, Arr<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Arr<Option<B>> MapT< A, B>(this Arr<Option<A>> ma, Func<A, B> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MArr<Option<B>>, Arr<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Option<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Option<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Option<A>> ma, Action<A> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Option<A>> Lift< A>(Option<A> ma) =>
            default(MArr<Option<A>>).Return(ma);

        public static Arr<Option<A>> Lift< A>(A a) =>
            default(MArr<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Arr<Option<A>> FilterT< A>(this Arr<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Arr<Option<A>> Where< A>(this Arr<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Arr<Option<B>> Select< A, B>(this Arr<Option<A>> ma, Func<A, B> f) =>
            Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MArr<Option<B>>, Arr<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Arr<Option<C>> SelectMany< A, B, C>(
            this Arr<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Option<A>>, Arr<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MArr<Option<C>>, Arr<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Arr<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<OptionUnsafe<A>> ma) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Arr<OptionUnsafe<B>> BindT< A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MArr<OptionUnsafe<B>>, Arr<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Arr<OptionUnsafe<B>> MapT< A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MArr<OptionUnsafe<B>>, Arr<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MArr<OptionUnsafe<A>>).Return(ma);

        public static Arr<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MArr<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Arr<OptionUnsafe<A>> FilterT< A>(this Arr<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Arr<OptionUnsafe<A>> Where< A>(this Arr<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Arr<OptionUnsafe<B>> Select< A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MArr<OptionUnsafe<B>>, Arr<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Arr<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Arr<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<OptionUnsafe<A>>, Arr<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MArr<OptionUnsafe<C>>, Arr<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrEither
    {
        public static A SumT<NumA, L, A>(this Arr<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Arr<Either<L, A>> ma) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Arr<Either<L, B>> BindT<L, A, B>(this Arr<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MArr<Either<L, B>>, Arr<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Arr<Either<L, B>> MapT<L, A, B>(this Arr<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MArr<Either<L, B>>, Arr<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Arr<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Arr<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Arr<Either<L, A>> ma, Action<A> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MArr<Either<L, A>>).Return(ma);

        public static Arr<Either<L, A>> Lift<L, A>(A a) =>
            default(MArr<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Arr<Either<L, A>> FilterT<L, A>(this Arr<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Arr<Either<L, A>> Where<L, A>(this Arr<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Arr<Either<L, B>> Select<L, A, B>(this Arr<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MArr<Either<L, B>>, Arr<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Arr<Either<L, C>> SelectMany<L, A, B, C>(
            this Arr<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Either<L, A>>, Arr<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MArr<Either<L, C>>, Arr<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class ArrEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Arr<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Arr<EitherUnsafe<L, A>> ma) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Arr<EitherUnsafe<L, B>> BindT<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MArr<EitherUnsafe<L, B>>, Arr<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Arr<EitherUnsafe<L, B>> MapT<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MArr<EitherUnsafe<L, B>>, Arr<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MArr<EitherUnsafe<L, A>>).Return(ma);

        public static Arr<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MArr<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Arr<EitherUnsafe<L, A>> FilterT<L, A>(this Arr<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Arr<EitherUnsafe<L, A>> Where<L, A>(this Arr<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Arr<EitherUnsafe<L, B>> Select<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MArr<EitherUnsafe<L, B>>, Arr<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Arr<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Arr<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<EitherUnsafe<L, A>>, Arr<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MArr<EitherUnsafe<L, C>>, Arr<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class ArrTry
    {
        public static A SumT<NumA,  A>(this Arr<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Try<A>> ma) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Arr<Try<B>> BindT< A, B>(this Arr<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MArr<Try<B>>, Arr<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Arr<Try<B>> MapT< A, B>(this Arr<Try<A>> ma, Func<A, B> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MArr<Try<B>>, Arr<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Try<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Try<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Try<A>> ma, Action<A> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Try<A>> Lift< A>(Try<A> ma) =>
            default(MArr<Try<A>>).Return(ma);

        public static Arr<Try<A>> Lift< A>(A a) =>
            default(MArr<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Arr<Try<A>> FilterT< A>(this Arr<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Arr<Try<A>> Where< A>(this Arr<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Arr<Try<B>> Select< A, B>(this Arr<Try<A>> ma, Func<A, B> f) =>
            Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MArr<Try<B>>, Arr<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Arr<Try<C>> SelectMany< A, B, C>(
            this Arr<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Try<A>>, Arr<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MArr<Try<C>>, Arr<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrTryAsync
    {
        public static A SumT<NumA,  A>(this Arr<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<TryAsync<A>> ma) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Arr<TryAsync<B>> BindT< A, B>(this Arr<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MArr<TryAsync<B>>, Arr<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Arr<TryAsync<B>> MapT< A, B>(this Arr<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MArr<TryAsync<B>>, Arr<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<TryAsync<A>> ma, Action<A> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MArr<TryAsync<A>>).Return(ma);

        public static Arr<TryAsync<A>> Lift< A>(A a) =>
            default(MArr<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Arr<TryAsync<A>> FilterT< A>(this Arr<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Arr<TryAsync<A>> Where< A>(this Arr<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Arr<TryAsync<B>> Select< A, B>(this Arr<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MArr<TryAsync<B>>, Arr<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Arr<TryAsync<C>> SelectMany< A, B, C>(
            this Arr<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<TryAsync<A>>, Arr<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MArr<TryAsync<C>>, Arr<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrTryOption
    {
        public static A SumT<NumA,  A>(this Arr<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<TryOption<A>> ma) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Arr<TryOption<B>> BindT< A, B>(this Arr<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MArr<TryOption<B>>, Arr<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Arr<TryOption<B>> MapT< A, B>(this Arr<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MArr<TryOption<B>>, Arr<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<TryOption<A>> ma, Action<A> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MArr<TryOption<A>>).Return(ma);

        public static Arr<TryOption<A>> Lift< A>(A a) =>
            default(MArr<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Arr<TryOption<A>> FilterT< A>(this Arr<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Arr<TryOption<A>> Where< A>(this Arr<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Arr<TryOption<B>> Select< A, B>(this Arr<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MArr<TryOption<B>>, Arr<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Arr<TryOption<C>> SelectMany< A, B, C>(
            this Arr<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<TryOption<A>>, Arr<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MArr<TryOption<C>>, Arr<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Arr<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<TryOptionAsync<A>> ma) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Arr<TryOptionAsync<B>> BindT< A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MArr<TryOptionAsync<B>>, Arr<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Arr<TryOptionAsync<B>> MapT< A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MArr<TryOptionAsync<B>>, Arr<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MArr<TryOptionAsync<A>>).Return(ma);

        public static Arr<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MArr<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Arr<TryOptionAsync<A>> FilterT< A>(this Arr<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Arr<TryOptionAsync<A>> Where< A>(this Arr<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Arr<TryOptionAsync<B>> Select< A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MArr<TryOptionAsync<B>>, Arr<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Arr<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Arr<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<TryOptionAsync<A>>, Arr<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MArr<TryOptionAsync<C>>, Arr<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrQue
    {
        public static A SumT<NumA,  A>(this Arr<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Que<A>> ma) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Arr<Que<B>> BindT< A, B>(this Arr<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MArr<Que<B>>, Arr<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Arr<Que<B>> MapT< A, B>(this Arr<Que<A>> ma, Func<A, B> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MArr<Que<B>>, Arr<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Que<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Que<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Que<A>> ma, Action<A> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Que<A>> Lift< A>(Que<A> ma) =>
            default(MArr<Que<A>>).Return(ma);

        public static Arr<Que<A>> Lift< A>(A a) =>
            default(MArr<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Arr<Que<A>> FilterT< A>(this Arr<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Arr<Que<A>> Where< A>(this Arr<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Arr<Que<B>> Select< A, B>(this Arr<Que<A>> ma, Func<A, B> f) =>
            Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MArr<Que<B>>, Arr<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Arr<Que<C>> SelectMany< A, B, C>(
            this Arr<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Que<A>>, Arr<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MArr<Que<C>>, Arr<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrIEnumerable
    {
        public static A SumT<NumA,  A>(this Arr<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<IEnumerable<A>> ma) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Arr<IEnumerable<B>> BindT< A, B>(this Arr<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MArr<IEnumerable<B>>, Arr<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Arr<IEnumerable<B>> MapT< A, B>(this Arr<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MArr<IEnumerable<B>>, Arr<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MArr<IEnumerable<A>>).Return(ma);

        public static Arr<IEnumerable<A>> Lift< A>(A a) =>
            default(MArr<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Arr<IEnumerable<A>> FilterT< A>(this Arr<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Arr<IEnumerable<A>> Where< A>(this Arr<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Arr<IEnumerable<B>> Select< A, B>(this Arr<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MArr<IEnumerable<B>>, Arr<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Arr<IEnumerable<C>> SelectMany< A, B, C>(
            this Arr<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<IEnumerable<A>>, Arr<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MArr<IEnumerable<C>>, Arr<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrSet
    {
        public static A SumT<NumA,  A>(this Arr<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Set<A>> ma) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Arr<Set<B>> BindT< A, B>(this Arr<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MArr<Set<B>>, Arr<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Arr<Set<B>> MapT< A, B>(this Arr<Set<A>> ma, Func<A, B> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MArr<Set<B>>, Arr<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Set<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Set<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Set<A>> ma, Action<A> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Set<A>> Lift< A>(Set<A> ma) =>
            default(MArr<Set<A>>).Return(ma);

        public static Arr<Set<A>> Lift< A>(A a) =>
            default(MArr<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Arr<Set<A>> FilterT< A>(this Arr<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Arr<Set<A>> Where< A>(this Arr<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Arr<Set<B>> Select< A, B>(this Arr<Set<A>> ma, Func<A, B> f) =>
            Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MArr<Set<B>>, Arr<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Arr<Set<C>> SelectMany< A, B, C>(
            this Arr<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Set<A>>, Arr<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MArr<Set<C>>, Arr<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class ArrStck
    {
        public static A SumT<NumA,  A>(this Arr<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Arr<Stck<A>> ma) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Arr<Stck<B>> BindT< A, B>(this Arr<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MArr<Stck<B>>, Arr<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Arr<Stck<B>> MapT< A, B>(this Arr<Stck<A>> ma, Func<A, B> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MArr<Stck<B>>, Arr<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Arr<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Arr<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Arr<Stck<A>> ma, Action<A> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Arr<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MArr<Stck<A>>).Return(ma);

        public static Arr<Stck<A>> Lift< A>(A a) =>
            default(MArr<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Arr<Stck<A>> FilterT< A>(this Arr<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Arr<Stck<A>> Where< A>(this Arr<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Arr<Stck<B>> Select< A, B>(this Arr<Stck<A>> ma, Func<A, B> f) =>
            Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MArr<Stck<B>>, Arr<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Arr<Stck<C>> SelectMany< A, B, C>(
            this Arr<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MArr<Stck<A>>, Arr<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MArr<Stck<C>>, Arr<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetArr
    {
        public static A SumT<NumA,  A>(this HashSet<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Arr<A>> ma) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static HashSet<Arr<B>> BindT< A, B>(this HashSet<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MHashSet<Arr<B>>, HashSet<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static HashSet<Arr<B>> MapT< A, B>(this HashSet<Arr<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MHashSet<Arr<B>>, HashSet<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Arr<A>> ma, Action<A> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MHashSet<Arr<A>>).Return(ma);

        public static HashSet<Arr<A>> Lift< A>(A a) =>
            default(MHashSet<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static HashSet<Arr<A>> FilterT< A>(this HashSet<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static HashSet<Arr<A>> Where< A>(this HashSet<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static HashSet<Arr<B>> Select< A, B>(this HashSet<Arr<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MHashSet<Arr<B>>, HashSet<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static HashSet<Arr<C>> SelectMany< A, B, C>(
            this HashSet<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Arr<A>>, HashSet<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MHashSet<Arr<C>>, HashSet<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetHashSet
    {
        public static A SumT<NumA,  A>(this HashSet<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<HashSet<A>> ma) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static HashSet<HashSet<B>> BindT< A, B>(this HashSet<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MHashSet<HashSet<B>>, HashSet<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static HashSet<HashSet<B>> MapT< A, B>(this HashSet<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MHashSet<HashSet<B>>, HashSet<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<HashSet<A>> ma, Action<A> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MHashSet<HashSet<A>>).Return(ma);

        public static HashSet<HashSet<A>> Lift< A>(A a) =>
            default(MHashSet<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static HashSet<HashSet<A>> FilterT< A>(this HashSet<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static HashSet<HashSet<A>> Where< A>(this HashSet<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static HashSet<HashSet<B>> Select< A, B>(this HashSet<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MHashSet<HashSet<B>>, HashSet<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static HashSet<HashSet<C>> SelectMany< A, B, C>(
            this HashSet<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<HashSet<A>>, HashSet<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MHashSet<HashSet<C>>, HashSet<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetLst
    {
        public static A SumT<NumA,  A>(this HashSet<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Lst<A>> ma) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static HashSet<Lst<B>> BindT< A, B>(this HashSet<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MHashSet<Lst<B>>, HashSet<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static HashSet<Lst<B>> MapT< A, B>(this HashSet<Lst<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MHashSet<Lst<B>>, HashSet<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Lst<A>> ma, Action<A> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MHashSet<Lst<A>>).Return(ma);

        public static HashSet<Lst<A>> Lift< A>(A a) =>
            default(MHashSet<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static HashSet<Lst<A>> FilterT< A>(this HashSet<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static HashSet<Lst<A>> Where< A>(this HashSet<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static HashSet<Lst<B>> Select< A, B>(this HashSet<Lst<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MHashSet<Lst<B>>, HashSet<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static HashSet<Lst<C>> SelectMany< A, B, C>(
            this HashSet<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Lst<A>>, HashSet<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MHashSet<Lst<C>>, HashSet<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetOption
    {
        public static A SumT<NumA,  A>(this HashSet<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Option<A>> ma) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static HashSet<Option<B>> BindT< A, B>(this HashSet<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MHashSet<Option<B>>, HashSet<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static HashSet<Option<B>> MapT< A, B>(this HashSet<Option<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MHashSet<Option<B>>, HashSet<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Option<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Option<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Option<A>> ma, Action<A> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Option<A>> Lift< A>(Option<A> ma) =>
            default(MHashSet<Option<A>>).Return(ma);

        public static HashSet<Option<A>> Lift< A>(A a) =>
            default(MHashSet<Option<A>>).Return(default(MOption<A>).Return(a));

        public static HashSet<Option<A>> FilterT< A>(this HashSet<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static HashSet<Option<A>> Where< A>(this HashSet<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static HashSet<Option<B>> Select< A, B>(this HashSet<Option<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MHashSet<Option<B>>, HashSet<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static HashSet<Option<C>> SelectMany< A, B, C>(
            this HashSet<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Option<A>>, HashSet<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MHashSet<Option<C>>, HashSet<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetOptionUnsafe
    {
        public static A SumT<NumA,  A>(this HashSet<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<OptionUnsafe<A>> ma) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static HashSet<OptionUnsafe<B>> BindT< A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MHashSet<OptionUnsafe<B>>, HashSet<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static HashSet<OptionUnsafe<B>> MapT< A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MHashSet<OptionUnsafe<B>>, HashSet<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MHashSet<OptionUnsafe<A>>).Return(ma);

        public static HashSet<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MHashSet<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static HashSet<OptionUnsafe<A>> FilterT< A>(this HashSet<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static HashSet<OptionUnsafe<A>> Where< A>(this HashSet<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static HashSet<OptionUnsafe<B>> Select< A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MHashSet<OptionUnsafe<B>>, HashSet<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static HashSet<OptionUnsafe<C>> SelectMany< A, B, C>(
            this HashSet<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<OptionUnsafe<A>>, HashSet<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MHashSet<OptionUnsafe<C>>, HashSet<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetEither
    {
        public static A SumT<NumA, L, A>(this HashSet<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this HashSet<Either<L, A>> ma) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static HashSet<Either<L, B>> BindT<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MHashSet<Either<L, B>>, HashSet<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static HashSet<Either<L, B>> MapT<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MHashSet<Either<L, B>>, HashSet<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this HashSet<Either<L, A>> ma, Action<A> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MHashSet<Either<L, A>>).Return(ma);

        public static HashSet<Either<L, A>> Lift<L, A>(A a) =>
            default(MHashSet<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static HashSet<Either<L, A>> FilterT<L, A>(this HashSet<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static HashSet<Either<L, A>> Where<L, A>(this HashSet<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static HashSet<Either<L, B>> Select<L, A, B>(this HashSet<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MHashSet<Either<L, B>>, HashSet<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static HashSet<Either<L, C>> SelectMany<L, A, B, C>(
            this HashSet<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Either<L, A>>, HashSet<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MHashSet<Either<L, C>>, HashSet<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this HashSet<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this HashSet<EitherUnsafe<L, A>> ma) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static HashSet<EitherUnsafe<L, B>> BindT<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MHashSet<EitherUnsafe<L, B>>, HashSet<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static HashSet<EitherUnsafe<L, B>> MapT<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MHashSet<EitherUnsafe<L, B>>, HashSet<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MHashSet<EitherUnsafe<L, A>>).Return(ma);

        public static HashSet<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MHashSet<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static HashSet<EitherUnsafe<L, A>> FilterT<L, A>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static HashSet<EitherUnsafe<L, A>> Where<L, A>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static HashSet<EitherUnsafe<L, B>> Select<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MHashSet<EitherUnsafe<L, B>>, HashSet<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static HashSet<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this HashSet<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<EitherUnsafe<L, A>>, HashSet<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MHashSet<EitherUnsafe<L, C>>, HashSet<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetTry
    {
        public static A SumT<NumA,  A>(this HashSet<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Try<A>> ma) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static HashSet<Try<B>> BindT< A, B>(this HashSet<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MHashSet<Try<B>>, HashSet<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static HashSet<Try<B>> MapT< A, B>(this HashSet<Try<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MHashSet<Try<B>>, HashSet<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Try<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Try<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Try<A>> ma, Action<A> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Try<A>> Lift< A>(Try<A> ma) =>
            default(MHashSet<Try<A>>).Return(ma);

        public static HashSet<Try<A>> Lift< A>(A a) =>
            default(MHashSet<Try<A>>).Return(default(MTry<A>).Return(a));

        public static HashSet<Try<A>> FilterT< A>(this HashSet<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static HashSet<Try<A>> Where< A>(this HashSet<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static HashSet<Try<B>> Select< A, B>(this HashSet<Try<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MHashSet<Try<B>>, HashSet<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static HashSet<Try<C>> SelectMany< A, B, C>(
            this HashSet<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Try<A>>, HashSet<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MHashSet<Try<C>>, HashSet<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetTryAsync
    {
        public static A SumT<NumA,  A>(this HashSet<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<TryAsync<A>> ma) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static HashSet<TryAsync<B>> BindT< A, B>(this HashSet<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MHashSet<TryAsync<B>>, HashSet<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static HashSet<TryAsync<B>> MapT< A, B>(this HashSet<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MHashSet<TryAsync<B>>, HashSet<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<TryAsync<A>> ma, Action<A> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MHashSet<TryAsync<A>>).Return(ma);

        public static HashSet<TryAsync<A>> Lift< A>(A a) =>
            default(MHashSet<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static HashSet<TryAsync<A>> FilterT< A>(this HashSet<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static HashSet<TryAsync<A>> Where< A>(this HashSet<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static HashSet<TryAsync<B>> Select< A, B>(this HashSet<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MHashSet<TryAsync<B>>, HashSet<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static HashSet<TryAsync<C>> SelectMany< A, B, C>(
            this HashSet<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<TryAsync<A>>, HashSet<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MHashSet<TryAsync<C>>, HashSet<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetTryOption
    {
        public static A SumT<NumA,  A>(this HashSet<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<TryOption<A>> ma) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static HashSet<TryOption<B>> BindT< A, B>(this HashSet<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MHashSet<TryOption<B>>, HashSet<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static HashSet<TryOption<B>> MapT< A, B>(this HashSet<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MHashSet<TryOption<B>>, HashSet<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<TryOption<A>> ma, Action<A> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MHashSet<TryOption<A>>).Return(ma);

        public static HashSet<TryOption<A>> Lift< A>(A a) =>
            default(MHashSet<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static HashSet<TryOption<A>> FilterT< A>(this HashSet<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static HashSet<TryOption<A>> Where< A>(this HashSet<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static HashSet<TryOption<B>> Select< A, B>(this HashSet<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MHashSet<TryOption<B>>, HashSet<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static HashSet<TryOption<C>> SelectMany< A, B, C>(
            this HashSet<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<TryOption<A>>, HashSet<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MHashSet<TryOption<C>>, HashSet<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetTryOptionAsync
    {
        public static A SumT<NumA,  A>(this HashSet<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<TryOptionAsync<A>> ma) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static HashSet<TryOptionAsync<B>> BindT< A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MHashSet<TryOptionAsync<B>>, HashSet<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static HashSet<TryOptionAsync<B>> MapT< A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MHashSet<TryOptionAsync<B>>, HashSet<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MHashSet<TryOptionAsync<A>>).Return(ma);

        public static HashSet<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MHashSet<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static HashSet<TryOptionAsync<A>> FilterT< A>(this HashSet<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static HashSet<TryOptionAsync<A>> Where< A>(this HashSet<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static HashSet<TryOptionAsync<B>> Select< A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MHashSet<TryOptionAsync<B>>, HashSet<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static HashSet<TryOptionAsync<C>> SelectMany< A, B, C>(
            this HashSet<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<TryOptionAsync<A>>, HashSet<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MHashSet<TryOptionAsync<C>>, HashSet<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetQue
    {
        public static A SumT<NumA,  A>(this HashSet<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Que<A>> ma) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static HashSet<Que<B>> BindT< A, B>(this HashSet<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MHashSet<Que<B>>, HashSet<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static HashSet<Que<B>> MapT< A, B>(this HashSet<Que<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MHashSet<Que<B>>, HashSet<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Que<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Que<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Que<A>> ma, Action<A> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Que<A>> Lift< A>(Que<A> ma) =>
            default(MHashSet<Que<A>>).Return(ma);

        public static HashSet<Que<A>> Lift< A>(A a) =>
            default(MHashSet<Que<A>>).Return(default(MQue<A>).Return(a));

        public static HashSet<Que<A>> FilterT< A>(this HashSet<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static HashSet<Que<A>> Where< A>(this HashSet<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static HashSet<Que<B>> Select< A, B>(this HashSet<Que<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MHashSet<Que<B>>, HashSet<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static HashSet<Que<C>> SelectMany< A, B, C>(
            this HashSet<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Que<A>>, HashSet<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MHashSet<Que<C>>, HashSet<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetIEnumerable
    {
        public static A SumT<NumA,  A>(this HashSet<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<IEnumerable<A>> ma) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static HashSet<IEnumerable<B>> BindT< A, B>(this HashSet<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MHashSet<IEnumerable<B>>, HashSet<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static HashSet<IEnumerable<B>> MapT< A, B>(this HashSet<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MHashSet<IEnumerable<B>>, HashSet<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MHashSet<IEnumerable<A>>).Return(ma);

        public static HashSet<IEnumerable<A>> Lift< A>(A a) =>
            default(MHashSet<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static HashSet<IEnumerable<A>> FilterT< A>(this HashSet<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static HashSet<IEnumerable<A>> Where< A>(this HashSet<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static HashSet<IEnumerable<B>> Select< A, B>(this HashSet<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MHashSet<IEnumerable<B>>, HashSet<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static HashSet<IEnumerable<C>> SelectMany< A, B, C>(
            this HashSet<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<IEnumerable<A>>, HashSet<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MHashSet<IEnumerable<C>>, HashSet<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetSet
    {
        public static A SumT<NumA,  A>(this HashSet<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Set<A>> ma) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static HashSet<Set<B>> BindT< A, B>(this HashSet<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MHashSet<Set<B>>, HashSet<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static HashSet<Set<B>> MapT< A, B>(this HashSet<Set<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MHashSet<Set<B>>, HashSet<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Set<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Set<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Set<A>> ma, Action<A> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Set<A>> Lift< A>(Set<A> ma) =>
            default(MHashSet<Set<A>>).Return(ma);

        public static HashSet<Set<A>> Lift< A>(A a) =>
            default(MHashSet<Set<A>>).Return(default(MSet<A>).Return(a));

        public static HashSet<Set<A>> FilterT< A>(this HashSet<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static HashSet<Set<A>> Where< A>(this HashSet<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static HashSet<Set<B>> Select< A, B>(this HashSet<Set<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MHashSet<Set<B>>, HashSet<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static HashSet<Set<C>> SelectMany< A, B, C>(
            this HashSet<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Set<A>>, HashSet<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MHashSet<Set<C>>, HashSet<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class HashSetStck
    {
        public static A SumT<NumA,  A>(this HashSet<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this HashSet<Stck<A>> ma) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static HashSet<Stck<B>> BindT< A, B>(this HashSet<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MHashSet<Stck<B>>, HashSet<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static HashSet<Stck<B>> MapT< A, B>(this HashSet<Stck<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MHashSet<Stck<B>>, HashSet<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this HashSet<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this HashSet<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this HashSet<Stck<A>> ma, Action<A> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static HashSet<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MHashSet<Stck<A>>).Return(ma);

        public static HashSet<Stck<A>> Lift< A>(A a) =>
            default(MHashSet<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static HashSet<Stck<A>> FilterT< A>(this HashSet<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static HashSet<Stck<A>> Where< A>(this HashSet<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static HashSet<Stck<B>> Select< A, B>(this HashSet<Stck<A>> ma, Func<A, B> f) =>
            Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MHashSet<Stck<B>>, HashSet<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static HashSet<Stck<C>> SelectMany< A, B, C>(
            this HashSet<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MHashSet<Stck<A>>, HashSet<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MHashSet<Stck<C>>, HashSet<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class LstArr
    {
        public static A SumT<NumA,  A>(this Lst<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Arr<A>> ma) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Lst<Arr<B>> BindT< A, B>(this Lst<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MLst<Arr<B>>, Lst<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Lst<Arr<B>> MapT< A, B>(this Lst<Arr<A>> ma, Func<A, B> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MLst<Arr<B>>, Lst<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Arr<A>> ma, Action<A> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MLst<Arr<A>>).Return(ma);

        public static Lst<Arr<A>> Lift< A>(A a) =>
            default(MLst<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Lst<Arr<A>> FilterT< A>(this Lst<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Lst<Arr<A>> Where< A>(this Lst<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Lst<Arr<B>> Select< A, B>(this Lst<Arr<A>> ma, Func<A, B> f) =>
            Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MLst<Arr<B>>, Lst<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Lst<Arr<C>> SelectMany< A, B, C>(
            this Lst<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Arr<A>>, Lst<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MLst<Arr<C>>, Lst<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class LstHashSet
    {
        public static A SumT<NumA,  A>(this Lst<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<HashSet<A>> ma) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Lst<HashSet<B>> BindT< A, B>(this Lst<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MLst<HashSet<B>>, Lst<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Lst<HashSet<B>> MapT< A, B>(this Lst<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MLst<HashSet<B>>, Lst<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<HashSet<A>> ma, Action<A> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MLst<HashSet<A>>).Return(ma);

        public static Lst<HashSet<A>> Lift< A>(A a) =>
            default(MLst<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Lst<HashSet<A>> FilterT< A>(this Lst<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Lst<HashSet<A>> Where< A>(this Lst<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Lst<HashSet<B>> Select< A, B>(this Lst<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MLst<HashSet<B>>, Lst<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Lst<HashSet<C>> SelectMany< A, B, C>(
            this Lst<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<HashSet<A>>, Lst<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MLst<HashSet<C>>, Lst<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class LstLst
    {
        public static A SumT<NumA,  A>(this Lst<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Lst<A>> ma) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Lst<Lst<B>> BindT< A, B>(this Lst<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MLst<Lst<B>>, Lst<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Lst<Lst<B>> MapT< A, B>(this Lst<Lst<A>> ma, Func<A, B> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MLst<Lst<B>>, Lst<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Lst<A>> ma, Action<A> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MLst<Lst<A>>).Return(ma);

        public static Lst<Lst<A>> Lift< A>(A a) =>
            default(MLst<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Lst<Lst<A>> FilterT< A>(this Lst<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Lst<Lst<A>> Where< A>(this Lst<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Lst<Lst<B>> Select< A, B>(this Lst<Lst<A>> ma, Func<A, B> f) =>
            Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MLst<Lst<B>>, Lst<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Lst<Lst<C>> SelectMany< A, B, C>(
            this Lst<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Lst<A>>, Lst<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MLst<Lst<C>>, Lst<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class LstOption
    {
        public static A SumT<NumA,  A>(this Lst<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Option<A>> ma) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Lst<Option<B>> BindT< A, B>(this Lst<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MLst<Option<B>>, Lst<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Lst<Option<B>> MapT< A, B>(this Lst<Option<A>> ma, Func<A, B> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MLst<Option<B>>, Lst<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Option<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Option<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Option<A>> ma, Action<A> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Option<A>> Lift< A>(Option<A> ma) =>
            default(MLst<Option<A>>).Return(ma);

        public static Lst<Option<A>> Lift< A>(A a) =>
            default(MLst<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Lst<Option<A>> FilterT< A>(this Lst<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Lst<Option<A>> Where< A>(this Lst<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Lst<Option<B>> Select< A, B>(this Lst<Option<A>> ma, Func<A, B> f) =>
            Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MLst<Option<B>>, Lst<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Lst<Option<C>> SelectMany< A, B, C>(
            this Lst<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Option<A>>, Lst<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MLst<Option<C>>, Lst<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class LstOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Lst<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<OptionUnsafe<A>> ma) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Lst<OptionUnsafe<B>> BindT< A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MLst<OptionUnsafe<B>>, Lst<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Lst<OptionUnsafe<B>> MapT< A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MLst<OptionUnsafe<B>>, Lst<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MLst<OptionUnsafe<A>>).Return(ma);

        public static Lst<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MLst<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Lst<OptionUnsafe<A>> FilterT< A>(this Lst<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Lst<OptionUnsafe<A>> Where< A>(this Lst<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Lst<OptionUnsafe<B>> Select< A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MLst<OptionUnsafe<B>>, Lst<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Lst<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Lst<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<OptionUnsafe<A>>, Lst<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MLst<OptionUnsafe<C>>, Lst<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class LstEither
    {
        public static A SumT<NumA, L, A>(this Lst<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Lst<Either<L, A>> ma) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Lst<Either<L, B>> BindT<L, A, B>(this Lst<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MLst<Either<L, B>>, Lst<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Lst<Either<L, B>> MapT<L, A, B>(this Lst<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MLst<Either<L, B>>, Lst<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Lst<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Lst<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Lst<Either<L, A>> ma, Action<A> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MLst<Either<L, A>>).Return(ma);

        public static Lst<Either<L, A>> Lift<L, A>(A a) =>
            default(MLst<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Lst<Either<L, A>> FilterT<L, A>(this Lst<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Lst<Either<L, A>> Where<L, A>(this Lst<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Lst<Either<L, B>> Select<L, A, B>(this Lst<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MLst<Either<L, B>>, Lst<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Lst<Either<L, C>> SelectMany<L, A, B, C>(
            this Lst<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Either<L, A>>, Lst<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MLst<Either<L, C>>, Lst<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class LstEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Lst<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Lst<EitherUnsafe<L, A>> ma) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Lst<EitherUnsafe<L, B>> BindT<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MLst<EitherUnsafe<L, B>>, Lst<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Lst<EitherUnsafe<L, B>> MapT<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MLst<EitherUnsafe<L, B>>, Lst<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MLst<EitherUnsafe<L, A>>).Return(ma);

        public static Lst<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MLst<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Lst<EitherUnsafe<L, A>> FilterT<L, A>(this Lst<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Lst<EitherUnsafe<L, A>> Where<L, A>(this Lst<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Lst<EitherUnsafe<L, B>> Select<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MLst<EitherUnsafe<L, B>>, Lst<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Lst<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Lst<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<EitherUnsafe<L, A>>, Lst<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MLst<EitherUnsafe<L, C>>, Lst<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class LstTry
    {
        public static A SumT<NumA,  A>(this Lst<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Try<A>> ma) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Lst<Try<B>> BindT< A, B>(this Lst<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MLst<Try<B>>, Lst<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Lst<Try<B>> MapT< A, B>(this Lst<Try<A>> ma, Func<A, B> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MLst<Try<B>>, Lst<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Try<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Try<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Try<A>> ma, Action<A> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Try<A>> Lift< A>(Try<A> ma) =>
            default(MLst<Try<A>>).Return(ma);

        public static Lst<Try<A>> Lift< A>(A a) =>
            default(MLst<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Lst<Try<A>> FilterT< A>(this Lst<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Lst<Try<A>> Where< A>(this Lst<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Lst<Try<B>> Select< A, B>(this Lst<Try<A>> ma, Func<A, B> f) =>
            Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MLst<Try<B>>, Lst<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Lst<Try<C>> SelectMany< A, B, C>(
            this Lst<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Try<A>>, Lst<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MLst<Try<C>>, Lst<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class LstTryAsync
    {
        public static A SumT<NumA,  A>(this Lst<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<TryAsync<A>> ma) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Lst<TryAsync<B>> BindT< A, B>(this Lst<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MLst<TryAsync<B>>, Lst<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Lst<TryAsync<B>> MapT< A, B>(this Lst<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MLst<TryAsync<B>>, Lst<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<TryAsync<A>> ma, Action<A> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MLst<TryAsync<A>>).Return(ma);

        public static Lst<TryAsync<A>> Lift< A>(A a) =>
            default(MLst<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Lst<TryAsync<A>> FilterT< A>(this Lst<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Lst<TryAsync<A>> Where< A>(this Lst<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Lst<TryAsync<B>> Select< A, B>(this Lst<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MLst<TryAsync<B>>, Lst<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Lst<TryAsync<C>> SelectMany< A, B, C>(
            this Lst<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<TryAsync<A>>, Lst<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MLst<TryAsync<C>>, Lst<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class LstTryOption
    {
        public static A SumT<NumA,  A>(this Lst<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<TryOption<A>> ma) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Lst<TryOption<B>> BindT< A, B>(this Lst<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MLst<TryOption<B>>, Lst<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Lst<TryOption<B>> MapT< A, B>(this Lst<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MLst<TryOption<B>>, Lst<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<TryOption<A>> ma, Action<A> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MLst<TryOption<A>>).Return(ma);

        public static Lst<TryOption<A>> Lift< A>(A a) =>
            default(MLst<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Lst<TryOption<A>> FilterT< A>(this Lst<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Lst<TryOption<A>> Where< A>(this Lst<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Lst<TryOption<B>> Select< A, B>(this Lst<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MLst<TryOption<B>>, Lst<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Lst<TryOption<C>> SelectMany< A, B, C>(
            this Lst<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<TryOption<A>>, Lst<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MLst<TryOption<C>>, Lst<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class LstTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Lst<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<TryOptionAsync<A>> ma) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Lst<TryOptionAsync<B>> BindT< A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MLst<TryOptionAsync<B>>, Lst<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Lst<TryOptionAsync<B>> MapT< A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MLst<TryOptionAsync<B>>, Lst<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MLst<TryOptionAsync<A>>).Return(ma);

        public static Lst<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MLst<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Lst<TryOptionAsync<A>> FilterT< A>(this Lst<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Lst<TryOptionAsync<A>> Where< A>(this Lst<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Lst<TryOptionAsync<B>> Select< A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MLst<TryOptionAsync<B>>, Lst<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Lst<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Lst<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<TryOptionAsync<A>>, Lst<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MLst<TryOptionAsync<C>>, Lst<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class LstQue
    {
        public static A SumT<NumA,  A>(this Lst<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Que<A>> ma) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Lst<Que<B>> BindT< A, B>(this Lst<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MLst<Que<B>>, Lst<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Lst<Que<B>> MapT< A, B>(this Lst<Que<A>> ma, Func<A, B> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MLst<Que<B>>, Lst<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Que<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Que<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Que<A>> ma, Action<A> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Que<A>> Lift< A>(Que<A> ma) =>
            default(MLst<Que<A>>).Return(ma);

        public static Lst<Que<A>> Lift< A>(A a) =>
            default(MLst<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Lst<Que<A>> FilterT< A>(this Lst<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Lst<Que<A>> Where< A>(this Lst<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Lst<Que<B>> Select< A, B>(this Lst<Que<A>> ma, Func<A, B> f) =>
            Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MLst<Que<B>>, Lst<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Lst<Que<C>> SelectMany< A, B, C>(
            this Lst<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Que<A>>, Lst<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MLst<Que<C>>, Lst<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class LstIEnumerable
    {
        public static A SumT<NumA,  A>(this Lst<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<IEnumerable<A>> ma) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Lst<IEnumerable<B>> BindT< A, B>(this Lst<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MLst<IEnumerable<B>>, Lst<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Lst<IEnumerable<B>> MapT< A, B>(this Lst<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MLst<IEnumerable<B>>, Lst<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MLst<IEnumerable<A>>).Return(ma);

        public static Lst<IEnumerable<A>> Lift< A>(A a) =>
            default(MLst<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Lst<IEnumerable<A>> FilterT< A>(this Lst<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Lst<IEnumerable<A>> Where< A>(this Lst<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Lst<IEnumerable<B>> Select< A, B>(this Lst<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MLst<IEnumerable<B>>, Lst<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Lst<IEnumerable<C>> SelectMany< A, B, C>(
            this Lst<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<IEnumerable<A>>, Lst<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MLst<IEnumerable<C>>, Lst<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class LstSet
    {
        public static A SumT<NumA,  A>(this Lst<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Set<A>> ma) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Lst<Set<B>> BindT< A, B>(this Lst<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MLst<Set<B>>, Lst<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Lst<Set<B>> MapT< A, B>(this Lst<Set<A>> ma, Func<A, B> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MLst<Set<B>>, Lst<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Set<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Set<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Set<A>> ma, Action<A> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Set<A>> Lift< A>(Set<A> ma) =>
            default(MLst<Set<A>>).Return(ma);

        public static Lst<Set<A>> Lift< A>(A a) =>
            default(MLst<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Lst<Set<A>> FilterT< A>(this Lst<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Lst<Set<A>> Where< A>(this Lst<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Lst<Set<B>> Select< A, B>(this Lst<Set<A>> ma, Func<A, B> f) =>
            Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MLst<Set<B>>, Lst<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Lst<Set<C>> SelectMany< A, B, C>(
            this Lst<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Set<A>>, Lst<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MLst<Set<C>>, Lst<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class LstStck
    {
        public static A SumT<NumA,  A>(this Lst<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Lst<Stck<A>> ma) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Lst<Stck<B>> BindT< A, B>(this Lst<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MLst<Stck<B>>, Lst<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Lst<Stck<B>> MapT< A, B>(this Lst<Stck<A>> ma, Func<A, B> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MLst<Stck<B>>, Lst<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Lst<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Lst<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Lst<Stck<A>> ma, Action<A> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Lst<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MLst<Stck<A>>).Return(ma);

        public static Lst<Stck<A>> Lift< A>(A a) =>
            default(MLst<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Lst<Stck<A>> FilterT< A>(this Lst<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Lst<Stck<A>> Where< A>(this Lst<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Lst<Stck<B>> Select< A, B>(this Lst<Stck<A>> ma, Func<A, B> f) =>
            Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MLst<Stck<B>>, Lst<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Lst<Stck<C>> SelectMany< A, B, C>(
            this Lst<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MLst<Stck<A>>, Lst<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MLst<Stck<C>>, Lst<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionArr
    {
        public static A SumT<NumA,  A>(this Option<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Arr<A>> ma) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Option<Arr<B>> BindT< A, B>(this Option<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOption<Arr<B>>, Option<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Option<Arr<B>> MapT< A, B>(this Option<Arr<A>> ma, Func<A, B> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MOption<Arr<B>>, Option<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Arr<A>> ma, Action<A> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MOption<Arr<A>>).Return(ma);

        public static Option<Arr<A>> Lift< A>(A a) =>
            default(MOption<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Option<Arr<A>> FilterT< A>(this Option<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Option<Arr<A>> Where< A>(this Option<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Option<Arr<B>> Select< A, B>(this Option<Arr<A>> ma, Func<A, B> f) =>
            Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MOption<Arr<B>>, Option<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Option<Arr<C>> SelectMany< A, B, C>(
            this Option<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Arr<A>>, Option<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MOption<Arr<C>>, Option<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionHashSet
    {
        public static A SumT<NumA,  A>(this Option<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<HashSet<A>> ma) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Option<HashSet<B>> BindT< A, B>(this Option<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOption<HashSet<B>>, Option<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Option<HashSet<B>> MapT< A, B>(this Option<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MOption<HashSet<B>>, Option<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<HashSet<A>> ma, Action<A> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MOption<HashSet<A>>).Return(ma);

        public static Option<HashSet<A>> Lift< A>(A a) =>
            default(MOption<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Option<HashSet<A>> FilterT< A>(this Option<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Option<HashSet<A>> Where< A>(this Option<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Option<HashSet<B>> Select< A, B>(this Option<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MOption<HashSet<B>>, Option<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Option<HashSet<C>> SelectMany< A, B, C>(
            this Option<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<HashSet<A>>, Option<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MOption<HashSet<C>>, Option<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionLst
    {
        public static A SumT<NumA,  A>(this Option<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Lst<A>> ma) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Option<Lst<B>> BindT< A, B>(this Option<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOption<Lst<B>>, Option<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Option<Lst<B>> MapT< A, B>(this Option<Lst<A>> ma, Func<A, B> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MOption<Lst<B>>, Option<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Lst<A>> ma, Action<A> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MOption<Lst<A>>).Return(ma);

        public static Option<Lst<A>> Lift< A>(A a) =>
            default(MOption<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Option<Lst<A>> FilterT< A>(this Option<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Option<Lst<A>> Where< A>(this Option<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Option<Lst<B>> Select< A, B>(this Option<Lst<A>> ma, Func<A, B> f) =>
            Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MOption<Lst<B>>, Option<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Option<Lst<C>> SelectMany< A, B, C>(
            this Option<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Lst<A>>, Option<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MOption<Lst<C>>, Option<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionOption
    {
        public static A SumT<NumA,  A>(this Option<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Option<A>> ma) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Option<Option<B>> BindT< A, B>(this Option<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOption<Option<B>>, Option<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Option<Option<B>> MapT< A, B>(this Option<Option<A>> ma, Func<A, B> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MOption<Option<B>>, Option<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Option<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Option<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Option<A>> ma, Action<A> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Option<A>> Lift< A>(Option<A> ma) =>
            default(MOption<Option<A>>).Return(ma);

        public static Option<Option<A>> Lift< A>(A a) =>
            default(MOption<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Option<Option<A>> FilterT< A>(this Option<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Option<Option<A>> Where< A>(this Option<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Option<Option<B>> Select< A, B>(this Option<Option<A>> ma, Func<A, B> f) =>
            Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MOption<Option<B>>, Option<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Option<Option<C>> SelectMany< A, B, C>(
            this Option<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Option<A>>, Option<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MOption<Option<C>>, Option<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Option<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<OptionUnsafe<A>> ma) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Option<OptionUnsafe<B>> BindT< A, B>(this Option<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOption<OptionUnsafe<B>>, Option<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Option<OptionUnsafe<B>> MapT< A, B>(this Option<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MOption<OptionUnsafe<B>>, Option<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MOption<OptionUnsafe<A>>).Return(ma);

        public static Option<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MOption<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Option<OptionUnsafe<A>> FilterT< A>(this Option<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Option<OptionUnsafe<A>> Where< A>(this Option<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Option<OptionUnsafe<B>> Select< A, B>(this Option<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MOption<OptionUnsafe<B>>, Option<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Option<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Option<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<OptionUnsafe<A>>, Option<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MOption<OptionUnsafe<C>>, Option<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionEither
    {
        public static A SumT<NumA, L, A>(this Option<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Option<Either<L, A>> ma) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Option<Either<L, B>> BindT<L, A, B>(this Option<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOption<Either<L, B>>, Option<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Option<Either<L, B>> MapT<L, A, B>(this Option<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MOption<Either<L, B>>, Option<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Option<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Option<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Option<Either<L, A>> ma, Action<A> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MOption<Either<L, A>>).Return(ma);

        public static Option<Either<L, A>> Lift<L, A>(A a) =>
            default(MOption<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Option<Either<L, A>> FilterT<L, A>(this Option<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Option<Either<L, A>> Where<L, A>(this Option<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Option<Either<L, B>> Select<L, A, B>(this Option<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MOption<Either<L, B>>, Option<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Option<Either<L, C>> SelectMany<L, A, B, C>(
            this Option<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Either<L, A>>, Option<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MOption<Either<L, C>>, Option<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class OptionEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Option<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Option<EitherUnsafe<L, A>> ma) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Option<EitherUnsafe<L, B>> BindT<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOption<EitherUnsafe<L, B>>, Option<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Option<EitherUnsafe<L, B>> MapT<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MOption<EitherUnsafe<L, B>>, Option<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MOption<EitherUnsafe<L, A>>).Return(ma);

        public static Option<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MOption<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Option<EitherUnsafe<L, A>> FilterT<L, A>(this Option<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Option<EitherUnsafe<L, A>> Where<L, A>(this Option<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Option<EitherUnsafe<L, B>> Select<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MOption<EitherUnsafe<L, B>>, Option<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Option<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Option<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<EitherUnsafe<L, A>>, Option<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MOption<EitherUnsafe<L, C>>, Option<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class OptionTry
    {
        public static A SumT<NumA,  A>(this Option<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Try<A>> ma) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Option<Try<B>> BindT< A, B>(this Option<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOption<Try<B>>, Option<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Option<Try<B>> MapT< A, B>(this Option<Try<A>> ma, Func<A, B> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MOption<Try<B>>, Option<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Try<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Try<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Try<A>> ma, Action<A> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Try<A>> Lift< A>(Try<A> ma) =>
            default(MOption<Try<A>>).Return(ma);

        public static Option<Try<A>> Lift< A>(A a) =>
            default(MOption<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Option<Try<A>> FilterT< A>(this Option<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Option<Try<A>> Where< A>(this Option<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Option<Try<B>> Select< A, B>(this Option<Try<A>> ma, Func<A, B> f) =>
            Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MOption<Try<B>>, Option<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Option<Try<C>> SelectMany< A, B, C>(
            this Option<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Try<A>>, Option<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MOption<Try<C>>, Option<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionTryAsync
    {
        public static A SumT<NumA,  A>(this Option<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<TryAsync<A>> ma) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Option<TryAsync<B>> BindT< A, B>(this Option<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOption<TryAsync<B>>, Option<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Option<TryAsync<B>> MapT< A, B>(this Option<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MOption<TryAsync<B>>, Option<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<TryAsync<A>> ma, Action<A> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MOption<TryAsync<A>>).Return(ma);

        public static Option<TryAsync<A>> Lift< A>(A a) =>
            default(MOption<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Option<TryAsync<A>> FilterT< A>(this Option<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Option<TryAsync<A>> Where< A>(this Option<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Option<TryAsync<B>> Select< A, B>(this Option<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MOption<TryAsync<B>>, Option<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Option<TryAsync<C>> SelectMany< A, B, C>(
            this Option<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<TryAsync<A>>, Option<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MOption<TryAsync<C>>, Option<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionTryOption
    {
        public static A SumT<NumA,  A>(this Option<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<TryOption<A>> ma) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Option<TryOption<B>> BindT< A, B>(this Option<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOption<TryOption<B>>, Option<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Option<TryOption<B>> MapT< A, B>(this Option<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MOption<TryOption<B>>, Option<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<TryOption<A>> ma, Action<A> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MOption<TryOption<A>>).Return(ma);

        public static Option<TryOption<A>> Lift< A>(A a) =>
            default(MOption<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Option<TryOption<A>> FilterT< A>(this Option<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Option<TryOption<A>> Where< A>(this Option<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Option<TryOption<B>> Select< A, B>(this Option<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MOption<TryOption<B>>, Option<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Option<TryOption<C>> SelectMany< A, B, C>(
            this Option<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<TryOption<A>>, Option<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MOption<TryOption<C>>, Option<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Option<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<TryOptionAsync<A>> ma) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Option<TryOptionAsync<B>> BindT< A, B>(this Option<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOption<TryOptionAsync<B>>, Option<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Option<TryOptionAsync<B>> MapT< A, B>(this Option<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MOption<TryOptionAsync<B>>, Option<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MOption<TryOptionAsync<A>>).Return(ma);

        public static Option<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MOption<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Option<TryOptionAsync<A>> FilterT< A>(this Option<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Option<TryOptionAsync<A>> Where< A>(this Option<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Option<TryOptionAsync<B>> Select< A, B>(this Option<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MOption<TryOptionAsync<B>>, Option<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Option<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Option<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<TryOptionAsync<A>>, Option<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MOption<TryOptionAsync<C>>, Option<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionQue
    {
        public static A SumT<NumA,  A>(this Option<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Que<A>> ma) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Option<Que<B>> BindT< A, B>(this Option<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOption<Que<B>>, Option<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Option<Que<B>> MapT< A, B>(this Option<Que<A>> ma, Func<A, B> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MOption<Que<B>>, Option<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Que<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Que<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Que<A>> ma, Action<A> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Que<A>> Lift< A>(Que<A> ma) =>
            default(MOption<Que<A>>).Return(ma);

        public static Option<Que<A>> Lift< A>(A a) =>
            default(MOption<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Option<Que<A>> FilterT< A>(this Option<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Option<Que<A>> Where< A>(this Option<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Option<Que<B>> Select< A, B>(this Option<Que<A>> ma, Func<A, B> f) =>
            Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MOption<Que<B>>, Option<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Option<Que<C>> SelectMany< A, B, C>(
            this Option<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Que<A>>, Option<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MOption<Que<C>>, Option<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionIEnumerable
    {
        public static A SumT<NumA,  A>(this Option<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<IEnumerable<A>> ma) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Option<IEnumerable<B>> BindT< A, B>(this Option<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOption<IEnumerable<B>>, Option<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Option<IEnumerable<B>> MapT< A, B>(this Option<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MOption<IEnumerable<B>>, Option<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MOption<IEnumerable<A>>).Return(ma);

        public static Option<IEnumerable<A>> Lift< A>(A a) =>
            default(MOption<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Option<IEnumerable<A>> FilterT< A>(this Option<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Option<IEnumerable<A>> Where< A>(this Option<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Option<IEnumerable<B>> Select< A, B>(this Option<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MOption<IEnumerable<B>>, Option<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Option<IEnumerable<C>> SelectMany< A, B, C>(
            this Option<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<IEnumerable<A>>, Option<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MOption<IEnumerable<C>>, Option<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionSet
    {
        public static A SumT<NumA,  A>(this Option<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Set<A>> ma) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Option<Set<B>> BindT< A, B>(this Option<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOption<Set<B>>, Option<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Option<Set<B>> MapT< A, B>(this Option<Set<A>> ma, Func<A, B> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MOption<Set<B>>, Option<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Set<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Set<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Set<A>> ma, Action<A> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Set<A>> Lift< A>(Set<A> ma) =>
            default(MOption<Set<A>>).Return(ma);

        public static Option<Set<A>> Lift< A>(A a) =>
            default(MOption<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Option<Set<A>> FilterT< A>(this Option<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Option<Set<A>> Where< A>(this Option<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Option<Set<B>> Select< A, B>(this Option<Set<A>> ma, Func<A, B> f) =>
            Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MOption<Set<B>>, Option<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Option<Set<C>> SelectMany< A, B, C>(
            this Option<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Set<A>>, Option<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MOption<Set<C>>, Option<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionStck
    {
        public static A SumT<NumA,  A>(this Option<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Option<Stck<A>> ma) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Option<Stck<B>> BindT< A, B>(this Option<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOption<Stck<B>>, Option<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Option<Stck<B>> MapT< A, B>(this Option<Stck<A>> ma, Func<A, B> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MOption<Stck<B>>, Option<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Option<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Option<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Option<Stck<A>> ma, Action<A> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Option<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MOption<Stck<A>>).Return(ma);

        public static Option<Stck<A>> Lift< A>(A a) =>
            default(MOption<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Option<Stck<A>> FilterT< A>(this Option<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Option<Stck<A>> Where< A>(this Option<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Option<Stck<B>> Select< A, B>(this Option<Stck<A>> ma, Func<A, B> f) =>
            Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MOption<Stck<B>>, Option<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Option<Stck<C>> SelectMany< A, B, C>(
            this Option<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOption<Stck<A>>, Option<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MOption<Stck<C>>, Option<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeArr
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Arr<A>> ma) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Arr<B>> BindT< A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOptionUnsafe<Arr<B>>, OptionUnsafe<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static OptionUnsafe<Arr<B>> MapT< A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MOptionUnsafe<Arr<B>>, OptionUnsafe<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Arr<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MOptionUnsafe<Arr<A>>).Return(ma);

        public static OptionUnsafe<Arr<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static OptionUnsafe<Arr<A>> FilterT< A>(this OptionUnsafe<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static OptionUnsafe<Arr<A>> Where< A>(this OptionUnsafe<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static OptionUnsafe<Arr<B>> Select< A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MOptionUnsafe<Arr<B>>, OptionUnsafe<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static OptionUnsafe<Arr<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Arr<A>>, OptionUnsafe<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MOptionUnsafe<Arr<C>>, OptionUnsafe<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeHashSet
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<HashSet<A>> ma) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<HashSet<B>> BindT< A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOptionUnsafe<HashSet<B>>, OptionUnsafe<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static OptionUnsafe<HashSet<B>> MapT< A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MOptionUnsafe<HashSet<B>>, OptionUnsafe<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<HashSet<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MOptionUnsafe<HashSet<A>>).Return(ma);

        public static OptionUnsafe<HashSet<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static OptionUnsafe<HashSet<A>> FilterT< A>(this OptionUnsafe<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static OptionUnsafe<HashSet<A>> Where< A>(this OptionUnsafe<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static OptionUnsafe<HashSet<B>> Select< A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MOptionUnsafe<HashSet<B>>, OptionUnsafe<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static OptionUnsafe<HashSet<C>> SelectMany< A, B, C>(
            this OptionUnsafe<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<HashSet<A>>, OptionUnsafe<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MOptionUnsafe<HashSet<C>>, OptionUnsafe<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeLst
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Lst<A>> ma) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Lst<B>> BindT< A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOptionUnsafe<Lst<B>>, OptionUnsafe<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static OptionUnsafe<Lst<B>> MapT< A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MOptionUnsafe<Lst<B>>, OptionUnsafe<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Lst<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MOptionUnsafe<Lst<A>>).Return(ma);

        public static OptionUnsafe<Lst<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static OptionUnsafe<Lst<A>> FilterT< A>(this OptionUnsafe<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static OptionUnsafe<Lst<A>> Where< A>(this OptionUnsafe<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static OptionUnsafe<Lst<B>> Select< A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MOptionUnsafe<Lst<B>>, OptionUnsafe<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static OptionUnsafe<Lst<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Lst<A>>, OptionUnsafe<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MOptionUnsafe<Lst<C>>, OptionUnsafe<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeOption
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Option<A>> ma) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Option<B>> BindT< A, B>(this OptionUnsafe<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOptionUnsafe<Option<B>>, OptionUnsafe<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static OptionUnsafe<Option<B>> MapT< A, B>(this OptionUnsafe<Option<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MOptionUnsafe<Option<B>>, OptionUnsafe<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Option<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Option<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Option<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Option<A>> Lift< A>(Option<A> ma) =>
            default(MOptionUnsafe<Option<A>>).Return(ma);

        public static OptionUnsafe<Option<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Option<A>>).Return(default(MOption<A>).Return(a));

        public static OptionUnsafe<Option<A>> FilterT< A>(this OptionUnsafe<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static OptionUnsafe<Option<A>> Where< A>(this OptionUnsafe<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static OptionUnsafe<Option<B>> Select< A, B>(this OptionUnsafe<Option<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MOptionUnsafe<Option<B>>, OptionUnsafe<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static OptionUnsafe<Option<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Option<A>>, OptionUnsafe<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MOptionUnsafe<Option<C>>, OptionUnsafe<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeOptionUnsafe
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<OptionUnsafe<A>> ma) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<OptionUnsafe<B>> BindT< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOptionUnsafe<OptionUnsafe<B>>, OptionUnsafe<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static OptionUnsafe<OptionUnsafe<B>> MapT< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MOptionUnsafe<OptionUnsafe<B>>, OptionUnsafe<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MOptionUnsafe<OptionUnsafe<A>>).Return(ma);

        public static OptionUnsafe<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static OptionUnsafe<OptionUnsafe<A>> FilterT< A>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static OptionUnsafe<OptionUnsafe<A>> Where< A>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static OptionUnsafe<OptionUnsafe<B>> Select< A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MOptionUnsafe<OptionUnsafe<B>>, OptionUnsafe<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static OptionUnsafe<OptionUnsafe<C>> SelectMany< A, B, C>(
            this OptionUnsafe<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<OptionUnsafe<A>>, OptionUnsafe<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MOptionUnsafe<OptionUnsafe<C>>, OptionUnsafe<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeEither
    {
        public static A SumT<NumA, L, A>(this OptionUnsafe<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this OptionUnsafe<Either<L, A>> ma) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Either<L, B>> BindT<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOptionUnsafe<Either<L, B>>, OptionUnsafe<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static OptionUnsafe<Either<L, B>> MapT<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MOptionUnsafe<Either<L, B>>, OptionUnsafe<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MOptionUnsafe<Either<L, A>>).Return(ma);

        public static OptionUnsafe<Either<L, A>> Lift<L, A>(A a) =>
            default(MOptionUnsafe<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static OptionUnsafe<Either<L, A>> FilterT<L, A>(this OptionUnsafe<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static OptionUnsafe<Either<L, A>> Where<L, A>(this OptionUnsafe<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static OptionUnsafe<Either<L, B>> Select<L, A, B>(this OptionUnsafe<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MOptionUnsafe<Either<L, B>>, OptionUnsafe<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static OptionUnsafe<Either<L, C>> SelectMany<L, A, B, C>(
            this OptionUnsafe<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Either<L, A>>, OptionUnsafe<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MOptionUnsafe<Either<L, C>>, OptionUnsafe<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this OptionUnsafe<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this OptionUnsafe<EitherUnsafe<L, A>> ma) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static OptionUnsafe<EitherUnsafe<L, B>> BindT<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOptionUnsafe<EitherUnsafe<L, B>>, OptionUnsafe<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static OptionUnsafe<EitherUnsafe<L, B>> MapT<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MOptionUnsafe<EitherUnsafe<L, B>>, OptionUnsafe<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MOptionUnsafe<EitherUnsafe<L, A>>).Return(ma);

        public static OptionUnsafe<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MOptionUnsafe<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static OptionUnsafe<EitherUnsafe<L, A>> FilterT<L, A>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static OptionUnsafe<EitherUnsafe<L, A>> Where<L, A>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static OptionUnsafe<EitherUnsafe<L, B>> Select<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MOptionUnsafe<EitherUnsafe<L, B>>, OptionUnsafe<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static OptionUnsafe<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this OptionUnsafe<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<EitherUnsafe<L, A>>, OptionUnsafe<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MOptionUnsafe<EitherUnsafe<L, C>>, OptionUnsafe<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeTry
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Try<A>> ma) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Try<B>> BindT< A, B>(this OptionUnsafe<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOptionUnsafe<Try<B>>, OptionUnsafe<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static OptionUnsafe<Try<B>> MapT< A, B>(this OptionUnsafe<Try<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MOptionUnsafe<Try<B>>, OptionUnsafe<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Try<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Try<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Try<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Try<A>> Lift< A>(Try<A> ma) =>
            default(MOptionUnsafe<Try<A>>).Return(ma);

        public static OptionUnsafe<Try<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Try<A>>).Return(default(MTry<A>).Return(a));

        public static OptionUnsafe<Try<A>> FilterT< A>(this OptionUnsafe<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static OptionUnsafe<Try<A>> Where< A>(this OptionUnsafe<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static OptionUnsafe<Try<B>> Select< A, B>(this OptionUnsafe<Try<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MOptionUnsafe<Try<B>>, OptionUnsafe<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static OptionUnsafe<Try<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Try<A>>, OptionUnsafe<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MOptionUnsafe<Try<C>>, OptionUnsafe<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeTryAsync
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<TryAsync<A>> ma) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<TryAsync<B>> BindT< A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryAsync<B>>, OptionUnsafe<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static OptionUnsafe<TryAsync<B>> MapT< A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MOptionUnsafe<TryAsync<B>>, OptionUnsafe<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<TryAsync<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MOptionUnsafe<TryAsync<A>>).Return(ma);

        public static OptionUnsafe<TryAsync<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static OptionUnsafe<TryAsync<A>> FilterT< A>(this OptionUnsafe<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static OptionUnsafe<TryAsync<A>> Where< A>(this OptionUnsafe<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static OptionUnsafe<TryAsync<B>> Select< A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MOptionUnsafe<TryAsync<B>>, OptionUnsafe<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static OptionUnsafe<TryAsync<C>> SelectMany< A, B, C>(
            this OptionUnsafe<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<TryAsync<A>>, OptionUnsafe<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MOptionUnsafe<TryAsync<C>>, OptionUnsafe<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeTryOption
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<TryOption<A>> ma) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<TryOption<B>> BindT< A, B>(this OptionUnsafe<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOption<B>>, OptionUnsafe<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static OptionUnsafe<TryOption<B>> MapT< A, B>(this OptionUnsafe<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MOptionUnsafe<TryOption<B>>, OptionUnsafe<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<TryOption<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MOptionUnsafe<TryOption<A>>).Return(ma);

        public static OptionUnsafe<TryOption<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static OptionUnsafe<TryOption<A>> FilterT< A>(this OptionUnsafe<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static OptionUnsafe<TryOption<A>> Where< A>(this OptionUnsafe<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static OptionUnsafe<TryOption<B>> Select< A, B>(this OptionUnsafe<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MOptionUnsafe<TryOption<B>>, OptionUnsafe<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static OptionUnsafe<TryOption<C>> SelectMany< A, B, C>(
            this OptionUnsafe<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<TryOption<A>>, OptionUnsafe<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MOptionUnsafe<TryOption<C>>, OptionUnsafe<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeTryOptionAsync
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<TryOptionAsync<A>> ma) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<TryOptionAsync<B>> BindT< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOptionAsync<B>>, OptionUnsafe<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static OptionUnsafe<TryOptionAsync<B>> MapT< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MOptionUnsafe<TryOptionAsync<B>>, OptionUnsafe<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MOptionUnsafe<TryOptionAsync<A>>).Return(ma);

        public static OptionUnsafe<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static OptionUnsafe<TryOptionAsync<A>> FilterT< A>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static OptionUnsafe<TryOptionAsync<A>> Where< A>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static OptionUnsafe<TryOptionAsync<B>> Select< A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MOptionUnsafe<TryOptionAsync<B>>, OptionUnsafe<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static OptionUnsafe<TryOptionAsync<C>> SelectMany< A, B, C>(
            this OptionUnsafe<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<TryOptionAsync<A>>, OptionUnsafe<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MOptionUnsafe<TryOptionAsync<C>>, OptionUnsafe<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeQue
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Que<A>> ma) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Que<B>> BindT< A, B>(this OptionUnsafe<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOptionUnsafe<Que<B>>, OptionUnsafe<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static OptionUnsafe<Que<B>> MapT< A, B>(this OptionUnsafe<Que<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MOptionUnsafe<Que<B>>, OptionUnsafe<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Que<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Que<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Que<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Que<A>> Lift< A>(Que<A> ma) =>
            default(MOptionUnsafe<Que<A>>).Return(ma);

        public static OptionUnsafe<Que<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Que<A>>).Return(default(MQue<A>).Return(a));

        public static OptionUnsafe<Que<A>> FilterT< A>(this OptionUnsafe<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static OptionUnsafe<Que<A>> Where< A>(this OptionUnsafe<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static OptionUnsafe<Que<B>> Select< A, B>(this OptionUnsafe<Que<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MOptionUnsafe<Que<B>>, OptionUnsafe<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static OptionUnsafe<Que<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Que<A>>, OptionUnsafe<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MOptionUnsafe<Que<C>>, OptionUnsafe<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeIEnumerable
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<IEnumerable<A>> ma) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<IEnumerable<B>> BindT< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOptionUnsafe<IEnumerable<B>>, OptionUnsafe<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static OptionUnsafe<IEnumerable<B>> MapT< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MOptionUnsafe<IEnumerable<B>>, OptionUnsafe<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MOptionUnsafe<IEnumerable<A>>).Return(ma);

        public static OptionUnsafe<IEnumerable<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static OptionUnsafe<IEnumerable<A>> FilterT< A>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static OptionUnsafe<IEnumerable<A>> Where< A>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static OptionUnsafe<IEnumerable<B>> Select< A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MOptionUnsafe<IEnumerable<B>>, OptionUnsafe<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static OptionUnsafe<IEnumerable<C>> SelectMany< A, B, C>(
            this OptionUnsafe<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<IEnumerable<A>>, OptionUnsafe<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MOptionUnsafe<IEnumerable<C>>, OptionUnsafe<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeSet
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Set<A>> ma) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Set<B>> BindT< A, B>(this OptionUnsafe<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOptionUnsafe<Set<B>>, OptionUnsafe<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static OptionUnsafe<Set<B>> MapT< A, B>(this OptionUnsafe<Set<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MOptionUnsafe<Set<B>>, OptionUnsafe<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Set<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Set<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Set<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Set<A>> Lift< A>(Set<A> ma) =>
            default(MOptionUnsafe<Set<A>>).Return(ma);

        public static OptionUnsafe<Set<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Set<A>>).Return(default(MSet<A>).Return(a));

        public static OptionUnsafe<Set<A>> FilterT< A>(this OptionUnsafe<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static OptionUnsafe<Set<A>> Where< A>(this OptionUnsafe<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static OptionUnsafe<Set<B>> Select< A, B>(this OptionUnsafe<Set<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MOptionUnsafe<Set<B>>, OptionUnsafe<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static OptionUnsafe<Set<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Set<A>>, OptionUnsafe<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MOptionUnsafe<Set<C>>, OptionUnsafe<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class OptionUnsafeStck
    {
        public static A SumT<NumA,  A>(this OptionUnsafe<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this OptionUnsafe<Stck<A>> ma) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static OptionUnsafe<Stck<B>> BindT< A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOptionUnsafe<Stck<B>>, OptionUnsafe<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static OptionUnsafe<Stck<B>> MapT< A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MOptionUnsafe<Stck<B>>, OptionUnsafe<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this OptionUnsafe<Stck<A>> ma, Action<A> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static OptionUnsafe<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MOptionUnsafe<Stck<A>>).Return(ma);

        public static OptionUnsafe<Stck<A>> Lift< A>(A a) =>
            default(MOptionUnsafe<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static OptionUnsafe<Stck<A>> FilterT< A>(this OptionUnsafe<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static OptionUnsafe<Stck<A>> Where< A>(this OptionUnsafe<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static OptionUnsafe<Stck<B>> Select< A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, B> f) =>
            Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MOptionUnsafe<Stck<B>>, OptionUnsafe<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static OptionUnsafe<Stck<C>> SelectMany< A, B, C>(
            this OptionUnsafe<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MOptionUnsafe<Stck<A>>, OptionUnsafe<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MOptionUnsafe<Stck<C>>, OptionUnsafe<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherArr
    {
        public static A SumT<NumA, L, A>(this Either<L, Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Arr<A>> ma) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Either<L, Arr<B>> BindT<L, A, B>(this Either<L, Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEither<L, Arr<B>>, Either<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Either<L, Arr<B>> MapT<L, A, B>(this Either<L, Arr<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MEither<L, Arr<B>>, Either<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Arr<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Arr<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Arr<A>> ma, Action<A> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Arr<A>> Lift<L, A>(Arr<A> ma) =>
            default(MEither<L, Arr<A>>).Return(ma);

        public static Either<L, Arr<A>> Lift<L, A>(A a) =>
            default(MEither<L, Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Either<L, Arr<A>> FilterT<L, A>(this Either<L, Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Either<L, Arr<A>> Where<L, A>(this Either<L, Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Either<L, Arr<B>> Select<L, A, B>(this Either<L, Arr<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MEither<L, Arr<B>>, Either<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Either<L, Arr<C>> SelectMany<L, A, B, C>(
            this Either<L, Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Arr<A>>, Either<L, Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MEither<L, Arr<C>>, Either<L, Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherHashSet
    {
        public static A SumT<NumA, L, A>(this Either<L, HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, HashSet<A>> ma) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Either<L, HashSet<B>> BindT<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEither<L, HashSet<B>>, Either<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Either<L, HashSet<B>> MapT<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MEither<L, HashSet<B>>, Either<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, HashSet<A>> ma, Action<A> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, HashSet<A>> Lift<L, A>(HashSet<A> ma) =>
            default(MEither<L, HashSet<A>>).Return(ma);

        public static Either<L, HashSet<A>> Lift<L, A>(A a) =>
            default(MEither<L, HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Either<L, HashSet<A>> FilterT<L, A>(this Either<L, HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Either<L, HashSet<A>> Where<L, A>(this Either<L, HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Either<L, HashSet<B>> Select<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MEither<L, HashSet<B>>, Either<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Either<L, HashSet<C>> SelectMany<L, A, B, C>(
            this Either<L, HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, HashSet<A>>, Either<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MEither<L, HashSet<C>>, Either<L, HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherLst
    {
        public static A SumT<NumA, L, A>(this Either<L, Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Lst<A>> ma) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Either<L, Lst<B>> BindT<L, A, B>(this Either<L, Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEither<L, Lst<B>>, Either<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Either<L, Lst<B>> MapT<L, A, B>(this Either<L, Lst<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MEither<L, Lst<B>>, Either<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Lst<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Lst<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Lst<A>> ma, Action<A> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Lst<A>> Lift<L, A>(Lst<A> ma) =>
            default(MEither<L, Lst<A>>).Return(ma);

        public static Either<L, Lst<A>> Lift<L, A>(A a) =>
            default(MEither<L, Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Either<L, Lst<A>> FilterT<L, A>(this Either<L, Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Either<L, Lst<A>> Where<L, A>(this Either<L, Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Either<L, Lst<B>> Select<L, A, B>(this Either<L, Lst<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MEither<L, Lst<B>>, Either<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Either<L, Lst<C>> SelectMany<L, A, B, C>(
            this Either<L, Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Lst<A>>, Either<L, Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MEither<L, Lst<C>>, Either<L, Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherOption
    {
        public static A SumT<NumA, L, A>(this Either<L, Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Option<A>> ma) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Either<L, Option<B>> BindT<L, A, B>(this Either<L, Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEither<L, Option<B>>, Either<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Either<L, Option<B>> MapT<L, A, B>(this Either<L, Option<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MEither<L, Option<B>>, Either<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Option<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Option<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Option<A>> ma, Action<A> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Option<A>> Lift<L, A>(Option<A> ma) =>
            default(MEither<L, Option<A>>).Return(ma);

        public static Either<L, Option<A>> Lift<L, A>(A a) =>
            default(MEither<L, Option<A>>).Return(default(MOption<A>).Return(a));

        public static Either<L, Option<A>> FilterT<L, A>(this Either<L, Option<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Either<L, Option<A>> Where<L, A>(this Either<L, Option<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Either<L, Option<B>> Select<L, A, B>(this Either<L, Option<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MEither<L, Option<B>>, Either<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Either<L, Option<C>> SelectMany<L, A, B, C>(
            this Either<L, Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Option<A>>, Either<L, Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MEither<L, Option<C>>, Either<L, Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherOptionUnsafe
    {
        public static A SumT<NumA, L, A>(this Either<L, OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, OptionUnsafe<A>> ma) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Either<L, OptionUnsafe<B>> BindT<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEither<L, OptionUnsafe<B>>, Either<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Either<L, OptionUnsafe<B>> MapT<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MEither<L, OptionUnsafe<B>>, Either<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, OptionUnsafe<A>> Lift<L, A>(OptionUnsafe<A> ma) =>
            default(MEither<L, OptionUnsafe<A>>).Return(ma);

        public static Either<L, OptionUnsafe<A>> Lift<L, A>(A a) =>
            default(MEither<L, OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Either<L, OptionUnsafe<A>> FilterT<L, A>(this Either<L, OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Either<L, OptionUnsafe<A>> Where<L, A>(this Either<L, OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Either<L, OptionUnsafe<B>> Select<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MEither<L, OptionUnsafe<B>>, Either<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Either<L, OptionUnsafe<C>> SelectMany<L, A, B, C>(
            this Either<L, OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, OptionUnsafe<A>>, Either<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MEither<L, OptionUnsafe<C>>, Either<L, OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherEither
    {
        public static A SumT<NumA, L, A>(this Either<L, Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Either<L, A>> ma) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Either<L, Either<L, B>> BindT<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEither<L, Either<L, B>>, Either<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Either<L, Either<L, B>> MapT<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MEither<L, Either<L, B>>, Either<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Either<L, A>> ma, Action<A> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MEither<L, Either<L, A>>).Return(ma);

        public static Either<L, Either<L, A>> Lift<L, A>(A a) =>
            default(MEither<L, Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Either<L, Either<L, A>> FilterT<L, A>(this Either<L, Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Either<L, Either<L, A>> Where<L, A>(this Either<L, Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Either<L, Either<L, B>> Select<L, A, B>(this Either<L, Either<L, A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MEither<L, Either<L, B>>, Either<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Either<L, Either<L, C>> SelectMany<L, A, B, C>(
            this Either<L, Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Either<L, A>>, Either<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MEither<L, Either<L, C>>, Either<L, Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class EitherEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Either<L, EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, EitherUnsafe<L, A>> ma) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Either<L, EitherUnsafe<L, B>> BindT<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEither<L, EitherUnsafe<L, B>>, Either<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Either<L, EitherUnsafe<L, B>> MapT<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MEither<L, EitherUnsafe<L, B>>, Either<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MEither<L, EitherUnsafe<L, A>>).Return(ma);

        public static Either<L, EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MEither<L, EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Either<L, EitherUnsafe<L, A>> FilterT<L, A>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Either<L, EitherUnsafe<L, A>> Where<L, A>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Either<L, EitherUnsafe<L, B>> Select<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MEither<L, EitherUnsafe<L, B>>, Either<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Either<L, EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Either<L, EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, EitherUnsafe<L, A>>, Either<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MEither<L, EitherUnsafe<L, C>>, Either<L, EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class EitherTry
    {
        public static A SumT<NumA, L, A>(this Either<L, Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Try<A>> ma) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Either<L, Try<B>> BindT<L, A, B>(this Either<L, Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEither<L, Try<B>>, Either<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Either<L, Try<B>> MapT<L, A, B>(this Either<L, Try<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MEither<L, Try<B>>, Either<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Try<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Try<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Try<A>> ma, Action<A> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Try<A>> Lift<L, A>(Try<A> ma) =>
            default(MEither<L, Try<A>>).Return(ma);

        public static Either<L, Try<A>> Lift<L, A>(A a) =>
            default(MEither<L, Try<A>>).Return(default(MTry<A>).Return(a));

        public static Either<L, Try<A>> FilterT<L, A>(this Either<L, Try<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Either<L, Try<A>> Where<L, A>(this Either<L, Try<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Either<L, Try<B>> Select<L, A, B>(this Either<L, Try<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MEither<L, Try<B>>, Either<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Either<L, Try<C>> SelectMany<L, A, B, C>(
            this Either<L, Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Try<A>>, Either<L, Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MEither<L, Try<C>>, Either<L, Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherTryAsync
    {
        public static A SumT<NumA, L, A>(this Either<L, TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, TryAsync<A>> ma) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Either<L, TryAsync<B>> BindT<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEither<L, TryAsync<B>>, Either<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Either<L, TryAsync<B>> MapT<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MEither<L, TryAsync<B>>, Either<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, TryAsync<A>> ma, Action<A> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, TryAsync<A>> Lift<L, A>(TryAsync<A> ma) =>
            default(MEither<L, TryAsync<A>>).Return(ma);

        public static Either<L, TryAsync<A>> Lift<L, A>(A a) =>
            default(MEither<L, TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Either<L, TryAsync<A>> FilterT<L, A>(this Either<L, TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Either<L, TryAsync<A>> Where<L, A>(this Either<L, TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Either<L, TryAsync<B>> Select<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MEither<L, TryAsync<B>>, Either<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Either<L, TryAsync<C>> SelectMany<L, A, B, C>(
            this Either<L, TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, TryAsync<A>>, Either<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MEither<L, TryAsync<C>>, Either<L, TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherTryOption
    {
        public static A SumT<NumA, L, A>(this Either<L, TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, TryOption<A>> ma) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Either<L, TryOption<B>> BindT<L, A, B>(this Either<L, TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEither<L, TryOption<B>>, Either<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Either<L, TryOption<B>> MapT<L, A, B>(this Either<L, TryOption<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MEither<L, TryOption<B>>, Either<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, TryOption<A>> ma, Action<A> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, TryOption<A>> Lift<L, A>(TryOption<A> ma) =>
            default(MEither<L, TryOption<A>>).Return(ma);

        public static Either<L, TryOption<A>> Lift<L, A>(A a) =>
            default(MEither<L, TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Either<L, TryOption<A>> FilterT<L, A>(this Either<L, TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Either<L, TryOption<A>> Where<L, A>(this Either<L, TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Either<L, TryOption<B>> Select<L, A, B>(this Either<L, TryOption<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MEither<L, TryOption<B>>, Either<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Either<L, TryOption<C>> SelectMany<L, A, B, C>(
            this Either<L, TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, TryOption<A>>, Either<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MEither<L, TryOption<C>>, Either<L, TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherTryOptionAsync
    {
        public static A SumT<NumA, L, A>(this Either<L, TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, TryOptionAsync<A>> ma) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Either<L, TryOptionAsync<B>> BindT<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEither<L, TryOptionAsync<B>>, Either<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Either<L, TryOptionAsync<B>> MapT<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MEither<L, TryOptionAsync<B>>, Either<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, TryOptionAsync<A>> Lift<L, A>(TryOptionAsync<A> ma) =>
            default(MEither<L, TryOptionAsync<A>>).Return(ma);

        public static Either<L, TryOptionAsync<A>> Lift<L, A>(A a) =>
            default(MEither<L, TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Either<L, TryOptionAsync<A>> FilterT<L, A>(this Either<L, TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Either<L, TryOptionAsync<A>> Where<L, A>(this Either<L, TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Either<L, TryOptionAsync<B>> Select<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MEither<L, TryOptionAsync<B>>, Either<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Either<L, TryOptionAsync<C>> SelectMany<L, A, B, C>(
            this Either<L, TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, TryOptionAsync<A>>, Either<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MEither<L, TryOptionAsync<C>>, Either<L, TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherQue
    {
        public static A SumT<NumA, L, A>(this Either<L, Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Que<A>> ma) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Either<L, Que<B>> BindT<L, A, B>(this Either<L, Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEither<L, Que<B>>, Either<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Either<L, Que<B>> MapT<L, A, B>(this Either<L, Que<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MEither<L, Que<B>>, Either<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Que<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Que<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Que<A>> ma, Action<A> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Que<A>> Lift<L, A>(Que<A> ma) =>
            default(MEither<L, Que<A>>).Return(ma);

        public static Either<L, Que<A>> Lift<L, A>(A a) =>
            default(MEither<L, Que<A>>).Return(default(MQue<A>).Return(a));

        public static Either<L, Que<A>> FilterT<L, A>(this Either<L, Que<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Either<L, Que<A>> Where<L, A>(this Either<L, Que<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Either<L, Que<B>> Select<L, A, B>(this Either<L, Que<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MEither<L, Que<B>>, Either<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Either<L, Que<C>> SelectMany<L, A, B, C>(
            this Either<L, Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Que<A>>, Either<L, Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MEither<L, Que<C>>, Either<L, Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherIEnumerable
    {
        public static A SumT<NumA, L, A>(this Either<L, IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, IEnumerable<A>> ma) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Either<L, IEnumerable<B>> BindT<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEither<L, IEnumerable<B>>, Either<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Either<L, IEnumerable<B>> MapT<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MEither<L, IEnumerable<B>>, Either<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, IEnumerable<A>> ma, Action<A> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, IEnumerable<A>> Lift<L, A>(IEnumerable<A> ma) =>
            default(MEither<L, IEnumerable<A>>).Return(ma);

        public static Either<L, IEnumerable<A>> Lift<L, A>(A a) =>
            default(MEither<L, IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Either<L, IEnumerable<A>> FilterT<L, A>(this Either<L, IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Either<L, IEnumerable<A>> Where<L, A>(this Either<L, IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Either<L, IEnumerable<B>> Select<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MEither<L, IEnumerable<B>>, Either<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Either<L, IEnumerable<C>> SelectMany<L, A, B, C>(
            this Either<L, IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, IEnumerable<A>>, Either<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MEither<L, IEnumerable<C>>, Either<L, IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherSet
    {
        public static A SumT<NumA, L, A>(this Either<L, Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Set<A>> ma) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Either<L, Set<B>> BindT<L, A, B>(this Either<L, Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEither<L, Set<B>>, Either<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Either<L, Set<B>> MapT<L, A, B>(this Either<L, Set<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MEither<L, Set<B>>, Either<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Set<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Set<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Set<A>> ma, Action<A> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Set<A>> Lift<L, A>(Set<A> ma) =>
            default(MEither<L, Set<A>>).Return(ma);

        public static Either<L, Set<A>> Lift<L, A>(A a) =>
            default(MEither<L, Set<A>>).Return(default(MSet<A>).Return(a));

        public static Either<L, Set<A>> FilterT<L, A>(this Either<L, Set<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Either<L, Set<A>> Where<L, A>(this Either<L, Set<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Either<L, Set<B>> Select<L, A, B>(this Either<L, Set<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MEither<L, Set<B>>, Either<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Either<L, Set<C>> SelectMany<L, A, B, C>(
            this Either<L, Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Set<A>>, Either<L, Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MEither<L, Set<C>>, Either<L, Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherStck
    {
        public static A SumT<NumA, L, A>(this Either<L, Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Either<L, Stck<A>> ma) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Either<L, Stck<B>> BindT<L, A, B>(this Either<L, Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEither<L, Stck<B>>, Either<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Either<L, Stck<B>> MapT<L, A, B>(this Either<L, Stck<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MEither<L, Stck<B>>, Either<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Either<L, Stck<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Either<L, Stck<A>> ma, Func<A, bool> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Either<L, Stck<A>> ma, Action<A> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Either<L, Stck<A>> Lift<L, A>(Stck<A> ma) =>
            default(MEither<L, Stck<A>>).Return(ma);

        public static Either<L, Stck<A>> Lift<L, A>(A a) =>
            default(MEither<L, Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Either<L, Stck<A>> FilterT<L, A>(this Either<L, Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Either<L, Stck<A>> Where<L, A>(this Either<L, Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Either<L, Stck<B>> Select<L, A, B>(this Either<L, Stck<A>> ma, Func<A, B> f) =>
            Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MEither<L, Stck<B>>, Either<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Either<L, Stck<C>> SelectMany<L, A, B, C>(
            this Either<L, Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEither<L, Stck<A>>, Either<L, Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MEither<L, Stck<C>>, Either<L, Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeArr
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Arr<A>> ma) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Arr<B>> BindT<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Arr<B>>, EitherUnsafe<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static EitherUnsafe<L, Arr<B>> MapT<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MEitherUnsafe<L, Arr<B>>, EitherUnsafe<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Arr<A>> Lift<L, A>(Arr<A> ma) =>
            default(MEitherUnsafe<L, Arr<A>>).Return(ma);

        public static EitherUnsafe<L, Arr<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Arr<A>>).Return(default(MArr<A>).Return(a));

        public static EitherUnsafe<L, Arr<A>> FilterT<L, A>(this EitherUnsafe<L, Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static EitherUnsafe<L, Arr<A>> Where<L, A>(this EitherUnsafe<L, Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static EitherUnsafe<L, Arr<B>> Select<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MEitherUnsafe<L, Arr<B>>, EitherUnsafe<L, Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static EitherUnsafe<L, Arr<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Arr<A>>, EitherUnsafe<L, Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Arr<C>>, EitherUnsafe<L, Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeHashSet
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, HashSet<A>> ma) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, HashSet<B>> BindT<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEitherUnsafe<L, HashSet<B>>, EitherUnsafe<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static EitherUnsafe<L, HashSet<B>> MapT<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MEitherUnsafe<L, HashSet<B>>, EitherUnsafe<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, HashSet<A>> Lift<L, A>(HashSet<A> ma) =>
            default(MEitherUnsafe<L, HashSet<A>>).Return(ma);

        public static EitherUnsafe<L, HashSet<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static EitherUnsafe<L, HashSet<A>> FilterT<L, A>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static EitherUnsafe<L, HashSet<A>> Where<L, A>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static EitherUnsafe<L, HashSet<B>> Select<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MEitherUnsafe<L, HashSet<B>>, EitherUnsafe<L, HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static EitherUnsafe<L, HashSet<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, HashSet<A>>, EitherUnsafe<L, HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, HashSet<C>>, EitherUnsafe<L, HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeLst
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Lst<A>> ma) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Lst<B>> BindT<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Lst<B>>, EitherUnsafe<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static EitherUnsafe<L, Lst<B>> MapT<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MEitherUnsafe<L, Lst<B>>, EitherUnsafe<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Lst<A>> Lift<L, A>(Lst<A> ma) =>
            default(MEitherUnsafe<L, Lst<A>>).Return(ma);

        public static EitherUnsafe<L, Lst<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Lst<A>>).Return(default(MLst<A>).Return(a));

        public static EitherUnsafe<L, Lst<A>> FilterT<L, A>(this EitherUnsafe<L, Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static EitherUnsafe<L, Lst<A>> Where<L, A>(this EitherUnsafe<L, Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static EitherUnsafe<L, Lst<B>> Select<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MEitherUnsafe<L, Lst<B>>, EitherUnsafe<L, Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static EitherUnsafe<L, Lst<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Lst<A>>, EitherUnsafe<L, Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Lst<C>>, EitherUnsafe<L, Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeOption
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Option<A>> ma) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Option<B>> BindT<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Option<B>>, EitherUnsafe<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static EitherUnsafe<L, Option<B>> MapT<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MEitherUnsafe<L, Option<B>>, EitherUnsafe<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Option<A>> Lift<L, A>(Option<A> ma) =>
            default(MEitherUnsafe<L, Option<A>>).Return(ma);

        public static EitherUnsafe<L, Option<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Option<A>>).Return(default(MOption<A>).Return(a));

        public static EitherUnsafe<L, Option<A>> FilterT<L, A>(this EitherUnsafe<L, Option<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static EitherUnsafe<L, Option<A>> Where<L, A>(this EitherUnsafe<L, Option<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static EitherUnsafe<L, Option<B>> Select<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MEitherUnsafe<L, Option<B>>, EitherUnsafe<L, Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static EitherUnsafe<L, Option<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Option<A>>, EitherUnsafe<L, Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Option<C>>, EitherUnsafe<L, Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeOptionUnsafe
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, OptionUnsafe<A>> ma) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, OptionUnsafe<B>> BindT<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEitherUnsafe<L, OptionUnsafe<B>>, EitherUnsafe<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static EitherUnsafe<L, OptionUnsafe<B>> MapT<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MEitherUnsafe<L, OptionUnsafe<B>>, EitherUnsafe<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, OptionUnsafe<A>> Lift<L, A>(OptionUnsafe<A> ma) =>
            default(MEitherUnsafe<L, OptionUnsafe<A>>).Return(ma);

        public static EitherUnsafe<L, OptionUnsafe<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static EitherUnsafe<L, OptionUnsafe<A>> FilterT<L, A>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static EitherUnsafe<L, OptionUnsafe<A>> Where<L, A>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static EitherUnsafe<L, OptionUnsafe<B>> Select<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MEitherUnsafe<L, OptionUnsafe<B>>, EitherUnsafe<L, OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static EitherUnsafe<L, OptionUnsafe<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, OptionUnsafe<A>>, EitherUnsafe<L, OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, OptionUnsafe<C>>, EitherUnsafe<L, OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeEither
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Either<L, A>> ma) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Either<L, B>> BindT<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, Either<L, B>>, EitherUnsafe<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static EitherUnsafe<L, Either<L, B>> MapT<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MEitherUnsafe<L, Either<L, B>>, EitherUnsafe<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MEitherUnsafe<L, Either<L, A>>).Return(ma);

        public static EitherUnsafe<L, Either<L, A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static EitherUnsafe<L, Either<L, A>> FilterT<L, A>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static EitherUnsafe<L, Either<L, A>> Where<L, A>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static EitherUnsafe<L, Either<L, B>> Select<L, A, B>(this EitherUnsafe<L, Either<L, A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MEitherUnsafe<L, Either<L, B>>, EitherUnsafe<L, Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static EitherUnsafe<L, Either<L, C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Either<L, A>>, EitherUnsafe<L, Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Either<L, C>>, EitherUnsafe<L, Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, EitherUnsafe<L, B>> BindT<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, EitherUnsafe<L, B>>, EitherUnsafe<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static EitherUnsafe<L, EitherUnsafe<L, B>> MapT<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MEitherUnsafe<L, EitherUnsafe<L, B>>, EitherUnsafe<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MEitherUnsafe<L, EitherUnsafe<L, A>>).Return(ma);

        public static EitherUnsafe<L, EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static EitherUnsafe<L, EitherUnsafe<L, A>> FilterT<L, A>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static EitherUnsafe<L, EitherUnsafe<L, A>> Where<L, A>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static EitherUnsafe<L, EitherUnsafe<L, B>> Select<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MEitherUnsafe<L, EitherUnsafe<L, B>>, EitherUnsafe<L, EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static EitherUnsafe<L, EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, EitherUnsafe<L, A>>, EitherUnsafe<L, EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MEitherUnsafe<L, EitherUnsafe<L, C>>, EitherUnsafe<L, EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeTry
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Try<A>> ma) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Try<B>> BindT<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Try<B>>, EitherUnsafe<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static EitherUnsafe<L, Try<B>> MapT<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MEitherUnsafe<L, Try<B>>, EitherUnsafe<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Try<A>> Lift<L, A>(Try<A> ma) =>
            default(MEitherUnsafe<L, Try<A>>).Return(ma);

        public static EitherUnsafe<L, Try<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Try<A>>).Return(default(MTry<A>).Return(a));

        public static EitherUnsafe<L, Try<A>> FilterT<L, A>(this EitherUnsafe<L, Try<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static EitherUnsafe<L, Try<A>> Where<L, A>(this EitherUnsafe<L, Try<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static EitherUnsafe<L, Try<B>> Select<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MEitherUnsafe<L, Try<B>>, EitherUnsafe<L, Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static EitherUnsafe<L, Try<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Try<A>>, EitherUnsafe<L, Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Try<C>>, EitherUnsafe<L, Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeTryAsync
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, TryAsync<A>> ma) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, TryAsync<B>> BindT<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryAsync<B>>, EitherUnsafe<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static EitherUnsafe<L, TryAsync<B>> MapT<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryAsync<B>>, EitherUnsafe<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, TryAsync<A>> Lift<L, A>(TryAsync<A> ma) =>
            default(MEitherUnsafe<L, TryAsync<A>>).Return(ma);

        public static EitherUnsafe<L, TryAsync<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static EitherUnsafe<L, TryAsync<A>> FilterT<L, A>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static EitherUnsafe<L, TryAsync<A>> Where<L, A>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static EitherUnsafe<L, TryAsync<B>> Select<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryAsync<B>>, EitherUnsafe<L, TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static EitherUnsafe<L, TryAsync<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, TryAsync<A>>, EitherUnsafe<L, TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, TryAsync<C>>, EitherUnsafe<L, TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeTryOption
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, TryOption<A>> ma) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, TryOption<B>> BindT<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOption<B>>, EitherUnsafe<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static EitherUnsafe<L, TryOption<B>> MapT<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryOption<B>>, EitherUnsafe<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, TryOption<A>> Lift<L, A>(TryOption<A> ma) =>
            default(MEitherUnsafe<L, TryOption<A>>).Return(ma);

        public static EitherUnsafe<L, TryOption<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static EitherUnsafe<L, TryOption<A>> FilterT<L, A>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static EitherUnsafe<L, TryOption<A>> Where<L, A>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static EitherUnsafe<L, TryOption<B>> Select<L, A, B>(this EitherUnsafe<L, TryOption<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryOption<B>>, EitherUnsafe<L, TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static EitherUnsafe<L, TryOption<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, TryOption<A>>, EitherUnsafe<L, TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, TryOption<C>>, EitherUnsafe<L, TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeTryOptionAsync
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, TryOptionAsync<A>> ma) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, TryOptionAsync<B>> BindT<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOptionAsync<B>>, EitherUnsafe<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static EitherUnsafe<L, TryOptionAsync<B>> MapT<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryOptionAsync<B>>, EitherUnsafe<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, TryOptionAsync<A>> Lift<L, A>(TryOptionAsync<A> ma) =>
            default(MEitherUnsafe<L, TryOptionAsync<A>>).Return(ma);

        public static EitherUnsafe<L, TryOptionAsync<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static EitherUnsafe<L, TryOptionAsync<A>> FilterT<L, A>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static EitherUnsafe<L, TryOptionAsync<A>> Where<L, A>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static EitherUnsafe<L, TryOptionAsync<B>> Select<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MEitherUnsafe<L, TryOptionAsync<B>>, EitherUnsafe<L, TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static EitherUnsafe<L, TryOptionAsync<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, TryOptionAsync<A>>, EitherUnsafe<L, TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, TryOptionAsync<C>>, EitherUnsafe<L, TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeQue
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Que<A>> ma) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Que<B>> BindT<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Que<B>>, EitherUnsafe<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static EitherUnsafe<L, Que<B>> MapT<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MEitherUnsafe<L, Que<B>>, EitherUnsafe<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Que<A>> Lift<L, A>(Que<A> ma) =>
            default(MEitherUnsafe<L, Que<A>>).Return(ma);

        public static EitherUnsafe<L, Que<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Que<A>>).Return(default(MQue<A>).Return(a));

        public static EitherUnsafe<L, Que<A>> FilterT<L, A>(this EitherUnsafe<L, Que<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static EitherUnsafe<L, Que<A>> Where<L, A>(this EitherUnsafe<L, Que<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static EitherUnsafe<L, Que<B>> Select<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MEitherUnsafe<L, Que<B>>, EitherUnsafe<L, Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static EitherUnsafe<L, Que<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Que<A>>, EitherUnsafe<L, Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Que<C>>, EitherUnsafe<L, Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeIEnumerable
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, IEnumerable<A>> ma) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, IEnumerable<B>> BindT<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEitherUnsafe<L, IEnumerable<B>>, EitherUnsafe<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static EitherUnsafe<L, IEnumerable<B>> MapT<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MEitherUnsafe<L, IEnumerable<B>>, EitherUnsafe<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, IEnumerable<A>> Lift<L, A>(IEnumerable<A> ma) =>
            default(MEitherUnsafe<L, IEnumerable<A>>).Return(ma);

        public static EitherUnsafe<L, IEnumerable<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static EitherUnsafe<L, IEnumerable<A>> FilterT<L, A>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static EitherUnsafe<L, IEnumerable<A>> Where<L, A>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static EitherUnsafe<L, IEnumerable<B>> Select<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MEitherUnsafe<L, IEnumerable<B>>, EitherUnsafe<L, IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static EitherUnsafe<L, IEnumerable<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, IEnumerable<A>>, EitherUnsafe<L, IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, IEnumerable<C>>, EitherUnsafe<L, IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeSet
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Set<A>> ma) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Set<B>> BindT<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Set<B>>, EitherUnsafe<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static EitherUnsafe<L, Set<B>> MapT<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MEitherUnsafe<L, Set<B>>, EitherUnsafe<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Set<A>> Lift<L, A>(Set<A> ma) =>
            default(MEitherUnsafe<L, Set<A>>).Return(ma);

        public static EitherUnsafe<L, Set<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Set<A>>).Return(default(MSet<A>).Return(a));

        public static EitherUnsafe<L, Set<A>> FilterT<L, A>(this EitherUnsafe<L, Set<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static EitherUnsafe<L, Set<A>> Where<L, A>(this EitherUnsafe<L, Set<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static EitherUnsafe<L, Set<B>> Select<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MEitherUnsafe<L, Set<B>>, EitherUnsafe<L, Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static EitherUnsafe<L, Set<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Set<A>>, EitherUnsafe<L, Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Set<C>>, EitherUnsafe<L, Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class EitherUnsafeStck
    {
        public static A SumT<NumA, L, A>(this EitherUnsafe<L, Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this EitherUnsafe<L, Stck<A>> ma) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static EitherUnsafe<L, Stck<B>> BindT<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Stck<B>>, EitherUnsafe<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static EitherUnsafe<L, Stck<B>> MapT<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MEitherUnsafe<L, Stck<B>>, EitherUnsafe<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, bool> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Action<A> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static EitherUnsafe<L, Stck<A>> Lift<L, A>(Stck<A> ma) =>
            default(MEitherUnsafe<L, Stck<A>>).Return(ma);

        public static EitherUnsafe<L, Stck<A>> Lift<L, A>(A a) =>
            default(MEitherUnsafe<L, Stck<A>>).Return(default(MStck<A>).Return(a));

        public static EitherUnsafe<L, Stck<A>> FilterT<L, A>(this EitherUnsafe<L, Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static EitherUnsafe<L, Stck<A>> Where<L, A>(this EitherUnsafe<L, Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static EitherUnsafe<L, Stck<B>> Select<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, B> f) =>
            Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MEitherUnsafe<L, Stck<B>>, EitherUnsafe<L, Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static EitherUnsafe<L, Stck<C>> SelectMany<L, A, B, C>(
            this EitherUnsafe<L, Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MEitherUnsafe<L, Stck<A>>, EitherUnsafe<L, Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MEitherUnsafe<L, Stck<C>>, EitherUnsafe<L, Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class TryArr
    {
        public static A SumT<NumA,  A>(this Try<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Arr<A>> ma) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Try<Arr<B>> BindT< A, B>(this Try<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTry<Arr<B>>, Try<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Try<Arr<B>> MapT< A, B>(this Try<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTry<Arr<B>>, Try<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Arr<A>> ma, Action<A> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MTry<Arr<A>>).Return(ma);

        public static Try<Arr<A>> Lift< A>(A a) =>
            default(MTry<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Try<Arr<A>> FilterT< A>(this Try<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Try<Arr<A>> Where< A>(this Try<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Try<Arr<B>> Select< A, B>(this Try<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTry<Arr<B>>, Try<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Try<Arr<C>> SelectMany< A, B, C>(
            this Try<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Arr<A>>, Try<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MTry<Arr<C>>, Try<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class TryHashSet
    {
        public static A SumT<NumA,  A>(this Try<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<HashSet<A>> ma) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Try<HashSet<B>> BindT< A, B>(this Try<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTry<HashSet<B>>, Try<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Try<HashSet<B>> MapT< A, B>(this Try<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTry<HashSet<B>>, Try<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<HashSet<A>> ma, Action<A> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MTry<HashSet<A>>).Return(ma);

        public static Try<HashSet<A>> Lift< A>(A a) =>
            default(MTry<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Try<HashSet<A>> FilterT< A>(this Try<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Try<HashSet<A>> Where< A>(this Try<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Try<HashSet<B>> Select< A, B>(this Try<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTry<HashSet<B>>, Try<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Try<HashSet<C>> SelectMany< A, B, C>(
            this Try<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<HashSet<A>>, Try<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MTry<HashSet<C>>, Try<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryLst
    {
        public static A SumT<NumA,  A>(this Try<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Lst<A>> ma) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Try<Lst<B>> BindT< A, B>(this Try<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTry<Lst<B>>, Try<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Try<Lst<B>> MapT< A, B>(this Try<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTry<Lst<B>>, Try<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Lst<A>> ma, Action<A> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MTry<Lst<A>>).Return(ma);

        public static Try<Lst<A>> Lift< A>(A a) =>
            default(MTry<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Try<Lst<A>> FilterT< A>(this Try<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Try<Lst<A>> Where< A>(this Try<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Try<Lst<B>> Select< A, B>(this Try<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTry<Lst<B>>, Try<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Try<Lst<C>> SelectMany< A, B, C>(
            this Try<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Lst<A>>, Try<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MTry<Lst<C>>, Try<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOption
    {
        public static A SumT<NumA,  A>(this Try<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Option<A>> ma) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Try<Option<B>> BindT< A, B>(this Try<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTry<Option<B>>, Try<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Try<Option<B>> MapT< A, B>(this Try<Option<A>> ma, Func<A, B> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTry<Option<B>>, Try<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Option<A>> ma, Action<A> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Option<A>> Lift< A>(Option<A> ma) =>
            default(MTry<Option<A>>).Return(ma);

        public static Try<Option<A>> Lift< A>(A a) =>
            default(MTry<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Try<Option<A>> FilterT< A>(this Try<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Try<Option<A>> Where< A>(this Try<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Try<Option<B>> Select< A, B>(this Try<Option<A>> ma, Func<A, B> f) =>
            Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTry<Option<B>>, Try<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Try<Option<C>> SelectMany< A, B, C>(
            this Try<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Option<A>>, Try<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MTry<Option<C>>, Try<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Try<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<OptionUnsafe<A>> ma) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Try<OptionUnsafe<B>> BindT< A, B>(this Try<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTry<OptionUnsafe<B>>, Try<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Try<OptionUnsafe<B>> MapT< A, B>(this Try<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTry<OptionUnsafe<B>>, Try<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MTry<OptionUnsafe<A>>).Return(ma);

        public static Try<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MTry<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Try<OptionUnsafe<A>> FilterT< A>(this Try<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Try<OptionUnsafe<A>> Where< A>(this Try<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Try<OptionUnsafe<B>> Select< A, B>(this Try<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTry<OptionUnsafe<B>>, Try<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Try<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Try<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<OptionUnsafe<A>>, Try<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MTry<OptionUnsafe<C>>, Try<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class TryEither
    {
        public static A SumT<NumA, L, A>(this Try<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Try<Either<L, A>> ma) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Try<Either<L, B>> BindT<L, A, B>(this Try<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTry<Either<L, B>>, Try<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Try<Either<L, B>> MapT<L, A, B>(this Try<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTry<Either<L, B>>, Try<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Try<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Try<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Try<Either<L, A>> ma, Action<A> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MTry<Either<L, A>>).Return(ma);

        public static Try<Either<L, A>> Lift<L, A>(A a) =>
            default(MTry<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Try<Either<L, A>> FilterT<L, A>(this Try<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Try<Either<L, A>> Where<L, A>(this Try<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Try<Either<L, B>> Select<L, A, B>(this Try<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTry<Either<L, B>>, Try<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Try<Either<L, C>> SelectMany<L, A, B, C>(
            this Try<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Either<L, A>>, Try<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MTry<Either<L, C>>, Try<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Try<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Try<EitherUnsafe<L, A>> ma) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Try<EitherUnsafe<L, B>> BindT<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTry<EitherUnsafe<L, B>>, Try<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Try<EitherUnsafe<L, B>> MapT<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTry<EitherUnsafe<L, B>>, Try<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MTry<EitherUnsafe<L, A>>).Return(ma);

        public static Try<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MTry<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Try<EitherUnsafe<L, A>> FilterT<L, A>(this Try<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Try<EitherUnsafe<L, A>> Where<L, A>(this Try<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Try<EitherUnsafe<L, B>> Select<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTry<EitherUnsafe<L, B>>, Try<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Try<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Try<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<EitherUnsafe<L, A>>, Try<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MTry<EitherUnsafe<L, C>>, Try<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryTry
    {
        public static A SumT<NumA,  A>(this Try<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Try<A>> ma) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Try<Try<B>> BindT< A, B>(this Try<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTry<Try<B>>, Try<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Try<Try<B>> MapT< A, B>(this Try<Try<A>> ma, Func<A, B> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTry<Try<B>>, Try<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Try<A>> ma, Action<A> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Try<A>> Lift< A>(Try<A> ma) =>
            default(MTry<Try<A>>).Return(ma);

        public static Try<Try<A>> Lift< A>(A a) =>
            default(MTry<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Try<Try<A>> FilterT< A>(this Try<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Try<Try<A>> Where< A>(this Try<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Try<Try<B>> Select< A, B>(this Try<Try<A>> ma, Func<A, B> f) =>
            Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTry<Try<B>>, Try<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Try<Try<C>> SelectMany< A, B, C>(
            this Try<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Try<A>>, Try<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MTry<Try<C>>, Try<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class TryTryAsync
    {
        public static A SumT<NumA,  A>(this Try<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<TryAsync<A>> ma) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Try<TryAsync<B>> BindT< A, B>(this Try<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTry<TryAsync<B>>, Try<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Try<TryAsync<B>> MapT< A, B>(this Try<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTry<TryAsync<B>>, Try<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<TryAsync<A>> ma, Action<A> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MTry<TryAsync<A>>).Return(ma);

        public static Try<TryAsync<A>> Lift< A>(A a) =>
            default(MTry<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Try<TryAsync<A>> FilterT< A>(this Try<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Try<TryAsync<A>> Where< A>(this Try<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Try<TryAsync<B>> Select< A, B>(this Try<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTry<TryAsync<B>>, Try<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Try<TryAsync<C>> SelectMany< A, B, C>(
            this Try<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<TryAsync<A>>, Try<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MTry<TryAsync<C>>, Try<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryTryOption
    {
        public static A SumT<NumA,  A>(this Try<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<TryOption<A>> ma) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Try<TryOption<B>> BindT< A, B>(this Try<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTry<TryOption<B>>, Try<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Try<TryOption<B>> MapT< A, B>(this Try<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTry<TryOption<B>>, Try<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<TryOption<A>> ma, Action<A> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MTry<TryOption<A>>).Return(ma);

        public static Try<TryOption<A>> Lift< A>(A a) =>
            default(MTry<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Try<TryOption<A>> FilterT< A>(this Try<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Try<TryOption<A>> Where< A>(this Try<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Try<TryOption<B>> Select< A, B>(this Try<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTry<TryOption<B>>, Try<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Try<TryOption<C>> SelectMany< A, B, C>(
            this Try<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<TryOption<A>>, Try<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MTry<TryOption<C>>, Try<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Try<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<TryOptionAsync<A>> ma) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Try<TryOptionAsync<B>> BindT< A, B>(this Try<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTry<TryOptionAsync<B>>, Try<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Try<TryOptionAsync<B>> MapT< A, B>(this Try<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTry<TryOptionAsync<B>>, Try<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MTry<TryOptionAsync<A>>).Return(ma);

        public static Try<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MTry<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Try<TryOptionAsync<A>> FilterT< A>(this Try<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Try<TryOptionAsync<A>> Where< A>(this Try<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Try<TryOptionAsync<B>> Select< A, B>(this Try<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTry<TryOptionAsync<B>>, Try<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Try<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Try<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<TryOptionAsync<A>>, Try<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MTry<TryOptionAsync<C>>, Try<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryQue
    {
        public static A SumT<NumA,  A>(this Try<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Que<A>> ma) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Try<Que<B>> BindT< A, B>(this Try<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTry<Que<B>>, Try<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Try<Que<B>> MapT< A, B>(this Try<Que<A>> ma, Func<A, B> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTry<Que<B>>, Try<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Que<A>> ma, Action<A> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Que<A>> Lift< A>(Que<A> ma) =>
            default(MTry<Que<A>>).Return(ma);

        public static Try<Que<A>> Lift< A>(A a) =>
            default(MTry<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Try<Que<A>> FilterT< A>(this Try<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Try<Que<A>> Where< A>(this Try<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Try<Que<B>> Select< A, B>(this Try<Que<A>> ma, Func<A, B> f) =>
            Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTry<Que<B>>, Try<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Try<Que<C>> SelectMany< A, B, C>(
            this Try<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Que<A>>, Try<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MTry<Que<C>>, Try<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class TryIEnumerable
    {
        public static A SumT<NumA,  A>(this Try<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<IEnumerable<A>> ma) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Try<IEnumerable<B>> BindT< A, B>(this Try<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTry<IEnumerable<B>>, Try<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Try<IEnumerable<B>> MapT< A, B>(this Try<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTry<IEnumerable<B>>, Try<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MTry<IEnumerable<A>>).Return(ma);

        public static Try<IEnumerable<A>> Lift< A>(A a) =>
            default(MTry<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Try<IEnumerable<A>> FilterT< A>(this Try<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Try<IEnumerable<A>> Where< A>(this Try<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Try<IEnumerable<B>> Select< A, B>(this Try<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTry<IEnumerable<B>>, Try<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Try<IEnumerable<C>> SelectMany< A, B, C>(
            this Try<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<IEnumerable<A>>, Try<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MTry<IEnumerable<C>>, Try<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class TrySet
    {
        public static A SumT<NumA,  A>(this Try<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Set<A>> ma) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Try<Set<B>> BindT< A, B>(this Try<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTry<Set<B>>, Try<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Try<Set<B>> MapT< A, B>(this Try<Set<A>> ma, Func<A, B> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTry<Set<B>>, Try<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Set<A>> ma, Action<A> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Set<A>> Lift< A>(Set<A> ma) =>
            default(MTry<Set<A>>).Return(ma);

        public static Try<Set<A>> Lift< A>(A a) =>
            default(MTry<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Try<Set<A>> FilterT< A>(this Try<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Try<Set<A>> Where< A>(this Try<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Try<Set<B>> Select< A, B>(this Try<Set<A>> ma, Func<A, B> f) =>
            Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTry<Set<B>>, Try<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Try<Set<C>> SelectMany< A, B, C>(
            this Try<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Set<A>>, Try<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MTry<Set<C>>, Try<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryStck
    {
        public static A SumT<NumA,  A>(this Try<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Try<Stck<A>> ma) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Try<Stck<B>> BindT< A, B>(this Try<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTry<Stck<B>>, Try<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Try<Stck<B>> MapT< A, B>(this Try<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTry<Stck<B>>, Try<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Try<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Try<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Try<Stck<A>> ma, Action<A> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Try<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MTry<Stck<A>>).Return(ma);

        public static Try<Stck<A>> Lift< A>(A a) =>
            default(MTry<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Try<Stck<A>> FilterT< A>(this Try<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Try<Stck<A>> Where< A>(this Try<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Try<Stck<B>> Select< A, B>(this Try<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTry<Stck<B>>, Try<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Try<Stck<C>> SelectMany< A, B, C>(
            this Try<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTry<Stck<A>>, Try<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MTry<Stck<C>>, Try<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncArr
    {
        public static A SumT<NumA,  A>(this TryAsync<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Arr<A>> ma) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static TryAsync<Arr<B>> BindT< A, B>(this TryAsync<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryAsync<Arr<B>>, TryAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryAsync<Arr<B>> MapT< A, B>(this TryAsync<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryAsync<Arr<B>>, TryAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Arr<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MTryAsync<Arr<A>>).Return(ma);

        public static TryAsync<Arr<A>> Lift< A>(A a) =>
            default(MTryAsync<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static TryAsync<Arr<A>> FilterT< A>(this TryAsync<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryAsync<Arr<A>> Where< A>(this TryAsync<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryAsync<Arr<B>> Select< A, B>(this TryAsync<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryAsync<Arr<B>>, TryAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryAsync<Arr<C>> SelectMany< A, B, C>(
            this TryAsync<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Arr<A>>, TryAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MTryAsync<Arr<C>>, TryAsync<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncHashSet
    {
        public static A SumT<NumA,  A>(this TryAsync<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<HashSet<A>> ma) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static TryAsync<HashSet<B>> BindT< A, B>(this TryAsync<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryAsync<HashSet<B>>, TryAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryAsync<HashSet<B>> MapT< A, B>(this TryAsync<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryAsync<HashSet<B>>, TryAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<HashSet<A>> ma, Action<A> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MTryAsync<HashSet<A>>).Return(ma);

        public static TryAsync<HashSet<A>> Lift< A>(A a) =>
            default(MTryAsync<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static TryAsync<HashSet<A>> FilterT< A>(this TryAsync<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryAsync<HashSet<A>> Where< A>(this TryAsync<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryAsync<HashSet<B>> Select< A, B>(this TryAsync<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryAsync<HashSet<B>>, TryAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryAsync<HashSet<C>> SelectMany< A, B, C>(
            this TryAsync<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<HashSet<A>>, TryAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MTryAsync<HashSet<C>>, TryAsync<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncLst
    {
        public static A SumT<NumA,  A>(this TryAsync<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Lst<A>> ma) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static TryAsync<Lst<B>> BindT< A, B>(this TryAsync<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryAsync<Lst<B>>, TryAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryAsync<Lst<B>> MapT< A, B>(this TryAsync<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryAsync<Lst<B>>, TryAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Lst<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MTryAsync<Lst<A>>).Return(ma);

        public static TryAsync<Lst<A>> Lift< A>(A a) =>
            default(MTryAsync<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static TryAsync<Lst<A>> FilterT< A>(this TryAsync<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryAsync<Lst<A>> Where< A>(this TryAsync<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryAsync<Lst<B>> Select< A, B>(this TryAsync<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryAsync<Lst<B>>, TryAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryAsync<Lst<C>> SelectMany< A, B, C>(
            this TryAsync<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Lst<A>>, TryAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MTryAsync<Lst<C>>, TryAsync<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncOption
    {
        public static A SumT<NumA,  A>(this TryAsync<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Option<A>> ma) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static TryAsync<Option<B>> BindT< A, B>(this TryAsync<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryAsync<Option<B>>, TryAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryAsync<Option<B>> MapT< A, B>(this TryAsync<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryAsync<Option<B>>, TryAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Option<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Option<A>> Lift< A>(Option<A> ma) =>
            default(MTryAsync<Option<A>>).Return(ma);

        public static TryAsync<Option<A>> Lift< A>(A a) =>
            default(MTryAsync<Option<A>>).Return(default(MOption<A>).Return(a));

        public static TryAsync<Option<A>> FilterT< A>(this TryAsync<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryAsync<Option<A>> Where< A>(this TryAsync<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryAsync<Option<B>> Select< A, B>(this TryAsync<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryAsync<Option<B>>, TryAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryAsync<Option<C>> SelectMany< A, B, C>(
            this TryAsync<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Option<A>>, TryAsync<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MTryAsync<Option<C>>, TryAsync<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncOptionUnsafe
    {
        public static A SumT<NumA,  A>(this TryAsync<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<OptionUnsafe<A>> ma) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static TryAsync<OptionUnsafe<B>> BindT< A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryAsync<OptionUnsafe<B>>, TryAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryAsync<OptionUnsafe<B>> MapT< A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryAsync<OptionUnsafe<B>>, TryAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MTryAsync<OptionUnsafe<A>>).Return(ma);

        public static TryAsync<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MTryAsync<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static TryAsync<OptionUnsafe<A>> FilterT< A>(this TryAsync<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryAsync<OptionUnsafe<A>> Where< A>(this TryAsync<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryAsync<OptionUnsafe<B>> Select< A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryAsync<OptionUnsafe<B>>, TryAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryAsync<OptionUnsafe<C>> SelectMany< A, B, C>(
            this TryAsync<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<OptionUnsafe<A>>, TryAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MTryAsync<OptionUnsafe<C>>, TryAsync<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncEither
    {
        public static A SumT<NumA, L, A>(this TryAsync<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryAsync<Either<L, A>> ma) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static TryAsync<Either<L, B>> BindT<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryAsync<Either<L, B>>, TryAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryAsync<Either<L, B>> MapT<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryAsync<Either<L, B>>, TryAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryAsync<Either<L, A>> ma, Action<A> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MTryAsync<Either<L, A>>).Return(ma);

        public static TryAsync<Either<L, A>> Lift<L, A>(A a) =>
            default(MTryAsync<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static TryAsync<Either<L, A>> FilterT<L, A>(this TryAsync<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryAsync<Either<L, A>> Where<L, A>(this TryAsync<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryAsync<Either<L, B>> Select<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryAsync<Either<L, B>>, TryAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryAsync<Either<L, C>> SelectMany<L, A, B, C>(
            this TryAsync<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Either<L, A>>, TryAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MTryAsync<Either<L, C>>, TryAsync<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this TryAsync<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryAsync<EitherUnsafe<L, A>> ma) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static TryAsync<EitherUnsafe<L, B>> BindT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryAsync<EitherUnsafe<L, B>>, TryAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryAsync<EitherUnsafe<L, B>> MapT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryAsync<EitherUnsafe<L, B>>, TryAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MTryAsync<EitherUnsafe<L, A>>).Return(ma);

        public static TryAsync<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MTryAsync<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static TryAsync<EitherUnsafe<L, A>> FilterT<L, A>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryAsync<EitherUnsafe<L, A>> Where<L, A>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryAsync<EitherUnsafe<L, B>> Select<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryAsync<EitherUnsafe<L, B>>, TryAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryAsync<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this TryAsync<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<EitherUnsafe<L, A>>, TryAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MTryAsync<EitherUnsafe<L, C>>, TryAsync<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncTry
    {
        public static A SumT<NumA,  A>(this TryAsync<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Try<A>> ma) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static TryAsync<Try<B>> BindT< A, B>(this TryAsync<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryAsync<Try<B>>, TryAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryAsync<Try<B>> MapT< A, B>(this TryAsync<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryAsync<Try<B>>, TryAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Try<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Try<A>> Lift< A>(Try<A> ma) =>
            default(MTryAsync<Try<A>>).Return(ma);

        public static TryAsync<Try<A>> Lift< A>(A a) =>
            default(MTryAsync<Try<A>>).Return(default(MTry<A>).Return(a));

        public static TryAsync<Try<A>> FilterT< A>(this TryAsync<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryAsync<Try<A>> Where< A>(this TryAsync<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryAsync<Try<B>> Select< A, B>(this TryAsync<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryAsync<Try<B>>, TryAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryAsync<Try<C>> SelectMany< A, B, C>(
            this TryAsync<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Try<A>>, TryAsync<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MTryAsync<Try<C>>, TryAsync<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncTryAsync
    {
        public static A SumT<NumA,  A>(this TryAsync<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<TryAsync<A>> ma) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static TryAsync<TryAsync<B>> BindT< A, B>(this TryAsync<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryAsync<TryAsync<B>>, TryAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryAsync<TryAsync<B>> MapT< A, B>(this TryAsync<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryAsync<TryAsync<B>>, TryAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<TryAsync<A>> ma, Action<A> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MTryAsync<TryAsync<A>>).Return(ma);

        public static TryAsync<TryAsync<A>> Lift< A>(A a) =>
            default(MTryAsync<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static TryAsync<TryAsync<A>> FilterT< A>(this TryAsync<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryAsync<TryAsync<A>> Where< A>(this TryAsync<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryAsync<TryAsync<B>> Select< A, B>(this TryAsync<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryAsync<TryAsync<B>>, TryAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryAsync<TryAsync<C>> SelectMany< A, B, C>(
            this TryAsync<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<TryAsync<A>>, TryAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MTryAsync<TryAsync<C>>, TryAsync<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncTryOption
    {
        public static A SumT<NumA,  A>(this TryAsync<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<TryOption<A>> ma) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static TryAsync<TryOption<B>> BindT< A, B>(this TryAsync<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryAsync<TryOption<B>>, TryAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryAsync<TryOption<B>> MapT< A, B>(this TryAsync<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryAsync<TryOption<B>>, TryAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<TryOption<A>> ma, Action<A> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MTryAsync<TryOption<A>>).Return(ma);

        public static TryAsync<TryOption<A>> Lift< A>(A a) =>
            default(MTryAsync<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static TryAsync<TryOption<A>> FilterT< A>(this TryAsync<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryAsync<TryOption<A>> Where< A>(this TryAsync<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryAsync<TryOption<B>> Select< A, B>(this TryAsync<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryAsync<TryOption<B>>, TryAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryAsync<TryOption<C>> SelectMany< A, B, C>(
            this TryAsync<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<TryOption<A>>, TryAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MTryAsync<TryOption<C>>, TryAsync<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncTryOptionAsync
    {
        public static A SumT<NumA,  A>(this TryAsync<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<TryOptionAsync<A>> ma) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static TryAsync<TryOptionAsync<B>> BindT< A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryAsync<TryOptionAsync<B>>, TryAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryAsync<TryOptionAsync<B>> MapT< A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryAsync<TryOptionAsync<B>>, TryAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MTryAsync<TryOptionAsync<A>>).Return(ma);

        public static TryAsync<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MTryAsync<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static TryAsync<TryOptionAsync<A>> FilterT< A>(this TryAsync<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryAsync<TryOptionAsync<A>> Where< A>(this TryAsync<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryAsync<TryOptionAsync<B>> Select< A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryAsync<TryOptionAsync<B>>, TryAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryAsync<TryOptionAsync<C>> SelectMany< A, B, C>(
            this TryAsync<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<TryOptionAsync<A>>, TryAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MTryAsync<TryOptionAsync<C>>, TryAsync<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncQue
    {
        public static A SumT<NumA,  A>(this TryAsync<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Que<A>> ma) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static TryAsync<Que<B>> BindT< A, B>(this TryAsync<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryAsync<Que<B>>, TryAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryAsync<Que<B>> MapT< A, B>(this TryAsync<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryAsync<Que<B>>, TryAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Que<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Que<A>> Lift< A>(Que<A> ma) =>
            default(MTryAsync<Que<A>>).Return(ma);

        public static TryAsync<Que<A>> Lift< A>(A a) =>
            default(MTryAsync<Que<A>>).Return(default(MQue<A>).Return(a));

        public static TryAsync<Que<A>> FilterT< A>(this TryAsync<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryAsync<Que<A>> Where< A>(this TryAsync<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryAsync<Que<B>> Select< A, B>(this TryAsync<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryAsync<Que<B>>, TryAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryAsync<Que<C>> SelectMany< A, B, C>(
            this TryAsync<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Que<A>>, TryAsync<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MTryAsync<Que<C>>, TryAsync<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncIEnumerable
    {
        public static A SumT<NumA,  A>(this TryAsync<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<IEnumerable<A>> ma) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static TryAsync<IEnumerable<B>> BindT< A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryAsync<IEnumerable<B>>, TryAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryAsync<IEnumerable<B>> MapT< A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryAsync<IEnumerable<B>>, TryAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MTryAsync<IEnumerable<A>>).Return(ma);

        public static TryAsync<IEnumerable<A>> Lift< A>(A a) =>
            default(MTryAsync<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static TryAsync<IEnumerable<A>> FilterT< A>(this TryAsync<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryAsync<IEnumerable<A>> Where< A>(this TryAsync<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryAsync<IEnumerable<B>> Select< A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryAsync<IEnumerable<B>>, TryAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryAsync<IEnumerable<C>> SelectMany< A, B, C>(
            this TryAsync<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<IEnumerable<A>>, TryAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MTryAsync<IEnumerable<C>>, TryAsync<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncSet
    {
        public static A SumT<NumA,  A>(this TryAsync<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Set<A>> ma) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static TryAsync<Set<B>> BindT< A, B>(this TryAsync<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryAsync<Set<B>>, TryAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryAsync<Set<B>> MapT< A, B>(this TryAsync<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryAsync<Set<B>>, TryAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Set<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Set<A>> Lift< A>(Set<A> ma) =>
            default(MTryAsync<Set<A>>).Return(ma);

        public static TryAsync<Set<A>> Lift< A>(A a) =>
            default(MTryAsync<Set<A>>).Return(default(MSet<A>).Return(a));

        public static TryAsync<Set<A>> FilterT< A>(this TryAsync<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryAsync<Set<A>> Where< A>(this TryAsync<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryAsync<Set<B>> Select< A, B>(this TryAsync<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryAsync<Set<B>>, TryAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryAsync<Set<C>> SelectMany< A, B, C>(
            this TryAsync<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Set<A>>, TryAsync<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MTryAsync<Set<C>>, TryAsync<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryAsyncStck
    {
        public static A SumT<NumA,  A>(this TryAsync<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryAsync<Stck<A>> ma) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static TryAsync<Stck<B>> BindT< A, B>(this TryAsync<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryAsync<Stck<B>>, TryAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryAsync<Stck<B>> MapT< A, B>(this TryAsync<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryAsync<Stck<B>>, TryAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryAsync<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryAsync<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryAsync<Stck<A>> ma, Action<A> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryAsync<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MTryAsync<Stck<A>>).Return(ma);

        public static TryAsync<Stck<A>> Lift< A>(A a) =>
            default(MTryAsync<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static TryAsync<Stck<A>> FilterT< A>(this TryAsync<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryAsync<Stck<A>> Where< A>(this TryAsync<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryAsync<Stck<B>> Select< A, B>(this TryAsync<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryAsync<Stck<B>>, TryAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryAsync<Stck<C>> SelectMany< A, B, C>(
            this TryAsync<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryAsync<Stck<A>>, TryAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MTryAsync<Stck<C>>, TryAsync<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionArr
    {
        public static A SumT<NumA,  A>(this TryOption<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Arr<A>> ma) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static TryOption<Arr<B>> BindT< A, B>(this TryOption<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOption<Arr<B>>, TryOption<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryOption<Arr<B>> MapT< A, B>(this TryOption<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryOption<Arr<B>>, TryOption<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Arr<A>> ma, Action<A> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MTryOption<Arr<A>>).Return(ma);

        public static TryOption<Arr<A>> Lift< A>(A a) =>
            default(MTryOption<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static TryOption<Arr<A>> FilterT< A>(this TryOption<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryOption<Arr<A>> Where< A>(this TryOption<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryOption<Arr<B>> Select< A, B>(this TryOption<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryOption<Arr<B>>, TryOption<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryOption<Arr<C>> SelectMany< A, B, C>(
            this TryOption<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Arr<A>>, TryOption<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MTryOption<Arr<C>>, TryOption<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionHashSet
    {
        public static A SumT<NumA,  A>(this TryOption<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<HashSet<A>> ma) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static TryOption<HashSet<B>> BindT< A, B>(this TryOption<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOption<HashSet<B>>, TryOption<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryOption<HashSet<B>> MapT< A, B>(this TryOption<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryOption<HashSet<B>>, TryOption<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<HashSet<A>> ma, Action<A> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MTryOption<HashSet<A>>).Return(ma);

        public static TryOption<HashSet<A>> Lift< A>(A a) =>
            default(MTryOption<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static TryOption<HashSet<A>> FilterT< A>(this TryOption<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryOption<HashSet<A>> Where< A>(this TryOption<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryOption<HashSet<B>> Select< A, B>(this TryOption<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryOption<HashSet<B>>, TryOption<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryOption<HashSet<C>> SelectMany< A, B, C>(
            this TryOption<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<HashSet<A>>, TryOption<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MTryOption<HashSet<C>>, TryOption<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionLst
    {
        public static A SumT<NumA,  A>(this TryOption<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Lst<A>> ma) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static TryOption<Lst<B>> BindT< A, B>(this TryOption<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOption<Lst<B>>, TryOption<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryOption<Lst<B>> MapT< A, B>(this TryOption<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryOption<Lst<B>>, TryOption<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Lst<A>> ma, Action<A> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MTryOption<Lst<A>>).Return(ma);

        public static TryOption<Lst<A>> Lift< A>(A a) =>
            default(MTryOption<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static TryOption<Lst<A>> FilterT< A>(this TryOption<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryOption<Lst<A>> Where< A>(this TryOption<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryOption<Lst<B>> Select< A, B>(this TryOption<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryOption<Lst<B>>, TryOption<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryOption<Lst<C>> SelectMany< A, B, C>(
            this TryOption<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Lst<A>>, TryOption<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MTryOption<Lst<C>>, TryOption<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionOption
    {
        public static A SumT<NumA,  A>(this TryOption<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Option<A>> ma) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static TryOption<Option<B>> BindT< A, B>(this TryOption<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOption<Option<B>>, TryOption<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryOption<Option<B>> MapT< A, B>(this TryOption<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryOption<Option<B>>, TryOption<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Option<A>> ma, Action<A> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Option<A>> Lift< A>(Option<A> ma) =>
            default(MTryOption<Option<A>>).Return(ma);

        public static TryOption<Option<A>> Lift< A>(A a) =>
            default(MTryOption<Option<A>>).Return(default(MOption<A>).Return(a));

        public static TryOption<Option<A>> FilterT< A>(this TryOption<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryOption<Option<A>> Where< A>(this TryOption<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryOption<Option<B>> Select< A, B>(this TryOption<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryOption<Option<B>>, TryOption<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryOption<Option<C>> SelectMany< A, B, C>(
            this TryOption<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Option<A>>, TryOption<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MTryOption<Option<C>>, TryOption<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionOptionUnsafe
    {
        public static A SumT<NumA,  A>(this TryOption<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<OptionUnsafe<A>> ma) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static TryOption<OptionUnsafe<B>> BindT< A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOption<OptionUnsafe<B>>, TryOption<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryOption<OptionUnsafe<B>> MapT< A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryOption<OptionUnsafe<B>>, TryOption<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MTryOption<OptionUnsafe<A>>).Return(ma);

        public static TryOption<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MTryOption<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static TryOption<OptionUnsafe<A>> FilterT< A>(this TryOption<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryOption<OptionUnsafe<A>> Where< A>(this TryOption<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryOption<OptionUnsafe<B>> Select< A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryOption<OptionUnsafe<B>>, TryOption<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryOption<OptionUnsafe<C>> SelectMany< A, B, C>(
            this TryOption<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<OptionUnsafe<A>>, TryOption<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MTryOption<OptionUnsafe<C>>, TryOption<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionEither
    {
        public static A SumT<NumA, L, A>(this TryOption<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryOption<Either<L, A>> ma) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static TryOption<Either<L, B>> BindT<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOption<Either<L, B>>, TryOption<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryOption<Either<L, B>> MapT<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryOption<Either<L, B>>, TryOption<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryOption<Either<L, A>> ma, Action<A> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MTryOption<Either<L, A>>).Return(ma);

        public static TryOption<Either<L, A>> Lift<L, A>(A a) =>
            default(MTryOption<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static TryOption<Either<L, A>> FilterT<L, A>(this TryOption<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryOption<Either<L, A>> Where<L, A>(this TryOption<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryOption<Either<L, B>> Select<L, A, B>(this TryOption<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryOption<Either<L, B>>, TryOption<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryOption<Either<L, C>> SelectMany<L, A, B, C>(
            this TryOption<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Either<L, A>>, TryOption<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MTryOption<Either<L, C>>, TryOption<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this TryOption<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryOption<EitherUnsafe<L, A>> ma) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static TryOption<EitherUnsafe<L, B>> BindT<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOption<EitherUnsafe<L, B>>, TryOption<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryOption<EitherUnsafe<L, B>> MapT<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryOption<EitherUnsafe<L, B>>, TryOption<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MTryOption<EitherUnsafe<L, A>>).Return(ma);

        public static TryOption<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MTryOption<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static TryOption<EitherUnsafe<L, A>> FilterT<L, A>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryOption<EitherUnsafe<L, A>> Where<L, A>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryOption<EitherUnsafe<L, B>> Select<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryOption<EitherUnsafe<L, B>>, TryOption<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryOption<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this TryOption<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<EitherUnsafe<L, A>>, TryOption<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MTryOption<EitherUnsafe<L, C>>, TryOption<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionTry
    {
        public static A SumT<NumA,  A>(this TryOption<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Try<A>> ma) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static TryOption<Try<B>> BindT< A, B>(this TryOption<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOption<Try<B>>, TryOption<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryOption<Try<B>> MapT< A, B>(this TryOption<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryOption<Try<B>>, TryOption<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Try<A>> ma, Action<A> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Try<A>> Lift< A>(Try<A> ma) =>
            default(MTryOption<Try<A>>).Return(ma);

        public static TryOption<Try<A>> Lift< A>(A a) =>
            default(MTryOption<Try<A>>).Return(default(MTry<A>).Return(a));

        public static TryOption<Try<A>> FilterT< A>(this TryOption<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryOption<Try<A>> Where< A>(this TryOption<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryOption<Try<B>> Select< A, B>(this TryOption<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryOption<Try<B>>, TryOption<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryOption<Try<C>> SelectMany< A, B, C>(
            this TryOption<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Try<A>>, TryOption<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MTryOption<Try<C>>, TryOption<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionTryAsync
    {
        public static A SumT<NumA,  A>(this TryOption<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<TryAsync<A>> ma) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static TryOption<TryAsync<B>> BindT< A, B>(this TryOption<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOption<TryAsync<B>>, TryOption<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryOption<TryAsync<B>> MapT< A, B>(this TryOption<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryOption<TryAsync<B>>, TryOption<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<TryAsync<A>> ma, Action<A> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MTryOption<TryAsync<A>>).Return(ma);

        public static TryOption<TryAsync<A>> Lift< A>(A a) =>
            default(MTryOption<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static TryOption<TryAsync<A>> FilterT< A>(this TryOption<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryOption<TryAsync<A>> Where< A>(this TryOption<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryOption<TryAsync<B>> Select< A, B>(this TryOption<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryOption<TryAsync<B>>, TryOption<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryOption<TryAsync<C>> SelectMany< A, B, C>(
            this TryOption<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<TryAsync<A>>, TryOption<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MTryOption<TryAsync<C>>, TryOption<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionTryOption
    {
        public static A SumT<NumA,  A>(this TryOption<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<TryOption<A>> ma) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static TryOption<TryOption<B>> BindT< A, B>(this TryOption<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOption<TryOption<B>>, TryOption<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryOption<TryOption<B>> MapT< A, B>(this TryOption<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryOption<TryOption<B>>, TryOption<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<TryOption<A>> ma, Action<A> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MTryOption<TryOption<A>>).Return(ma);

        public static TryOption<TryOption<A>> Lift< A>(A a) =>
            default(MTryOption<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static TryOption<TryOption<A>> FilterT< A>(this TryOption<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryOption<TryOption<A>> Where< A>(this TryOption<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryOption<TryOption<B>> Select< A, B>(this TryOption<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryOption<TryOption<B>>, TryOption<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryOption<TryOption<C>> SelectMany< A, B, C>(
            this TryOption<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<TryOption<A>>, TryOption<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MTryOption<TryOption<C>>, TryOption<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionTryOptionAsync
    {
        public static A SumT<NumA,  A>(this TryOption<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<TryOptionAsync<A>> ma) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static TryOption<TryOptionAsync<B>> BindT< A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOption<TryOptionAsync<B>>, TryOption<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryOption<TryOptionAsync<B>> MapT< A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryOption<TryOptionAsync<B>>, TryOption<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MTryOption<TryOptionAsync<A>>).Return(ma);

        public static TryOption<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MTryOption<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static TryOption<TryOptionAsync<A>> FilterT< A>(this TryOption<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryOption<TryOptionAsync<A>> Where< A>(this TryOption<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryOption<TryOptionAsync<B>> Select< A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryOption<TryOptionAsync<B>>, TryOption<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryOption<TryOptionAsync<C>> SelectMany< A, B, C>(
            this TryOption<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<TryOptionAsync<A>>, TryOption<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MTryOption<TryOptionAsync<C>>, TryOption<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionQue
    {
        public static A SumT<NumA,  A>(this TryOption<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Que<A>> ma) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static TryOption<Que<B>> BindT< A, B>(this TryOption<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOption<Que<B>>, TryOption<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryOption<Que<B>> MapT< A, B>(this TryOption<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryOption<Que<B>>, TryOption<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Que<A>> ma, Action<A> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Que<A>> Lift< A>(Que<A> ma) =>
            default(MTryOption<Que<A>>).Return(ma);

        public static TryOption<Que<A>> Lift< A>(A a) =>
            default(MTryOption<Que<A>>).Return(default(MQue<A>).Return(a));

        public static TryOption<Que<A>> FilterT< A>(this TryOption<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryOption<Que<A>> Where< A>(this TryOption<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryOption<Que<B>> Select< A, B>(this TryOption<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryOption<Que<B>>, TryOption<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryOption<Que<C>> SelectMany< A, B, C>(
            this TryOption<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Que<A>>, TryOption<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MTryOption<Que<C>>, TryOption<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionIEnumerable
    {
        public static A SumT<NumA,  A>(this TryOption<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<IEnumerable<A>> ma) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static TryOption<IEnumerable<B>> BindT< A, B>(this TryOption<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOption<IEnumerable<B>>, TryOption<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryOption<IEnumerable<B>> MapT< A, B>(this TryOption<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryOption<IEnumerable<B>>, TryOption<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MTryOption<IEnumerable<A>>).Return(ma);

        public static TryOption<IEnumerable<A>> Lift< A>(A a) =>
            default(MTryOption<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static TryOption<IEnumerable<A>> FilterT< A>(this TryOption<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryOption<IEnumerable<A>> Where< A>(this TryOption<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryOption<IEnumerable<B>> Select< A, B>(this TryOption<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryOption<IEnumerable<B>>, TryOption<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryOption<IEnumerable<C>> SelectMany< A, B, C>(
            this TryOption<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<IEnumerable<A>>, TryOption<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MTryOption<IEnumerable<C>>, TryOption<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionSet
    {
        public static A SumT<NumA,  A>(this TryOption<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Set<A>> ma) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static TryOption<Set<B>> BindT< A, B>(this TryOption<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOption<Set<B>>, TryOption<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryOption<Set<B>> MapT< A, B>(this TryOption<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryOption<Set<B>>, TryOption<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Set<A>> ma, Action<A> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Set<A>> Lift< A>(Set<A> ma) =>
            default(MTryOption<Set<A>>).Return(ma);

        public static TryOption<Set<A>> Lift< A>(A a) =>
            default(MTryOption<Set<A>>).Return(default(MSet<A>).Return(a));

        public static TryOption<Set<A>> FilterT< A>(this TryOption<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryOption<Set<A>> Where< A>(this TryOption<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryOption<Set<B>> Select< A, B>(this TryOption<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryOption<Set<B>>, TryOption<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryOption<Set<C>> SelectMany< A, B, C>(
            this TryOption<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Set<A>>, TryOption<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MTryOption<Set<C>>, TryOption<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionStck
    {
        public static A SumT<NumA,  A>(this TryOption<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOption<Stck<A>> ma) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static TryOption<Stck<B>> BindT< A, B>(this TryOption<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOption<Stck<B>>, TryOption<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryOption<Stck<B>> MapT< A, B>(this TryOption<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryOption<Stck<B>>, TryOption<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOption<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOption<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOption<Stck<A>> ma, Action<A> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOption<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MTryOption<Stck<A>>).Return(ma);

        public static TryOption<Stck<A>> Lift< A>(A a) =>
            default(MTryOption<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static TryOption<Stck<A>> FilterT< A>(this TryOption<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryOption<Stck<A>> Where< A>(this TryOption<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryOption<Stck<B>> Select< A, B>(this TryOption<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryOption<Stck<B>>, TryOption<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryOption<Stck<C>> SelectMany< A, B, C>(
            this TryOption<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOption<Stck<A>>, TryOption<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MTryOption<Stck<C>>, TryOption<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncArr
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Arr<A>> ma) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Arr<B>> BindT< A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOptionAsync<Arr<B>>, TryOptionAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryOptionAsync<Arr<B>> MapT< A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryOptionAsync<Arr<B>>, TryOptionAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Arr<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MTryOptionAsync<Arr<A>>).Return(ma);

        public static TryOptionAsync<Arr<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static TryOptionAsync<Arr<A>> FilterT< A>(this TryOptionAsync<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryOptionAsync<Arr<A>> Where< A>(this TryOptionAsync<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static TryOptionAsync<Arr<B>> Select< A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MTryOptionAsync<Arr<B>>, TryOptionAsync<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static TryOptionAsync<Arr<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Arr<A>>, TryOptionAsync<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MTryOptionAsync<Arr<C>>, TryOptionAsync<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncHashSet
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<HashSet<A>> ma) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<HashSet<B>> BindT< A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOptionAsync<HashSet<B>>, TryOptionAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryOptionAsync<HashSet<B>> MapT< A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryOptionAsync<HashSet<B>>, TryOptionAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<HashSet<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MTryOptionAsync<HashSet<A>>).Return(ma);

        public static TryOptionAsync<HashSet<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static TryOptionAsync<HashSet<A>> FilterT< A>(this TryOptionAsync<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryOptionAsync<HashSet<A>> Where< A>(this TryOptionAsync<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static TryOptionAsync<HashSet<B>> Select< A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MTryOptionAsync<HashSet<B>>, TryOptionAsync<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static TryOptionAsync<HashSet<C>> SelectMany< A, B, C>(
            this TryOptionAsync<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<HashSet<A>>, TryOptionAsync<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MTryOptionAsync<HashSet<C>>, TryOptionAsync<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncLst
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Lst<A>> ma) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Lst<B>> BindT< A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOptionAsync<Lst<B>>, TryOptionAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryOptionAsync<Lst<B>> MapT< A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryOptionAsync<Lst<B>>, TryOptionAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Lst<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MTryOptionAsync<Lst<A>>).Return(ma);

        public static TryOptionAsync<Lst<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static TryOptionAsync<Lst<A>> FilterT< A>(this TryOptionAsync<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryOptionAsync<Lst<A>> Where< A>(this TryOptionAsync<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static TryOptionAsync<Lst<B>> Select< A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MTryOptionAsync<Lst<B>>, TryOptionAsync<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static TryOptionAsync<Lst<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Lst<A>>, TryOptionAsync<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MTryOptionAsync<Lst<C>>, TryOptionAsync<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncOption
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Option<A>> ma) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Option<B>> BindT< A, B>(this TryOptionAsync<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOptionAsync<Option<B>>, TryOptionAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryOptionAsync<Option<B>> MapT< A, B>(this TryOptionAsync<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryOptionAsync<Option<B>>, TryOptionAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Option<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Option<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Option<A>> Lift< A>(Option<A> ma) =>
            default(MTryOptionAsync<Option<A>>).Return(ma);

        public static TryOptionAsync<Option<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Option<A>>).Return(default(MOption<A>).Return(a));

        public static TryOptionAsync<Option<A>> FilterT< A>(this TryOptionAsync<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryOptionAsync<Option<A>> Where< A>(this TryOptionAsync<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static TryOptionAsync<Option<B>> Select< A, B>(this TryOptionAsync<Option<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MTryOptionAsync<Option<B>>, TryOptionAsync<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static TryOptionAsync<Option<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Option<A>>, TryOptionAsync<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MTryOptionAsync<Option<C>>, TryOptionAsync<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncOptionUnsafe
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<OptionUnsafe<A>> ma) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<OptionUnsafe<B>> BindT< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOptionAsync<OptionUnsafe<B>>, TryOptionAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryOptionAsync<OptionUnsafe<B>> MapT< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryOptionAsync<OptionUnsafe<B>>, TryOptionAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MTryOptionAsync<OptionUnsafe<A>>).Return(ma);

        public static TryOptionAsync<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static TryOptionAsync<OptionUnsafe<A>> FilterT< A>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryOptionAsync<OptionUnsafe<A>> Where< A>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static TryOptionAsync<OptionUnsafe<B>> Select< A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MTryOptionAsync<OptionUnsafe<B>>, TryOptionAsync<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static TryOptionAsync<OptionUnsafe<C>> SelectMany< A, B, C>(
            this TryOptionAsync<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<OptionUnsafe<A>>, TryOptionAsync<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MTryOptionAsync<OptionUnsafe<C>>, TryOptionAsync<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncEither
    {
        public static A SumT<NumA, L, A>(this TryOptionAsync<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryOptionAsync<Either<L, A>> ma) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Either<L, B>> BindT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOptionAsync<Either<L, B>>, TryOptionAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryOptionAsync<Either<L, B>> MapT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryOptionAsync<Either<L, B>>, TryOptionAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MTryOptionAsync<Either<L, A>>).Return(ma);

        public static TryOptionAsync<Either<L, A>> Lift<L, A>(A a) =>
            default(MTryOptionAsync<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static TryOptionAsync<Either<L, A>> FilterT<L, A>(this TryOptionAsync<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryOptionAsync<Either<L, A>> Where<L, A>(this TryOptionAsync<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static TryOptionAsync<Either<L, B>> Select<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MTryOptionAsync<Either<L, B>>, TryOptionAsync<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static TryOptionAsync<Either<L, C>> SelectMany<L, A, B, C>(
            this TryOptionAsync<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Either<L, A>>, TryOptionAsync<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MTryOptionAsync<Either<L, C>>, TryOptionAsync<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this TryOptionAsync<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this TryOptionAsync<EitherUnsafe<L, A>> ma) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static TryOptionAsync<EitherUnsafe<L, B>> BindT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOptionAsync<EitherUnsafe<L, B>>, TryOptionAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryOptionAsync<EitherUnsafe<L, B>> MapT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryOptionAsync<EitherUnsafe<L, B>>, TryOptionAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MTryOptionAsync<EitherUnsafe<L, A>>).Return(ma);

        public static TryOptionAsync<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MTryOptionAsync<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static TryOptionAsync<EitherUnsafe<L, A>> FilterT<L, A>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryOptionAsync<EitherUnsafe<L, A>> Where<L, A>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static TryOptionAsync<EitherUnsafe<L, B>> Select<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MTryOptionAsync<EitherUnsafe<L, B>>, TryOptionAsync<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static TryOptionAsync<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this TryOptionAsync<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<EitherUnsafe<L, A>>, TryOptionAsync<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MTryOptionAsync<EitherUnsafe<L, C>>, TryOptionAsync<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncTry
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Try<A>> ma) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Try<B>> BindT< A, B>(this TryOptionAsync<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOptionAsync<Try<B>>, TryOptionAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryOptionAsync<Try<B>> MapT< A, B>(this TryOptionAsync<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryOptionAsync<Try<B>>, TryOptionAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Try<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Try<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Try<A>> Lift< A>(Try<A> ma) =>
            default(MTryOptionAsync<Try<A>>).Return(ma);

        public static TryOptionAsync<Try<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Try<A>>).Return(default(MTry<A>).Return(a));

        public static TryOptionAsync<Try<A>> FilterT< A>(this TryOptionAsync<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryOptionAsync<Try<A>> Where< A>(this TryOptionAsync<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static TryOptionAsync<Try<B>> Select< A, B>(this TryOptionAsync<Try<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MTryOptionAsync<Try<B>>, TryOptionAsync<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static TryOptionAsync<Try<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Try<A>>, TryOptionAsync<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MTryOptionAsync<Try<C>>, TryOptionAsync<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncTryAsync
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<TryAsync<A>> ma) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<TryAsync<B>> BindT< A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryAsync<B>>, TryOptionAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryOptionAsync<TryAsync<B>> MapT< A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryOptionAsync<TryAsync<B>>, TryOptionAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<TryAsync<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MTryOptionAsync<TryAsync<A>>).Return(ma);

        public static TryOptionAsync<TryAsync<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static TryOptionAsync<TryAsync<A>> FilterT< A>(this TryOptionAsync<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryOptionAsync<TryAsync<A>> Where< A>(this TryOptionAsync<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static TryOptionAsync<TryAsync<B>> Select< A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MTryOptionAsync<TryAsync<B>>, TryOptionAsync<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static TryOptionAsync<TryAsync<C>> SelectMany< A, B, C>(
            this TryOptionAsync<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<TryAsync<A>>, TryOptionAsync<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MTryOptionAsync<TryAsync<C>>, TryOptionAsync<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncTryOption
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<TryOption<A>> ma) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<TryOption<B>> BindT< A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOption<B>>, TryOptionAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryOptionAsync<TryOption<B>> MapT< A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryOptionAsync<TryOption<B>>, TryOptionAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<TryOption<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MTryOptionAsync<TryOption<A>>).Return(ma);

        public static TryOptionAsync<TryOption<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static TryOptionAsync<TryOption<A>> FilterT< A>(this TryOptionAsync<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryOptionAsync<TryOption<A>> Where< A>(this TryOptionAsync<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static TryOptionAsync<TryOption<B>> Select< A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MTryOptionAsync<TryOption<B>>, TryOptionAsync<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static TryOptionAsync<TryOption<C>> SelectMany< A, B, C>(
            this TryOptionAsync<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<TryOption<A>>, TryOptionAsync<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MTryOptionAsync<TryOption<C>>, TryOptionAsync<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncTryOptionAsync
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<TryOptionAsync<A>> ma) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<TryOptionAsync<B>> BindT< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOptionAsync<B>>, TryOptionAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryOptionAsync<TryOptionAsync<B>> MapT< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryOptionAsync<TryOptionAsync<B>>, TryOptionAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MTryOptionAsync<TryOptionAsync<A>>).Return(ma);

        public static TryOptionAsync<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static TryOptionAsync<TryOptionAsync<A>> FilterT< A>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryOptionAsync<TryOptionAsync<A>> Where< A>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static TryOptionAsync<TryOptionAsync<B>> Select< A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MTryOptionAsync<TryOptionAsync<B>>, TryOptionAsync<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static TryOptionAsync<TryOptionAsync<C>> SelectMany< A, B, C>(
            this TryOptionAsync<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<TryOptionAsync<A>>, TryOptionAsync<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MTryOptionAsync<TryOptionAsync<C>>, TryOptionAsync<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncQue
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Que<A>> ma) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Que<B>> BindT< A, B>(this TryOptionAsync<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOptionAsync<Que<B>>, TryOptionAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryOptionAsync<Que<B>> MapT< A, B>(this TryOptionAsync<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryOptionAsync<Que<B>>, TryOptionAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Que<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Que<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Que<A>> Lift< A>(Que<A> ma) =>
            default(MTryOptionAsync<Que<A>>).Return(ma);

        public static TryOptionAsync<Que<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Que<A>>).Return(default(MQue<A>).Return(a));

        public static TryOptionAsync<Que<A>> FilterT< A>(this TryOptionAsync<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryOptionAsync<Que<A>> Where< A>(this TryOptionAsync<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static TryOptionAsync<Que<B>> Select< A, B>(this TryOptionAsync<Que<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MTryOptionAsync<Que<B>>, TryOptionAsync<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static TryOptionAsync<Que<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Que<A>>, TryOptionAsync<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MTryOptionAsync<Que<C>>, TryOptionAsync<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncIEnumerable
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<IEnumerable<A>> ma) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<IEnumerable<B>> BindT< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOptionAsync<IEnumerable<B>>, TryOptionAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryOptionAsync<IEnumerable<B>> MapT< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryOptionAsync<IEnumerable<B>>, TryOptionAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MTryOptionAsync<IEnumerable<A>>).Return(ma);

        public static TryOptionAsync<IEnumerable<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static TryOptionAsync<IEnumerable<A>> FilterT< A>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryOptionAsync<IEnumerable<A>> Where< A>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static TryOptionAsync<IEnumerable<B>> Select< A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MTryOptionAsync<IEnumerable<B>>, TryOptionAsync<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static TryOptionAsync<IEnumerable<C>> SelectMany< A, B, C>(
            this TryOptionAsync<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<IEnumerable<A>>, TryOptionAsync<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MTryOptionAsync<IEnumerable<C>>, TryOptionAsync<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncSet
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Set<A>> ma) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Set<B>> BindT< A, B>(this TryOptionAsync<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOptionAsync<Set<B>>, TryOptionAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryOptionAsync<Set<B>> MapT< A, B>(this TryOptionAsync<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryOptionAsync<Set<B>>, TryOptionAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Set<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Set<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Set<A>> Lift< A>(Set<A> ma) =>
            default(MTryOptionAsync<Set<A>>).Return(ma);

        public static TryOptionAsync<Set<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Set<A>>).Return(default(MSet<A>).Return(a));

        public static TryOptionAsync<Set<A>> FilterT< A>(this TryOptionAsync<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryOptionAsync<Set<A>> Where< A>(this TryOptionAsync<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static TryOptionAsync<Set<B>> Select< A, B>(this TryOptionAsync<Set<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MTryOptionAsync<Set<B>>, TryOptionAsync<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static TryOptionAsync<Set<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Set<A>>, TryOptionAsync<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MTryOptionAsync<Set<C>>, TryOptionAsync<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class TryOptionAsyncStck
    {
        public static A SumT<NumA,  A>(this TryOptionAsync<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this TryOptionAsync<Stck<A>> ma) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static TryOptionAsync<Stck<B>> BindT< A, B>(this TryOptionAsync<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOptionAsync<Stck<B>>, TryOptionAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryOptionAsync<Stck<B>> MapT< A, B>(this TryOptionAsync<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryOptionAsync<Stck<B>>, TryOptionAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this TryOptionAsync<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this TryOptionAsync<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this TryOptionAsync<Stck<A>> ma, Action<A> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static TryOptionAsync<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MTryOptionAsync<Stck<A>>).Return(ma);

        public static TryOptionAsync<Stck<A>> Lift< A>(A a) =>
            default(MTryOptionAsync<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static TryOptionAsync<Stck<A>> FilterT< A>(this TryOptionAsync<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryOptionAsync<Stck<A>> Where< A>(this TryOptionAsync<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static TryOptionAsync<Stck<B>> Select< A, B>(this TryOptionAsync<Stck<A>> ma, Func<A, B> f) =>
            Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MTryOptionAsync<Stck<B>>, TryOptionAsync<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static TryOptionAsync<Stck<C>> SelectMany< A, B, C>(
            this TryOptionAsync<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MTryOptionAsync<Stck<A>>, TryOptionAsync<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MTryOptionAsync<Stck<C>>, TryOptionAsync<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class QueArr
    {
        public static A SumT<NumA,  A>(this Que<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Arr<A>> ma) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Que<Arr<B>> BindT< A, B>(this Que<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MQue<Arr<B>>, Que<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Que<Arr<B>> MapT< A, B>(this Que<Arr<A>> ma, Func<A, B> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MQue<Arr<B>>, Que<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Arr<A>> ma, Action<A> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MQue<Arr<A>>).Return(ma);

        public static Que<Arr<A>> Lift< A>(A a) =>
            default(MQue<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Que<Arr<A>> FilterT< A>(this Que<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Que<Arr<A>> Where< A>(this Que<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Que<Arr<B>> Select< A, B>(this Que<Arr<A>> ma, Func<A, B> f) =>
            Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MQue<Arr<B>>, Que<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Que<Arr<C>> SelectMany< A, B, C>(
            this Que<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Arr<A>>, Que<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MQue<Arr<C>>, Que<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class QueHashSet
    {
        public static A SumT<NumA,  A>(this Que<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<HashSet<A>> ma) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Que<HashSet<B>> BindT< A, B>(this Que<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MQue<HashSet<B>>, Que<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Que<HashSet<B>> MapT< A, B>(this Que<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MQue<HashSet<B>>, Que<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<HashSet<A>> ma, Action<A> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MQue<HashSet<A>>).Return(ma);

        public static Que<HashSet<A>> Lift< A>(A a) =>
            default(MQue<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Que<HashSet<A>> FilterT< A>(this Que<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Que<HashSet<A>> Where< A>(this Que<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Que<HashSet<B>> Select< A, B>(this Que<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MQue<HashSet<B>>, Que<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Que<HashSet<C>> SelectMany< A, B, C>(
            this Que<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<HashSet<A>>, Que<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MQue<HashSet<C>>, Que<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class QueLst
    {
        public static A SumT<NumA,  A>(this Que<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Lst<A>> ma) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Que<Lst<B>> BindT< A, B>(this Que<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MQue<Lst<B>>, Que<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Que<Lst<B>> MapT< A, B>(this Que<Lst<A>> ma, Func<A, B> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MQue<Lst<B>>, Que<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Lst<A>> ma, Action<A> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MQue<Lst<A>>).Return(ma);

        public static Que<Lst<A>> Lift< A>(A a) =>
            default(MQue<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Que<Lst<A>> FilterT< A>(this Que<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Que<Lst<A>> Where< A>(this Que<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Que<Lst<B>> Select< A, B>(this Que<Lst<A>> ma, Func<A, B> f) =>
            Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MQue<Lst<B>>, Que<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Que<Lst<C>> SelectMany< A, B, C>(
            this Que<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Lst<A>>, Que<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MQue<Lst<C>>, Que<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class QueOption
    {
        public static A SumT<NumA,  A>(this Que<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Option<A>> ma) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Que<Option<B>> BindT< A, B>(this Que<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MQue<Option<B>>, Que<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Que<Option<B>> MapT< A, B>(this Que<Option<A>> ma, Func<A, B> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MQue<Option<B>>, Que<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Option<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Option<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Option<A>> ma, Action<A> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Option<A>> Lift< A>(Option<A> ma) =>
            default(MQue<Option<A>>).Return(ma);

        public static Que<Option<A>> Lift< A>(A a) =>
            default(MQue<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Que<Option<A>> FilterT< A>(this Que<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Que<Option<A>> Where< A>(this Que<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Que<Option<B>> Select< A, B>(this Que<Option<A>> ma, Func<A, B> f) =>
            Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MQue<Option<B>>, Que<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Que<Option<C>> SelectMany< A, B, C>(
            this Que<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Option<A>>, Que<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MQue<Option<C>>, Que<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class QueOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Que<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<OptionUnsafe<A>> ma) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Que<OptionUnsafe<B>> BindT< A, B>(this Que<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MQue<OptionUnsafe<B>>, Que<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Que<OptionUnsafe<B>> MapT< A, B>(this Que<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MQue<OptionUnsafe<B>>, Que<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MQue<OptionUnsafe<A>>).Return(ma);

        public static Que<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MQue<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Que<OptionUnsafe<A>> FilterT< A>(this Que<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Que<OptionUnsafe<A>> Where< A>(this Que<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Que<OptionUnsafe<B>> Select< A, B>(this Que<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MQue<OptionUnsafe<B>>, Que<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Que<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Que<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<OptionUnsafe<A>>, Que<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MQue<OptionUnsafe<C>>, Que<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class QueEither
    {
        public static A SumT<NumA, L, A>(this Que<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Que<Either<L, A>> ma) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Que<Either<L, B>> BindT<L, A, B>(this Que<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MQue<Either<L, B>>, Que<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Que<Either<L, B>> MapT<L, A, B>(this Que<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MQue<Either<L, B>>, Que<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Que<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Que<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Que<Either<L, A>> ma, Action<A> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MQue<Either<L, A>>).Return(ma);

        public static Que<Either<L, A>> Lift<L, A>(A a) =>
            default(MQue<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Que<Either<L, A>> FilterT<L, A>(this Que<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Que<Either<L, A>> Where<L, A>(this Que<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Que<Either<L, B>> Select<L, A, B>(this Que<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MQue<Either<L, B>>, Que<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Que<Either<L, C>> SelectMany<L, A, B, C>(
            this Que<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Either<L, A>>, Que<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MQue<Either<L, C>>, Que<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class QueEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Que<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Que<EitherUnsafe<L, A>> ma) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Que<EitherUnsafe<L, B>> BindT<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MQue<EitherUnsafe<L, B>>, Que<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Que<EitherUnsafe<L, B>> MapT<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MQue<EitherUnsafe<L, B>>, Que<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MQue<EitherUnsafe<L, A>>).Return(ma);

        public static Que<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MQue<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Que<EitherUnsafe<L, A>> FilterT<L, A>(this Que<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Que<EitherUnsafe<L, A>> Where<L, A>(this Que<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Que<EitherUnsafe<L, B>> Select<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MQue<EitherUnsafe<L, B>>, Que<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Que<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Que<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<EitherUnsafe<L, A>>, Que<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MQue<EitherUnsafe<L, C>>, Que<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class QueTry
    {
        public static A SumT<NumA,  A>(this Que<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Try<A>> ma) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Que<Try<B>> BindT< A, B>(this Que<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MQue<Try<B>>, Que<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Que<Try<B>> MapT< A, B>(this Que<Try<A>> ma, Func<A, B> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MQue<Try<B>>, Que<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Try<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Try<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Try<A>> ma, Action<A> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Try<A>> Lift< A>(Try<A> ma) =>
            default(MQue<Try<A>>).Return(ma);

        public static Que<Try<A>> Lift< A>(A a) =>
            default(MQue<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Que<Try<A>> FilterT< A>(this Que<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Que<Try<A>> Where< A>(this Que<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Que<Try<B>> Select< A, B>(this Que<Try<A>> ma, Func<A, B> f) =>
            Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MQue<Try<B>>, Que<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Que<Try<C>> SelectMany< A, B, C>(
            this Que<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Try<A>>, Que<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MQue<Try<C>>, Que<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class QueTryAsync
    {
        public static A SumT<NumA,  A>(this Que<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<TryAsync<A>> ma) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Que<TryAsync<B>> BindT< A, B>(this Que<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MQue<TryAsync<B>>, Que<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Que<TryAsync<B>> MapT< A, B>(this Que<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MQue<TryAsync<B>>, Que<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<TryAsync<A>> ma, Action<A> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MQue<TryAsync<A>>).Return(ma);

        public static Que<TryAsync<A>> Lift< A>(A a) =>
            default(MQue<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Que<TryAsync<A>> FilterT< A>(this Que<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Que<TryAsync<A>> Where< A>(this Que<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Que<TryAsync<B>> Select< A, B>(this Que<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MQue<TryAsync<B>>, Que<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Que<TryAsync<C>> SelectMany< A, B, C>(
            this Que<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<TryAsync<A>>, Que<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MQue<TryAsync<C>>, Que<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class QueTryOption
    {
        public static A SumT<NumA,  A>(this Que<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<TryOption<A>> ma) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Que<TryOption<B>> BindT< A, B>(this Que<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MQue<TryOption<B>>, Que<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Que<TryOption<B>> MapT< A, B>(this Que<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MQue<TryOption<B>>, Que<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<TryOption<A>> ma, Action<A> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MQue<TryOption<A>>).Return(ma);

        public static Que<TryOption<A>> Lift< A>(A a) =>
            default(MQue<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Que<TryOption<A>> FilterT< A>(this Que<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Que<TryOption<A>> Where< A>(this Que<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Que<TryOption<B>> Select< A, B>(this Que<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MQue<TryOption<B>>, Que<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Que<TryOption<C>> SelectMany< A, B, C>(
            this Que<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<TryOption<A>>, Que<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MQue<TryOption<C>>, Que<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class QueTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Que<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<TryOptionAsync<A>> ma) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Que<TryOptionAsync<B>> BindT< A, B>(this Que<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MQue<TryOptionAsync<B>>, Que<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Que<TryOptionAsync<B>> MapT< A, B>(this Que<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MQue<TryOptionAsync<B>>, Que<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MQue<TryOptionAsync<A>>).Return(ma);

        public static Que<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MQue<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Que<TryOptionAsync<A>> FilterT< A>(this Que<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Que<TryOptionAsync<A>> Where< A>(this Que<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Que<TryOptionAsync<B>> Select< A, B>(this Que<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MQue<TryOptionAsync<B>>, Que<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Que<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Que<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<TryOptionAsync<A>>, Que<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MQue<TryOptionAsync<C>>, Que<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class QueQue
    {
        public static A SumT<NumA,  A>(this Que<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Que<A>> ma) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Que<Que<B>> BindT< A, B>(this Que<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MQue<Que<B>>, Que<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Que<Que<B>> MapT< A, B>(this Que<Que<A>> ma, Func<A, B> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MQue<Que<B>>, Que<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Que<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Que<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Que<A>> ma, Action<A> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Que<A>> Lift< A>(Que<A> ma) =>
            default(MQue<Que<A>>).Return(ma);

        public static Que<Que<A>> Lift< A>(A a) =>
            default(MQue<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Que<Que<A>> FilterT< A>(this Que<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Que<Que<A>> Where< A>(this Que<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Que<Que<B>> Select< A, B>(this Que<Que<A>> ma, Func<A, B> f) =>
            Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MQue<Que<B>>, Que<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Que<Que<C>> SelectMany< A, B, C>(
            this Que<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Que<A>>, Que<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MQue<Que<C>>, Que<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class QueIEnumerable
    {
        public static A SumT<NumA,  A>(this Que<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<IEnumerable<A>> ma) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Que<IEnumerable<B>> BindT< A, B>(this Que<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MQue<IEnumerable<B>>, Que<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Que<IEnumerable<B>> MapT< A, B>(this Que<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MQue<IEnumerable<B>>, Que<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MQue<IEnumerable<A>>).Return(ma);

        public static Que<IEnumerable<A>> Lift< A>(A a) =>
            default(MQue<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Que<IEnumerable<A>> FilterT< A>(this Que<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Que<IEnumerable<A>> Where< A>(this Que<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Que<IEnumerable<B>> Select< A, B>(this Que<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MQue<IEnumerable<B>>, Que<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Que<IEnumerable<C>> SelectMany< A, B, C>(
            this Que<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<IEnumerable<A>>, Que<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MQue<IEnumerable<C>>, Que<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class QueSet
    {
        public static A SumT<NumA,  A>(this Que<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Set<A>> ma) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Que<Set<B>> BindT< A, B>(this Que<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MQue<Set<B>>, Que<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Que<Set<B>> MapT< A, B>(this Que<Set<A>> ma, Func<A, B> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MQue<Set<B>>, Que<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Set<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Set<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Set<A>> ma, Action<A> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Set<A>> Lift< A>(Set<A> ma) =>
            default(MQue<Set<A>>).Return(ma);

        public static Que<Set<A>> Lift< A>(A a) =>
            default(MQue<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Que<Set<A>> FilterT< A>(this Que<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Que<Set<A>> Where< A>(this Que<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Que<Set<B>> Select< A, B>(this Que<Set<A>> ma, Func<A, B> f) =>
            Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MQue<Set<B>>, Que<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Que<Set<C>> SelectMany< A, B, C>(
            this Que<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Set<A>>, Que<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MQue<Set<C>>, Que<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class QueStck
    {
        public static A SumT<NumA,  A>(this Que<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Que<Stck<A>> ma) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Que<Stck<B>> BindT< A, B>(this Que<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MQue<Stck<B>>, Que<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Que<Stck<B>> MapT< A, B>(this Que<Stck<A>> ma, Func<A, B> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MQue<Stck<B>>, Que<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Que<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Que<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Que<Stck<A>> ma, Action<A> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Que<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MQue<Stck<A>>).Return(ma);

        public static Que<Stck<A>> Lift< A>(A a) =>
            default(MQue<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Que<Stck<A>> FilterT< A>(this Que<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Que<Stck<A>> Where< A>(this Que<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Que<Stck<B>> Select< A, B>(this Que<Stck<A>> ma, Func<A, B> f) =>
            Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MQue<Stck<B>>, Que<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Que<Stck<C>> SelectMany< A, B, C>(
            this Que<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MQue<Stck<A>>, Que<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MQue<Stck<C>>, Que<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableArr
    {
        public static A SumT<NumA,  A>(this IEnumerable<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Arr<A>> ma) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static IEnumerable<Arr<B>> BindT< A, B>(this IEnumerable<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSeq<Arr<B>>, IEnumerable<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static IEnumerable<Arr<B>> MapT< A, B>(this IEnumerable<Arr<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MSeq<Arr<B>>, IEnumerable<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Arr<A>> ma, Action<A> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MSeq<Arr<A>>).Return(ma);

        public static IEnumerable<Arr<A>> Lift< A>(A a) =>
            default(MSeq<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static IEnumerable<Arr<A>> FilterT< A>(this IEnumerable<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static IEnumerable<Arr<A>> Where< A>(this IEnumerable<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static IEnumerable<Arr<B>> Select< A, B>(this IEnumerable<Arr<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MSeq<Arr<B>>, IEnumerable<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static IEnumerable<Arr<C>> SelectMany< A, B, C>(
            this IEnumerable<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Arr<A>>, IEnumerable<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MSeq<Arr<C>>, IEnumerable<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableHashSet
    {
        public static A SumT<NumA,  A>(this IEnumerable<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<HashSet<A>> ma) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static IEnumerable<HashSet<B>> BindT< A, B>(this IEnumerable<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSeq<HashSet<B>>, IEnumerable<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static IEnumerable<HashSet<B>> MapT< A, B>(this IEnumerable<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MSeq<HashSet<B>>, IEnumerable<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<HashSet<A>> ma, Action<A> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MSeq<HashSet<A>>).Return(ma);

        public static IEnumerable<HashSet<A>> Lift< A>(A a) =>
            default(MSeq<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static IEnumerable<HashSet<A>> FilterT< A>(this IEnumerable<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static IEnumerable<HashSet<A>> Where< A>(this IEnumerable<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static IEnumerable<HashSet<B>> Select< A, B>(this IEnumerable<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MSeq<HashSet<B>>, IEnumerable<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static IEnumerable<HashSet<C>> SelectMany< A, B, C>(
            this IEnumerable<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<HashSet<A>>, IEnumerable<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MSeq<HashSet<C>>, IEnumerable<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableLst
    {
        public static A SumT<NumA,  A>(this IEnumerable<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Lst<A>> ma) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static IEnumerable<Lst<B>> BindT< A, B>(this IEnumerable<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSeq<Lst<B>>, IEnumerable<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static IEnumerable<Lst<B>> MapT< A, B>(this IEnumerable<Lst<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MSeq<Lst<B>>, IEnumerable<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Lst<A>> ma, Action<A> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MSeq<Lst<A>>).Return(ma);

        public static IEnumerable<Lst<A>> Lift< A>(A a) =>
            default(MSeq<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static IEnumerable<Lst<A>> FilterT< A>(this IEnumerable<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static IEnumerable<Lst<A>> Where< A>(this IEnumerable<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static IEnumerable<Lst<B>> Select< A, B>(this IEnumerable<Lst<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MSeq<Lst<B>>, IEnumerable<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static IEnumerable<Lst<C>> SelectMany< A, B, C>(
            this IEnumerable<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Lst<A>>, IEnumerable<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MSeq<Lst<C>>, IEnumerable<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableOption
    {
        public static A SumT<NumA,  A>(this IEnumerable<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Option<A>> ma) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static IEnumerable<Option<B>> BindT< A, B>(this IEnumerable<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSeq<Option<B>>, IEnumerable<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static IEnumerable<Option<B>> MapT< A, B>(this IEnumerable<Option<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MSeq<Option<B>>, IEnumerable<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Option<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Option<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Option<A>> ma, Action<A> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Option<A>> Lift< A>(Option<A> ma) =>
            default(MSeq<Option<A>>).Return(ma);

        public static IEnumerable<Option<A>> Lift< A>(A a) =>
            default(MSeq<Option<A>>).Return(default(MOption<A>).Return(a));

        public static IEnumerable<Option<A>> FilterT< A>(this IEnumerable<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static IEnumerable<Option<A>> Where< A>(this IEnumerable<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static IEnumerable<Option<B>> Select< A, B>(this IEnumerable<Option<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MSeq<Option<B>>, IEnumerable<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static IEnumerable<Option<C>> SelectMany< A, B, C>(
            this IEnumerable<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Option<A>>, IEnumerable<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MSeq<Option<C>>, IEnumerable<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableOptionUnsafe
    {
        public static A SumT<NumA,  A>(this IEnumerable<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<OptionUnsafe<A>> ma) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static IEnumerable<OptionUnsafe<B>> BindT< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSeq<OptionUnsafe<B>>, IEnumerable<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static IEnumerable<OptionUnsafe<B>> MapT< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MSeq<OptionUnsafe<B>>, IEnumerable<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MSeq<OptionUnsafe<A>>).Return(ma);

        public static IEnumerable<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MSeq<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static IEnumerable<OptionUnsafe<A>> FilterT< A>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static IEnumerable<OptionUnsafe<A>> Where< A>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static IEnumerable<OptionUnsafe<B>> Select< A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MSeq<OptionUnsafe<B>>, IEnumerable<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static IEnumerable<OptionUnsafe<C>> SelectMany< A, B, C>(
            this IEnumerable<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<OptionUnsafe<A>>, IEnumerable<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MSeq<OptionUnsafe<C>>, IEnumerable<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableEither
    {
        public static A SumT<NumA, L, A>(this IEnumerable<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this IEnumerable<Either<L, A>> ma) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static IEnumerable<Either<L, B>> BindT<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSeq<Either<L, B>>, IEnumerable<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static IEnumerable<Either<L, B>> MapT<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MSeq<Either<L, B>>, IEnumerable<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this IEnumerable<Either<L, A>> ma, Action<A> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MSeq<Either<L, A>>).Return(ma);

        public static IEnumerable<Either<L, A>> Lift<L, A>(A a) =>
            default(MSeq<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static IEnumerable<Either<L, A>> FilterT<L, A>(this IEnumerable<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static IEnumerable<Either<L, A>> Where<L, A>(this IEnumerable<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static IEnumerable<Either<L, B>> Select<L, A, B>(this IEnumerable<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MSeq<Either<L, B>>, IEnumerable<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static IEnumerable<Either<L, C>> SelectMany<L, A, B, C>(
            this IEnumerable<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Either<L, A>>, IEnumerable<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MSeq<Either<L, C>>, IEnumerable<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this IEnumerable<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this IEnumerable<EitherUnsafe<L, A>> ma) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static IEnumerable<EitherUnsafe<L, B>> BindT<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSeq<EitherUnsafe<L, B>>, IEnumerable<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static IEnumerable<EitherUnsafe<L, B>> MapT<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MSeq<EitherUnsafe<L, B>>, IEnumerable<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MSeq<EitherUnsafe<L, A>>).Return(ma);

        public static IEnumerable<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MSeq<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static IEnumerable<EitherUnsafe<L, A>> FilterT<L, A>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static IEnumerable<EitherUnsafe<L, A>> Where<L, A>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static IEnumerable<EitherUnsafe<L, B>> Select<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MSeq<EitherUnsafe<L, B>>, IEnumerable<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static IEnumerable<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this IEnumerable<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<EitherUnsafe<L, A>>, IEnumerable<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MSeq<EitherUnsafe<L, C>>, IEnumerable<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableTry
    {
        public static A SumT<NumA,  A>(this IEnumerable<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Try<A>> ma) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static IEnumerable<Try<B>> BindT< A, B>(this IEnumerable<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSeq<Try<B>>, IEnumerable<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static IEnumerable<Try<B>> MapT< A, B>(this IEnumerable<Try<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MSeq<Try<B>>, IEnumerable<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Try<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Try<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Try<A>> ma, Action<A> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Try<A>> Lift< A>(Try<A> ma) =>
            default(MSeq<Try<A>>).Return(ma);

        public static IEnumerable<Try<A>> Lift< A>(A a) =>
            default(MSeq<Try<A>>).Return(default(MTry<A>).Return(a));

        public static IEnumerable<Try<A>> FilterT< A>(this IEnumerable<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static IEnumerable<Try<A>> Where< A>(this IEnumerable<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static IEnumerable<Try<B>> Select< A, B>(this IEnumerable<Try<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MSeq<Try<B>>, IEnumerable<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static IEnumerable<Try<C>> SelectMany< A, B, C>(
            this IEnumerable<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Try<A>>, IEnumerable<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MSeq<Try<C>>, IEnumerable<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableTryAsync
    {
        public static A SumT<NumA,  A>(this IEnumerable<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<TryAsync<A>> ma) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static IEnumerable<TryAsync<B>> BindT< A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSeq<TryAsync<B>>, IEnumerable<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static IEnumerable<TryAsync<B>> MapT< A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MSeq<TryAsync<B>>, IEnumerable<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<TryAsync<A>> ma, Action<A> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MSeq<TryAsync<A>>).Return(ma);

        public static IEnumerable<TryAsync<A>> Lift< A>(A a) =>
            default(MSeq<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static IEnumerable<TryAsync<A>> FilterT< A>(this IEnumerable<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static IEnumerable<TryAsync<A>> Where< A>(this IEnumerable<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static IEnumerable<TryAsync<B>> Select< A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MSeq<TryAsync<B>>, IEnumerable<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static IEnumerable<TryAsync<C>> SelectMany< A, B, C>(
            this IEnumerable<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<TryAsync<A>>, IEnumerable<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MSeq<TryAsync<C>>, IEnumerable<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableTryOption
    {
        public static A SumT<NumA,  A>(this IEnumerable<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<TryOption<A>> ma) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static IEnumerable<TryOption<B>> BindT< A, B>(this IEnumerable<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSeq<TryOption<B>>, IEnumerable<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static IEnumerable<TryOption<B>> MapT< A, B>(this IEnumerable<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MSeq<TryOption<B>>, IEnumerable<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<TryOption<A>> ma, Action<A> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MSeq<TryOption<A>>).Return(ma);

        public static IEnumerable<TryOption<A>> Lift< A>(A a) =>
            default(MSeq<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static IEnumerable<TryOption<A>> FilterT< A>(this IEnumerable<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static IEnumerable<TryOption<A>> Where< A>(this IEnumerable<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static IEnumerable<TryOption<B>> Select< A, B>(this IEnumerable<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MSeq<TryOption<B>>, IEnumerable<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static IEnumerable<TryOption<C>> SelectMany< A, B, C>(
            this IEnumerable<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<TryOption<A>>, IEnumerable<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MSeq<TryOption<C>>, IEnumerable<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableTryOptionAsync
    {
        public static A SumT<NumA,  A>(this IEnumerable<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<TryOptionAsync<A>> ma) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static IEnumerable<TryOptionAsync<B>> BindT< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSeq<TryOptionAsync<B>>, IEnumerable<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static IEnumerable<TryOptionAsync<B>> MapT< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MSeq<TryOptionAsync<B>>, IEnumerable<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MSeq<TryOptionAsync<A>>).Return(ma);

        public static IEnumerable<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MSeq<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static IEnumerable<TryOptionAsync<A>> FilterT< A>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static IEnumerable<TryOptionAsync<A>> Where< A>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static IEnumerable<TryOptionAsync<B>> Select< A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MSeq<TryOptionAsync<B>>, IEnumerable<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static IEnumerable<TryOptionAsync<C>> SelectMany< A, B, C>(
            this IEnumerable<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<TryOptionAsync<A>>, IEnumerable<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MSeq<TryOptionAsync<C>>, IEnumerable<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableQue
    {
        public static A SumT<NumA,  A>(this IEnumerable<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Que<A>> ma) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static IEnumerable<Que<B>> BindT< A, B>(this IEnumerable<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSeq<Que<B>>, IEnumerable<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static IEnumerable<Que<B>> MapT< A, B>(this IEnumerable<Que<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MSeq<Que<B>>, IEnumerable<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Que<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Que<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Que<A>> ma, Action<A> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Que<A>> Lift< A>(Que<A> ma) =>
            default(MSeq<Que<A>>).Return(ma);

        public static IEnumerable<Que<A>> Lift< A>(A a) =>
            default(MSeq<Que<A>>).Return(default(MQue<A>).Return(a));

        public static IEnumerable<Que<A>> FilterT< A>(this IEnumerable<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static IEnumerable<Que<A>> Where< A>(this IEnumerable<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static IEnumerable<Que<B>> Select< A, B>(this IEnumerable<Que<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MSeq<Que<B>>, IEnumerable<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static IEnumerable<Que<C>> SelectMany< A, B, C>(
            this IEnumerable<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Que<A>>, IEnumerable<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MSeq<Que<C>>, IEnumerable<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableIEnumerable
    {
        public static A SumT<NumA,  A>(this IEnumerable<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<IEnumerable<A>> ma) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static IEnumerable<IEnumerable<B>> BindT< A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSeq<IEnumerable<B>>, IEnumerable<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static IEnumerable<IEnumerable<B>> MapT< A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MSeq<IEnumerable<B>>, IEnumerable<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MSeq<IEnumerable<A>>).Return(ma);

        public static IEnumerable<IEnumerable<A>> Lift< A>(A a) =>
            default(MSeq<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static IEnumerable<IEnumerable<A>> FilterT< A>(this IEnumerable<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static IEnumerable<IEnumerable<A>> Where< A>(this IEnumerable<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static IEnumerable<IEnumerable<B>> Select< A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MSeq<IEnumerable<B>>, IEnumerable<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static IEnumerable<IEnumerable<C>> SelectMany< A, B, C>(
            this IEnumerable<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<IEnumerable<A>>, IEnumerable<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MSeq<IEnumerable<C>>, IEnumerable<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableSet
    {
        public static A SumT<NumA,  A>(this IEnumerable<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Set<A>> ma) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static IEnumerable<Set<B>> BindT< A, B>(this IEnumerable<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSeq<Set<B>>, IEnumerable<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static IEnumerable<Set<B>> MapT< A, B>(this IEnumerable<Set<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MSeq<Set<B>>, IEnumerable<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Set<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Set<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Set<A>> ma, Action<A> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Set<A>> Lift< A>(Set<A> ma) =>
            default(MSeq<Set<A>>).Return(ma);

        public static IEnumerable<Set<A>> Lift< A>(A a) =>
            default(MSeq<Set<A>>).Return(default(MSet<A>).Return(a));

        public static IEnumerable<Set<A>> FilterT< A>(this IEnumerable<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static IEnumerable<Set<A>> Where< A>(this IEnumerable<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static IEnumerable<Set<B>> Select< A, B>(this IEnumerable<Set<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MSeq<Set<B>>, IEnumerable<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static IEnumerable<Set<C>> SelectMany< A, B, C>(
            this IEnumerable<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Set<A>>, IEnumerable<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MSeq<Set<C>>, IEnumerable<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class IEnumerableStck
    {
        public static A SumT<NumA,  A>(this IEnumerable<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this IEnumerable<Stck<A>> ma) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static IEnumerable<Stck<B>> BindT< A, B>(this IEnumerable<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSeq<Stck<B>>, IEnumerable<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static IEnumerable<Stck<B>> MapT< A, B>(this IEnumerable<Stck<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MSeq<Stck<B>>, IEnumerable<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this IEnumerable<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this IEnumerable<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this IEnumerable<Stck<A>> ma, Action<A> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static IEnumerable<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MSeq<Stck<A>>).Return(ma);

        public static IEnumerable<Stck<A>> Lift< A>(A a) =>
            default(MSeq<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static IEnumerable<Stck<A>> FilterT< A>(this IEnumerable<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static IEnumerable<Stck<A>> Where< A>(this IEnumerable<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static IEnumerable<Stck<B>> Select< A, B>(this IEnumerable<Stck<A>> ma, Func<A, B> f) =>
            Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MSeq<Stck<B>>, IEnumerable<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static IEnumerable<Stck<C>> SelectMany< A, B, C>(
            this IEnumerable<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSeq<Stck<A>>, IEnumerable<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MSeq<Stck<C>>, IEnumerable<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class SetArr
    {
        public static A SumT<NumA,  A>(this Set<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Arr<A>> ma) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Set<Arr<B>> BindT< A, B>(this Set<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSet<Arr<B>>, Set<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Set<Arr<B>> MapT< A, B>(this Set<Arr<A>> ma, Func<A, B> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MSet<Arr<B>>, Set<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Arr<A>> ma, Action<A> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MSet<Arr<A>>).Return(ma);

        public static Set<Arr<A>> Lift< A>(A a) =>
            default(MSet<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Set<Arr<A>> FilterT< A>(this Set<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Set<Arr<A>> Where< A>(this Set<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Set<Arr<B>> Select< A, B>(this Set<Arr<A>> ma, Func<A, B> f) =>
            Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MSet<Arr<B>>, Set<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Set<Arr<C>> SelectMany< A, B, C>(
            this Set<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Arr<A>>, Set<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MSet<Arr<C>>, Set<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class SetHashSet
    {
        public static A SumT<NumA,  A>(this Set<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<HashSet<A>> ma) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Set<HashSet<B>> BindT< A, B>(this Set<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSet<HashSet<B>>, Set<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Set<HashSet<B>> MapT< A, B>(this Set<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MSet<HashSet<B>>, Set<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<HashSet<A>> ma, Action<A> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MSet<HashSet<A>>).Return(ma);

        public static Set<HashSet<A>> Lift< A>(A a) =>
            default(MSet<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Set<HashSet<A>> FilterT< A>(this Set<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Set<HashSet<A>> Where< A>(this Set<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Set<HashSet<B>> Select< A, B>(this Set<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MSet<HashSet<B>>, Set<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Set<HashSet<C>> SelectMany< A, B, C>(
            this Set<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<HashSet<A>>, Set<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MSet<HashSet<C>>, Set<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class SetLst
    {
        public static A SumT<NumA,  A>(this Set<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Lst<A>> ma) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Set<Lst<B>> BindT< A, B>(this Set<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSet<Lst<B>>, Set<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Set<Lst<B>> MapT< A, B>(this Set<Lst<A>> ma, Func<A, B> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MSet<Lst<B>>, Set<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Lst<A>> ma, Action<A> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MSet<Lst<A>>).Return(ma);

        public static Set<Lst<A>> Lift< A>(A a) =>
            default(MSet<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Set<Lst<A>> FilterT< A>(this Set<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Set<Lst<A>> Where< A>(this Set<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Set<Lst<B>> Select< A, B>(this Set<Lst<A>> ma, Func<A, B> f) =>
            Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MSet<Lst<B>>, Set<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Set<Lst<C>> SelectMany< A, B, C>(
            this Set<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Lst<A>>, Set<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MSet<Lst<C>>, Set<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class SetOption
    {
        public static A SumT<NumA,  A>(this Set<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Option<A>> ma) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Set<Option<B>> BindT< A, B>(this Set<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSet<Option<B>>, Set<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Set<Option<B>> MapT< A, B>(this Set<Option<A>> ma, Func<A, B> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MSet<Option<B>>, Set<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Option<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Option<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Option<A>> ma, Action<A> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Option<A>> Lift< A>(Option<A> ma) =>
            default(MSet<Option<A>>).Return(ma);

        public static Set<Option<A>> Lift< A>(A a) =>
            default(MSet<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Set<Option<A>> FilterT< A>(this Set<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Set<Option<A>> Where< A>(this Set<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Set<Option<B>> Select< A, B>(this Set<Option<A>> ma, Func<A, B> f) =>
            Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MSet<Option<B>>, Set<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Set<Option<C>> SelectMany< A, B, C>(
            this Set<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Option<A>>, Set<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MSet<Option<C>>, Set<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class SetOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Set<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<OptionUnsafe<A>> ma) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Set<OptionUnsafe<B>> BindT< A, B>(this Set<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSet<OptionUnsafe<B>>, Set<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Set<OptionUnsafe<B>> MapT< A, B>(this Set<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MSet<OptionUnsafe<B>>, Set<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MSet<OptionUnsafe<A>>).Return(ma);

        public static Set<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MSet<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Set<OptionUnsafe<A>> FilterT< A>(this Set<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Set<OptionUnsafe<A>> Where< A>(this Set<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Set<OptionUnsafe<B>> Select< A, B>(this Set<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MSet<OptionUnsafe<B>>, Set<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Set<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Set<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<OptionUnsafe<A>>, Set<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MSet<OptionUnsafe<C>>, Set<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class SetEither
    {
        public static A SumT<NumA, L, A>(this Set<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Set<Either<L, A>> ma) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Set<Either<L, B>> BindT<L, A, B>(this Set<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSet<Either<L, B>>, Set<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Set<Either<L, B>> MapT<L, A, B>(this Set<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MSet<Either<L, B>>, Set<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Set<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Set<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Set<Either<L, A>> ma, Action<A> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MSet<Either<L, A>>).Return(ma);

        public static Set<Either<L, A>> Lift<L, A>(A a) =>
            default(MSet<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Set<Either<L, A>> FilterT<L, A>(this Set<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Set<Either<L, A>> Where<L, A>(this Set<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Set<Either<L, B>> Select<L, A, B>(this Set<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MSet<Either<L, B>>, Set<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Set<Either<L, C>> SelectMany<L, A, B, C>(
            this Set<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Either<L, A>>, Set<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MSet<Either<L, C>>, Set<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class SetEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Set<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Set<EitherUnsafe<L, A>> ma) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Set<EitherUnsafe<L, B>> BindT<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSet<EitherUnsafe<L, B>>, Set<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Set<EitherUnsafe<L, B>> MapT<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MSet<EitherUnsafe<L, B>>, Set<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MSet<EitherUnsafe<L, A>>).Return(ma);

        public static Set<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MSet<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Set<EitherUnsafe<L, A>> FilterT<L, A>(this Set<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Set<EitherUnsafe<L, A>> Where<L, A>(this Set<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Set<EitherUnsafe<L, B>> Select<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MSet<EitherUnsafe<L, B>>, Set<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Set<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Set<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<EitherUnsafe<L, A>>, Set<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MSet<EitherUnsafe<L, C>>, Set<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class SetTry
    {
        public static A SumT<NumA,  A>(this Set<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Try<A>> ma) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Set<Try<B>> BindT< A, B>(this Set<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSet<Try<B>>, Set<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Set<Try<B>> MapT< A, B>(this Set<Try<A>> ma, Func<A, B> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MSet<Try<B>>, Set<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Try<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Try<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Try<A>> ma, Action<A> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Try<A>> Lift< A>(Try<A> ma) =>
            default(MSet<Try<A>>).Return(ma);

        public static Set<Try<A>> Lift< A>(A a) =>
            default(MSet<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Set<Try<A>> FilterT< A>(this Set<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Set<Try<A>> Where< A>(this Set<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Set<Try<B>> Select< A, B>(this Set<Try<A>> ma, Func<A, B> f) =>
            Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MSet<Try<B>>, Set<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Set<Try<C>> SelectMany< A, B, C>(
            this Set<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Try<A>>, Set<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MSet<Try<C>>, Set<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class SetTryAsync
    {
        public static A SumT<NumA,  A>(this Set<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<TryAsync<A>> ma) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Set<TryAsync<B>> BindT< A, B>(this Set<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSet<TryAsync<B>>, Set<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Set<TryAsync<B>> MapT< A, B>(this Set<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MSet<TryAsync<B>>, Set<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<TryAsync<A>> ma, Action<A> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MSet<TryAsync<A>>).Return(ma);

        public static Set<TryAsync<A>> Lift< A>(A a) =>
            default(MSet<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Set<TryAsync<A>> FilterT< A>(this Set<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Set<TryAsync<A>> Where< A>(this Set<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Set<TryAsync<B>> Select< A, B>(this Set<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MSet<TryAsync<B>>, Set<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Set<TryAsync<C>> SelectMany< A, B, C>(
            this Set<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<TryAsync<A>>, Set<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MSet<TryAsync<C>>, Set<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class SetTryOption
    {
        public static A SumT<NumA,  A>(this Set<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<TryOption<A>> ma) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Set<TryOption<B>> BindT< A, B>(this Set<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSet<TryOption<B>>, Set<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Set<TryOption<B>> MapT< A, B>(this Set<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MSet<TryOption<B>>, Set<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<TryOption<A>> ma, Action<A> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MSet<TryOption<A>>).Return(ma);

        public static Set<TryOption<A>> Lift< A>(A a) =>
            default(MSet<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Set<TryOption<A>> FilterT< A>(this Set<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Set<TryOption<A>> Where< A>(this Set<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Set<TryOption<B>> Select< A, B>(this Set<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MSet<TryOption<B>>, Set<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Set<TryOption<C>> SelectMany< A, B, C>(
            this Set<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<TryOption<A>>, Set<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MSet<TryOption<C>>, Set<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class SetTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Set<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<TryOptionAsync<A>> ma) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Set<TryOptionAsync<B>> BindT< A, B>(this Set<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSet<TryOptionAsync<B>>, Set<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Set<TryOptionAsync<B>> MapT< A, B>(this Set<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MSet<TryOptionAsync<B>>, Set<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MSet<TryOptionAsync<A>>).Return(ma);

        public static Set<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MSet<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Set<TryOptionAsync<A>> FilterT< A>(this Set<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Set<TryOptionAsync<A>> Where< A>(this Set<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Set<TryOptionAsync<B>> Select< A, B>(this Set<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MSet<TryOptionAsync<B>>, Set<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Set<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Set<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<TryOptionAsync<A>>, Set<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MSet<TryOptionAsync<C>>, Set<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class SetQue
    {
        public static A SumT<NumA,  A>(this Set<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Que<A>> ma) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Set<Que<B>> BindT< A, B>(this Set<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSet<Que<B>>, Set<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Set<Que<B>> MapT< A, B>(this Set<Que<A>> ma, Func<A, B> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MSet<Que<B>>, Set<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Que<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Que<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Que<A>> ma, Action<A> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Que<A>> Lift< A>(Que<A> ma) =>
            default(MSet<Que<A>>).Return(ma);

        public static Set<Que<A>> Lift< A>(A a) =>
            default(MSet<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Set<Que<A>> FilterT< A>(this Set<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Set<Que<A>> Where< A>(this Set<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Set<Que<B>> Select< A, B>(this Set<Que<A>> ma, Func<A, B> f) =>
            Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MSet<Que<B>>, Set<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Set<Que<C>> SelectMany< A, B, C>(
            this Set<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Que<A>>, Set<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MSet<Que<C>>, Set<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class SetIEnumerable
    {
        public static A SumT<NumA,  A>(this Set<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<IEnumerable<A>> ma) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Set<IEnumerable<B>> BindT< A, B>(this Set<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSet<IEnumerable<B>>, Set<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Set<IEnumerable<B>> MapT< A, B>(this Set<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MSet<IEnumerable<B>>, Set<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MSet<IEnumerable<A>>).Return(ma);

        public static Set<IEnumerable<A>> Lift< A>(A a) =>
            default(MSet<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Set<IEnumerable<A>> FilterT< A>(this Set<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Set<IEnumerable<A>> Where< A>(this Set<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Set<IEnumerable<B>> Select< A, B>(this Set<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MSet<IEnumerable<B>>, Set<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Set<IEnumerable<C>> SelectMany< A, B, C>(
            this Set<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<IEnumerable<A>>, Set<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MSet<IEnumerable<C>>, Set<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class SetSet
    {
        public static A SumT<NumA,  A>(this Set<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Set<A>> ma) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Set<Set<B>> BindT< A, B>(this Set<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSet<Set<B>>, Set<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Set<Set<B>> MapT< A, B>(this Set<Set<A>> ma, Func<A, B> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MSet<Set<B>>, Set<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Set<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Set<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Set<A>> ma, Action<A> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Set<A>> Lift< A>(Set<A> ma) =>
            default(MSet<Set<A>>).Return(ma);

        public static Set<Set<A>> Lift< A>(A a) =>
            default(MSet<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Set<Set<A>> FilterT< A>(this Set<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Set<Set<A>> Where< A>(this Set<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Set<Set<B>> Select< A, B>(this Set<Set<A>> ma, Func<A, B> f) =>
            Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MSet<Set<B>>, Set<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Set<Set<C>> SelectMany< A, B, C>(
            this Set<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Set<A>>, Set<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MSet<Set<C>>, Set<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class SetStck
    {
        public static A SumT<NumA,  A>(this Set<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Set<Stck<A>> ma) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Set<Stck<B>> BindT< A, B>(this Set<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSet<Stck<B>>, Set<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Set<Stck<B>> MapT< A, B>(this Set<Stck<A>> ma, Func<A, B> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MSet<Stck<B>>, Set<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Set<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Set<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Set<Stck<A>> ma, Action<A> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Set<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MSet<Stck<A>>).Return(ma);

        public static Set<Stck<A>> Lift< A>(A a) =>
            default(MSet<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Set<Stck<A>> FilterT< A>(this Set<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Set<Stck<A>> Where< A>(this Set<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Set<Stck<B>> Select< A, B>(this Set<Stck<A>> ma, Func<A, B> f) =>
            Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MSet<Stck<B>>, Set<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Set<Stck<C>> SelectMany< A, B, C>(
            this Set<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MSet<Stck<A>>, Set<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MSet<Stck<C>>, Set<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
    public static class StckArr
    {
        public static A SumT<NumA,  A>(this Stck<Arr<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Arr<A>> ma) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>.Inst.Count(ma);

        public static Stck<Arr<B>> BindT< A, B>(this Stck<Arr<A>> ma, Func<A, Arr<B>> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MStck<Arr<B>>, Stck<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Stck<Arr<B>> MapT< A, B>(this Stck<Arr<A>> ma, Func<A, B> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MStck<Arr<B>>, Stck<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Arr<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Arr<A>> ma, Action<A> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Arr<A>> Lift< A>(Arr<A> ma) =>
            default(MStck<Arr<A>>).Return(ma);

        public static Stck<Arr<A>> Lift< A>(A a) =>
            default(MStck<Arr<A>>).Return(default(MArr<A>).Return(a));

        public static Stck<Arr<A>> FilterT< A>(this Stck<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Stck<Arr<A>> Where< A>(this Stck<Arr<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Bind<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>(ma, 
                    a => pred(a)
                        ? default(MArr<A>).Return(a)
                        : default(MArr<A>).Zero());

        public static Stck<Arr<B>> Select< A, B>(this Stck<Arr<A>> ma, Func<A, B> f) =>
            Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                .Inst.Map<MStck<Arr<B>>, Stck<Arr<B>>, MArr<B>, Arr<B>, B>(ma, f);

        public static Stck<Arr<C>> SelectMany< A, B, C>(
            this Stck<Arr<A>> ma,
            Func<A, Arr<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Arr<A>>, Stck<Arr<A>>, MArr<A>, Arr<A>, A>
                    .Inst.Bind<MStck<Arr<C>>, Stck<Arr<C>>, MArr<C>, Arr<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MArr<B>).Bind<MArr<C>, Arr<C>, C>(mb, b => default(MArr<C>).Return(project(a, b)));
                    });
    } 
    public static class StckHashSet
    {
        public static A SumT<NumA,  A>(this Stck<HashSet<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<HashSet<A>> ma) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>.Inst.Count(ma);

        public static Stck<HashSet<B>> BindT< A, B>(this Stck<HashSet<A>> ma, Func<A, HashSet<B>> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MStck<HashSet<B>>, Stck<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Stck<HashSet<B>> MapT< A, B>(this Stck<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MStck<HashSet<B>>, Stck<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<HashSet<A>> ma, Func<A, bool> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<HashSet<A>> ma, Action<A> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<HashSet<A>> Lift< A>(HashSet<A> ma) =>
            default(MStck<HashSet<A>>).Return(ma);

        public static Stck<HashSet<A>> Lift< A>(A a) =>
            default(MStck<HashSet<A>>).Return(default(MHashSet<A>).Return(a));

        public static Stck<HashSet<A>> FilterT< A>(this Stck<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Stck<HashSet<A>> Where< A>(this Stck<HashSet<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Bind<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>(ma, 
                    a => pred(a)
                        ? default(MHashSet<A>).Return(a)
                        : default(MHashSet<A>).Zero());

        public static Stck<HashSet<B>> Select< A, B>(this Stck<HashSet<A>> ma, Func<A, B> f) =>
            Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                .Inst.Map<MStck<HashSet<B>>, Stck<HashSet<B>>, MHashSet<B>, HashSet<B>, B>(ma, f);

        public static Stck<HashSet<C>> SelectMany< A, B, C>(
            this Stck<HashSet<A>> ma,
            Func<A, HashSet<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<HashSet<A>>, Stck<HashSet<A>>, MHashSet<A>, HashSet<A>, A>
                    .Inst.Bind<MStck<HashSet<C>>, Stck<HashSet<C>>, MHashSet<C>, HashSet<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MHashSet<B>).Bind<MHashSet<C>, HashSet<C>, C>(mb, b => default(MHashSet<C>).Return(project(a, b)));
                    });
    } 
    public static class StckLst
    {
        public static A SumT<NumA,  A>(this Stck<Lst<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Lst<A>> ma) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>.Inst.Count(ma);

        public static Stck<Lst<B>> BindT< A, B>(this Stck<Lst<A>> ma, Func<A, Lst<B>> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MStck<Lst<B>>, Stck<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Stck<Lst<B>> MapT< A, B>(this Stck<Lst<A>> ma, Func<A, B> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MStck<Lst<B>>, Stck<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Lst<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Lst<A>> ma, Action<A> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Lst<A>> Lift< A>(Lst<A> ma) =>
            default(MStck<Lst<A>>).Return(ma);

        public static Stck<Lst<A>> Lift< A>(A a) =>
            default(MStck<Lst<A>>).Return(default(MLst<A>).Return(a));

        public static Stck<Lst<A>> FilterT< A>(this Stck<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Stck<Lst<A>> Where< A>(this Stck<Lst<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Bind<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>(ma, 
                    a => pred(a)
                        ? default(MLst<A>).Return(a)
                        : default(MLst<A>).Zero());

        public static Stck<Lst<B>> Select< A, B>(this Stck<Lst<A>> ma, Func<A, B> f) =>
            Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                .Inst.Map<MStck<Lst<B>>, Stck<Lst<B>>, MLst<B>, Lst<B>, B>(ma, f);

        public static Stck<Lst<C>> SelectMany< A, B, C>(
            this Stck<Lst<A>> ma,
            Func<A, Lst<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Lst<A>>, Stck<Lst<A>>, MLst<A>, Lst<A>, A>
                    .Inst.Bind<MStck<Lst<C>>, Stck<Lst<C>>, MLst<C>, Lst<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MLst<B>).Bind<MLst<C>, Lst<C>, C>(mb, b => default(MLst<C>).Return(project(a, b)));
                    });
    } 
    public static class StckOption
    {
        public static A SumT<NumA,  A>(this Stck<Option<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Option<A>> ma) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>.Inst.Count(ma);

        public static Stck<Option<B>> BindT< A, B>(this Stck<Option<A>> ma, Func<A, Option<B>> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MStck<Option<B>>, Stck<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Stck<Option<B>> MapT< A, B>(this Stck<Option<A>> ma, Func<A, B> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MStck<Option<B>>, Stck<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Option<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Option<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Option<A>> ma, Action<A> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Option<A>> Lift< A>(Option<A> ma) =>
            default(MStck<Option<A>>).Return(ma);

        public static Stck<Option<A>> Lift< A>(A a) =>
            default(MStck<Option<A>>).Return(default(MOption<A>).Return(a));

        public static Stck<Option<A>> FilterT< A>(this Stck<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Stck<Option<A>> Where< A>(this Stck<Option<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Bind<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOption<A>).Return(a)
                        : default(MOption<A>).Zero());

        public static Stck<Option<B>> Select< A, B>(this Stck<Option<A>> ma, Func<A, B> f) =>
            Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                .Inst.Map<MStck<Option<B>>, Stck<Option<B>>, MOption<B>, Option<B>, B>(ma, f);

        public static Stck<Option<C>> SelectMany< A, B, C>(
            this Stck<Option<A>> ma,
            Func<A, Option<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Option<A>>, Stck<Option<A>>, MOption<A>, Option<A>, A>
                    .Inst.Bind<MStck<Option<C>>, Stck<Option<C>>, MOption<C>, Option<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOption<B>).Bind<MOption<C>, Option<C>, C>(mb, b => default(MOption<C>).Return(project(a, b)));
                    });
    } 
    public static class StckOptionUnsafe
    {
        public static A SumT<NumA,  A>(this Stck<OptionUnsafe<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<OptionUnsafe<A>> ma) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Count(ma);

        public static Stck<OptionUnsafe<B>> BindT< A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, OptionUnsafe<B>> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MStck<OptionUnsafe<B>>, Stck<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Stck<OptionUnsafe<B>> MapT< A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MStck<OptionUnsafe<B>>, Stck<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, bool> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<OptionUnsafe<A>> ma, Action<A> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<OptionUnsafe<A>> Lift< A>(OptionUnsafe<A> ma) =>
            default(MStck<OptionUnsafe<A>>).Return(ma);

        public static Stck<OptionUnsafe<A>> Lift< A>(A a) =>
            default(MStck<OptionUnsafe<A>>).Return(default(MOptionUnsafe<A>).Return(a));

        public static Stck<OptionUnsafe<A>> FilterT< A>(this Stck<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Stck<OptionUnsafe<A>> Where< A>(this Stck<OptionUnsafe<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Bind<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>(ma, 
                    a => pred(a)
                        ? default(MOptionUnsafe<A>).Return(a)
                        : default(MOptionUnsafe<A>).Zero());

        public static Stck<OptionUnsafe<B>> Select< A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, B> f) =>
            Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                .Inst.Map<MStck<OptionUnsafe<B>>, Stck<OptionUnsafe<B>>, MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, f);

        public static Stck<OptionUnsafe<C>> SelectMany< A, B, C>(
            this Stck<OptionUnsafe<A>> ma,
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<OptionUnsafe<A>>, Stck<OptionUnsafe<A>>, MOptionUnsafe<A>, OptionUnsafe<A>, A>
                    .Inst.Bind<MStck<OptionUnsafe<C>>, Stck<OptionUnsafe<C>>, MOptionUnsafe<C>, OptionUnsafe<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MOptionUnsafe<B>).Bind<MOptionUnsafe<C>, OptionUnsafe<C>, C>(mb, b => default(MOptionUnsafe<C>).Return(project(a, b)));
                    });
    } 
    public static class StckEither
    {
        public static A SumT<NumA, L, A>(this Stck<Either<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Stck<Either<L, A>> ma) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>.Inst.Count(ma);

        public static Stck<Either<L, B>> BindT<L, A, B>(this Stck<Either<L, A>> ma, Func<A, Either<L, B>> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MStck<Either<L, B>>, Stck<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Stck<Either<L, B>> MapT<L, A, B>(this Stck<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MStck<Either<L, B>>, Stck<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Stck<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Stck<Either<L, A>> ma, Func<A, bool> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Stck<Either<L, A>> ma, Action<A> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Either<L, A>> Lift<L, A>(Either<L, A> ma) =>
            default(MStck<Either<L, A>>).Return(ma);

        public static Stck<Either<L, A>> Lift<L, A>(A a) =>
            default(MStck<Either<L, A>>).Return(default(MEither<L, A>).Return(a));

        public static Stck<Either<L, A>> FilterT<L, A>(this Stck<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Stck<Either<L, A>> Where<L, A>(this Stck<Either<L, A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Bind<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEither<L, A>).Return(a)
                        : default(MEither<L, A>).Zero());

        public static Stck<Either<L, B>> Select<L, A, B>(this Stck<Either<L, A>> ma, Func<A, B> f) =>
            Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                .Inst.Map<MStck<Either<L, B>>, Stck<Either<L, B>>, MEither<L, B>, Either<L, B>, B>(ma, f);

        public static Stck<Either<L, C>> SelectMany<L, A, B, C>(
            this Stck<Either<L, A>> ma,
            Func<A, Either<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Either<L, A>>, Stck<Either<L, A>>, MEither<L, A>, Either<L, A>, A>
                    .Inst.Bind<MStck<Either<L, C>>, Stck<Either<L, C>>, MEither<L, C>, Either<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEither<L, B>).Bind<MEither<L, C>, Either<L, C>, C>(mb, b => default(MEither<L, C>).Return(project(a, b)));
                    });
    } 
    public static class StckEitherUnsafe
    {
        public static A SumT<NumA, L, A>(this Stck<EitherUnsafe<L, A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, NumA, A>.Inst.Sum(ma);

        public static int CountT<L, A>(this Stck<EitherUnsafe<L, A>> ma) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>.Inst.Count(ma);

        public static Stck<EitherUnsafe<L, B>> BindT<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, EitherUnsafe<L, B>> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MStck<EitherUnsafe<L, B>>, Stck<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Stck<EitherUnsafe<L, B>> MapT<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MStck<EitherUnsafe<L, B>>, Stck<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static bool ExistsT<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, bool> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Action<A> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<EitherUnsafe<L, A>> Lift<L, A>(EitherUnsafe<L, A> ma) =>
            default(MStck<EitherUnsafe<L, A>>).Return(ma);

        public static Stck<EitherUnsafe<L, A>> Lift<L, A>(A a) =>
            default(MStck<EitherUnsafe<L, A>>).Return(default(MEitherUnsafe<L, A>).Return(a));

        public static Stck<EitherUnsafe<L, A>> FilterT<L, A>(this Stck<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Stck<EitherUnsafe<L, A>> Where<L, A>(this Stck<EitherUnsafe<L, A>> ma, Func<A, bool> pred) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Bind<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>(ma, 
                    a => pred(a)
                        ? default(MEitherUnsafe<L, A>).Return(a)
                        : default(MEitherUnsafe<L, A>).Zero());

        public static Stck<EitherUnsafe<L, B>> Select<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, B> f) =>
            Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                .Inst.Map<MStck<EitherUnsafe<L, B>>, Stck<EitherUnsafe<L, B>>, MEitherUnsafe<L, B>, EitherUnsafe<L, B>, B>(ma, f);

        public static Stck<EitherUnsafe<L, C>> SelectMany<L, A, B, C>(
            this Stck<EitherUnsafe<L, A>> ma,
            Func<A, EitherUnsafe<L, B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<EitherUnsafe<L, A>>, Stck<EitherUnsafe<L, A>>, MEitherUnsafe<L, A>, EitherUnsafe<L, A>, A>
                    .Inst.Bind<MStck<EitherUnsafe<L, C>>, Stck<EitherUnsafe<L, C>>, MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MEitherUnsafe<L, B>).Bind<MEitherUnsafe<L, C>, EitherUnsafe<L, C>, C>(mb, b => default(MEitherUnsafe<L, C>).Return(project(a, b)));
                    });
    } 
    public static class StckTry
    {
        public static A SumT<NumA,  A>(this Stck<Try<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Try<A>> ma) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>.Inst.Count(ma);

        public static Stck<Try<B>> BindT< A, B>(this Stck<Try<A>> ma, Func<A, Try<B>> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MStck<Try<B>>, Stck<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Stck<Try<B>> MapT< A, B>(this Stck<Try<A>> ma, Func<A, B> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MStck<Try<B>>, Stck<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Try<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Try<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Try<A>> ma, Action<A> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Try<A>> Lift< A>(Try<A> ma) =>
            default(MStck<Try<A>>).Return(ma);

        public static Stck<Try<A>> Lift< A>(A a) =>
            default(MStck<Try<A>>).Return(default(MTry<A>).Return(a));

        public static Stck<Try<A>> FilterT< A>(this Stck<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Stck<Try<A>> Where< A>(this Stck<Try<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Bind<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTry<A>).Return(a)
                        : default(MTry<A>).Zero());

        public static Stck<Try<B>> Select< A, B>(this Stck<Try<A>> ma, Func<A, B> f) =>
            Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                .Inst.Map<MStck<Try<B>>, Stck<Try<B>>, MTry<B>, Try<B>, B>(ma, f);

        public static Stck<Try<C>> SelectMany< A, B, C>(
            this Stck<Try<A>> ma,
            Func<A, Try<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Try<A>>, Stck<Try<A>>, MTry<A>, Try<A>, A>
                    .Inst.Bind<MStck<Try<C>>, Stck<Try<C>>, MTry<C>, Try<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTry<B>).Bind<MTry<C>, Try<C>, C>(mb, b => default(MTry<C>).Return(project(a, b)));
                    });
    } 
    public static class StckTryAsync
    {
        public static A SumT<NumA,  A>(this Stck<TryAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<TryAsync<A>> ma) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>.Inst.Count(ma);

        public static Stck<TryAsync<B>> BindT< A, B>(this Stck<TryAsync<A>> ma, Func<A, TryAsync<B>> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MStck<TryAsync<B>>, Stck<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Stck<TryAsync<B>> MapT< A, B>(this Stck<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MStck<TryAsync<B>>, Stck<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<TryAsync<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<TryAsync<A>> ma, Action<A> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<TryAsync<A>> Lift< A>(TryAsync<A> ma) =>
            default(MStck<TryAsync<A>>).Return(ma);

        public static Stck<TryAsync<A>> Lift< A>(A a) =>
            default(MStck<TryAsync<A>>).Return(default(MTryAsync<A>).Return(a));

        public static Stck<TryAsync<A>> FilterT< A>(this Stck<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Stck<TryAsync<A>> Where< A>(this Stck<TryAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Bind<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryAsync<A>).Return(a)
                        : default(MTryAsync<A>).Zero());

        public static Stck<TryAsync<B>> Select< A, B>(this Stck<TryAsync<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                .Inst.Map<MStck<TryAsync<B>>, Stck<TryAsync<B>>, MTryAsync<B>, TryAsync<B>, B>(ma, f);

        public static Stck<TryAsync<C>> SelectMany< A, B, C>(
            this Stck<TryAsync<A>> ma,
            Func<A, TryAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<TryAsync<A>>, Stck<TryAsync<A>>, MTryAsync<A>, TryAsync<A>, A>
                    .Inst.Bind<MStck<TryAsync<C>>, Stck<TryAsync<C>>, MTryAsync<C>, TryAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryAsync<B>).Bind<MTryAsync<C>, TryAsync<C>, C>(mb, b => default(MTryAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class StckTryOption
    {
        public static A SumT<NumA,  A>(this Stck<TryOption<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<TryOption<A>> ma) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>.Inst.Count(ma);

        public static Stck<TryOption<B>> BindT< A, B>(this Stck<TryOption<A>> ma, Func<A, TryOption<B>> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MStck<TryOption<B>>, Stck<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Stck<TryOption<B>> MapT< A, B>(this Stck<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MStck<TryOption<B>>, Stck<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<TryOption<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<TryOption<A>> ma, Action<A> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<TryOption<A>> Lift< A>(TryOption<A> ma) =>
            default(MStck<TryOption<A>>).Return(ma);

        public static Stck<TryOption<A>> Lift< A>(A a) =>
            default(MStck<TryOption<A>>).Return(default(MTryOption<A>).Return(a));

        public static Stck<TryOption<A>> FilterT< A>(this Stck<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Stck<TryOption<A>> Where< A>(this Stck<TryOption<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Bind<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOption<A>).Return(a)
                        : default(MTryOption<A>).Zero());

        public static Stck<TryOption<B>> Select< A, B>(this Stck<TryOption<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                .Inst.Map<MStck<TryOption<B>>, Stck<TryOption<B>>, MTryOption<B>, TryOption<B>, B>(ma, f);

        public static Stck<TryOption<C>> SelectMany< A, B, C>(
            this Stck<TryOption<A>> ma,
            Func<A, TryOption<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<TryOption<A>>, Stck<TryOption<A>>, MTryOption<A>, TryOption<A>, A>
                    .Inst.Bind<MStck<TryOption<C>>, Stck<TryOption<C>>, MTryOption<C>, TryOption<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOption<B>).Bind<MTryOption<C>, TryOption<C>, C>(mb, b => default(MTryOption<C>).Return(project(a, b)));
                    });
    } 
    public static class StckTryOptionAsync
    {
        public static A SumT<NumA,  A>(this Stck<TryOptionAsync<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<TryOptionAsync<A>> ma) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>.Inst.Count(ma);

        public static Stck<TryOptionAsync<B>> BindT< A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, TryOptionAsync<B>> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MStck<TryOptionAsync<B>>, Stck<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Stck<TryOptionAsync<B>> MapT< A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MStck<TryOptionAsync<B>>, Stck<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, bool> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<TryOptionAsync<A>> ma, Action<A> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<TryOptionAsync<A>> Lift< A>(TryOptionAsync<A> ma) =>
            default(MStck<TryOptionAsync<A>>).Return(ma);

        public static Stck<TryOptionAsync<A>> Lift< A>(A a) =>
            default(MStck<TryOptionAsync<A>>).Return(default(MTryOptionAsync<A>).Return(a));

        public static Stck<TryOptionAsync<A>> FilterT< A>(this Stck<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Stck<TryOptionAsync<A>> Where< A>(this Stck<TryOptionAsync<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Bind<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>(ma, 
                    a => pred(a)
                        ? default(MTryOptionAsync<A>).Return(a)
                        : default(MTryOptionAsync<A>).Zero());

        public static Stck<TryOptionAsync<B>> Select< A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, B> f) =>
            Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                .Inst.Map<MStck<TryOptionAsync<B>>, Stck<TryOptionAsync<B>>, MTryOptionAsync<B>, TryOptionAsync<B>, B>(ma, f);

        public static Stck<TryOptionAsync<C>> SelectMany< A, B, C>(
            this Stck<TryOptionAsync<A>> ma,
            Func<A, TryOptionAsync<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<TryOptionAsync<A>>, Stck<TryOptionAsync<A>>, MTryOptionAsync<A>, TryOptionAsync<A>, A>
                    .Inst.Bind<MStck<TryOptionAsync<C>>, Stck<TryOptionAsync<C>>, MTryOptionAsync<C>, TryOptionAsync<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MTryOptionAsync<B>).Bind<MTryOptionAsync<C>, TryOptionAsync<C>, C>(mb, b => default(MTryOptionAsync<C>).Return(project(a, b)));
                    });
    } 
    public static class StckQue
    {
        public static A SumT<NumA,  A>(this Stck<Que<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Que<A>> ma) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>.Inst.Count(ma);

        public static Stck<Que<B>> BindT< A, B>(this Stck<Que<A>> ma, Func<A, Que<B>> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MStck<Que<B>>, Stck<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Stck<Que<B>> MapT< A, B>(this Stck<Que<A>> ma, Func<A, B> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MStck<Que<B>>, Stck<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Que<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Que<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Que<A>> ma, Action<A> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Que<A>> Lift< A>(Que<A> ma) =>
            default(MStck<Que<A>>).Return(ma);

        public static Stck<Que<A>> Lift< A>(A a) =>
            default(MStck<Que<A>>).Return(default(MQue<A>).Return(a));

        public static Stck<Que<A>> FilterT< A>(this Stck<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Stck<Que<A>> Where< A>(this Stck<Que<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Bind<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>(ma, 
                    a => pred(a)
                        ? default(MQue<A>).Return(a)
                        : default(MQue<A>).Zero());

        public static Stck<Que<B>> Select< A, B>(this Stck<Que<A>> ma, Func<A, B> f) =>
            Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                .Inst.Map<MStck<Que<B>>, Stck<Que<B>>, MQue<B>, Que<B>, B>(ma, f);

        public static Stck<Que<C>> SelectMany< A, B, C>(
            this Stck<Que<A>> ma,
            Func<A, Que<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Que<A>>, Stck<Que<A>>, MQue<A>, Que<A>, A>
                    .Inst.Bind<MStck<Que<C>>, Stck<Que<C>>, MQue<C>, Que<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MQue<B>).Bind<MQue<C>, Que<C>, C>(mb, b => default(MQue<C>).Return(project(a, b)));
                    });
    } 
    public static class StckIEnumerable
    {
        public static A SumT<NumA,  A>(this Stck<IEnumerable<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<IEnumerable<A>> ma) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>.Inst.Count(ma);

        public static Stck<IEnumerable<B>> BindT< A, B>(this Stck<IEnumerable<A>> ma, Func<A, IEnumerable<B>> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MStck<IEnumerable<B>>, Stck<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Stck<IEnumerable<B>> MapT< A, B>(this Stck<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MStck<IEnumerable<B>>, Stck<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<IEnumerable<A>> ma, Func<A, bool> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<IEnumerable<A>> ma, Action<A> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<IEnumerable<A>> Lift< A>(IEnumerable<A> ma) =>
            default(MStck<IEnumerable<A>>).Return(ma);

        public static Stck<IEnumerable<A>> Lift< A>(A a) =>
            default(MStck<IEnumerable<A>>).Return(default(MSeq<A>).Return(a));

        public static Stck<IEnumerable<A>> FilterT< A>(this Stck<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Stck<IEnumerable<A>> Where< A>(this Stck<IEnumerable<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Bind<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSeq<A>).Return(a)
                        : default(MSeq<A>).Zero());

        public static Stck<IEnumerable<B>> Select< A, B>(this Stck<IEnumerable<A>> ma, Func<A, B> f) =>
            Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                .Inst.Map<MStck<IEnumerable<B>>, Stck<IEnumerable<B>>, MSeq<B>, IEnumerable<B>, B>(ma, f);

        public static Stck<IEnumerable<C>> SelectMany< A, B, C>(
            this Stck<IEnumerable<A>> ma,
            Func<A, IEnumerable<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<IEnumerable<A>>, Stck<IEnumerable<A>>, MSeq<A>, IEnumerable<A>, A>
                    .Inst.Bind<MStck<IEnumerable<C>>, Stck<IEnumerable<C>>, MSeq<C>, IEnumerable<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSeq<B>).Bind<MSeq<C>, IEnumerable<C>, C>(mb, b => default(MSeq<C>).Return(project(a, b)));
                    });
    } 
    public static class StckSet
    {
        public static A SumT<NumA,  A>(this Stck<Set<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Set<A>> ma) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>.Inst.Count(ma);

        public static Stck<Set<B>> BindT< A, B>(this Stck<Set<A>> ma, Func<A, Set<B>> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MStck<Set<B>>, Stck<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Stck<Set<B>> MapT< A, B>(this Stck<Set<A>> ma, Func<A, B> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MStck<Set<B>>, Stck<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Set<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Set<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Set<A>> ma, Action<A> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Set<A>> Lift< A>(Set<A> ma) =>
            default(MStck<Set<A>>).Return(ma);

        public static Stck<Set<A>> Lift< A>(A a) =>
            default(MStck<Set<A>>).Return(default(MSet<A>).Return(a));

        public static Stck<Set<A>> FilterT< A>(this Stck<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Stck<Set<A>> Where< A>(this Stck<Set<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Bind<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>(ma, 
                    a => pred(a)
                        ? default(MSet<A>).Return(a)
                        : default(MSet<A>).Zero());

        public static Stck<Set<B>> Select< A, B>(this Stck<Set<A>> ma, Func<A, B> f) =>
            Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                .Inst.Map<MStck<Set<B>>, Stck<Set<B>>, MSet<B>, Set<B>, B>(ma, f);

        public static Stck<Set<C>> SelectMany< A, B, C>(
            this Stck<Set<A>> ma,
            Func<A, Set<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Set<A>>, Stck<Set<A>>, MSet<A>, Set<A>, A>
                    .Inst.Bind<MStck<Set<C>>, Stck<Set<C>>, MSet<C>, Set<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MSet<B>).Bind<MSet<C>, Set<C>, C>(mb, b => default(MSet<C>).Return(project(a, b)));
                    });
    } 
    public static class StckStck
    {
        public static A SumT<NumA,  A>(this Stck<Stck<A>> ma)
            where NumA : struct, Num<A> =>
                Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, NumA, A>.Inst.Sum(ma);

        public static int CountT< A>(this Stck<Stck<A>> ma) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>.Inst.Count(ma);

        public static Stck<Stck<B>> BindT< A, B>(this Stck<Stck<A>> ma, Func<A, Stck<B>> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MStck<Stck<B>>, Stck<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Stck<Stck<B>> MapT< A, B>(this Stck<Stck<A>> ma, Func<A, B> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MStck<Stck<B>>, Stck<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static bool ExistsT< A, B>(this Stck<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, false, (s, x) => s || f(x));

        public static bool ForAllT< A, B>(this Stck<Stck<A>> ma, Func<A, bool> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, true, (s, x) => s && f(x));

        public static Unit IterT< A, B>(this Stck<Stck<A>> ma, Action<A> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Fold(ma, unit, (s, x) => { f(x); return unit; });

        public static Stck<Stck<A>> Lift< A>(Stck<A> ma) =>
            default(MStck<Stck<A>>).Return(ma);

        public static Stck<Stck<A>> Lift< A>(A a) =>
            default(MStck<Stck<A>>).Return(default(MStck<A>).Return(a));

        public static Stck<Stck<A>> FilterT< A>(this Stck<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Stck<Stck<A>> Where< A>(this Stck<Stck<A>> ma, Func<A, bool> pred) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Bind<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>(ma, 
                    a => pred(a)
                        ? default(MStck<A>).Return(a)
                        : default(MStck<A>).Zero());

        public static Stck<Stck<B>> Select< A, B>(this Stck<Stck<A>> ma, Func<A, B> f) =>
            Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                .Inst.Map<MStck<Stck<B>>, Stck<Stck<B>>, MStck<B>, Stck<B>, B>(ma, f);

        public static Stck<Stck<C>> SelectMany< A, B, C>(
            this Stck<Stck<A>> ma,
            Func<A, Stck<B>> bind,
            Func<A, B, C> project) =>
                Trans<MStck<Stck<A>>, Stck<Stck<A>>, MStck<A>, Stck<A>, A>
                    .Inst.Bind<MStck<Stck<C>>, Stck<Stck<C>>, MStck<C>, Stck<C>, C>(ma, a =>
                    {
                        var mb = bind(a);
                        return default(MStck<B>).Bind<MStck<C>, Stck<C>, C>(mb, b => default(MStck<C>).Return(project(a, b)));
                    });
    } 
}
