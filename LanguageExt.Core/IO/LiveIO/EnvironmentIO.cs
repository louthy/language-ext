using System;
using static LanguageExt.Prelude;

namespace LanguageExt.LiveIO
{
    public struct EnvironmentIO : Interfaces.EnvironmentIO
    {
        public static Interfaces.EnvironmentIO Default =>
            new EnvironmentIO();

        /// <summary>
        /// Gets the command line for this process.
        /// </summary>
        public string CommandLine() =>
            Environment.CommandLine;

        /// <summary>
        /// Gets the fully qualified path of the current working directory.
        /// </summary>
        public string CurrentDirectory() =>
            Environment.CurrentDirectory;

        /// <summary>
        /// Sets the fully qualified path of the current working directory.
        /// </summary>
        /// directory: fully qualified path of the current working directory.
        public Unit SetCurrentDirectory(string directory)
        {
            Environment.CurrentDirectory = directory;
            return unit;
        }

        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// </summary>
        public int CurrentManagedThreadId() =>
            Environment.CurrentManagedThreadId;

        /// <summary>
        /// Terminates this process and returns an exit code to the operating system.
        /// </summary>
        public Unit Exit(int exitCode)
        {
            Environment.Exit(exitCode);
            return unit;
        }

        /// <summary>
        /// Gets the exit code of the process.
        /// </summary>
        public int ExitCode() =>
            Environment.ExitCode;

        /// <summary>
        /// Sets the exit code of the process.
        /// </summary>
        // exitCode: exit code of the process
        public Unit SetExitCode(int exitCode)
        {
            Environment.ExitCode = exitCode;
            return unit;
        }

        /// <summary>
        /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// name: A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).
        public string ExpandEnvironmentVariables(string name) =>
            Environment.ExpandEnvironmentVariables(name);

        /// <summary>
        /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message in error reporting to Microsoft.
        /// </summary>
        /// message: A message that explains why the process was terminated, or null if no explanation is provided.
        public Unit FailFast(Option<string> message)
        {
            Environment.FailFast(message.IfNone(() => null));
            return unit;
        }

        /// <summary>
        /// Immediately terminates a process after writing a message to the Windows Application event log, and then includes the message and exception information in error reporting to Microsoft.
        /// </summary>
        /// message: A message that explains why the process was terminated, or null if no explanation is provided.
        /// exception: An exception that represents the error that caused the termination. This is typically the exception in a catch block.
        public Unit FailFast(Option<string> message, Option<Exception> exception)
        {
            Environment.FailFast(message.IfNone(() => null), exception.IfNone(() => null));
            return unit;
        }

        /// <summary>
        /// Returns a string array containing the command-line arguments for the current process.
        /// </summary>
        public Arr<string> GetCommandLineArgs() =>
            Environment.GetCommandLineArgs();

        /// <summary>
        // Retrieves the value of an environment variable from the current process.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable) =>
            Environment.GetEnvironmentVariable(variable);

        /// </summary>
        /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
            Environment.GetEnvironmentVariable(variable, target);

        /// </summary>
        /// Retrieves all environment variable names and their values from the current process.
        /// </summary>
        public System.Collections.IDictionary GetEnvironmentVariables() =>
            Environment.GetEnvironmentVariables();

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// target: One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.
        public System.Collections.IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target) =>
            Environment.GetEnvironmentVariables(target);

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration.
        /// </summary>
        /// folder: One of enumeration values that identifies a system special folder.
        public string GetFolderPath(Environment.SpecialFolder folder) =>
            Environment.GetFolderPath(folder);

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
        /// </summary>
        // folder: One of the enumeration values that identifies a system special folder.
        /// option: One of the enumeration values that specifies options to use for accessing a special folder.
        public string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option) =>
            Environment.GetFolderPath(folder, option);

        /// <summary>
        /// Returns an array of string containing the names of the logical drives on the current computer.
        /// </summary>
        /// string[] Environment.GetLogicalDrives()
        public Arr<string> GetLogicalDrives() =>
            Environment.GetLogicalDrives();

        /// <summary>
        /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
        /// </summary>
        public bool HasShutdownStarted() =>
            Environment.HasShutdownStarted;

        /// <summary>
        // Determines whether the current operating system is a 64-bit operating system.
        /// </summary>
        public bool Is64BitOperatingSystem() =>
            Environment.Is64BitOperatingSystem;

        /// <summary>
        // Determines whether the current process is a 64-bit process.
        /// </summary>
        public bool Is64BitProcess() =>
            Environment.Is64BitProcess;

        /// <summary>
        // Gets the NetBIOS name of this local computer.
        /// </summary>
        public string MachineName() =>
            Environment.MachineName;

        /// <summary>
        /// Gets the newline string defined for this environment.
        /// </summary>
        public string NewLine() =>
            Environment.NewLine;

        /// <summary>
        /// Gets an OperatingSystem object that contains the current platform identifier and version number.
        /// </summary>
        public OperatingSystem OSVersion() =>
            Environment.OSVersion;

        /// </summary>
        /// Gets the number of processors on the current machine.
        /// </summary>
        public int ProcessorCount() =>
            Environment.ProcessorCount;

        /// </summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process.
        /// </summary>
        // variable: The name of an environment variable.
        // value: A value to assign to variable .
        public Unit SetEnvironmentVariable(string variable, Option<string> value)
        {
            Environment.SetEnvironmentVariable(variable, value.IfNone(() => null));
            return unit;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
        /// </summary>
        // variable: The name of an environment variable.
        // value: A value to assign to variable.
        // target: One of the enumeration values that specifies the location of the environment variable.
        public Unit SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target)
        {
            Environment.SetEnvironmentVariable(variable, value.IfNone(() => null), target);
            return unit;
        }

        /// <summary>
        /// Gets current stack trace information.
        /// </summary>
        public string StackTrace() =>
            Environment.StackTrace;

        /// <summary>
        /// Gets the fully qualified path of the system directory.
        /// </summary>
        public string SystemDirectory() =>
            Environment.SystemDirectory;

        /// <summary>
        /// Gets the number of bytes in the operating system's memory page.
        /// </summary>
        public int SystemPageSize() =>
            Environment.SystemPageSize;

        /// <summary>
        /// Gets the number of milliseconds elapsed since the system started.
        /// </summary>
        public int TickCount() =>
            Environment.TickCount;

        // NOTE: This seems to be a newer interface, but I'm not sure how to handle it
        // Gets the number of milliseconds elapsed since the system started.
        // public long TickCount64() =>
        //     Environment.TickCount64;

        /// <summary>
        /// Gets the network domain name associated with the current user.
        /// </summary>
        public string UserDomainName() =>
            Environment.UserDomainName;

        /// <summary>
        /// Gets a value indicating whether the current process is running in user interactive mode.
        /// </summary>
        public bool UserInteractive() =>
            Environment.UserInteractive;

        /// <summary>
        /// Gets the user name of the person who is currently logged on to the operating system.
        /// </summary>
        public string UserName() =>
            Environment.UserName;

        /// <summary>
        /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
        /// </summary>
        public Version Version() =>
            Environment.Version;

        /// <summary>
        /// Gets the amount of physical memory mapped to the process context.
        /// </summary>
        public long WorkingSet() =>
            Environment.WorkingSet;
    }
}
