using System;

namespace LanguageExt.Sys.Traits;

public interface EnvironmentIO
{
    IO<string> CommandLine();
    IO<int> CurrentManagedThreadId();
    IO<Unit> Exit(int exitCode);
    IO<int> ExitCode();
    IO<Unit> SetExitCode(int exitCode);
    IO<string> ExpandEnvironmentVariables(string name);
    IO<Unit> FailFast(Option<string> message);
    IO<Unit> FailFast(Option<string> message, Option<Exception> exception);
    IO<Seq<string>> GetCommandLineArgs();
    IO<Option<string>> GetEnvironmentVariable(string variable);
    IO<Option<string>> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
    IO<HashMap<string, Option<string>>> GetEnvironmentVariables();
    IO<HashMap<string, Option<string>>> GetEnvironmentVariables(EnvironmentVariableTarget target);
    IO<string> GetFolderPath(Environment.SpecialFolder folder);
    IO<string> GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option);
    IO<Seq<string>> GetLogicalDrives();
    IO<bool> HasShutdownStarted();
    IO<bool> Is64BitOperatingSystem();
    IO<bool> Is64BitProcess();
    IO<string> MachineName();
    IO<string> NewLine();
    IO<OperatingSystem> OSVersion();
    IO<int> ProcessorCount();
    IO<Unit> SetEnvironmentVariable(string variable, Option<string> value);
    IO<Unit> SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target);
    IO<string> StackTrace();
    IO<string> SystemDirectory();
    IO<int> SystemPageSize();
    IO<long> TickCount();
    IO<string> UserDomainName();
    IO<bool> UserInteractive();
    IO<string> UserName();
    IO<Version> Version();
    IO<long> WorkingSet();
}
