using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// ReducerAsync delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate IO<Reduced<S>> ReducerIO<in A, S>(S state, A input);
