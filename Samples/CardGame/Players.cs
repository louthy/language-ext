using LanguageExt;
using static LanguageExt.Prelude;
namespace CardGame;

/// <summary>
/// Bulk management of players
/// </summary>
public static class Players
{
    /// <summary>
    /// For each player in a collection of players:
    ///
    ///     * Make into the current player
    ///     * Rub the  `ma` computation in that context
    ///     * Reset the current player
    /// </summary>
    /// <returns>
    /// Ignores the results
    /// </returns>
    public static GameM<Unit> with<A>(GameM<Seq<Player>> playersM, GameM<A> ma) =>
        playersM.Bind(ps => with(ps, ma))
                .Map(_ => unit);
    
    /// <summary>
    /// For each player in a collection of players:
    ///
    ///     * Make into the current player
    ///     * Rub the  `ma` computation in that context
    ///     * Reset the current player
    ///
    /// Return a sequence of results, one for each player
    /// </summary>
    public static GameM<Unit> with<A>(Seq<Player> players, GameM<A> ma) =>
        players.Traverse(p => Player.with(p, ma)).
                Map(_ => unit)
               .As();
    
    /// <summary>
    /// For each player in a collection of players:
    ///
    ///     * Make into the current player
    ///     * Rub the  `ma` computation in that context
    ///     * Reset the current player
    ///
    /// </summary>
    /// <returns>
    /// Return a sequence of results, one for each player
    /// </returns>
    public static GameM<Seq<A>> map<A>(GameM<Seq<Player>> playersM, GameM<A> ma) =>
        playersM.Bind(ps => map(ps, ma));
    
    /// <summary>
    /// For each player in a collection of players:
    ///
    ///     * Make into the current player
    ///     * Rub the  `ma` computation in that context
    ///     * Reset the current player
    ///
    /// </summary>
    /// <returns>
    /// Return a sequence of results, one for each player
    /// </returns>
    public static GameM<Seq<A>> map<A>(Seq<Player> players, GameM<A> ma) =>
        players.Traverse(p => Player.with(p, ma))
               .As();
}
