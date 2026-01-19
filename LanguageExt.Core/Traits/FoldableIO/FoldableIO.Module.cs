using System;
using System.Numerics;

namespace LanguageExt.Traits;

public static class FoldableIO
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static IO<S> foldWhileIO<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState,
        K<T, A> ta)
        where T : FoldableIO<T> =>
        ta.FoldWhileIO(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableIO<T>
        where M : MonadIO<M> =>
        ta.FoldWhileM(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static IO<S> foldUntilIO<T, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState, 
        K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.FoldUntilIO(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : MonadIO<M>
        where T : FoldableIO<T> => 
        ta.FoldUntilM(f, predicate, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static IO<S> foldIO<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.FoldIO(f, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static K<M, S> foldM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableIO<T>
        where M : MonadIO<M> =>
        ta.FoldM(f, initialState);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static IO<A> foldIO<T, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where A : Monoid<A> =>
        ta.FoldIO();

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static IO<A> foldWhileIO<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where T : FoldableIO<T>
        where A : Monoid<A> =>
        ta.FoldWhileIO(predicate) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static IO<A> foldUntilIO<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where T : FoldableIO<T>
        where A : Monoid<A> =>
        ta.FoldUntilIO(predicate) ;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static IO<Seq<A>> toSeqIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.ToSeqIO();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static IO<Lst<A>> toLstIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.ToLstIO();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static IO<bool> isEmptyIO<T, A>(K<T, A> ta)
        where T : FoldableIO<T> =>
        ta.IsEmptyIO;

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static IO<long> countIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.CountIO;

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static IO<bool> existsIO<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.ExistsIO(predicate);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static IO<bool> forAllIO<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.ForAllIO(predicate);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static IO<bool> containsIO<EqA, T, A>(A value, K<T, A> ta) 
        where EqA : Eq<A> 
        where T : FoldableIO<T> =>
        T.ContainsIO<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static IO<bool> containsIO<T, A>(A value, K<T, A> ta)
        where T : FoldableIO<T> =>
        ta.ContainsIO(value);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static IO<Option<A>> findIO<T, A>(Func<A, bool> predicate, K<T, A> ta)
        where T : FoldableIO<T> =>
        ta.FindIO(predicate);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static IO<A> sumIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        ta.SumIO();

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static IO<A> productIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        ta.ProductIO();

    /// <summary>
    /// Get the head item in the FoldableIO or `None`
    /// </summary>
    public static IO<Option<A>> headIO<T, A>(K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.HeadIO;

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> iterM<T, A, F, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where T : FoldableIO<T>
        where F : MonadIO<F> =>
        ta.IterM(f);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static IO<Unit> iterIO<T, A>(Action<long, A> f, K<T, A> ta) 
        where T : FoldableIO<T> =>
        ta.IterIO(f);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static IO<Unit> iterIO<T, A>(Action<A> f, K<T, A> ta)
        where T : FoldableIO<T> =>
        ta.IterIO(f);
        
    /// <summary>
    /// Inject a value in between each item in the enumerable 
    /// </summary>
    /// <param name="ta">Foldable structure</param>
    /// <param name="sep">Item to inject</param>
    /// <returns>An iterable with the values injected</returns>
    public static IteratorIO<A> intersperseIO<T, A>(A sep, K<T, A> ta) 
        where T : FoldableIO<T> =>
        T.IntersperseIO(sep, ta);
    
}
