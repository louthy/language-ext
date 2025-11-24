namespace LanguageExt.Megaparsec;

public static class ErrorFancy
{
    public static ErrorFancy<E> Fail<E>(string value) => 
        new ErrorFancy<E>.Fail(value);
    
    public static ErrorFancy<E> Indentation<E>(int ordering, int reference, int actual) => 
        new ErrorFancy<E>.Indentation(ordering, reference, actual);
    
    public static ErrorFancy<E> Custom<E>(E value) => 
        new ErrorFancy<E>.Custom(value);
}
