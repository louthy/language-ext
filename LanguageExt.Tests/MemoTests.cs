using Xunit;
using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.List;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class MemoTests
    {
        [Fact]
        public void MemoTest1()
        {
            var saved = DateTime.Now;
            var date = saved;

            var f = Prelude.memo(() => date.ToString());

            var res1 = f();

            date = DateTime.Now.AddDays(1);

            var res2 = f();

            Assert.True(res1 == res2);
        }

        [Fact]
        public void MemoTest2()
        {
            var fix = 0;
            var count = 100;

            Func<int, int> fn = x => x + fix;

            var m = fn.MemoUnsafe();

            var nums1 = map(Range(0, count), i => m(i));

            fix = 1000;

            var nums2 = map(Range(0, count), i => m(i));

            Assert.True(
                length(filter(zip(nums1, nums2, (a, b) => a == b), v => v)) == count
                );
        }
        
        // Commenting out because this test is unreliable when all the other tests are
        // running.  This functionality is likely never going to change, so I'm fine with
        // that for now.
        //[Fact]
        //public void MemoTest3()
        //{
        //    GC.Collect();
        //    var fix = 0;
        //    var count = 100;
        //    Func<int, int> fn = x => x + fix;
        //    var m = fn.Memo();
        //    var nums1 = freeze(map(Range(0, count), i => m(i)));
        //    fix = 100;
        //    var nums2 = freeze(map(Range(0, count), i => m(i)));
        //    var matches = length(filter(zip(nums1, nums2, (a, b) => a == b), v => v));
        //    Assert.True(matches == count, "Numbers don't match (" + matches + " total matches, should be " + count + ")");
        //}

        [Fact]
        public void ListMemoTest()
        {
            var vals = List(1,2,3,4,5).Memo();

            Assert.True(vals.Sum() == 15);
            Assert.True(vals.Sum() == 15);
        }

        /*      
            Uncomment this if you have time on your hands

        [Fact]
        public void MemoMemoryTest()
        {
            var mbStart = GC.GetTotalMemory(false) / 1048576L;

            Func<int, string> fn = x => x.ToString();
            var m = fn.Memo();

            Range(0, Int32.MaxValue).Iter(i => m(i));

            var mbFinish = GC.GetTotalMemory(false) / 1048576L;

            Assert.True(mbFinish - mbStart < 30);
        }
        */
    }
}
