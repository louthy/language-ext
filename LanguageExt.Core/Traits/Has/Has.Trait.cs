namespace LanguageExt.Traits;

/// <summary>
/// Makes a value accessible via a property.  This allows for _structural property access_. 
/// </summary>
/// <typeparam name="M">Higher-kind that supports access to the trait</typeparam>
/// <typeparam name="VALUE">Type to return as the bound-value</typeparam>
public interface Has<in M, VALUE>
{
    public static abstract K<M, VALUE> Ask { get; }
}
