namespace LanguageExt.Traits;

public ref struct IterableBackEnumeratorRef<F, FS, A>(K<F, A> ta)
    where F : IterableBackK<F, FS>
    where FS : allows ref struct
{
    FS foldState = F.StepBackSetup(ta);
    A? current;

    public bool MoveNext()
    {
        if (F.StepBack(ta, ref foldState, out var value))
        {
            current = value;
            return true;
        }
        else
        {
            return false;
        }
    }

    public IterableBackEnumeratorRef<F, FS, A> GetEnumerator() =>
        this;

    public void Reset() => 
        foldState = F.StepBackSetup(ta);

    public A Current => 
        current!;
}
