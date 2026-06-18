namespace LanguageExt;

public abstract partial class Iterator 
{
    /// <summary>
    /// Yield a value forever
    /// </summary>
    internal class IterForever<A>(A value) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() => 
            Head.Exist(value, this);

        public override string ToString() => 
            $"Forever({value})";
    }
}
