using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public record GameM<A>(StateT<GameState, OptionT<IO>, A> runGame) : K<GameM, A>
{
    public GameM<B> Map<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public GameM<B> Select<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public GameM<B> Bind<B>(Func<A, K<GameM, B>> f) =>
        this.Kind().Bind(f).As();
    
    public GameM<B> Bind<B>(Func<A, IO<B>> f) =>
        this.Kind().Bind(f).As();

    public GameM<C> SelectMany<B, C>(Func<A, K<GameM, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public GameM<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(a => MonadIO.liftIO<GameM, B>(bind(a)), project);

    public GameM<C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        SelectMany(a => MonadIO.liftIO<GameM, B>(bind(a).As()), project);

    public GameM<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(a => project(a, bind(a).Value));

    public static implicit operator GameM<A>(Pure<A> ma) =>
        GameM.Pure(ma.Value).As();

    public static implicit operator GameM<A>(IO<A> ma) =>
        GameM.liftIO(ma);

    public static implicit operator GameM<A>(Option<A> ma) =>
        GameM.lift(ma);
}
