using LanguageExt.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public partial class Fin
{
    /// <summary>
    /// Construct a `Fin` value in a `Succ` state (success)
    /// </summary>
    /// <param name="value">Value to construct the `Succ` state with</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed `Fin` value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Succ<A>(A value) => 
        new Fin<A>.Succ(value);

    /// <summary>
    /// Construct a `Fin` value in a `Fail` state
    /// </summary>
    /// <param name="value">Value to construct the `Fail` state with</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed `Fin` value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail<A>(Error error) => 
        new Fin<A>.Fail(error);

    /// <summary>
    /// Construct a `Fin` value in a `Fail` state
    /// </summary>
    /// <param name="value">Value to construct the `Fail` state with</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed `Fin` value</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail<A>(string error) => 
        new Fin<A>.Fail(Error.New(error));
}
