using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Reflection;
using System.Diagnostics.Contracts;

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
    ///     class Metres : NewType<double> { public class Metres(int x) : base(x) {} }
    ///     class Hours : NewType<double> { public class Hours(int x) : base(x) {} }
    ///
    /// Will not accept null values
    ///
    /// </summary>
#if !COREFX
    [Serializable]
#endif
    public abstract class NewType<T> : 
        IEquatable<NewType<T>>, 
        IComparable<NewType<T>>,
        IAppendable<NewType<T>>,
        ISubtractable<NewType<T>>,
        IMultiplicable<NewType<T>>,
        IDivisible<NewType<T>>
    {
        public readonly T Value;

        public NewType(T value)
        {
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        [Pure]
        public int CompareTo(NewType<T> other) =>
            !ReferenceEquals(other, null) &&
            GetType() == other.GetType()
                ? Comparer<T>.Default.Compare(Value, other.Value)
                : failwith<int>("Mismatched NewTypes used in comparison");

        [Pure]
        public bool Equals(NewType<T> other) =>
            !ReferenceEquals(other, null) &&
            GetType() == other.GetType() &&
            Value.Equals(other.Value);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is NewType<T> &&
            Equals((NewType<T>)obj);

        [Pure]
        public override int GetHashCode() =>
            Value == null ? 0 : Value.GetHashCode();

        [Pure]
        public static bool operator ==(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(NewType<T> lhs, NewType<T> rhs) =>
            !lhs.Equals(rhs);

        [Pure]
        public static bool operator >(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public static bool operator <(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public NewType<T> Bind(Func<T, NewType<T>> bind)
        {
            var ures = bind(Value);
            if (GetType() != ures.GetType()) throw new Exception("LINQ statement with mismatched NewTypes");
            return ures;
        }

        [Pure]
        public bool Exists(Func<T, bool> predicate) =>
            predicate(Value);

        [Pure]
        public bool ForAll(Func<T, bool> predicate) =>
            predicate(Value);

        [Pure]
        public int Count() => 1;

        [Pure]
        public NewType<T> Map(Func<T, T> map) =>
            Select(map);

        [Pure]
        public static NewType<T> operator +(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public static NewType<T> operator -(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public static NewType<T> operator /(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Divide(rhs);

        [Pure]
        public static NewType<T> operator *(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Multiply(rhs);

        [Pure]
        public NewType<T> Select(Func<T, T> map) =>
            (NewType<T>)NewType.Construct(GetType(), map(Value));

        [Pure]
        public NewType<T> SelectMany(
            Func<T, NewType<T>> bind,
            Func<T, T, T> project
            )
        {
            var ures = bind(Value);
            if (GetType() != ures.GetType()) throw new Exception("LINQ statement with mismatched NewTypes");
            return (NewType<T>)NewType.Construct(GetType(), project(Value, ures.Value));
        }

        [Pure]
        public NewType<T> Append(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Append(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in append/add");

        [Pure]
        public NewType<T> Subtract(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Subtract(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in subtract");

        [Pure]
        public NewType<T> Divide(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Divide(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in divide");

        [Pure]
        public NewType<T> Multiply(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Multiply(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in multiply");

        public Unit Iter(Action<T> f)
        {
            f(Value);
            return unit;
        }

        [Pure]
        public NT As<NT>() where NT : NewType<T> =>
            GetType() == typeof(NT)
                ? (NT)this
                : failwith<NT>("Mismatched NewTypes cast");

        [Pure]
        public override string ToString() =>
            $"{GetType().Name}({Value})";
    }

    public static class NewType
    {
        static Map<string, ConstructorInfo> constructors = Map.empty<string, ConstructorInfo>();
        private static ConstructorInfo GetCtor(Type newType)
        {
            if (newType.Name == "NewType") throw new ArgumentException("Only use NewType.Contruct to build construct types derived from NewType<T>");
            var ctors = (from c in newType.GetTypeInfo().DeclaredConstructors
                         where c.GetParameters().Length == 1
                         select c)
                        .ToArray();

            if (ctors.Length == 0) throw new ArgumentException($"{newType.FullName} hasn't any one-argument constructors");
            if (ctors.Length > 1) throw new ArgumentException($"{newType.FullName} has more than one constructor with 1 parameter");

            var ctor = ctors.First();
            // No locks because we don't really care if it's done
            // more than once, but we do care about locking unnecessarily.
            constructors = constructors.AddOrUpdate(newType.FullName, ctor);
            return ctor;
        }

        public static object Construct(Type newTypeT, object arg) =>
            constructors.Find(newTypeT.FullName).IfNone(() => GetCtor(newTypeT)).Invoke(new object[] { arg });

        public static NewTypeT Construct<NewTypeT, T>(T arg) where NewTypeT : NewType<T> =>
            (NewTypeT)constructors.Find(typeof(NewTypeT).FullName).IfNone(() => GetCtor(typeof(NewTypeT))).Invoke(new object[] { arg });
    }
}

public static class __NewTypeExts
{
    public static S Fold<T, S>(this NewType<T> self, S state, Func<S, T, S> folder) =>
        folder(state, self.Value);

    public static int Sum(this NewType<int> self) =>
        self.Value;
}
