using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Constant value type-class
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    [Typeclass]
    public interface Const<TYPE> 
    {
        TYPE Value { get; }
    }
}
