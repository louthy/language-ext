using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity module
/// </summary>
public class IdentityT<M> : MonadT<IdentityT<M>, M>, Alternative<IdentityT<M>>
    where M : Monad<M>, Alternative<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Module
    //
    
    public static K<IdentityT<M>, A> Pure<A>(A value) =>
        IdentityT<M, A>.Pure(value);
    
    public static IdentityT<M, B> bind<A, B>(IdentityT<M, A> ma, Func<A, IdentityT<M, B>> f) =>
        ma.As().Bind(f);

    public static IdentityT<M, B> map<A, B>(Func<A, B> f, IdentityT<M, A> ma) => 
        ma.As().Map(f);

    public static IdentityT<M, B> apply<A, B>(IdentityT<M, Func<A, B>> mf, IdentityT<M, A> ma) =>
        mf.Bind(ma.Map);

    public static IdentityT<M, B> action<A, B>(IdentityT<M, A> ma, IdentityT<M, B> mb) =>
        ma.Bind(_ => mb);
 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //
    
    static K<IdentityT<M>, B> Monad<IdentityT<M>>.Bind<A, B>(K<IdentityT<M>, A> ma, Func<A, K<IdentityT<M>, B>> f) =>
        ma.As().Bind(f);

    static K<IdentityT<M>, B> Functor<IdentityT<M>>.Map<A, B>(Func<A, B> f, K<IdentityT<M>, A> ma) => 
        ma.As().Map(f);

    static K<IdentityT<M>, B> Applicative<IdentityT<M>>.Apply<A, B>(K<IdentityT<M>, Func<A, B>> mf, K<IdentityT<M>, A> ma) =>
        mf.As().Bind(f => ma.As().Map(f));

    static K<IdentityT<M>, B> Applicative<IdentityT<M>>.Action<A, B>(K<IdentityT<M>, A> ma, K<IdentityT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    public static K<IdentityT<M>, A> Lift<A>(K<M, A> ma) =>
        IdentityT<M, A>.Lift(ma);

    public static K<IdentityT<M>, A> LiftIO<A>(IO<A> ma) => 
        IdentityT<M, A>.Lift(M.LiftIO(ma));

    static K<IdentityT<M>, B> Monad<IdentityT<M>>.WithRunInIO<A, B>(
        Func<Func<K<IdentityT<M>, A>, IO<A>>, IO<B>> inner) =>
        new IdentityT<M, B>(
            M.WithRunInIO<A, B>(
                run =>
                    inner(ma => run(ma.As().Value))));

    static K<IdentityT<M>, A> Alternative<IdentityT<M>>.Empty<A>() =>
        new IdentityT<M, A>(M.Empty<A>());

    static K<IdentityT<M>, A> Alternative<IdentityT<M>>.Or<A>(K<IdentityT<M>, A> ma, K<IdentityT<M>, A> mb) =>
        new IdentityT<M, A>(M.Or(ma.As().Value, mb.As().Value));
}
