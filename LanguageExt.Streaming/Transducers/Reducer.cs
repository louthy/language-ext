using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reducer delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate Reduced<S> Reducer<in A, S>(S state, A input);
