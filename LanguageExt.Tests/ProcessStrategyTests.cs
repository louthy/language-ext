using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExtTests
{
    public class ProcessStrategyTests
    {
        [Fact]
        public void OneForOneTest()
        {
            var state = oneForOneStrategy.NewState(ProcessId.NoSender);

            var res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new NotSupportedException());
            Assert.True(res.Item2 == Directive.Escalate);
            state = res.Item1;

            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new NullReferenceException());
            Assert.True(res.Item2 == Directive.Restart(10*s));
            state = res.Item1;

            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ApplicationException());
            Assert.True(res.Item2 == Directive.RestartNow);
            state = res.Item1;

            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ProcessKillException());
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ProcessSetupException(null, null));
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            // This should return Directive.Stop because there are 5 failures within 10 seconds
            // after this is asserted true the failure count will be reduced to 0
            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new Exception());
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = oneForOneStrategy.HandleFailure(ProcessId.NoSender, state, new Exception());
            Assert.True(res.Item2 == Directive.Resume);
            state = res.Item1;
        }

        [Fact]
        public void AllForOneTest()
        {
            var state = allForOneStrategy.NewState(ProcessId.NoSender);

            var res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new NotSupportedException());
            Assert.True(res.Item2 == Directive.Escalate);
            state = res.Item1;

            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new NullReferenceException());
            Assert.True(res.Item2 == Directive.Restart(10 * s));
            state = res.Item1;

            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ApplicationException());
            Assert.True(res.Item2 == Directive.RestartNow);
            state = res.Item1;

            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ProcessKillException());
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new ProcessSetupException(null, null));
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            // This should return Directive.Stop because there are 5 failures within 10 seconds
            // after this is asserted true the failure count will be reduced to 0
            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new Exception());
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = allForOneStrategy.HandleFailure(ProcessId.NoSender, state, new Exception());
            Assert.True(res.Item2 == Directive.Resume);
            state = res.Item1;

        }

        [Fact]
        public void OneForOneAlwaysTest()
        {
            var always = OneForOne().Always(Directive.Resume);
            var state = always.NewState(ProcessId.NoSender);

            var res = always.HandleFailure(ProcessId.NoSender, state, new ProcessKillException());
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = always.HandleFailure(ProcessId.NoSender, state, new ProcessSetupException(null, null));
            Assert.True(res.Item2 == Directive.Stop);
            state = res.Item1;

            res = always.HandleFailure(ProcessId.NoSender, state, new Exception());
            Assert.True(res.Item2 == Directive.Resume);
            state = res.Item1;
        }

        IProcessStrategy oneForOneStrategy =
            OneForOne(MaxRetries: 5, Duration: 10 * s).Match()
                .With<NotSupportedException>(Directive.Escalate)
                .With<NullReferenceException>(Directive.Restart(10*s))
                .With<ApplicationException>(Directive.RestartNow)
                .Otherwise(Directive.Resume);

        IProcessStrategy allForOneStrategy = 
            AllForOne(MaxRetries: 5, Duration: 10 * s).Match()
                .With<NotSupportedException>(Directive.Escalate)
                .With<NullReferenceException>(Directive.Restart(10*s))
                .With<ApplicationException>(Directive.RestartNow)
                .Otherwise(Directive.Resume);
    }
}
