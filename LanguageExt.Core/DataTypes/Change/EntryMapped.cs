using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Existing entry updated to this value
    /// </summary>
    /// <typeparam name="B">Value mapped to type</typeparam>
    public interface EntryMappedTo<B>
    {
        /// <summary>
        /// Value mapped-to
        /// </summary>
        public B To { get; }

        /// <summary>
        /// Deconstructs to a single value
        /// </summary>
        void Deconstruct(out B @to);
    }
    
    /// <summary>
    /// Existing entry updated from this value
    /// </summary>
    /// <typeparam name="A">Value mapped from type</typeparam>
    public interface EntryMappedFrom<A>
    {
        /// <summary>
        /// Value mapped-from
        /// </summary>
        public A From { get; }

        /// <summary>
        /// Deconstructs to a single value
        /// </summary>
        void Deconstruct(out A @from);
    }
    
    /// <summary>
    /// Existing entry updated 
    /// </summary>
    public sealed class EntryMapped<A, B> : 
        Change<B>,
        EntryMappedFrom<A>,
        EntryMappedTo<B>,
        IEquatable<EntryMapped<A, B>>
    {
        internal EntryMapped(A @from, B to)
        {
            From = @from;
            To = @to;
        }

        /// <summary>
        /// Value mapped from 
        /// </summary>
        public A From { get; }
        
        /// <summary>
        /// Value mapped to
        /// </summary>
        public B To { get; }

        public override bool Equals(Change<B> obj) =>
            obj is EntryMapped<A, B> rhs && Equals(rhs);

        public bool Equals(EntryMapped<A, B> rhs) =>
           !ReferenceEquals(rhs, null) &&
            default(EqDefault<A>).Equals(From, rhs.From) &&
            default(EqDefault<B>).Equals(To, rhs.To);

        public override int GetHashCode() =>
            FNV32.Next(
                From?.GetHashCode() ?? FNV32.OffsetBasis,
                To?.GetHashCode() ?? FNV32.OffsetBasis);

        public override string ToString() => $"{From} -> {To}";

        public void Deconstruct(out B to) =>
            to = To;

        public void Deconstruct(out A @from) =>
            @from = From;

        public void Deconstruct(out A @from, out B to)
        {
            @from = From;
            to = To;
        }
    }
}
