/*
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Derive your concrete types from this interface to acquire to common set of
/// operators that will make working with the generic `Fallible` trait more elegant.
/// 
/// It isn't a requirement to implement this interface for `Fallible` to work, but
/// it guides the implementor. It is optional!
/// </summary>
/// <remarks>
/// Primarily makes `@catch` work nicely, but is generally beneficial.  
/// </remarks>
/// <typeparam name="FA">'Self' type, for example `Either〈L, R〉`</typeparam>
/// <typeparam name="F">Trait implementation, for example `Either〈L〉`</typeparam>
/// <typeparam name="E">Failure type, for example `L`</typeparam>
/// <typeparam name="A">Bound value type, for example `R`</typeparam>
public interface Fallible<FA, F, E, A> : 
    K<F, A>
    where FA : 
        Fallible<FA, F, E, A>
    where F :
        Fallible<E, F>, 
        Applicative<F>
{
    [Pure]
    public static abstract implicit operator FA(Fail<E> fail);
    
    [Pure]
    public static abstract implicit operator FA(Pure<A> fail);

    [Pure]
    public static abstract FA operator |(FA lhs, FA rhs);

    [Pure]
    public static abstract FA operator |(K<F, A> lhs, FA rhs);

    [Pure]
    public static abstract FA operator |(FA lhs, K<F, A> rhs);

    [Pure]
    public static abstract FA operator |(FA lhs, Pure<A> rhs);

    [Pure]
    public static abstract FA operator |(FA lhs, Fail<E> rhs);

    [Pure]
    public static abstract FA operator |(FA lhs, CatchM<E, F, A> rhs);
}
*/
