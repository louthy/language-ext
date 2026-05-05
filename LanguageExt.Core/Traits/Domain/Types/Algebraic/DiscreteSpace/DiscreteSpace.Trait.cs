using System;
using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a domain set: a domain type with explicit equality semantics.
/// </summary>
/// <typeparam name="SELF">
/// The concrete domain set type.
/// </typeparam>
public interface DiscreteSpace<SELF> : 
    IEquatable<SELF>,
    IEqualityOperators<SELF, SELF, bool>
    where SELF : DiscreteSpace<SELF>;
