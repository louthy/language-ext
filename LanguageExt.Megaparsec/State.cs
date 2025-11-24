namespace LanguageExt.Megaparsec;

public readonly record struct State<S, T, E>(
    S Input,
    int Offset,
    PosState<S> PosState,
    Seq<ParseError<T, E>> ParseErrors);
