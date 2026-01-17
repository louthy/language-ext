using System.Threading;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    internal class OnceOnly<A>(IteratorIO<A> iter) : IteratorIO<A>
    {
        int cached;
        A? head;
        IteratorIO<A>? tail;

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            IO.liftVAsync(async e =>
                          {
                              // Naive implementation, consider alternatives. 
                              
                              SpinWait sw = default;
                              while (true)
                              {
                                  if (cached == 2)
                                  {
                                      if (tail is null)
                                      {
                                          return Head.NilIO<A>();
                                      }
                                      else
                                      {
                                          return Head.ExistIO(head!, tail);
                                      }
                                  }

                                  if (Interlocked.CompareExchange(ref cached, 1, 0) == 0)
                                  {
                                      try
                                      {
                                          var xs = await iter.NextIO().RunAsync(e);
                                          if (xs is (Exist<A> (var h), var t))
                                          {
                                              head = h;
                                              tail = t;
                                              cached = 2;
                                              return Head.ExistIO(head, tail.OnceOnly());
                                          }
                                          else
                                          {
                                              iter.Dispose();
                                              return Head.NilIO<A>();
                                          }
                                      }
                                      catch
                                      {
                                          cached = 0;
                                          throw;
                                      }
                                  }
                                  else
                                  {
                                      // Another thread must be in the CompareExchange section, but hasn't yet
                                      // assigned the cached value.  So, we wait...
                                      sw.SpinOnce();
                                  }
                              }
                          });

        public override void Dispose() =>
            iter.Dispose();

        public override string ToString() =>
            cached switch
            {
                0                   => "OnceOnly [...]",
                _ when tail is null => "OnceOnly []",
                _ when head is null => $"OnceOnly null :: {tail}",
                _                   => $"OnceOnly {head} :: {tail}",
            };

        public override IteratorIO<A> Using() =>
            new OnceOnly<A>(iter.Using());
    }
}
