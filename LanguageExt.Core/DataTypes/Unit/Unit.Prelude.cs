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
        public static Unit ignore<T>(T anything) =>
            unit;
    }
}
