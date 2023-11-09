#nullable enable
using System;
using LanguageExt.Common;

namespace LanguageExt.Transducers;

public static class TResult
{
    public static TResult<A> Continue<A>(A value) => new TContinue<A>(value);
    public static TResult<A> Complete<A>(A value) => new TComplete<A>(value);
    public static TResult<A> Cancel<A>() => TCancelled<A>.Default;
    public static TResult<A> None<A>() => TNone<A>.Default;
    public static TResult<A> Fail<A>(Error Error) => new TFail<A>(Error);
    
    public static TResult<S> Recursive<S, A>(TState st, S s, A value, Reducer<S, A> reduce) => 
        new TRecursive<S>(new TRecursiveReduce<S, A>(st, s, value, reduce));
    
    public static TResult<S> Recursive<S>(TRecursiveRunner<S> reduce) => 
        new TRecursive<S>(reduce);
}

public abstract record TResultBase
{
    public virtual Error ErrorUnsafe => throw new InvalidOperationException("Can't call ErrorUnsafe on a TResult that succeeded");
    public abstract bool Success { get; }
    public abstract bool Continue { get; }
    public abstract bool Faulted { get; }
    public abstract bool Recursive { get; }
}
public abstract record TResult<A> : TResultBase
{
    public virtual A ValueUnsafe => throw new InvalidOperationException("Can't call ValueUnsafe on a TResult that has no value");
    public abstract TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer);
    public abstract TResult<B> Map<B>(Func<A, B> f);
    public abstract TResult<B> Bind<B>(Func<A, TResult<B>> f);
}
public sealed record TContinue<A>(A Value) : TResult<A>
{
    public override bool Success => true;
    public override bool Continue => true;
    public override bool Faulted => false;
    public override bool Recursive => false;
    public override A ValueUnsafe => Value;

    public override TResult<B> Map<B>(Func<A, B> f) =>
        TResult.Continue(f(Value));

    public override TResult<B> Bind<B>(Func<A, TResult<B>> f) =>
        f(Value);

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer) =>
        TResult.Recursive(state, stateValue, Value, reducer);

    public override string ToString() =>
        $"Continue({Value})";
}
public sealed record TComplete<A>(A Value) : TResult<A>
{
    public override bool Success => true;
    public override bool Continue => false;
    public override bool Faulted => false;
    public override bool Recursive => false;
    public override A ValueUnsafe => Value;

    public override TResult<B> Map<B>(Func<A, B> f) =>
        TResult.Complete(f(Value));

    public override TResult<B> Bind<B>(Func<A, TResult<B>> f) =>
        f(Value);

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer) =>
        TResult.Complete(stateValue);

    public override string ToString() =>
        $"Complete({Value})";
}
public sealed record TCancelled<A> : TResult<A>
{
    public static readonly TResult<A> Default = new TCancelled<A>();
    
    public override bool Success => false;
    public override bool Continue => false;
    public override bool Faulted => true;
    public override bool Recursive => false;
    public override Error ErrorUnsafe => Errors.Cancelled;

    public override TResult<B> Map<B>(Func<A, B> _) =>
        TCancelled<B>.Default;

    public override TResult<B> Bind<B>(Func<A, TResult<B>> _) =>
        TCancelled<B>.Default;

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer) =>
        TCancelled<S>.Default;
    
    public override string ToString() =>
        "Cancelled";
}
public sealed record TNone<A> : TResult<A>
{
    public static readonly TResult<A> Default = new TNone<A>();
    
    public override bool Success => true;
    public override bool Continue => false;
    public override bool Recursive => false;
    public override bool Faulted => false;

    public override TResult<B> Map<B>(Func<A, B> _) =>
        TNone<B>.Default;

    public override TResult<B> Bind<B>(Func<A, TResult<B>> _) =>
        TNone<B>.Default;

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer) =>
        TNone<S>.Default;

    public override string ToString() =>
        "None";
}
public sealed record TFail<A>(Error Error) : TResult<A>
{
    public override bool Success => false;
    public override bool Continue => false;
    public override bool Faulted => true;
    public override bool Recursive => false;
    public override Error ErrorUnsafe => Error;

    public override TResult<B> Map<B>(Func<A, B> _) =>
        TResult.Fail<B>(Error);

    public override TResult<B> Bind<B>(Func<A, TResult<B>> _) =>
        TResult.Fail<B>(Error);

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<S, A> reducer) =>
        TResult.Fail<S>(Error);

    public override string ToString() =>
        $"Fail({Error})";
}

public sealed record TRecursive<A>(TRecursiveRunner<A> Runner) : TResult<A>
{
    public override bool Success => false;
    public override bool Continue => false;
    public override bool Faulted => false;
    public override bool Recursive => true;

    public override TResult<B> Map<B>(Func<A, B> f) =>
        TResult.Recursive(Runner.Map(f));

    public override TResult<B> Bind<B>(Func<A, TResult<B>> f) =>
        TResult.Recursive(Runner.Bind(f));
    
    public TResult<A> Run() =>
        Runner.Run();

    public override TResult<S> Reduce<S>(TState st, S s, Reducer<S, A> r) =>
        Runner.Run() switch
        {
            TContinue<A> va => TResult.Recursive(st, s, va.Value, r),
            TComplete<A> => TResult.Complete(s),
            TFail<A> f => TResult.Fail<S>(f.Error),
            TCancelled<A> => TResult.Cancel<S>(),
            TNone<A> => TResult.None<S>(),
            TRecursive<A> vr => vr.Bind(a => TResult.Recursive(st, s, a, r)),
            _ => throw new NotSupportedException()
        };

    public override string ToString() =>
        "Recursive";
}

public abstract record TRecursiveRunner<A>
{
    public abstract TResult<A> Run();

    public TRecursiveRunner<B> Map<B>(Func<A, B> f) =>
        new TRecursiveMap<A, B>(this, f);
    
    public TRecursiveRunner<B> Bind<B>(Func<A, TResult<B>> f) =>
        new TRecursiveBind<A, B>(this, f);
}

public sealed record TRecursiveReduce<S, A>(TState State, S StateValue, A Value, Reducer<S, A> Next) 
    : TRecursiveRunner<S>
{
    public override TResult<S> Run() =>
        Next.Run(State, StateValue, Value);
}

public sealed record TRecursiveMap<A, B>(TRecursiveRunner<A> Next, Func<A, B> F)
    : TRecursiveRunner<B>
{
    public override TResult<B> Run() =>
        Next.Run().Map(F);
}

public sealed record TRecursiveBind<A, B>(TRecursiveRunner<A> Next, Func<A, TResult<B>> F)
    : TRecursiveRunner<B>
{
    public override TResult<B> Run() =>
        Next.Run().Bind(F);
}
