namespace LanguageExt.Megaparsec;

public abstract record ErrorFancy<E>;
public record ErrorFail<E>(string Value) : ErrorFancy<E>;
public record ErrorIndentation<E>(int Ordering, int Reference, int Actual) : ErrorFancy<E>;
public record ErrorCustom<E>(E Value) : ErrorFancy<E>;
