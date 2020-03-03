using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static class ValidationTExtensions
    {
        public static Validation<MonoidL, L, Arr<B>> Traverse<MonoidL, L, A, B>(this Arr<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new Arr<B>(res)
                : Validation<MonoidL, L, Arr<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Either<L, B>> Traverse<MonoidL, L, A, B>(this Either<L, Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsLeft)
            {
                return Validation<MonoidL, L, Either<L, B>>.Fail(ma.LeftValue);
            }
            else
            {
                var mb = (Validation<MonoidL, L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<MonoidL, L, Either<L, B>>.Fail(mb.FailValue);
                }
                else
                {
                    return Validation<MonoidL, L, Either<L, B>>.Success(f((A)mb));
                }
            }
        }
        
        public static Validation<MonoidL, L, EitherUnsafe<L, B>> Traverse<MonoidL, L, A, B>(this EitherUnsafe<L, Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsLeft)
            {
                return Validation<MonoidL, L, EitherUnsafe<L, B>>.Fail(ma.LeftValue);
            }
            else
            {
                var mb = (Validation<MonoidL, L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<MonoidL, L, EitherUnsafe<L, B>>.Fail(mb.FailValue);
                }
                else
                {
                    return Validation<MonoidL, L, EitherUnsafe<L, B>>.Success(f((A)mb));
                }
            }
        }
        
        public static Validation<MonoidL, L, HashSet<B>> Traverse<MonoidL, L, A, B>(this HashSet<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new HashSet<B>(res)
                : Validation<MonoidL, L, HashSet<B>>.Fail(errs);
        }

        public static Validation<MonoidL, L, Identity<B>> Traverse<MonoidL, L, A, B>(this Identity<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.Value.IsFail)
            {
                return Validation<MonoidL, L, Identity<B>>.Fail(ma.Value.FailValue);
            }
            else
            {
                return Validation<MonoidL, L, Identity<B>>.Success(new Identity<B>(f((A)ma.Value)));
            }
        }

        public static Validation<MonoidL, L, Lst<B>> Traverse<MonoidL, L, A, B>(this Lst<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new Lst<B>(res)
                : Validation<MonoidL, L, Lst<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Option<B>> Traverse<MonoidL, L, A, B>(this Option<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsNone)
            {
                return Validation<MonoidL, L, Option<B>>.Success(Option<B>.None);
            }
            else
            {
                var mb = (Validation<MonoidL, L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<MonoidL, L, Option<B>>.Fail(mb.FailValue);
                }
                else
                {
                    return Validation<MonoidL, L, Option<B>>.Success(f((A)mb));
                }
            }
        }        
        
        public static Validation<MonoidL, L, OptionUnsafe<B>> Traverse<MonoidL, L, A, B>(this OptionUnsafe<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsNone)
            {
                return Validation<MonoidL, L, OptionUnsafe<B>>.Success(OptionUnsafe<B>.None);
            }
            else
            {
                var mb = (Validation<MonoidL, L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<MonoidL, L, OptionUnsafe<B>>.Fail(mb.FailValue);
                }
                else
                {
                    return Validation<MonoidL, L, OptionUnsafe<B>>.Success(f((A)mb));
                }
            }
        }        
        
        public static Validation<MonoidL, L, Que<B>> Traverse<MonoidL, L, A, B>(this Que<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new Que<B>(res)
                : Validation<MonoidL, L, Que<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Seq<B>> Traverse<MonoidL, L, A, B>(this Seq<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? Seq.FromArray(res)
                : Validation<MonoidL, L, Seq<B>>.Fail(errs);
        }
                
        public static Validation<MonoidL, L, IEnumerable<B>> Traverse<MonoidL, L, A, B>(this IEnumerable<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new List<B>();
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res.Add(f((A)x));                    
                }
            }

            return isSuccess
                ? Validation<MonoidL, L, IEnumerable<B>>.Success(res)
                : Validation<MonoidL, L, IEnumerable<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Set<B>> Traverse<MonoidL, L, A, B>(this Set<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new Set<B>(res)
                : Validation<MonoidL, L, Set<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Stck<B>> Traverse<MonoidL, L, A, B>(this Stck<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var res = new B[ma.Count];
            var errs = default(MonoidL).Empty();
            var isSuccess = true;
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs = default(MonoidL).Append(errs, x.FailValue);
                    isSuccess = false;
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return isSuccess
                ? new Stck<B>(res)
                : Validation<MonoidL, L, Stck<B>>.Fail(errs);
        }
        
        public static Validation<MonoidL, L, Try<B>> Traverse<MonoidL, L, A, B>(this Try<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var tres = ma.Try();
            
            if (tres.IsBottom || tres.IsFaulted)
            {
                return default(MValidation<MonoidL, L, Try<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsFail)
            {
                return Validation<MonoidL, L, Try<B>>.Fail(tres.Value.FailValue);
            }
            else
            {
                return Validation<MonoidL, L, Try<B>>.Success(Try(f((A)tres.Value)));
            }
        }
        
        public static Validation<MonoidL, L, TryOption<B>> Traverse<MonoidL, L, A, B>(this TryOption<Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            var tres = ma.Try();
            
            if (tres.IsBottom || tres.IsFaulted)
            {
                return default(MValidation<MonoidL, L, TryOption<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsNone)
            {
                return Validation<MonoidL, L, TryOption<B>>.Success(TryOption<B>(None));
            }
            else if (tres.Value.Value.IsFail)
            {
                return Validation<MonoidL, L, TryOption<B>>.Fail(tres.Value.Value.FailValue);
            }
            else
            {
                return Validation<MonoidL, L, TryOption<B>>.Success(TryOption(f((A)tres.Value.Value)));
            }
        }
        
        public static Validation<MonoidL, L, Validation<L, B>> Traverse<MonoidL, L, A, B>(this Validation<L, Validation<MonoidL, L, A>> ma, Func<A, B> f)
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsFail)
            {
                return Validation<MonoidL, L, Validation<L, B>>.Fail(mconcat<MonoidL, L>(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsSuccess)
                {
                    return Validation<MonoidL, L, Validation<L, B>>.Success(f((A)mb));
                }
                else
                {
                    return Validation<MonoidL, L, Validation<L, B>>.Fail(mb.FailValue);
                }
            }
        }
        
        public static Validation<MonoidL, L, Validation<MonoidL, L, B>> Traverse<MonoidL, L, A, B>(this Validation<MonoidL, L, Validation<MonoidL, L, A>> ma, Func<A, B> f) 
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsFail)
            {
                return Validation<MonoidL, L, Validation<MonoidL, L, B>>.Fail(ma.FailValue);
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsSuccess)
                {
                    return Validation<MonoidL, L, Validation<MonoidL, L, B>>.Success(f((A)mb));
                }
                else
                {
                    return Validation<MonoidL, L, Validation<MonoidL, L, B>>.Fail(mb.FailValue);
                }
            }
        }
    }
}
