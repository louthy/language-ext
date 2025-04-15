using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override SourceIterator<A> GetIterator()
    {
        var channel = Channel.CreateUnbounded<A>();

        var envIO = EnvIO.New();
        Sources.Traverse(
                    s => s.Reduce(unit, async (_, x) =>
                                        {
                                            if (envIO.Token.IsCancellationRequested)
                                                return Reduced.Done(unit);
                                            await channel.Writer.WriteAsync(x);
                                            return envIO.Token.IsCancellationRequested
                                                       ? Reduced.Done(unit)
                                                       : Reduced.Continue(unit);
                                        })
                          .Fork())
               .Run(envIO)
               .Strict();
        
        return new MergedReaderSourceIterator<A>(channel.Reader, envIO);
    }
}
