using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// IterableT extensions
/// </summary>
public static class IterableTExtensions
{
    public static IterableT<M, A> As<M, A>(this K<IterableT<M>, A> ma)
        where M : Monad<M> =>
        (IterableT<M, A>)ma;

    public static K<M, Option<(A Head, IterableT<M, A> Tail)>> Run<M, A>(this IterableT<M, A> mma)
        where M : Monad<M> =>
        mma.As().Run();

    public static IterableT<M, A> Flatten<M, A>(this K<IterableT<M>, IterableT<M, A>> mma)
        where M : Monad<M> =>
        new IterableMainT<M, A>(mma.As().runListT.Map(ml => ml.Map(ma => ma.runListT)).Flatten());

    public static IterableT<M, A> Flatten<M, A>(this K<IterableT<M>, K<IterableT<M>, A>> mma)
        where M : Monad<M> =>
        new IterableMainT<M, A>(mma.As().runListT.Map(ml => ml.Map(ma => ma.As().runListT)).Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this K<M, MList<K<M, MList<A>>>> mma)
        where M : Monad<M> =>
        mma.Bind(la => la.Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this MList<K<M, MList<A>>> mma)
        where M : Monad<M> =>
        mma switch
        {
            MNil<K<M, MList<A>>>                     => M.Pure(MNil<A>.Default),
            MCons<M, K<M, MList<A>>> (var h, var t)  => h.Append(t.Flatten()),
            MIter<M, K<M, MList<A>>> (var h, _) iter => h.Append(iter.TailToMList().Flatten()),
            _                                        => throw new NotSupportedException()
        };

    public static K<M, MList<A>> Append<M, A>(this K<M, MList<A>> xs, K<M, MList<A>> ys)
        where M : Monad<M> =>
        xs.Bind(x => x.Append(ys));

    public static IterableT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, IterableT<M, B>> f)
        where M : Monad<M> =>
        IterableT<M>.pure(ma.Value).Bind(f);
    
    public static IterableT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, K<IterableT<M>, B>> f)
        where M : Monad<M> =>
        IterableT<M>.pure(ma.Value).Bind(f);

    public static IterableT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, IterableT<M, B>> f)
        where M : Monad<M> =>
        IterableT<M>.liftIO(ma).Bind(f);
    
    public static IterableT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, K<IterableT<M>, B>> f)
        where M : Monad<M> =>
        IterableT<M>.liftIO(ma).Bind(f);

    public static IterableT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma, 
        Func<A, IterableT<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        IterableT<M>.pure(ma.Value).SelectMany(bind, project);

    public static IterableT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma, 
        Func<A, K<IterableT<M>, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        IterableT<M>.pure(ma.Value).SelectMany(bind, project);

    public static IterableT<M, C> SelectMany<M, A, B, C>(
        this IO<A> ma, 
        Func<A, IterableT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        IterableT<M>.liftIO(ma).SelectMany(bind, project);
    
    public static IterableT<M, C> SelectMany<M, A, B, C>(
        this IO<A> ma, 
        Func<A, K<IterableT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        IterableT<M>.liftIO(ma).SelectMany(bind, project);
}
