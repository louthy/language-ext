namespace LanguageExt;

/// <summary>
/// A unit type that represents an untyped empty collection.
///
/// This type can be implicitly converted any of the collection types in language-ext.
/// Use `Prelude.Empty` to access a constant instance of this type.
/// </summary>
public readonly struct UnitCollection
{
    public static readonly UnitCollection Default = new();
}
