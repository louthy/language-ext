using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace EffectsExamples
{
    public class Menu<RT>
        where RT : struct,
        HasCancel<RT>,
        HasTime<RT>,
        HasConsole<RT> 
    {
        public static Aff<RT, Unit> menu =>
            repeat(from __0 in clearConsole(ConsoleColor.Green)
                   from __1 in showOptions
                   from key in Console<RT>.readKey
                   from __2 in clearConsole(ConsoleColor.White)
                   from __3 in runExample(key.KeyChar - 48) 
                   select unit);
 
        static Aff<RT, Seq<Unit>> showOptions =>
            menuItems.Sequence(p => Console<RT>.writeLine($"{p.Item}. {p.Text}"));

        static Aff<RT, Unit> clearConsole(ConsoleColor color) =>
            from _   in Console<RT>.clear
            from __4 in Console<RT>.setColor(color)
            select unit;

        static Aff<RT, Unit> runExample(int ix) =>
            from exa in findExample(ix)
            from __0 in Console<RT>.setColor(ConsoleColor.Yellow)
            from __1 in Console<RT>.writeLine(exa.Desc)
            from __2 in Console<RT>.setColor(ConsoleColor.White)
            from res in exa.Example | @catch(unit)
            from __3 in showComplete
            select res;

        static Aff<RT, (Aff<RT, Unit> Example, string Desc)> findExample(int ix) =>
            menuItems.Find(item => item.Item == ix)
                     .Map(item => (Example: item.Example, Desc: item.Desc))
                     .ToAff()
          | @catch((SuccessAff<RT, Unit>(unit), "invalid menu option"));
        
        static Aff<RT, Unit> showComplete =>
            from _0 in Console<RT>.setColor(ConsoleColor.Cyan)
            from _1 in Console<RT>.writeLine("Returning to menu in 3")
            from _2 in Time<RT>.sleepFor(1 * second)
            from _3 in Console<RT>.writeLine("Returning to menu in 2")
            from _4 in Time<RT>.sleepFor(1 * second)
            from _5 in Console<RT>.writeLine("Returning to menu in 1")
            from _6 in Time<RT>.sleepFor(1 * second)
            select unit;
        
        static Seq<(int Item, Aff<RT, Unit> Example, string Text, string Desc)> menuItems =>
            Seq(
                (1, ErrorAndGuardExample<RT>.main.ToAff(), "Error handling and guards example", "Repeats the text you type in until you press Enter on an empty line, which will write a UserExited error, or 'sys' that will throw a SystemException, or 'err' that will throw an Exception"),
                (2, ForkCancelExample<RT>.main, "Process forking and cancelling example", "Forks a process that runs 10 times, summing a value each time.  If you press enter before the 10 iterations then the forked process will be cancelled"),
                (3, TimeoutExample<RT>.main, "Process timeout example", "Repeats a backing off process for 1 minutes, the back-off follows the fibonacci sequence in terms of the delay"),
                (4, TimeExample<RT>.main, "Clock example", "Prints the time every second for 15 seconds")
            );
    }
}
