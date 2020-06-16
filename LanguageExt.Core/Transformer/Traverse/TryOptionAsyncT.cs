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
    public static partial class TryOptionAsyncT
    {
        static TryOptionAsync<A> ToTry<A>(Func<Task<OptionalResult<A>>> ma) => 
            new TryOptionAsync<A>(ma);
        
        //
        // Collections
        //

        public static TryOptionAsync<Arr<B>> Traverse<A, B>(this Arr<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Arr<B>>> Go(Arr<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Arr<B>>(b.Exception)).Head()
                     : rb.Exists(d => d.IsNone) ? OptionalResult<Arr<B>>.None
                     : new OptionalResult<Arr<B>>(new Arr<B>(rb.Map(d => d.Value.Value)));
            }
        }

        public static TryOptionAsync<HashSet<B>> Traverse<A, B>(this HashSet<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<HashSet<B>>> Go(HashSet<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted)
                    ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<HashSet<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<HashSet<B>>.None
                    : new OptionalResult<HashSet<B>>(new HashSet<B>(rb.Map(d => d.Value.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryOptionAsync<IEnumerable<B>> Traverse<A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static TryOptionAsync<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<IEnumerable<B>>> Go(IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a.Try();
                    if (mb.IsFaulted) return new OptionalResult<IEnumerable<B>>(mb.Exception);
                    if (mb.IsNone) return OptionalResult<IEnumerable<B>>.None;
                    rb.Add(f(mb.Value.Value));
                }
                return new OptionalResult<IEnumerable<B>>(rb);
            };
        }

        public static TryOptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);

        public static TryOptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<TryOptionAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<IEnumerable<B>>> Go(IEnumerable<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Try()).WindowMap(windowSize, Prelude.identity);
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<IEnumerable<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<IEnumerable<B>>.None
                    : new OptionalResult<IEnumerable<B>>(rb.Map(d => d.Value.Value).ToArray());
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryOptionAsync<IEnumerable<A>> Sequence<A>(this IEnumerable<TryOptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static TryOptionAsync<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<TryOptionAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static TryOptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<TryOptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static TryOptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<TryOptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);

        public static TryOptionAsync<Lst<B>> Traverse<A, B>(this Lst<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Lst<B>>> Go(Lst<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Lst<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<Lst<B>>.None
                    : new OptionalResult<Lst<B>>(new Lst<B>(rb.Map(d => d.Value.Value)));
            }
        }

        public static TryOptionAsync<Que<B>> Traverse<A, B>(this Que<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Que<B>>> Go(Que<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Que<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<Que<B>>.None
                    : new OptionalResult<Que<B>>(new Que<B>(rb.Map(d => d.Value.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryOptionAsync<Seq<B>> Traverse<A, B>(this Seq<TryOptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static TryOptionAsync<Seq<B>> TraverseSerial<A, B>(this Seq<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Seq<B>>> Go(Seq<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var mb = await a.Try();
                    if (mb.IsFaulted) return new OptionalResult<Seq<B>>(mb.Exception);
                    if(mb.IsNone) return OptionalResult<Seq<B>>.None;
                    rb.Add(f(mb.Value.Value));
                }
                return new OptionalResult<Seq<B>>(Seq.FromArray(rb.ToArray()));
            };
        }

        public static TryOptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<TryOptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, Sys.DefaultAsyncSequenceConcurrency, f);
 
        public static TryOptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<TryOptionAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Seq<B>>> Go(Seq<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Try()).WindowMap(windowSize, Prelude.identity);
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Seq<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<Seq<B>>.None
                    : new OptionalResult<Seq<B>>(Seq.FromArray(rb.Map(d => d.Value.Value).ToArray()));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static TryOptionAsync<Seq<A>> Sequence<A>(this Seq<TryOptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static TryOptionAsync<Seq<A>> SequenceSerial<A>(this Seq<TryOptionAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static TryOptionAsync<Seq<A>> SequenceParallel<A>(this Seq<TryOptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static TryOptionAsync<Seq<A>> SequenceParallel<A>(this Seq<TryOptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);

        public static TryOptionAsync<Set<B>> Traverse<A, B>(this Set<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Set<B>>> Go(Set<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Set<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<Set<B>>.None
                    : new OptionalResult<Set<B>>(new Set<B>(rb.Map(d => d.Value.Value)));
            }
        }

        public static TryOptionAsync<Stck<B>> Traverse<A, B>(this Stck<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Stck<B>>> Go(Stck<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Reverse().Map(a => a.Map(f).Try()));
                return rb.Exists(d => d.IsFaulted) ? rb.Filter(b => b.IsFaulted).Map(b => new OptionalResult<Stck<B>>(b.Exception)).Head()
                    : rb.Exists(d => d.IsNone) ? OptionalResult<Stck<B>>.None
                    : new OptionalResult<Stck<B>>(new Stck<B>(rb.Map(d => d.Value.Value)));
            }
        }
        
        //
        // Async types
        //

        public static TryOptionAsync<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<EitherAsync<L, B>>> Go(EitherAsync<L, TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data;
                if (da.State == EitherStatus.IsBottom) return OptionalResult<EitherAsync<L, B>>.Bottom;
                if (da.State == EitherStatus.IsLeft) return new OptionalResult<EitherAsync<L, B>>(EitherAsync<L,B>.Left(da.Left));
                var rb = await da.Right.Try();
                if (rb.IsFaulted) return new OptionalResult<EitherAsync<L, B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<EitherAsync<L, B>>.None;
                return new OptionalResult<EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(rb.Value.Value)));
            }
        }

        public static TryOptionAsync<OptionAsync<B>> Traverse<A, B>(this OptionAsync<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<OptionAsync<B>>> Go(OptionAsync<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data;
                if (!isSome) return new OptionalResult<OptionAsync<B>>(OptionAsync<B>.None);
                var rb = await value.Try();
                if (rb.IsFaulted) return new OptionalResult<OptionAsync<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<OptionAsync<B>>.None;
                return new OptionalResult<OptionAsync<B>>(OptionAsync<B>.Some(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<TryAsync<B>> Traverse<A, B>(this TryAsync<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<TryAsync<B>>> Go(TryAsync<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try();
                if (ra.IsFaulted) return new OptionalResult<TryAsync<B>>(TryAsync<B>(ra.Exception));
                var rb = await ra.Value.Try();
                if (rb.IsFaulted) return new OptionalResult<TryAsync<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<TryAsync<B>>.None;
                return new OptionalResult<TryAsync<B>>(TryAsync<B>(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<TryOptionAsync<B>>> Go(TryOptionAsync<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try();
                if (ra.IsFaulted) return new OptionalResult<TryOptionAsync<B>>(TryOptionAsync<B>(ra.Exception));
                if (ra.IsNone) return new OptionalResult<TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var rb = await ra.Value.Value.Try();
                if (rb.IsFaulted) return new OptionalResult<TryOptionAsync<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<TryOptionAsync<B>>.None;
                return new OptionalResult<TryOptionAsync<B>>(TryOptionAsync<B>(f(rb.Value.Value)));
            }
        }

        public static TryOptionAsync<Task<B>> Traverse<A, B>(this Task<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Task<B>>> Go(Task<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var ra = await ma;
                var rb = await ra.Try();
                if (rb.IsFaulted) return new OptionalResult<Task<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Task<B>>.None;
                return new OptionalResult<Task<B>>(f(rb.Value.Value).AsTask());
            }
        }

        //
        // Sync types
        // 
        
        public static TryOptionAsync<Either<L, B>> Traverse<L, A, B>(this Either<L, TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Either<L, B>>> Go(Either<L, TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return OptionalResult<Either<L, B>>.Bottom;
                if(ma.IsLeft) return new OptionalResult<Either<L, B>>(Left<L, B>(ma.LeftValue));
                var rb = await ma.RightValue.Try();
                if(rb.IsFaulted) return new OptionalResult<Either<L, B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Either<L, B>>.None;
                return OptionalResult<Either<L, B>>.Some(f(rb.Value.Value));
            }
        }

        public static TryOptionAsync<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<EitherUnsafe<L, B>>> Go(EitherUnsafe<L, TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return OptionalResult<EitherUnsafe<L, B>>.Bottom;
                if(ma.IsLeft) return new OptionalResult<EitherUnsafe<L, B>>(LeftUnsafe<L, B>(ma.LeftValue));
                var rb = await ma.RightValue.Try();
                if(rb.IsFaulted) return new OptionalResult<EitherUnsafe<L, B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<EitherUnsafe<L, B>>.None;
                return OptionalResult<EitherUnsafe<L, B>>.Some(f(rb.Value.Value));
            }
        }

        public static TryOptionAsync<Identity<B>> Traverse<A, B>(this Identity<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Identity<B>>> Go(Identity<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return OptionalResult<Identity<B>>.Bottom;
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new OptionalResult<Identity<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Identity<B>>.None;
                return OptionalResult<Identity<B>>.Some(new Identity<B>(f(rb.Value.Value)));
            }
        }

        public static TryOptionAsync<Option<B>> Traverse<A, B>(this Option<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Option<B>>> Go(Option<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return OptionalResult<Option<B>>.Some(Option<B>.None);
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new OptionalResult<Option<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Option<B>>.None;
                return OptionalResult<Option<B>>.Some(Some(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<OptionUnsafe<B>>> Go(OptionUnsafe<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return OptionalResult<OptionUnsafe<B>>.Some(OptionUnsafe<B>.None);
                var rb = await ma.Value.Try();
                if(rb.IsFaulted) return new OptionalResult<OptionUnsafe<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<OptionUnsafe<B>>.None;
                return OptionalResult<OptionUnsafe<B>>.Some(OptionUnsafe<B>.Some(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<Try<B>> Traverse<A, B>(this Try<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Try<B>>> Go(Try<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return new OptionalResult<Try<B>>(TryFail<B>(ra.Exception));
                var rb = await ra.Value.Try();
                if (rb.IsFaulted) return new OptionalResult<Try<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Try<B>>.None;
                return OptionalResult<Try<B>>.Some(Try<B>(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<TryOption<B>> Traverse<A, B>(this TryOption<TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<TryOption<B>>> Go(TryOption<TryOptionAsync<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return OptionalResult<TryOption<B>>.Bottom;
                if (ra.IsNone) return new OptionalResult<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return new OptionalResult<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = await ra.Value.Value.Try();
                if (rb.IsFaulted) return new OptionalResult<TryOption<B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<TryOption<B>>.None;
                return OptionalResult<TryOption<B>>.Some(TryOption<B>(f(rb.Value.Value)));
            }
        }
        
        public static TryOptionAsync<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, TryOptionAsync<A>> ma, Func<A, B> f)
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Validation<Fail, B>>> Go(Validation<Fail, TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return new OptionalResult<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Try();
                if(rb.IsFaulted) return new OptionalResult<Validation<Fail, B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Validation<Fail, B>>.None;
                return OptionalResult<Validation<Fail, B>>.Some(f(rb.Value.Value));
            }
        }
        
        public static TryOptionAsync<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, TryOptionAsync<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return ToTry(() => Go(ma, f));
            async Task<OptionalResult<Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, TryOptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return new OptionalResult<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Try();
                if(rb.IsFaulted) return new OptionalResult<Validation<MonoidFail, Fail, B>>(rb.Exception);
                if(rb.IsNone) return OptionalResult<Validation<MonoidFail, Fail, B>>.None;
                return OptionalResult<Validation<MonoidFail, Fail, B>>.Some(f(rb.Value.Value));
            }
        }
    }
}
