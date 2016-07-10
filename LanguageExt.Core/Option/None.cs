namespace LanguageExt
{
    /// <summary>
    /// Option None state type
    /// </summary>
    /// <typeparam name="A">Bound value type - not used for None</typeparam>
    public class None<A> : Option<A>
    {
        internal None()
        { }

        public override string ToString() =>
            $"None";

        public override int GetHashCode() =>
            0;
    }
}
