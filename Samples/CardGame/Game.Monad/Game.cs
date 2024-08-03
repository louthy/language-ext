using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public record Game<A>(StateT<GameState, OptionT<IO>, A> runGame) : K<Game, A>
{
    public Game<B> Map<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public Game<B> Select<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public Game<B> Bind<B>(Func<A, K<Game, B>> f) =>
        this.Kind().Bind(f).As();
    
    public Game<B> Bind<B>(Func<A, IO<B>> f) =>
        this.Kind().Bind(f).As();

    public Game<C> SelectMany<B, C>(Func<A, K<Game, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public Game<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(a => MonadIO.liftIO<Game, B>(bind(a)), project);

    public Game<C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        SelectMany(a => MonadIO.liftIO<Game, B>(bind(a).As()), project);

    public Game<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(a => project(a, bind(a).Value));

    public static implicit operator Game<A>(Pure<A> ma) =>
        Game.Pure(ma.Value).As();

    public static implicit operator Game<A>(IO<A> ma) =>
        Game.liftIO(ma);

    public static implicit operator Game<A>(Option<A> ma) =>
        Game.lift(ma);
}
