#nullable enable
using System;
using System.Diagnostics;
using LanguageExt.Common;

namespace LanguageExt.Transducers;

record BindTransducer1<A, B, C>(Transducer<A, B> M, Transducer<B, Transducer<A, C>> F) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    record Reduce<S>(Transducer<A, B> M, Transducer<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder1<S>(value, F, Reducer)).Run(st, s, value);
    }
    
    record Binder1<S>(A Value, Transducer<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B value) =>
            TResult.Recursive(st, s, value, F.Transform(new Binder2<S>(Value, Reducer)));
    }
    
    record Binder2<S>(A Value, Reducer<C, S> Reducer) : Reducer<Transducer<A, C>, S>
    {
        public override TResult<S> Run(TState st, S s, Transducer<A, C> f) =>
            f.Transform(Reducer).Run(st, s, Value);
    }
    
    public override string ToString() =>  
        "bind";
}

record BindTransducer2<A, B, C>(Transducer<A, B> M, Transducer<B, Func<A, C>> F) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(M, F, reduce);
    
    record Reduce<S>(Transducer<A, B> M, Transducer<B, Func<A, C>> F, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder1<S>(value, F, Reducer)).Run(st, s, value);
    }
    
    record Binder1<S>(A Value, Transducer<B, Func<A, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B value) =>
            TResult.Recursive(st, s, value, F.Transform(new Binder2<S>(Value, Reducer)));
    }
    
    record Binder2<S>(A Value, Reducer<C, S> Reducer) : Reducer<Func<A, C>, S>
    {
        public override TResult<S> Run(TState st, S s, Func<A, C> f) =>
            Reducer.Run(st, s, f(Value));
    }
    
    public override string ToString() =>  
        "bind";
}

record BindTransducer3<A, B, C>(Transducer<A, B> M, Func<B, Transducer<A, C>> F) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    internal record Reduce<S>(Transducer<A, B> M, Func<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            TResult.Recursive(st, s, value, M.Transform(new Binder<S>(value, F, Reducer)));
    }
    
    internal record Binder<S>(A Value, Func<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B value) =>
            F(value).Transform(Reducer).Run(st, s, Value);
    }
    
    public override string ToString() =>  
        "bind";
}

record BindTransducer3A<A, B, C>(Transducer<A, B> M, Func<B, Transducer<Unit, C>> F) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    internal record Reduce<S>(Transducer<A, B> M, Func<B, Transducer<Unit, C>> F, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder<S>(F, Reducer)).Run(st, s, value);
    }
    
    internal record Binder<S>(Func<B, Transducer<Unit, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B value) =>
            TResult.Recursive(st, s, default, F(value).Transform(Reducer));
    }
    
    public override string ToString() =>  
        "bind";
}

record BindTransducer3B<A, B, C>(Transducer<Unit, B> M, Func<B, Transducer<A, C>> F) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    internal record Reduce<S>(Transducer<Unit, B> M, Func<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder<S>(value, F, Reducer)).Run(st, s, default);
    }
    
    internal record Binder<S>(A Value, Func<B, Transducer<A, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B value) =>
            TResult.Recursive(st, s, Value, F(value).Transform(Reducer));
    }
    
    public override string ToString() =>  
        "bind";
}

record BindTransducerSum<X, A, B, C>(Transducer<A, Sum<X, B>> M, Transducer<B, Transducer<A, Sum<X, C>>> F) : 
    Transducer<A, Sum<X, C>>
{
    public override Reducer<A, S> Transform<S>(Reducer<Sum<X, C>, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    record Reduce<S>(Transducer<A, Sum<X, B>> M, Transducer<B, Transducer<A, Sum<X, C>>> F, Reducer<Sum<X, C>, S> Reducer) 
        : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder1<S>(value, F, Reducer)).Run(st, s, value);
    }
    
    record Binder1<S>(A Value, Transducer<B, Transducer<A, Sum<X, C>>> F, Reducer<Sum<X, C>, S> Reducer) : Reducer<Sum<X, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Sum<X, B> value) =>
            value switch
            {
                SumRight<X, B> r =>
                    TResult.Recursive(st, s, r.Value, F.Transform(new Binder2<S>(Value, Reducer))),
                
                SumLeft<X, B> l => Reducer.Run(st, s, Sum<X, C>.Left(l.Value)),
                _ => TResult.Complete(s)
            };
    }
    
    record Binder2<S>(A Value, Reducer<Sum<X, C>, S> Reducer) : Reducer<Transducer<A, Sum<X, C>>, S>
    {
        public override TResult<S> Run(TState st, S s, Transducer<A, Sum<X, C>> f) =>
            f.Transform(Reducer).Run(st, s, Value);
    }

    public override string ToString() =>  
        "bind";
}

record BindTransducerSum2<X, A, B, C>(Transducer<A, Sum<X, B>> M, Func<B, Transducer<A, Sum<X, C>>> F) : 
    Transducer<A, Sum<X, C>>
{
    public override Reducer<A, S> Transform<S>(Reducer<Sum<X, C>, S> reduce) =>
        new Reduce<S>(M, F, reduce);

    record Reduce<S>(Transducer<A, Sum<X, B>> M, Func<B, Transducer<A, Sum<X, C>>> F, Reducer<Sum<X, C>, S> Reducer) 
        : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A value) =>
            M.Transform(new Binder1<S>(value, F, Reducer)).Run(st, s, value);
    }
    
    record Binder1<S>(A Value, Func<B, Transducer<A, Sum<X, C>>> F, Reducer<Sum<X, C>, S> Reducer) : Reducer<Sum<X, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Sum<X, B> value) =>
            value switch
            {
                SumRight<X, B> r =>
                    TResult.Recursive(st, s, Value, F(r.Value).Transform(Reducer)),
                
                SumLeft<X, B> l => Reducer.Run(st, s, Sum<X, C>.Left(l.Value)),
                _ => TResult.Complete(s)
            };
    }
    
    public override string ToString() =>  
        "bind";
}
