using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// ReducerM delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate K<M, Reduced<S>> ReducerM<in M, in A, S>(S state, A input);
