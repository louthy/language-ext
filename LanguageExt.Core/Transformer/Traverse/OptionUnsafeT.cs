#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class OptionUnsafeT
    {
        public static OptionUnsafe<Arr<B>> Traverse<A, B>(this Arr<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }

            return OptionUnsafe<Arr<B>>.Some(new Arr<B>(res));
        }

        public static OptionUnsafe<Either<L, B>> Traverse<L, A, B>(this Either<L, OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return SomeUnsafe(Either<L, B>.Left(ma.LeftValue));
            }
            else if (ma.RightValue.IsNone)
            {
                return None;
            }
            else
            {
                return OptionUnsafe<Either<L, B>>.Some(f(ma.RightValue.Value));
            }
        }
        
        public static OptionUnsafe<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return SomeUnsafe(EitherUnsafe<L, B>.Left(ma.LeftValue));
            }
            else if (ma.RightValue.IsNone)
            {
                return None;
            }
            else
            {
                return OptionUnsafe<EitherUnsafe<L, B>>.Some(f(ma.RightValue.Value));
            }
        }

        public static OptionUnsafe<HashSet<B>> Traverse<A, B>(this HashSet<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }
            return OptionUnsafe<HashSet<B>>.Some(new HashSet<B>(res));            
        }

        public static OptionUnsafe<Identity<B>> Traverse<A, B>(this Identity<OptionUnsafe<A>> ma, Func<A, B> f) =>
            ma.Value.IsSome
                ? OptionUnsafe<Identity<B>>.Some(new Identity<B>(f(ma.Value.Value)))
                : OptionUnsafe<Identity<B>>.None;
        
        public static OptionUnsafe<Lst<B>> Traverse<A, B>(this Lst<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }
            return OptionUnsafe<Lst<B>>.Some(new Lst<B>(res));                
        }

        public static OptionUnsafe<Fin<B>> Traverse<A, B>(this Fin<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return SomeUnsafe<Fin<B>>(ma.Cast<B>());
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(FinSucc(f(ma.Value.Value)));
            }
        }

        public static OptionUnsafe<Option<B>> Traverse<A, B>(this Option<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return SomeUnsafe<Option<B>>(None);
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(Some(f(ma.Value.Value)));
            }
        }
        
        public static OptionUnsafe<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return SomeUnsafe<OptionUnsafe<B>>(None);
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(SomeUnsafe(f(ma.Value.Value)));
            }
        }
        
        public static OptionUnsafe<Que<B>> Traverse<A, B>(this Que<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }
            return OptionUnsafe<Que<B>>.Some(new Que<B>(res));                
        }
        
        public static OptionUnsafe<Seq<B>> Traverse<A, B>(this Seq<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }
            return OptionUnsafe<Seq<B>>.Some(Seq.FromArray<B>(res));                
        }
        
        public static OptionUnsafe<IEnumerable<B>> Traverse<A, B>(this IEnumerable<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res.Add(f(xs.Value));
            }
            return OptionUnsafe<IEnumerable<B>>.Some(Seq.FromArray<B>(res.ToArray()));                
        }
        
        public static OptionUnsafe<Set<B>> Traverse<A, B>(this Set<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix++;
            }
            return OptionUnsafe<Set<B>>.Some(new Set<B>(res));                
        }
        
        public static OptionUnsafe<Stck<B>> Traverse<A, B>(this Stck<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = ma.Count - 1;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                res[ix] = f(xs.Value);
                ix--;
            }
            return OptionUnsafe<Stck<B>>.Some(new Stck<B>(res));                
        }
        
        public static OptionUnsafe<Try<B>> Traverse<A, B>(this Try<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFaulted)
            {
                return SomeUnsafe(TryFail<B>(tres.Exception));
            }
            else if (tres.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(Try(f(tres.Value.Value)));
            }
        }
        
        public static OptionUnsafe<TryOption<B>> Traverse<A, B>(this TryOption<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFaulted)
            {
                return SomeUnsafe(TryOptionFail<B>(tres.Exception));
            }
            else if (tres.Value.IsNone)
            {
                return SomeUnsafe(TryOptional<B>(None));
            }
            else if (tres.Value.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(TryOption(f(tres.Value.Value.Value)));
            }
        }
        
        public static OptionUnsafe<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, OptionUnsafe<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return SomeUnsafe(Fail<Fail, B>(ma.FailValue));
            }
            else if (ma.SuccessValue.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(Validation<Fail, B>.Success(f(ma.SuccessValue.Value)));
            }
        }

        public static OptionUnsafe<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(
            this Validation<MonoidFail, Fail, OptionUnsafe<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail)
            {
                return SomeUnsafe(Fail<MonoidFail, Fail, B>(ma.FailValue));
            }
            else if (ma.SuccessValue.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(Validation<MonoidFail, Fail, B>.Success(f(ma.SuccessValue.Value)));
            }
        }
                
        public static OptionUnsafe<Eff<B>> Traverse<A, B>(this Eff<OptionUnsafe<A>> ma, Func<A, B> f)
        {
            var tres = ma.Run();

            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFail)
            {
                return SomeUnsafe(FailEff<B>(tres.Error));
            }
            else if (tres.Value.IsNone)
            {
                return None;
            }
            else
            {
                return SomeUnsafe(SuccessEff(f(tres.Value.Value)));
            }
        }
    }
}
