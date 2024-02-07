using LanguageExt.HKT;

namespace LanguageExt;

//public delegate Fin<A> Reader<in Env, A>(Env env);

public static class Reader
{
    public static Reader<Env, A> As<Env, A>(this KArrow<MReaderT<Env, MIdentity>, Env, MIdentity, A> ma) =>
        (Reader<Env, A>)ma;
}

public record Reader<Env, A>(Transducer<Env, KStar<MIdentity, A>> runReaderT)
    : ReaderT<Env, MIdentity, A>(runReaderT);

/*
public record Reader<Env, A>(Transducer<Env, KStar<MIdentity, A>> runReader) : KArr<MReader<Env>, Env, MIdentity, A> 
{
    public static Reader<Env, Env> Ask =>
        MonadReaderT.ask<MReader<Env>, Env, MIdentity>().As();
    
    public static Reader<Env, A> Pure(A value) =>
        MonadReaderT.pure<MReader<Env>, Env, MIdentity, A>(value).As();
    
    public static Reader<Env, A> Lift(KStar<MIdentity, A> monad) => 
        MonadReaderT.lift<MReader<Env>, Env, MIdentity, A>(monad).As();
    
    public Reader<Env, B> Map<B>(Transducer<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public Reader<Env, B> Map<B>(Func<A, B> f) =>
        MonadReaderT.map(this, f).As();

    public Reader<Env, B> Bind<B>(Transducer<A, Reader<Env, B>> f) =>
        MonadReaderT.bind<
            MReader<Env>, 
            Reader<Env, B>,
            Env, MIdentity, A, B>(this, f);

    public Reader<Env, B> Bind<B>(Func<A, Reader<Env, B>> f) =>
        MonadReaderT.bind<
            MReader<Env>, 
            Reader<Env, B>,
            Env, MIdentity, A, B>(this, f);

    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public Reader<Env, C> SelectMany<B, C>(Func<A, KStar<MIdentity, B>> bind, Func<A, B, C> project) =>
        Bind(x => MReader<Env>.Lift(bind(x)).Map(y => project(x, y)));
    
    public static implicit operator Reader<Env, A>(Transducer<Env, KStar<MIdentity, A>> runReader) =>
        new (runReader);

    public Transducer<Env, KStar<MIdentity, A>> Morphism =>
        runReader.Morphism;
}
*/
