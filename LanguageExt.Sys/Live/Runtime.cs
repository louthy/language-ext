using System;
using System.Diagnostics;
using System.Text;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live IO runtime
/// </summary>
public record Runtime<M>(RuntimeEnv Env) : 
    Has<M, ActivitySourceIO>,
    Mutates<M, Runtime<M>, ActivityEnv>,
    Has<M, ConsoleIO>,
    Has<M, FileIO>,
    Has<M, TextReadIO>,
    Has<M, TimeIO>,
    Has<M, EnvironmentIO>,
    Has<M, DirectoryIO>
    where M : State<M, Runtime<M>>, Monad<M>
{
    /// <summary>
    /// Constructor function
    /// </summary>
    public static Runtime<M> New() =>
        new (new RuntimeEnv(ActivityEnv.Default, EnvIO.New(), Encoding.Default));

    public Runtime<M> WithActivity(Activity? activity) =>
        new(Env with
                {
                    Activity = Env.Activity with { Activity = activity, ParentId = Env.Activity.Activity?.Id ?? "" }
                });

    public Activity? CurrentActivity =>
        Env.Activity.Activity;

    public Runtime<M> WithIO(EnvIO envIO) =>
        new (Env with { EnvIO = envIO });

    public EnvIO EnvIO =>
        Env.EnvIO;

    /// <summary>
    /// Activity
    /// </summary>
    K<M, ActivitySourceIO> Has<M, ActivitySourceIO>.Trait =>
        M.Gets(rt => new Implementations.ActivitySourceIO(rt.Env.Activity));

    /// <summary>
    /// Modify the activity state
    /// </summary>
    public K<M, Unit> Modify(Func<ActivityEnv, ActivityEnv> f) =>
        M.Modify(rt => rt with { Env = rt.Env with { Activity = f(rt.Env.Activity) } });

    /// <summary>
    /// Read the activity state
    /// </summary>
    public K<M, ActivityEnv> Get { get; } =
        M.Gets(rt => rt.Env.Activity);

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    K<M, ConsoleIO> Has<M, ConsoleIO>.Trait { get; } =
        M.Pure(Implementations.ConsoleIO.Default);

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    K<M, FileIO> Has<M, FileIO>.Trait { get; } =
        M.Pure(Implementations.FileIO.Default);

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    K<M, TextReadIO> Has<M, TextReadIO>.Trait { get; } =
        M.Pure(Implementations.TextReadIO.Default);
 
    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    K<M, TimeIO> Has<M, TimeIO>.Trait { get; } =
        M.Pure(Implementations.TimeIO.Default);

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    K<M, EnvironmentIO> Has<M, EnvironmentIO>.Trait { get; } =
        M.Pure(Implementations.EnvironmentIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    K<M, DirectoryIO> Has<M, DirectoryIO>.Trait { get; } =
        M.Pure(Implementations.DirectoryIO.Default);
}

public record RuntimeEnv(ActivityEnv Activity, EnvIO EnvIO, Encoding Encoding) : IDisposable
{
    public void Dispose()
    {
        Activity.Dispose();
        EnvIO.Dispose();
    }
}
