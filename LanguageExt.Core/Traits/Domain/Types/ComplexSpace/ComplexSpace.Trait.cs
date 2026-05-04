using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a complex-like domain type supporting affine transformations,
/// vector operations, and multiplicative composition.
/// </summary>
/// <typeparam name="SELF">The concrete complex space type.</typeparam>
public interface ComplexSpace<SELF> :
    AffineSpace<SELF, SELF, double>,
    VectorSpace<SELF, double>,
    IMultiplyOperators<SELF, SELF, SELF>
    where SELF : ComplexSpace<SELF>
{
    /// <summary>
    /// The imaginary unit of the complex space.
    /// 
    /// This value represents the fundamental basis element that satisfies:
    /// <code>
    /// I * I = -1
    /// </code>
    /// </summary>
    static abstract SELF I { get; }
}
