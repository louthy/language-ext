using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `OptionT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class OptionT<M> : 
    MonadT<OptionT<M>, M>,
    Alternative<OptionT<M>>,
    Fallible<Unit, OptionT<M>>,
    MonadIO<OptionT<M>>
    where M : Monad<M>
{
    static K<OptionT<M>, B> Monad<OptionT<M>>.Bind<A, B>(K<OptionT<M>, A> ma, Func<A, K<OptionT<M>, B>> f) => 
        ma.As().Bind(f);

    static K<OptionT<M>, B> Functor<OptionT<M>>.Map<A, B>(Func<A, B> f, K<OptionT<M>, A> ma) => 
        ma.As().Map(f);

    static K<OptionT<M>, A> Applicative<OptionT<M>>.Pure<A>(A value) => 
        OptionT<M, A>.Some(value);

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Apply<A, B>(K<OptionT<M>, Func<A, B>> mf, K<OptionT<M>, A> ma) =>
        mf.As().Bind(x => ma.As().Map(x));

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Action<A, B>(K<OptionT<M>, A> ma, K<OptionT<M>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<OptionT<M>, A> MonadT<OptionT<M>, M>.Lift<A>(K<M, A> ma) => 
        OptionT<M, A>.Lift(ma);
        
    static K<OptionT<M>, A> Maybe.MonadIO<OptionT<M>>.LiftIO<A>(IO<A> ma) => 
        OptionT<M, A>.Lift(M.LiftIO(ma));

    static K<OptionT<M>, A> MonoidK<OptionT<M>>.Empty<A>() =>
        OptionT<M, A>.None;
 
    static K<OptionT<M>, A> Choice<OptionT<M>>.Choose<A>(K<OptionT<M>, A> ma, K<OptionT<M>, A> mb) =>
        new OptionT<M, A>(
            M.Bind(ma.As().runOption,
                   ea => ea.IsSome
                             ? M.Pure(ea)
                             : mb.As().runOption));

    static K<OptionT<M>, A> Choice<OptionT<M>>.Choose<A>(K<OptionT<M>, A> ma, Func<K<OptionT<M>, A>> mb) => 
        new OptionT<M, A>(
            M.Bind(ma.As().runOption,
                   ea => ea.IsSome
                             ? M.Pure(ea)
                             : mb().As().runOption));

    static K<OptionT<M>, A> SemigroupK<OptionT<M>>.Combine<A>(K<OptionT<M>, A> lhs, K<OptionT<M>, A> rhs) =>
        lhs.Choose(rhs);

    static K<OptionT<M>, A> Fallible<Unit, OptionT<M>>.Fail<A>(Unit error) =>
        None<A>();

    static K<OptionT<M>, A> Fallible<Unit, OptionT<M>>.Catch<A>(
        K<OptionT<M>, A> fa, 
        Func<Unit, bool> Predicate, 
        Func<Unit, K<OptionT<M>, A>> Fail) => 
        fa.As().BindNone(() => Predicate(default) ? Fail(default).As() : OptionT<M, A>.None);
}
