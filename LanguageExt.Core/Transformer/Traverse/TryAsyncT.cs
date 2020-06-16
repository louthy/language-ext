using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class TryAsyncT
    {
        static TryAsync<A> ToTry<A>(Func<Task<Result<A>>> ma) => 
            new TryAsync<A>(ma);
        
        //
        // Collections
        //

        public static TryAsync<Arr<B>> Traverse<A, B>(this Arr<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Arr<B>>> Go(Arr<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Arr<B>>(b.Exception)).Head()
                    : new Result<Arr<B>>(new Arr<B>(rb.Map(d => d.Value)));
            }
        }

        public static TryAsync<HashSet<B>> Traverse<A, B>(this HashSet<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<HashSet<B>>> Go(HashSet<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<HashSet<B>>(b.Exception)).Head()
                    : new Result<HashSet<B>>(new HashSet<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryAsync<IEnumerable<B>> Traverse<A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static TryAsync<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<IEnumerable<B>>> Go(IEnumerable<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a.Try();
                    if (mb.IsFaulted) return new Result<IEnumerable<B>>(mb.Exception);
                    rb.Add(f(mb.Value));
                }
                return new Result<IEnumerable<B>>(rb);
            };
        }

        public static TryAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<TryAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);
 
        public static TryAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<TryAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<IEnumerable<B>>> Go(IEnumerable<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Try()).WindowMap(windowSize, Prelude.identity);
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<IEnumerable<B>>(b.Exception)).Head()
                    : new Result<IEnumerable<B>>(rb.Map(d => d.Value).ToArray());
            }
        }
                
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryAsync<IEnumerable<A>> Sequence<A>(this IEnumerable<TryAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
        
        public static TryAsync<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<TryAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static TryAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<TryAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static TryAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<TryAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        

        public static TryAsync<Lst<B>> Traverse<A, B>(this Lst<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Lst<B>>> Go(Lst<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Lst<B>>(b.Exception)).Head()
                    : new Result<Lst<B>>(new Lst<B>(rb.Map(d => d.Value)));
            }
        }

        public static TryAsync<Que<B>> Traverse<A, B>(this Que<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Que<B>>> Go(Que<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Que<B>>(b.Exception)).Head()
                    : new Result<Que<B>>(new Que<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryAsync<Seq<B>> Traverse<A, B>(this Seq<TryAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static TryAsync<Seq<B>> TraverseSerial<A, B>(this Seq<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Seq<B>>> Go(Seq<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a.Try();
                    if (mb.IsFaulted) return new Result<Seq<B>>(mb.Exception);
                    rb.Add(f(mb.Value));
                }
                return new Result<Seq<B>>(Seq.FromArray(rb.ToArray()));
            };
        }

        public static TryAsync<Seq<B>> TraverseParallel<A, B>(this Seq<TryAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);
 
        public static TryAsync<Seq<B>> TraverseParallel<A, B>(this Seq<TryAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Seq<B>>> Go(Seq<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Try()).WindowMap(windowSize, Prelude.identity);
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Seq<B>>(b.Exception)).Head()
                    : new Result<Seq<B>>(Seq.FromArray(rb.Map(d => d.Value).ToArray()));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryAsync<Seq<A>> Sequence<A>(this Seq<TryAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static TryAsync<Seq<A>> SequenceSerial<A>(this Seq<TryAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static TryAsync<Seq<A>> SequenceParallel<A>(this Seq<TryAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static TryAsync<Seq<A>> SequenceParallel<A>(this Seq<TryAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        

        public static TryAsync<Set<B>> Traverse<A, B>(this Set<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Set<B>>> Go(Set<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Set<B>>(b.Exception)).Head()
                    : new Result<Set<B>>(new Set<B>(rb.Map(d => d.Value)));
            }
        }

        public static TryAsync<Stck<B>> Traverse<A, B>(this Stck<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Stck<B>>> Go(Stck<TryAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Reverse().Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new Result<Stck<B>>(b.Exception)).Head()
                    : new Result<Stck<B>>(new Stck<B>(rb.Map(d => d.Value)));
            }
        }
        
        //
        // Async types
        //

        public static TryAsync<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<EitherAsync<L, B>>> Go(EitherAsync<L, TryAsync<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data;
                if (da.State == EitherStatus.IsBottom) return Result<EitherAsync<L, B>>.Bottom;
                if (da.State == EitherStatus.IsLeft) return new Result<EitherAsync<L,B>>(da.Left);
                var rb = await da.Right.Try();
                if (rb.IsFaulted) return new Result<EitherAsync<L, B>>(rb.Exception);
                return new Result<EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(rb.Value)));
            }
        }

        public static TryAsync<OptionAsync<B>> Traverse<A, B>(this OptionAsync<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<OptionAsync<B>>> Go(OptionAsync<TryAsync<A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data;
                if (!isSome) return new Result<OptionAsync<B>>(OptionAsync<B>.None);
                var rb = await value.Try();
                if (rb.IsFaulted) return new Result<OptionAsync<B>>(rb.Exception);
                return new Result<OptionAsync<B>>(OptionAsync<B>.Some(f(rb.Value)));
            }
        }
        
        public static TryAsync<TryAsync<B>> Traverse<A, B>(this TryAsync<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<TryAsync<B>>> Go(TryAsync<TryAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try();
                if (ra.IsFaulted) return new Result<TryAsync<B>>(TryAsync<B>(ra.Exception));
                var rb = await ra.Value.Try();
                if (rb.IsFaulted) return new Result<TryAsync<B>>(rb.Exception);
                return new Result<TryAsync<B>>(TryAsync<B>(f(rb.Value)));
            }
        }
        
        public static TryAsync<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<TryOptionAsync<B>>> Go(TryOptionAsync<TryAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try();
                if (ra.IsFaulted) return new Result<TryOptionAsync<B>>(TryOptionAsync<B>(ra.Exception));
                if (ra.IsNone) return new Result<TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var rb = await ra.Value.Value.Try();
                if (rb.IsFaulted) return new Result<TryOptionAsync<B>>(rb.Exception);
                return new Result<TryOptionAsync<B>>(TryOptionAsync<B>(f(rb.Value)));
            }
        }

        public static TryAsync<Task<B>> Traverse<A, B>(this Task<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Task<B>>> Go(Task<TryAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma;
                var rb = await ra.Try();
                if (rb.IsFaulted) return new Result<Task<B>>(rb.Exception);
                return new Result<Task<B>>(f(rb.Value).AsTask());
            }
        }

        //
        // Sync types
        // 
        
        public static TryAsync<Either<L, B>> Traverse<L, A, B>(this Either<L, TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Either<L, B>>> Go(Either<L, TryAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return Result<Either<L, B>>.Bottom;
                if(ma.IsLeft) return new Result<Either<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Try();
                if(rb.IsFaulted) return new Result<Either<L, B>>(rb.Exception);
                return new Result<Either<L, B>>(f(rb.Value));
            }
        }

        public static TryAsync<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<EitherUnsafe<L, B>>> Go(EitherUnsafe<L, TryAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return Result<EitherUnsafe<L, B>>.Bottom;
                if(ma.IsLeft) return new Result<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Try();
                if(rb.IsFaulted) return new Result<EitherUnsafe<L, B>>(rb.Exception);
                return new Result<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static TryAsync<Identity<B>> Traverse<A, B>(this Identity<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Identity<B>>> Go(Identity<TryAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return Result<Identity<B>>.Bottom;
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new Result<Identity<B>>(rb.Exception);
                return new Result<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static TryAsync<Option<B>> Traverse<A, B>(this Option<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Option<B>>> Go(Option<TryAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return new Result<Option<B>>(None);
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new Result<Option<B>>(rb.Exception);
                return new Result<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static TryAsync<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<OptionUnsafe<B>>> Go(OptionUnsafe<TryAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return new Result<OptionUnsafe<B>>(None);
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new Result<OptionUnsafe<B>>(rb.Exception);
                return new Result<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static TryAsync<Try<B>> Traverse<A, B>(this Try<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Try<B>>> Go(Try<TryAsync<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return new Result<Try<B>>(TryFail<B>(ra.Exception));
                var rb = await ra.Value.Try();
                if (rb.IsFaulted) return new Result<Try<B>>(rb.Exception);
                return new Result<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static TryAsync<TryOption<B>> Traverse<A, B>(this TryOption<TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<TryOption<B>>> Go(TryOption<TryAsync<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return Result<TryOption<B>>.Bottom;
                if (ra.IsNone) return new Result<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return new Result<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = await ra.Value.Value.Try();
                if (rb.IsFaulted) return new Result<TryOption<B>>(rb.Exception);
                return new Result<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static TryAsync<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, TryAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Validation<Fail, B>>> Go(Validation<Fail, TryAsync<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return new Result<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Try();
                if(rb.IsFaulted) return new Result<Validation<Fail, B>>(rb.Exception);
                return new Result<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static TryAsync<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, TryAsync<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return ToTry(() => Go(ma, f));
            async Task<Result<Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, TryAsync<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return new Result<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Try();
                if(rb.IsFaulted) return new Result<Validation<MonoidFail, Fail, B>>(rb.Exception);
                return new Result<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
    }
}
