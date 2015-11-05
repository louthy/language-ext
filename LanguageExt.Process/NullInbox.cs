using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    class NullInbox : IActorInbox
    {
        public Unit Shutdown() => unit;
        public Unit Startup(IActor pid, ActorItem parent, Option<ICluster> cluster, int version) => unit;
        public Unit Tell(object message, ProcessId sender) => unit;
        public Unit TellSystem(SystemMessage message) => unit;
        public Unit TellUserControl(UserControlMessage message) => unit;
        public Unit Pause() => unit;
        public Unit Unpause() => unit;
        public bool IsPaused => false;

        public void Dispose()
        {
        }
    }
}
