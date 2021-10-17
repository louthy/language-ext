using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// A `Patch` is a collection of edits performed to a _document_, in this case an 'IEnumerable&lt;A&gt;'. 
    /// They are implemented as a list of 'Edit', and can be converted to and from raw lists of edits using 
    /// `toList` and `fromList` respectively.
    /// 
    /// Patches form a groupoid (a 'Monoid' with inverses, and a partial composition relation), where the 
    /// inverse element can be computed with 'inverse' and the groupoid operation is a _composition_ of 
    /// patches.  Applying `append(p1, p2)` is the same as applying `p1` _then_
    /// `p2` (see `Patch.apply`). This composition operator may produce structurally different patches depending 
    /// on associativity, however the patches are guaranteed to be _equivalent_ in the sense that the resultant 
    /// document will be the same when they are applied.
    /// 
    /// For convenience, we make our composition operator here total, to fit the `Monoid` typeclass, but provide 
    /// some predicates (`Patch.composable` and `Patch.applicable`) to determine if the operation can be validly 
    /// used.
    /// </summary>
    public readonly struct Patch<EqA, A> : IEquatable<Patch<EqA, A>> where EqA : struct, Eq<A>
    {
        /// <summary>
        /// Empty patch
        /// </summary>
        public readonly static Patch<EqA, A> Empty = new Patch<EqA, A>(Seq<Edit<EqA, A>>());

        /// <summary>
        /// Edits that represent the operations to perform on a document 
        /// to transform it.
        /// </summary>
        public readonly Seq<Edit<EqA, A>> Edits;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Patch(Seq<Edit<EqA, A>> edits) =>
            Edits = edits;

        /// <summary>
        /// Equality operator
        /// </summary>
        public bool Equals(Patch<EqA, A> mb) =>
            default(EqSeq<EqEdit<EqA, A>, Edit<EqA, A>>).Equals(Edits, mb.Edits);

        /// <summary>
        /// Equality operator
        /// </summary>
        public override bool Equals(object obj) =>
            obj is Patch<EqA, A> p && Equals(p);

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Patch<EqA, A> pa, Patch<EqA, A> pb) =>
            pa.Equals(pb);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        public static bool operator !=(Patch<EqA, A> pa, Patch<EqA, A> pb) =>
            !(pa == pb);

        /// <summary>
        /// Patch summing operator: monoid append
        /// </summary>
        public static Patch<EqA, A> operator +(Patch<EqA, A> pa, Patch<EqA, A> pb) => pa.Append(pb);

        /// <summary>
        /// Patch inverse operator
        /// </summary>
        public static Patch<EqA, A> operator -(Patch<EqA, A> pa) =>
            Patch.inverse(pa);

        /// <summary>
        /// Hash code provider
        /// </summary>
        public override int GetHashCode() =>
            Edits.GetHashCode();

        /// <summary>
        /// Return a string representation of the patch
        /// </summary>
        public override string ToString() =>
            $"[{String.Join("; ", Edits.Map(x => x.ToString()))}]";

        /// <summary>
        /// Returns true if a patch can be safely applied to a document, that is,
        /// `applicable(p, d)` holds when `d` is a valid source document for the patch `p`.
        /// </summary>
        public bool Applicable(IEnumerable<A> document) =>
            Patch.applicable(this, document);

        /// <summary>
        /// Monoid append: produces a patch is a merged version of this 
        /// and the provided patch.  
        /// </summary>
        public Patch<EqA, A> Append(Patch<EqA, A> mb) =>
            Patch.append(this, mb);

        /// <summary>
        /// Compute the inverse of a patch
        /// </summary>
        public Patch<EqA, A> Inverse() =>
            Patch.inverse(this);

        /// <summary>
        /// Returns the delta of the document's size when a patch is applied.
        /// Essentially the number of `Insert` minus the number of `Delete`
        /// </summary>
        public int SizeChange() =>
            Patch.sizeChange(this);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(IEnumerable<A> va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(Seq<A> va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(Lst<A> va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(SpanArray<A> va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(Arr<A> va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(A[] va) =>
            Patch.apply(this, va);

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public IEnumerable<A> Apply(List<A> va) =>
            Patch.apply(this, va);

    }
}
