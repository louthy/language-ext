using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
public abstract record IterableT<M, A> : 
    K<IterableT<M>, A>,
    Monoid<IterableT<M, A>> 
    where M : Monad<M>
{
    public abstract K<M, MList<A>> runListT { get; }
    
    public K<M, Option<(A Head, IterableT<M, A> Tail)>> Run() =>
        runListT.Map(
            ma => ma switch
                  {
                      MNil<A> =>
                          Option<(A Head, IterableT<M, A> Tail)>.None,

                      MCons<M, A>(var h, var t) =>
                          Option<(A Head, IterableT<M, A> Tail)>.Some((h, new IterableMainT<M, A>(t()))),
                      
                      _ => throw new NotSupportedException()
                  });
    
    public static IterableT<M, A> Pure(A value) =>
        new IterablePureT<M, A>(value);

    public static IterableT<M, A> Lift(Seq<A> list) =>
        list switch
        {
            []          => Empty,
            var (x, xs) => new IterableMainT<M, A>(M.Pure<MList<A>>(new MCons<M, A>(x, () => Lift(xs).runListT))) 
        };

    public static IterableT<M, A> Lift(IEnumerable<A> list) =>
        new IterableEnumerableT<M, A>(list);

    public static IterableT<M, A> Lift(K<M, A> ma) =>
        new IterableLiftM<M, A>(ma);

    public static IterableT<M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    public abstract IterableT<M, B> Map<B>(Func<A, B> f);    

    /// <summary>
    /// Concatenate sequences
    /// </summary>
    public IterableT<M, A> Combine(IterableT<M, A> rhs) =>
        new IterableMainT<M, A>(
            from mxs in runListT
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
    public static IterableT<M, A> Empty { get; } = new IterableMainT<M, A>(M.Pure<MList<A>>(new MNil<A>()));

    public IterableT<M, A> Filter(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public IterableT<M, A> Where(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public IterableT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);
    
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
