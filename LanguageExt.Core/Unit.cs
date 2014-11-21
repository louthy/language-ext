namespace LanguageExt
{
    public struct Unit
    {
        public static readonly Unit Default = new Unit();

        public override int GetHashCode() => 0;

        public override bool Equals(object obj) =>
            obj == null
                ? false
                : obj is Unit
                    ? true
                    : false;

        public override string ToString() => "[unit]";
    }
}
