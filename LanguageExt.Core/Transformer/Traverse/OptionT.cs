#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class OptionT
    {
        public static Option<Arr<B>> Traverse<A, B>(this Arr<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }

            return Option<Arr<B>>.Some(new Arr<B>(res));
        }

        public static Option<Either<L, B>> Traverse<L, A, B>(this Either<L, Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Some(Either<L, B>.Left(ma.LeftValue));
            }
            else if (ma.RightValue.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Option<Either<L, B>>.Some(f(ma.RightValue.Value));
                #nullable enable
            }
        }
        
        public static Option<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Some(EitherUnsafe<L, B>.Left(ma.LeftValue));
            }
            else if (ma.RightValue.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Option<EitherUnsafe<L, B>>.Some(f(ma.RightValue.Value));
                #nullable enable
            }
        }

        public static Option<HashSet<B>> Traverse<A, B>(this HashSet<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }
            return Option<HashSet<B>>.Some(new HashSet<B>(res));            
        }

        public static Option<Identity<B>> Traverse<A, B>(this Identity<Option<A>> ma, Func<A, B> f) =>
            #nullable disable
            ma.Value.IsSome
                ? Option<Identity<B>>.Some(new Identity<B>(f(ma.Value.Value)))
                : Option<Identity<B>>.None;
            #nullable enable
        
        public static Option<Lst<B>> Traverse<A, B>(this Lst<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }
            return Option<Lst<B>>.Some(new Lst<B>(res));                
        }

        public static Option<Fin<B>> Traverse<A, B>(this Fin<Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return Some(ma.Cast<B>());
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(FinSucc(f(ma.Value.Value)));
                #nullable enable
            }
        }

        public static Option<Option<B>> Traverse<A, B>(this Option<Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Some<Option<B>>(None);
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(Some(f(ma.Value.Value)));
                #nullable enable
            }
        }
        
        public static Option<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Some<OptionUnsafe<B>>(None);
            }
            else if (ma.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(SomeUnsafe(f(ma.Value.Value)));
                #nullable enable
            }
        }
        
        public static Option<Que<B>> Traverse<A, B>(this Que<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }
            return Option<Que<B>>.Some(new Que<B>(res));                
        }
        
        public static Option<Seq<B>> Traverse<A, B>(this Seq<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }
            return Option<Seq<B>>.Some(Seq.FromArray<B>(res));                
        }
                
        public static Option<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Option<A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res.Add(f(xs.Value));
                #nullable enable
            }
            return Option<IEnumerable<B>>.Some(Seq.FromArray<B>(res.ToArray()));                
        }
        
        public static Option<Set<B>> Traverse<A, B>(this Set<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix++;
            }
            return Option<Set<B>>.Some(new Set<B>(res));                
        }
        
        public static Option<Stck<B>> Traverse<A, B>(this Stck<Option<A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = ma.Count - 1;
            foreach (var xs in ma)
            {
                if (xs.IsNone) return None;
                #nullable disable
                res[ix] = f(xs.Value);
                #nullable enable
                ix--;
            }
            return Option<Stck<B>>.Some(new Stck<B>(res));                
        }
        
        public static Option<Try<B>> Traverse<A, B>(this Try<Option<A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();

            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFaulted)
            {
                return Some(TryFail<B>(tres.Exception));
            }
            else if (tres.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(Try(f(tres.Value.Value)));
                #nullable enable
            }
        }
        
        public static Option<TryOption<B>> Traverse<A, B>(this TryOption<Option<A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFaulted)
            {
                return Some(TryOptionFail<B>(tres.Exception));
            }
            else if (tres.Value.IsNone)
            {
                return Some(TryOptional<B>(None));
            }
            else if (tres.Value.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(TryOption(f(tres.Value.Value.Value)));
                #nullable enable
            }
        }
        
        public static Option<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Option<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return Some(Fail<Fail, B>(ma.FailValue));
            }
            else if (ma.SuccessValue.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(Validation<Fail, B>.Success(f(ma.SuccessValue.Value)));
                #nullable enable
            }
        }

        public static Option<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(
            this Validation<MonoidFail, Fail, Option<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail)
            {
                return Some(Fail<MonoidFail, Fail, B>(ma.FailValue));
            }
            else if (ma.SuccessValue.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(Validation<MonoidFail, Fail, B>.Success(f(ma.SuccessValue.Value)));
                #nullable enable
            }
        }
                
        public static Option<Eff<B>> Traverse<A, B>(this Eff<Option<A>> ma, Func<A, B> f)
        {
            var tres = ma.Run();

            if (tres.IsBottom)
            {
                return None;
            }
            else if (tres.IsFail)
            {
                return Some(FailEff<B>(tres.Error));
            }
            else if (tres.Value.IsNone)
            {
                return None;
            }
            else
            {
                #nullable disable
                return Some(SuccessEff(f(tres.Value.Value)));
                #nullable enable
            }
        }
    }
}
