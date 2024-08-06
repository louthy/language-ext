using System;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

/// <summary>
/// Environment IO
/// </summary>
public static class Environment<M, RT>
    where M : 
        Monad<M>
    where RT : 
        Has<M, EnvironmentIO>
{
    static readonly K<M, EnvironmentIO> envIO =
        Has<M, RT, EnvironmentIO>.ask;
    
    /// <summary>
    /// Gets the command line for this process.
    /// </summary>
    public static K<M, string> commandLine =>
        envIO.Bind(e => e.CommandLine());

    /// <summary>
    /// Gets a unique identifier for the current managed thread.
    /// </summary>
    public static K<M, int> currentManagedThreadId =>
        envIO.Bind(e => e.CurrentManagedThreadId());

    /// <summary>
    /// Terminates this process and returns an exit code to the operating system.
    /// </summary>
    public static K<M, Unit> exit(int exitCode) =>
        envIO.Bind(e => e.Exit(exitCode));

    /// <summary>
    /// Gets the exit code of the process.
    /// </summary>
    public static K<M, int> exitCode =>
        envIO.Bind(e => e.ExitCode());

    /// <summary>
    /// Sets the exit code of the process.
    /// </summary>
    /// <param name="exitCode">exit code of the process</param>
    public static K<M, Unit> setExitCode(int exitCode) =>
        envIO.Bind(e => e.SetExitCode(exitCode));

    /// <summary>
    /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
    /// </summary>
    /// <param name="name">A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).</param>
    public static K<M, string> expandEnvironmentVariables(string name) =>
        envIO.Bind(e => e.ExpandEnvironmentVariables(name));

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
    /// </summary>
    /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
    public static K<M, Unit> failFast(Option<string> message) =>
        envIO.Bind(e => e.FailFast(message));

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
    /// </summary>
    /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
    /// <param name="exception">An exception that represents the error that caused the termination. This is typically the exception in a catch block.</param>
    public static K<M, Unit> failFast(Option<string> message, Option<Exception> exception) =>
        envIO.Bind(e => e.FailFast(message, exception));

    /// <summary>
    /// Returns a string array containing the command-line arguments for the current process.
    /// </summary>
    public static K<M, Seq<string>> commandLineArgs =>
        envIO.Bind(e => e.GetCommandLineArgs());

    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    public static K<M, Option<string>> getEnvironmentVariable(string variable) =>
        envIO.Bind(e => e.GetEnvironmentVariable(variable));

    /// <summary>
    /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="target">Target</param>
    public static K<M, Option<string>> getEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
        envIO.Bind(e => e.GetEnvironmentVariable(variable, target));

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public static K<M, HashMap<string, Option<string>>> environmentVariables =>
        envIO.Bind(e => e.GetEnvironmentVariables());

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// <param name="target">One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.</param>
    public static K<M, HashMap<string, Option<string>>> getEnvironmentVariables(EnvironmentVariableTarget target) =>
        envIO.Bind(e => e.GetEnvironmentVariables(target));

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration.
    /// </summary>
    /// <param name="folder">One of enumeration values that identifies a system special folder.</param>
    public static K<M, string> getFolderPath(Environment.SpecialFolder folder) =>
        envIO.Bind(e => e.GetFolderPath(folder));

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
    /// </summary>
    /// <param name="folder">One of the enumeration values that identifies a system special folder.</param>
    /// <param name="option">One of the enumeration values that specifies options to use for accessing a special folder.</param>
    public static K<M, string> getFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option) =>
        envIO.Bind(e => e.GetFolderPath(folder, option));

    /// <summary>
    /// Returns an array of string containing the names of the logical drives on the current computer.
    /// </summary>
    public static K<M, Seq<string>> logicalDrives =>
        envIO.Bind(e => e.GetLogicalDrives());

    /// <summary>
    /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
    /// </summary>
    public static K<M, bool> hasShutdownStarted =>
        envIO.Bind(e => e.HasShutdownStarted());

    /// <summary>
    /// Determines whether the current operating system is a 64-bit operating system.
    /// </summary>
    public static K<M, bool> is64BitOperatingSystem =>
        envIO.Bind(e => e.Is64BitOperatingSystem());

    /// <summary>
    /// Determines whether the current process is a 64-bit process.
    /// </summary>
    public static K<M, bool> is64BitProcess =>
        envIO.Bind(e => e.Is64BitProcess());

    /// <summary>
    /// Gets the NetBIOS name of this local computer.
    /// </summary>
    public static K<M, string> machineName =>
        envIO.Bind(e => e.MachineName());

    /// <summary>
    /// Gets the newline string defined for this environment.
    /// </summary>
    public static K<M, string> newLine =>
        envIO.Bind(e => e.NewLine());

    /// <summary>
    /// Gets an OperatingSystem object that contains the current platform identifier and version number.
    /// </summary>
    public static K<M, OperatingSystem> osVersion =>
        envIO.Bind(e => e.OSVersion());

    /// <summary>
    /// Gets the number of processors on the current machine.
    /// </summary>
    public static K<M, int> processorCount =>
        envIO.Bind(e => e.ProcessorCount());

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="value">A value to assign to variable .</param>
    public static K<M, Unit> setEnvironmentVariable(string variable, Option<string> value) =>
        envIO.Bind(e => e.SetEnvironmentVariable(variable, value));

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="value">A value to assign to variable.</param>
    /// <param name="target">One of the enumeration values that specifies the location of the environment variable.</param>
    public static K<M, Unit> SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target) =>
        envIO.Bind(e => e.SetEnvironmentVariable(variable, value, target));

    /// <summary>
    /// Gets current stack trace information.
    /// </summary>
    public static K<M, string> stackTrace =>
        envIO.Bind(e => e.StackTrace());

    /// <summary>
    /// Gets the fully qualified path of the system directory.
    /// </summary>
    public static K<M, string> systemDirectory =>
        envIO.Bind(e => e.SystemDirectory());

    /// <summary>
    /// Gets the number of bytes in the operating system's memory page.
    /// </summary>
    public static K<M, int> systemPageSize =>
        envIO.Bind(e => e.SystemPageSize());

    /// <summary>
    /// Gets the number of milliseconds elapsed since the system started.
    /// </summary>
    public static K<M, long> tickCount =>
        envIO.Bind(e => e.TickCount());

    /// <summary>
    /// Gets the network domain name associated with the current user.
    /// </summary>
    public static K<M, string> userDomainName =>
        envIO.Bind(e => e.UserDomainName());

    /// <summary>
    /// Gets a value indicating whether the current process is running in user interactive mode.
    /// </summary>
    public static K<M, bool> userInteractive =>
        envIO.Bind(e => e.UserInteractive());

    /// <summary>
    /// Gets the user name of the person who is currently logged on to the operating system.
    /// </summary>
    public static K<M, string> userName =>
        envIO.Bind(e => e.UserName());

    /// <summary>
    /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
    /// </summary>
    public static K<M, Version> version =>
        envIO.Bind(e => e.Version());

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public static K<M, long> workingSet =>
        envIO.Bind(e => e.WorkingSet());
}
