using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test.Implementations;

public record EnvironmentIO(MemorySystemEnvironment env) : Sys.Traits.EnvironmentIO
{
    /// <summary>
    /// Gets the command line for this process.
    /// </summary>
    public IO<string> CommandLine() =>
        lift(() => env.CommandLine);

    /// <summary>
    /// Gets a unique identifier for the current managed thread.
    /// </summary>
    public IO<int> CurrentManagedThreadId() =>
        lift(() => env.CurrentManagedThreadId);

    /// <summary>
    /// Terminates this process and returns an exit code to the operating system.
    /// </summary>
    public IO<Unit> Exit(int exitCode) =>
        lift(() =>
             {
                 Environment.Exit(exitCode);
                 return unit;
             });

    /// <summary>
    /// Gets the exit code of the process.
    /// </summary>
    public IO<int> ExitCode() =>
        lift(() => env.ExitCode);

    /// <summary>
    /// Sets the exit code of the process.
    /// </summary>
    // exitCode: exit code of the process
    public IO<Unit> SetExitCode(int exitCode) =>
        lift(() =>
             {
                 env.ExitCode = exitCode;
                 return unit;
             });

    /// <summary>
    /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
    /// </summary>
    /// name: A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).
    public IO<string> ExpandEnvironmentVariables(string name) =>
        lift<string>(() => throw new NotImplementedException());

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
    /// </summary>
    /// message: A message that explains why the process was terminated, or null if no explanation is provided.
    public IO<Unit> FailFast(Option<string> message) =>
        lift(() =>
             {
                 Environment.FailFast(message.IfNone(""));
                 return unit;
             });

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
    /// </summary>
    /// message: A message that explains why the process was terminated, or null if no explanation is provided.
    /// exception: An exception that represents the error that caused the termination. This is typically the exception in a catch block.
    public IO<Unit> FailFast(Option<string> message, Option<Exception> exception) =>
        lift(() =>
             {
                 Environment.FailFast(message.IfNone(""), exception.IfNone(BottomException.Default));
                 return unit;
             });

    /// <summary>
    /// Returns a string array containing the command-line arguments for the current process.
    /// </summary>
    public IO<Seq<string>> GetCommandLineArgs() =>
        lift(() => env.CommandLineArgs);

    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// variable: The name of an environment variable.
    public IO<Option<string>> GetEnvironmentVariable(string variable) =>
        lift(() =>
             {
                 if (env.ProcessEnvironmentVariables.TryGetValue(variable, out var processEnvironmentVariable))
                 {
                     return processEnvironmentVariable;
                 }
                 else if (env.UserEnvironmentVariables.TryGetValue(variable, out var userEnvironmentVariable))
                 {
                     return userEnvironmentVariable;
                 }
                 else if (env.SystemEnvironmentVariables.TryGetValue(variable, out var environmentVariable))
                 {
                     return environmentVariable;
                 }
                 else
                 {
                     return None;
                 }
             });

    /// <summary>
    /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// variable: The name of an environment variable.
    public IO<Option<string>> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
        lift(() =>
             {
                 if (target == EnvironmentVariableTarget.Process &&
                     env.ProcessEnvironmentVariables.TryGetValue(variable, out var processEnvironmentVariable))
                 {
                     return processEnvironmentVariable;
                 }
                 else if (target == EnvironmentVariableTarget.User &&
                          env.UserEnvironmentVariables.TryGetValue(variable, out var userEnvironmentVariable))
                 {
                     return userEnvironmentVariable;
                 }
                 else if (target == EnvironmentVariableTarget.Machine &&
                          env.SystemEnvironmentVariables.TryGetValue(variable, out var environmentVariable))
                 {
                     return environmentVariable;
                 }
                 else
                 {
                     return default;
                 }
             });

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public IO<HashMap<string, Option<string>>> GetEnvironmentVariables() =>
        lift(() => env.ProcessEnvironmentVariables.ToHashMap() +
                   env.UserEnvironmentVariables.ToHashMap()    +
                   env.SystemEnvironmentVariables.ToHashMap());

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// target: One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.
    public IO<HashMap<string, Option<string>>> GetEnvironmentVariables(EnvironmentVariableTarget target) =>
        lift(() => target switch
                   {
                       EnvironmentVariableTarget.Process => env.ProcessEnvironmentVariables.ToHashMap(),
                       EnvironmentVariableTarget.User    => env.UserEnvironmentVariables.ToHashMap(),
                       EnvironmentVariableTarget.Machine => env.SystemEnvironmentVariables.ToHashMap(),
                       _                                 => default
                   });

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration.
    /// </summary>
    /// folder: One of enumeration values that identifies a system special folder.
    public IO<string> GetFolderPath(Environment.SpecialFolder folder) =>
        lift(() => env.GetFolderPath(folder, Environment.SpecialFolderOption.None));

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
    /// </summary>
    /// folder: One of the enumeration values that identifies a system special folder.
    /// option: One of the enumeration values that specifies options to use for accessing a special folder.
    public IO<string> GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option) =>
        lift(() => env.GetFolderPath(folder, option));

    /// <summary>
    /// Returns an array of string containing the names of the logical drives on the current computer.
    /// </summary>
    /// string[] Environment.GetLogicalDrives()
    public IO<Seq<string>> GetLogicalDrives() =>
        lift(() => env.LogicalDrives);

    /// <summary>
    /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
    /// </summary>
    public IO<bool> HasShutdownStarted() =>
        lift(() => env.HasShutdownStarted);

    /// <summary>
    /// Determines whether the current operating system is a 64-bit operating system.
    /// </summary>
    public IO<bool> Is64BitOperatingSystem() =>
        lift(() => env.Is64BitOperatingSystem);

    /// <summary>
    /// Determines whether the current process is a 64-bit process.
    /// </summary>
    public IO<bool> Is64BitProcess() =>
        lift(() => env.Is64BitProcess);

    /// <summary>
    /// Gets the NetBIOS name of this local computer.
    /// </summary>
    public IO<string> MachineName() =>
        lift(() => env.MachineName);

    /// <summary>
    /// Gets the newline string defined for this environment.
    /// </summary>
    public IO<string> NewLine() =>
        lift(() => env.NewLine);

    /// <summary>
    /// Gets an OperatingSystem object that contains the current platform identifier and version number.
    /// </summary>
    public IO<OperatingSystem> OSVersion() =>
        lift(() => env.OSVersion);

    /// <summary>
    /// Gets the number of processors on the current machine.
    /// </summary>
    public IO<int> ProcessorCount() =>
        lift(() => env.ProcessorCount);

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process.
    /// </summary>
    /// variable: The name of an environment variable.
    /// value: A value to assign to variable .
    public IO<Unit> SetEnvironmentVariable(string variable, Option<string> value) =>
        lift(() =>
             {
                 if (value.IsSome)
                 {
                     env.ProcessEnvironmentVariables.AddOrUpdate(variable, (string)value, (_, _) => (string)value);
                 }
                 else
                 {
                     env.ProcessEnvironmentVariables.TryRemove(variable, out _);
                 }
                 return unit;
             });

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
    /// </summary>
    /// variable: The name of an environment variable.
    /// value: A value to assign to variable.
    /// target: One of the enumeration values that specifies the location of the environment variable.
    public IO<Unit> SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target) =>
        lift(() =>
             {
                 switch (target)
                 {
                     case EnvironmentVariableTarget.Process:
                         if (value.IsSome)
                         {
                             env.ProcessEnvironmentVariables.AddOrUpdate(
                                 variable, (string)value, (_, _) => (string)value);
                         }
                         else
                         {
                             env.ProcessEnvironmentVariables.TryRemove(variable, out var _);
                         }

                         break;

                     case EnvironmentVariableTarget.User:
                         if (value.IsSome)
                         {
                             env.UserEnvironmentVariables.AddOrUpdate(variable, (string)value, (_, _) => (string)value);
                         }
                         else
                         {
                             env.UserEnvironmentVariables.TryRemove(variable, out _);
                         }

                         break;

                     case EnvironmentVariableTarget.Machine:
                         if (value.IsSome)
                         {
                             env.SystemEnvironmentVariables.AddOrUpdate(
                                 variable, (string)value, (_, _) => (string)value);
                         }
                         else
                         {
                             env.SystemEnvironmentVariables.TryRemove(variable, out _);
                         }

                         break;
                 }
                 return unit;
             });

    /// <summary>
    /// Gets current stack trace information.
    /// </summary>
    public IO<string> StackTrace() =>
        lift(() => env.StackTrace);

    /// <summary>
    /// Gets the fully qualified path of the system directory.
    /// </summary>
    public IO<string> SystemDirectory() =>
        lift(() => env.SystemDirectory);

    /// <summary>
    /// Gets the number of bytes in the operating system's memory page.
    /// </summary>
    public IO<int> SystemPageSize() =>
        lift(() => env.SystemPageSize);

    /// <summary>
    /// Gets the number of milliseconds elapsed since the system started.
    /// </summary>
    public IO<long> TickCount() =>
        lift(() => env.TickCount);

    // NOTE: This seems to be a newer interface, but I'm not sure how to handle it
    // Gets the number of milliseconds elapsed since the system started.
    // public IO<long TickCount64() =>
    //     Environment.TickCount64);

    /// <summary>
    /// Gets the network domain name associated with the current user.
    /// </summary>
    public IO<string> UserDomainName() =>
        lift(() => env.UserDomainName);

    /// <summary>
    /// Gets a value indicating whether the current process is running in user interactive mode.
    /// </summary>
    public IO<bool> UserInteractive() =>
        lift(() => env.UserInteractive);

    /// <summary>
    /// Gets the user name of the person who is currently logged on to the operating system.
    /// </summary>
    public IO<string> UserName() =>
        lift(() => env.UserName);

    /// <summary>
    /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
    /// </summary>
    public IO<Version> Version() =>
        lift(() => env.Version);

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public IO<long> WorkingSet() =>
        lift(() => env.WorkingSet);
}
