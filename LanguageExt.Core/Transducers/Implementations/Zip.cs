#nullable enable

using System;

namespace LanguageExt;

record ZipTransducer2<E, A, B>(Transducer<E, A> First, Transducer<E, B> Second)
    : Transducer<E, (A First, B Second)>
{
    public override Reducer<E, S> Transform<S>(Reducer<(A First, B Second), S> reduce) =>
        (from a in First
         from b in Second
         select (a, b)).Transform(reduce);
                                
    public override string ToString() =>  
        $"zip";
}

record ZipSumTransducer2<E, X, A, B>(Transducer<E, Sum<X, A>> First, Transducer<E, Sum<X, B>> Second)
    : Transducer<E, Sum<X, (A First, B Second)>>
{
    public override Reducer<E, S> Transform<S>(Reducer<Sum<X, (A First, B Second)>, S> reduce) =>
        (from xa in First
         from xb in Second
         select (xa, xb) switch
                {
                    (SumRight<X, A> a, SumRight<X, B> b) => Sum<X, (A, B)>.Right((a.Value, b.Value)),
                    (SumLeft<X, A> a, _)                 => Sum<X, (A, B)>.Left(a.Value),
                    (_, SumLeft<X, B> b)                 => Sum<X, (A, B)>.Left(b.Value),
                    _                                    => throw new NotSupportedException("Should be Left or Right")
                }).Transform(reduce);
                                
    public override string ToString() =>  
        $"zip";
}

record ZipTransducer3<E, A, B, C>(
        Transducer<E, A> First, 
        Transducer<E, B> Second,
        Transducer<E, C> Third)
    : Transducer<E, (A First, B Second, C Third)>
{
    public override Reducer<E, S> Transform<S>(Reducer<(A First, B Second, C Third), S> reduce) =>
        (from a in First
         from b in Second
         from c in Third
         select (a, b, c)).Transform(reduce);
    
    public override string ToString() =>  
        $"zip";
}

record ZipSumTransducer3<E, X, A, B, C>(
        Transducer<E, Sum<X, A>> First, 
        Transducer<E, Sum<X, B>> Second,
        Transducer<E, Sum<X, C>> Third
        )
    : Transducer<E, Sum<X, (A First, B Second, C Third)>>
{
    public override Reducer<E, S> Transform<S>(Reducer<Sum<X, (A First, B Second, C Third)>, S> reduce) =>
        (from xa in First
         from xb in Second
         from xc in Third
         select (xa, xb, xc) switch
                {
                    (SumRight<X, A> a, SumRight<X, B> b, SumRight<X, C> c) => Sum<X, (A, B, C)>.Right((a.Value, b.Value, c.Value)),
                    (SumLeft<X, A> a, _, _)                                => Sum<X, (A, B, C)>.Left(a.Value),
                    (_, SumLeft<X, B> b, _)                                => Sum<X, (A, B, C)>.Left(b.Value),
                    (_, _, SumLeft<X, C> c)                                => Sum<X, (A, B, C)>.Left(c.Value),
                    _                                                      => throw new NotSupportedException("Should be Left or Right")
                }).Transform(reduce);
    
    public override string ToString() =>  
        $"zip";
}


record ZipTransducer4<E, A, B, C, D>(
    Transducer<E, A> First, 
    Transducer<E, B> Second,
    Transducer<E, C> Third,
    Transducer<E, D> Fourth)
    : Transducer<E, (A First, B Second, C Third, D Fourth)>
{
    public override Reducer<E, S> Transform<S>(Reducer<(A First, B Second, C Third, D Fourth), S> reduce) =>
        (from a in First
         from b in Second
         from c in Third
         from d in Fourth
         select (a, b, c, d)).Transform(reduce);
    
    public override string ToString() =>  
        $"zip";
}


record ZipSumTransducer4<E, X, A, B, C, D>(
    Transducer<E, Sum<X, A>> First, 
    Transducer<E, Sum<X, B>> Second,
    Transducer<E, Sum<X, C>> Third,
    Transducer<E, Sum<X, D>> Fourth
)
    : Transducer<E, Sum<X, (A First, B Second, C Third, D Fourth)>>
{
    public override Reducer<E, S> Transform<S>(Reducer<Sum<X, (A First, B Second, C Third, D Fourth)>, S> reduce) =>
        (from xa in First
         from xb in Second
         from xc in Third
         from xd in Fourth
         select (xa, xb, xc, xd) switch
                {
                    (SumRight<X, A> a, SumRight<X, B> b, SumRight<X, C> c, SumRight<X, D> d) =>
                        Sum<X, (A, B, C, D)>.Right((a.Value, b.Value, c.Value, d.Value)),
                    
                    (SumLeft<X, A> a, _, _, _) => Sum<X, (A, B, C, D)>.Left(a.Value),
                    (_, SumLeft<X, B> b, _, _) => Sum<X, (A, B, C, D)>.Left(b.Value),
                    (_, _, SumLeft<X, C> c, _) => Sum<X, (A, B, C, D)>.Left(c.Value),
                    (_, _, _, SumLeft<X, D> d) => Sum<X, (A, B, C, D)>.Left(d.Value),
                    _                          => throw new NotSupportedException("Should be Left or Right")
                }).Transform(reduce);
    
    public override string ToString() =>  
        $"zip";
}
