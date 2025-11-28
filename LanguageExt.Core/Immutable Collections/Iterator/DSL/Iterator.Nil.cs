using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator<A> : 
    IEnumerable<A>,
    IEquatable<Iterator<A>>,
    IDisposable,
    K<Iterator, A>
{
    /// <summary>
    /// Nil iterator case
    ///
    /// The end of the sequence.
    /// </summary>
    public sealed class Nil : Iterator<A>
    {
        public static readonly Iterator<A> Default = new Nil();

        public override string ToString() => 
            "Nil";

        /// <summary>
        /// Head element
        /// </summary>
        public override A Head =>
            throw new InvalidOperationException("Nil iterator has no head");

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Iterator<A> Tail =>
            this;

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override bool IsEmpty =>
            true;

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override Iterator<A> Clone() =>
            this;

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override Iterator<A> Split() =>
            this;

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override long Count =>
            0;

        public override void Dispose()
        {
        }
    }
}
