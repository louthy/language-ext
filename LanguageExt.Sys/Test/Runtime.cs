using System;
using System.Text;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys.Test;

/// <summary>
/// Test IO runtime
/// </summary>
public readonly struct Runtime<M> : 
    Has<M, ConsoleIO>,
    Has<M, FileIO>,
    Has<M, TextReadIO>,
    Has<M, TimeIO>,
    Has<M, EnvironmentIO>,
    Has<M, DirectoryIO>
    where M : Reader<M, Runtime<M>>, Monad<M>
{
    public readonly RuntimeEnv env;

    /// <summary>
    /// Constructor
    /// </summary>
    Runtime(RuntimeEnv env) =>
        this.env = env;

    /// <summary>
    /// Configuration environment accessor
    /// </summary>
    public RuntimeEnv Env =>
        env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

    /// <summary>
    /// Constructor function
    /// </summary>
    /// <param name="timeSpec">Defines how time works in the runtime</param>
    public static Runtime<M> New(Implementations.TestTimeSpec? timeSpec = default) =>
        new(new RuntimeEnv(EnvIO.New(),
                           Encoding.Default,
                           new MemoryConsole(),
                           new MemoryFS(),
                           timeSpec ?? Implementations.TestTimeSpec.RunningFromNow(),
                           MemorySystemEnvironment.InitFromSystem()));

    /// <summary>
    /// Constructor function
    /// </summary>
    /// <param name="encoding">Text encoding</param>
    /// <param name="timeSpec">Defines how time works in the runtime</param>
    public static Runtime<M> New(Encoding encoding, 
                                 Implementations.TestTimeSpec? timeSpec = default) =>
        new(new RuntimeEnv(EnvIO.New(),
                           encoding,
                           new MemoryConsole(),
                           new MemoryFS(),
                           timeSpec ?? Implementations.TestTimeSpec.RunningFromNow(),
                           MemorySystemEnvironment.InitFromSystem()));

    /// <summary>
    /// Constructor function
    /// </summary>
    /// <param name="encoding">Text encoding</param>
    /// <param name="envIO">Environment for the IO monad</param>
    /// <param name="timeSpec">Defines how time works in the runtime</param>
    public static Runtime<M> New(EnvIO envIO, 
                                 Encoding encoding, 
                                 Implementations.TestTimeSpec? timeSpec = default) =>
        new(new RuntimeEnv(envIO,
                           encoding,
                           new MemoryConsole(),
                           new MemoryFS(),
                           timeSpec ?? Implementations.TestTimeSpec.RunningFromNow(),
                           MemorySystemEnvironment.InitFromSystem()));        

    /// <summary>
    /// Get encoding
    /// </summary>
    /// <returns></returns>
    public Encoding Encoding =>
        Env.Encoding;

    public Runtime<M> WithIO(EnvIO envIO) =>
        new(Env with { EnvIO = envIO });

    public EnvIO EnvIO =>
        Env.EnvIO;

    /// <summary>
    /// Access the console environment
    /// </summary>
    /// <returns>Console environment</returns>
    K<M, ConsoleIO> Has<M, ConsoleIO>.Trait => 
        M.Asks(rt => new Implementations.ConsoleIO(rt.Env.Console));

    /// <summary>
    /// Access the file environment
    /// </summary>
    /// <returns>File environment</returns>
    K<M, FileIO> Has<M, FileIO>.Trait => 
        from n in Time<M, Runtime<M>>.now
        from r in M.Asks(rt => new Implementations.FileIO(rt.Env.FileSystem, n))
        select r;

    /// <summary>
    /// Access the TextReader environment
    /// </summary>
    /// <returns>TextReader environment</returns>
    K<M, TextReadIO> Has<M, TextReadIO>.Trait => 
        M.Asks(_ => Implementations.TextReadIO.Default);

    /// <summary>
    /// Access the time environment
    /// </summary>
    /// <returns>Time environment</returns>
    K<M, TimeIO> Has<M, TimeIO>.Trait => 
        M.Asks(rt => new Implementations.TimeIO(rt.Env.TimeSpec));

    /// <summary>
    /// Access the operating-system environment
    /// </summary>
    /// <returns>Operating-system environment environment</returns>
    K<M, EnvironmentIO> Has<M, EnvironmentIO>.Trait => 
        M.Asks(rt => new Implementations.EnvironmentIO(rt.Env.SysEnv));

    /// <summary>
    /// Access the directory environment
    /// </summary>
    /// <returns>Directory environment</returns>
    K<M, DirectoryIO> Has<M, DirectoryIO>.Trait =>
        from n in Time<M, Runtime<M>>.now
        from r in M.Asks(rt => new Implementations.DirectoryIO(rt.Env.FileSystem, n))
        select r;
}
    
public record RuntimeEnv(
    EnvIO EnvIO,
    Encoding Encoding,
    MemoryConsole Console,
    MemoryFS FileSystem,
    Implementations.TestTimeSpec TimeSpec,
    MemorySystemEnvironment SysEnv)
{
    public RuntimeEnv LocalCancel =>
        this with { EnvIO = EnvIO.LocalCancel }; 
}
