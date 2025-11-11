using LanguageExt;
using LanguageExt.Traits;

namespace CardGame;

public record Game<A>(StateT<GameState, OptionT<IO>, A> runGame) : K<Game, A>
{
    public static Game<A> Pure(A x) =>
        new(StateT<GameState, OptionT<IO>, A>.Pure(x));
    
    public static Game<A> None =>
        new(StateT<GameState, OptionT<IO>, A>.Lift(OptionT.None<IO, A>()));
    
    public static Game<A> Lift(Option<A> mx) =>
        new(StateT<GameState, OptionT<IO>, A>.Lift(OptionT.lift<IO, A>(mx)));
    
    public static Game<A> LiftIO(IO<A> mx) =>
        new(StateT<GameState, OptionT<IO>, A>.LiftIO(mx));
    
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
        Pure(ma.Value);

    public static implicit operator Game<A>(IO<A> ma) =>
        Game.liftIO(ma);

    public static implicit operator Game<A>(Option<A> ma) =>
        Game.lift(ma);

    public static Game<A> operator >>(Game<A> ma, K<Game, A> mb) =>
        ma.Bind(_ => mb);

    public static Game<A> operator >>(Game<A> ma, K<Game, Unit> mb) =>
        ma.Bind(x => mb.Map(_ => x));

    public static Game<A> operator >>(Game<A> ma, K<IO, A> mb) =>
        ma.Bind(_ => mb.As());

    public static Game<A> operator >>(Game<A> ma, K<IO, Unit> mb) =>
        ma.Bind(x => mb.As().Map(_ => x));
}
