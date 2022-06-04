#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class IdentityT
    {
        public static Identity<Arr<B>> Traverse<A, B>(this Arr<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<Arr<B>>(new Arr<B>(res));            
        }

        public static Identity<Either<L, B>> Traverse<L, A, B>(this Either<L, Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Right: x => new Identity<Either<L, B>>(f(x.Value)),
                Left: e => new Identity<Either<L, B>>(Either<L, B>.Left(e)));

        public static Identity<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Identity<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Right: x => new Identity<EitherUnsafe<L, B>>(f(x.Value)),
                Left: e => new Identity<EitherUnsafe<L, B>>(EitherUnsafe<L, B>.Left(e)));

        public static Identity<HashSet<B>> Traverse<A, B>(this HashSet<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<HashSet<B>>(new HashSet<B>(res));            
        }

        public static Identity<Identity<B>> Traverse<A, B>(this Identity<Identity<A>> ma, Func<A, B> f) =>
            new Identity<Identity<B>>(new Identity<B>(f(ma.Value.Value)));
        
        public static Identity<Lst<B>> Traverse<A, B>(this Lst<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<Lst<B>>(new Lst<B>(res));            
        }
        
        public static Identity<Fin<B>> Traverse<A, B>(this Fin<Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Succ: x => new Identity<Fin<B>>(f(x.Value)),
                Fail: e => new Identity<Fin<B>>(Fin<B>.Fail(e)));
        
        public static Identity<Option<B>> Traverse<A, B>(this Option<Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Some: x => new Identity<Option<B>>(f(x.Value)),
                None: () => new Identity<Option<B>>(Option<B>.None));
        
        public static Identity<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Identity<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Some: x => new Identity<OptionUnsafe<B>>(f(x.Value)),
                None: () => new Identity<OptionUnsafe<B>>(OptionUnsafe<B>.None));
        
        public static Identity<Que<B>> Traverse<A, B>(this Que<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<Que<B>>(new Que<B>(res));            
        }
        
        public static Identity<Seq<B>> Traverse<A, B>(this Seq<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<Seq<B>>(Seq.FromArray(res));            
        }
        
        public static Identity<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Identity<A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            foreach (var xs in ma)
            {
                res.Add(f(xs.Value));
            }
            return new Identity<IEnumerable<B>>(Seq.FromArray(res.ToArray()));            
        }
        
        public static Identity<Set<B>> Traverse<A, B>(this Set<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix++;
            }
            return new Identity<Set<B>>(new Set<B>(res));            
        }
        
        public static Identity<Stck<B>> Traverse<A, B>(this Stck<Identity<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = ma.Count - 1;
            foreach (var xs in ma)
            {
                res[ix] = f(xs.Value);
                ix--;
            }
            return new Identity<Stck<B>>(new Stck<B>(res));            
        }
        
        public static Identity<Try<B>> Traverse<A, B>(this Try<Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Succ: x => new Identity<Try<B>>(Try(f(x.Value))),
                Fail: e => new Identity<Try<B>>(Try<B>(e)));
        
        public static Identity<TryOption<B>> Traverse<A, B>(this TryOption<Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Some: x  => new Identity<TryOption<B>>(TryOption(f(x.Value))),
                None: () => new Identity<TryOption<B>>(TryOption<B>(Option<B>.None)),
                Fail: e  => new Identity<TryOption<B>>(TryOption<B>(e)));
        
        public static Identity<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Succ: x => new Identity<Validation<Fail, B>>(f(x.Value)),
                Fail: e => new Identity<Validation<Fail, B>>(Validation<Fail, B>.Fail(e)));
        
        public static Identity<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Identity<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Succ: x => new Identity<Validation<MonoidFail, Fail, B>>(f(x.Value)),
                Fail: e => new Identity<Validation<MonoidFail, Fail, B>>(Validation<MonoidFail, Fail, B>.Fail(e)));

        public static Identity<Eff<B>> Traverse<A, B>(this Eff<Identity<A>> ma, Func<A, B> f) =>
            ma.Match(
                Succ: x => new Identity<Eff<B>>(SuccessEff(f(x.Value))),
                Fail: e => new Identity<Eff<B>>(FailEff<B>(e)))
                .Run().Value;
    }
}
