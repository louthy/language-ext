using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using static LanguageExt.Strategy;

namespace LanguageExtTests
{
    public class ProcessStrategyTests
    {
        State<StrategyContext, Unit> retriesAndBackOff =
            OneForOne(
                Retries(Count: 5, Duration: 30*seconds),
                Backoff(Min: 2*seconds, Max: 1*hour, Step: 5*seconds),
                Match(
                    With<NotImplementedException>(Directive.Stop),
                    With<ArgumentNullException>(Directive.Escalate),
                    Otherwise(Directive.Restart)),
                Redirect(
                    When<Restart>(MessageDirective.ForwardToParent),
                    When<Escalate>(MessageDirective.ForwardToSelf),
                    Otherwise(MessageDirective.ForwardToDeadLetters)));

        State<StrategyContext, Unit> backOff =
            AllForOne(
                Backoff(Min: 1*seconds, Max: 10*seconds, Step: 5*seconds),
                Always(Directive.Restart),
                Redirect(
                    When<Restart>(MessageDirective.ForwardToParent),
                    Otherwise(MessageDirective.ForwardToDeadLetters)));


        [Fact]
        public void TooManyRetriesInSpecifiedDurationTest()
        {
            // This exception causes a Stop (from its Match -> With), that means that even
            // though there's a retries counter, it will be reset, because the Process will
            // be no more if the decision is enacted.
            var res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotImplementedException(), null)(StrategyState.Empty);
            Assert.True(res.Value.ProcessDirective == Directive.Stop);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToDeadLetters);
            Assert.True(res.Value.Pause == 0*seconds);

            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new ArgumentNullException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Escalate);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToSelf);
            Assert.True(res.Value.Pause == 2*seconds);

            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotSupportedException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 7*seconds);

            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotSupportedException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 12*seconds);

            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotSupportedException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 17*seconds);

            // This is the 5th event in a row, that causes the the decision to be 'Stop' and 
            // resets all 'retry counters' and 'last failure' date-times.
            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotSupportedException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Stop);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToDeadLetters);
            Assert.True(res.Value.Pause == 0*seconds);

            res = retriesAndBackOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new NotSupportedException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 2 * seconds);
        }

        [Fact]
        public void BackOffGrowsTooLargeTest()
        {
            var res = backOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new ArgumentNullException(), null)(StrategyState.Empty);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 1 * seconds);

            res = backOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new ArgumentNullException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Restart);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToParent);
            Assert.True(res.Value.Pause == 6 * seconds);

            // This is the event where the backoff-max boundary is hit; that forces the
            // decision to be 'Stop' and resets all pauses.
            res = backOff.Failure(ProcessId.None, ProcessId.None, ProcessId.None, null, new ArgumentNullException(), null)(res.State);
            Assert.True(res.Value.ProcessDirective == Directive.Stop);
            Assert.True(res.Value.MessageDirective == MessageDirective.ForwardToDeadLetters);
            Assert.True(res.Value.Pause == 0 * seconds);

        }

    }
}
