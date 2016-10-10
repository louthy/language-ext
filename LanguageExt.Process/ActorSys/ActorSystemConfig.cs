﻿using LanguageExt.UnitsOfMeasure;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization.Formatters;
using System.Reflection;

namespace LanguageExt
{
    class ActorSystemConfig
    {
        public readonly ProcessName RootProcessName              = "root";
        public readonly ProcessName SystemProcessName            = "system";
        public readonly ProcessName UserProcessName              = "user";
        public readonly ProcessName DeadLettersProcessName       = "dead-letters";
        public readonly ProcessName ErrorsProcessName            = "errors";
        public readonly ProcessName AskProcessName               = "ask";
        public readonly ProcessName ReplyProcessName             = "reply";
        public readonly ProcessName InboxShutdownProcessName     = "inbox-shutdown";
        public readonly ProcessName Sessions                     = "sessions";
        public readonly ProcessName MonitorProcessName           = "monitor";

        public readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

        public readonly static ActorSystemConfig Default =
            new ActorSystemConfig();
    }

    public class ProcessSetting
    {
        public const int DefaultMailboxSize             = -1;
        public const int UnlimitedMaibox                = int.MaxValue;

        public static readonly State<Exception, Option<Directive>>[] StandardDirectives = {
            Strategy.With<ProcessKillException>(Directive.Stop),
            Strategy.With<ProcessSetupException>(Directive.Stop)
        };
    }
}
