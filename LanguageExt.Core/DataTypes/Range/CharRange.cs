using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class CharRange : Range<CharRange, TNumericChar, char>
    {
        public static readonly char Minus1 = unchecked((char)-1);

        CharRange(char min, char max) : base(min, max, (char)1) { }
        CharRange(char min, char max, char step) : base(min, max, step) { }

        /// <summary>
        /// Construct a new range
        /// </summary>
        /// <param name="from">The minimum value in the range</param>
        /// <param name="to">The maximum value in the range</param>
        public static CharRange FromMinMax(char min, char max) =>
            min > max
                ? new CharRange(min, max, Minus1)
                : new CharRange(min, max, (char)1);

        /// <summary>
        /// Construct a new range
        /// </summary>
        /// <param name="from">The minimum value in the range</param>
        /// <param name="count">The number of items in the range</param>
        public static CharRange FromCount(char from, int count) =>
            FromCount(from, (char)count, (char)1);
    }
}
