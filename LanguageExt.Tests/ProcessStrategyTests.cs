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
            var pid = spawn<Unit>("test", _ => { }, ProcessFlags.Default, oneForOneStrategy);
        }

        [Fact]
        public void AllForOneTest()
        {
            var pid = spawn<Unit>("test", _ => { }, ProcessFlags.Default, allForOneStrategy);
        }

        ProcessStrategy oneForOneStrategy =
            AllForOne(MaxRetries: 5, Duration: 10 * s)
                .Match(
                    exception<NotSupportedException>(Directive.Stop),
                    exception<NullReferenceException>(Directive.Stop),
                    exception<ApplicationException>(Directive.RestartNow),
                    otherwise(_ => Directive.Resume));

        ProcessStrategy allForOneStrategy = 
            AllForOne(MaxRetries: 5, Duration: 10 * s)
                .Match(
                    exception<NotSupportedException>(Directive.Stop),
                    exception<NullReferenceException>(Directive.Stop),
                    exception<ApplicationException>(Directive.RestartNow),
                    otherwise(_ => Directive.Resume));
    }
}
