namespace LanguageExt
{
    public struct Unit
    {
        public static readonly Unit Default = new Unit();

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
