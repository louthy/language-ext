namespace LanguageExt;

/// <summary>
/// Label to be used with Identifiable structures
/// </summary>
/// <param name="Value">Label value</param>
/// <typeparam name="L">Label type</typeparam>
public readonly record struct Label<L>(L Value);
