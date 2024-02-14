using System;
using LanguageExt.HKT;

namespace LanguageExt;

public partial class OptionT<M>
    where M : MonadIO<M>
{
    public static OptionT<M, B> bind<A, B>(OptionT<M, A> ma, Func<A, OptionT<M, B>> f) => 
        ma.As().Bind(f);

    public static OptionT<M, B> map<A, B>(Func<A, B> f, OptionT<M, A> ma) => 
        ma.As().Map(f);

    public static OptionT<M, A> Pure<A>(A value) => 
        OptionT<M, A>.Some(value);

    public static OptionT<M, B> apply<A, B>(OptionT<M, Func<A, B>> mf, OptionT<M, A> ma) => 
        mf.As().Bind(ma.As().Map);

    public static OptionT<M, B> action<A, B>(OptionT<M, A> ma, OptionT<M, B> mb) =>
        ma.As().Bind(_ => mb);

    public static OptionT<M, A> lift<A>(Option<A> ma) => 
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<A>(K<M, A> ma) => 
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> liftIO<A>(IO<A> ma) => 
        OptionT<M, A>.Lift(M.LiftIO(ma));
}
