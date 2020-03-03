using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationSeqTExtensions
    {
        public static Validation<L, Arr<B>> Traverse<L, A, B>(this Arr<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new Arr<B>(res)
                : Validation<L, Arr<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Either<L, B>> Traverse<L, A, B>(this Either<L, Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Validation<L, Either<L, B>>.Fail(Seq1(ma.LeftValue));
            }
            else
            {
                var mb = (Validation<L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<L, Either<L, B>>.Fail((Seq<L>)mb);
                }
                else
                {
                    return Validation<L, Either<L, B>>.Success(f((A)mb));
                }
            }
        }
        
        public static Validation<L, EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsLeft)
            {
                return Validation<L, EitherUnsafe<L, B>>.Fail(Seq1(ma.LeftValue));
            }
            else
            {
                var mb = (Validation<L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<L, EitherUnsafe<L, B>>.Fail((Seq<L>)mb);
                }
                else
                {
                    return Validation<L, EitherUnsafe<L, B>>.Success(f((A)mb));
                }
            }
        }
        
        public static Validation<L, HashSet<B>> Traverse<L, A, B>(this HashSet<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new HashSet<B>(res)
                : Validation<L, HashSet<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }

        public static Validation<L, Identity<B>> Traverse<L, A, B>(this Identity<Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.Value.IsFail)
            {
                return Validation<L, Identity<B>>.Fail((Seq<L>)ma.Value);
            }
            else
            {
                return Validation<L, Identity<B>>.Success(new Identity<B>(f((A)ma.Value)));
            }
        }

        public static Validation<L, Lst<B>> Traverse<L, A, B>(this Lst<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new Lst<B>(res)
                : Validation<L, Lst<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Option<B>> Traverse<L, A, B>(this Option<Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Validation<L, Option<B>>.Success(Option<B>.None);
            }
            else
            {
                var mb = (Validation<L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<L, Option<B>>.Fail((Seq<L>)mb);
                }
                else
                {
                    return Validation<L, Option<B>>.Success(f((A)mb));
                }
            }
        }        
        
        public static Validation<L, OptionUnsafe<B>> Traverse<L, A, B>(this OptionUnsafe<Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsNone)
            {
                return Validation<L, OptionUnsafe<B>>.Success(OptionUnsafe<B>.None);
            }
            else
            {
                var mb = (Validation<L, A>)ma;
                if (mb.IsFail)
                {
                    return Validation<L, OptionUnsafe<B>>.Fail((Seq<L>)mb);
                }
                else
                {
                    return Validation<L, OptionUnsafe<B>>.Success(f((A)mb));
                }
            }
        }        
        
        public static Validation<L, Que<B>> Traverse<L, A, B>(this Que<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new Que<B>(res)
                : Validation<L, Que<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Seq<B>> Traverse<L, A, B>(this Seq<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? Seq.FromArray(res)
                : Validation<L, Seq<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, IEnumerable<B>> Traverse<L, A, B>(this IEnumerable<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new List<B>();
            var errs = new List<L>();
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res.Add(f((A)x));                    
                }
            }

            return errs.Count == 0
                ? Validation<L, IEnumerable<B>>.Success(res)
                : Validation<L, IEnumerable<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Set<B>> Traverse<L, A, B>(this Set<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new Set<B>(res)
                : Validation<L, Set<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Stck<B>> Traverse<L, A, B>(this Stck<Validation<L, A>> ma, Func<A, B> f)
        {
            var res = new B[ma.Count];
            var errs = new List<L>();
            var ix = 0;
            foreach (var x in ma)
            {
                if (x.IsFail)
                {
                    errs.AddRange((Seq<L>)x);
                }
                else
                {
                    res[ix] = f((A)x);                    
                    ix++;
                }
            }

            return errs.Count == 0
                ? new Stck<B>(res)
                : Validation<L, Stck<B>>.Fail(Seq.FromArray(errs.ToArray()));
        }
        
        public static Validation<L, Try<B>> Traverse<L, A, B>(this Try<Validation<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom || tres.IsFaulted)
            {
                return default(MValidation<L, Try<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsFail)
            {
                return Validation<L, Try<B>>.Fail((Seq<L>)tres.Value);
            }
            else
            {
                return Validation<L, Try<B>>.Success(Try(f((A)tres.Value)));
            }
        }
        
        public static Validation<L, TryOption<B>> Traverse<L, A, B>(this TryOption<Validation<L, A>> ma, Func<A, B> f)
        {
            var tres = ma.Try();
            
            if (tres.IsBottom || tres.IsFaulted)
            {
                return default(MValidation<L, TryOption<B>>).Fail(tres.Exception);
            }
            else if (tres.Value.IsNone)
            {
                return Validation<L, TryOption<B>>.Success(TryOption<B>(None));
            }
            else if (tres.Value.Value.IsFail)
            {
                return Validation<L, TryOption<B>>.Fail((Seq<L>)tres.Value.Value);
            }
            else
            {
                return Validation<L, TryOption<B>>.Success(TryOption(f((A)tres.Value.Value)));
            }
        }
        
        public static Validation<L, Validation<L, B>> Traverse<L, A, B>(this Validation<L, Validation<L, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail)
            {
                return Validation<L, Validation<L, B>>.Fail(ma.FailValue);
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsSuccess)
                {
                    return Validation<L, Validation<L, B>>.Success(f((A)mb));
                }
                else
                {
                    return Validation<L, Validation<L, B>>.Fail((Seq<L>)mb);
                }
            }
        }
        
        public static Validation<L, Validation<MonoidL, L, B>> Traverse<MonoidL, L, A, B>(this Validation<MonoidL, L, Validation<L, A>> ma, Func<A, B> f) 
            where MonoidL : struct, Monoid<L>, Eq<L>
        {
            if (ma.IsFail)
            {
                return Validation<L, Validation<MonoidL, L, B>>.Fail(Seq1(ma.FailValue));
            }
            else
            {
                var mb = ma.SuccessValue;
                if (mb.IsSuccess)
                {
                    return Validation<L, Validation<MonoidL, L, B>>.Success(f((A)mb));
                }
                else
                {
                    return Validation<L, Validation<MonoidL, L, B>>.Fail((Seq<L>)mb);
                }
            }
        }
    }
}
