using System;

namespace LanguageExt.HKT;

/// <summary>
/// Monad interface
/// </summary>
/// <typeparam name="M">Monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Monad<M, A> : Applicative<M, A>
    where M : Monad<M>
{
    public Monad<M, B> Select<B>(Func<A, B> f) =>
        M.Map(this, f);
    
    public Monad<M, C> SelectMany<MB, B, C>(Func<A, MB> bind, Func<A, B, C> project) where MB : Monad<M, B> =>
        M.Bind(this, x => bind(x).Select(y => project(x, y)));
}
