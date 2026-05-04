using DomainTypesExamples.Capabilities;
using DomainTypesExamples.Roots;
using DomainTypesExamples.ValueObjects;
using DomainTypesExamples.ValueObjects.Scalars;
using DomainTypesExamples.ValueObjects.Spaces;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

var time = new DefaultTimeProvider(TimeProvider.System);
var sequence = new InMemorySequenceIO();
var random = new InMemoryRandomIO();

var runtime = new Runtime(time, sequence, random);

GenerateWorkPeriod<Runtime> generatePeriod =
    overtime => from effHours in nextRandom<Runtime>(min: 1, max: 4)
                let restHours = 4 - effHours
                from effective in FinT.lift<Eff<Runtime>, WorkDuration>(WorkDuration.From((effHours + overtime, 0)))
                from rest in FinT.lift<Eff<Runtime>, WorkDuration>(WorkDuration.From((restHours, 0)))
                select (effective, rest);

GenerateWorkBlocks generateBlock =
    (start, effectHours, restHours) =>
        from effective in WorkBlock.Effective.From((start, effectHours))
        from rest in WorkBlock.Rest.From((effective.Finish, restHours))
        select (Seq<WorkBlock>(effective, rest), rest.Finish);

GenerateWorkDay<Runtime> generateBlockDay =
    (overtime) =>
        from start in FinT.lift<Eff<Runtime>, HourOnly>(HourOnly.From((9, 0)))
        from startPeriod in generatePeriod(0).Bind(d => generateBlock(start, d.Work, d.Rest1))
        from middleBlock in WorkBlock.Lunch.From(startPeriod.Finished)
        from finishedBlock in generatePeriod(overtime).Bind(d => generateBlock(middleBlock.Finish, d.Work, d.Rest1))
        from currentDate in NonFutureDate.GetNow<Runtime>()

        let blocks = Seq<WorkBlock>([.. startPeriod.Blocks, middleBlock, .. finishedBlock.Blocks])
        from day in WorkDay.From((currentDate, blocks))
        select day;

GenerateDetailUser detailUser =
    user =>
    {
        var (id, name, createdAt, workDays) = user;

        Console.WriteLine($"User: {name} (Id: {id})");
        Console.WriteLine($"Created at: {createdAt}");
        Console.WriteLine($"Work days: {workDays.Count}");

        foreach (var day in workDays)
        {
            var (at, blocks) = day.To();

            Console.WriteLine($"  Date: {at}");
            Console.WriteLine($"    Blocks: {blocks.Count}");
            Console.WriteLine($"    Tracked: {day.TrackedDuration}");
            Console.WriteLine($"    Effective: {day.EffectiveDuration}");
            Console.WriteLine($"    Overtime: {day.Overtime}");
        }

        Console.WriteLine(new string('-', 40));

        return IO.pure(user);
    };


var process = from hernan in User.Factory<Runtime>.FromM("Hernán Álvarez")
              from workDayHernan1 in generateBlockDay(0)
              from hernanAfterWork1 in detailUser(hernan with
              {
                  WorkDays = hernan.WorkDays.Add(workDayHernan1)
              })
              from workDayHernan2 in generateBlockDay(2)
              from _1 in detailUser(hernanAfterWork1 with
              {
                  WorkDays = hernanAfterWork1.WorkDays.Add(workDayHernan2)
              })

              from leon in User.Factory<Runtime>.FromM("León Casavant")
              from workDayLeon1 in generateBlockDay(0)
              from leonAfterWork1 in detailUser(leon with
              {
                  WorkDays = leon.WorkDays.Add(workDayLeon1)
              })
              from workDayLeon2 in generateBlockDay(5)
              from _2 in detailUser(leonAfterWork1 with
              {
                  WorkDays = leonAfterWork1.WorkDays.Add(workDayLeon2)
              })

              select unit;

var result = process.Run().Run(runtime).Flatten()
    .Match(Succ: _ =>
        {
            Console.WriteLine("Proceso completado");
        },
        Fail: f =>
        {
            foreach (var error in f.AsIterable())
            {
                Console.WriteLine(error.Message);
            }
        });

return;

public delegate IO<User> GenerateDetailUser(User user);

public delegate FinT<Eff<RT>, WorkDay> GenerateWorkDay<RT>(int overtime);

public delegate FinT<Eff<RT>, (WorkDuration Work, WorkDuration Rest1)> GenerateWorkPeriod<RT>(int overtime);

public delegate Fin<(Seq<WorkBlock> Blocks, HourOnly Finished)> GenerateWorkBlocks(HourOnly start, WorkDuration work, WorkDuration rest);

public sealed class Runtime(TimeIO time, SequencesIO sequences, RandomIO random) :
    HasTime<Runtime>, HasSequences<Runtime>, HasRandom<Runtime>
{
    public TimeIO Time { get; } = time;

    public SequencesIO Sequences { get; } = sequences;

    public RandomIO Random { get; } = random;

    static K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Ask =>
        liftEff((Runtime rt) => rt.Time);

    static K<Eff<Runtime>, SequencesIO> Has<Eff<Runtime>, SequencesIO>.Ask =>
        liftEff((Runtime rt) => rt.Sequences);

    static K<Eff<Runtime>, RandomIO> Has<Eff<Runtime>, RandomIO>.Ask =>
        liftEff((Runtime rt) => rt.Random);
}
