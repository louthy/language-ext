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
            Assert.True(oneForOneStrategy.HandleFailure(new NotSupportedException(), Process.Root) == Directive.Escalate);
            Assert.True(oneForOneStrategy.HandleFailure(new NullReferenceException(), Process.Root) == Directive.Restart(10*s));
            Assert.True(oneForOneStrategy.HandleFailure(new ApplicationException(), Process.Root) == Directive.RestartNow);
            Assert.True(oneForOneStrategy.HandleFailure(new ProcessKillException(), Process.Root) == Directive.Stop);
            Assert.True(oneForOneStrategy.HandleFailure(new ProcessSetupException(null,null), Process.Root) == Directive.Stop);

            // This should return Directive.Stop because there are 5 failures within 10 seconds
            // after this is asserted true the failure count will be reduced to 0
            Assert.True(oneForOneStrategy.HandleFailure(new Exception(), Process.Root) == Directive.Stop);
            Assert.True(oneForOneStrategy.HandleFailure(new Exception(), Process.Root) == Directive.Resume);
        }

        [Fact]
        public void AllForOneTest()
        {
            Assert.True(allForOneStrategy.HandleFailure(new NotSupportedException(), Process.Root) == Directive.Escalate);
            Assert.True(allForOneStrategy.HandleFailure(new NullReferenceException(), Process.Root) == Directive.Restart(10*s));
            Assert.True(allForOneStrategy.HandleFailure(new ApplicationException(), Process.Root) == Directive.RestartNow);
            Assert.True(allForOneStrategy.HandleFailure(new ProcessKillException(), Process.Root) == Directive.Stop);
            Assert.True(allForOneStrategy.HandleFailure(new ProcessSetupException(null, null), Process.Root) == Directive.Stop);

            // This should return Directive.Stop because there are 5 failures within 10 seconds
            // after this is asserted true the failure count will be reduced to 0
            Assert.True(allForOneStrategy.HandleFailure(new Exception(), Process.Root) == Directive.Stop);
            Assert.True(allForOneStrategy.HandleFailure(new Exception(), Process.Root) == Directive.Resume);
        }

        [Fact]
        public void OneForOneAlwaysTest()
        {
            var always = OneForOne().Always(Directive.Resume);

            Assert.True(always.HandleFailure(new ProcessKillException(), Process.Root) == Directive.Stop);
            Assert.True(always.HandleFailure(new ProcessSetupException(null, null), Process.Root) == Directive.Stop);
            Assert.True(always.HandleFailure(new Exception(), Process.Root) == Directive.Resume);
        }

        ProcessStrategy oneForOneStrategy =
            OneForOne(MaxRetries: 5, Duration: 10 * s).Match()
                .With<NotSupportedException>(Directive.Escalate)
                .With<NullReferenceException>(Directive.Restart(10*s))
                .With<ApplicationException>(Directive.RestartNow)
                .Otherwise(Directive.Resume);

        ProcessStrategy allForOneStrategy = 
            AllForOne(MaxRetries: 5, Duration: 10 * s).Match()
                .With<NotSupportedException>(Directive.Escalate)
                .With<NullReferenceException>(Directive.Restart(10*s))
                .With<ApplicationException>(Directive.RestartNow)
                .Otherwise(Directive.Resume);
    }
}
