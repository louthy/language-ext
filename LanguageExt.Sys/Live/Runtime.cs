using System;
using System.Diagnostics;
using System.Text;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live;

/// <summary>
/// Live IO runtime
/// </summary>
public record Runtime(RuntimeEnv Env) : 
    HasActivitySource<Runtime>,
    HasConsole<Runtime>,
    HasFile<Runtime>,
    HasTextRead<Runtime>,
    HasTime<Runtime>,
    HasEnvironment<Runtime>,
    HasDirectory<Runtime>
{
    /// <summary>
    /// Constructor function
    /// </summary>
    public static Runtime New() =>
        new (new RuntimeEnv(ActivityEnv.Default, EnvIO.New(), Encoding.Default));

    /// <summary>
    /// Get encoding
    /// </summary>
    /// <returns></returns>
    public Encoding Encoding =>
        Env.Encoding;

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    public Eff<Runtime, Traits.ConsoleIO> ConsoleEff =>
        Pure(ConsoleIO.Default);

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    public Eff<Runtime, Traits.FileIO> FileEff =>
        Pure(FileIO.Default);

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    public Eff<Runtime, Traits.DirectoryIO> DirectoryEff =>
        Pure(DirectoryIO.Default);

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    public Eff<Runtime, Traits.TextReadIO> TextReadEff =>
        Pure(TextReadIO.Default);

    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    public Eff<Runtime, Traits.TimeIO> TimeEff  =>
        Pure(TimeIO.Default);

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    public Eff<Runtime, Traits.EnvironmentIO> EnvironmentEff =>
        Pure(EnvironmentIO.Default);

    public Eff<Runtime, Traits.ActivitySourceIO> ActivitySourceEff =>
        lift<Runtime, Traits.ActivitySourceIO>(rt => new ActivitySourceIO(rt.Env.Activity.ActivitySource));
        
    public Runtime WithActivity(Activity? activity) => 
        new (Env 
                 with {Activity = Env.Activity 
                                      with {Activity = activity, ParentId = Env.Activity.Activity?.Id ?? ""}});

    public Activity? CurrentActivity =>
        Env.Activity.Activity;

    public Runtime WithIO(EnvIO envIO) =>
        new (Env with { EnvIO = envIO });

    public EnvIO EnvIO =>
        Env.EnvIO;
}

public record RuntimeEnv(ActivityEnv Activity, EnvIO EnvIO, Encoding Encoding) : IDisposable
{
    public void Dispose()
    {
        Activity.Dispose();
        EnvIO.Dispose();
    }
}
