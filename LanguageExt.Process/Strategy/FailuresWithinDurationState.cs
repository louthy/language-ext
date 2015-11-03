using System;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class FailuresWithinDurationState
    {
        public readonly int Failures;
        public readonly DateTime LastFailure;

        public readonly static FailuresWithinDurationState Empty =
            new FailuresWithinDurationState(0, DateTime.Now);

        internal FailuresWithinDurationState(int failures, DateTime lastFailure)
        {
            Failures = failures;
            LastFailure = lastFailure;
        }

        FailuresWithinDurationState FailedOnce() =>
            new FailuresWithinDurationState(1, DateTime.Now);

        FailuresWithinDurationState FailedAgain() =>
            new FailuresWithinDurationState(Failures + 1, DateTime.Now);

        public FailuresWithinDurationState CheckExpired(Time duration) =>
            (DateTime.Now - LastFailure).TotalSeconds * seconds > duration
                ? FailedOnce()
                : FailedAgain();

        public Tuple<FailuresWithinDurationState, Option<Directive>> CheckRetriesNotExceded(int maxRetries) =>
            maxRetries > -1 && Failures > maxRetries
                ? Tuple<FailuresWithinDurationState, Option<Directive>>(Empty, Some(Directive.Stop))
                : Tuple<FailuresWithinDurationState, Option<Directive>>(this, None);
    }
}
