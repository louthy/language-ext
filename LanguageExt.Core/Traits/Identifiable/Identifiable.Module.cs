namespace LanguageExt.Traits;

/// <summary>
/// Identifiable module
/// </summary>
public static class Identifiable
{
    /// <summary>
    /// Identify the structure
    /// </summary>
    /// <param name="fa">Structure to label</param>
    /// <param name="label">Label to apply</param>
    /// <typeparam name="F">Structure</typeparam>
    /// <typeparam name="L">Identifier type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Identified structure</returns>
    public static K<F, A> identify<F, L, A>(K<F, A> fa, L label) 
        where F : Identifiable<F, L> =>
        F.Identify(fa, new Label<L>(label));
    
    /// <summary>
    /// Identify the structure
    /// </summary>
    /// <param name="fa">Structure to label</param>
    /// <param name="label">Label to apply</param>
    /// <typeparam name="F">Structure</typeparam>
    /// <typeparam name="L">Identifier type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Identified structure</returns>
    public static K<F, A> identify<F, L, A>(K<F, A> fa, Label<L> label) 
        where F : Identifiable<F, L> =>
        F.Identify(fa, label);    
}
