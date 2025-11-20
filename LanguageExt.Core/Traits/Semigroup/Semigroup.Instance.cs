using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Contains the trait in record form.  This allows the trait to be passed
/// around as a value rather than resolved as a type.  It helps us get around limitations
/// in the C# constraint system.
/// </summary>
/// <param name="Combine">An associative binary operation.</param>
/// <typeparam name="A">Trait type</typeparam>
public record SemigroupInstance<A>(Func<A, A, A> Combine)
{
    /// <summary>
    /// The `A` type should derive from `Semigroup〈A〉`.  If so, we can get a `SemigroupInstance〈A〉` that we can
    /// pass around as a value.  If not, then we will get `None`, which means the type is not a semigroup.
    /// </summary>
    public static Option<SemigroupInstance<A>> Instance { get; } =
        // NOTE: I don't like this, but it's the only way I can think of to do ad hoc trait resolution
        Try.lift(GetInstance)
           .ToOption()
           .Bind(x => x is null ? None : Some(x));
    
    static SemigroupInstance<A>? GetInstance()
    {
        var type  = typeof(Semigroup<>).MakeGenericType(typeof(A));
        var prop  = type.GetProperty("Instance");
        var value = prop?.GetValue(null);
        return (SemigroupInstance<A>?)value;
    }    
}
