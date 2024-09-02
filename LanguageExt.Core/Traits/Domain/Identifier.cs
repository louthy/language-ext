using System;
using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// One of the most common uses of domain types is a transparent handle for an entity or an asset in the real world,
/// such as a customer identifier in an online store or an employee number in a payroll application. I call these types
/// identifiers.
/// 
/// Identifiers have no structure, i.e., we don’t care about their internal representation. The only fundamental
/// requirement is the ability to compare values of those types for equality. This lack of structure suggests an
/// appropriate mathematical model for such types: a set, a collection of distinct objects.
/// </summary>
/// <typeparam name="SELF">Type implementing this interface</typeparam>
/// <typeparam name="REPR">Underlying representation</typeparam>
public interface Identifier<SELF, REPR> : 
    DomainType<SELF, REPR>,
    IEquatable<SELF>,
    IEqualityOperators<SELF, SELF, bool>
    where SELF : Identifier<SELF, REPR>;
