using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `ChronicleT` trait implementations
/// </summary>
/// <typeparam name="Ch">Chronicle type (a semigroup)</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
public class ChronicleT<Ch, M> : 
    MonadT<ChronicleT<Ch, M>, M>,
    MonadIO<ChronicleT<Ch, M>>,
    Fallible<Ch, ChronicleT<Ch, M>>,
    Choice<ChronicleT<Ch, M>> 
    where M : Monad<M>
    where Ch : Semigroup<Ch>
{
    static K<ChronicleT<Ch, M>, B> Functor<ChronicleT<Ch, M>>.Map<A, B>(
        Func<A, B> f,
        K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Map(f);

    static K<ChronicleT<Ch, M>, A> Applicative<ChronicleT<Ch, M>>.Pure<A>(A value) =>
        ChronicleT<Ch, M, A>.Dictate(value);

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
        ChronicleT.confess<Ch, M, A>(error);

    static K<ChronicleT<Ch, M>, A> Fallible<Ch, ChronicleT<Ch, M>>.Catch<A>(
        K<ChronicleT<Ch, M>, A> fa,
        Func<Ch, bool> Predicate,
        Func<Ch, K<ChronicleT<Ch, M>, A>> Fail) =>
        fa.As().Catch(Predicate, Fail);

    public static K<ChronicleT<Ch, M>, A> Combine<A>(K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, A> rhs) => 
        lhs.As().Choose(rhs);

    public static K<ChronicleT<Ch, M>, A> Choose<A>(K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, A> rhs) => 
        lhs.As().Choose(rhs);

    public static K<ChronicleT<Ch, M>, A> Choose<A>(K<ChronicleT<Ch, M>, A> lhs, Func<K<ChronicleT<Ch, M>, A>> rhs) => 
        lhs.As().Choose(rhs);
}
