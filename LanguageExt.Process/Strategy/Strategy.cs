using System;
using System.Linq;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Strategy
    {
        /// <summary>
        /// Compose a sequence of state computations
        /// </summary>
        public static State<StrategyContext, Unit> Compose(params State<StrategyContext, Unit>[] stages) =>
            state => StateResult.Return(stages.Fold(state, (s, c) => c(s).State), unit);

        /// <summary>
        /// One-for-one strategy
        /// This strategy affects only the process that failed
        /// </summary>
        /// <param name="stages">Set of computations to compose that results in a behaviour 
        /// for the strategy</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> OneForOne(params State<StrategyContext, Unit>[] stages) =>
            state => Compose(stages)(state.With(Affects: new ProcessId[1] { state.Self }));

        /// <summary>
        /// All-for-one strategy
        /// This strategy affects the process that failed and its siblings
        /// </summary>
        /// <param name="stages">Set of computations to compose that results in a behaviour 
        /// for the strategy</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> AllForOne(params State<StrategyContext, Unit>[] stages) =>
            state => Compose(stages)(state.With(Affects: state.Siblings));

        /// <summary>
        /// Get the context state State monad
        /// </summary>
        public static readonly State<StrategyContext, StrategyContext> Context =
            get<StrategyContext>();

        /// <summary>
        /// Sets the decision directive.  Once set, cant be un-set.
        /// If the directive is Stop then the Global state is reset for this
        /// strategy (global to the Process)
        /// </summary>
        /// <param name="directive">Directive to set</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> SetDirective(Option<Directive> directive) =>
            from x in Context
            from y in put(x.With(Directive: directive))
            select y;

        /// <summary>
        /// Sets the last-failure state.  
        /// </summary>
        /// <param name="when">Time to set</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> SetLastFailure(DateTime when) =>
            from x in Context
            from y in put(x.With(Global: x.Global.With(LastFailure: when)))
            select y;

        /// <summary>
        /// Sets the current back off amount
        /// </summary>
        /// <param name="step">Step size for the next Process pause before
        /// resuming</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> SetBackOffAmount(Time step) =>
            from x in Context
            from y in put(x.With(Global: x.Global.With(BackoffAmount: step), Pause: step))
            select y;

        /// <summary>
        /// Pauses the Process for a fixed amount of time
        /// </summary>
        /// <param name="duration">Duration of the pause before
        /// resuming</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Pause(Time duration) =>
            from x in Context
            from y in put(x.With(Pause: duration))
            select y;

        /// <summary>
        /// Resets the global (to the Process) state.  This wipes out things like
        /// the current retries counter, the time since the last failure, etc.
        /// </summary>
        public static State<StrategyContext, Unit> Reset =>
            from x in Context
            from y in put(x.With(Global: StrategyState.Empty))
            select y;

        /// <summary>
        /// Maps the global (to the Process) state.
        /// </summary>
        public static State<StrategyContext, Unit> MapGlobal(Func<StrategyState, StrategyState> map) =>
            from x in Context
            from y in put(x.With(Global: map(x.Global)))
            select y;

        /// <summary>
        /// Identity function for the strategy state monad.  Use when you
        /// want a no-op
        /// </summary>
        public static State<StrategyContext, Unit> Identity => state =>
            StateResult.Return(state, unit);

        /// <summary>
        /// Gives the strategy a behaviour that will only fail N times before
        /// forcing the Process to stop
        /// </summary>
        /// <param name="Count">Number of times to retry</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Retries(int Count) =>
            from x in Context
            from y in x.Global.Failures >= Count
                ? SetDirective(Directive.Stop)
                : Identity
            select y;

        /// <summary>
        /// Gives the strategy a behaviour that will only fail N times before
        /// forcing the Process to stop.  However if a time-peroid of Duration
        /// elapses, then the number of failures 'so far' is reset to zero.
        /// 
        /// This behaviour allows something that's rapidly failing to shutdown,
        /// but will allow the occasional failure.
        /// </summary>
        /// <param name="Count">Number of times to retry</param>
        /// <param name="Duration">Time between failures</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Retries(int Count, Time Duration) =>
            from x in Context
            let now = DateTime.UtcNow
            let expired = (Time)(now - x.Global.LastFailure) > Duration
            let failures = x.Global.Failures
            from y in expired
                        ? Reset
                        : failures >= Count 
                            ? SetDirective(Directive.Stop)
                            : Identity
            select y;

        /// <summary>
        /// Applies a strategy that causes the Process to 'back off'.  That is it will
        /// be paused for an amount of time before it can continue doing other operations.
        /// </summary>
        /// <param name="Min">Minimum back-off time</param>
        /// <param name="Max">Maximum back-off time; once this point is reached the Process 
        /// will stop for good</param>
        /// <param name="Step">The amount to add to the current back-off time for each failure.
        /// That allows for the steps to grow gradually larger as the Process keeps failing</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Backoff(Time Min, Time Max, Time Step) =>
            from x in Context
            let current = x.Global.Failures < 2
                ? Min
                : (x.Global.BackoffAmount + Step).Max(Min)
            from y in current > Max
                ? SetDirective(Directive.Stop)
                : SetBackOffAmount(current)
            select y;

        /// <summary>
        /// Increase the failure count state
        /// </summary>
        public static readonly State<StrategyContext, Unit> IncFailureCount =
            MapGlobal(g => g.With(Failures: g.Failures + 1));

        /// <summary>
        /// Reset the failure count state to zero and set LastFailure to max-value
        /// </summary>
        public static readonly State<StrategyContext, Unit> ResetFailureCount =
            MapGlobal(g => g.With(Failures: 0, LastFailure: DateTime.MaxValue));

        /// <summary>
        /// Set the failure count to 1 and set LastFailure to UtcNow
        /// </summary>
        public static readonly State<StrategyContext, Unit> FailedOnce =
            MapGlobal(g => g.With(Failures: 1, LastFailure: DateTime.UtcNow));

        /// <summary>
        /// Always return this Directive in the final StrategyDecision
        /// </summary>
        /// <param name="directive">Directive to return</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Always(Directive directive) =>
            Match(
                With<ProcessKillException>(Directive.Stop),
                With<ProcessSetupException>(Directive.Stop),
                Otherwise(directive)
            );

        /// <summary>
        /// Match a range of State computations that take the Exception that caused
        /// the failure and map it to an Optional Directive.  The first computation to 
        /// return a Some(Directive) will succeed
        /// </summary>
        /// <param name="directives">Directive maps</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Match(params State<Exception, Option<Directive>>[] directives) =>
            state => StateResult.Return(
                state.With(
                    Directive: choose(ProcessSetting.StandardDirectives
                                                    .Concat(directives)
                                                    .ToArray())(state.Exception).Value), unit);

        /// <summary>
        /// Match a range of State computations that take the currently selected 
        /// Directive and map it to an Optional MessageDirective.  The first computation 
        /// to return a Some(MessageDirective) will succeed.  If a Directive hasn't been
        /// chosen by the time this is invoked then RestartNow is used by default.
        /// </summary>
        /// <param name="directives">Directive maps</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<StrategyContext, Unit> Redirect(params State<Directive, Option<MessageDirective>>[] directives) =>
            state => StateResult.Return(
                state.With(MessageDirective: choose(directives)(state.Directive.IfNone(Directive.Restart)).Value), 
                unit
            );

        /// <summary>
        /// Used within the Strategy.Match function to match an Exception to a Directive.  
        /// Use the function's generic type to specify the type of Exception to match.
        /// </summary>
        /// <typeparam name="TException">Type of Exception to match</typeparam>
        /// <param name="map">Map from the TException to a Directive</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<Exception, Option<Directive>> With<TException>(Func<TException, Directive> map) where TException : Exception =>
            from ex in get<Exception>()
            let typeMatch = typeof(TException).IsAssignableFrom(ex.GetType())
            select typeMatch
                ? Some(map((TException)ex))
                : None;

        /// <summary>
        /// Used within the Strategy.Match function to match an Exception to a Directive.  
        /// Use the function's generic type to specify the type of Exception to match.
        /// </summary>
        /// <typeparam name="TException">Type of Exception to match</typeparam>
        /// <param name="directive">Directive to use if the Exception matches TException</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<Exception, Option<Directive>> With<TException>(Directive directive) where TException : Exception =>
            With<TException>(_ => directive);

        /// <summary>
        /// Used within the Strategy.Match function to provide a default Directive if the
        /// Exception that caused the failure doesn't match any of the previous With clauses.
        /// </summary>
        /// <param name="map">Map from the Exception to a Directive</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<Exception, Option<Directive>> Otherwise(Func<Exception, Directive> map) =>
            from ex in get<Exception>()
            select Some(map(ex));

        /// <summary>
        /// Used within the Strategy.Match function to provide a default Directive if the
        /// Exception that caused the failure doesn't match any of the previous With clauses.
        /// </summary>
        /// <param name="directive">Directive to use</param>
        /// <returns>Strategy computation as a State monad</returns>
        public static State<Exception, Option<Directive>> Otherwise(Directive directive) =>
            Otherwise(_ => directive);

        public static State<Directive, Option<MessageDirective>> When<TDirective>(Func<TDirective, MessageDirective> map) where TDirective : Directive =>
            from directive in get<Directive>()
            let typeMatch = typeof(TDirective).IsAssignableFrom(directive.GetType())
            select typeMatch
                ? Some(map((TDirective)directive))
                : None;

        public static State<Directive, Option<MessageDirective>> When<TDirective>(MessageDirective directive) where TDirective : Directive =>
            When<TDirective>(_ => directive);

        public static State<Directive, Option<MessageDirective>> Otherwise(Func<Directive, MessageDirective> map) =>
            from directive in get<Directive>()
            select Some(map(directive));

        public static State<Directive, Option<MessageDirective>> Otherwise(MessageDirective directive) =>
            Otherwise(_ => directive);
    }
}
