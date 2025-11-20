using System;
using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// A typical use of domain types is representing quantities, such as the amount of money in USD in a bank account,
/// or the file-size in bytes. Being able to compare, add, and subtract amounts is essential.
/// 
/// Generally, we cannot multiply or divide two compatible amounts and expect to get the amount of the same type back. 
///
/// Unless we’re modeling mathematical entities, such as probabilities or points on an elliptic curve.. Multiplying two
/// dollars by two dollars gives four squared dollars. I don’t know about you, but I’m yet to find a practical use for
/// squared dollars.
/// 
/// Multiplying amounts by a dimensionless number, however, is meaningful. There is nothing wrong with a banking app
/// increasing a dollar amount by ten percent or a disk utility dividing the total number of allocated bytes by the file
/// count.
/// 
/// The appropriate mathematical abstraction for amounts is vector spaces. Vector space is a set with additional
/// operations defined on the elements of this set: addition, subtraction, and scalar multiplication, such that
/// behaviors of these operations satisfy a few natural axioms.
/// </summary>
/// <remarks>This is the same as `VectorSpace` but with ordering</remarks>
/// <typeparam name="SELF">Type implementing this interface</typeparam>
/// <typeparam name="SCALAR">Scalar units</typeparam>
public interface Amount<SELF, SCALAR> :
    VectorSpace<SELF, SCALAR>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>
    where SELF : Amount<SELF, SCALAR>;
