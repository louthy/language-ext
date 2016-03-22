using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    class ActorConfigToken
    { }

    class PidToken : ActorConfigToken
    {
        public readonly ProcessId Pid;
        public PidToken(ProcessId pid)
        {
            Pid = pid;
        }
    }

    class FlagsToken : ActorConfigToken
    {
        public readonly ProcessFlags Flags;
        public FlagsToken(ProcessFlags flags)
        {
            Flags = flags;
        }
    }

    class MailboxSizeToken : ActorConfigToken
    {
        public readonly int Size;
        public MailboxSizeToken(int size)
        {
            Size = size;
        }
    }

    class StrategyToken : ActorConfigToken
    {
        public readonly State<StrategyContext, Unit> Strategy;
        public StrategyToken(State<StrategyContext, Unit> strategy)
        {
            Strategy = strategy;
        }
    }

    class SettingsToken : ActorConfigToken
    {
        public readonly Map<string, string> Settings;

        public SettingsToken(Map<string, string> settings)
        {
            Settings = settings;
        }
    }
}
