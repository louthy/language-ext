using System.Threading;

namespace LanguageExt;

public abstract partial class Iterator
{
    internal class OnceOnly<A>(Iterator<A> iter) : Iterator<A>
    {
        int cached;
        A? head;
        Iterator<A>? tail;

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            SpinWait sw = default; 
            while (true)
            {
                if (cached == 2)
                {
                    if (tail is null)
                    {
                        return Head.Nil<A>();
                    }
                    else
                    {
                        return Head.Exist(head!, tail);
                    }
                }                
                
                if (Interlocked.CompareExchange(ref cached, 1, 0) == 0)
                {
                    try
                    {
                        if (iter is (Exist<A> (var h), var t))
                        {
                            head = h;
                            tail = t;
                            cached = 2;
                            return (new Exist<A>(head), tail.OnceOnly());
                        }
                        else
                        {
                            iter.Dispose();
                            return (Nil<A>.Default, Nil.Default);
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
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
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
                                          return Head.Nil<A>();
                                      }
                                      else
                                      {
                                          return Head.Exist(head!, tail);
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
                                              return (new Exist<A>(head), tail.OnceOnly());
                                          }
                                          else
                                          {
                                              iter.Dispose();
                                              return (Nil<A>.Default, Nil.Default);
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

        public override Iterator<A> Using() =>
            new OnceOnly<A>(iter.Using());
    }
}
