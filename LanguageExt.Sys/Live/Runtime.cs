using System;
using System.Diagnostics;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live IO runtime
/// </summary>
public record Runtime(RuntimeEnv Env) : 
    Has<Eff<Runtime>, ActivitySourceIO>,
    Mutates<Eff<Runtime>, Runtime, ActivityEnv>,
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
        new (new RuntimeEnv(ActivityEnv.Default, EnvIO.New()));

    public Runtime WithActivity(Activity? activity) =>
        new(Env with
                {
                    Activity = Env.Activity with { Activity = activity, ParentId = Env.Activity.Activity?.Id ?? "" }
                });

    public Activity? CurrentActivity =>
        Env.Activity.Activity;

    public Runtime WithIO(EnvIO envIO) =>
        new (Env with { EnvIO = envIO });

    public EnvIO EnvIO =>
        Env.EnvIO;

    static K<Eff<Runtime>, A> gets<A>(Func<Runtime, A> f) =>
        Stateful.gets<Eff<Runtime>, Runtime, A>(f);
    
    static K<Eff<Runtime>, Unit> modify(Func<RuntimeEnv, RuntimeEnv> f) =>
        Stateful.modify<Eff<Runtime>, Runtime>(rt => rt with { Env = f(rt.Env) } );
    
    static K<Eff<Runtime>, A> pure<A>(A value) =>
        Eff<Runtime, A>.Pure(value);
    
    /// <summary>
    /// Activity
    /// </summary>
    K<Eff<Runtime>, ActivitySourceIO> Has<Eff<Runtime>, ActivitySourceIO>.Trait =>
        gets<ActivitySourceIO>(rt => new Implementations.ActivitySourceIO(rt.Env.Activity));

    /// <summary>
    /// Modify the activity state
    /// </summary>
    public K<Eff<Runtime>, Unit> Modify(Func<ActivityEnv, ActivityEnv> f) =>
        modify(rt => rt with { Activity = f(rt.Activity) });

    /// <summary>
    /// Read the activity state
    /// </summary>
    public K<Eff<Runtime>, ActivityEnv> Get { get; } =
        gets(rt => rt.Env.Activity);

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Trait { get; } =
        pure(Implementations.ConsoleIO.Default);

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Trait { get; } =
        pure(Implementations.FileIO.Default);

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    K<Eff<Runtime>, TextReadIO> Has<Eff<Runtime>, TextReadIO>.Trait { get; } =
        pure(Implementations.TextReadIO.Default);
 
    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Trait { get; } =
        pure(Implementations.TimeIO.Default);

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    K<Eff<Runtime>, EnvironmentIO> Has<Eff<Runtime>, EnvironmentIO>.Trait { get; } =
        pure(Implementations.EnvironmentIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Trait { get; } =
        pure(Implementations.DirectoryIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Trait { get; } =
        pure(Implementations.EncodingIO.Default);
}

public record RuntimeEnv(ActivityEnv Activity, EnvIO EnvIO) : IDisposable
{
    public void Dispose()
    {
        Activity.Dispose();
        EnvIO.Dispose();
    }
}
