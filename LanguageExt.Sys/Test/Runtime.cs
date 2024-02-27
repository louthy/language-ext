using System;
using System.Text;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.Test;

/// <summary>
/// Test IO runtime
/// </summary>
public record Runtime(RuntimeEnv Env) : 
    Mutates<Eff<Runtime>, Runtime, ActivityEnv>,
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


    /// <summary>
    /// Get encoding
    /// </summary>
    /// <returns></returns>
    public Encoding Encoding =>
        Env.Encoding;

    public Runtime WithIO(EnvIO envIO) =>
        new(Env with { EnvIO = envIO });

    public EnvIO EnvIO =>
        Env.EnvIO;

    static K<Eff<Runtime>, A> gets<A>(Func<Runtime, A> f) =>
        StateM.gets<Eff<Runtime>, Runtime, A>(f);
    
    static K<Eff<Runtime>, Unit> modify(Func<RuntimeEnv, RuntimeEnv> f) =>
        StateM.modify<Eff<Runtime>, Runtime>(rt => new Runtime(Env: f(rt.Env)) );
    
    static K<Eff<Runtime>, A> pure<A>(A value) =>
        Eff<Runtime, A>.Pure(value);
    
    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Trait => 
        gets(rt => new Implementations.ConsoleIO(rt.Env.Console));

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Trait => 
        from n in Time<Eff<Runtime>, Runtime>.now
        from r in gets(rt => new Implementations.FileIO(rt.Env.FileSystem, n))
        select r;

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    K<Eff<Runtime>, TextReadIO> Has<Eff<Runtime>, TextReadIO>.Trait => 
        gets(_ => Implementations.TextReadIO.Default);

    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Trait => 
        gets(rt => new Implementations.TimeIO(rt.Env.TimeSpec));

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    K<Eff<Runtime>, EnvironmentIO> Has<Eff<Runtime>, EnvironmentIO>.Trait => 
        gets(rt => new Implementations.EnvironmentIO(rt.Env.SysEnv));

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Trait =>
        from n in Time<Eff<Runtime>, Runtime>.now
        from r in gets(rt => new Implementations.DirectoryIO(rt.Env.FileSystem, n))
        select r;

    K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Trait =>
        gets(_ => Live.Implementations.EncodingIO.Default);

    K<Eff<Runtime>, ActivitySourceIO> Has<Eff<Runtime>, ActivitySourceIO>.Trait => 
        gets(rt => new Live.Implementations.ActivitySourceIO(rt.Env.ActivityEnv));

    K<Eff<Runtime>, ActivityEnv> Reads<Eff<Runtime>, Runtime, ActivityEnv>.Get =>
        gets(rt => rt.Env.ActivityEnv);

    K<Eff<Runtime>, Unit> Mutates<Eff<Runtime>, Runtime, ActivityEnv>.Modify(Func<ActivityEnv, ActivityEnv> f) => 
        modify(rt => rt with { ActivityEnv = f(rt.ActivityEnv) });
}
    
public record RuntimeEnv(
    EnvIO EnvIO,
    Encoding Encoding,
    MemoryConsole Console,
    MemoryFS FileSystem,
    Implementations.TestTimeSpec TimeSpec,
    MemorySystemEnvironment SysEnv,
    ActivityEnv ActivityEnv)
{
    public RuntimeEnv LocalCancel =>
        this with { EnvIO = EnvIO.LocalCancel };

    public static readonly RuntimeEnv Default =
        new(EnvIO.New(),
            Encoding.Default,
            new MemoryConsole(),
            new MemoryFS(),
            Implementations.TestTimeSpec.RunningFromNow(),
            MemorySystemEnvironment.InitFromSystem(),
            ActivityEnv.Default);
}
