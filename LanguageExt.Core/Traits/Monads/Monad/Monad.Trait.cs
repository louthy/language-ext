using System;
using System.Collections.Generic;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Monad trait
/// </summary>
/// <typeparam name="M">Self-referring trait</typeparam>
public interface Monad<M> :
    Applicative<M>, 
    Maybe.MonadUnliftIO<M> 
    where M : Monad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);

    /// <summary>
    /// Tail-recursive bind
    /// </summary>
    /// <param name="value">Initial value to the bind expression</param>
    /// <param name="f">Bind function</param>
    /// <typeparam name="A">Continuation value type</typeparam>
    /// <typeparam name="B">Completed value type</typeparam>
    /// <returns>Result of repeatedly invoking `f` until Right(b) is returned</returns>
    public static abstract K<M, B> Recur<A, B>(A value, Func<A, K<M, Next<A, B>>> f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    /// <summary>
    /// Lift a Next.Done value into the monad
    /// </summary>
    public static virtual K<M, Next<A, B>> Done<A, B>(B value) =>
        M.Pure(Next.Done<A, B>(value));
    
    /// <summary>
    /// Lift a Next.Done value into the monad
    /// </summary>
    public static virtual K<M, Next<A, B>> Loop<A, B>(A value) =>
        M.Pure(Next.Loop<A, B>(value));

    public static virtual K<M, C> SelectMany<A, B, C>(K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        ma.Bind(x => bind(x).Map(y => project(x, y)));

    public static virtual K<M, C> SelectMany<A, B, C>(K<M, A> ma, Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        M.Map(x => project(x, bind(x).Value), ma);

    public static virtual K<M, A> Flatten<A>(K<M, K<M, A>> mma) =>
        M.Bind(mma, identity);

    static K<M, A> Applicative<M>.Actions<A>(IterableNE<K<M, A>> mas)
    {
        // TODO: Check if this implementation is valid
        
        return M.Recur(mas.GetIterator(), go).Flatten();
        K<M, Next<Iterator<K<M, A>>, K<M, A>>> go(Iterator<K<M, A>> iter) =>
            iter switch
            {
                (var head, { IsEmpty : true }) => M.Pure(Next.Done<Iterator<K<M, A>>, K<M, A>>(head)),
                var (head, tail)               => head.Map(_ => Next.Loop<Iterator<K<M, A>>, K<M, A>>(tail.Clone()))
            };
    }   
}
