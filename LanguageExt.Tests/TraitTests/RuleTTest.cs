using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests;

public sealed class UserId 
    : DomainType<UserId, string>, 
      Identifier<UserId>
{
    private readonly string _value;

    private UserId(string value) =>
        _value = value;

    public string To() => _value;

    public bool Equals(UserId? other) => 
        _value.Equals(other?._value);

    public override bool Equals(object? obj) => 
        ReferenceEquals(this, obj) || 
        obj is UserId other && Equals(other);

    public override int GetHashCode() => 
        _value.GetHashCode();

    public static Fin<UserId> From(string repr) =>
        Guid.TryParse(repr, out _)
            ? new UserId(repr)
            : Error.New("Invalid length");

    public static bool operator ==(UserId? left, UserId? right) => 
        object.Equals(left, right);

    public static bool operator !=(UserId? left, UserId? right) => 
        !(left == right);
}

public sealed class UserTable(UserId id, DateOnly addedAt, bool active)
{
    public UserTable(UserId id) : this(id, DateOnly.FromDateTime(DateTime.Now), false) {}

    public UserId Id { get; } = id;

    public DateOnly AddedAt { get; } = addedAt;

    public bool Active { get; } = active;
}

public interface DatabaseIO
{
    IO<Seq<UserTable>> GetUsers();
}

public sealed class InMemoryDatabaseIO(Seq<UserTable> users) : DatabaseIO
{
    public IO<Seq<UserTable>> GetUsers() => IO.pure(users);
}

public sealed class RT(DatabaseIO db) : Has<ReaderT<RT, IO>, DatabaseIO>
{
    public DatabaseIO Database { get; } = db;

    public static K<ReaderT<RT, IO>, DatabaseIO> Ask =>
        Readable.asks<ReaderT<RT, IO>, RT, DatabaseIO>(r => r.Database);
}

public sealed class DoesUserExists<RT> : RuleT<DoesUserExists<RT>, ReaderT<RT, IO>, IO, UserId>
    where RT : Has<ReaderT<RT, IO>, DatabaseIO>
{
    public static K<ReaderT<RT, IO>, bool> Check(K<IO, UserId> value) =>
        from database in RT.Ask
        from users in database.GetUsers()
        from searchId in value.As()
        select users.Exists(u => u.Id == searchId);
}

public sealed class IsUserActive<RT> : RuleT<IsUserActive<RT>, ReaderT<RT, IO>, IO, UserId>
    where RT : Has<ReaderT<RT, IO>, DatabaseIO>
{
    public static K<ReaderT<RT, IO>, bool> Check(K<IO, UserId> value) =>
        from database in RT.Ask
        from users in database.GetUsers()
        from searchId in value.As()
        select users.Exists(u => u.Id == searchId && u.Active);
}

public sealed class UserWasAddedBefore2021<RT> : RuleT<UserWasAddedBefore2021<RT>, ReaderT<RT, IO>, IO, UserId>
    where RT : Has<ReaderT<RT, IO>, DatabaseIO>
{
    public static K<ReaderT<RT, IO>, bool> Check(K<IO, UserId> value) =>
        from database in RT.Ask
        from users in database.GetUsers()
        from searchId in value.As()
        select users.Exists(u => u.Id == searchId && u.AddedAt < new DateOnly(2021, 1, 1));
}

public sealed class RuleTTest
{
    [Fact]
    public void Check_ShouldReturnTrue()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([new UserTable(userId1), new UserTable(userId2)]);

        var tryValue = userId1;

        var mResult = DoesUserExists<RT>.Check(IO.pure(tryValue))
                                          .Run(new RT(users));

        Assert.True(mResult.Run());
    }

    [Fact]
    public void Check_ShouldReturnFalse()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([new UserTable(userId1), new UserTable(userId2)]);

        var tryValue = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var mResult = DoesUserExists<RT>.Check(IO.pure(tryValue))
            .Run(new RT(users));

        Assert.False(mResult.Run());
    }

    [Fact]
    public void Validate_ShouldReturnSuccess()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([new UserTable(userId1), new UserTable(userId2)]);

        var tryValue = userId1;

        var mResult1 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue),
                      K<ReaderT<RT, IO>, Error> (_, _) => throw new UnreachableException())
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult2 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue), K<ReaderT<RT, IO>, Error> () => throw new UnreachableException())
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult3 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue), 
                      ReaderT<RT, IO>.pure(Error.New("No deberia de llegar, nunca")))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        Assert.Equal(tryValue, mResult1.SuccValue);
        Assert.Equal(tryValue, mResult2.SuccValue);
        Assert.Equal(tryValue, mResult3.SuccValue);
    }
    
    [Fact]
    public void Validate_ShouldReturnError()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([new UserTable(userId1), new UserTable(userId2)]);

        var tryValue1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var tryValue2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var tryValue3 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var buildMsg = (UserId id) => $"The user with id {id.To()} does not exist";

        var expMsg1 = buildMsg(tryValue1);
        const string expMsg2 = "Good try, now go of this property";
        const string expMsg3 = "Good try, I'm proud";

        var mResult1 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue1),
                (_, v) => ReaderT<RT, IO>.pure(Error.New(buildMsg(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult2 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue2), 
                () => ReaderT<RT, IO>.pure(Error.New(expMsg2)))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult3 = DoesUserExists<RT>
            .ValidateT(IO.pure(tryValue3),
                ReaderT<RT, IO>.pure(Error.New(expMsg3)))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        Assert.Equal(expMsg1, mResult1.FailValue.Message);
        Assert.Equal(expMsg2, mResult2.FailValue.Message);
        Assert.Equal(expMsg3, mResult3.FailValue.Message);
    }
    
    [Fact]
    public void Not_ShouldNegate()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([new UserTable(userId1), new UserTable(userId2)]);

        var tryValue1 = userId1;
        var tryValue2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var buildMsg = (UserId id) => $"The user with id {id.To()} DOES exist";

        var expMsg1 = buildMsg(tryValue1);

        var mResult1 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .Not<DoesUserExists<RT>>
            .ValidateT(IO.pure(tryValue1),
                (_, v) => ReaderT<RT, IO>.pure(Error.New(buildMsg(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult2 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .Not<DoesUserExists<RT>>
            .ValidateT(IO.pure(tryValue2),
                (_, v) => ReaderT<RT, IO>.pure(Error.New(buildMsg(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        Assert.Equal(expMsg1, mResult1.FailValue.Message);
        Assert.Equal(tryValue2, mResult2.SuccValue);
    }

    [Fact]
    public void All_ShouldVerifyExistingAndActiveUser()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([
            new UserTable(userId1), 
            new UserTable(userId2, new DateOnly(2000, 12, 23), true)
        ]);

        var tryValue1 = userId1;
        var tryValue2 = userId2;
        var tryValue3 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var buildMsgIsNotActive = (UserId id) => $"The user with id {id.To()} IS NOT ACTIVE";
        var buildMsgDoesNotExists = (UserId id) => $"The user with id {id.To()} DOES NOT exist";

        var expNotActiveMsg1 = buildMsgIsNotActive(tryValue1);
        var expNotExistsMsg3 = buildMsgDoesNotExists(tryValue3);

        var mResult1 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .All<DoesUserExists<RT>, IsUserActive<RT>>
            .ValidateT(IO.pure(tryValue1),
                (r, v) => ReaderT<RT, IO>.pure(Error.New(buildMsgIsNotActive(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult2 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .All<DoesUserExists<RT>, IsUserActive<RT>>
            .ValidateT(IO.pure(tryValue2),
                (r, v) => ReaderT<RT, IO>.pure(Error.New("no, jamas")))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult3 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .All<DoesUserExists<RT>, IsUserActive<RT>>
            .ValidateT(IO.pure(tryValue3),
                (r, v) => ReaderT<RT, IO>.pure(Error.New(buildMsgDoesNotExists(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        Assert.Equal(expNotActiveMsg1, mResult1.FailValue.Message);
        Assert.Equal(tryValue2, mResult2.SuccValue);
        Assert.Equal(expNotExistsMsg3, mResult3.FailValue.Message);

    }

    [Fact]
    public void Any_ShouldVerifyEitherActiveOrOldEnoght()
    {
        var userId1 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId2 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();
        var userId3 = UserId.From(Guid.NewGuid().ToString()).ThrowIfFail();

        var users = new InMemoryDatabaseIO([
            new UserTable(userId1, DateOnly.FromDateTime(DateTime.Now), true),
            new UserTable(userId2, new DateOnly(2000, 12, 23), false),
            new UserTable(userId3)
        ]);

        var tryValue1 = userId1;
        var tryValue2 = userId2;
        var tryValue3 = userId3;

        var buildMsgIsNotActiveOrOldEnought = (UserId id) => $"The user with id {id.To()} IS NOT ACTIVE or OLD ENOUGH";

        var expInvalid3 = buildMsgIsNotActiveOrOldEnought(tryValue3);

        var mResult1 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .Any<IsUserActive<RT>, UserWasAddedBefore2021<RT>>
            .ValidateT(IO.pure(tryValue1),
                (r, v) => ReaderT<RT, IO>.pure(Error.New("no, jamas")))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult2 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .Any<IsUserActive<RT>, UserWasAddedBefore2021<RT>>
            .ValidateT(IO.pure(tryValue2),
                (r, v) => ReaderT<RT, IO>.pure(Error.New("no, jamas")))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        var mResult3 = RuleT<ReaderT<RT, IO>, IO>.For<UserId>
            .Any<IsUserActive<RT>, UserWasAddedBefore2021<RT>>
            .ValidateT(IO.pure(tryValue3),
                (r, v) => ReaderT<RT, IO>.pure(Error.New(buildMsgIsNotActiveOrOldEnought(v))))
            .Run().As()
            .Run(new RT(users)).As()
            .Run();

        Assert.Equal(tryValue1, mResult1.SuccValue);
        Assert.Equal(tryValue2, mResult2.SuccValue);
        Assert.Equal(expInvalid3, mResult3.FailValue.Message);

    }
}
