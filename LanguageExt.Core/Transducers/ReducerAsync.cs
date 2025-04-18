using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reducer delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate Reduced<S> Reducer<in A, S>(S state, A input);

/// <summary>
/// ReducerAsync delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate ValueTask<Reduced<S>> ReducerAsync<in A, S>(S state, A input);

/// <summary>
/// ReducerM delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate K<M, S> ReducerM<in M, in A, S>(S state, A input)
    where M : Applicative<M>, Alternative<M>;
