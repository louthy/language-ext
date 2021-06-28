using System;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    public interface EnvironmentIO
    {
        string CommandLine();
        int CurrentManagedThreadId();
        Unit Exit(int exitCode);
        int ExitCode();
        Unit SetExitCode(int exitCode);
        string ExpandEnvironmentVariables(string name);
        Unit FailFast(Option<string> message);
        Unit FailFast(Option<string> message, Option<Exception> exception);
        Seq<string> GetCommandLineArgs();
        Option<string> GetEnvironmentVariable(string variable);
        Option<string> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
        HashMap<string, string> GetEnvironmentVariables();
        HashMap<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target);
        string GetFolderPath(System.Environment.SpecialFolder folder);
        string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option);
        Seq<string> GetLogicalDrives();
        bool HasShutdownStarted();
        bool Is64BitOperatingSystem();
        bool Is64BitProcess();
        string MachineName();
        string NewLine();
        OperatingSystem OSVersion();
        int ProcessorCount();
        Unit SetEnvironmentVariable(string variable, Option<string> value);
        Unit SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target);
        string StackTrace();
        string SystemDirectory();
        int SystemPageSize();
        int TickCount();
        // This seems to be a newer interface, but I'm not sure how to handle it
        // long TickCount64();
        string UserDomainName();
        bool UserInteractive();
        string UserName();
        Version Version();
        long WorkingSet();
    }

    /// <summary>
    /// Type-class giving a struct the trait of supporting Environment IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasEnvironment<RT> : HasCancel<RT>
        where RT : struct, HasEnvironment<RT>, HasCancel<RT>
    {
        /// <summary>
        /// Access the environment synchronous effect environment
        /// </summary>
        /// <returns>Environment synchronous effect environment</returns>
        Eff<RT, EnvironmentIO> EnvironmentEff { get; }
    }
}
