using System;
using Xunit;

namespace LanguageExt.Tests
{
    public partial class RecordCodeGenTests
    {
        [Record]
        public partial class MyRecord
        {
            public int DateId { get; }
    
            // BUG(?): this works only if method is named @New
            public static MyRecord New(DateTime dt) => MyRecord.New(dt.Year * 10000 + dt.Month * 100 + dt.Day);
        }
    
        [Fact]
        void TestAdditionalNew()
        {
            Assert.Equal(20201231, MyRecord.New(20201231).DateId);
            Assert.Equal(20201231, MyRecord.New(new DateTime(2020, 12, 31)).DateId);
            
        }
    }
}
