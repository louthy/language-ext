using System;
using System.Collections.Generic;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class StrategyDecision
    {
        public readonly Directive ProcessDirective;
        public readonly MessageDirective MessageDirective;
        public readonly IEnumerable<ProcessId> Affects;
        public readonly Time Pause;

        public StrategyDecision(
            Directive processDirective,
            MessageDirective messageDirective,
            IEnumerable<ProcessId> affects,
            Time pause
        )
        {
            ProcessDirective = processDirective;
            MessageDirective = messageDirective;
            Affects = affects;
            Pause = pause;
        }

        public static StrategyDecision New(
            Directive processDirective,
            MessageDirective messageDirective,
            IEnumerable<ProcessId> affects,
            Time pause
        ) =>
            new StrategyDecision(processDirective, messageDirective, affects, pause);

    }
}
