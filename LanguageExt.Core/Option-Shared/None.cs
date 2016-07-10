namespace LanguageExt
{
    /// <summary>
    /// Option None state type
    /// </summary>
    /// <typeparam name="A">Bound value type - not used for None</typeparam>
    internal class None<A> : OptionV<A>
    {
        internal None()
        { }

        public override string ToString() =>
            $"None";

        public override int GetHashCode() =>
            0;
    }
}
