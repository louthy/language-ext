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
using Microsoft.FSharp.Core;

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
            var someValue = FSharpOption<string>.Some("Hello");
            var noneValue = FSharpOption<string>.None;

            Console.WriteLine(someValue.Value);
            Console.WriteLine(noneValue.Value);


            Tests.PStringCasting();
            return;

            Tests.VersionTest();
            Tests.SerialiseDeserialiseCoreTypes();
            //Tests.StopStart();

            //Tests.QueueCollectionFunctions();
            //Tests.StackCollectionFunctions();
            //Tests.WrappedListOfOptionsTest1();
            //Tests.WrappedListOfOptionsTest2();
            //Tests.Lists();

            ProcessSystemLog.Subscribe(Console.WriteLine);

            Tests.KillChildTest();

            Tests.ProcessStartupError();
            //Tests.LocalRegisterTest();
            Tests.AskReply();
            Tests.MassiveSpawnAndKillHierarchy();
            Tests.AskReplyError();
            //Tests.RegisterTest();

            Tests.ProcessStartupError();
            //Tests.RegisteredAskReply();

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
            Tests.ExTest4();
            Tests.MemoTest();
            Tests.UnsafeOptionTest();
        }
    }
}
