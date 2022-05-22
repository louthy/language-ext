using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Data type that represents an edit in a patch.  Supports three sub-class 'cases':
    /// 
    ///     Edit.Insert
    ///     Edit.Delete
    ///     Edit.Replace
    ///     
    /// These represent the total set of operations that can be represented in a `Patch`
    /// </summary>
    /// <typeparam name="EqA"></typeparam>
    /// <typeparam name="A"></typeparam>
    public abstract class Edit<EqA, A> : IEquatable<Edit<EqA, A>> where EqA : struct, Eq<A>
    {
        public readonly int Position;
        public readonly A Element;

        /// <summary>
        /// Ctor
        /// </summary>
        Edit(int position, A element)
        {
            Position = position;
            Element = element;
        }

        /// <summary>
        /// Position lens that allows for the Position to be modified
        /// </summary>
        internal abstract Edit<EqA, A> Index(Func<int, int> f);

        /// <summary>
        /// Maps the outgoing value using the function provided.  This has
        /// no effect on Insert
        /// </summary>
        public abstract Edit<EqA, A> MapOld(Func<A, A> f);

        /// <summary>
        /// Maps the new value using the function provided.  This has
        /// no effect on Delete
        /// </summary>
        public abstract Edit<EqA, A> MapNew(Func<A, A> f);

        /// <summary>
        /// Equality operator
        /// </summary>
        public override bool Equals(object obj) => obj is Edit<EqA, A> edit ? Equals(edit) : false;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Edit<EqA, A> a, Edit<EqA, A> b) => a.Equals(b);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        public static bool operator !=(Edit<EqA, A> a, Edit<EqA, A> b) => !(a == b);

        /// <summary>
        /// Hash code provider
        /// </summary>
        public override int GetHashCode() => (Position + 13) * Element.GetHashCode();

        /// <summary>
        /// Equality operator
        /// </summary>
        public bool Equals(Edit<EqA, A> other) =>
            ReferenceEquals(this, other) ||
            (Position == other.Position &&
             GetType() == other.GetType() &&
             default(EqA).Equals(Element, other.Element));

        /// <summary>
        /// Represents an Insert edit operation
        /// </summary>
        public sealed class Insert : Edit<EqA, A>, IEquatable<Insert>
        {
            public Insert(int position, A element) : base(position, element) { }
            public static Insert New(int position, A element) => new Insert(position, element);
            public override string ToString() => $"Insert({Position}, {Element})";
            internal override Edit<EqA, A> Index(Func<int, int> f) => new Insert(f(Position), Element);
            public override Edit<EqA, A> MapOld(Func<A, A> f) => this;
            public override Edit<EqA, A> MapNew(Func<A, A> f) => new Insert(Position, f(Element));
            public bool Equals(Insert other) => base.Equals(other);
            public override bool Equals(object obj) => obj is Insert ins && Equals(ins);
            public static bool operator ==(Insert a, Insert b) => a.Equals(b);
            public static bool operator !=(Insert a, Insert b) => !(a == b);
            public override int GetHashCode() => (Position + 13) * (Element.GetHashCode() + 7);
        }

        /// <summary>
        /// Represents an Delete edit operation
        /// </summary>
        public sealed class Delete : Edit<EqA, A>, IEquatable<Delete>
        {
            public Delete(int position, A element) : base(position, element) { }
            public static Delete New(int position, A element) => new Delete(position, element);
            public override string ToString() => $"Delete({Position}, {Element})";
            internal override Edit<EqA, A> Index(Func<int, int> f) => new Delete(f(Position), Element);
            public override Edit<EqA, A> MapOld(Func<A, A> f) => new Delete(Position, f(Element));
            public override Edit<EqA, A> MapNew(Func<A, A> f) => this;
            public bool Equals(Delete other) => base.Equals(other);
            public override bool Equals(object obj) => obj is Delete del && Equals(del);
            public static bool operator ==(Delete a, Delete b) => a.Equals(b);
            public static bool operator !=(Delete a, Delete b) => !(a == b);
            public override int GetHashCode() => (Position + 13) * (Element.GetHashCode() + 7);
        }

        /// <summary>
        /// Represents an Replace edit operation
        /// </summary>
        public sealed class Replace : Edit<EqA, A>, IEquatable<Replace>
        {
            public readonly A ReplaceElement;
            public Replace(int position, A element, A replaceElement) : base(position, element) =>
                ReplaceElement = replaceElement;
            public static Replace New(int position, A element, A replaceElement) => new Replace(position, element, replaceElement);
            public override string ToString() => $"Replace({Position}, {Element} -> {ReplaceElement})";
            internal override Edit<EqA, A> Index(Func<int, int> f) => new Replace(f(Position), Element, ReplaceElement);
            public override Edit<EqA, A> MapOld(Func<A, A> f) => new Replace(Position, f(Element), ReplaceElement);
            public override Edit<EqA, A> MapNew(Func<A, A> f) => new Replace(Position, Element, f(ReplaceElement));
            public bool Equals(Replace other) =>
                ReferenceEquals(this, other) ||
                (base.Equals(other) && default(EqA).Equals(ReplaceElement, other.ReplaceElement));
            public override bool Equals(object obj) => obj is Replace repl && Equals(repl);
            public static bool operator ==(Replace a, Replace b) => a.Equals(b);
            public static bool operator !=(Replace a, Replace b) => !(a == b);
            public override int GetHashCode() => (Position + 13) * (Element.GetHashCode() + 17) * (ReplaceElement.GetHashCode() + 7);
        }
    }
}
