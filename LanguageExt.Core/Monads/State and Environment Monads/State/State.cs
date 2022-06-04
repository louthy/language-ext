#nullable enable

namespace LanguageExt
{
    public delegate (A Value, S? State, bool IsFaulted) State<S, A>(S state);
}
