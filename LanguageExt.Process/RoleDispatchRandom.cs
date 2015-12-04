using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    class RoleDispatchRandom : IActorDispatch
    {
        readonly ProcessName role;
        readonly ProcessId leaf;

        public RoleDispatchRandom(ProcessName role, ProcessId leaf)
        {
            this.role = role;
            this.leaf = leaf;
        }

        private IEnumerable<ProcessId> GetWorkers() =>
            ActorContext.ClusterState
                        .Members
                        .Filter(state => state.Role == role)
                        .Keys
                        .Map(node => ProcessId.Top[node].Append(leaf));

        private IActorDispatch GetRandom()
        {
            var workers = GetWorkers().ToArray();
            return ActorContext.GetDispatcher(workers[random(workers.Length)]);
        }

        public Unit Ask(object message, ProcessId sender) =>
            GetRandom().Ask(message, sender);

        public Unit DispatchUnWatch(ProcessId pid) =>
            GetRandom().DispatchUnWatch(pid);

        public Unit DispatchWatch(ProcessId pid) =>
            GetRandom().DispatchWatch(pid);

        public Map<string, ProcessId> GetChildren() =>
            GetRandom().GetChildren();

        public int GetInboxCount() =>
            GetRandom().GetInboxCount();

        public Unit Kill() =>
            GetRandom().Kill();

        public IObservable<T> Observe<T>() =>
            GetRandom().Observe<T>();

        public IObservable<T> ObserveState<T>() =>
            GetRandom().ObserveState<T>();

        public Unit Publish(object message) =>
            GetRandom().Publish(message);

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            GetRandom().Tell(message, sender, tag);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            GetRandom().TellSystem(message, sender);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            GetRandom().TellUserControl(message, sender);

        public Unit UnWatch(ProcessId pid) =>
            GetRandom().UnWatch(pid);

        public Unit Watch(ProcessId pid) =>
            GetRandom().Watch(pid);
    }
}