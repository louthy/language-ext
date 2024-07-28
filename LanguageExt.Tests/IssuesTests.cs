using Xunit;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class IssuesTests
    {
        /// <summary>
        /// https://github.com/louthy/language-ext/issues/207
        /// </summary>
        public void Issue207() =>
            Initialization
                .Bind(createUserMapping)
                .Bind(addUser)
                .Run();

        public EitherT<Exception, IO, int> Issue207_2() =>
            from us in Initialization
            from mu in createUserMapping(us)
            from id in addUser(mu)
            select id;

        static EitherT<Exception, IO, ADUser> Initialization =>
            EitherT.Right<Exception, IO, ADUser>(ADUser.New("test user"));

        static Either<Exception, UserMapping> createUserMapping(ADUser user) =>
            Right<Exception, UserMapping>(UserMapping.New(user + " mapped"));

        static EitherT<Exception, IO, int> addUser(UserMapping user) =>
            EitherT.Right<Exception, IO, int>(user.ToString().Length);

        static IO<int> addUser2(UserMapping user) => 
            IO.lift(() => user.ToString().Length);

        static IO<UserMapping> createUserMapping2(ADUser user) => 
            IO.lift(() => UserMapping.New(user + " mapped"));

        public IO<int> Issue207_5() =>
            from us in IO.lift<ADUser>(() => throw new Exception("fail"))
            from mu in createUserMapping2(us)
            from id in addUser2(mu)
            select id;

        //https://github.com/louthy/language-ext/issues/242
        [Fact]
        public void Issue208()
        {
            var r = from a in Left<Exception, int>(new Exception("error 1"))
                    from b in Right<Exception, int>(1)
                    select a + b;
        }

        [Fact]
        public void Issue346()
        {
            var list = 1.Cons().ToList();
        }

        [Fact]
        public void Issue675()
        {
            var l1 = List(1, 2, 3);
            var l2 = List(4, 5, 6);

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

    public class ADUser(string u) : NewType<ADUser, string>(u);
    public class UserMapping(string u) : NewType<UserMapping, string>(u);
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

    OptionT<IO, int> SumOptionAsync() => liftIO(async _ =>
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
        public void what_i_desire_EitherAsync()
        {
            EitherT<Error, IO, Pixel> GetPixelE(PixelId id) =>
                GetPixel(id).ToEither(Error.New("pixel not found"));

            var program =
                from pixel in GetPixelE(PixelId.New("wkrp"))
                from id in GenerateLinkId(pixel.Value)
                from resource in ScrapeUrl("http://google.com")
                select resource;

            program.Match(
                Right: _ => Assert.Fail("this should not pass"),
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

        public static OptionT<IO, Pixel> GetPixel(PixelId id) =>
            OptionT<IO, Pixel>.None;

        public static EitherT<Error, IO, string> GenerateLinkId(PixelId pixelId) =>
            Right<Error, string>($"{pixelId}-1234");

        public static EitherT<Error, IO, WebResource> ScrapeUrl(string url) =>
            Right<Error, WebResource>(new WebResource(200));

        public class WebResource(int value) : NewType<WebResource, int>(value);
        public class PixelId(string value) : NewType<PixelId, string>(value);
        public class Pixel(PixelId value) : NewType<Pixel, PixelId>(value);
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
        public Collector(CollectorId id, string name, TenantId tenant, UserId assignedBy, Instant dateAssigned)
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
        public class Error(string value) : NewType<Error, string>(value);
        public class ErrorException(Error error) : Exception(error.Value)
        {
            public readonly Error Error = error;
        }

        public static OptionT<IO, A> AsOptionT<A>(this Either<Error, Option<A>> ma) =>
            ma.Match(
                Right: liftIO,
                Left: e => lift<A>(() => throw new ErrorException(e)));

        public static OptionT<IO, A> AsTryOption<A>(this Either<Error, Option<A>> ma) =>
            ma.Match(
                Right: liftIO,
                Left: e => lift<A>(() => throw new ErrorException(e)));

        public static OptionT<IO, A> AsOptionT<A>(this Task<Either<Error, Option<A>>> ma) =>
            ma.Map(either => either.AsTryOption()).Flatten();

        public static Error AsError(this Exception ex) =>
            ex is ErrorException err
                ? err.Error
                : Error.New(ex.Message);
    }

    public class Issue263
    {
        public readonly Func<long, Unit> fire = i => unit;

        public void Test() => ignore(act(fire));
    }

    public class Issue261
    {
        [Fact]
        public void Test1()
        {
            var ma = Writer.Pure<Seq<string>, int>(100);
            var mb = Writer.Pure<Seq<string>, int>(200);

            var mc = from x in ma
                     from y in mb
                     from _1 in tell(Seq("Hello"))
                     from _2 in tell(Seq("World"))
                     from _3 in tell(Seq($"the result is {x + y}"))
                     select x + y;

            var r = mc.Run();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == Seq("Hello", "World", "the result is 300"));
        }

        [Fact]
        public void Test2()
        {
            var ma = Writer.Pure<Lst<string>, int>(100);
            var mb = Writer.Pure<Lst<string>, int>(200);

            var mc = from x in ma
                     from y in mb
                     from _1 in tell(List("Hello"))
                     from _2 in tell(List("World"))
                     from _3 in tell(List($"the result is {x + y}"))
                     select x + y;

            var r = mc.Run();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == List("Hello", "World", "the result is 300"));
        }

        [Fact]
        public void Test3()
        {
            var ma = (100, Seq<string>());
            var mb = (200, Seq<string>());

            var mc = from x in Writer.write(ma)
                     from y in Writer.write(mb)
                     from _1 in tell(Seq("Hello"))
                     from _2 in tell(Seq("World"))
                     from _3 in tell(Seq($"the result is {x + y}"))
                     select x + y;

            var r = mc.Run();

            Assert.True(r.Value == 300);
            Assert.True(r.Output == Seq("Hello", "World", "the result is 300"));
        }
    }

    public class Issue376
    {
        public static EitherT<string, IO, int> Op1() =>
            Pure(1);

        public static EitherT<string, IO, int> Op2() =>
            IO.Pure(2);

        public static EitherT<string, IO, int> Op3() =>
            Fail("error");

        public static EitherT<string, IO, int> Calculate(int x, int y, int z) =>
            Pure(x + y + z);

        public static int Test() =>
            (from x in Op1()
             from y in Op2()
             from z in Op3()
             from w in Calculate(x, y, z)
             select w)
            .IfLeft(0)
            .As().Run();
    }

    public class Issue376_2
    {
        static async Task<Either<string, int>> Op1() => await 1.AsTask();
        static async Task<Either<string, int>> Op2() => await 2.AsTask();
        static async Task<Either<string, int>> Op3() => await "error".AsTask();
        static async Task<Either<string, int>> Calculate(int x, int y, int z) => await Task.FromResult(x + y + z);

        public static IO<Either<string, int>> Test() =>
            (from x in Op1().ToIO()
             from y in Op2().ToIO()
             from z in Op3().ToIO()
             from w in Calculate(x, y, z).ToIO()
             select w)
            .Run().As();
    }

    public class Issue376_3
    {
        static async Task<Option<int>> Op1() => await 1.AsTask();
        static async Task<Option<int>> Op2() => await 2.AsTask();
        static async Task<Option<int>> Op3() => await Option<int>.None.AsTask();
        static async Task<Option<int>> Calculate(int x, int y, int z) => await Task.FromResult(x + y + z);

        public static IO<Option<int>> Test() =>
            (from x in Op1().ToIO()
             from y in Op2().ToIO()
             from z in Op3().ToIO()
             from w in Calculate(x, y, z).ToIO()
             select w).Run().As();
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
                            .AsEnumerableM()
                            .Traverse(x => x)
                            .Map(x => x.ToArray())
                            .As();
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

            static Either<Error, Func<string, EitherT<Error, IO, R>>> GetCommand<R>(
                Map<string, Func<string, EitherT<Error, IO, R>>> commandMap, 
                Command hostCommand) =>
                     commandMap.Find(hostCommand.Name)
                               .ToEither(new Error());

            internal static EitherT<Error, IO, R> ExecuteCommand<R>(
                Func<string, EitherT<Error, IO, R>> command,
                Command cmd) =>
                    command(cmd.Name);

            static Either<Error, Unit> Validate<R>(
                Map<string, Func<string, EitherT<Error, IO, R>>> commandMap, 
                Command hostCommand) =>
                    commandMap.Find(hostCommand.Name)
                              .Map(_ => unit)
                              .ToEither(new Error());

            public static EitherT<Error, IO, Seq<R>> ExecuteAllAsync<R>(
                Map<string, Func<string, EitherT<Error, IO, R>>> commandMap,
                Seq<Command> hostCommands) =>
                    hostCommands.Map(cmd =>
                        from _ in Validate(commandMap, cmd).ToIO()
                        from f in GetCommand(commandMap, cmd).ToIO()
                        from r in ExecuteCommand(f, cmd)
                        select r)
                       .Traverse(x => x).As();
        }
    }
    
    
    public class Issue1340
    {
        public sealed record CustomExpected(string Message, int Code, string Another) : Expected(Message, Code);

        [Fact]
        public void Test()
        {
            // Arrange
            var      expected = new CustomExpected("Name", 100, "This is loss");
            Eff<int> effect   = liftEff<int>(() => expected);

            // Act
            Fin<int> fin = effect.Run();

            // Assert
            fin.Match(_ => Assert.True(false),
                      error =>
                      {
                          Assert.Equal(error.Code, expected.Code);
                          Assert.Equal(error.Message, expected.Message);

                          var fail = (CustomExpected)error;
                          Assert.Equal(fail.Another, expected.Another);
                      });

        }
    }
}
