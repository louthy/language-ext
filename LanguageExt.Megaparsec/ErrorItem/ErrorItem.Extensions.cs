using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ErrorItemExtensions
{
    public static ErrorItem<T> As<T>(this K<ErrorItem, T> ea) =>
        (ErrorItem<T>)ea;
}
