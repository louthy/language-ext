using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSourceTIterator<M, A>(SourceTIterator<M, A> Left, SourceTIterator<M, A> Right) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read()
    {
        try
        {
            return Left.Read().Choose(() => Right.Read());
        }
        catch
        {
            return Right.Read();
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceTInternal.ReadyToRead([Left, Right], token);
}
