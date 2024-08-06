using System;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live IO runtime
/// </summary>
public record Runtime(RuntimeEnv Env) : 
    Local<Eff<Runtime>, ActivityEnv>,
    Has<Eff<Runtime>, ActivitySourceIO>,
    Has<Eff<Runtime>, ConsoleIO>,
    Has<Eff<Runtime>, FileIO>,
    Has<Eff<Runtime>, TextReadIO>,
    Has<Eff<Runtime>, TimeIO>,
    Has<Eff<Runtime>, EnvironmentIO>,
    Has<Eff<Runtime>, DirectoryIO>,
    Has<Eff<Runtime>, EncodingIO>
{
    /// <summary>
    /// Constructor function
    /// </summary>
    public static Runtime New() =>
        new (new RuntimeEnv(ActivityEnv.Default));

    static K<Eff<Runtime>, A> asks<A>(Func<Runtime, A> f) =>
        Readable.asks<Eff<Runtime>, Runtime, A>(f);

    static K<Eff<Runtime>, A> local<A>(Func<Runtime, Runtime> f, K<Eff<Runtime>, A> ma) =>
        Readable.local(f, ma);

    static K<Eff<Runtime>, A> pure<A>(A value) =>
        Eff<Runtime, A>.Pure(value);
    
    /// <summary>
    /// Activity
    /// </summary>
    static K<Eff<Runtime>, ActivitySourceIO> Has<Eff<Runtime>, ActivitySourceIO>.Ask =>
        asks<ActivitySourceIO>(rt => new Implementations.ActivitySourceIO(rt.Env.Activity));

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    static K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Ask { get; } =
        pure(Implementations.ConsoleIO.Default);

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    static K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Ask { get; } =
        pure(Implementations.FileIO.Default);

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    static K<Eff<Runtime>, TextReadIO> Has<Eff<Runtime>, TextReadIO>.Ask { get; } =
        pure(Implementations.TextReadIO.Default);
 
    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    static K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Ask { get; } =
        pure(Implementations.TimeIO.Default);

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    static K<Eff<Runtime>, EnvironmentIO> Has<Eff<Runtime>, EnvironmentIO>.Ask { get; } =
        pure(Implementations.EnvironmentIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    static K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Ask { get; } =
        pure(Implementations.DirectoryIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    static K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Ask { get; } =
        pure(Implementations.EncodingIO.Default);

    /// <summary>
    /// Run with a local ActivityEnv 
    /// </summary>
    static K<Eff<Runtime>, A> Local<Eff<Runtime>, ActivityEnv>.With<A>(Func<ActivityEnv, ActivityEnv> f, K<Eff<Runtime>, A> ma) => 
        local(rt => rt with { Env = rt.Env with { Activity = f(rt.Env.Activity) } }, ma);

    /// <summary>
    /// Read the current ActivityEnv
    /// </summary>
    static K<Eff<Runtime>, ActivityEnv> Has<Eff<Runtime>, ActivityEnv>.Ask =>
        asks(rt => rt.Env.Activity);
}

public record RuntimeEnv(ActivityEnv Activity) : IDisposable
{
    public void Dispose() => 
        Activity.Dispose();
}
