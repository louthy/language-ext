#nullable enable
using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class TryT
    {
        public static Try<Arr<B>> Traverse<A, B>(this Arr<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsBottom) return new Result<Arr<B>>(BottomException.Default);
                if (x.IsFaulted) return new Result<Arr<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<Arr<B>>(new Arr<B>(res));
        };

        public static Try<Either<L, B>> Traverse<L, A, B>(this Either<L, Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsBottom)
            {
                return Result<Either<L, B>>.Bottom;
            }
            else if (ma.IsLeft)
            {
                return new Result<Either<L, B>>(ma.LeftValue);
            }
            else
            {
                var mr = ma.RightValue();  
                if (mr.IsBottom) return new Result<Either<L, B>>(BottomException.Default);
                if (mr.IsFaulted) return new Result<Either<L, B>>(mr.Exception);
                return new Result<Either<L, B>>(Either<L, B>.Right(f(mr.Value)));
            }
        };
        
        public static Try<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsBottom)
            {
                return Result<EitherUnsafe<L, B>>.Bottom;
            }
            else if (ma.IsLeft)
            {
                return new Result<EitherUnsafe<L, B>>(ma.LeftValue);
            }
            else
            {
                var mr = ma.RightValue();  
                if (mr.IsBottom) return new Result<EitherUnsafe<L, B>>(BottomException.Default);
                if (mr.IsFaulted) return new Result<EitherUnsafe<L, B>>(mr.Exception);
                return new Result<EitherUnsafe<L, B>>(EitherUnsafe<L, B>.Right(f(mr.Value)));
            }
        };

        public static Try<HashSet<B>> Traverse<A, B>(this HashSet<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<HashSet<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<HashSet<B>>(new HashSet<B>(res));
        };

        public static Try<Identity<B>> Traverse<A, B>(this Identity<Try<A>> ma, Func<A, B> f) => () =>
        {
            var mr = ma.Value();
            if (mr.IsBottom) return new Result<Identity<B>>(BottomException.Default);
            if (mr.IsFaulted) return new Result<Identity<B>>(mr.Exception);
            return new Result<Identity<B>>(new Identity<B>(f(mr.Value)));
        };

        public static Try<Lst<B>> Traverse<A, B>(this Lst<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<Lst<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<Lst<B>>(new Lst<B>(res));
        };

        public static Try<Fin<B>> Traverse<A, B>(this Fin<Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsFail) return new Result<Fin<B>>(ma.Cast<B>());
            var mr = ma.Value();
            if (mr.IsBottom) return new Result<Fin<B>>(BottomException.Default);
            if (mr.IsFaulted) return new Result<Fin<B>>(mr.Exception);
            return new Result<Fin<B>>(Fin<B>.Succ(f(mr.Value)));
        };
        
        public static Try<Option<B>> Traverse<A, B>(this Option<Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsNone) return new Result<Option<B>>(Option<B>.None);
            #nullable disable
            var mr = ma.Value();
            #nullable enable
            if (mr.IsBottom) return new Result<Option<B>>(BottomException.Default);
            if (mr.IsFaulted) return new Result<Option<B>>(mr.Exception);
            return new Result<Option<B>>(Option<B>.Some(f(mr.Value)));
        };
        
        public static Try<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsNone) return new Result<OptionUnsafe<B>>(OptionUnsafe<B>.None);
            var mr = ma.Value();
            if (mr.IsBottom) return new Result<OptionUnsafe<B>>(BottomException.Default);
            if (mr.IsFaulted) return new Result<OptionUnsafe<B>>(mr.Exception);
            return new Result<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(mr.Value)));
        };

        public static Try<Que<B>> Traverse<A, B>(this Que<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<Que<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<Que<B>>(new Que<B>(res));
        };

        public static Try<Seq<B>> Traverse<A, B>(this Seq<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<Seq<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<Seq<B>>(Seq.FromArray(res));
        };

        public static Try<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new List<B>();
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<IEnumerable<B>>(x.Exception);
                res.Add(f(x.Value));
            }

            return new Result<IEnumerable<B>>(res);
        };

        public static Try<Set<B>> Traverse<A, B>(this Set<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<Set<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix++;
            }

            return new Result<Set<B>>(new Set<B>(res));
        };

        public static Try<Stck<B>> Traverse<A, B>(this Stck<Try<A>> ma, Func<A, B> f) => () =>
        {
            var res = new B[ma.Count];
            var ix = ma.Count - 1;
            foreach (var xs in ma)
            {
                var x = xs();
                if (x.IsFaulted) return new Result<Stck<B>>(x.Exception);
                res[ix] = f(x.Value);
                ix--;
            }

            return new Result<Stck<B>>(new Stck<B>(res));
        };
        
        public static Try<Try<B>> Traverse<A, B>(this Try<Try<A>> ma, Func<A, B> f) => () =>
        {
            var mb = ma.Try();
            if (mb.IsBottom) return Result<Try<B>>.Bottom;
            if (mb.IsFaulted) return new Result<Try<B>>(TryFail<B>(mb.Exception));
            var mr = mb.Value.Try();
            if (mr.IsBottom) return Result<Try<B>>.Bottom;
            if (mr.IsFaulted) return new Result<Try<B>>(mr.Exception);
            return new Result<Try<B>>(Try(f(mr.Value)));
        };
        
        public static Try<TryOption<B>> Traverse<A, B>(this TryOption<Try<A>> ma, Func<A, B> f) => () =>
        {
            var mb = ma.Try();
            if (mb.IsBottom) return Result<TryOption<B>>.Bottom;
            if (mb.IsFaulted) return new Result<TryOption<B>>(TryOptionFail<B>(mb.Exception));
            if (mb.IsNone) return new Result<TryOption<B>>(TryOptional<B>(None));
            var mr = mb.Value.Value.Try();
            if (mr.IsBottom) return Result<TryOption<B>>.Bottom;
            if (mr.IsFaulted) return new Result<TryOption<B>>(mr.Exception);
            return new Result<TryOption<B>>(TryOption(f(mr.Value)));
        };
        
        public static Try<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Try<A>> ma, Func<A, B> f) => () =>
        {
            if (ma.IsFail)
            {
                return new Result<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
            }
            else
            {
                var mr = ma.SuccessValue();  
                if (mr.IsBottom) return new Result<Validation<Fail, B>>(BottomException.Default);
                if (mr.IsFaulted) return new Result<Validation<Fail, B>>(mr.Exception);
                return new Result<Validation<Fail, B>>(Validation<Fail, B>.Success(f(mr.Value)));
            }
        };

        public static Try<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(
            this Validation<MonoidFail, Fail, Try<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> => () =>
        {
            if (ma.IsFail)
            {
                return new Result<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
            }
            else
            {
                var mr = ma.SuccessValue();  
                if (mr.IsBottom) return new Result<Validation<MonoidFail, Fail, B>>(BottomException.Default);
                if (mr.IsFaulted) return new Result<Validation<MonoidFail, Fail, B>>(mr.Exception);
                return new Result<Validation<MonoidFail, Fail, B>>(Validation<MonoidFail, Fail, B>.Success(f(mr.Value)));
            }
        };
        
        public static Try<Eff<B>> Traverse<A, B>(this Eff<Try<A>> ma, Func<A, B> f) => () =>
        {
            var mb = ma.Run();
            if (mb.IsBottom) return Result<Eff<B>>.Bottom;
            if (mb.IsFail) return new Result<Eff<B>>(FailEff<B>(mb.Error));
            var mr = mb.Value.Try();
            if (mr.IsBottom) return Result<Eff<B>>.Bottom;
            if (mr.IsFaulted) return new Result<Eff<B>>(mr.Exception);
            return new Result<Eff<B>>(SuccessEff<B>(f(mr.Value)));
        };    }
}
