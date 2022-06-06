using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples
{
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
        where RT : struct, 
        HasCancel<RT>,
        HasConsole<RT>
    {
        public static Aff<RT, Unit> main()
        {
            // Create two queues.  Queues are Producers that have an Enqueue function
            var queue1 = Queue<RT, string>();
            var queue2 = Queue<RT, string>();

            // Compose the queues with a pipe that prepends some text to what they produce
            var queues = Seq(queue1 | prepend("Queue 1: "), queue2 | prepend("Queue 2: "));

            // Run the queues in a forked task
            // Repeatedly read from the console and write to one of the two queues depending on
            // whether the first char is 1 or 2
            return from c in fork(Producer.merge(queues) | writeLine)
                   from x in repeat(Console<RT>.readLines) | writeToQueue(queue1, queue2)
                   from _ in c // cancels the forked task
                   select unit;
        }

        // Consumer of the console.  It enqueues the item to queue1 or queue2 depending
        // on the first char of the string it awaits
        static Consumer<RT, string, Unit> writeToQueue(Queue<RT, string, Unit> queue1, Queue<RT, string, Unit> queue2) =>
            from x in awaiting<string>()
            from _ in x.HeadOrNone().Case switch
                      {
                          '1' => queue1.EnqueueEff(x.Substring(1)),
                          '2' => queue2.EnqueueEff(x.Substring(1)),
                          _   => FailEff<Unit>(Errors.Cancelled)
                      }
            select unit;
        
        /// <summary>
        /// Pipe that prepends the text provided to the awaited value and then yields it
        /// </summary>
        static Pipe<RT, string, string, Unit> prepend(string x) =>
            from l in awaiting<string>()         
            from _ in yield($"{x}{l}")
            select unit;
        
        /// <summary>
        /// Consumer that simply writes to the console
        /// </summary>
        static Consumer<RT, string, Unit> writeLine =>
            from l in awaiting<string>()
            from _ in Console<RT>.writeLine(l)
            select unit;
    }
}
