using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    class ActorDispatchGroup : IActorDispatch
    {
        readonly ProcessId[] group;
        readonly int count;

        public ActorDispatchGroup(IEnumerable<ProcessId> group)
        {
            this.group = group.ToArray();
            this.count = this.group.Length;
        }

        private IEnumerable<IActorDispatch> GetWorkers() =>
            group.Map(pid => ActorContext.GetDispatcher(pid));

        private Unit IterRoleMembers(Action<IActorDispatch> action) =>
            GetWorkers().Iter(action);

        private IEnumerable<R> MapRoleMembers<R>(Func<IActorDispatch, R> map) =>
            count > 1
                ? GetWorkers().Map(map).AsParallel()
                : GetWorkers().Map(map);

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

        public Either<string, bool> CanAccept<T>() =>
            List.fold(MapRoleMembers(disp => disp.CanAccept<T>()), true, (s, x) => s && x.IsRight);

        public Either<string, bool> HasStateTypeOf<T>() =>
            List.fold(MapRoleMembers(disp => disp.HasStateTypeOf<T>()), true, (s, x) => s && x.IsRight);

        public Unit Kill() =>
            IterRoleMembers(d => d.Kill());

        public Unit Shutdown() =>
            IterRoleMembers(d => d.Shutdown());

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

        public bool IsLocal => false;
    }
}