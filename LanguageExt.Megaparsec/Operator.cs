using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// This data type specifies operators that work on values of type `A`. An operator
/// is either binary infix or unary prefix or postfix. A binary operator has also
/// an associated associativity.
/// </summary>
/// <typeparam name="M">Monad trait type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public abstract record Operator<M, A>
{
    /// <summary>
    /// Non-associative infix operator
    /// </summary>
    /// <param name="Operation">Lifted binary operation</param>
    public record InfixN(K<M, Func<A, A, A>> Operation) : Operator<M, A>;

    /// <summary>
    /// Left-associative infix operator
    /// </summary>
    /// <param name="Operation">Lifted binary operation</param>
    public record InfixL(K<M, Func<A, A, A>> Operation) : Operator<M, A>;

    /// <summary>
    /// Right-associative infix operator
    /// </summary>
    /// <param name="Operation">Lifted binary operation</param>
    public record InfixR(K<M, Func<A, A, A>> Operation) : Operator<M, A>;

    /// <summary>
    /// Prefix operator
    /// </summary>
    /// <param name="Operation">Lifted unary operation</param>
    public record Prefix(K<M, Func<A, A>> Operation) : Operator<M, A>;

    /// <summary>
    /// Prefix operator
    /// </summary>
    /// <param name="Operation">Lifted unary operation</param>
    public record Postfix(K<M, Func<A, A>> Operation) : Operator<M, A>;

    /// <summary>
    /// Right-associative ternary. 
    /// </summary>
    /// <remarks>
    /// Right-associative means that...
    /// 
    ///     a
    ///       ? b
    ///       : d
    ///             ? e
    ///             : f
    ///
    /// ... is parsed as...
    ///
    /// 
    ///     a
    ///       ? b
    ///       : (d
    ///             ? e
    ///             : f)
    ///
    /// ... and not as...
    /// 
    ///     (a
    ///       ? b
    ///       : d)
    ///             ? e
    ///             : f
    ///
    /// </remarks>
    /// <remarks>
    /// The outer monadic action parses the first separator (e.g. `?`) and
    /// returns an action (of type: `m (a -> a -> a -> a)`) that parses the
    /// second separator (e.g. `:`).
    /// </remarks>
    /// <param name="Operation"></param>
    public record TernR(K<M, K<M, Func<A, A, A, A>>> Operation) : Operator<M, A>;

}

