using System;
using System.Collections;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live
{
    public readonly struct EnvironmentIO : Sys.Traits.EnvironmentIO
    {
        public static readonly Sys.Traits.EnvironmentIO Default =
            new EnvironmentIO();

        /// <summary>
        /// Gets the command line for this process.
        /// </summary>
        public string CommandLine() =>
            System.Environment.CommandLine;

        /// <summary>
        /// Sets the fully qualified path of the current working directory.
        /// </summary>
        /// directory: fully qualified path of the current working directory.
        public Unit SetCurrentDirectory(string directory)
        {
            System.Environment.CurrentDirectory = directory;
            return unit;
        }

        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// </summary>
        public int CurrentManagedThreadId() =>
            System.Environment.CurrentManagedThreadId;

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
            System.Environment.ExitCode;

        /// <summary>
        /// Sets the exit code of the process.
        /// </summary>
        // exitCode: exit code of the process
        public Unit SetExitCode(int exitCode)
        {
            System.Environment.ExitCode = exitCode;
            return unit;
        }

        /// <summary>
        /// Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// name: A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%).
        public string ExpandEnvironmentVariables(string name) =>
            System.Environment.ExpandEnvironmentVariables(name);

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
            System.Environment.FailFast(message.IfNone(""), exception.IfNone(() => BottomException.Default));
            return unit;
        }

        /// <summary>
        /// Returns a string array containing the command-line arguments for the current process.
        /// </summary>
        public Seq<string> GetCommandLineArgs() =>
            System.Environment.GetCommandLineArgs().ToSeq();

        /// <summary>
        /// Retrieves the value of an environment variable from the current process.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable) =>
            System.Environment.GetEnvironmentVariable(variable);

        /// <summary>
        /// Retrieves the value of an environment variable from the current process or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// variable: The name of an environment variable.
        public Option<string> GetEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
            System.Environment.GetEnvironmentVariable(variable, target);

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process.
        /// </summary>
        public HashMap<string, string> GetEnvironmentVariables()
        {
            var hm = HashMap<string, string>();
            foreach (DictionaryEntry de in System.Environment.GetEnvironmentVariables())
            {
                hm = hm.Add(de.Key.ToString(), de.Value.ToString());
            }
            return hm;
        }

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process, or from the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// target: One of the System.EnvironmentVariableTarget values. Only System.EnvironmentVariableTarget.Process is supported on .NET Core running on Unix-based systems.
        public HashMap<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target)
        {
            var hm = HashMap<string, string>();
            foreach (DictionaryEntry de in System.Environment.GetEnvironmentVariables(target))
            {
                hm = hm.Add(de.Key.ToString(), de.Value.ToString());
            }
            return hm;
        }

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration.
        /// </summary>
        /// folder: One of enumeration values that identifies a system special folder.
        public string GetFolderPath(System.Environment.SpecialFolder folder) =>
            System.Environment.GetFolderPath(folder);

        /// <summary>
        /// Gets the path to the system special folder that is identified by the specified enumeration, and uses a specified option for accessing special folders.
        /// </summary>
        /// folder: One of the enumeration values that identifies a system special folder.
        /// option: One of the enumeration values that specifies options to use for accessing a special folder.
        public string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option) =>
            System.Environment.GetFolderPath(folder, option);

        /// <summary>
        /// Returns an array of string containing the names of the logical drives on the current computer.
        /// </summary>
        /// string[] Environment.GetLogicalDrives()
        public Seq<string> GetLogicalDrives() =>
            System.Environment.GetLogicalDrives().ToSeq();

        /// <summary>
        /// Gets a value that indicates whether the current application domain is being unloaded or the common language runtime (CLR) is shutting down.
        /// </summary>
        public bool HasShutdownStarted() =>
            System.Environment.HasShutdownStarted;

        /// <summary>
        /// Determines whether the current operating system is a 64-bit operating system.
        /// </summary>
        public bool Is64BitOperatingSystem() =>
            System.Environment.Is64BitOperatingSystem;

        /// <summary>
        /// Determines whether the current process is a 64-bit process.
        /// </summary>
        public bool Is64BitProcess() =>
            System.Environment.Is64BitProcess;

        /// <summary>
        /// Gets the NetBIOS name of this local computer.
        /// </summary>
        public string MachineName() =>
            System.Environment.MachineName;

        /// <summary>
        /// Gets the newline string defined for this environment.
        /// </summary>
        public string NewLine() =>
            System.Environment.NewLine;

        /// <summary>
        /// Gets an OperatingSystem object that contains the current platform identifier and version number.
        /// </summary>
        public OperatingSystem OSVersion() =>
            System.Environment.OSVersion;

        /// <summary>
        /// Gets the number of processors on the current machine.
        /// </summary>
        public int ProcessorCount() =>
            System.Environment.ProcessorCount;

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process.
        /// </summary>
        /// variable: The name of an environment variable.
        /// value: A value to assign to variable .
        public Unit SetEnvironmentVariable(string variable, Option<string> value)
        {
            System.Environment.SetEnvironmentVariable(variable, value.IfNone(""));
            return unit;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process or in the Windows operating system registry key reserved for the current user or local machine.
        /// </summary>
        /// variable: The name of an environment variable.
        /// value: A value to assign to variable.
        /// target: One of the enumeration values that specifies the location of the environment variable.
        public Unit SetEnvironmentVariable(string variable, Option<string> value, EnvironmentVariableTarget target)
        {
            System.Environment.SetEnvironmentVariable(variable, value.IfNone(""), target);
            return unit;
        }

        /// <summary>
        /// Gets current stack trace information.
        /// </summary>
        public string StackTrace() =>
            System.Environment.StackTrace;

        /// <summary>
        /// Gets the fully qualified path of the system directory.
        /// </summary>
        public string SystemDirectory() =>
            System.Environment.SystemDirectory;

        /// <summary>
        /// Gets the number of bytes in the operating system's memory page.
        /// </summary>
        public int SystemPageSize() =>
            System.Environment.SystemPageSize;

        /// <summary>
        /// Gets the number of milliseconds elapsed since the system started.
        /// </summary>
        public int TickCount() =>
            System.Environment.TickCount;

        // NOTE: This seems to be a newer interface, but I'm not sure how to handle it
        // Gets the number of milliseconds elapsed since the system started.
        // public long TickCount64() =>
        //     Environment.TickCount64;

        /// <summary>
        /// Gets the network domain name associated with the current user.
        /// </summary>
        public string UserDomainName() =>
            System.Environment.UserDomainName;

        /// <summary>
        /// Gets a value indicating whether the current process is running in user interactive mode.
        /// </summary>
        public bool UserInteractive() =>
            System.Environment.UserInteractive;

        /// <summary>
        /// Gets the user name of the person who is currently logged on to the operating system.
        /// </summary>
        public string UserName() =>
            System.Environment.UserName;

        /// <summary>
        /// Gets a Version object that describes the major, minor, build, and revision numbers of the common language runtime.
        /// </summary>
        public Version Version() =>
            System.Environment.Version;

        /// <summary>
        /// Gets the amount of physical memory mapped to the process context.
        /// </summary>
        public long WorkingSet() =>
            System.Environment.WorkingSet;
    }
}
