namespace DomainTypesExamples.Invariants;

/// <summary>
/// Base StringHas*Format invariant.
/// </summary>
/// <typeparam name="T">Type to assert format</typeparam>
public sealed class StringHasFormat<T> : Rule<StringHasFormat<T>, string>
    where T : IParsable<T>
{
    public static bool Check(string value) =>
        Guid.TryParse(value, out _);
}

/// <summary>
/// Validates that a given string has <see cref="Guid" /> format.
/// </summary>
public sealed class StringHasGuidFormat 
    : Rule.For<string>.Id<StringHasFormat<Guid>>,
      Rule<StringHasGuidFormat, string>;
