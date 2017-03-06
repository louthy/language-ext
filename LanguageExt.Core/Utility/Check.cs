namespace LanguageExt
{
    internal static class Check
    {
        internal static T NullReturn<T>(T value) =>
            Prelude.isnull(value)
                ? Prelude.raise<T>(new ResultIsNullException())
                : value;
    }
}
