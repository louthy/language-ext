using System;
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

// ************************************************************************************
// 
//  This is just a dumping ground I use for debugging the library, you can ignore this
// 
// ************************************************************************************


namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            Tests.QueueCollectionFunctions();
            Tests.StackCollectionFunctions();
            Tests.WrappedListOfOptionsTest1();
            Tests.WrappedListOfOptionsTest2();
            Tests.Lists();
            return;

            ProcessLog.Subscribe(Console.WriteLine);

            Tests.KillChildTest();

            Tests.ProcessStartupError();
            Tests.LocalRegisterTest();
            Tests.AskReply();
            Tests.MassiveSpawnAndKillHierarchy();
            Tests.AskReplyError();
            Tests.RegisterTest();

            Tests.ProcessStartupError();
            Tests.RegisteredAskReply();

            Tests.ClassBasedProcess();
            Tests.AsyncOption();

            Tests.MapOptionTest();
            Tests.MassAddRemoveTest();

            Tests.SpawnProcess();
            Tests.SpawnProcess();
            Tests.SpawnProcess();
            Tests.PubSubTest();
            Tests.SpawnErrorSurviveProcess();
            Tests.SpawnErrorSurviveProcess();
            Tests.SpawnErrorSurviveProcess();
            Tests.SpawnAndKillHierarchy();
            Tests.SpawnAndKillHierarchy();
            Tests.SpawnAndKillHierarchy();
            Tests.SpawnAndKillProcess();
            Tests.SpawnAndKillProcess();
            Tests.SpawnAndKillProcess();

            Tests.MassiveSpawnAndKillHierarchy();
            Tests.MassiveSpawnAndKillHierarchy2();

            Tests.ReaderAskTest();
            Tests.LiftTest();
            Tests.BindingTest();
            Tests.WrappedOptionOptionLinqTest();
            Tests.WrappedListLinqTest();
            Tests.WrappedListTest();
            Tests.LinqTest();
            Tests.ExTest4();
            Tests.MemoTest();
            Tests.UnsafeOptionTest();
        }
    }
}
