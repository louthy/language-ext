using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace IOExamples;

class Program
{
    static void Main(string[] args) =>
        infiniteIterator();
        //infiniteLoop(0).Run();

    static void infiniteIterator()
    {
        // NOTE: This should be run in Release mode, otherwise you might get a space leak
        
        for(var iter = Naturals.GetIterator(); !iter.IsEmpty; iter = iter.Tail)
        {
            if (iter.Head % 10000 == 0)
            {
                Console.WriteLine(iter.Head);
            }
        }
    }
    
    static IO<Unit> infiniteLoop(int value) =>
        from _ in value % 10000 == 0
                    ? writeLine($"{value}")
                    : Pure(unit)
        from r in tail(infiniteLoop(value + 1))
        select unit;

    static IO<Unit> infiniteLoop1(int value) =>
        (value % 10000 == 0
            ? writeLine($"{value}")
            : Pure(unit))
           .Bind(_ => infiniteLoop1(value + 1));
    
    static IO<int> recursiveAskForNumber =>
        from n in askForNumber(1)
        from _ in writeLine($"Your number is: {n}")
        select n;

    static IO<int> askForNumber(int attempts) =>
        from _ in writeLine($"Enter a number (attempt number: {attempts})")
        from l in readLine
        from n in int.TryParse(l, out var v)
                      ? Pure(v)
                      : from _ in writeLine("That's not a number!")
                        from r in askForNumber(attempts + 1)
                        select r
        select n;

    // static readonly Transducer<int, int> folder =
    //     foldUntil(Schedule.spaced(TimeSpan.FromMilliseconds(100)),
    //               0,
    //               (int s, int x) => s + x,
    //               valueIs: x => x % 10 == 0);
    //
    // static IO<Unit> folding =>
    //     from n in many(Range(1, 25))
    //     from _ in writeLine($"item flowing in: {n}")
    //     from s in n | folder
    //     from r in writeLine($"Total {s}")
    //     select unit;
    
    // static IO<Unit> retrying =>
    //     from ix in many(Range(1, 10))
    //     from _1 in retry(from _2 in writeLine($"Enter a number to add to {ix}")
    //                      from nm in readLine.Map(parseInt)
    //                      from _3 in guard(nm.IsSome, Error.New("Please enter a valid integer"))
    //                      from _4 in writeLine($"Number {ix} + {(int)nm} = {ix + (int)nm}")
    //                      select unit)
    //     from _4 in waitOneSecond
    //     select unit;

    static IO<Unit> repeating =>
        repeat(Schedule.recurs(5),
               from x in readNumber("Enter the first number to add")
               from y in readNumber("Enter the second number to add")
               from _ in writeLine($"{x} + {y} = {x + y}")
               from t in waitOneSecond
               select unit).As()
      | writeLine("Obviously you don't know what a number is so I'm off.");
    
    static IO<int> readNumber(string question) =>
        retry(Schedule.recurs(3),
              from _1 in writeLine(question)
              from nx in readLine.Map(int.Parse)
              select nx).As();
    
    static readonly IO<string> readLine =
        lift(() => Console.ReadLine() ?? "");
    
    static IO<Unit> writeLine(string line) =>
        lift(() =>
        {
            Console.WriteLine(line);
            return unit;
        });
    
    static IO<Unit> waitOneSecond =>
        wait(1000);

    static IO<Unit> wait(int milliseconds) =>
        yieldFor(milliseconds);

    static IO<DateTime> now =>
        lift(() => DateTime.Now);
}
