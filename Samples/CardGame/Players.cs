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
    public static Game<Unit> with<A>(Game<Seq<Player>> playersM, Game<A> ma) =>
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
    public static Game<Unit> with<A>(Seq<Player> players, Game<A> ma) =>
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
    public static Game<Seq<A>> map<A>(Game<Seq<Player>> playersM, Game<A> ma) =>
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
    public static Game<Seq<A>> map<A>(Seq<Player> players, Game<A> ma) =>
        players.Traverse(p => Player.with(p, ma))
               .As();
}
