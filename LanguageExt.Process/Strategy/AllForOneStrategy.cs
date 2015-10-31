using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class AllForOneStrategy : XForOneStrategy
    {
        public AllForOneStrategy(int maxRetries, Time duration)
            : 
            base(maxRetries, duration)
        {
        }

        public override bool ApplyToAllChildrenOfSupervisor =>
            true;
    }
}
