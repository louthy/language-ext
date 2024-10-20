using System;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

/// <summary>
/// Environment IO
/// </summary>
public static class Environment<RT>
    where RT : 
        Has<Eff<RT>, EnvironmentIO>
{
    /// <summary>
    /// Gets the command line for this process.
    /// </summary>
    public static Eff<RT, string> commandLine =>
        Environment<Eff<RT>, RT>.commandLine.As();

    /// <summary>
    /// Gets a unique identifier for the current managed thread.
    /// </summary>
    public static Eff<RT, int> currentManagedThreadId =>
        Environment<Eff<RT>, RT>.currentManagedThreadId.As();

    /// <summary>
    /// Terminates this process and returns an exit code to the operating system.
    /// </summary>
    public static Eff<RT, Unit> exit(int exitCode) =>
        Environment<Eff<RT>, RT>.exit(exitCode).As();

    /// <summary>
    /// Gets the exit code of the process.
    /// </summary>
    public static Eff<RT, int> exitCode =>
        Environment<Eff<RT>, RT>.exitCode.As();

    /// <summary>
    /// Sets the exit code of the process.
    /// </summary>
    /// <param name="exitCode">exit code of the process</param>
    public static Eff<RT, Unit> setExitCode(int exitCode) =>
        Environment<Eff<RT>, RT>.setExitCode(exitCode).As();

    /// <summary>
    /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
    /// </summary>
    /// <param name="name">A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).</param>
    public static Eff<RT, string> expandEnvironmentVariables(string name) =>
        Environment<Eff<RT>, RT>.expandEnvironmentVariables(name).As();

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
    /// </summary>
    /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
    public static Eff<RT, Unit> failFast(Option<string> message) =>
        Environment<Eff<RT>, RT>.failFast(message).As();

    /// <summary>
    /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
    /// </summary>
    /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
    /// <param name="exception">An exception that represents the error that caused the termination. This is typically the exception in a catch block.</param>
    public static Eff<RT, Unit> failFast(Option<string> message, Option<Exception> exception) =>
        Environment<Eff<RT>, RT>.failFast(message, exception).As();

    /// <summary>
    /// Returns a string array containing the command-line arguments for the current process.
    /// </summary>
    public static Eff<RT, Seq<string>> commandLineArgs =>
        Environment<Eff<RT>, RT>.commandLineArgs.As();

    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    public static Eff<RT, Option<string>> getEnvironmentVariable(string variable) =>
        Environment<Eff<RT>, RT>.getEnvironmentVariable(variable).As();

    /// <summary>
    /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="target">Target</param>
    public static Eff<RT, Option<string>> getEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
        Environment<Eff<RT>, RT>.getEnvironmentVariable(variable, target).As();

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public static Eff<RT, HashMap<string, Option<string>>> environmentVariables =>
        Environment<Eff<RT>, RT>.environmentVariables.As();

    /// <summary>
    /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
    /// </summary>
    /// <param name="target">One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.</param>
    public static Eff<RT, HashMap<string, Option<string>>> getEnvironmentVariables(EnvironmentVariableTarget target) =>
        Environment<Eff<RT>, RT>.getEnvironmentVariables(target).As();

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration.
    /// </summary>
    /// <param name="folder">One of enumeration values that identifies a system special folder.</param>
    public static Eff<RT, string> getFolderPath(Environment.SpecialFolder folder) =>
        Environment<Eff<RT>, RT>.getFolderPath(folder).As();

    /// <summary>
    /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
    /// </summary>
    /// <param name="folder">One of the enumeration values that identifies a system special folder.</param>
    /// <param name="option">One of the enumeration values that specifies options to use for accessing a special folder.</param>
    public static Eff<RT, string> getFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option) =>
        Environment<Eff<RT>, RT>.getFolderPath(folder, option).As();

    /// <summary>
    /// Returns an array of string containing the names of the logical drives on the current computer.
    /// </summary>
    public static Eff<RT, Seq<string>> logicalDrives =>
        Environment<Eff<RT>, RT>.logicalDrives.As();

    /// <summary>
    /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
    /// </summary>
    public static Eff<RT, bool> hasShutdownStarted =>
        Environment<Eff<RT>, RT>.hasShutdownStarted.As();

    /// <summary>
    /// Determines whether the current operating system is a 64-bit operating system.
    /// </summary>
    public static Eff<RT, bool> is64BitOperatingSystem =>
        Environment<Eff<RT>, RT>.is64BitOperatingSystem.As();

    /// <summary>
    /// Determines whether the current process is a 64-bit process.
    /// </summary>
    public static Eff<RT, bool> is64BitProcess =>
        Environment<Eff<RT>, RT>.is64BitProcess.As();

    /// <summary>
    /// Gets the NetBIOS name of this local computer.
    /// </summary>
    public static Eff<RT, string> machineName =>
        Environment<Eff<RT>, RT>.machineName.As();

    /// <summary>
    /// Gets the newline string defined for this environment.
    /// </summary>
    public static Eff<RT, string> newLine =>
        Environment<Eff<RT>, RT>.newLine.As();

    /// <summary>
    /// Gets an OperatingSystem object that contains the current platform identifier and version number.
    /// </summary>
    public static Eff<RT, OperatingSystem> osVersion =>
        Environment<Eff<RT>, RT>.osVersion.As();

    /// <summary>
    /// Gets the number of processors on the current machine.
    /// </summary>
    public static Eff<RT, int> processorCount =>
        Environment<Eff<RT>, RT>.processorCount.As();

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="value">A value to assign to variable .</param>
    public static Eff<RT, Unit> setEnvironmentVariable(string variable, Option<string> value) =>
        Environment<Eff<RT>, RT>.setEnvironmentVariable(variable, value).As();

    /// <summary>
    /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
    /// </summary>
    /// <param name="variable">The name of an environment variable.</param>
    /// <param name="value">A value to assign to variable.</param>
    /// <param name="target">One of the enumeration values that specifies the location of the environment variable.</param>
    public static Eff<RT, Unit> setEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target) =>
        Environment<Eff<RT>, RT>.setEnvironmentVariable(variable, value, target).As();

    /// <summary>
    /// Gets current stack trace information.
    /// </summary>
    public static Eff<RT, string> stackTrace =>
        Environment<Eff<RT>, RT>.stackTrace.As();

    /// <summary>
    /// Gets the fully qualified path of the system directory.
    /// </summary>
    public static Eff<RT, string> systemDirectory =>
        Environment<Eff<RT>, RT>.systemDirectory.As();

    /// <summary>
    /// Gets the number of bytes in the operating system's memory page.
    /// </summary>
    public static Eff<RT, int> systemPageSize =>
        Environment<Eff<RT>, RT>.systemPageSize.As();

    /// <summary>
    /// Gets the number of milliseconds elapsed since the system started.
    /// </summary>
    public static Eff<RT, long> tickCount =>
        Environment<Eff<RT>, RT>.tickCount.As();

    /// <summary>
    /// Gets the network domain name associated with the current user.
    /// </summary>
    public static Eff<RT, string> userDomainName =>
        Environment<Eff<RT>, RT>.userDomainName.As();

    /// <summary>
    /// Gets a value indicating whether the current process is running in user interactive mode.
    /// </summary>
    public static Eff<RT, bool> userInteractive =>
        Environment<Eff<RT>, RT>.userInteractive.As();

    /// <summary>
    /// Gets the user name of the person who is currently logged on to the operating system.
    /// </summary>
    public static Eff<RT, string> userName =>
        Environment<Eff<RT>, RT>.userName.As();

    /// <summary>
    /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
    /// </summary>
    public static Eff<RT, Version> version =>
        Environment<Eff<RT>, RT>.version.As();

    /// <summary>
    /// Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public static Eff<RT, long> workingSet =>
        Environment<Eff<RT>, RT>.workingSet.As();
}
