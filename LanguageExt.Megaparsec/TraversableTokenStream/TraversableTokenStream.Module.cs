using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static class TraversableTokenStream
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
    public static (Option<LineText> Line, PosState<S> Updated) reachOffset<S, T>(int offset, PosState<S> pst) 
        where S : TraversableTokenStream<S, T> =>
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
    public static PosState<S> reachOffsetNoLine<S, T>(int offset, PosState<S> pst) 
        where S : TraversableTokenStream<S, T> =>
        S.ReachOffset(offset, pst).Updated;
}
