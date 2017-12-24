using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    public interface AsyncPair<SyncA, AsyncA>
    {
        AsyncA ToAsync(SyncA sa);
    }
}
