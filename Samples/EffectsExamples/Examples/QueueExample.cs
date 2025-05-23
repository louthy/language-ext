using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

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
public static class QueueExample<RT> 
    where RT : 
        Has<Eff<RT>, ConsoleIO>
{
    public static Eff<RT, Unit> main()
    {
        // Create two queues.  Queues are Producers that have an Enqueue function
        var queue1 = Conduit.make<string>();
        var queue2 = Conduit.make<string>();

        // Compose the queues with a pipe that prepends some text to what they produce
        var queues = Seq(queue1.ToProducer<RT>() | prepend("Queue 1: "), 
                         queue2.ToProducer<RT>() | prepend("Queue 2: "));

        // Run the queues in a forked task
        // Repeatedly read from the console and write to one of the two queues depending on
        // whether the first char is 1 or 2
        var effect = from f in fork(Producer.merge(queues) | writeLine).As()
                     from x in Console<RT>.readLines | writeToQueue(queue1, queue2) | Schedule.Forever
                     from _ in f.Cancel // cancels the forked task
                     select unit;

        return effect.As().Run();
    }

    // Consumer of the console.  It enqueues the item to queue1 or queue2 depending
    // on the first char of the string it awaits
    static Consumer<RT, string, Unit> writeToQueue(
        Conduit<string, string> queue1, 
        Conduit<string, string> queue2) =>
        from x in Consumer.awaiting<RT, string>()
        from u in guard(x.Length > 0, Error.New("exiting"))
        from _ in x[0] switch
                  {
                      '1' => queue1.Post(x.Substring(1)).As(),
                      '2' => queue2.Post(x.Substring(1)).As(),
                      _   => Fail(Errors.Cancelled)
                  }
        select unit;
        
    /// <summary>
    /// Pipe that prepends the text provided to the awaited value and then yields it
    /// </summary>
    static Pipe<RT, string, string, Unit> prepend(string x) =>
        from l in Pipe.awaiting<RT, string, string>()
        from _ in Pipe.yield<RT, string, string>($"{x}{l}")
        select unit;
        
    /// <summary>
    /// Consumer that simply writes to the console
    /// </summary>
    static Consumer<RT, string, Unit> writeLine =>
        from l in Consumer.awaiting<RT, string>()
        from _ in Console<RT>.writeLine(l)
        select unit;
}
