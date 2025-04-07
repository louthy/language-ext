using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace TestBed;

internal class UseTest
{
    public static Task Main() =>
        Method1();

    static async Task Method1()
    {
        var op = from disposable in Prelude.use(() => new Disposable1())
                 from _1 in Method2()
                 from _2 in IO.lift(() => Console.WriteLine($"Do hard work: {disposable.Value}"))
                 select Unit.Default;

        await op.RunAsync();

        Console.WriteLine("IO completed");
    }

    static IO<Unit> Method2() =>
        IO.liftAsync(() => Task.Delay(TimeSpan.FromSeconds(1)).ToUnit());
}

public sealed class Disposable1 : IDisposable
{
    volatile int Disposed;

    public string Value =>
        Disposed == 0
            ? "This is a valid value"
            : throw new ObjectDisposedException(nameof(Disposable1));

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
        {
            Console.WriteLine("Disposed");
        }
        else
        {
            Console.WriteLine("Already disposed");
        }
    }
}
