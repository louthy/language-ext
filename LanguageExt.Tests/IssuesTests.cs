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

        [Fact]
        public void Issue208()
        {
            var r2 = from a in Task.FromResult(Option<int>.None)
                     from b in Task.FromResult(Some(1))
                     select a + b;

            Assert.True(r2.Result == None);

            var r = from a in Task.FromResult(Left<Error, int>(Error.New("error 1")))
                    from b in Task.FromResult(Right<Error, int>(1))
                    select a + b;

            Assert.True(r.Result == Left<Error, int>(Error.New("error 1")));
        }

        static void EqPar()
        {
            var eq = par<string, string, bool>(equals<EqStringOrdinalIgnoreCase, string>, "abc");
        }
    }

    public class ADUser : NewType<ADUser, string> { public ADUser(string u) : base(u) { } }
    public class UserMapping : NewType<UserMapping, string> { public UserMapping(string u) : base(u) { } }
}
