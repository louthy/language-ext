using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class EitherAsyncT
    {
        //
        // Collections
        //

        public static EitherAsync<L, Arr<B>> Traverse<L, A, B>(this Arr<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Arr<B>>(Go(ma, f));
            async Task<EitherData<L, Arr<B>>> Go(Arr<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,Arr<B>>(d.Left)).Head()
                    : EitherData.Right<L, Arr<B>>(new Arr<B>(rb.Map(d => d.Right)));
            }
        }

        public static EitherAsync<L, HashSet<B>> Traverse<L, A, B>(this HashSet<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, HashSet<B>>(Go(ma, f));
            async Task<EitherData<L, HashSet<B>>> Go(HashSet<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,HashSet<B>>(d.Left)).Head()
                    : EitherData.Right<L, HashSet<B>>(new HashSet<B>(rb.Map(d => d.Right)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static EitherAsync<L, IEnumerable<B>> Traverse<L, A, B>(this IEnumerable<EitherAsync<L, A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);

        public static EitherAsync<L, IEnumerable<B>> TraverseSerial<L, A, B>(this IEnumerable<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, IEnumerable<B>>(Go(ma, f));
            async Task<EitherData<L, IEnumerable<B>>> Go(IEnumerable<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a;
                    if (mb.IsBottom) return default(EitherData<L, IEnumerable<B>>);
                    if (mb.IsLeft) return EitherData.Left<L, IEnumerable<B>>(mb.LeftValue);
                    rb.Add(f(mb.RightValue));
                }

                return EitherData.Right<L, IEnumerable<B>>(rb);
            };
        }

        public static EitherAsync<L, IEnumerable<B>> TraverseParallel<L, A, B>(this IEnumerable<EitherAsync<L, A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);
 
        public static EitherAsync<L, IEnumerable<B>> TraverseParallel<L, A, B>(this IEnumerable<EitherAsync<L, A>> ma, int windowSize, Func<A, B> f)
        {
            return new EitherAsync<L, IEnumerable<B>>(Go(ma, f));
            async Task<EitherData<L, IEnumerable<B>>> Go(IEnumerable<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Data).WindowMap(windowSize, identity);
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,IEnumerable<B>>(d.Left)).Head()
                    : EitherData.Right<L, IEnumerable<B>>(rb.Map(d => d.Right));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static EitherAsync<L, IEnumerable<A>> Sequence<L, A>(this IEnumerable<EitherAsync<L, A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static EitherAsync<L, IEnumerable<A>> SequenceSerial<L, A>(this IEnumerable<EitherAsync<L, A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static EitherAsync<L, IEnumerable<A>> SequenceParallel<L, A>(this IEnumerable<EitherAsync<L, A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static EitherAsync<L, IEnumerable<A>> SequenceParallel<L, A>(this IEnumerable<EitherAsync<L, A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity); 

        public static EitherAsync<L, Lst<B>> Traverse<L, A, B>(this Lst<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Lst<B>>(Go(ma, f));
            async Task<EitherData<L, Lst<B>>> Go(Lst<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,Lst<B>>(d.Left)).Head()
                    : EitherData.Right<L, Lst<B>>(new Lst<B>(rb.Map(d => d.Right)));
            }
        }

        public static EitherAsync<L, Que<B>> Traverse<L, A, B>(this Que<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Que<B>>(Go(ma, f));
            async Task<EitherData<L, Que<B>>> Go(Que<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,Que<B>>(d.Left)).Head()
                    : EitherData.Right<L, Que<B>>(new Que<B>(rb.Map(d => d.Right)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static EitherAsync<L, Seq<B>> Traverse<L, A, B>(this Seq<EitherAsync<L, A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static EitherAsync<L, Seq<B>> TraverseSerial<L, A, B>(this Seq<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Seq<B>>(Go(ma, f));
            async Task<EitherData<L, Seq<B>>> Go(Seq<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = new B[ma.Count];
                var ix = 0;
                foreach (var a in ma)
                {
                    var mb = await a;
                    if (mb.IsBottom) return default(EitherData<L, Seq<B>>);
                    if (mb.IsLeft) return EitherData.Left<L, Seq<B>>(mb.LeftValue);
                    rb[ix] = f(mb.RightValue);
                    ix++;
                }

                return EitherData.Right<L, Seq<B>>(Seq.FromArray<B>(rb));
            };
        }

        public static EitherAsync<L, Seq<B>> TraverseParallel<L, A, B>(this Seq<EitherAsync<L, A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);
 
        public static EitherAsync<L, Seq<B>> TraverseParallel<L, A, B>(this Seq<EitherAsync<L, A>> ma, int windowSize, Func<A, B> f)
        {
            return new EitherAsync<L, Seq<B>>(Go(ma, f));
            async Task<EitherData<L, Seq<B>>> Go(Seq<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Data).WindowMap(windowSize, identity);
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                     ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L, Seq<B>>(d.Left)).Head()
                     : EitherData.Right<L, Seq<B>>(Seq.FromArray(rb.Map(d => d.Right).ToArray()));
            }
        }

        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static EitherAsync<L, Seq<A>> Sequence<L, A>(this Seq<EitherAsync<L, A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static EitherAsync<L, Seq<A>> SequenceSerial<L, A>(this Seq<EitherAsync<L, A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static EitherAsync<L, Seq<A>> SequenceParallel<L, A>(this Seq<EitherAsync<L, A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static EitherAsync<L, Seq<A>> SequenceParallel<L, A>(this Seq<EitherAsync<L, A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        

        public static EitherAsync<L, Set<B>> Traverse<L, A, B>(this Set<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Set<B>>(Go(ma, f));
            async Task<EitherData<L, Set<B>>> Go(Set<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,Set<B>>(d.Left)).Head()
                    : EitherData.Right<L, Set<B>>(new Set<B>(rb.Map(d => d.Right)));
            }
        }
        
        public static EitherAsync<L, Stck<B>> Traverse<L, A, B>(this Stck<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Stck<B>>(Go(ma, f));
            async Task<EitherData<L, Stck<B>>> Go(Stck<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Reverse().Map(a => a.Map(f).Data));
                return rb.Exists(d => d.State == EitherStatus.IsLeft)
                    ? rb.Filter(d => d.State == EitherStatus.IsLeft).Map(d => EitherData.Left<L,Stck<B>>(d.Left)).Head()
                    : EitherData.Right<L, Stck<B>>(new Stck<B>(rb.Map(d => d.Right)));
            }
        }
        
        //
        // Async types
        //

        public static EitherAsync<L, EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, EitherAsync<L, B>>(Go(ma, f));
            async Task<EitherData<L, EitherAsync<L, B>>> Go(EitherAsync<L, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var da = await ma.Data;
                if (da.State == EitherStatus.IsBottom) return EitherData<L, EitherAsync<L, B>>.Bottom;
                if (da.State == EitherStatus.IsLeft) return EitherData.Right<L, EitherAsync<L, B>>(EitherAsync<L, B>.Left(da.Left));
                var db = await da.Right.Data;
                if (db.State == EitherStatus.IsBottom) return EitherData<L, EitherAsync<L, B>>.Bottom;
                if (db.State == EitherStatus.IsLeft) return EitherData.Left<L, EitherAsync<L, B>>(db.Left);
                return EitherData.Right<L, EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(db.Right)));
            }
        }

        public static EitherAsync<L, OptionAsync<B>> Traverse<L, A, B>(this OptionAsync<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, OptionAsync<B>>(Go(ma, f));
            async Task<EitherData<L, OptionAsync<B>>> Go(OptionAsync<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data;
                if (!isSome) return EitherData.Right<L, OptionAsync<B>>(OptionAsync<B>.None);
                var db = await value.Data;
                if (db.State == EitherStatus.IsBottom) return EitherData<L, OptionAsync<B>>.Bottom;
                if (db.State == EitherStatus.IsLeft) return EitherData.Left<L, OptionAsync<B>>(db.Left);
                return EitherData.Right<L, OptionAsync<B>>(OptionAsync<B>.Some(f(db.Right)));
            }
        }
        
        public static EitherAsync<L, TryAsync<B>> Traverse<L, A, B>(this TryAsync<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, TryAsync<B>>(Go(ma, f));
            async Task<EitherData<L, TryAsync<B>>> Go(TryAsync<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var result = await ma.Try();
                if (result.IsBottom) return EitherData<L, TryAsync<B>>.Bottom;
                if (result.IsFaulted) return EitherData.Right<L, TryAsync<B>>(TryAsyncFail<B>(result.Exception));
                var db = await result.Value.Data;
                if (db.State == EitherStatus.IsBottom) return EitherData<L, TryAsync<B>>.Bottom;
                if (db.State == EitherStatus.IsLeft) return EitherData.Left<L, TryAsync<B>>(db.Left);
                return EitherData.Right<L, TryAsync<B>>(TryAsync<B>(f(db.Right)));
            }
        }
        
        public static EitherAsync<L, TryOptionAsync<B>> Traverse<L, A, B>(this TryOptionAsync<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, TryOptionAsync<B>>(Go(ma, f));
            async Task<EitherData<L, TryOptionAsync<B>>> Go(TryOptionAsync<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var result = await ma.Try();
                if (result.IsBottom) return EitherData<L, TryOptionAsync<B>>.Bottom;
                if (result.IsFaulted) return EitherData.Right<L, TryOptionAsync<B>>(TryOptionAsyncFail<B>(result.Exception));
                if (result.IsNone) return EitherData.Right<L, TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var db = await result.Value.Value.Data;
                if (db.State == EitherStatus.IsBottom) return EitherData<L, TryOptionAsync<B>>.Bottom;
                if (db.State == EitherStatus.IsLeft) return EitherData.Left<L, TryOptionAsync<B>>(db.Left);
                return EitherData.Right<L, TryOptionAsync<B>>(TryOptionAsync<B>(f(db.Right)));
            }
        }

        public static EitherAsync<L, Task<B>> Traverse<L, A, B>(this Task<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Task<B>>(Go(ma, f));
            async Task<EitherData<L, Task<B>>> Go(Task<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                var result = await ma;
                var db = await result.Data;
                if (db.State == EitherStatus.IsBottom) return EitherData<L, Task<B>>.Bottom;
                if (db.State == EitherStatus.IsLeft) return EitherData.Left<L, Task<B>>(db.Left);
                return EitherData.Right<L, Task<B>>(f(db.Right).AsTask());
            }
        }

        //
        // Sync types
        // 
        
        public static EitherAsync<L, Either<L, B>> Traverse<L, A, B>(this Either<L, EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Either<L, B>>(Go(ma, f));
            async Task<EitherData<L, Either<L, B>>> Go(Either<L, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return EitherData<L, Either<L, B>>.Bottom;
                if(ma.IsLeft) return EitherData.Right<L, Either<L, B>>(Left(ma.LeftValue));
                var da = await ma.RightValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Either<L, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Either<L, B>>(da.Left);
                return EitherData.Right<L, Either<L, B>>(f(da.Right));
            }
        }

        public static EitherAsync<L, EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, EitherUnsafe<L, B>>(Go(ma, f));
            async Task<EitherData<L, EitherUnsafe<L, B>>> Go(EitherUnsafe<L, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return EitherData<L, EitherUnsafe<L, B>>.Bottom;
                if(ma.IsLeft) return EitherData.Right<L, EitherUnsafe<L, B>>(Left(ma.LeftValue));
                var da = await ma.RightValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, EitherUnsafe<L, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, EitherUnsafe<L, B>>(da.Left);
                return EitherData.Right<L, EitherUnsafe<L, B>>(f(da.Right));
            }
        }

        public static EitherAsync<L, Identity<B>> Traverse<L, A, B>(this Identity<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Identity<B>>(Go(ma, f));
            async Task<EitherData<L, Identity<B>>> Go(Identity<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return EitherData<L, Identity<B>>.Bottom;
                var da = await ma.Value.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Identity<B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Identity<B>>(da.Left);
                return EitherData.Right<L, Identity<B>>(new Identity<B>(f(da.Right)));
            }
        }

        public static EitherAsync<L, Option<B>> Traverse<L, A, B>(this Option<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Option<B>>(Go(ma, f));
            async Task<EitherData<L, Option<B>>> Go(Option<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return EitherData.Right<L, Option<B>>(None);
                var da = await ma.Value.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Option<B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Option<B>>(da.Left);
                return EitherData.Right<L, Option<B>>(f(da.Right));
            }
        }
        
        public static EitherAsync<L, OptionUnsafe<B>> Traverse<L, A, B>(this OptionUnsafe<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, OptionUnsafe<B>>(Go(ma, f));
            async Task<EitherData<L, OptionUnsafe<B>>> Go(OptionUnsafe<EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return EitherData.Right<L, OptionUnsafe<B>>(None);
                var da = await ma.Value.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, OptionUnsafe<B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, OptionUnsafe<B>>(da.Left);
                return EitherData.Right<L, OptionUnsafe<B>>(f(da.Right));
            }
        }
        
        public static EitherAsync<L, Try<B>> Traverse<L, A, B>(this Try<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            try
            {
                return new EitherAsync<L, Try<B>>(Go(ma, f));
                async Task<EitherData<L, Try<B>>> Go(Try<EitherAsync<L, A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if (ra.IsBottom) return EitherData<L, Try<B>>.Bottom;
                    if (ra.IsFaulted) return EitherData.Right<L, Try<B>>(TryFail<B>(ra.Exception));
                    var da = await ra.Value.Data;
                    if(da.State == EitherStatus.IsBottom) return EitherData<L, Try<B>>.Bottom;
                    if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Try<B>>(da.Left);
                    return EitherData.Right<L, Try<B>>(Try<B>(f(da.Right)));
                }
            }
            catch (Exception e)
            {
                return Try<B>(e);
            }
        }
        
        public static EitherAsync<L, TryOption<B>> Traverse<L, A, B>(this TryOption<EitherAsync<L, A>> ma, Func<A, B> f)
        {
            try
            {
                return new EitherAsync<L, TryOption<B>>(Go(ma, f));
                async Task<EitherData<L, TryOption<B>>> Go(TryOption<EitherAsync<L, A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if (ra.IsBottom) return EitherData<L, TryOption<B>>.Bottom;
                    if (ra.IsNone) return EitherData.Right<L, TryOption<B>>(TryOptional<B>(None));
                    if (ra.IsFaulted) return EitherData.Right<L, TryOption<B>>(TryOptionFail<B>(ra.Exception));
                    var da = await ra.Value.Value.Data;
                    if(da.State == EitherStatus.IsBottom) return EitherData<L, TryOption<B>>.Bottom;
                    if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, TryOption<B>>(da.Left);
                    return EitherData.Right<L, TryOption<B>>(TryOption<B>(f(da.Right)));
                }
            }
            catch (Exception e)
            {
                return TryOption<B>(e);
            }
        }
        
        public static EitherAsync<L, Validation<L, B>> Traverse<L, A, B>(this Validation<L, EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Validation<L, B>>(Go(ma, f));
            async Task<EitherData<L, Validation<L, B>>> Go(Validation<L, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return EitherData.Right<L, Validation<L, B>>(Fail<L, B>(ma.FailValue));
                var da = await ma.SuccessValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Validation<L, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Validation<L, B>>(da.Left);
                return EitherData.Right<L, Validation<L, B>>(f(da.Right));
            }
        }
        
        public static EitherAsync<L, Validation<Fail, B>> Traverse<Fail, L, A, B>(this Validation<Fail, EitherAsync<L, A>> ma, Func<A, B> f)
        {
            return new EitherAsync<L, Validation<Fail, B>>(Go(ma, f));
            async Task<EitherData<L, Validation<Fail, B>>> Go(Validation<Fail, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return EitherData.Right<L, Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var da = await ma.SuccessValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Validation<Fail, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Validation<Fail, B>>(da.Left);
                return EitherData.Right<L, Validation<Fail, B>>(f(da.Right));
            }
        }
        
        public static EitherAsync<Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, EitherAsync<Fail, A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return new EitherAsync<Fail, Validation<MonoidFail, Fail, B>>(Go(ma, f));
            async Task<EitherData<Fail, Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, EitherAsync<Fail, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return EitherData.Right<Fail, Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var da = await ma.SuccessValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<Fail, Validation<MonoidFail, Fail, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<Fail, Validation<MonoidFail, Fail, B>>(da.Left);
                return EitherData.Right<Fail, Validation<MonoidFail, Fail, B>>(f(da.Right));
            }
        }
        
        public static EitherAsync<L, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, L, A, B>(this Validation<MonoidFail, Fail, EitherAsync<L, A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return new EitherAsync<L, Validation<MonoidFail, Fail, B>>(Go(ma, f));
            async Task<EitherData<L, Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, EitherAsync<L, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return EitherData.Right<L, Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var da = await ma.SuccessValue.Data;
                if(da.State == EitherStatus.IsBottom) return EitherData<L, Validation<MonoidFail, Fail, B>>.Bottom;
                if(da.State == EitherStatus.IsLeft) return EitherData.Left<L, Validation<MonoidFail, Fail, B>>(da.Left);
                return EitherData.Right<L, Validation<MonoidFail, Fail, B>>(f(da.Right));
            }
        }
    }
}
