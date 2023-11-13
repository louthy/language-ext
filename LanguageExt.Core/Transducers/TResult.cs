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
    
    public static TResult<S> Recursive<S, A>(TState st, S s, A value, Reducer<A, S> reduce) => 
        new TRecursive<S>(new TRecursiveReduce<A, S>(st, s, value, reduce));
    
    public static TResult<S> Recursive<S>(TRecursiveRunner<S> reduce) => 
        new TRecursive<S>(reduce);

    public static Fin<A> ToFin<A>(this TResult<A> ma) =>
        ma switch
        {
            TContinue<A> r => Fin<A>.Succ(r.Value),
            TComplete<A> r => Fin<A>.Succ(r.Value),
            TFail<A> r => Fin<A>.Fail(r.Error),
            TCancelled<A> => Fin<A>.Fail(Errors.Cancelled),
            TNone<A> => Fin<A>.Fail(Errors.None),
            _ => Fin<A>.Fail(Errors.Bottom)
        };

    public static Fin<A> ToFin<A>(this TResult<Sum<Error, A>> ma) =>
        ma switch
        {
            TContinue<Sum<Error, A>> r => SumToFin(r.Value),
            TComplete<Sum<Error, A>> r => SumToFin(r.Value),
            TFail<Sum<Error, A>> r => Fin<A>.Fail(r.Error),
            TCancelled<Sum<Error, A>> => Fin<A>.Fail(Errors.Cancelled),
            TNone<Sum<Error, A>> => Fin<A>.Fail(Errors.None),
            _ => Fin<A>.Fail(Errors.Bottom)
        };

    public static Either<E, A> ToEither<E, A>(this TResult<Sum<E, A>> ma, Func<Error, Either<E, A>> errorMap) =>
        ma switch
        {
            TContinue<Sum<E, A>> r => SumToEither(r.Value),
            TComplete<Sum<E, A>> r => SumToEither(r.Value),
            TFail<Sum<E, A>> r => errorMap(r.Error),
            TCancelled<Sum<E, A>> => errorMap(Errors.Cancelled),
            TNone<Sum<E, A>> => errorMap(Errors.None),
            _ => Either<E, A>.Bottom
        };

    static Fin<A> SumToFin<A>(Sum<Error, A> value) =>
        value switch
        {
            SumRight<Error, A> r => Fin<A>.Succ(r.Value),
            SumLeft<Error, A> l => Fin<A>.Fail(l.Value),
            _ => Fin<A>.Fail(Errors.Bottom)
        };

    static Either<E, A> SumToEither<E, A>(Sum<E, A> value) =>
        value switch
        {
            SumRight<E, A> r => Either<E, A>.Right(r.Value),
            SumLeft<E, A> l => Either<E, A>.Left(l.Value),
            _ => Either<E, A>.Bottom
        };
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
    public abstract TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer);
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

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer) =>
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

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer) =>
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

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer) =>
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

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer) =>
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

    public override TResult<S> Reduce<S>(TState state, S stateValue, Reducer<A, S> reducer) =>
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

    public override TResult<S> Reduce<S>(TState st, S s, Reducer<A, S> r) =>
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

public sealed record TRecursiveReduce<A, S>(TState State, S StateValue, A Value, Reducer<A, S> Next) 
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
