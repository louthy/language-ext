using LanguageExt.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    class NullProcess : IActor
    {
        public Map<string, ActorItem> Children => Map.empty<string, ActorItem>();
        public ProcessId Id => ProcessId.Top;
        public ProcessFlags Flags => ProcessFlags.Default;
        public ProcessName Name => "$";
        public ActorItem Parent => new ActorItem(new NullProcess(), new NullInbox(), ProcessFlags.Default);
        public State<StrategyContext, Unit> Strategy => Process.DefaultStrategy;
        public Unit Restart() => unit;
        public Unit Startup() => unit;
        public Unit Shutdown(bool maintainState) => unit;
        public Unit LinkChild(ActorItem item) => unit;
        public Unit UnlinkChild(ProcessId item) => unit;
        public Unit AddWatcher(ProcessId item) => unit;
        public Unit RemoveWatcher(ProcessId item) => unit;
        public Unit DispatchWatch(ProcessId item) => unit;
        public Unit DispatchUnWatch(ProcessId item) => unit;
        public Unit Publish(object message) => unit;
        public IObservable<object> PublishStream => null;
        public IObservable<object> StateStream => null;
        public InboxDirective ProcessTerminated(ProcessId pid) => InboxDirective.Default;
        public InboxDirective ProcessMessage(object message) => InboxDirective.Default;
        public InboxDirective ProcessAsk(ActorRequest request) => InboxDirective.Default;
        public Unit AddSubscription(ProcessId pid, IDisposable sub) => Unit.Default;
        public Unit RemoveSubscription(ProcessId pid) => Unit.Default;
        public int GetNextRoundRobinIndex() => 0;
        public R ProcessRequest<R>(ProcessId pid, object message) => default(R);
        public Unit ProcessResponse(ActorResponse response) => unit;
        public Unit ShutdownProcess(bool maintainState) => unit;

        public void Dispose()
        {
        }

    }
}
