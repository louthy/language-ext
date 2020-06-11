using System;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static System.Console;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;



namespace LanguageExt.Tests
{
    public class IssuesTests
    {
        /// <summary>
        /// https://github.com/louthy/language-ext/issues/207
        /// </summary>
        public Task<Either<Exception, int>> Issue207() =>
            Initialization
                .BindT(createUserMapping)
                .BindT(addUser);

        public Task<Either<Exception, int>> Issue207_2() =>
            from us in Initialization
            from mu in createUserMapping(us).AsTask()
            from id in addUser(mu)
            select id;

        static Task<Either<Exception, ADUser>> Initialization =>
            Right<Exception, ADUser>(ADUser.New("test user")).AsTask();

        static Either<Exception, UserMapping> createUserMapping(ADUser user) =>
            Right<Exception, UserMapping>(UserMapping.New(user.ToString() + " mapped"));

        static Task<Either<Exception, int>> addUser(UserMapping user) =>
            Right<Exception, int>(user.ToString().Length).AsTask();

        static Try<int> addUser2(UserMapping user) => () =>
            user.ToString().Length;

        static Try<UserMapping> createUserMapping2(ADUser user) => () =>
            UserMapping.New(user.ToString() + " mapped");

        [Fact]
        public TryAsync<int> Issue207_5() =>
            from us in TryAsync<ADUser>(() => throw new Exception("fail"))
            from mu in createUserMapping2(us).ToAsync()
            from id in addUser2(mu).ToAsync()
            select id;

        //https://github.com/louthy/language-ext/issues/242
        [Fact]
        public void Issue208()
        {
            var r = from a in Left<Exception, int>(new Exception("error 1")).AsTask()
                    from b in Right<Exception, int>(1).AsTask()
                    select a + b;
        }

        [Fact]
        public void Issue346()
        {
            var list = 1.Cons().ToList();
        }

        static void EqPar()
        {
            var eq = par<string, string, bool>(equals<EqStringOrdinalIgnoreCase, string>, "abc");
        }

        static Writer<MSeq<string>, Seq<string>, Seq<int>> multWithLog(Seq<int> input) =>
            from _ in Writer(0, Seq1("Start"))
            let c = input.Map(i => Writer(i * 10, Seq1($"Number: {i}")))
            from r in c.Sequence()
            select r;

        [Fact]
        public void Issue675()
        {
            var l1 = LanguageExt.Prelude.List<int>(1, 2, 3);
            var l2 = LanguageExt.Prelude.List<int>(4, 5, 6);

            var a = l1.AddRange(l2); // Count 6, [1,2,3,4,5,6]
            var b = l1.AddRange(l2); // Count 5, [1,2,4,5,6]
            var c = l1.AddRange(l2); // Count 8, [1,2,4,5,6,4,5,6]
            var d = l1.AddRange(l2); // Count 7, [1,2,4,5,4,5,6]
            var e = l1.AddRange(l2); // Count 6, [1,2,4,4,5,6]

            Assert.True(a == b);
            Assert.True(a == c);
            Assert.True(a == d);
            Assert.True(a == e);
        }
    }

    public class ADUser : NewType<ADUser, string> { public ADUser(string u) : base(u) { } }
    public class UserMapping : NewType<UserMapping, string> { public UserMapping(string u) : base(u) { } }
}

// https://github.com/louthy/language-ext/issues/245
public class TopHatTests
{
    public class TopHat : Record<TopHat>
    {
        public TopHat(int id, Option<int> id2)
        {
            Id = id;
            Id2 = id2;
        }

        TopHat(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public int Id { get; set; }
        public Option<int> Id2 { get; set; }
    }

    OptionAsync<int> SumOptionAsync() => SomeAsync(async _ =>
    {
        var first = await Task.FromResult(1);
        var second = await Task.FromResult(2);

        return first + second;
    });

    [Fact]
    public void TopHatSerialisationTest()
    {
        var t1 = new TopHat(1, 1416);
        var t3 = new TopHat(1, 1413);

        var str = JsonConvert.SerializeObject(t1);

        var t2 = JsonConvert.DeserializeObject<TopHat>(str);

        Assert.True(t2 == t1);
        Assert.True(t3 != t1);
    }
}

//https://github.com/louthy/language-ext/issues/242
namespace Core.Tests
{
    using static ExternalSystem;

    public class ExternalOptionsAndEithersTests
    {
        [Fact]
        public async Task what_i_desire_EitherAsync()
        {
            EitherAsync<Error, Pixel> GetPixelE(PixelId id) =>
                GetPixel(id).ToEither(Error.New("pixel not found"));

            var program =
                from pixel in GetPixelE(PixelId.New("wkrp"))
                from id in GenerateLinkId(pixel.Value)
                from resource in ScrapeUrl("http://google.com")
                select resource;

            await program.Match(
                Right: r => Assert.True(false, "this should not pass"),
                Left: e => Assert.Equal("pixel not found", e.Value)
            );
        }
    }

    static class ExternalSystem
    {
        public class Error : NewType<Error, string>
        {
            public Error(string value) : base(value) { }
        }

        public static OptionAsync<Pixel> GetPixel(PixelId id) =>
            Option<Pixel>.None.ToAsync();

        public static EitherAsync<Error, string> GenerateLinkId(PixelId pixelId) =>
            Right<Error, string>($"{pixelId}-1234").ToAsync();

        public static EitherAsync<Error, WebResource> ScrapeUrl(string url) =>
            Right<Error, WebResource>(new WebResource(200)).ToAsync();

        public class WebResource : NewType<WebResource, int>
        {
            public WebResource(int value) : base(value) { }
        }

        public class PixelId : NewType<PixelId, string>
        {
            public PixelId(string value) : base(value) { }
        }

        public class Pixel : NewType<Pixel, PixelId>
        {
            public Pixel(PixelId value) : base(value) { }
        }
    }

}
namespace Issues
{
    public class CollectorId : NewType<CollectorId, int> { public CollectorId(int value) : base(value) { } };
    public class TenantId : NewType<TenantId, int> { public TenantId(int value) : base(value) { } };
    public class UserId : NewType<UserId, int> { public UserId(int value) : base(value) { } };
    public class Instant : NewType<Instant, int> { public Instant(int value) : base(value) { } };

    public class Collector : Record<Collector>, ISerializable
    {
        public CollectorId Id { get; }
        public string Name { get; }
        public TenantId CurrentTenant { get; }
        public UserId AssignedBy { get; }
        public Instant InstantAssigned { get; }
        public Collector(Some<CollectorId> id, Some<string> name, Some<TenantId> tenant, Some<UserId> assignedBy, Instant dateAssigned)
        {
            Id = id;
            Name = name;
            CurrentTenant = tenant;
            AssignedBy = assignedBy;
            InstantAssigned = dateAssigned;
        }

        Collector(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
    }

    public class GitterTests
    {
        [Fact]
        public void TestSerial()
        {
            var x = new Collector(CollectorId.New(1), "nick", TenantId.New(2), UserId.New(3), Instant.New(4));
            var y = new Collector(CollectorId.New(1), "nick", TenantId.New(2), UserId.New(3), Instant.New(4));

            var z1 = x == y;
            var z2 = x.Equals(y);
            var z3 = x.Equals((object)y);

            var r = JsonConvert.SerializeObject(x);
            var r2 = JsonConvert.DeserializeObject<Collector>(r);
        }
    }

    public static class Issue251
    {
        public class Error : NewType<Error, string>
        {
            public Error(string value) : base(value)
            {
            }
        }

        public class ErrorException : Exception
        {
            public readonly Error Error;
            public ErrorException(Error error) : base(error.Value)
            {
                Error = error;
            }
        }

        public static TryOptionAsync<A> AsTryOptionAsync<A>(this Either<Error, Option<A>> ma) =>
            ma.Match(
                Right: r => TryOptionAsync(r),
                Left: e => TryOptionAsync<A>(new ErrorException(e)));

        public static TryOption<A> AsTryOption<A>(this Either<Error, Option<A>> ma) =>
            ma.Match(
                Right: r => TryOption(r),
                Left: e => TryOption<A>(new ErrorException(e)));

        public static TryOptionAsync<A> AsTryOptionAsync<A>(this Task<Either<Error, Option<A>>> ma) =>
            ma.Map(either => either.AsTryOption()).ToAsync();

        public static Error AsError(this Exception ex) =>
            ex is ErrorException err
                ? err.Error
                : Error.New(ex.Message);

        [Fact]
        public static async void MatchTest()
        {
            var m1 = Right<Error, Option<string>>("Testing").AsTask();
            var m2 = Left<Error, Option<string>>(Error.New("Testing")).AsTask();
            var m3 = Right<Error, Option<string>>(None).AsTask();

            await m1.AsTryOptionAsync()
                    .Match(
                         Some: va => Console.WriteLine(va),
                         None: () => Console.WriteLine("none"),
                         Fail: ex => Console.WriteLine(ex.AsError()));

            var t1 = Right<Error, Option<string>>("Testing").AsTask().AsTryOptionAsync();
            var t2 = Left<Error, Option<string>>(Error.New("Testing")).AsTask().AsTryOptionAsync();
            var t3 = Right<Error, Option<string>>(None).AsTask().AsTryOptionAsync();

            var list = List(t1, t2, t3);

            var resu = list.Sequence();

        }
    }

    public class Issue242
    {

        [Fact]
        public async Task Issue242_ExpectNoException()
        {
            var failableTask = fun((Either<string, int> value) =>
                value.AsTask());

            var result = await from a in failableTask("This will NOT cause a Bottom Exception")
                               from b in failableTask(3)
                               select a + b;
        }

    }

    public class Issue263
    {
        public readonly Func<long, Unit> fire = i =>
        {
            return unit;
        };

        public void Test()
        {
            act(fire);
        }
    }

    public class Issue261
    {
        [Fact]
        public void Test1()
        {
            var ma = Writer<MSeq<string>, Seq<string>, int>(100);
            var mb = Writer<MSeq<string>, Seq<string>, int>(200);

            var mc = from x in ma
                     from y in mb
                     from _1 in tell<MSeq<string>, Seq<string>>(Seq1("Hello"))
                     from _2 in tell<MSeq<string>, Seq<string>>(Seq1("World"))
                     from _3 in tell<MSeq<string>, Seq<string>>(Seq1($"the result is {x + y}"))
                     select x + y;

            var r = mc();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == Seq("Hello", "World", "the result is 300"));
        }

        [Fact]
        public void Test2()
        {
            var ma = Writer<string, int>(100);
            var mb = Writer<string, int>(200);

            var mc = from x in ma
                     from y in mb
                     from _1 in tell("Hello")
                     from _2 in tell("World")
                     from _3 in tell($"the result is {x + y}")
                     select x + y;

            var r = mc();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == Seq("Hello", "World", "the result is 300"));
        }

        [Fact]
        public void Test3()
        {
            var ma = (100, Seq<string>());
            var mb = (200, Seq<string>());

            var mc = from x in ma.ToWriter()
                     from y in mb.ToWriter()
                     from _1 in tell("Hello")
                     from _2 in tell("World")
                     from _3 in tell($"the result is {x + y}")
                     select x + y;

            var r = mc();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == Seq("Hello", "World", "the result is 300"));
        }
    }

    public class Issue376
    {
        static Task<int> Number(int n) => n.AsTask();
        static Task<string> Error(string err) => err.AsTask();

        public static EitherAsync<string, int> Op1() =>
            Number(1);

        public static EitherAsync<string, int> Op2() =>
            RightAsync<string, int>(2.AsTask());

        public static EitherAsync<string, int> Op3() =>
            Error("error");

        public static EitherAsync<string, int> Calculate(int x, int y, int z) =>
            (x + y + z);

        public static async Task Test()
        {
            var res = await (from x in Op1()
                             from y in Op2()
                             from z in Op3()
                             from w in Calculate(x, y, z)
                             select w)
                            .IfLeft(0);
        }
    }

    public class Issue376_2
    {
        public static async Task<Either<string, int>> Op1()
        {
            return await 1.AsTask();
        }

        public static async Task<Either<string, int>> Op2()
        {
            return await 2.AsTask();
        }

        public static async Task<Either<string, int>> Op3()
        {
            return await "error".AsTask();
        }

        public static async Task<Either<string, int>> Calculate(int x, int y, int z)
        {
            return await Task.FromResult(x + y + z);
        }

        public static async Task Test()
        {
            var res = await (from x in Op1()
                             from y in Op2()
                             from z in Op3()
                             from w in Calculate(x, y, z)
                             select w);
        }
    }

    public static class TestExt
    {
        public static Task<Either<L, C>> SelectMany<L, A, B, C>(
            this Task<Either<L, A>> ma,
            Func<A, Task<Either<L, B>>> bind,
            Func<A, B, C> project) =>
            ma.BindT(a =>
                bind(a).BindT(b =>
                    default(MEither<L, C>).Return(project(a, b))));
    }

    public class Issue376_3
    {
        public static async Task<Option<int>> Op1()
        {
            return await 1.AsTask();
        }

        public static async Task<Option<int>> Op2()
        {
            return await 2.AsTask();
        }

        public static async Task<Option<int>> Op3()
        {
            return await Option<int>.None.AsTask();
        }

        public static async Task<Option<int>> Calculate(int x, int y, int z)
        {
            return await Task.FromResult(x + y + z);
        }

        //public static async Task Test()
        //{
        //    var res = await (from x in Op1()
        //                     from y in Op2()
        //                     from z in Op3()
        //                     from w in Calculate(x, y, z)
        //                     select w)
        //                    .IfLeft(0);
        //}
    }

    public class Issue533
    {
        [Fact]
        public void Test()
        {

            var someData = Enumerable
                .Range(0, 30000)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var result = someData
                .Select(Some)
                .Sequence()
                .Map(x => x.ToArray());
        }

    }

    // https://stackoverflow.com/questions/54609459/languageext-eitherasyn-with-aggegrate-bind-with-validation
    public class StackOverflow_54609459
    {
        public class Error { }
        public class HostResponse { }
        public class Response { }

        public class Command {

            public readonly string Name;

            static Either<Error, Func<string, EitherAsync<Error, R>>> GetCommand<R>(
                Map<string, Func<string, EitherAsync<Error, R>>> commandMap, 
                Command hostCommand) =>
                     commandMap.Find(hostCommand.Name)
                               .ToEither(new Error());

            internal static EitherAsync<Error, R> ExecuteCommand<R>(
                Func<string, EitherAsync<Error, R>> command,
                Command cmd) =>
                    command(cmd.Name);

            static Either<Error, Unit> Validate<R>(
                Map<string, Func<string, EitherAsync<Error, R>>> commandMap, 
                Command hostCommand) =>
                    commandMap.Find(hostCommand.Name)
                              .Map(_ => unit)
                              .ToEither(new Error());

            public static EitherAsync<Error, Seq<R>> ExecuteAllAsync<R>(
                Map<string, Func<string, EitherAsync<Error, R>>> commandMap,
                Seq<Command> hostCommands) =>
                    hostCommands.Map(cmd =>
                        from _ in Command.Validate(commandMap, cmd).ToAsync()
                        from f in Command.GetCommand<R>(commandMap, cmd).ToAsync()
                        from r in Command.ExecuteCommand(f, cmd)
                        select r)
                       .SequenceParallel();
        }
    }
}
