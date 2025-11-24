using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Type class for inputs that can also be used for error reporting.
/// </summary>
/// <remarks>
/// NOTE that both virtual methods have default implementations that call each other.
/// You must implement at least one of them otherwise you will get a stack overflow.
/// </remarks>
/// <typeparam name="S">Stream type</typeparam>
/// <typeparam name="T">Stream token type</typeparam>
public interface TraversableTokenStream<S, T> : TokenStream<S, T>
    where S : TraversableTokenStream<S, T>
{
    /// <summary>
    /// Given an offset @o@ and initial 'PosState', adjust the state in such
    /// a way that it starts at the offset.
    /// </summary>
    /// <param name="offset">Offset to reach</param>
    /// <param name="pst">Initial `PosState` to use</param>
    /// <returns>
    /// <para>
    /// Reached source position and updated state. It returns two values:
    /// </para>
    /// <para>
    /// * `Option string` representing the line on which the given `offset`
    ///   is located. It can be omitted (i.e. `None`); in that case
    ///   error reporting functions will not show offending lines. If
    ///   returned, the line should satisfy a number of conditions that are
    ///   described below.
    ///  * The updated `PosState` which can be in turn used to locate
    ///   another offset `offset'` given that `offset' >= offset`.
    /// </para>
    /// <para>
    /// The `string` representing the offending line in the input stream should
    /// satisfy the following:
    /// </para>
    /// <para>
    ///  * It should adequately represent the location of a token at the offset
    ///   of interest, that is, a character at 'sourceColumn' of the returned
    ///   `SourcePos` should correspond to the token at the `offset`.
    ///  * It should not include the newline at the end.
    ///  * It should not be empty, if the line happens to be empty, it should be
    ///   replaced with the string "&lt;empty line&gt;".
    ///  * Tab characters should be replaced by an appropriate number of spaces,
    ///   which is determined by the `stateTabWidth` field of `PosState`.
    /// </para>
    /// </returns>
    public static virtual (Option<LineText> Line, PosState<S> Updated) ReachOffset(int offset, PosState<S> pst) =>
        (None, S.ReachOffsetNoLine(offset, pst));

    /// <summary>
    /// A version of `ReachOffset` that may be faster because it doesn't need
    /// to fetch the line at which the given offset is located.
    /// </summary>
    /// <param name="offset">Offset to reach</param>
    /// <param name="pst">Initial `PosState` to use</param>
    /// <returns>
    /// <para>
    /// Reached source position and updated state. It returns two values:
    /// </para>
    /// <para>
    /// * `Option string` representing the line on which the given `offset`
    ///   is located. It can be omitted (i.e. `None`); in that case
    ///   error reporting functions will not show offending lines. If
    ///   returned, the line should satisfy a number of conditions that are
    ///   described below.
    ///  * The updated `PosState` which can be in turn used to locate
    ///   another offset `offset'` given that `offset' >= offset`.
    /// </para>
    /// <para>
    /// The `string` representing the offending line in the input stream should
    /// satisfy the following:
    /// </para>
    /// <para>
    ///  * It should adequately represent the location of a token at the offset
    ///   of interest, that is, a character at 'sourceColumn' of the returned
    ///   `SourcePos` should correspond to the token at the `offset`.
    ///  * It should not include the newline at the end.
    ///  * It should not be empty, if the line happens to be empty, it should be
    ///   replaced with the string "&lt;empty line&gt;".
    ///  * Tab characters should be replaced by an appropriate number of spaces,
    ///   which is determined by the `stateTabWidth` field of `PosState`.
    /// </para>
    /// </returns>
    public static virtual PosState<S> ReachOffsetNoLine(int offset, PosState<S> pst) =>
        S.ReachOffset(offset, pst).Updated;
}
