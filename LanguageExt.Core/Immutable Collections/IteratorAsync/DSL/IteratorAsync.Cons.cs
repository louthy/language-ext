using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorAsync<A> 
{
    /// <summary>
    /// Cons iterator case.
    ///
    /// Contains a head value and a tail that represents the rest of the sequence.
    /// </summary>
    public abstract class Cons : IteratorAsync<A>
    {
        public void Deconstruct(out ValueTask<A> head, out ValueTask<IteratorAsync<A>> tail)
        {
            head = Head;
            tail = Tail;
        }
    }
}
