using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples;

/// <summary>
/// Queue example
/// </summary>
/// <remarks>
/// Creates two queues.  Queues are Producers that have an Enqueue function.
/// The two producers are merged into a single producer and piped to a writeLine consumer to create an Effect
/// The effect is forked to run without awaiting the result
/// Another effect is created that listens to input from the user and pipes it to queue 1 or 2 depending on if the
/// text starts with a '1' or a '2'.
/// If the text starts with anything else, the effect is cancelled.
/// Then the fork is cancelled. 
/// </remarks>
/// <typeparam name="RT"></typeparam>
public class QueueExample<RT> 
    where RT : 
        Has<Eff<RT>, ConsoleIO>
{
    public static Eff<RT, Unit> main()
    {
        // Create two queues.  Queues are Producers that have an Enqueue function
        var queue1 = Queue<Eff<RT>, string>();
        var queue2 = Queue<Eff<RT>, string>();

        // Compose the queues with a pipe that prepends some text to what they produce
        var queues = Seq(queue1 | prepend("Queue 1: "), queue2 | prepend("Queue 2: "));

        // Run the queues in a forked task
        // Repeatedly read from the console and write to one of the two queues depending on
        // whether the first char is 1 or 2
        return from f in forkIO(Producer.merge(queues) | writeLine).As()
               from x in repeat(Console<Eff<RT>, RT>.readLines) | writeToQueue(queue1, queue2)
               from _ in f.Cancel // cancels the forked task
               select unit;
    }

    // Consumer of the console.  It enqueues the item to queue1 or queue2 depending
    // on the first char of the string it awaits
    static Consumer<string, Eff<RT>, Unit> writeToQueue(
        Queue<string, Eff<RT>, Unit> queue1, 
        Queue<string, Eff<RT>, Unit> queue2) =>
        from x in awaiting<string>()
        from u in guard(x.Length > 0, Error.New("exiting"))
        from _ in x[0] switch
                  {
                      '1' => queue1.EnqueueM(x.Substring(1)).As(),
                      '2' => queue2.EnqueueM(x.Substring(1)).As(),
                      _   => Fail(Errors.Cancelled)
                  }
        select unit;
        
    /// <summary>
    /// Pipe that prepends the text provided to the awaited value and then yields it
    /// </summary>
    static Pipe<string, string, Eff<RT>, Unit> prepend(string x) =>
        from l in awaiting<string>()         
        from _ in yield($"{x}{l}")
        select unit;
        
    /// <summary>
    /// Consumer that simply writes to the console
    /// </summary>
    static Consumer<string, Eff<RT>, Unit> writeLine =>
        from l in awaiting<string>()
        from _ in Console<Eff<RT>, RT>.writeLine(l)
        select unit;
}
