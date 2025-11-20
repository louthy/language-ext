using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class TransducerMExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static TransducerM<M, A, B> As<M, A, B>(this K<TransduceFromM<M, A>, B> ma) =>
        (TransducerM<M, A, B>)ma;
    
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static TransducerM<M, A, B> As<M, A, B>(this K<TransduceToM<M, B>, A> ma) =>
        (TransducerM<M, A, B>)ma;
    
    /// <summary>
    /// Filter values flowing through the transducer. 
    /// </summary>
    public static TransducerM<M, A, B> Filter<M, A, B>(this TransducerM<M, A, B> tab, Func<B, bool> f) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.filter<M, B>(f));
    
    /// <summary>
    /// Filter values flowing through the transducer. 
    /// </summary>
    public static TransducerM<M, A, B> Where<M, A, B>(this TransducerM<M, A, B> tab, Func<B, bool> f) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.filter<M, B>(f));

    /// <summary>
    /// Take the first `n` values from the stream.  
    /// </summary>
    public static TransducerM<M, A, B> Take<M, A, B>(this TransducerM<M, A, B> tab, int n) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.take<M, B>(n));

    /// <summary>
    /// Skip the first `n` values from the stream. 
    /// </summary>
    public static TransducerM<M, A, B> Skip<M, A, B>(this TransducerM<M, A, B> tab, int n) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.skip<M, B>(n));

    public static TransducerM<M, A, S> FoldWhile<M, A, B, S>(
        this TransducerM<M, A, B> tab, 
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.foldWhile<M, B, S>(Folder, Pred, State));
    
    public static TransducerM<M, A, S> FoldWhile<M, A, B, S>(
        this TransducerM<M, A, B> tab, 
        Schedule Schedule, 
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.foldWhile<M, B, S>(Schedule, Folder, Pred, State));
    
    public static TransducerM<M, A, S> FoldUntil<M, A, B, S>(
        this TransducerM<M, A, B> tab,         
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.foldUntil<M, B, S>(Folder, Pred, State));
    
    public static TransducerM<M, A, S> FoldUntil<M, A, B, S>(
        this TransducerM<M, A, B> tab, 
        Schedule Schedule, 
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) 
        where M : Applicative<M> =>
        tab.Compose(TransducerM.foldUntil<M, B, S>(Schedule, Folder, Pred, State));      
}
