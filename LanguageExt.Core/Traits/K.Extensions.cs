using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class KExtensions
{
    /// <summary>
    /// Get the base kind type.  Avoids casts mid-expression
    /// </summary>
    public static K<F, A> Kind<F, A>(this K<F, A> fa) =>
        fa;
    
    /// <summary>
    /// Get the base kind type.  Avoids casts mid-expression
    /// </summary>
    public static K<F, A, B> Kind2<F, A, B>(this K<F, A, B> fab) =>
        fab;
    
    public static FA SafeCast<FA, F, A>(this K<F, A> fa)
        where FA : K<F, A>
        where F : Functor<F>
    {
        try
        {
            return (FA)fa;
        }
        catch(InvalidCastException)
        {
            return (FA)F.Map(x => (A)x, fa);
        }
    }
    
    /// <summary>
    /// `KindT` converts a nested Kind type (inherits `K〈M, A〉`), where the inner
    /// type is a concrete type and not `K〈N, A〉` to the more general version - which
    /// allows the other `T` variant methods to work seamlessly.
    /// </summary>
    /// <remarks>
    /// The casting of nested types is especially problematic for C#'s type-system, 
    /// so even though this isn't ideal, it does allow for a truly generic system
    /// of working with any nested types as long as there's a `Functor` implementation
    /// for the outer type.
    /// </remarks>
    /// <example>
    ///
    ///     var mx = Seq〈Option〈int〉〉(Some(1), Some(2), Some(3));
    ///         
    ///     var ma = mx.KindT〈Seq, Option, Option〈int〉, int〉()
    ///                .BindT(a => Some(a + 1))
    ///                .MapT(a => a + 1);
    ///                .AsT〈Seq, Option, Option〈int〉, int〉();
    ///
    /// </example>
    /// <param name="mna">Nested functor value</param>
    /// <typeparam name="M">Outer functor trait (i.e. `Seq`)</typeparam>
    /// <typeparam name="N">Inner trait (i.e. `Option`)</typeparam>
    /// <typeparam name="NA">Concrete nested type (i.e. `Option〈int〉`)</typeparam>
    /// <typeparam name="A">Concrete bound value type (i.e. `int`)</typeparam>
    /// <returns>More general version of the type that can be used with other `T` extensions, like `BindT`</returns>
    public static K<M, K<N, A>> KindT<M, N, NA, A>(this K<M, NA> mna)
        where NA : K<N, A>
        where M : Functor<M> =>
        M.Map(na => (K<N, A>)na, mna);

    /// <summary>
    /// `AsT` converts a nested Kind type (inherits `K〈M, A〉`), where the inner type
    /// is a general type (`K〈N, A〉`) to its downcast concrete version.
    /// </summary>
    /// <remarks>
    /// The casting of nested types is especially problematic for C#'s type-system, 
    /// so even though this isn't ideal, it does allow for a truly generic system
    /// of working with any nested types as long as there's a `Functor` implementation
    /// for the outer type.
    /// </remarks>
    /// <example>
    ///
    ///     var mx = Seq〈Option〈int〉〉(Some(1), Some(2), Some(3));
    ///         
    ///     var ma = mx.KindT〈Seq, Option, Option〈int〉, int〉()
    ///                .BindT(a => Some(a + 1))
    ///                .MapT(a => a + 1);
    ///                .AsT〈Seq, Option, Option〈int〉, int〉();
    ///
    /// </example>
    /// <param name="mna">Nested functor value</param>
    /// <typeparam name="M">Outer functor trait (i.e. `Seq`)</typeparam>
    /// <typeparam name="N">Inner trait (i.e. `Option`)</typeparam>
    /// <typeparam name="NA">Concrete nested type (i.e. `Option〈int〉`)</typeparam>
    /// <typeparam name="A">Concrete nested-monad bound value type (i.e. `int`)</typeparam>
    /// <returns>Concrete version of the general type.</returns>
    public static K<M, NA> AsT<M, N, NA, A>(this K<M, K<N, A>> mna)
        where NA : K<N, A>
        where M : Functor<M> =>
        M.Map(na => (NA)na, mna);    
}
