using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys.Live;
using static LanguageExt.Prelude;

namespace TestBed;

public static class ResourcesDiscussion1366
{
    public static async Task Run()
    {
        Console.WriteLine("## 1. Expect");
        await Expect();

        Console.WriteLine("\n## 2. use using IO");
        await Case2();

        Console.WriteLine("\n## 3. use using IO w/ EnvIO");
        await Case3();

        Console.WriteLine("\n## 4. use using Eff");
        await Case4();

        Console.WriteLine("\n## 5. use using Eff w/ Runtime");
        await Case5();
    }


    static async Task Expect()
    {
        try
        {
            await using var x = new DisposableClass("1");
            throw new Exception("crash");
        }
        catch
        {
            // do nothing
        }

    }

    static async Task Case2()
    {
        var io =
            from r in use(() => new DisposableClass("2"))
            from x in liftIO(() => throw new Exception("crash"))
            from _ in release(r)
            select unit;

        try
        {
            await io.RunAsync();
        }
        catch
        {
            // do nothing
        }
    }

    static async Task Case3()
    {
        var io =
            from r in use(() => new DisposableClass("3"))
            from x in liftIO(() => throw new Exception("crash"))
            from _ in release(r)
            select unit;

        try
        {
            using var envIO = EnvIO.New();
            await io.RunAsync(envIO);
        }
        catch
        {
            // do nothing
        }
    }

    static async Task Case4()
    {
        Eff<Unit> effect =
            from r in use(() => new DisposableClass("4"))
            from x in liftIO(() => throw new Exception("crash"))
            from _ in release(r)
            select unit;

        await effect.RunAsync();
    }

    static async Task Case5()
    {
        Eff<Runtime, Unit> effect =
            from r in use(() => new DisposableClass("5"))
            from x in liftIO(() => throw new Exception("crash"))
            from _ in release(r)
            select unit;

        await effect.RunAsync(Runtime.New());
    }

    public class DisposableClass(string Id) : IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"- Disposed {Id}");
        }

        public ValueTask DisposeAsync()
        {
            Console.WriteLine($"- DisposedAsync {Id}");

            return ValueTask.CompletedTask;
        }
    }
}
