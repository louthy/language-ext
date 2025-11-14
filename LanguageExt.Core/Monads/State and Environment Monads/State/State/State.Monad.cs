using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Traits implementation for `State` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
public partial class State<S> : 
    Monad<State<S>>, 
    Choice<State<S>>,
    Stateful<State<S>, S>
{
    static K<State<S>, B> Monad<State<S>>.Bind<A, B>(K<State<S>, A> ma, Func<A, K<State<S>, B>> f) => 
        ma.As().Bind(f);

    static K<State<S>, B> Functor<State<S>>.Map<A, B>(Func<A, B> f, K<State<S>, A> ma) => 
        ma.As().Map(f);

    static K<State<S>, A> Applicative<State<S>>.Pure<A>(A value) => 
        State<S, A>.Pure(value);

    static K<State<S>, B> Applicative<State<S>>.Apply<A, B>(K<State<S>, Func<A, B>> mf, K<State<S>, A> ma) => 
        mf.As().Bind(x => ma.As().Map(x));

    static K<State<S>, B> Applicative<State<S>>.Action<A, B>(K<State<S>, A> ma, K<State<S>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<State<S>, Unit> Stateful<State<S>, S>.Modify(Func<S, S> modify) => 
        State<S, S>.Modify(modify);

    static K<State<S>, A> Stateful<State<S>, S>.Gets<A>(Func<S, A> f) => 
        State<S, A>.Gets(f);

    static K<State<S>, Unit> Stateful<State<S>, S>.Put(S value) => 
        State<S, S>.Put(value);

    static K<State<S>, A> Choice<State<S>>.Choose<A>(K<State<S>, A> fa, K<State<S>, A> fb) => 
        fa;

    static K<State<S>, A> Choice<State<S>>.Choose<A>(K<State<S>, A> fa, Func<K<State<S>, A>> fb) => 
        fa;
}
