namespace LanguageExt.Traits;

/// <summary>
/// `Deriving` is an alias for `NaturalIso` 
/// </summary>
/// <remarks>
/// It is aliased to be more declarative when used in trait-implementations.  The idea is that your
/// `Supertype` is a wrapper around your `Subtype`.  Use of the `Deriving` trait, and the `Deriving.*` interfaces,
/// makes trait-implementations much easier.
/// </remarks>
/// <typeparam name="Supertype">Wrapper type that is a super-type wrapper of `Subtype`</typeparam>
/// <typeparam name="Subtype">`Subtype` of `Supertype`</typeparam>
public interface Deriving<Supertype, Subtype> : 
    NaturalIso<Supertype, Subtype>;
