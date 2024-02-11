namespace LanguageExt.HKT;

/// <summary>
/// Arrow kind: `* -> *` used to represent higher-kinded types.
/// </summary>
/// <remarks>
/// `K<F, A>` should be thought of as `F<A>` - which currently can't be represented in C#, so this allows
/// us to define higher-kinded types and pass them around.  We can then build traits that expected a `K`
/// where the trait is tied to the `F`.  For example:
///
///     K<F, A> where F : Functor<F>
///     K<M, A> where M : Monad<F>
///
/// That means we can write generic functions that work with monads, functors, etc.
/// </remarks>
/// <typeparam name="F">Trait type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface K<in F, out A>
{
    K<F, A> Kind { get; }
}
