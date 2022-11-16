namespace LanguageExt
{
    public static class Prism
    {
        /// <summary>
        /// Identity lens
        /// </summary>
        public static Prism<A, A> identity<A>() =>
            Prism<A, A>.New(
                Get: a => a,
                Set: a => _ => a);
    }
}
