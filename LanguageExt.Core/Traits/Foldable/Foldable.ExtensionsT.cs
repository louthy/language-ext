using System;
using System.Numerics;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt.Traits;

public static partial class Foldable
{
    /// <summary>
    /// Sum the items in the nested foldables 
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .SumT();
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Summed value</returns>
    public static A SumT<M, N, A>(this K<M, K<N, A>> mna)
        where A : INumber<A>
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => s1 + sum(na), A.Zero, mna);

    /// <summary>
    /// Count the items in the nested foldables 
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .CountT();
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Total number of items counted</returns>
    public static int CountT<M, N, A>(this K<M, K<N, A>> mna)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => s1 + count(na), 0, mna);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldT(0, (s, x) => s + x);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="S">Aggregate state</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static S FoldT<M, N, S, A>(this K<M, K<N, A>> mna, S state, Func<A, S, S> f)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => fold(f, s1, na), state, mna);    

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldT(0, s => x => s + x);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="S">Aggregate state</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static S FoldT<M, N, S, A>(this K<M, K<N, A>> mna, S state, Func<A, Func<S, S>> f)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => fold(f, s1, na), state, mna);    

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
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldBackT(0, (x, s) => s + x);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="S">Aggregate state</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static S FoldBackT<M, N, S, A>(this K<M, K<N, A>> mna, S state, Func<S, A, S> f)
        where M : Foldable<M>
        where N : Foldable<N> =>
        foldBack((s1, na) => foldBack(f, s1, na), state, mna);    

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
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldBackT(0, x => s => s + x);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="S">Aggregate state</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static S FoldBackT<M, N, S, A>(this K<M, K<N, A>> mna, S state, Func<S, Func<A, S>> f)
        where M : Foldable<M>
        where N : Foldable<N> =>
        foldBack((s1, na) => foldBack(f, s1, na), state, mna);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldT();
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static A FoldT<M, N, A>(this K<M, K<N, A>> mna)
        where A : Monoid<A>
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => fold((x, s2) => s2.Append(x), s1, na), A.Empty, mna);    

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldMapT(x => x + 1);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static B FoldMapT<M, N, A, B>(this K<M, K<N, A>> mna, Func<A, B> f)
        where B : Monoid<B>
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s1) => fold((x, s2) => s2.Append(f(x)), s1, na), B.Empty, mna);
    
    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .FoldMapBackT(x => x + 1);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Bound value type</typeparam>
    /// <returns>Folded value</returns>
    public static B FoldMapBackT<M, N, A, B>(this K<M, K<N, A>> mna, Func<A, B> f)
        where B : Monoid<B>
        where M : Foldable<M>
        where N : Foldable<N> =>
        foldBack((s1, na) => foldBack((s2, x) => s2.Append(f(x)), s1, na), B.Empty, mna);


    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .ToSeqT();
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Sequence of values</returns>
    public static Seq<A> ToSeqT<M, N, A>(this K<M, K<N, A>> mna)
        where M : Foldable<M>
        where N : Foldable<N> =>
        mna.FoldT(Seq<A>.Empty, (x, s) => s.Add(x));

    /// <summary>
    /// Returns true if no values are in the foldable
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .IsEmptyT();
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>True if empty</returns>
    public static bool IsEmptyT<M, N, A>(this K<M, K<N, A>> mna)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s) => s && isEmpty(na), true, mna);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .ExistsT(x => x == 5);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>True if the predicate holds for at least one item in the structure</returns>
    public static bool ExistsT<M, N, A>(this K<M, K<N, A>> mna, Func<A, bool> predicate)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s) => s || exists(predicate, na), false, mna);


    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .ForAllT(x => x == 5);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>True if predicate holds for all items in the structure.  True if there are no items.</returns>
    public static bool ForAllT<M, N, A>(this K<M, K<N, A>> mna, Func<A, bool> predicate)
        where M : Foldable<M>
        where N : Foldable<N> =>
        fold((na, s) => s && forAll(predicate, na), true, mna);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .Contains(5);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>True if value exists in the structure.</returns>
    public static bool ContainsT<M, N, EqA, A>(this K<M, K<N, A>> mna, A value)
        where M : Foldable<M>
        where N : Foldable<N>
        where EqA : Eq<A> =>
        mna.ExistsT(x => EqA.Equals(x, value));

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .Contains(5);
    ///
    /// </example>
    /// <param name="mna">Nested value</param>
    /// <typeparam name="M">Outer foldable trait</typeparam>
    /// <typeparam name="N">Inner foldable trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>True if value exists in the structure.</returns>
    public static bool ContainsT<M, N, A>(this K<M, K<N, A>> mna, A value)
        where M : Foldable<M>
        where N : Foldable<N> =>
        mna.ExistsT(x => EqDefault<A>.Equals(x, value));
}
