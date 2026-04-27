using System.Numerics;
using System.Runtime.Intrinsics;
using DomainTypesExamples.Invariants;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using static LanguageExt.Prelude;

namespace DomainTypesExamples;

/// <summary>
/// Vector type that can be of any dimension `D` and hold any intrinsic value `A`
/// </summary>
/// <remarks>
/// Operations are performed using SIMD instructions for performance, but if `A`
/// is not an intrinsic then an exception will be thrown.
/// </remarks>
/// <typeparam name="DimSize">Dimension type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public class Vector<DimSize, A> :
    DomainType<Vector<DimSize, A>, Arr<A>>,
    VectorSpace<Vector<DimSize, A>, A>
    where A : 
        IAdditiveIdentity<A, A>,
        IAdditionOperators<A, A, A>,
        ISubtractionOperators<A, A, A>,
        IMultiplyOperators<A, A, A>,
        IDivisionOperators<A, A, A>,
        IUnaryNegationOperators<A, A>
    where DimSize : DimensionSize
{
    readonly Arr<A> Values;
    
    private Vector(Arr<A> values)
    {
        if(values.Count != DimSize.Value) throw new ArgumentException(nameof(values));
        Values = values;
    }

    public static Fin<Vector<DimSize, A>> From(Arr<A> repr) =>
        SizeEqualsTo<DimSize, Arr<int>>
            .Validate(repr,
            (r, v) => Error.New($"Array isn't the correct size. " +
                                $"Expected: {r.Value}, got: {v}"));

    public Arr<A> To() => 
        Values;

    public override bool Equals(object? obj) =>
        obj is Vector<DimSize, A> rhs && Equals(rhs);

    public virtual bool Equals(Vector<DimSize, A>? other)
    {
        var ia = Values.GetEnumerator();
        var ib = (other?.To() ?? Arr.empty<A>()).GetEnumerator();
        while (ia.MoveNext() && ib.MoveNext())
        {
            if (!ia.Current.Equals(ib.Current)) 
                return false;
        }
        return ia.MoveNext() == ib.MoveNext();
    }

    public override int GetHashCode() =>
        hash(Values);

    public static bool operator ==(Vector<DimSize, A>? left, Vector<DimSize, A>? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Vector<DimSize, A>? left, Vector<DimSize, A>? right) => 
        !(left == right);

    public static Vector<DimSize, A> operator -(Vector<DimSize, A> value)
    {
        var vector = new A[DimSize.Value];
        var ix     = 0;
        foreach (var x in value.To())
        {
            vector[ix++] = -x;
        }
        return new(Arr.create(vector));
    }

    public static Vector<DimSize, A> operator +(Vector<DimSize, A> left, Vector<DimSize, A> right) 
    {
        var vector = new A[DimSize.Value];
        var rem    = DimSize.Value % Vector<A>.Count;
        var total  = DimSize.Value - rem;
        var larray = left.Values;
        var rarray = right.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v1 = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            var v2 = new Vector<A>(rarray.AsSpan(i, Vector<A>.Count));
            (v1 + v2).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            vector[i] = left.Values[i] + right.Values[i];
        }        
        return new(Arr.create(vector));
    }
    
    public static Vector<DimSize, A> operator -(Vector<DimSize, A> left, Vector<DimSize, A> right) 
    {
        var vector = new A[DimSize.Value];
        var rem    = DimSize.Value % Vector<A>.Count;
        var total  = DimSize.Value - rem;
        var larray = left.Values;
        var rarray = right.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v1 = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            var v2 = new Vector<A>(rarray.AsSpan(i, Vector<A>.Count));
            (v1 - v2).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            vector[i] = left.Values[i] - right.Values[i];
        }        
        return new(Arr.create(vector));
    }
    
    /// <summary>
    /// Returns a new vector whose values are the product of each pair of elements in two specified vectors.
    /// </summary>
    public static Vector<DimSize, A> operator *(Vector<DimSize, A> left, Vector<DimSize, A> right) 
    {
        var vector = new A[DimSize.Value];
        var rem    = DimSize.Value % Vector<A>.Count;
        var total  = DimSize.Value - rem;
        var larray = left.Values;
        var rarray = right.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v1 = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            var v2 = new Vector<A>(rarray.AsSpan(i, Vector<A>.Count));
            (v1 * v2).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            vector[i] = left.Values[i] * right.Values[i];
        }        
        return new(Arr.create(vector));
    }

    public static Vector<DimSize, A> operator *(Vector<DimSize, A> left, A right) 
    {
        var vector = new A[DimSize.Value];
        var rem    = DimSize.Value % Vector<A>.Count;
        var total  = DimSize.Value - rem;
        var larray = left.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            (v * right).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            vector[i] = left.Values[i] * right;
        }        
        return new(Arr.create(vector));
    }

    public static Vector<DimSize, A> operator /(Vector<DimSize, A> left, A right) 
    {
        var vector = new A[DimSize.Value];
        var rem    = DimSize.Value % Vector<A>.Count;
        var total  = DimSize.Value - rem;
        var larray = left.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            (v / right).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            vector[i] = left.Values[i] / right;
        }        
        return new(Arr.create(vector));
    }

    /// <summary>
    /// Calculate the dot product between two vectors 
    /// </summary>
    public A Dot(Vector<DimSize, A> rhs) =>
        (this * rhs).Sum();

    /// <summary>
    /// Calculate of all values in the vector
    /// </summary>
    public A Sum()
    {
        var rem   = DimSize.Value % 16;
        var total = DimSize.Value - rem;
        var array = Values;
        var sum   = A.AdditiveIdentity;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var span = array.AsSpan(i, 16);
            sum += span[0]  + span[1]  + span[2]  + span[3]  +
                   span[4]  + span[5]  + span[6]  + span[7]  +
                   span[8]  + span[9]  + span[10] + span[11] +
                   span[12] + span[13] + span[14] + span[15];
        }
        
        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = DimSize.Value - rem; i < DimSize.Value; i++)
        {
            sum += array[i];
        }        
        return sum;
    }

    public override string ToString() => 
        Values.ToFullArrayString();
}
