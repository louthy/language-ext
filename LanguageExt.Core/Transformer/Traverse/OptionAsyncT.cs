#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class OptionAsyncT
    {
        //
        // Collections
        //
        
        public static OptionAsync<Arr<B>> Traverse<A, B>(this Arr<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));
 
        public static OptionAsync<HashSet<B>> Traverse<A, B>(this HashSet<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<IEnumerable<B>> Traverse<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));

        public static OptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, SysInfo.DefaultAsyncSequenceParallelism, f);
 
        public static OptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<OptionAsync<A>> ma, int windowSize, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseParallel(f, windowSize));
               
        [Obsolete("use SequenceSerial or SequenceParallel instead")]
        public static OptionAsync<IEnumerable<A>> Sequence<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseParallel(ma, identity);
 
        public static OptionAsync<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseSerial(ma, identity);
 
        public static OptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseParallel(ma, identity);

        public static OptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<OptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        

        public static OptionAsync<Lst<B>> Traverse<A, B>(this Lst<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));

        public static OptionAsync<Que<B>> Traverse<A, B>(this Que<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<Seq<B>> Traverse<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<Seq<B>> TraverseSerial<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));

        public static OptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, SysInfo.DefaultAsyncSequenceParallelism, f);
 
        public static OptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<OptionAsync<A>> ma, int windowSize, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseParallel(f, windowSize));
               
        [Obsolete("use SequenceSerial or SequenceParallel instead")]
        public static OptionAsync<Seq<A>> Sequence<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static OptionAsync<Seq<A>> SequenceSerial<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static OptionAsync<Seq<A>> SequenceParallel<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static OptionAsync<Seq<A>> SequenceParallel<A>(this Seq<OptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        
        
        public static OptionAsync<Set<B>> Traverse<A, B>(this Set<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));

        public static OptionAsync<Stck<B>> Traverse<A, B>(this Stck<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).TraverseSerial(f));
        
        //
        // Async types
        //

        public static OptionAsync<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<OptionAsync<B>> Traverse<A, B>(this OptionAsync<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<TryAsync<B>> Traverse<A, B>(this TryAsync<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<Task<B>> Traverse<A, B>(this Task<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<ValueTask<B>> Traverse<A, B>(this ValueTask<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
                
        public static OptionAsync<Aff<B>> Traverse<A, B>(this Aff<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        //
        // Sync types
        // 
        
        public static OptionAsync<Either<L, B>> Traverse<L, A, B>(this Either<L, OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<Identity<B>> Traverse<A, B>(this Identity<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<Fin<B>> Traverse<A, B>(this Fin<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));

        public static OptionAsync<Option<B>> Traverse<A, B>(this Option<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<Try<B>> Traverse<A, B>(this Try<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<TryOption<B>> Traverse<A, B>(this TryOption<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, OptionAsync<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            new(ma.Map(static a => a.Effect).Traverse(f));
        
        public static OptionAsync<Eff<B>> Traverse<A, B>(this Eff<OptionAsync<A>> ma, Func<A, B> f) =>
            new(ma.Map(static a => a.Effect).Traverse(f));
    }
}
