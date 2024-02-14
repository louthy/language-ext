using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Foldable
{
    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<A, S, S> f, S initialState, K<T, A> ta)
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);
    
    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<A, Func<S, S>> f, S initialState, K<T, A> ta)
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);
    
    /// <summary>
    /// Left-associative fold of a structure, lazy in the accumulator.  This
    /// is rarely what you want, but can work well for structures with efficient
    /// right-to-left sequencing and an operator that is lazy in its left
    /// argument.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack' will diverge if given an infinite list.
    /// </remarks>
    public static S foldBack<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack(f, initialState, ta);
    
    /// <summary>
    /// Left-associative fold of a structure, lazy in the accumulator.  This
    /// is rarely what you want, but can work well for structures with efficient
    /// right-to-left sequencing and an operator that is lazy in its left
    /// argument.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack' will diverge if given an infinite list.
    /// </remarks>
    public static S foldBack<T, A, S>(Func<S, Func<A, S>> f, S initialState, K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack(f, initialState, ta);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A fold<T, M, A>(K<T, A> tm)
        where M : Monoid<A>
        where T : Foldable<T> =>
        T.Fold<M, A>(tm);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMap<T, M, A, B>(Func<A, B> f, K<T, A> ta)
        where M : Monoid<B>
        where T : Foldable<T> =>
        T.FoldMap<M, A, B>(f, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBack<T, M, A, B>(Func<A, B> f, K<T, A> ta)
        where M : Monoid<B>
        where T : Foldable<T> =>
        T.FoldBack((x, a) => M.Append(x, f(a)), M.Empty, ta);

    /// <summary>
    /// Right-to-left monadic fold over the elements of a structure.
    /// 
    /// Given a structure `T` with elements `[a, b, c, ..., x, y]`, the result of
    /// a fold with an operator function `f` is equivalent to running each element
    /// through the monadic bind operator and chaining it to the next element,
    /// resulting in a single monadic result.
    /// </summary>
    public static K<M, S> foldM<T, M, A, S>(
        Func<A, Func<S, K<M, S>>> f,
        S initialState,
        K<T, A> ta)
        where T : Foldable<T>
        where M : Monad<M>
    {
        return T.FoldBack(acc, Monad.pure<M, S>, ta)(initialState);

        Func<A, Func<S, K<M, S>>> acc(Func<S, K<M, S>> k) =>
            bind => z => Monad.bind(f(bind)(z), k);
    }

    /// <summary>
    /// Right-to-left monadic fold over the elements of a structure.
    /// 
    /// Given a structure `T` with elements `[a, b, c, ..., x, y]`, the result of
    /// a fold with an operator function `f` is equivalent to running each element
    /// through the monadic bind operator and chaining it to the next element,
    /// resulting in a single monadic result.
    /// </summary>
    public static K<M, S> foldM<T, M, A, S>(
        Func<A, S, K<M, S>> f,
        S initialState,
        K<T, A> ta)
        where T : Foldable<T>
        where M : Monad<M>
    {
        return foldBack(acc, Monad.pure<M, S>, ta)(initialState);

        Func<A, Func<S, K<M, S>>> acc(Func<S, K<M, S>> k) =>
            bind => z => Monad.bind(f(bind, z), k);
    }

    /// <summary>
    /// Left-to-right monadic fold over the elements of a structure.
    /// 
    /// Given a structure `T` with elements `[a, b, c, ..., x, y]`, the result of
    /// a fold with an operator function `f` is equivalent to running each element
    /// through the monadic bind operator and chaining it to the next element,
    /// resulting in a single monadic result.
    /// </summary>
    public static K<M, B> foldBackM<T, M, A, B>(
        Func<B, Func<A, K<M, B>>> f,
        B initialState,
        K<T, A> ta)
        where T : Foldable<T>
        where M : Monad<M>
    {
        return fold(acc, Monad.pure<M, B>, ta)(initialState);

        Func<Func<B, K<M, B>>, Func<B, K<M, B>>> acc(A x) =>
            bind => z => Monad.bind(f(z)(x), bind);
    }

    /// <summary>
    /// Left-to-right monadic fold over the elements of a structure.
    /// 
    /// Given a structure `T` with elements `[a, b, c, ..., x, y]`, the result of
    /// a fold with an operator function `f` is equivalent to running each element
    /// through the monadic bind operator and chaining it to the next element,
    /// resulting in a single monadic result.
    /// </summary>
    public static K<M, B> foldBackM<T, M, A, B>(
        Func<B, A, K<M, B>> f,
        B initialState,
        K<T, A> ta)
        where T : Foldable<T>
        where M : Monad<M>
    {
        return fold(acc, Monad.pure<M, B>, ta)(initialState);

        Func<Func<B, K<M, B>>, Func<B, K<M, B>>> acc(A x) =>
            bind => z => Monad.bind(f(z, x), bind);
    }
    
    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> toSeq<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        T.ToSeq(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool isEmpty<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        T.IsEmpty(ta);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static int count<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((c, _) => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool exists<T, A>(Func<A, bool> predicate, K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((s, c) => s || predicate(c), false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAll<T, A>(Func<A, bool> predicate, K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((s, c) => s && predicate(c), true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, EqA, A>(A value, K<T, A> ta)
        where T : Foldable<T>
        where EqA : Eq<A> =>
        T.Contains<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, A>(A value, K<T, A> ta)
        where T : Foldable<T> =>
        T.Contains<EqDefault<A>, A>(value, ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A sum<T, NumA, A>(K<T, A> ta) 
        where NumA : Num<A>
        where T : Foldable<T> =>
        T.Sum<NumA, A>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static int sum<T>(K<T, int> ta) 
        where T : Foldable<T> =>
        T.Sum<TInt, int>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static long sum<T>(K<T, long> ta) 
        where T : Foldable<T> =>
        T.Sum<TLong, long>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static float sum<T>(K<T, float> ta) 
        where T : Foldable<T> =>
        T.Sum<TFloat, float>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static double sum<T>(K<T, double> ta) 
        where T : Foldable<T> =>
        T.Sum<TDouble, double>(ta);

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see 'Data.Traversable.traverse'.
    /// </summary>
    public static K<F, Unit> traverse<T, F, A, B>(Func<A, K<F, B>> f, K<T, A> ta) 
        where T : Foldable<T>
        where F : Applicative<F>
    {
        return T.Fold(acc, F.Pure(unit), ta);

        Func<K<F, Unit>, K<F, Unit>> acc(A x) =>
            k => F.Map(_ => unit, F.Action(f(x), k));
    }

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see 'Data.Traversable.traverse'.
    /// </summary>
    public static K<F, Unit> mapM<T, F, A, B>(Func<A, K<F, B>> f, K<T, A> ta) 
        where T : Foldable<T>
        where F : Applicative<F>
    {
        return T.Fold(acc, F.Pure(unit), ta);

        Func<K<F, Unit>, K<F, Unit>> acc(A x) =>
            k => F.Map(_ => unit, F.Action(f(x), k));
    }
}
