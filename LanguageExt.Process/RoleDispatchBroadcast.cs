using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    class RoleDispatchBroadcast : IActorDispatch
    {
        readonly ProcessName role;
        readonly ProcessId leaf;

        public RoleDispatchBroadcast(ProcessName role, ProcessId leaf)
        {
            this.role = role;
            this.leaf = leaf;
        }

        private IEnumerable<IActorDispatch> GetWorkers() =>
            ActorContext.ClusterState
                        .Members
                        .Filter(state => state.Role == role)
                        .Keys
                        .Map(node => ActorContext.GetDispatcher(ProcessId.Top[node].Append(leaf)));

        private Unit IterRoleMembers(Action<IActorDispatch> action) =>
            GetWorkers().Iter(action);

        private IEnumerable<R> MapRoleMembers<R>(Func<IActorDispatch, R> map) =>
            GetWorkers().Map(map).AsParallel();

        public Unit Ask(object message, ProcessId sender) =>
            IterRoleMembers(d => d.Ask(message, sender));

        public Unit DispatchUnWatch(ProcessId pid) =>
            IterRoleMembers(d => d.DispatchUnWatch(pid));

        public Unit DispatchWatch(ProcessId pid) =>
            IterRoleMembers(d => d.DispatchWatch(pid));

        public Map<string, ProcessId> GetChildren() =>
            List.fold(
                MapRoleMembers(disp => disp.GetChildren()),
                Map.empty<string, ProcessId>(), 
                (s, x) => s + x
                );

        public int GetInboxCount() =>
            List.fold(MapRoleMembers(disp => disp.GetInboxCount()), 0, (s, x) => s + x);

        public Unit Kill() =>
            IterRoleMembers(d => d.Kill());

        public IObservable<T> Observe<T>() =>
            Observable.Merge(MapRoleMembers(disp => disp.Observe<T>()));

        public IObservable<T> ObserveState<T>() =>
            Observable.Merge(MapRoleMembers(disp => disp.ObserveState<T>()));

        public Unit Publish(object message) =>
            IterRoleMembers(d => d.Publish(message));

        public Unit Tell(object message, ProcessId sender, Message.TagSpec tag) =>
            IterRoleMembers(d => d.Tell(message, sender, tag));

        public Unit TellSystem(SystemMessage message, ProcessId sender) =>
            IterRoleMembers(d => d.TellSystem(message, sender));

        public Unit TellUserControl(UserControlMessage message, ProcessId sender) =>
            IterRoleMembers(d => d.TellUserControl(message, sender));

        public Unit UnWatch(ProcessId pid) =>
            IterRoleMembers(d => d.UnWatch(pid));

        public Unit Watch(ProcessId pid) =>
            IterRoleMembers(d => d.Watch(pid));
    }
}