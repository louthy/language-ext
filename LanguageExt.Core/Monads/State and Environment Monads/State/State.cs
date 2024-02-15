
using LanguageExt;

/// <summary>
/// State monad
/// </summary>
/// <param name="state"></param>
/// <typeparam name="S"></typeparam>
/// <typeparam name="A"></typeparam>
public record State<S, A>(StateT<S, Identity, A> state) 
    : StateT<S, Identity, A>(state.runState);
