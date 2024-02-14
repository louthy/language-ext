using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `OptionT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class OptionT<M> : MonadT<OptionT<M>, M>
    where M : Monad<M>
{
    static K<OptionT<M>, B> Monad<OptionT<M>>.Bind<A, B>(K<OptionT<M>, A> ma, Func<A, K<OptionT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<OptionT<M>, B> Functor<OptionT<M>>.Map<A, B>(Func<A, B> f, K<OptionT<M>, A> ma) => 
        ma.As().Map(f);

    static K<OptionT<M>, A> Applicative<OptionT<M>>.Pure<A>(A value) => 
        OptionT<M, A>.Some(value);

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Apply<A, B>(K<OptionT<M>, Func<A, B>> mf, K<OptionT<M>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Action<A, B>(K<OptionT<M>, A> ma, K<OptionT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<OptionT<M>, A> MonadT<OptionT<M>, M>.Lift<A>(K<M, A> ma) => 
        OptionT<M, A>.Lift(ma);
}

/// <summary>
/// Trait implementation for `OptionT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public class OptionIO<M> : OptionT<M>, MonadIO<OptionIO<M>>
    where M : MonadIO<M>
{
    static K<OptionIO<M>, A> MonadIO<OptionIO<M>>.LiftIO<A>(IO<A> ma) =>
        OptionT.liftIO<M, A>(ma);

    public static K<OptionIO<M>, B> Bind<A, B>(K<OptionIO<M>, A> ma, Func<A, K<OptionIO<M>, B>> f) => 
        throw new NotImplementedException();

    public static K<OptionIO<M>, B> Map<A, B>(Func<A, B> f, K<OptionIO<M>, A> ma) => 
        throw new NotImplementedException();

    public static K<OptionIO<M>, A> Pure<A>(A value) => 
        throw new NotImplementedException();

    public static K<OptionIO<M>, B> Apply<A, B>(K<OptionIO<M>, Func<A, B>> mf, K<OptionIO<M>, A> ma) => 
        throw new NotImplementedException();

    public static K<OptionIO<M>, B> Action<A, B>(K<OptionIO<M>, A> ma, K<OptionIO<M>, B> mb) => 
        throw new NotImplementedException();

    public static K<OptionIO<M>, A> LiftIO<A>(IO<A> ma) => 
        throw new NotImplementedException();
}
