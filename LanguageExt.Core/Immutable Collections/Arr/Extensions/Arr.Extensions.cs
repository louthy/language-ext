using System;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ArrExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Arr<A> Flatten<A>(this Arr<Arr<A>> mma)
    {
        var writer = ArrayWriterRef<A>.Init();
        var state = mma.StepSetup<Arr, Arr.FoldState, Arr<A>>();
        while (mma.Step(ref state, out var ma))
        {
            writer.AddRange(ma.AsSpan());
        }
        return writer.ToArr();
    }

    extension<A>(K<Arr, char> ma)
    {
        public string ToString() => 
            new (ma.As().AsSpan());
    }

    extension<A>(K<Arr, A> ma)
    {
        /// <summary>
        /// Provide a sorted Arr
        /// </summary>
        [Pure]
        public  Arr<A> Sort<OrdA>() where OrdA : Ord<A> =>
            ma.As().OrderBy(x => x, OrdComparer<OrdA, A>.Default).AsIterable().ToArr();
        
        [Pure]
        public Arr<A> Filter(Func<A, bool> f)
        {
            var writer = ArrayWriterRef<A>.Init(ma.Count);
        
            var state = ma.StepSetup<Arr, Arr.FoldState, A>();
            while (ma.Step(ref state, out var a))
            {
                if(f(a)) writer.Add(a);
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Map<B>(Func<A, B> f) 
        {
            var writer = ArrayWriterRef<B>.Init(ma.Count);
        
            var state = ma.StepSetup<Arr, Arr.FoldState, A>();
            while (ma.Step(ref state, out var a))
            {
                writer.Add(f(a));
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Bind<B>(Func<A, Arr<B>> f)
        {
            var writer = ArrayWriterRef<B>.Init();
        
            var astate = ma.StepSetup<Arr, Arr.FoldState, A>();
            while (ma.Step(ref astate, out var a))
            {
                var mb     = +f(a);
                var bstate = mb.StepSetup<Arr, Arr.FoldState, B>();
                while (mb.Step(ref bstate, out var b))
                {
                    writer.Add(b);
                }
            }
            return writer.ToArr();
        }
        
        [Pure]
        public Arr<A> As() =>
            (Arr<A>)ma;
    }    
}
