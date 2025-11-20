using System.Numerics;
using System.Runtime.Intrinsics;
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
/// <typeparam name="D">Dimension type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public class Vector<D, A> :
    DomainType<Vector<D, A>, Arr<A>>,
    VectorSpace<Vector<D, A>, A>
    where A : 
        IAdditiveIdentity<A, A>,
        IAdditionOperators<A, A, A>,
        ISubtractionOperators<A, A, A>,
        IMultiplyOperators<A, A, A>,
        IDivisionOperators<A, A, A>,
        IUnaryNegationOperators<A, A>
    where D : Dimension
{
    readonly Arr<A> Values;
    
    private Vector(Arr<A> values)
    {
        if(values.Count != D.Size) throw new ArgumentException(nameof(values));
        Values = values;
    }

    public static Fin<Vector<D, A>> From(Arr<A> repr) =>
        repr.Count == D.Size
            ? new Vector<D, A>(repr)
            : Error.New($"Array isn't the correct size.  Expected: {D.Size}, got: {repr.Count}");

    public Arr<A> To() => 
        Values;

    public override bool Equals(object? obj) =>
        obj is Vector<D, A> rhs && Equals(rhs);

    public virtual bool Equals(Vector<D, A>? other)
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

    public static bool operator ==(Vector<D, A>? left, Vector<D, A>? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Vector<D, A>? left, Vector<D, A>? right) => 
        !(left == right);

    public static Vector<D, A> operator -(Vector<D, A> value)
    {
        var vector = new A[D.Size];
        var ix     = 0;
        foreach (var x in value.To())
        {
            vector[ix++] = -x;
        }
        return new(Arr.create(vector));
    }

    public static Vector<D, A> operator +(Vector<D, A> left, Vector<D, A> right) 
    {
        var vector = new A[D.Size];
        var rem    = D.Size % Vector<A>.Count;
        var total  = D.Size - rem;
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
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            vector[i] = left.Values[i] + right.Values[i];
        }        
        return new(Arr.create(vector));
    }
    
    public static Vector<D, A> operator -(Vector<D, A> left, Vector<D, A> right) 
    {
        var vector = new A[D.Size];
        var rem    = D.Size % Vector<A>.Count;
        var total  = D.Size - rem;
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
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            vector[i] = left.Values[i] - right.Values[i];
        }        
        return new(Arr.create(vector));
    }
    
    /// <summary>
    /// Returns a new vector whose values are the product of each pair of elements in two specified vectors.
    /// </summary>
    public static Vector<D, A> operator *(Vector<D, A> left, Vector<D, A> right) 
    {
        var vector = new A[D.Size];
        var rem    = D.Size % Vector<A>.Count;
        var total  = D.Size - rem;
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
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            vector[i] = left.Values[i] * right.Values[i];
        }        
        return new(Arr.create(vector));
    }

    public static Vector<D, A> operator *(Vector<D, A> left, A right) 
    {
        var vector = new A[D.Size];
        var rem    = D.Size % Vector<A>.Count;
        var total  = D.Size - rem;
        var larray = left.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            (v * right).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            vector[i] = left.Values[i] * right;
        }        
        return new(Arr.create(vector));
    }

    public static Vector<D, A> operator /(Vector<D, A> left, A right) 
    {
        var vector = new A[D.Size];
        var rem    = D.Size % Vector<A>.Count;
        var total  = D.Size - rem;
        var larray = left.Values;

        // Perform the operation using SIMD intrinsics
        for (var i = 0; i < total; i += Vector<A>.Count)
        {
            var v = new Vector<A>(larray.AsSpan(i, Vector<A>.Count));
            (v / right).CopyTo(vector, i);
        }

        // Perform the remainder of the operation that couldn't fit into a SIMD intrinsic
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            vector[i] = left.Values[i] / right;
        }        
        return new(Arr.create(vector));
    }

    /// <summary>
    /// Calculate the dot product between two vectors 
    /// </summary>
    public A Dot(Vector<D, A> rhs) =>
        (this * rhs).Sum();

    /// <summary>
    /// Calculate of all values in the vector
    /// </summary>
    public A Sum()
    {
        var rem   = D.Size % 16;
        var total = D.Size - rem;
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
        for (var i = D.Size - rem; i < D.Size; i++)
        {
            sum += array[i];
        }        
        return sum;
    }

    public override string ToString() => 
        Values.ToFullArrayString();
}
