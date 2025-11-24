namespace LanguageExt;

/// <summary>
/// Identifiable module
/// </summary>
public static partial class Prelude
{
    /// <summary>
    /// Construct a label
    /// </summary>
    /// <param name="value">Label value</param>
    /// <typeparam name="L">Label value-type</typeparam>
    /// <returns></returns>
    public static Label<L> label<L>(L value) => 
        new(value);
}
