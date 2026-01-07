using System;
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
        var writer = ArrayWriter<A>.Init();
        Arr.FoldState state = default!;
        mma.StepSetup(ref state);
        while (mma.Step(ref state, out var ma))
        {
            writer.AddRange(ma.AsSpan());
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
            ma.StepSetup(ref state);
            while (ma.Step(ref state, out var a))
            {
                if(f(a)) writer.Add(a);
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Map<B>(Func<A, B> f) 
        {
            var writer = ArrayWriter<B>.Init(ma.Count);
        
            Arr.FoldState state = default!;
            ma.StepSetup(ref state);
            while (ma.Step(ref state, out var a))
            {
                writer.Add(f(a));
            }
            return writer.ToArr();
        }

        [Pure]
        public Arr<B> Bind<B>(Func<A, Arr<B>> f)
        {
            var writer = ArrayWriter<B>.Init();
        
            Arr.FoldState astate = default!;
            ma.StepSetup(ref astate);
            while (ma.Step(ref astate, out var a))
            {
                var           mb     = +f(a);
                Arr.FoldState bstate = default!;
                mb.StepSetup(ref bstate);
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
