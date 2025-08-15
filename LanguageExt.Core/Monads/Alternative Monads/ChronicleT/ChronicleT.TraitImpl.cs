using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class ChronicleT<Ch, M> : 
    MonadT<ChronicleT<Ch, M>, M>,
    MonadIO<ChronicleT<Ch, M>>,
    Fallible<Ch, ChronicleT<Ch, M>>
    where M : Monad<M>
    where Ch : Monoid<Ch>
{
    static K<ChronicleT<Ch, M>, B> Functor<ChronicleT<Ch, M>>.Map<A, B>(
        Func<A, B> f,
        K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Map(f);

    static K<ChronicleT<Ch, M>, A> Applicative<ChronicleT<Ch, M>>.Pure<A>(A value) =>
        ChronicleT<Ch, M, A>.That(value);

    static K<ChronicleT<Ch, M>, B> Applicative<ChronicleT<Ch, M>>.Apply<A, B>(
        K<ChronicleT<Ch, M>, Func<A, B>> mf,
        K<ChronicleT<Ch, M>, A> ma)
    {
        return new ChronicleT<Ch, M, B>(Applicative.lift(apply, mf.Run(), ma.Run()));
        static These<Ch, B> apply(These<Ch, Func<A, B>> mf, These<Ch, A> mx) => 
            mf.Apply(mx);
    }

    static K<ChronicleT<Ch, M>, B> Monad<ChronicleT<Ch, M>>.Bind<A, B>(
        K<ChronicleT<Ch, M>, A> ma,
        Func<A, K<ChronicleT<Ch, M>, B>> f) =>
        ma.As().Bind(f);

    static K<ChronicleT<Ch, M>, A> MonadT<ChronicleT<Ch, M>, M>.Lift<A>(K<M, A> ma) =>
        ChronicleT<Ch, M, A>.Lift(ma);

    static K<ChronicleT<Ch, M>, A> MonadIO<ChronicleT<Ch, M>>.LiftIO<A>(IO<A> ma) => 
        ChronicleT<Ch, M, A>.LiftIO(ma);

    static K<ChronicleT<Ch, M>, A> Fallible<Ch, ChronicleT<Ch, M>>.Fail<A>(Ch error) => 
        ChronicleT.This<Ch, M, A>(error);

    static K<ChronicleT<Ch, M>, A> Fallible<Ch, ChronicleT<Ch, M>>.Catch<A>(
        K<ChronicleT<Ch, M>, A> fa,
        Func<Ch, bool> Predicate,
        Func<Ch, K<ChronicleT<Ch, M>, A>> Fail) =>
        new ChronicleT<Ch, M, A>(
            fa.Run().Bind(these => these switch
                                   {
                                       This<Ch, A> (var c) =>
                                           Predicate(c)
                                               ? Fail(c).Run()
                                               : M.Pure(These.This<Ch, A>(c)),

                                       That<Ch, A> (var c) =>
                                           M.Pure(These.That<Ch, A>(c)),

                                       Pair<Ch, A> (var c1, var a1) =>
                                           Predicate(c1)
                                               ? Fail(c1).Run() switch
                                                 {
                                                     This<Ch, A> (var c2) =>
                                                         M.Pure(These.This<Ch, A>(c1 + c2)),

                                                     That<Ch, A> (var a2) =>
                                                         M.Pure(These.That<Ch, A>(a2)),

                                                     Pair<Ch, A> (var c2, var a2) =>
                                                         M.Pure(These.Pair(c1 + c2, a2)),

                                                     _ => throw new NotSupportedException()
                                                 }
                                               : M.Pure(These.Pair(c1, a1)),

                                       _ => throw new NotSupportedException()
                                   }));
}
