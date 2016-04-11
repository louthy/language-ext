using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class CancelShutdown
    {
        public bool Cancelled
        {
            get;
            private set;
        }

        public void Cancel()
        {
            Cancelled = true;
        }
    }
}
