using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExtTests
{
    public class TryOptionTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("/", "")]
        [InlineData("one", "one")]
        [InlineData("two/three", "three")]
        [InlineData("four/five/six", "six")]
        public void TryOptionTest1(string input, string expected)
        {
            //when
            string output = GetLastPathObj(input).Match(
                Some: v => v,
                None: () => "empty",
                Fail: e => e.ToString()
                );

            //then
            Assert.Contains(expected, output);
        }
    
        public TryOption<string> GetLastPathObj(string text) =>
            () => text.Split('/').Last();
    }
}
