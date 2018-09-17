using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Unit constructor
        /// </summary>
        public static Unit unit =>
            Unit.Default;

        /// <summary>
        /// Takes any value, ignores it, returns a unit
        /// </summary>
        /// <param name="anything">Value to ignore</param>
        /// <returns>Unit</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Unit ignore<A>(A anything) =>
            unit;
    }
}
