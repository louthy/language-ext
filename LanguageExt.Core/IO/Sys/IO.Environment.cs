using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// IO
    /// </summary>
    public static partial class IO
    {
        /// <summary>
        /// Environment IO
        /// </summary>
        public static class Environment
        {
            /// <summary>
            /// Gets the command line for this process.
            /// </summary>
            public static Eff<RT, string> commandLine<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.CommandLine());

            /// <summary>
            /// Gets the fully qualified path of the current working directory.
            /// </summary>
            public static Eff<RT, string> currentDirectory<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.CurrentDirectory());

            /// <summary>
            /// Sets the fully qualified path of the current working directory.
            /// </summary>
            /// <param name="directory">fully qualified path of the current working directory.</param>
            public static Eff<RT, Unit> setCurrentDirectory<RT>(string directory)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SetCurrentDirectory(directory));

            /// <summary>
            /// Gets a unique identifier for the current managed thread.
            /// </summary>
            public static Eff<RT, int> currentManagedThreadId<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.CurrentManagedThreadId());

            /// <summary>
            /// Terminates this process and returns an exit code to the operating system.
            /// </summary>
            public static Eff<RT, Unit> exit<RT>(int exitCode)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.Exit(exitCode));

            /// <summary>
            /// Gets the exit code of the process.
            /// </summary>
            public static Eff<RT, int> exitCode<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.ExitCode());

            /// <summary>
            /// Sets the exit code of the process.
            /// </summary>
            /// <param name="exitCode">exit code of the process</param>
            public static Eff<RT, Unit> SetExitCode<RT>(int exitCode)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SetExitCode(exitCode));

            /// <summary>
            /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
            /// </summary>
            /// <param name="name">A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).</param>
            public static Eff<RT, string> expandEnvironmentVariables<RT>(string name)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.ExpandEnvironmentVariables(name));

            /// <summary>
            /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
            /// </summary>
            /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
            public static Eff<RT, Unit> failFast<RT>(Option<string> message)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.FailFast(message));

            /// <summary>
            /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
            /// </summary>
            /// <param name="message">A message that explains why the process was terminated, or null if no explanation is provided.</param>
            /// <param name="exception">An exception that represents the error that caused the termination. This is typically the exception in a catch block.</param>
            public static Eff<RT, Unit> failFast<RT>(Option<string> message, Option<Exception> exception)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.FailFast(message, exception));

            /// <summary>
            /// Returns a string array containing the command-line arguments for the current process.
            /// </summary>
            public static Eff<RT, Arr<string>> getCommandLineArgs<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetCommandLineArgs());

            /// <summary>
            // Retrieves the value of an environment variable from the current process.
            /// </summary>
            /// <param name="variable">The name of an environment variable.</param>
            public static Eff<RT, Option<string>> getEnvironmentVariable<RT>(string variable)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetEnvironmentVariable(variable));

            /// </summary>
            /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
            /// </summary>
            /// <param name="variable">The name of an environment variable.</param>
            public static Eff<RT, Option<string>> getEnvironmentVariable<RT>(string variable, EnvironmentVariableTarget target)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetEnvironmentVariable(variable, target));

            /// </summary>
            /// Retrieves all environment variable names and their values from the current process.
            /// </summary>
            public static Eff<RT, System.Collections.IDictionary> getEnvironmentVariables<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetEnvironmentVariables());

            /// <summary>
            /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
            /// </summary>
            /// <param name="target">One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.</param>
            public static Eff<RT, System.Collections.IDictionary> getEnvironmentVariables<RT>(System.EnvironmentVariableTarget target)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetEnvironmentVariables(target));

            /// <summary>
            /// Gets the path to the system special folder that is identified by the specified enumeration.
            /// </summary>
            /// <param name="folder">One of enumeration values that identifies a system special folder.</param>
            public static Eff<RT, string> getFolderPath<RT>(System.Environment.SpecialFolder folder)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetFolderPath(folder));

            /// <summary>
            /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
            /// </summary>
            /// <param name="folder">One of the enumeration values that identifies a system special folder.</param>
            /// <param name="option">One of the enumeration values that specifies options to use for accessing a special folder.</param>
            public static Eff<RT, string> getFolderPath<RT>(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetFolderPath(folder, option));

            /// <summary>
            /// Returns an array of string containing the names of the logical drives on the current computer.
            /// </summary>
            public static Eff<RT, Arr<string>> getLogicalDrives<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.GetLogicalDrives());

            /// <summary>
            /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
            /// </summary>
            public static Eff<RT, bool> hasShutdownStarted<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.HasShutdownStarted());

            /// <summary>
            /// Determines whether the current operating system is a 64-bit operating system.
            /// </summary>
            public static Eff<RT, bool> is64BitOperatingSystem<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.Is64BitOperatingSystem());

            /// <summary>
            /// Determines whether the current process is a 64-bit process.
            /// </summary>
            public static Eff<RT, bool> is64BitProcess<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.Is64BitProcess());

            /// <summary>
            /// Gets the NetBIOS name of this local computer.
            /// </summary>
            public static Eff<RT, string> machineName<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.MachineName());

            /// <summary>
            /// Gets the newline string defined for this environment.
            /// </summary>
            public static Eff<RT, string> newLine<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.NewLine());

            /// <summary>
            /// Gets an OperatingSystem object that contains the current platform identifier and version number.
            /// </summary>
            public static Eff<RT, OperatingSystem> osVersion<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.OSVersion());

            /// </summary>
            /// Gets the number of processors on the current machine.
            /// </summary>
            public static Eff<RT, int> processorCount<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.ProcessorCount());

            /// </summary>
            /// Creates, modifies, or deletes an environment variable stored in the current process.
            /// </summary>
            /// <param name="variable">The name of an environment variable.</param>
            /// <param name="value">A value to assign to variable .</param>
            public static Eff<RT, Unit> setEnvironmentVariable<RT>(string variable, Option<string> value)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SetEnvironmentVariable(variable, value));

            /// <summary>
            /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
            /// </summary>
            /// <param name="variable">The name of an environment variable.</param>
            /// <param name="value">A value to assign to variable.</param>
            /// <param name="target">One of the enumeration values that specifies the location of the environment variable.</param>
            public static Eff<RT, Unit> SetEnvironmentVariable<RT>(string variable, Option<string> value, EnvironmentVariableTarget target)
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SetEnvironmentVariable(variable, value, target));

            /// <summary>
            /// Gets current stack trace information.
            /// </summary>
            public static Eff<RT, string> stackTrace<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.StackTrace());

            /// <summary>
            /// Gets the fully qualified path of the system directory.
            /// </summary>
            public static Eff<RT, string> systemDirectory<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SystemDirectory());

            /// <summary>
            /// Gets the number of bytes in the operating system's memory page.
            /// </summary>
            public static Eff<RT, int> SystemPageSize<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.SystemPageSize());

            /// <summary>
            /// Gets the number of milliseconds elapsed since the system started.
            /// </summary>
            public static Eff<RT, int> TickCount<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.TickCount());

            // NOTE: This seems to be a newer interface, but I'm not sure how to handle it
            // Gets the number of milliseconds elapsed since the system started.
            // public long TickCount64() =>
            //     Environment.TickCount64;
            // public static Eff<RT, long> tickCount64<RT>()
            //     where RT : struct, HasEnvironment<RT> =>
            //     default(RT).EnvironmentEff.Map(e => e.TickCount64());

            /// <summary>
            /// Gets the network domain name associated with the current user.
            /// </summary>
            public static Eff<RT, string> userDomainName<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.UserDomainName());

            /// <summary>
            /// Gets a value indicating whether the current process is running in user interactive mode.
            /// </summary>
            public static Eff<RT, bool> userInteractive<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.UserInteractive());

            /// <summary>
            /// Gets the user name of the person who is currently logged on to the operating system.
            /// </summary>
            public static Eff<RT, string> userName<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.UserName());

            /// <summary>
            /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
            /// </summary>
            public static Eff<RT, Version> version<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.Version());

            /// <summary>
            /// Gets the amount of physical memory mapped to the process context.
            /// </summary>
            public static Eff<RT, long> workingSet<RT>()
                where RT : struct, HasEnvironment<RT> =>
                default(RT).EnvironmentEff.Map(e => e.WorkingSet());
        }
    }
}