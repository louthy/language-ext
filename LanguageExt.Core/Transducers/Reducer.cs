namespace LanguageExt;

/// <summary>
/// Reducer delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate S Reducer<in A, S>(S state, A input);
