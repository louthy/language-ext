using System;
using System.Collections.Concurrent;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    public readonly struct EnvironmentIO : Sys.Traits.EnvironmentIO
    {
        readonly MemorySystemEnvironment env;

        public EnvironmentIO(MemorySystemEnvironment env) =>
            this.env = env;
        
        /// <summary>
        /// Gets the command line for this process.
        /// </summary>
        public string CommandLine() =>
            env.CommandLine;

        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// </summary>
        public int CurrentManagedThreadId() =>
            env.CurrentManagedThreadId;

        /// <summary>
        /// Terminates this process and returns an exit code to the operating system.
        /// </summary>
        public Unit Exit(int exitCode)
        {
            System.Environment.Exit(exitCode);
            return unit;
        }

        /// <summary>
        /// Gets the exit code of the process.
        /// </summary>
        public int ExitCode() =>
            env.ExitCode;

        /// <summary>
        /// Sets the exit code of the process.
        /// </summary>
        // exitCode: exit code of the process
        public Unit SetExitCode(int exitCode)
        {
            env.ExitCode = exitCode;
            return unit;
        }

        /// <summary>
        /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// name: A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).
        public string ExpandEnvironmentVariables(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
        /// </summary>
        /// message: A message that explains why the process was terminated, or null if no explanation is provided.
        public Unit FailFast(Option<string> message)
        {
            System.Environment.FailFast(message.IfNone(""));
            return unit;
        }

        /// <summary>
        /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
        /// </summary>
        /// message: A message that explains why the process was terminated, or null if no explanation is provided.
        /// exception: An exception that represents the error that caused the termination. This is typically the exception in a catch block.
        public Unit FailFast(Option<string> message, Option<Exception> exception)
        {
            System.Environment.FailFast(message.IfNone(""), exception.IfNone(BottomException.Default));
            return unit;
        }

        /// <summary>
        /// Returns a string array containing the command-line arguments for the current process.
        /// </summary>
        public Seq<string> GetCommandLineArgs() =>
            env.CommandLineArgs;

        /// <summary>
        /// Retrieves the value of an environment variable from the current process.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable)
        {
            if (env.ProcessEnvironmentVariables.ContainsKey(variable))
            {
                return env.ProcessEnvironmentVariables[variable];
            }
            else if (env.UserEnvironmentVariables.ContainsKey(variable))
            {
                return env.UserEnvironmentVariables[variable];
            }
            else if (env.SystemEnvironmentVariables.ContainsKey(variable))
            {
                return env.SystemEnvironmentVariables[variable];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process && env.ProcessEnvironmentVariables.ContainsKey(variable))
            {
                return env.ProcessEnvironmentVariables[variable];
            }
            else if (target == EnvironmentVariableTarget.User && env.UserEnvironmentVariables.ContainsKey(variable))
            {
                return env.UserEnvironmentVariables[variable];
            }
            else if (target == EnvironmentVariableTarget.Machine && env.SystemEnvironmentVariables.ContainsKey(variable))
            {
                return env.SystemEnvironmentVariables[variable];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process.
        /// </summary>
        public HashMap<string, string> GetEnvironmentVariables() =>
            env.ProcessEnvironmentVariables.ToHashMap() +
            env.UserEnvironmentVariables.ToHashMap() +
            env.SystemEnvironmentVariables.ToHashMap();

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// target: One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.
        public HashMap<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target) =>
            target switch
            {
                EnvironmentVariableTarget.Process => env.ProcessEnvironmentVariables.ToHashMap(),
                EnvironmentVariableTarget.User => env.UserEnvironmentVariables.ToHashMap(),
                EnvironmentVariableTarget.Machine => env.SystemEnvironmentVariables.ToHashMap(),
                _ => default
            };

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration.
        /// </summary>
        /// folder: One of enumeration values that identifies a system special folder.
        public string GetFolderPath(System.Environment.SpecialFolder folder) =>
            env.GetFolderPath(folder, System.Environment.SpecialFolderOption.None);

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
        /// </summary>
        /// folder: One of the enumeration values that identifies a system special folder.
        /// option: One of the enumeration values that specifies options to use for accessing a special folder.
        public string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option) =>
            env.GetFolderPath(folder, option);

        /// <summary>
        /// Returns an array of string containing the names of the logical drives on the current computer.
        /// </summary>
        /// string[] Environment.GetLogicalDrives()
        public Seq<string> GetLogicalDrives() =>
            env.LogicalDrives;

        /// <summary>
        /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
        /// </summary>
        public bool HasShutdownStarted() =>
            env.HasShutdownStarted;

        /// <summary>
        /// Determines whether the current operating system is a 64-bit operating system.
        /// </summary>
        public bool Is64BitOperatingSystem() =>
            env.Is64BitOperatingSystem;

        /// <summary>
        /// Determines whether the current process is a 64-bit process.
        /// </summary>
        public bool Is64BitProcess() =>
            env.Is64BitProcess;

        /// <summary>
        /// Gets the NetBIOS name of this local computer.
        /// </summary>
        public string MachineName() =>
            env.MachineName;

        /// <summary>
        /// Gets the newline string defined for this environment.
        /// </summary>
        public string NewLine() =>
            env.NewLine;

        /// <summary>
        /// Gets an OperatingSystem object that contains the current platform identifier and version number.
        /// </summary>
        public OperatingSystem OSVersion() =>
            env.OSVersion;

        /// <summary>
        /// Gets the number of processors on the current machine.
        /// </summary>
        public int ProcessorCount() =>
            env.ProcessorCount;

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process.
        /// </summary>
        /// variable: The name of an environment variable.
        /// value: A value to assign to variable .
        public Unit SetEnvironmentVariable(string variable, Option<string> value)
        {
            if (value.IsSome)
            {
                env.ProcessEnvironmentVariables.AddOrUpdate(variable, (string)value, (_, _) => (string)value);
            }
            else
            {
                env.ProcessEnvironmentVariables.TryRemove(variable, out var _);
            }
            return default;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
        /// </summary>
        /// variable: The name of an environment variable.
        /// value: A value to assign to variable.
        /// target: One of the enumeration values that specifies the location of the environment variable.
        public Unit SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target)
        {
            switch (target)
            {
                case EnvironmentVariableTarget.Process:
                    if (value.IsSome)
                    {
                        env.ProcessEnvironmentVariables.AddOrUpdate(variable, (string)value, (_, _) => (string)value);
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
                        env.UserEnvironmentVariables.TryRemove(variable, out var _);
                    }
                    break;

                case EnvironmentVariableTarget.Machine:
                    if (value.IsSome)
                    {
                        env.SystemEnvironmentVariables.AddOrUpdate(variable, (string)value, (_, _) => (string)value);
                    }
                    else
                    {
                        env.SystemEnvironmentVariables.TryRemove(variable, out var _);
                    }
                    break;
            }
            return default;
        }

        /// <summary>
        /// Gets current stack trace information.
        /// </summary>
        public string StackTrace() =>
            env.StackTrace;

        /// <summary>
        /// Gets the fully qualified path of the system directory.
        /// </summary>
        public string SystemDirectory() =>
            env.SystemDirectory;

        /// <summary>
        /// Gets the number of bytes in the operating system's memory page.
        /// </summary>
        public int SystemPageSize() =>
            env.SystemPageSize;

        /// <summary>
        /// Gets the number of milliseconds elapsed since the system started.
        /// </summary>
        public int TickCount() =>
            env.TickCount;

        // NOTE: This seems to be a newer interface, but I'm not sure how to handle it
        // Gets the number of milliseconds elapsed since the system started.
        // public long TickCount64() =>
        //     Environment.TickCount64;

        /// <summary>
        /// Gets the network domain name associated with the current user.
        /// </summary>
        public string UserDomainName() =>
            env.UserDomainName;

        /// <summary>
        /// Gets a value indicating whether the current process is running in user interactive mode.
        /// </summary>
        public bool UserInteractive() =>
            env.UserInteractive;

        /// <summary>
        /// Gets the user name of the person who is currently logged on to the operating system.
        /// </summary>
        public string UserName() =>
            env.UserName;

        /// <summary>
        /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
        /// </summary>
        public Version Version() =>
            env.Version;

        /// <summary>
        /// Gets the amount of physical memory mapped to the process context.
        /// </summary>
        public long WorkingSet() =>
            env.WorkingSet;
    }
}
