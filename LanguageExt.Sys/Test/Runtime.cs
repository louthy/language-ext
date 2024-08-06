using System;
using System.Text;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.Test;

/// <summary>
/// Test IO runtime
/// </summary>
public record Runtime(RuntimeEnv Env) : 
    Local<Eff<Runtime>, ActivityEnv>,
    Has<Eff<Runtime>, ActivitySourceIO>,
    Has<Eff<Runtime>, ConsoleIO>,
    Has<Eff<Runtime>, EncodingIO>,
    Has<Eff<Runtime>, FileIO>,
    Has<Eff<Runtime>, TextReadIO>,
    Has<Eff<Runtime>, TimeIO>,
    Has<Eff<Runtime>, EnvironmentIO>,
    Has<Eff<Runtime>, DirectoryIO>
{
    /// <summary>
    /// Constructor function
    /// </summary>
    public static Runtime New() =>
        new(RuntimeEnv.Default);
    /// <summary>
    /// Constructor function
    /// </summary>
    /// <param name="env">Data environment for the runtime.  Call `RuntimeEnv.Default with { ... }` to modify</param>
    public static Runtime New(RuntimeEnv env) =>
        new(env);

    static K<Eff<Runtime>, A> asks<A>(Func<Runtime, A> f) =>
        Readable.asks<Eff<Runtime>, Runtime, A>(f);
    
    static K<Eff<Runtime>, A> local<A>(Func<Runtime, Runtime> f, K<Eff<Runtime>, A> ma) =>
        Readable.local(f, ma);

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    static K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Ask => 
        asks<ConsoleIO>(rt => new Implementations.ConsoleIO(rt.Env.Console));

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    static K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Ask => 
        from n in Time<Eff<Runtime>, Runtime>.now
        from r in asks<FileIO>(rt => new Implementations.FileIO(rt.Env.FileSystem, n))
        select r;

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    static K<Eff<Runtime>, TextReadIO> Has<Eff<Runtime>, TextReadIO>.Ask => 
        asks<TextReadIO>(_ => Implementations.TextReadIO.Default);

    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    static K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Ask => 
        asks<TimeIO>(rt => new Implementations.TimeIO(rt.Env.TimeSpec));

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    static K<Eff<Runtime>, EnvironmentIO> Has<Eff<Runtime>, EnvironmentIO>.Ask => 
        asks<EnvironmentIO>(rt => new Implementations.EnvironmentIO(rt.Env.SysEnv));

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    static K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Ask =>
        from n in Time<Eff<Runtime>, Runtime>.now
        from r in asks<DirectoryIO>(rt => new Implementations.DirectoryIO(rt.Env.FileSystem, n))
        select r;

    static K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Ask =>
        asks<EncodingIO>(_ => Live.Implementations.EncodingIO.Default);

    static K<Eff<Runtime>, ActivitySourceIO> Has<Eff<Runtime>, ActivitySourceIO>.Ask => 
        asks<ActivitySourceIO>(rt => new Live.Implementations.ActivitySourceIO(rt.Env.Activity));

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
    
public record RuntimeEnv(
    EnvIO EnvIO,
    Encoding Encoding,
    MemoryConsole Console,
    MemoryFS FileSystem,
    Implementations.TestTimeSpec TimeSpec,
    MemorySystemEnvironment SysEnv,
    ActivityEnv Activity)
{
    public RuntimeEnv LocalCancel =>
        this with { EnvIO = EnvIO.LocalCancel };

    public static RuntimeEnv Default =>
        new(EnvIO.New(),
            Encoding.Default,
            new MemoryConsole(),
            new MemoryFS(),
            Implementations.TestTimeSpec.RunningFromNow(),
            MemorySystemEnvironment.InitFromSystem(),
            ActivityEnv.Default);
}
