namespace LanguageExt.Traits;

/// <summary>
/// Makes a value accessible via a property
/// </summary>
/// <typeparam name="M"></typeparam>
/// <typeparam name="TRAIT"></typeparam>
public interface Has<in M, TRAIT>
{
    public static abstract K<M, TRAIT> Ask { get; }
}
