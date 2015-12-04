using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    class RoleDispatchRoundRobin : IActorDispatch
    {
        readonly ProcessName role;
        readonly ProcessId leaf;
        readonly string key;
        readonly object sync = new object();

        static Map<string, int> state = Map.empty<string, int>();

        public RoleDispatchRoundRobin(ProcessName role, ProcessId leaf)
        {
            this.role = role;
            this.leaf = leaf;
            this.key = ProcessId.Top[role].Append(leaf).ToString();
        }

        private IEnumerable<ProcessId> GetWorkers() =>
            ActorContext.ClusterState
                        .Members
                        .Filter(state => state.Role == role)
                        .Keys
                        .Map(node => ProcessId.Top[node].Append(leaf));

        private IActorDispatch GetNext()
        {
            var workers = GetWorkers().ToArray();

            int index = 0;
            lock(sync)
            {
                state = state.AddOrUpdate(key, x => { index = x % workers.Length; return x + 1; }, 0);
            }
            return ActorContext.GetDispatcher(workers[index]);
        }

        public Unit Ask(object message, ProcessId sender) =>
            GetNext().Ask(message, sender);

        public Unit DispatchUnWatch(ProcessId pid) =>
            GetNext().DispatchUnWatch(pid);

        public Unit DispatchWatch(ProcessId pid) =>
            GetNext().DispatchWatch(pid);

        public Map<string, ProcessId> GetChildren() =>
            GetNext().GetChildren();

        public int GetInboxCount() =>
            GetNext().GetInboxCount();

        public Unit Kill() =>
            GetNext().Kill();

        public IObservable<T> Observe<T>() =>
            GetNext().Observe<T>();

        public IObservable<T> ObserveState<T>() =>
            GetNext().ObserveState<T>();

        public Unit Publish(object message) =>
            GetNext().Publish(message);

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            GetNext().Tell(message, sender, tag);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            GetNext().TellSystem(message, sender);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            GetNext().TellUserControl(message, sender);

        public Unit UnWatch(ProcessId pid) =>
            GetNext().UnWatch(pid);

        public Unit Watch(ProcessId pid) =>
            GetNext().Watch(pid);
    }
}