using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// ReducerAsync delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate ValueTask<Reduced<S>> ReducerAsync<in A, S>(S state, A input);
