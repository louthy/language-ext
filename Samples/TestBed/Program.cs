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
        public class JenkinsBuild
        {
            public int number;
        }

        public class JenkinsJob
        {
            public JenkinsBuild lastBuild;
        }

        public static Uri GetJobURIStem() => new Uri("http://www.google.com");

        public static Task<Either<Exception, JenkinsJob>> FetchJSON(Uri url) =>
            Task.FromResult(Right<Exception, JenkinsJob>(new JenkinsJob { lastBuild = new JenkinsBuild { number = 1 } }));


        //var r = (await jk)
        //    .Right(j => j.lastBuild == null ? Left<Exception, JenkinsJob>(new ArgumentException("No build completed")) : Right<Exception, JenkinsJob>(j))
        //    .Left(e => Left<Exception, JenkinsJob>(e))
        //    .Map(j => j.lastBuild.number);

        public static async Task<Either<Exception, JenkinsBuildId>> GetCurrentJobNumber() =>
            (await FetchJSON(GetJobURIStem()))
                .Bind(EnsureCompletedBuild);

        public static Either<Exception, JenkinsBuildId> EnsureCompletedBuild(this JenkinsJob self) =>
            self.lastBuild == null
                ? AppError("No build completed")
                : Success(new JenkinsBuildId(self.lastBuild.number));

        public static Either<Exception, JenkinsBuildId> Success(JenkinsBuildId value) =>
            Right<Exception, JenkinsBuildId>(value);

        public static Either<Exception, JenkinsBuildId> AppError(string msg) =>
            Left<Exception, JenkinsBuildId>(new ApplicationException(msg));

        public class JenkinsBuildId : NewType<int> { public JenkinsBuildId(int id) : base(id) { } }

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
            Tests.LinqTest();
            Tests.ExTest4();
            Tests.MemoTest();
            Tests.UnsafeOptionTest();
        }
    }
}
