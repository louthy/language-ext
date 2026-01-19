using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

public struct IteratorAsyncEnumeratorIO<A> : IAsyncEnumerator<A>
{
    readonly IteratorIO<A> reset;
    readonly EnvIO envIO;
    IteratorIO<A> iter;
    A? current;

    public IteratorAsyncEnumeratorIO(IteratorIO<A> iter, EnvIO env)
    {
        reset = iter;
        envIO = env;
        this.iter = reset.Using();
    }

    public readonly A Current => 
        current!;

    public async ValueTask<bool> MoveNextAsync()
    {
        if (await iter.NextIO().RunAsync(envIO) is (Exist<A> (var head), var tail))
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
    
    public IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        using var env = EnvIO.New(token: cancellationToken);
        return GetAsyncEnumerator(env);
    }

    public async IAsyncEnumerator<A> GetAsyncEnumerator(EnvIO env)
    {
        using var iter1 = reset.Using();
        var       iter2 = iter1;
        while (await iter2.NextIO().RunAsync(env) is (Exist<A> head, var tail))
        {
            yield return head.Value;
            iter2 = tail;
        }
    }

    public void Reset()
    {
        iter.Dispose();
        iter = reset.Using();
    }

    public ValueTask DisposeAsync()
    {
        iter.Dispose();
        envIO.Dispose();
        return default;
    }
}
