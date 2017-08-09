using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization;

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
            var r = from a in Task.FromResult(Left<Exception, int>(new Exception("error 1")))
                    from b in Task.FromResult(Right<Exception, int>(1))
                    select a + b;

        }

        static void EqPar()
        {
            var eq = par<string, string, bool>(equals<EqStringOrdinalIgnoreCase, string>, "abc");
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
        public async Task what_i_desire()
        {
            Task<Either<Error, Pixel>> GetPixelE(PixelId id) =>
                GetPixel(id).MatchAsync(
                    Some: p => Right<Error, Pixel>(p),
                    None: () => Error.New("pixel not found")
                );

            var program =
                from pixel in GetPixelE(PixelId.New("wkrp"))
                from id in GenerateLinkId(pixel.Value)
                from resource in ScrapeUrl("http://google.com")
                select resource;

            (await program).Match(
                Right: r => Assert.True(false, "this should not pass"),
                Left: e => Assert.Equal("pixel not found", e.Value)
            );
        }

        [Fact]
        public async Task what_im_forced_to_do()
        {
            var program =
                from pixel in GetPixel(PixelId.New("wkrp")).AsTry("pixel not found")
                from id in GenerateLinkId(pixel.Value).AsTry()
                from resource in ScrapeUrl("http://google.com").AsTry()
                select resource;

            (await program.Try()).Match(
                Succ: r =>
                {
                    Assert.True(false, "this should not pass");
                    return unit;
                },
                Fail: e =>
                {
                    Assert.Equal("pixel not found", e.Message);
                    return unit;
                }
            );
        }
    }

    static class Ext
    {
        public static Try<T> AsTry<TL, T>(this Either<TL, T> either) where TL : NewType<TL, string> =>
            Try(either.Match(
                    Left: e => throw new Exception(e.Value),
                    Right: identity
                ));

        public static Try<T> AsTry<T>(this Option<T> option, string error) =>
            Try(option.Match(
                    None: () => throw new Exception(error),
                    Some: identity
                ));

        public static TryAsync<T> AsTry<TL, T>(this Task<Either<TL, T>> task) where TL : NewType<TL, string> =>
            task.Map(AsTry).ToAsync();

        public static TryAsync<T> AsTry<T>(this Task<Option<T>> task, string error) =>
            task.Map(o => o.AsTry(error)).ToAsync();
    }

    static class ExternalSystem
    {
        public class Error : NewType<Error, string>
        {
            public Error(string value) : base(value) { }
        }

        public static Task<Option<Pixel>> GetPixel(PixelId id) =>
            Task.FromResult(Option<Pixel>.None);

        public static Task<Either<Error, string>> GenerateLinkId(PixelId pixelId) =>
            Task.FromResult(Right<Error, string>($"{pixelId}-1234"));

        public static Task<Either<Error, WebResource>> ScrapeUrl(string url) =>
            Task.FromResult(Right<Error, WebResource>(new WebResource(200)));

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
namespace NickCuthbertOnGitter_RecordsTests
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
}