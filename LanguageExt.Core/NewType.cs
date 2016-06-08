using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Reflection;

namespace LanguageExt
{
    public interface INewType
    {
    }

    /// <summary>
    /// NewType - inspired by Haskell's 'newtype' keyword.
    /// https://wiki.haskell.org/Newtype
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Subtractable, 
    /// Multiplicable, Divisible strongly typed values.  For example:
    ///     Metres : NewType<double>
    ///     Hours : NewType<double>
    /// </summary>
    public class NewType<T> : 
        IEquatable<NewType<T>>, 
        IComparable<NewType<T>>, 
        IAppendable<NewType<T>>,
        ISubtractable<NewType<T>>,
        IMultiplicable<NewType<T>>,
        IDivisible<NewType<T>>,
        INewType
    {
        public readonly T Value;

        public NewType(T value)
        {
            Value = value;
        }

        public int CompareTo(NewType<T> other) =>
            !Object.ReferenceEquals(other, null) &&
            this.GetType() == other.GetType()
                ? Comparer<T>.Default.Compare(Value, other.Value)
                : -1;

        public bool Equals(NewType<T> other) =>
            !Object.ReferenceEquals(other, null) &&
            this.GetType() == other.GetType() &&
            Value.Equals(other.Value);

        public override bool Equals(object obj) =>
            !Object.ReferenceEquals(obj, null) &&
            obj is NewType<T> &&
            Equals((NewType<T>)obj);

        public override int GetHashCode() =>
            Value == null ? 0 : Value.GetHashCode();

        public static bool operator ==(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(NewType<T> lhs, NewType<T> rhs) =>
            !lhs.Equals(rhs);

        public static bool operator >(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
             lhs.GetType() == rhs.GetType()
                ? lhs.CompareTo(rhs) > 0
                : failwith<bool>("Mismatched NewTypes used with '>'");

        public static bool operator >=(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.GetType() == rhs.GetType()
                ? lhs.CompareTo(rhs) >= 0
                : failwith<bool>("Mismatched NewTypes used with '>='");

        public static bool operator <(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.GetType() == rhs.GetType()
                ? lhs.CompareTo(rhs) < 0
                : failwith<bool>("Mismatched NewTypes in used with '>'");

        public static bool operator <=(NewType<T> lhs, NewType<T> rhs) =>
            !ReferenceEquals(lhs, null) &&
            !ReferenceEquals(rhs, null) &&
            lhs.GetType() == rhs.GetType()
                ? lhs.CompareTo(rhs) <= 0
                : failwith<bool>("Mismatched NewTypes in used with '<='");

        public static NewType<T> operator +(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Append(rhs);

        public static NewType<T> operator -(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Subtract(rhs);

        public static NewType<T> operator /(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Divide(rhs);

        public static NewType<T> operator *(NewType<T> lhs, NewType<T> rhs) =>
            lhs.Multiply(rhs);

        public NewType<T> Map(Func<T, T> map) =>
            Select(map);

        public NewType<T> Bind(Func<T, NewType<T>> bind)
        {
            var ures = bind(Value);
            if (GetType() != ures.GetType()) throw new Exception("LINQ statement with mismatched NewTypes");
            return ures;
        }

        public NewType<T> Fold<S>(S state, Func<S, T, S> folder) =>
            (NewType<T>)NewType.Construct(GetType(), folder(state, Value));

        public NewType<T> Select(Func<T, T> map) =>
            (NewType<T>)NewType.Construct(GetType(), map(Value));

        public NewType<T> SelectMany(
            Func<T, NewType<T>> bind,
            Func<T, T, T> project
            )
        {
            var ures = bind(Value);
            if (GetType() != ures.GetType()) throw new Exception("LINQ statement with mismatched NewTypes");
            return (NewType<T>)NewType.Construct(GetType(), project(Value, ures.Value));
        }

        public Unit Iter(Action<T> f)
        {
            f(Value);
            return unit;
        }

        public NewType<T> Append(NewType<T> rhs) =>
            this.GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Append(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in append/add");

        public NewType<T> Subtract(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Subtract(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in subtract");

        public NewType<T> Divide(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Divide(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in divide");

        public NewType<T> Multiply(NewType<T> rhs) =>
            GetType() == rhs.GetType()
                ? (NewType<T>)NewType.Construct(GetType(), TypeDesc.Multiply(Value, rhs.Value, TypeDesc<T>.Default))
                : failwith<NewType<T>>("Mismatched NewTypes in multiply");

        public NT As<NT>() where NT : NewType<T> =>
            GetType() == typeof(NT)
                ? (NT)this
                : failwith<NT>("Mismatched NewTypes cast");

        public override string ToString() =>
            $"{GetType().Name}({Value})";
    }

    public static class NewType
    {
        static Map<string, ConstructorInfo> constructors = Map.empty<string, ConstructorInfo>();

        private static ConstructorInfo GetCtor(Type newType)
        {
            if (newType.Name == "NewType") throw new ArgumentException("Only use NewType.Contruct to build construct types derived from NewType<T>");
            var ctors = (from c in newType.GetConstructors()
                         where c.GetParameters().Length == 1
                         select c)
                        .ToArray();

            if (ctors.Length > 1) throw new ArgumentException($"{newType.FullName} has more than one constructor with 1 parameter");

            var ctor = ctors.First();
            constructors = constructors.AddOrUpdate(newType.FullName,ctor);
            return ctor;
        }

        public static object Construct(Type newTypeT, object arg) =>
            constructors.Find(newTypeT.FullName).IfNone(GetCtor(newTypeT)).Invoke(new object[] { arg });

        public static NewTypeT Construct<NewTypeT, T>(T arg) where NewTypeT : NewType<T> =>
            (NewTypeT)constructors.Find(typeof(NewTypeT).FullName).IfNone(GetCtor(typeof(NewTypeT))).Invoke(new object[] { arg });
    }

}
