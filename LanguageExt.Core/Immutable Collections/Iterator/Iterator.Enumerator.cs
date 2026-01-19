using System.Collections.Generic;

namespace LanguageExt;

public struct IteratorEnumerator<A>
{
    readonly Iterator<A> reset;
    Iterator<A> iter;
    A? current;

    public IteratorEnumerator(Iterator<A> iter)
    {
        this.reset = iter;
        this.iter = reset;
    }
    
    public readonly A Current => 
        current!;

    public bool MoveNext()
    {
        if (iter.Next() is (Exist<A> (var head), var tail))
        {
            iter = tail;
            current = head;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public IEnumerator<A> GetEnumerator()
    {
        foreach (var x in reset)
        {
            yield return x;
        }
    }

    public void Reset() => 
        iter = reset;
}
