using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LanguageExt;
using LanguageExt.Trans;
using LanguageExt.Trans.Linq;
using System.Reactive.Linq;
using static LanguageExt.List;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Diagnostics;
using System.Threading.Tasks;
//using static TestBed.Process2;


namespace TestBed
{
    //public static class CacheTest
    //{
    //    public static Map<string, object> StringInbox(Map<string, object> state, string msg)
    //    {
    //        return state.Add(msg.GetType().ToString(), msg);
    //    }

    //    public static Map<string, object> IntInbox(Map<string, object> state, int msg)
    //    {
    //        return state.Add(msg.GetType().ToString(), msg);
    //    }

    //    public static void Startup()
    //    {
    //        spawn("test", () => Map<string, object>(),
    //            many(
    //                inbox<Map<string, object>,string>(StringInbox),
    //                inbox<Map<string, object>,int>(IntInbox)
    //            ));
    //    }
    //}
}
