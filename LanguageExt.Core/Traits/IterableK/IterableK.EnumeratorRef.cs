using System.Collections;
using System.Collections.Generic;

namespace LanguageExt.Traits;

public ref struct IterableEnumeratorRef<F, FS, A>(K<F, A> ta) : IEnumerator<A>
    where F : IterableK<F, FS>
    where FS : allows ref struct
{
    FS foldState = F.StepSetup(ta);
    A? current;

    public bool MoveNext()
    {
        if (F.Step(ta, ref foldState, out var value))
        {
            current = value;
            return true;
        }
        else
        {
            return false;
        }
    }

    public IterableEnumeratorRef<F, FS, A> GetEnumerator() =>
        this;
    
    public void Reset() => 
        foldState = F.StepSetup(ta);

    object? IEnumerator.Current => 
        Current;

    public A Current => 
        current!;

    public void Dispose()
    {
    }
}
