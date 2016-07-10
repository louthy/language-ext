using System;
using System.Collections.Generic;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Keeps a running context whilst a strategy computation is running
    /// </summary>
    public class StrategyContext
    {
        public readonly StrategyState Global;
        public readonly Exception Exception;
        public readonly object Message;
        public readonly ProcessId Sender;
        public readonly ProcessId Self;
        public readonly ProcessId ParentProcess;
        public readonly IEnumerable<ProcessId> Siblings = new ProcessId[0];
        public readonly IEnumerable<ProcessId> Affects = new ProcessId[0];
        public readonly Option<Directive> Directive;
        public readonly Option<MessageDirective> MessageDirective;
        public readonly Time Pause;

        /// <summary>
        /// Default strategy context
        /// </summary>
        public static readonly StrategyContext Empty =
            new StrategyContext(
                StrategyState.Empty,
                null,
                null,
                ProcessId.None,
                ProcessId.None,
                ProcessId.None,
                null,
                null,
                0 * seconds,
                None,
                None);

        StrategyContext(
            StrategyState global,
            Exception exception,
            object message,
            ProcessId sender,
            ProcessId failedProcess,
            ProcessId parentProcess,
            IEnumerable<ProcessId> siblings,
            IEnumerable<ProcessId> affects,
            Time pause,
            Option<Directive> directive,
            Option<MessageDirective> messageDirective
            )
        {
            bool isStop = directive == LanguageExt.Directive.Stop;

            Global = isStop ? StrategyState.Empty : global;
            Exception = exception;
            Message = message;
            Sender = sender;
            Self = failedProcess;
            ParentProcess = parentProcess;
            Siblings = siblings ?? Siblings;
            Affects = affects ?? Affects;
            Pause = isStop ? 0 * s : pause;
            Directive = directive;
            MessageDirective = messageDirective;
        }

        public StrategyContext With(
            StrategyState Global = null,
            Exception Exception = null,
            object Message = null,
            ProcessId? Sender = null,
            ProcessId? FailedProcess = null,
            ProcessId? ParentProcess = null,
            IEnumerable<ProcessId> Siblings = null,
            IEnumerable<ProcessId> Affects = null,
            Time? Pause = null,
            Option<Directive> Directive = default(Option<Directive>),
            Option<MessageDirective> MessageDirective = default(Option<MessageDirective>)
        ) =>
            new StrategyContext(
                Global ?? this.Global,
                Exception ?? this.Exception,
                Message ?? this.Message,
                Sender ?? this.Sender,
                FailedProcess ?? this.Self,
                ParentProcess ?? this.ParentProcess,
                Siblings ?? this.Siblings,
                Affects ?? this.Affects,
                Pause ?? this.Pause,

                // The following may look like bugs, but it's intentional that when
                // a Directive or MessageDirective is set, they stay set to their first
                // concrete value.  That means the State strategy expression that runs 
                // has the equivalent of an 'early out'.  The whole expression is still
                // processed, but you can't override the earlier value.
                this.Directive.IsSome()
                    ? this.Directive
                    : Directive,
                this.MessageDirective.IsSome()
                    ? this.MessageDirective
                    : MessageDirective
            );
    }
}
