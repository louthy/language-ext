#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class EitherUnsafeT
    {
        public static EitherUnsafe<L, Arr<B>> Traverse<L, A, B>(this Arr<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Arr<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Arr<B>(res);
        }
        
        public static EitherUnsafe<L, Either<L, B>> Traverse<L, A, B>(this Either<L, EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Right(Either<L, B>.Left((L)ma));
            }
            else
            {
                var mb = (EitherUnsafe<L, A>)ma;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Either<L, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Either<L, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Right(EitherUnsafe<L, B>.Left((L)ma));
            }
            else
            {
                var mb = (EitherUnsafe<L, A>)ma;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, EitherUnsafe<L, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, EitherUnsafe<L, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, HashSet<B>> Traverse<L, A, B>(this HashSet<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, HashSet<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new HashSet<B>(res);
        }
                
        public static EitherUnsafe<L, Identity<B>> Traverse<L, A, B>(this Identity<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.Value.IsLeft)
            {
                return EitherUnsafe<L, Identity<B>>.Left((L)ma.Value);
            }
            else
            {
                return EitherUnsafe<L, Identity<B>>.Right(new Identity<B>(f((A)ma.Value)));
            }
        }
        
        public static EitherUnsafe<L, Lst<B>> Traverse<L, A, B>(this Lst<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Lst<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Lst<B>(res);
        }
        
        public static EitherUnsafe<L, Fin<B>> Traverse<L, A, B>(this Fin<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return Right(ma.Cast<B>());
            }
            else
            {
                var mb = (EitherUnsafe<L, A>)ma;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Fin<B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Fin<B>>.Right(f((A)mb));
                }
            }
        }        
        
        public static EitherUnsafe<L, Option<B>> Traverse<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Right(Option<B>.None);
            }
            else
            {
                var mb = (EitherUnsafe<L, A>)ma;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Option<B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Option<B>>.Right(f((A)mb));
                }
            }
        }        
        
        public static EitherUnsafe<L, OptionUnsafe<B>> Traverse<L, A, B>(this OptionUnsafe<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Right(OptionUnsafe<B>.None);
            }
            else
            {
                var mb = (EitherUnsafe<L, A>)ma;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, OptionUnsafe<B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, OptionUnsafe<B>>.Right(f((A)mb));
                }
            }
        }        
        
        public static EitherUnsafe<L, Que<B>> Traverse<L, A, B>(this Que<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Que<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Que<B>(res);
        }
        
        public static EitherUnsafe<L, Seq<B>> Traverse<L, A, B>(this Seq<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Seq<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return Seq.FromArray<B>(res);
        }
        
        public static EitherUnsafe<L, IEnumerable<B>> Traverse<L, A, B>(this IEnumerable<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, IEnumerable<B>>.Left((L)x);
                }
                else
                {
                    res.Add(f((A)x));                    
                }
            }
            return Seq.FromArray<B>(res.ToArray());
        }
        
        public static EitherUnsafe<L, Set<B>> Traverse<L, A, B>(this Set<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Set<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }
            return new Set<B>(res);
        }
        
        public static EitherUnsafe<L, Stck<B>> Traverse<L, A, B>(this Stck<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var ix = ma.Count - 1;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Stck<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix--;
                }
            }
            return new Stck<B>(res);
        }
        
        public static EitherUnsafe<L, Try<B>> Traverse<L, A, B>(this Try<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return EitherUnsafe<L, Try<B>>.Bottom;
            }
            else if (tres.IsFaulted)
            {
                return Right(TryFail<B>(tres.Exception));
            }
            else if (tres.Value.IsLeft)
            {
                return EitherUnsafe<L, Try<B>>.Left((L)tres.Value);
            }
            else
            {
                return EitherUnsafe<L, Try<B>>.Right(Try(f((A)tres.Value)));
            }
        }
        
        public static EitherUnsafe<L, TryOption<B>> Traverse<L, A, B>(this TryOption<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom)
            {
                return EitherUnsafe<L, TryOption<B>>.Bottom;
            }
            else if (tres.IsFaulted)
            {
                return Right(TryOptionFail<B>(tres.Exception));
            }
            else if (tres.IsNone)
            {
                return Right(TryOptional<B>(None));
            }
            else if (tres.Value.Value.IsLeft)
            {
                return EitherUnsafe<L, TryOption<B>>.Left((L)tres.Value.Value);
            }
            else
            {
                return EitherUnsafe<L, TryOption<B>>.Right(TryOption(f((A)tres.Value.Value)));
            }
        }
        
        public static EitherUnsafe<Fail, Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, EitherUnsafe<Fail, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail && ma.FailValue.IsEmpty)
            {
                return EitherUnsafe<Fail, Validation<Fail, B>>.Bottom;
            }
            if (ma.IsFail)
            {
                return Right(Validation<Fail, B>.Fail(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<Fail, Validation<Fail, B>>.Left((Fail)mb);
                }
                else
                {
                    return EitherUnsafe<Fail, Validation<Fail, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, EitherUnsafe<Fail, A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail)
            {
                return Right(Validation<MonoidFail, Fail, B>.Fail(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<Fail, Validation<MonoidFail, Fail, B>>.Left((Fail)mb);
                }
                else
                {
                    return EitherUnsafe<Fail, Validation<MonoidFail, Fail, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, Validation<Fail, B>> Traverse<Fail, L, A, B>(this Validation<Fail, EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return Right(Validation<Fail, B>.Fail(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Validation<Fail, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Validation<Fail, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, L, A, B>(this Validation<MonoidFail, Fail, EitherUnsafe<L, A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail)
            {
                return Right(Validation<MonoidFail, Fail, B>.Fail(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Validation<MonoidFail, Fail, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Validation<MonoidFail, Fail, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, Eff<B>> Traverse<L, A, B>(this Eff<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Run();
            
            if (tres.IsBottom)
            {
                return EitherUnsafe<L, Eff<B>>.Bottom;
            }
            else if (tres.IsFail)
            {
                return RightUnsafe(FailEff<B>(tres.Error));
            }
            else if (tres.Value.IsLeft)
            {
                return EitherUnsafe<L, Eff<B>>.Left((L)tres.Value);
            }
            else
            {
                return EitherUnsafe<L, Eff<B>>.Right(SuccessEff(f((A)tres.Value)));
            }
        }
    }
}
