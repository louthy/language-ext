namespace LanguageExt.Traits;

/// <summary>
/// Identifiable structure
/// </summary>
/// <typeparam name="F">Structure</typeparam>
/// <typeparam name="L">Identifier type</typeparam>
public interface Identifiable<F, L>
    where F : Identifiable<F, L>
{
    /// <summary>
    /// Identify the structure
    /// </summary>
    /// <param name="fa">Structure to label</param>
    /// <param name="label">Label to apply</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Identified structure</returns>
    public static abstract K<F, A> Identify<A>(K<F, A> fa, Label<L> label);
}
