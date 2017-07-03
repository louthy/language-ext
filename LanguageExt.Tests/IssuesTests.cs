using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
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
    }

    public class ADUser : NewType<ADUser, string> { public ADUser(string u) : base(u) { } }
    public class UserMapping : NewType<UserMapping, string> { public UserMapping(string u) : base(u) { } }

}
