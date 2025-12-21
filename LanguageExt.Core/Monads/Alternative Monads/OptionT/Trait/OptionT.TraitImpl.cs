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

    static K<OptionT<M>, B> Monad<OptionT<M>>.Recur<A, B>(A value, Func<A, K<OptionT<M>, Next<A, B>>> f) => 
        new OptionT<M, B>(
            M.Recur<A, Option<B>>(
                value,
                a => f(a).As()
                         .runOption
                         .Map(e => e switch
                                   {
                                       { IsNone: true }                            => Next.Done<A, Option<B>>(default), 
                                       { IsSome: true, Value: { IsDone: true } n } => Next.Done<A, Option<B>>(n.DoneValue), 
                                       { IsSome: true, Value: { IsCont: true } n } => Next.Cont<A, Option<B>>(n.ContValue),
                                       _                                           => throw new NotSupportedException()
                                   })));

    static K<OptionT<M>, B> Functor<OptionT<M>>.Map<A, B>(Func<A, B> f, K<OptionT<M>, A> ma) => 
        ma.As().Map(f);

    static K<OptionT<M>, A> Applicative<OptionT<M>>.Pure<A>(A value) => 
        OptionT.Some<M, A>(value);

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Apply<A, B>(K<OptionT<M>, Func<A, B>> mf, K<OptionT<M>, A> ma) =>
        mf.As().Bind(x => ma.As().Map(x));

    static K<OptionT<M>, B> Applicative<OptionT<M>>.Apply<A, B>(K<OptionT<M>, Func<A, B>> mf, Memo<OptionT<M>, A> ma) =>
        mf.As().Bind(x => ma.Value.As().Map(x));

    static K<OptionT<M>, A> MonadT<OptionT<M>, M>.Lift<A>(K<M, A> ma) => 
        OptionT.lift(ma);
        
    static K<OptionT<M>, A> MonadIO<OptionT<M>>.LiftIO<A>(IO<A> ma) => 
        OptionT.lift(M.LiftIOMaybe(ma));

    static K<OptionT<M>, A> Alternative<OptionT<M>>.Empty<A>() =>
        OptionT<M, A>.None;
 
    static K<OptionT<M>, A> Choice<OptionT<M>>.Choose<A>(K<OptionT<M>, A> ma, K<OptionT<M>, A> mb) =>
        new OptionT<M, A>(
            M.Bind(ma.As().runOption,
                   ea => ea.IsSome
                             ? M.Pure(ea)
                             : mb.As().runOption));

    static K<OptionT<M>, A> Choice<OptionT<M>>.Choose<A>(K<OptionT<M>, A> ma, Memo<OptionT<M>, A> mb) => 
        new OptionT<M, A>(
            M.Bind(ma.As().runOption,
                   ea => ea.IsSome
                             ? M.Pure(ea)
                             : mb.Value.As().runOption));

    static K<OptionT<M>, A> Fallible<Unit, OptionT<M>>.Fail<A>(Unit error) =>
        OptionT.None<M, A>();

    static K<OptionT<M>, A> Fallible<Unit, OptionT<M>>.Catch<A>(
        K<OptionT<M>, A> fa, 
        Func<Unit, bool> Predicate, 
        Func<Unit, K<OptionT<M>, A>> Fail) => 
        fa.As().BindNone(() => Predicate(default) ? Fail(default).As() : OptionT<M, A>.None);
}
