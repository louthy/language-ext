namespace LanguageExt.Megaparsec;

public static class ParseError
{
    public static ParseError<T, E> Trivial<T, E>(
        int offset, 
        Option<ErrorItem<T>> unexpected, 
        Set<ErrorItem<T>> expected) =>
        new ParseError<T, E>.Trivial(offset, unexpected, expected);
    
    public static ParseError<T, E> Fancy<T, E>(int offset, Set<ErrorFancy<E>> errors) => 
        new ParseError<T, E>.Fancy(offset, errors);
}
