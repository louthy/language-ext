using System;
using System.Collections.Generic;
using System.Text;
using static LanguageExt.Tests.AorBCon;
using Xunit;

namespace LanguageExt.Tests {
    [Union]
    public interface AorB {
        public AorB A();
        public AorB B(int value);
    }

    public class UnionTests {
        public static IEnumerable<object[]> DataArray<T1, T2, T3>((T1, T2, T3)[] rows) =>
            rows.Map(row => {
                (var t1, var t2, var t3) = row;
                return new object[] { t1, t2, t3 };
            });

        public static IEnumerable<object[]> CompareToUnspecialized_Data() => DataArray(new (AorB, AorB, int) [] {
            (A(),  A(),  0),
            (A(),  B(0), 1),
            (B(0), A(),  1),
            (B(0), B(0), 0),
            (B(0), B(1), -1),
            (B(1), B(0), 1),
        });
            
        [Theory]
        [MemberData(nameof(CompareToUnspecialized_Data), new object[]{ })]
        void CompareTo_Unspecialized(AorB left, AorB right, int expected) {
            Assert.Equal(
                expected,
                ((IComparable) left).CompareTo((IComparable) right));
        }

        [Fact]
        void CompareTo_Specialized_A() {
            Assert.Equal(0, ((IComparable<A>) A()).CompareTo((A) A()));
        }
        public static IEnumerable<object[]> CompareTo_Specialized_B_Data() => DataArray(new (AorB, AorB, int) [] {
            (B(0), B(0), 0),
            (B(0), B(1), -1),
            (B(1), B(0), 1),
        });

        [Theory]
        [MemberData(nameof(CompareTo_Specialized_B_Data), new object[] { })]
        void CompareTo_Specialized_B(AorB left, AorB right, int expected) {
            Assert.Equal(expected, ((IComparable<B>) left).CompareTo((B) right));
        }
        public static IEnumerable<object[]> EqualsTest_Data() => DataArray(new (AorB, AorB, bool) [] {
            (A(),  A(),  true),
            (A(),  B(0), false),
            (B(0), A(),  false),
            (B(0), B(0), true),
            (B(0), B(1), false),
            (B(1), B(0), false),
        });

        [Theory]
        [MemberData(nameof(EqualsTest_Data), new object[] { })]
        public void EqualsTest(AorB left, AorB right, bool expected) =>
            Assert.Equal(expected, left.Equals(right));
    }
}
