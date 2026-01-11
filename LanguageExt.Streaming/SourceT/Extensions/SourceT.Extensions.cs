using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    [Pure]
    public static SourceT<M, A> As<M, A>(this K<SourceT<M>, A> ma) 
        where M : MonadIO<M> =>
        (SourceT<M, A>)ma;
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Channel<A> items)
        where M : MonadIO<M>, Fallible<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this Channel<K<M, A>> items)
        where M : MonadIO<M>, Fallible<M> =>
        SourceT.liftM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Source<A> items)
        where M : MonadIO<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this Source<K<M, A>> items)
        where M : MonadIO<M> =>
        SourceT.liftM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Iterable<A> items)
        where M : MonadIO<M> =>
        SourceT.liftIterable<Iterable, M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this Iterable<K<M, A>> items)
        where M : MonadIO<M> =>
        SourceT.liftIterableM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IterableNE<A> items)
        where M : MonadIO<M> =>
        SourceT.liftIterable<IterableNE, M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this IterableNE<K<M, A>> items)
        where M : MonadIO<M> =>
        SourceT.liftIterableM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Iterator<A> items)
        where M : MonadIO<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this Iterator<K<M, A>> items)
        where M : MonadIO<M> =>
        SourceT.liftM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IObservable<A> items)
        where M : MonadIO<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceM<M, A>(this IObservable<K<M, A>> items)
        where M : MonadIO<M> =>
        SourceT.liftM(items);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M> =>
        SourceT.liftIO<M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M> =>
        SourceT.pure<M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M> =>
        SourceT.liftM(ma).Bind(f);    
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this K<M, A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M> =>
        SourceT.liftM(ma).As().SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this IO<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M> =>
        SourceT.liftIO<M, A>(ma).As().SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this Pure<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M> =>
        bind(ma.Value).Map(y => project(ma.Value, y));
}
