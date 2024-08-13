using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
public record IterableT<M, A>(K<M, MList<A>> runListT) : 
    K<IterableT<M>, A>,
    Monoid<IterableT<M, A>> 
    where M : Monad<M>
{
    public static IterableT<M, A> Lift(Seq<A> list) =>
        list switch
        {
            []          => Empty,
            var (x, xs) => new IterableT<M, A>(M.Pure<MList<A>>(new MCons<M, A>(x, Lift(xs).runListT))) 
        };

    public static IterableT<M, A> Lift(K<M, A> ma) =>
        new (ma.Map(a => MList<A>.Cons(a, M.Pure(MList<A>.Nil))));

    public static IterableT<M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    /// <summary>
    /// Concatenate sequences
    /// </summary>
    public IterableT<M, A> Combine(IterableT<M, A> rhs) =>
        new(from mxs in runListT
            from mys in rhs.runListT
            select (mxs, mys) switch
                   {
                       (MNil<A> l, MNil<A>) => l,
                       (MNil<A>, var r)     => r,
                       (var l, MNil<A>)     => l,
                       (MCons<M, A> (var lh, var lt), MCons<M, A> (var rh, var rt)) =>
                           throw new NotImplementedException()
                   });
    
    /// <summary>
    /// Empty sequence
    /// </summary>
    public static IterableT<M, A> Empty { get; } = new (M.Pure<MList<A>>(new MNil<A>()));

    public IterableT<M, A> Filter(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public IterableT<M, A> Where(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public IterableT<M, B> Map<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();

    public IterableT<M, B> Select<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public IterableT<M, B> Bind<B>(Func<A, IterableT<M, B>> f) =>
        this.Kind().Bind(f).As();
    
    public IterableT<M, B> Bind<B>(Func<A, K<IterableT<M>, B>> f) =>
        this.Kind().Bind(f).As();
    
    public IterableT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        this.Kind().Bind(f).As();
    
    public IterableT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        this.Kind().Bind(f).As();
    
    public IterableT<M, B> Bind<B>(Func<A, K<IO, B>> f) =>
        this.Kind().Bind(f).As();

    public IterableT<M, C> SelectMany<B, C>(Func<A, IterableT<M, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public IterableT<M, C> SelectMany<B, C>(Func<A, K<IterableT<M>, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public IterableT<M, C> SelectMany<B, C>(Func<A, Pure<M, B>> bind, Func<A, B, C> project) =>
        this.Kind().Map(x => project(x, bind(x).Value)).As();

    public IterableT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public IterableT<M, C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();
}

public abstract record MList<A>
{
    public abstract MList<B> Map<B>(Func<A, B> f);

    public static MList<A> Nil = 
        MNil<A>.Default;
    
    public static MList<A> Cons<M>(A head, K<M, MList<A>> tail) 
        where M : Monad<M> => 
        new MCons<M, A>(head, tail);

    public K<M, MList<A>> Append<M>(K<M, MList<A>> ys)
        where M : Monad<M> =>
        this switch
        {
            MNil<A>                    => ys,
            MCons<M, A> (var h, var t) => M.Pure(Cons(h, t.Append(ys))),
            _                          => throw new NotSupportedException()
        };
}

public record MNil<A> : MList<A>
{
    public static readonly MList<A> Default = new MNil<A>();

    public override MList<B> Map<B>(Func<A, B> f) =>
        MNil<B>.Default;
}

public record MCons<M, A>(A Head, K<M, MList<A>> Tail) : MList<A>
    where M : Monad<M>
{
    public override MList<B> Map<B>(Func<A, B> f) => 
        new MCons<M, B>(f(Head), Tail.Map(l => l.Map(f)));
}

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
        mma.As().runListT.Map(
            ma => ma switch
                  {
                      MNil<A> =>
                          Option<(A Head, IterableT<M, A> Tail)>.None,

                      MCons<M, A>(var h, var t) =>
                          Option<(A Head, IterableT<M, A> Tail)>.Some((h, new IterableT<M, A>(t))),
                      
                      _ => throw new NotSupportedException()
                  });

    public static IterableT<M, A> Flatten<M, A>(this K<IterableT<M>, IterableT<M, A>> mma)
        where M : Monad<M> =>
        new (mma.As().runListT.Map(ml => ml.Map(ma => ma.runListT)).Flatten());

    public static IterableT<M, A> Flatten<M, A>(this K<IterableT<M>, K<IterableT<M>, A>> mma)
        where M : Monad<M> =>
        new (mma.As().runListT.Map(ml => ml.Map(ma => ma.As().runListT)).Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this K<M, MList<K<M, MList<A>>>> mma)
        where M : Monad<M> =>
        mma.Bind(la => la.Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this MList<K<M, MList<A>>> mma)
        where M : Monad<M> =>
        mma switch
        {
            MNil<K<M, MList<A>>>                    => M.Pure(MNil<A>.Default),
            MCons<M, K<M, MList<A>>> (var h, var t) => h.Append(t.Flatten()),
            _                                       => throw new NotSupportedException()
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

/// <summary>
/// IterableT module
/// </summary>
public static class IterableT
{
    public static IterableT<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        new (M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil))));
    
    public static IterableT<M, A> lift<M, A>(IEnumerable<A> items) 
        where M : Monad<M> =>
        lift<M, A>(toSeq(items));

    public static IterableT<M, A> lift<M, A>(Seq<A> items)
        where M : Monad<M> =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        IterableT<M, A>.LiftIO(ma);
}

/// <summary>
/// IterableT trait implementations
/// </summary>
public class IterableT<M> : 
    MonadT<IterableT<M>, M>, 
    Traversable<IterableT<M>>, 
    Alternative<IterableT<M>>
    where M : Monad<M>
{
    public static IterableT<M, A> pure<A>(A value) =>
        new (M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil))));
    
    public static IterableT<M, A> lift<A>(IEnumerable<A> items) =>
        lift(toSeq(items));

    public static IterableT<M, A> lift<A>(Seq<A> items) =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> liftIO<A>(IO<A> ma) =>
        IterableT<M, A>.LiftIO(ma);
    
    static K<IterableT<M>, B> Monad<IterableT<M>>.Bind<A, B>(
        K<IterableT<M>, A> mma,
        Func<A, K<IterableT<M>, B>> f) =>
        mma.As().Map(f).Flatten();

    static K<IterableT<M>, B> Functor<IterableT<M>>.Map<A, B>(
        Func<A, B> f,
        K<IterableT<M>, A> mma) =>
        new IterableT<M, B>(mma.As().runListT.Map(ma => ma.Map(f)));

    static K<IterableT<M>, A> Applicative<IterableT<M>>.Pure<A>(A value) => 
        new IterableT<M, A>(M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil))));

    static K<IterableT<M>, B> Applicative<IterableT<M>>.Apply<A, B>(
        K<IterableT<M>, Func<A, B>> mf,
        K<IterableT<M>, A> ma) =>
        mf.Bind(f => ma.Map(f));

    static K<IterableT<M>, A> MonadT<IterableT<M>, M>.Lift<A>(
        K<M, A> ma) =>
        new IterableT<M, A>(ma.Map(a => MList<A>.Cons(a, M.Pure(MList<A>.Nil))));

    static S Foldable<IterableT<M>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static S Foldable<IterableT<M>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static K<F, K<IterableT<M>, B>> Traversable<IterableT<M>>.Traverse<F, A, B>(
        Func<A, K<F, B>> f, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static K<IterableT<M>, A> SemigroupK<IterableT<M>>.Combine<A>(K<IterableT<M>, A> lhs, K<IterableT<M>, A> rhs) => 
        throw new NotImplementedException();

    static K<IterableT<M>, A> MonoidK<IterableT<M>>.Empty<A>() => 
        new IterableT<M, A>(M.Pure(MList<A>.Nil));

    static K<IterableT<M>, B> MonadIO<IterableT<M>>.MapIO<A, B>(K<IterableT<M>, A> ma, Func<IO<A>, IO<B>> f) =>
        throw new NotImplementedException();

    static K<IterableT<M>, A> MonadIO<IterableT<M>>.LiftIO<A>(IO<A> ma) =>
        new IterableT<M, A>(M.LiftIO(ma).Map(a => MList<A>.Cons(a, M.Pure(MList<A>.Nil))));
}
