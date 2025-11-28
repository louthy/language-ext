namespace LanguageExt.Megaparsec;

/// <summary>
/// Precomputed expected errors
/// </summary>
/// <remarks>
/// By precomputing expected errors, we can avoid creating a lot of additional memory allocations for common
/// parser combinators.
/// </remarks>
public static class ExpectedErrors
{
    public static readonly Set<ErrorItem<char>> newline =
        Set.singleton(ErrorItem.Label<char>("newline"));
    
    public static readonly Set<ErrorItem<char>> tab =
        Set.singleton(ErrorItem.Label<char>("tab"));
    
    public static readonly Option<string> whiteSpace =
        "white space";
    
    public static readonly Set<ErrorItem<char>> whiteSpaceChar =
        Set.singleton(ErrorItem.Label<char>("white space"));
    
    public static readonly Set<ErrorItem<char>> upperChar =
        Set.singleton(ErrorItem.Label<char>("uppercase letter"));
    
    public static readonly Set<ErrorItem<char>> lowerChar =
        Set.singleton(ErrorItem.Label<char>("lowercase letter"));
    
    public static readonly Set<ErrorItem<char>> letterChar =
        Set.singleton(ErrorItem.Label<char>("letter"));
    
    public static readonly Set<ErrorItem<char>> alphaNumChar =
        Set.singleton(ErrorItem.Label<char>("alphanumeric character"));
    
    public static readonly Set<ErrorItem<char>> digitChar =
        Set.singleton(ErrorItem.Label<char>("digit"));
    
    public static readonly Set<ErrorItem<char>> binaryDigitChar =
        Set.singleton(ErrorItem.Label<char>("binary digit"));
    
    public static readonly Set<ErrorItem<char>> hexDigitChar =
        Set.singleton(ErrorItem.Label<char>("hexadecimal digit"));
    
    public static readonly Set<ErrorItem<char>> numberChar =
        Set.singleton(ErrorItem.Label<char>("numeric character"));
    
    public static readonly Set<ErrorItem<char>> symbolChar =
        Set.singleton(ErrorItem.Label<char>("symbol"));
    
    public static readonly Set<ErrorItem<char>> punctuationChar =
        Set.singleton(ErrorItem.Label<char>("punctuation"));

    public static readonly Set<ErrorItem<char>> control =
        Set.singleton(ErrorItem.Label<char>("control character"));

    public static readonly Set<ErrorItem<char>> separator =
        Set.singleton(ErrorItem.Label<char>("separator"));
}
