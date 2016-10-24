using System;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// NewType - inspired by Haskell's 'newtype' keyword.
    /// https://wiki.haskell.org/Newtype
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Subtractable, 
    /// Multiplicable, Divisible, Foldable, Monadic, Functor, Interable: strongly typed values.  
    ///
    /// For example:
    ///
    ///     class Metres : NewType<Metres, double> { public Metres(double x) : base(x) {} }
    ///     class Hours : NewType<Hours, double> { public Hours(double x) : base(x) {} }
    ///
    /// Will not accept null values
    ///
    /// </summary>
#if !COREFX
    [Serializable]
#endif
    public abstract class NewType<NEWTYPE, A> :
        IEquatable<NEWTYPE>,
        IComparable<NEWTYPE>
        where NEWTYPE : NewType<NEWTYPE, A>
    {
        public readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, NEWTYPE> New = Reflect.Util.CreateDynamicConstructor<A, NEWTYPE>();

        public NewType(A value)
        {
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        [Pure]
        public int CompareTo(NEWTYPE other) =>
             OrdNewType<NEWTYPE, OrdDefault<A>, A>.Inst.Compare(this, other);

        [Pure]
        public bool Equals(NEWTYPE other) =>
            EqDefault<A>.Inst.Equals(Value, other.Value);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is NEWTYPE && Equals((NEWTYPE)obj);

        [Pure]
        public override int GetHashCode() =>
            Value == null ? 0 : Value.GetHashCode();

        [Pure]
        public static bool operator ==(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
             EqNewType<NEWTYPE, EqDefault<A>, A>.Inst.Equals(lhs, rhs);

        [Pure]
        public static bool operator !=(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
             !EqNewType<NEWTYPE, EqDefault<A>, A>.Inst.Equals(lhs, rhs);

        [Pure]
        public static bool operator >(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
            OrdNewType<NEWTYPE, OrdDefault<A>, A>.Inst.Compare(lhs, rhs) > 0;

        [Pure]
        public static bool operator >=(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
            OrdNewType<NEWTYPE, OrdDefault<A>, A>.Inst.Compare(lhs, rhs) >= 0;

        [Pure]
        public static bool operator <(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
            OrdNewType<NEWTYPE, OrdDefault<A>, A>.Inst.Compare(lhs, rhs) < 0;

        [Pure]
        public static bool operator <=(NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs) =>
            OrdNewType<NEWTYPE, OrdDefault<A>, A>.Inst.Compare(lhs, rhs) <= 0;

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        [Pure]
        public NEWTYPE Bind(Func<A, NEWTYPE> bind) =>
            bind(Value);

        /// <summary>
        /// Run a predicate for all values in the NewType (only ever one)
        /// </summary>
        [Pure]
        public bool Exists(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Run a predicate for all values in the NewType (only ever one)
        /// </summary>
        [Pure]
        public bool ForAll(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Number of items (always 1)
        /// </summary>
        /// <returns></returns>
        [Pure]
        public int Count() => 1;

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public NEWTYPE Map(Func<A, A> map) =>
            Select(map);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public NEWTYPE Select(Func<A, A> map) =>
            New(map(Value));

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        /// <param name="project">Final projection (select)</param>
        [Pure]
        public NEWTYPE SelectMany(
            Func<A, NewType<NEWTYPE, A>> bind,
            Func<A, A, A> project) =>
            New(project(Value, bind(Value).Value));

        /// <summary>
        /// Invoke an action that takes the bound value as an argument
        /// </summary>
        /// <param name="f">Action to invoke</param>
        public Unit Iter(Action<A> f)
        {
            f(Value);
            return unit;
        }

        /// <summary>
        /// Generate a text representation of the NewType and value
        /// </summary>
        [Pure]
        public override string ToString() =>
            $"{GetType().Name}({Value})";

        /// <summary>
        /// Fold
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and NewType bound value</returns>
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);

        /// <summary>
        /// Fold back
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and NewType bound value</returns>
        public S FoldBack<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);
   }
}