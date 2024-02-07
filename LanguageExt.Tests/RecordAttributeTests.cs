using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests
{
    public partial class RecordAttributeTests
    {
        [Record]
        public partial class TestRecord
        {
            [Ord(typeof(OrdStringOrdinalIgnoreCase))]
            [Eq(typeof(OrdStringOrdinalIgnoreCase))]
            [Hashable(typeof(OrdStringOrdinalIgnoreCase))]
            public string DirPath { get; }
        }

        [Fact]
        public void RecordCustomComparer()
        {
            var test = TestRecord.New(@"c:\temp");

            Assert.Equal(TestRecord.New(@"C:\Temp"), test);
            Assert.Equal(TestRecord.New(@"C:\Temp").GetHashCode(), test.GetHashCode());
            Assert.Equal(0, TestRecord.New(@"C:\Temp").CompareTo(test));
        }

        [Fact]
        public void RecordCustomComparerAttributes()
        {
            var attributes = typeof(TestRecord).GetProperty(nameof(TestRecord.DirPath))!.GetCustomAttributes(false);

            Assert.Equal(3, attributes.Length);
        }
    }
}
