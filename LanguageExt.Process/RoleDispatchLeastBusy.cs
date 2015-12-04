using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    class RoleDispatchLeastBusy : IActorDispatch
    {
        readonly ProcessName role;
        readonly ProcessId leaf;

        public RoleDispatchLeastBusy(ProcessName role, ProcessId leaf)
        {
            this.role = role;
            this.leaf = leaf;
        }

        private IEnumerable<ProcessId> GetWorkers()
        {
            return ActorContext.ClusterState
                        .Members
                        .Filter(state => state.Role == role)
                        .Keys
                        .Map(node => ProcessId.Top[node].Append(leaf));
        }

        private IActorDispatch GetLeastBusy() =>
            (from child in GetWorkers().Map(c => Tuple(c, ActorContext.GetDispatcher(c)))
             let count = child.Item2.GetInboxCount()
             where count >= 0
             orderby count
             select child.Item2)
            .FirstOrDefault();

        public Unit Ask(object message, ProcessId sender) =>
            GetLeastBusy().Ask(message, sender);

        public Unit DispatchUnWatch(ProcessId pid) =>
            GetLeastBusy().DispatchUnWatch(pid);

        public Unit DispatchWatch(ProcessId pid) =>
            GetLeastBusy().DispatchWatch(pid);

        public Map<string, ProcessId> GetChildren() =>
            GetLeastBusy().GetChildren();

        public int GetInboxCount() =>
            GetLeastBusy().GetInboxCount();

        public Unit Kill() =>
            GetLeastBusy().Kill();

        public IObservable<T> Observe<T>() =>
            GetLeastBusy().Observe<T>();

        public IObservable<T> ObserveState<T>() =>
            GetLeastBusy().ObserveState<T>();

        public Unit Publish(object message) =>
            GetLeastBusy().Publish(message);

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            GetLeastBusy().Tell(message, sender, tag);

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            GetLeastBusy().TellSystem(message, sender);

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            GetLeastBusy().TellUserControl(message, sender);

        public Unit UnWatch(ProcessId pid) =>
            GetLeastBusy().UnWatch(pid);

        public Unit Watch(ProcessId pid) =>
            GetLeastBusy().Watch(pid);
    }
}