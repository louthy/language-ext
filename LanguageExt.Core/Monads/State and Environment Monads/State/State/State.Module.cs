using System;

namespace LanguageExt;

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class State<S>
{
    public static State<S, A> Pure<A>(A value) => 
        State<S, A>.Pure(value);
}

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class State
{
    public static State<S, B> bind<S, A, B>(State<S, A> ma, Func<A, State<S, B>> f) => 
        ma.As().Bind(f);

    public static State<S, B> map<S, A, B>(Func<A, B> f, State<S, A> ma) =>  
        ma.As().Map(f);

    public static State<S, A> Pure<S, A>(A value) =>  
        State<S, A>.Pure(value);

    public static State<S, B> apply<S, A, B>(State<S, Func<A, B>> mf, State<S, A> ma) =>  
        mf.As().Bind(x =>ma.As().Map(x));

    public static State<S, B> action<S, A, B>(State<S, A> ma, State<S, B> mb) => 
        ma.As().Bind(_ => mb);
    
    public static State<S, S> get<S>() => 
        State<S, S>.Get;
    
    public static State<S, A> gets<S, A>(Func<S, A> f) => 
        State<S, A>.Gets(f);
    
    public static State<S, A> getsM<S, A>(Func<S, State<S, A>> f) => 
        State<S, A>.GetsM(f);

    public static State<S, Unit> put<S>(S state) =>  
        State<S, Unit>.Put(state);

    public static State<S, Unit> modify<S>(Func<S, S> f) =>  
        State<S, Unit>.Modify(f);
}
