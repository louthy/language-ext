using LanguageExt;
using System;
using System.Runtime.Serialization;

namespace RecordsNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            GitterTests.TestSerial();
        }
    }

    public class TenantId : NewType<TenantId, int> { public TenantId(int value) : base(value) { } };
    public class UserId : NewType<UserId, int> { public UserId(int value) : base(value) { } };
    public class Instant : NewType<Instant, int> { public Instant(int value) : base(value) { } };

    public class Collector : Record<Collector>
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
        public static void TestSerial()
        {
            var x = new Collector(new CollectorId(1), "nick", TenantId.New(2), UserId.New(3), Instant.New(4));
            var y = new Collector(new CollectorId(1), "nick", TenantId.New(2), UserId.New(3), Instant.New(4));
            var z = new Collector(new CollectorId(4), "nick", TenantId.New(2), UserId.New(3), Instant.New(4));

            var z1 = x == y;
            var z2 = x.Equals(y);
            var z3 = x.Equals((object)y);

            var nz1 = x == z;
            var nz2 = x.Equals(z);
            var nz3 = x.Equals((object)z);

        }
    }

    public abstract class NonZeroIntegerId
    {
        protected NonZeroIntegerId(int value)
        {
            Value = value;
        }
        public int Value { get; }

        public static implicit operator int(NonZeroIntegerId id) => id.Value;
    }

    public class CollectorId : NonZeroIntegerId
    {
        public CollectorId(int id) : base(id)
        {
        }
    }
}