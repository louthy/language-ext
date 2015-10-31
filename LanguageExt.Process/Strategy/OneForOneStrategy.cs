using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class OneForOneStrategy : XForOneStrategy
    {
        public OneForOneStrategy(int maxRetries = 0, Time duration = default(Time))
            : 
            base(maxRetries, duration)
        {
        }

        public override bool ApplyToAllChildrenOfSupervisor =>
            false;
    }
}
