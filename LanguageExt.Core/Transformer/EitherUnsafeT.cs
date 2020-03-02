using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EitherUnsafeTExtensions
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
                return EitherUnsafe<L, Either<L, B>>.Left((L)ma);
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
                return EitherUnsafe<L, EitherUnsafe<L, B>>.Left((L)ma);
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
        
        public static EitherUnsafe<L, Option<B>> Traverse<L, A, B>(this Option<EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return EitherUnsafe<L, Option<B>>.Right(Option<B>.None);
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
                return EitherUnsafe<L, OptionUnsafe<B>>.Right(OptionUnsafe<B>.None);
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
            return new Seq<B>(res);
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
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsLeft)
                {
                    return EitherUnsafe<L, Stck<B>>.Left((L)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
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
                return default(MEitherUnsafe<L, Try<B>>).Fail(tres.Exception);
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
                return default(MEitherUnsafe<L, TryOption<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsNone)
            {
                return EitherUnsafe<L, TryOption<B>>.Right(TryOption<B>(None));
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
        
        public static EitherUnsafe<L, Validation<L, B>> Traverse<L, A, B>(this Validation<L, EitherUnsafe<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail && ma.FailValue.IsEmpty)
            {
                return EitherUnsafe<L, Validation<L, B>>.Bottom;
            }
            if (ma.IsFail)
            {
                return EitherUnsafe<L, Validation<L, B>>.Left(ma.FailValue.Head());
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Validation<L, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Validation<L, B>>.Right(f((A)mb));
                }
            }
        }
        
        public static EitherUnsafe<L, Validation<MonoidL, L, B>> Traverse<MonoidL, L, A, B>(this Validation<MonoidL, L, EitherUnsafe<L, A>> ma, Func<A, B> f) 
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsFail)
            {
                return EitherUnsafe<L, Validation<MonoidL, L, B>>.Left(ma.FailValue);
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsLeft)
                {
                    return EitherUnsafe<L, Validation<MonoidL, L, B>>.Left((L)mb);
                }
                else
                {
                    return EitherUnsafe<L, Validation<MonoidL, L, B>>.Right(f((A)mb));
                }
            }
        }
    }
}
