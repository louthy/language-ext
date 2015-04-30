using System;

namespace LanguageExt
{
    /// <summary>
    /// ActorProxy
    /// This class will eventually be responsible for remote messaging
    /// </summary>
    internal class ActorProxy : Actor<ActorProxyConfig, object>
    {
        public ActorProxy(ProcessId parent, ProcessName name, Func<ActorProxyConfig, object, ActorProxyConfig> actor, Func<ActorProxyConfig> setup)
            :
            base(parent,name,actor,setup)
        {
        }
    }

    internal class ActorProxyConfig
    {
        public ActorProxyConfig(ProcessId dest)
        {
            Destination = dest;
        }

        public ProcessId Destination { get; }
    }

    internal static class ActorProxyTemplate
    {
        public static Func<ActorProxyConfig, object, ActorProxyConfig> Registered => 
            (config,msg) =>
            {
                Process.tell(config.Destination, msg, Process.Sender);
                return config;
            };
    }
}