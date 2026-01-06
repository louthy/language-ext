using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ArrExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Arr<A> Flatten<A>(this Arr<Arr<A>> mma)
    {
        var writer = ArrayWriter<A>.Init();
        Arr.FoldState state = default!;
        Foldable.stepSetup(mma, ref state);
        while (Foldable.step(mma, ref state, out var ma))
        {
            ArrayWriter<A>.AddRange(ref writer, ma.AsSpan());
        }
        return writer.ToArr();
    }

    extension<A>(K<Arr, A> ma)
    {
        [Pure]
        public Arr<A> Filter(Func<A, bool> f)
        {
            var writer = ArrayWriter<A>.Init(ma.Count);
        
            Arr.FoldState state = default!;
            Foldable.stepSetup(ma, ref state);
            while (Foldable.step(ma, ref state, out var a))
            {
                if(f(a)) ArrayWriter<A>.Add(ref writer, a);
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Map<B>(Func<A, B> f) 
        {
            var writer = ArrayWriter<B>.Init(ma.Count);
        
            Arr.FoldState state = default!;
            Foldable.stepSetup(ma, ref state);
            while (Foldable.step(ma, ref state, out var a))
            {
                ArrayWriter<B>.Add(ref writer, f(a));
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Bind<B>(Func<A, Arr<B>> f)
        {
            var writer = ArrayWriter<B>.Init();
        
            Arr.FoldState astate = default!;
            Foldable.stepSetup(ma, ref astate);
            while (Foldable.step(ma, ref astate, out var a))
            {
                var           mb     = +f(a);
                Arr.FoldState bstate = default!;
                Foldable.stepSetup(mb, ref bstate);
                while (Foldable.step(mb, ref bstate, out var b))
                {
                    ArrayWriter<B>.Add(ref writer, b);
                }
            }
            return writer.ToArr();
        }
        
        [Pure]
        public Arr<A> As() =>
            (Arr<A>)ma;
    }    
    
    extension<A>(Arr<A> ma)
    {

    }
}
