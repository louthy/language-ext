using System;
using System.Buffers;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `ChronicleT` trait implementations
/// </summary>
/// <typeparam name="Ch">Chronicle type (a semigroup)</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
public partial class ChronicleT<Ch, M> : 
    MonadT<ChronicleT<Ch, M>, M>,
    MonadIO<ChronicleT<Ch, M>>,
    Fallible<Ch, ChronicleT<Ch, M>>,
    Choice<ChronicleT<Ch, M>>,
    Chronicaler<ChronicleT<Ch, M>, Ch>
    where M : Monad<M>
{
    static K<ChronicleT<Ch, M>, B> Functor<ChronicleT<Ch, M>>.Map<A, B>(
        Func<A, B> f,
        K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Map(f);

    static K<ChronicleT<Ch, M>, A> Applicative<ChronicleT<Ch, M>>.Pure<A>(A value) =>
        ChronicleT.dictate<Ch, M, A>(value);

    static K<ChronicleT<Ch, M>, B> Applicative<ChronicleT<Ch, M>>.Apply<A, B>(
        K<ChronicleT<Ch, M>, Func<A, B>> mf,
        K<ChronicleT<Ch, M>, A> ma)
    {
        return new ChronicleT<Ch, M, B>(t => Applicative.lift(apply(t.Combine), mf.As().Run(t), ma.As().Run(t)));
        static Func<These<Ch, Func<A, B>>, These<Ch, A>, These<Ch, B>> apply(Func<Ch, Ch, Ch> combine) =>
            (mf, mx) =>
                mf.Apply(mx, combine);
    }

    static K<ChronicleT<Ch, M>, B> Applicative<ChronicleT<Ch, M>>.Apply<A, B>(
        K<ChronicleT<Ch, M>, Func<A, B>> mf,
        Memo<ChronicleT<Ch, M>, A> mma)
    {
        return new ChronicleT<Ch, M, B>(
            semi => Applicative.lift(
                apply(semi.Combine), 
                memoF(mf.As().Run(semi)),
                mma.Lower().Map(ma => ma.As().Run(semi)).Lift()));
        
        static Func<These<Ch, Func<A, B>>, These<Ch, A>, These<Ch, B>> apply(Func<Ch, Ch, Ch> combine) =>
            (mf, mx) =>
                mf.Apply(mx, combine);
        
    }

    static K<ChronicleT<Ch, M>, B> Monad<ChronicleT<Ch, M>>.Bind<A, B>(
        K<ChronicleT<Ch, M>, A> ma,
        Func<A, K<ChronicleT<Ch, M>, B>> f) =>
        ma.As().Bind(f);

    static K<ChronicleT<Ch, M>, A> MonadT<ChronicleT<Ch, M>, M>.Lift<A>(K<M, A> ma) =>
        ChronicleT.lift<Ch, M, A>(ma);

    static K<ChronicleT<Ch, M>, A> MonadIO<ChronicleT<Ch, M>>.LiftIO<A>(IO<A> ma) => 
        ChronicleT.liftIO<Ch, M, A>(ma);

    static K<ChronicleT<Ch, M>, A> Fallible<Ch, ChronicleT<Ch, M>>.Fail<A>(Ch error) => 
        ChronicleT.confess<Ch, M, A>(error);

    static K<ChronicleT<Ch, M>, A> Fallible<Ch, ChronicleT<Ch, M>>.Catch<A>(
        K<ChronicleT<Ch, M>, A> fa,
        Func<Ch, bool> Predicate,
        Func<Ch, K<ChronicleT<Ch, M>, A>> Fail) =>
        fa.As().Catch(Predicate, Fail);

    static K<ChronicleT<Ch, M>, A> Choice<ChronicleT<Ch, M>>.Choose<A>(K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, A> rhs) => 
        lhs.As().Choose(rhs);

    static K<ChronicleT<Ch, M>, A> Choice<ChronicleT<Ch, M>>.Choose<A>(K<ChronicleT<Ch, M>, A> lhs, Memo<ChronicleT<Ch, M>, A> rhs) => 
        lhs.As().Choose(rhs);

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Dictate<A>(A value) => 
        ChronicleT.dictate<Ch, M, A>(value);

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Confess<A>(Ch c) => 
        ChronicleT.confess<Ch, M, A>(c);

    static K<ChronicleT<Ch, M>, Either<Ch, A>> Chronicaler<ChronicleT<Ch, M>, Ch>.Memento<A>(
        K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Memento();

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Absolve<A>(
        A defaultValue, 
        K<ChronicleT<Ch, M>, A> ma) => 
        ma.As().Absolve(defaultValue);

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Condemn<A>(K<ChronicleT<Ch, M>, A> ma) => 
        ma.As().Condemn();

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Censor<A>(
        Func<Ch, Ch> f,
        K<ChronicleT<Ch, M>, A> ma) => 
        ma.As().Censor(f);

    static K<ChronicleT<Ch, M>, A> Chronicaler<ChronicleT<Ch, M>, Ch>.Chronicle<A>(These<Ch, A> ma) => 
        ChronicleT.chronicle<Ch, M, A>(ma);
}
