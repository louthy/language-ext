using System;
using System.Collections;
using System.Collections.Concurrent;

namespace LanguageExt.Sys
{
    /// <summary>
    /// In-memory representation of the System.Environment 
    /// </summary>
    /// <remarks>
    /// Use: MemorySystemEnvironment.InitFromSystem(), to initialise from the real System.Environment and then tweak
    ///      for use in unit-tests using the With(...) method. 
    /// </remarks>
    public class MemorySystemEnvironment
    {
        public MemorySystemEnvironment(
            ConcurrentDictionary<string, string> processEnvironmentVariables, 
            ConcurrentDictionary<string, string> userEnvironmentVariables, 
            ConcurrentDictionary<string, string> systemEnvironmentVariables, 
            int exitCode, 
            string commandLine, 
            int currentManagedThreadId, 
            Seq<string> commandLineArgs, 
            Seq<string> logicalDrives, 
            string newLine, 
            bool hasShutdownStarted,
            bool is64BitOperatingSystem, 
            bool is64BitProcess, 
            string machineName, 
            OperatingSystem osVersion, 
            int processorCount, 
            string stackTrace, 
            string systemDirectory, 
            int systemPageSize, 
            int tickCount, 
            string userDomainName, 
            bool userInteractive, 
            string userName, 
            Version version, 
            long workingSet, 
            Func<System.Environment.SpecialFolder, System.Environment.SpecialFolderOption, string> getFolderPath)
        {
            ProcessEnvironmentVariables = processEnvironmentVariables;
            UserEnvironmentVariables    = userEnvironmentVariables;
            SystemEnvironmentVariables  = systemEnvironmentVariables;
            ExitCode                    = exitCode;
            CommandLine                 = commandLine;
            CurrentManagedThreadId      = currentManagedThreadId;
            CommandLineArgs             = commandLineArgs;
            LogicalDrives               = logicalDrives;
            NewLine                     = newLine;
            HasShutdownStarted          = hasShutdownStarted;
            Is64BitOperatingSystem      = is64BitOperatingSystem;
            Is64BitProcess              = is64BitProcess;
            MachineName                 = machineName;
            OSVersion                   = osVersion;
            ProcessorCount              = processorCount;
            StackTrace                  = stackTrace;
            SystemDirectory             = systemDirectory;
            SystemPageSize              = systemPageSize;
            TickCount                   = tickCount;
            UserDomainName              = userDomainName;
            UserInteractive             = userInteractive;
            UserName                    = userName;
            Version                     = version;
            WorkingSet                  = workingSet;
            GetFolderPath               = getFolderPath;
        }

        // Mutable
        public ConcurrentDictionary<string, string> ProcessEnvironmentVariables;
        public ConcurrentDictionary<string, string> UserEnvironmentVariables;
        public ConcurrentDictionary<string, string> SystemEnvironmentVariables;
        public int ExitCode;
        public bool HasShutdownStarted;
        
        // Immutable
        public readonly string CommandLine;
        public readonly int CurrentManagedThreadId;
        public readonly Seq<string> CommandLineArgs;
        public readonly Seq<string> LogicalDrives;
        public readonly string NewLine;
        public readonly bool Is64BitOperatingSystem;
        public readonly bool Is64BitProcess;
        public readonly string MachineName;
        public readonly OperatingSystem OSVersion;
        public readonly int ProcessorCount;
        public readonly string StackTrace;
        public readonly string SystemDirectory;
        public readonly int SystemPageSize;
        public readonly int TickCount;
        public readonly string UserDomainName;
        public readonly bool UserInteractive;
        public readonly string UserName;
        public readonly Version Version;
        public readonly long WorkingSet;

        // Injectable 
        public readonly Func<System.Environment.SpecialFolder, System.Environment.SpecialFolderOption, string> GetFolderPath;
        
        public MemorySystemEnvironment With(
            ConcurrentDictionary<string, string>? ProcessEnvironmentVariables = null, 
            ConcurrentDictionary<string, string>? UserEnvironmentVariables = null, 
            ConcurrentDictionary<string, string>? SystemEnvironmentVariables = null, 
            int? ExitCode = null, 
            string? CommandLine = null, 
            int? CurrentManagedThreadId = null, 
            Seq<string>? CommandLineArgs = null, 
            Seq<string>? LogicalDrives = null, 
            string? NewLine = null, 
            bool? HasShutdownStarted = null,
            bool? Is64BitOperatingSystem = null, 
            bool? Is64BitProcess = null, 
            string? MachineName = null, 
            OperatingSystem? OSVersion = null, 
            int? ProcessorCount = null, 
            string? StackTrace = null, 
            string? SystemDirectory = null, 
            int? SystemPageSize = null, 
            int? TickCount = null, 
            string? UserDomainName = null, 
            bool? UserInteractive = null, 
            string? UserName = null, 
            Version? Version = null, 
            long? WorkingSet = null, 
            Func<System.Environment.SpecialFolder, System.Environment.SpecialFolderOption, string>? GetFolderPath = null) =>
            new MemorySystemEnvironment(
                ProcessEnvironmentVariables ?? this.ProcessEnvironmentVariables,
                UserEnvironmentVariables    ?? this.UserEnvironmentVariables,
                SystemEnvironmentVariables  ?? this.SystemEnvironmentVariables,
                ExitCode                    ?? this.ExitCode,
                CommandLine                 ?? this.CommandLine,
                CurrentManagedThreadId      ?? this.CurrentManagedThreadId,
                CommandLineArgs             ?? this.CommandLineArgs,
                LogicalDrives               ?? this.LogicalDrives,
                NewLine                     ?? this.NewLine,
                HasShutdownStarted          ?? this.HasShutdownStarted,
                Is64BitOperatingSystem      ?? this.Is64BitOperatingSystem,
                Is64BitProcess              ?? this.Is64BitProcess,
                MachineName                 ?? this.MachineName,
                OSVersion                   ?? this.OSVersion,
                ProcessorCount              ?? this.ProcessorCount,
                StackTrace                  ?? this.StackTrace,
                SystemDirectory             ?? this.SystemDirectory,
                SystemPageSize              ?? this.SystemPageSize,
                TickCount                   ?? this.TickCount,
                UserDomainName              ?? this.UserDomainName,
                UserInteractive             ?? this.UserInteractive,
                UserName                    ?? this.UserName,
                Version                     ?? this.Version,
                WorkingSet                  ?? this.WorkingSet,
                GetFolderPath               ?? this.GetFolderPath
             );

        public static MemorySystemEnvironment InitFromSystem() =>
            new MemorySystemEnvironment(
                processEnvironmentVariables: GetEnvs(EnvironmentVariableTarget.Process),
                userEnvironmentVariables: GetEnvs(EnvironmentVariableTarget.User),
                systemEnvironmentVariables: GetEnvs(EnvironmentVariableTarget.Machine),
                exitCode: System.Environment.ExitCode,
                commandLine: System.Environment.CommandLine,
                currentManagedThreadId: System.Environment.CurrentManagedThreadId,
                commandLineArgs: System.Environment.GetCommandLineArgs().ToSeq().Strict(),
                logicalDrives: System.Environment.GetLogicalDrives().ToSeq().Strict(),
                newLine: System.Environment.NewLine,
                hasShutdownStarted: System.Environment.HasShutdownStarted,
                is64BitOperatingSystem: System.Environment.Is64BitOperatingSystem,
                is64BitProcess: System.Environment.Is64BitProcess,
                machineName: System.Environment.MachineName,
                osVersion: System.Environment.OSVersion,
                processorCount: System.Environment.ProcessorCount,
                stackTrace: System.Environment.StackTrace,
                systemDirectory: System.Environment.SystemDirectory,
                systemPageSize: System.Environment.SystemPageSize,
                tickCount: System.Environment.TickCount,
                userDomainName: System.Environment.UserDomainName,
                userInteractive: System.Environment.UserInteractive,
                userName: System.Environment.UserName,
                version: System.Environment.Version,
                workingSet: System.Environment.WorkingSet,
                getFolderPath: System.Environment.GetFolderPath
                );

        static ConcurrentDictionary<string, string> GetEnvs(EnvironmentVariableTarget target)
        {
            var dict = new ConcurrentDictionary<string, string>();
            foreach (DictionaryEntry de in System.Environment.GetEnvironmentVariables(target))
            {
                dict.TryAdd(de.Key.ToString(), de.Value.ToString());
            }

            return dict;
        }
    }
}
