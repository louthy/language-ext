using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace EffectsExamples
{
    public class Menu<RT>
        where RT : struct,
        HasCancel<RT>,
        HasFile<RT>,
        HasTextRead<RT>,
        HasTime<RT>,
        HasConsole<RT>
    {
        public static Aff<RT, Unit> menu =>
            repeat(from __0 in clearConsole(ConsoleColor.Green)
                   from __1 in showOptions
                   from key in Console<RT>.readKey
                   from __2 in clearConsole(ConsoleColor.White)
                   from __3 in runExample(key.KeyChar) 
                   select unit);
 
        static Aff<RT, Seq<Unit>> showOptions =>
            menuItems.Sequence(p => Console<RT>.writeLine($"{p.Item}. {p.Text}"));

        static Aff<RT, Unit> clearConsole(ConsoleColor color) =>
            from _0 in Console<RT>.clear
            from _1 in Console<RT>.setColor(color)
            select unit;

        static Aff<RT, Unit> runExample(char ix) =>
            from exa in findExample(ix)
            from __0 in Console<RT>.setColor(ConsoleColor.Yellow)
            from __1 in Console<RT>.writeLine(exa.Desc)
            from __2 in Console<RT>.setColor(ConsoleColor.White)
            from res in localCancel(exa.Example) | @catch(logError)
            from __3 in showComplete(5)
            select res;

        static Aff<RT, (Aff<RT, Unit> Example, string Desc)> findExample(char ix) =>
            menuItems.Find(item => item.Item == ix)
                     .Map(item => (Example: item.Example, Desc: item.Desc))
                     .ToAff()
          | @catch((SuccessAff<RT, Unit>(unit), "invalid menu option"));

        static Aff<RT, Unit> logError(Error e) =>
            from _0 in Console<RT>.setColor(ConsoleColor.Red)
            from _1 in Console<RT>.writeLine($"{e}")
            from _2 in Console<RT>.setColor(ConsoleColor.Yellow)
            select unit;

        static Aff<RT, Unit> showComplete(int x) =>
            x == 0
                ? unitEff
                : from _0 in Console<RT>.setColor(ConsoleColor.Cyan)
                  from _1 in Console<RT>.writeLine($"Returning to menu in {x}")
                  from _2 in Time<RT>.sleepFor(1 * second)
                  from _3 in showComplete(x - 1)
                  select unit;
        
        static Seq<(char Item, Aff<RT, Unit> Example, string Text, string Desc)> menuItems =>
            Seq(
                ('1', ErrorAndGuardExample<RT>.main.ToAff(), "Error handling and guards example", "Repeats the text you type in until you press Enter on an empty line, which will write a UserExited error - this will be caught for a safe exit\nOr, 'sys' that will throw a SystemException - this will be caught and 'sys error' will be printed\nOr, 'err' that will throw an Exception - this will be caught to become 'there was a problem'"),
                ('2', ForkCancelExample<RT>.main, "Process forking and cancelling example", "Forks a process that runs 10 times, summing a value each time.\nIf you press enter before the 10 iterations then the forked process will be cancelled"),
                ('3', TimeoutExample<RT>.main, "Process timeout example", "Repeats a backing off process for 1 minutes\nThe back-off follows the fibonacci sequence in terms of the delay"),
                ('4', TimeExample<RT>.main, "Clock example", "Prints the time for 15 repetitions, the space between the prints follows the Fibonacci sequence up to 10 seconds\nand then it's clamped"),
                ('5', CancelExample<RT>.main, "Cancel example", "Accepts key presses and echos them to the console until Enter is pressed.\nWhen Enter is pressed it calls `cancel<RT>()` to trigger the cancellation token"),
                ('6', RetryExample<RT>.main, "Retry example", "Asks you to say hello.\nIf you don't type 'hello' then an error will be raised and it will retry."),
                ('7', QueueExample<RT>.main(), "Queue effect example", "Creates two queues.  Queues are Producers that have an Enqueue function.\nThe two producers are merged into a single producer and piped to a writeLine consumer to create an Effect\nThe effect is forked to run without awaiting the result\nAnother effect is created that listens to input from the user and pipes it to queue 1 or 2 depending on if the text starts with a '1' or a '2'.\nIf the text starts with anything else, the effect is cancelled.\nThen the fork is cancelled."),
                ('8', ClientServer<RT>.main, "Client / server effect example", "Simple request/response example.  The client sends 3 values to the server and it increments them"),
                ('9', FoldTest<RT>.main, "Pipe word folding example", "Folds keys from the keyboard into words, when a whitespace is encountered the folded word is yielded\ndown the pipe"),
                ('a', TextFileLineStreamExample<RT>.main, "Text file line streaming example", "Streams the contents of a text file, one line at a time"), 
                ('b', TextFileChunkStreamExample<RT>.main, "Text file chunk streaming example", "Streams the contents of a text file in chunks of 80 characters") 
            );
    }
}
