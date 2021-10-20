using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Alt*")]
    public interface Alternative<AltAB, A, B> : Typeclass
    {
        /// <summary>
        /// Identity
        /// </summary>
        AltAB Empty();

        /// <summary>
        /// As associative binary operation
        /// </summary>
        /// <param name="x">Left hand side</param>
        /// <param name="y">Right hand side</param>
        /// <returns>The result of the associative binary operation</returns>
        AltAB Append(AltAB x, AltAB y);
    }
}
