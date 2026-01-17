using System.Collections.Generic;

namespace LanguageExt;

public struct IteratorEnumeratorIO<A>
{
    readonly IteratorIO<A> reset;
    IteratorIO<A> iter;
    A? current;

    public IteratorEnumeratorIO(IteratorIO<A> iter)
    {
        this.reset = iter;
        this.iter = reset.Using();
    }
    
    public readonly A Current => 
        current!;

    public bool MoveNext()
    {
        if (iter.NextIO().Run() is (Exist<A> (var head), var tail))
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
        foreach (var x in iter)
        {
            yield return x;
        }
    }

    public void Reset()
    {
        iter.Dispose();
        iter = reset.Using();
    }

    public void Dispose() =>
        iter.Dispose();
}
