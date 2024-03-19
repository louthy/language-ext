using System;
using System.Collections;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live.Implementations;

public record EnvironmentIO : Sys.Traits.EnvironmentIO
{
    public static readonly Sys.Traits.EnvironmentIO Default =
        new EnvironmentIO();

    /// <summary>
    /// Gets the command line for this process.
    /// </summary>
    public IO<string> CommandLine() =>
        lift(() => Environment.CommandLine);

    /// <summary>
    /// Sets the fully qualified path of the current working directory.
    /// </summary>
    /// directory: fully qualified path of the current working directory.
    public IO<Unit> SetCurrentDirectory(string directory) =>
        lift(() => { Environment.CurrentDirectory = directory; });

    /// <summary>
    /// Gets a unique identifier for the current managed thread.
    /// </summary>
    public IO<int> CurrentManagedThreadId() =>
        lift(() => Environment.CurrentManagedThreadId);

    /// <summary>
    /// Terminates this process and returns an exit code to the operating 
    /// </summary>
    public IO<Unit> Exit(int exitCode) =>
        lift(() => Environment.Exit(exitCode));

    /// <summary>
    /// Gets the exit code of the process.
    /// </summary>
    public IO<int> ExitCode() =>
        lift(() => Environment.ExitCode);

    /// <summary>
    /// Sets the exit code of the process.
    /// </summary>
    // exitCode: exit code of the process
    public IO<Unit> SetExitCode(int exitCode) =>
        lift(() => { Environment.ExitCode = exitCode; });

    /// <summary>
    /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
    /// </summary>
    /// name: A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).
    public IO<string> ExpandEnvironmentVariables(string name) =>
        lift(() => Environment.ExpandEnvironmentVariables(name));

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
    /// </summary>
    /// message: A message that explains why the process was terminated, or null if no explanation is provided.
    public IO<Unit> FailFast(Option<string> message) =>
        lift(() => Environment.FailFast(message.IfNone("")));

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
    /// </summary>
    /// message: A message that explains why the process was terminated, or null if no explanation is provided.
    /// exception: An exception that represents the error that caused the termination. This is typically the exception in a catch block.
    public IO<Unit> FailFast(Option<string> message, Option<Exception> exception) =>
        lift(() => Environment.FailFast(message.IfNone(""), exception.IfNone(() => BottomException.Default)));

    /// <summary>
    /// Returns a string array containing the command-line arguments for the current process.
    /// </summary>
    public IO<Seq<string>> GetCommandLineArgs() =>
        lift(() => Environment.GetCommandLineArgs().AsEnumerableM().ToSeq());

    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// variable: The name of an environment variable.
    public IO<Option<string>> GetEnvironmentVariable(string variable) =>
        lift(() => Optional(Environment.GetEnvironmentVariable(variable)));

    /// <summary>
    /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// variable: The name of an environment variable.
    public IO<Option<string>> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
        lift(() => Optional(Environment.GetEnvironmentVariable(variable, target)));

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public IO<HashMap<string, Option<string>>> GetEnvironmentVariables() =>
        lift(() =>
             {
                 var hm = HashMap<string, Option<string>>();
                 foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                 {
                     if (de.Key.ToString() is { } key)
                     {
                         hm = hm.Add(key, Optional(de.Value?.ToString()));
                     }
                 }

                 return hm;
             });

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// target: One of the EnvironmentVariableTarget values. Only EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.
    public IO<HashMap<string, Option<string>>> GetEnvironmentVariables(EnvironmentVariableTarget target) =>
        lift(() =>
             {
                 var hm = HashMap<string, Option<string>>();
                 foreach (DictionaryEntry de in Environment.GetEnvironmentVariables(target))
                 {
                     if (de.Key.ToString() is { } key)
                     {
                         hm = hm.Add(key, Optional(de.Value?.ToString()));
                     }
                 }

                 return hm;
             });

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration.
    /// </summary>
    /// folder: One of enumeration values that identifies a system special folder.
    public IO<string> GetFolderPath(Environment.SpecialFolder folder) =>
        lift(() => Environment.GetFolderPath(folder));

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
    /// </summary>
    /// folder: One of the enumeration values that identifies a system special folder.
    /// option: One of the enumeration values that specifies options to use for accessing a special folder.
    public IO<string> GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option) =>
        lift(() => Environment.GetFolderPath(folder, option));

    /// <summary>
    /// Returns an array of string containing the names of the logical drives on the current computer.
    /// </summary>
    /// string[] Environment.GetLogicalDrives()
    public IO<Seq<string>> GetLogicalDrives() =>
        lift(() => Environment.GetLogicalDrives().AsEnumerableM().ToSeq());

    /// <summary>
    /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
    /// </summary>
    public IO<bool> HasShutdownStarted() =>
        lift(() => Environment.HasShutdownStarted);

    /// <summary>
    /// Determines whether the current operating system is a 64-bit operating 
    /// </summary>
    public IO<bool> Is64BitOperatingSystem() =>
        lift(() => Environment.Is64BitOperatingSystem);

    /// <summary>
    /// Determines whether the current process is a 64-bit process.
    /// </summary>
    public IO<bool> Is64BitProcess() =>
        lift(() => Environment.Is64BitProcess);

    /// <summary>
    /// Gets the NetBIOS name of this local computer.
    /// </summary>
    public IO<string> MachineName() =>
        lift(() => Environment.MachineName);

    /// <summary>
    /// Gets the newline string defined for this environment.
    /// </summary>
    public IO<string> NewLine() =>
        lift(() => Environment.NewLine);

    /// <summary>
    /// Gets an OperatingSystem object that contains the current platform identifier and version number.
    /// </summary>
    public IO<OperatingSystem> OSVersion() =>
        lift(() => Environment.OSVersion);

    /// <summary>
    /// Gets the number of processors on the current machine.
    /// </summary>
    public IO<int> ProcessorCount() =>
        lift(() => Environment.ProcessorCount);

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process.
    /// </summary>
    /// variable: The name of an environment variable.
    /// value: A value to assign to variable .
    public IO<Unit> SetEnvironmentVariable(string variable, Option<string> value) =>
        lift(() => Environment.SetEnvironmentVariable(variable, value.IfNone("")));

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
    /// </summary>
    /// variable: The name of an environment variable.
    /// value: A value to assign to variable.
    /// target: One of the enumeration values that specifies the location of the environment variable.
    public IO<Unit> SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target) =>
        lift(() => Environment.SetEnvironmentVariable(variable, value.IfNone(""), target));

    /// <summary>
    /// Gets current stack trace information.
    /// </summary>
    public IO<string> StackTrace() =>
        lift(() => Environment.StackTrace);

    /// <summary>
    /// Gets the fully qualified path of the system directory.
    /// </summary>
    public IO<string> SystemDirectory() =>
        lift(() => Environment.SystemDirectory);

    /// <summary>
    /// Gets the number of bytes in the operating system's memory page.
    /// </summary>
    public IO<int> SystemPageSize() =>
        lift(() => Environment.SystemPageSize);

    /// <summary>
    /// Gets the number of milliseconds elapsed since the system started.
    /// </summary>
    public IO<long> TickCount() =>
        lift(() => Environment.TickCount64);

    /// <summary>
    /// Gets the network domain name associated with the current user.
    /// </summary>
    public IO<string> UserDomainName() =>
        lift(() => Environment.UserDomainName);

    /// <summary>
    /// Gets a value indicating whether the current process is running in user interactive mode.
    /// </summary>
    public IO<bool> UserInteractive() =>
        lift(() => Environment.UserInteractive);

    /// <summary>
    /// Gets the user name of the person who is currently logged on to the operating 
    /// </summary>
    public IO<string> UserName() =>
        lift(() => Environment.UserName);

    /// <summary>
    /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
    /// </summary>
    public IO<Version> Version() =>
        lift(() => Environment.Version);

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public IO<long> WorkingSet() =>
        lift(() => Environment.WorkingSet);
}
