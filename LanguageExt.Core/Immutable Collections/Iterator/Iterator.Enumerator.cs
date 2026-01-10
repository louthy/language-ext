namespace LanguageExt;

public struct IteratorEnumerator<A>
{
    readonly Iterator<A> reset;
    Iterator<A> iter;
    A? current;

    public IteratorEnumerator(Iterator<A> iter)
    {
        this.reset = iter;
        this.iter = reset.Using();
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

    public void Reset()
    {
        iter.Dispose();
        iter = reset.Using();
    }

    public void Dispose() =>
        iter.Dispose();
}
