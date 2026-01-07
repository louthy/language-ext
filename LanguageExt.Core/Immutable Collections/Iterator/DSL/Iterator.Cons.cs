namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Cons iterator case.
    ///
    /// Contains a head value and a tail that represents the rest of the sequence.
    /// </summary>
    public abstract class Cons : Iterator<A>
    {
        public new void Deconstruct(out A head, out Iterator<A> tail)
        {
            head = Head;
            tail = Tail;
        }
    }
}
