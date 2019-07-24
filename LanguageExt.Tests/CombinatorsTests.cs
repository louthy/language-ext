using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using static LanguageExt.CombinatorsDynamic;

namespace LanguageExt.Tests
{
    public class CombinatorsTests
    {
        /// <summary>
        /// Derive the Identity combinator
        /// </summary>
        [Fact]
        public void DeriveIdiotBird()
        {
            var I = S(K)(K);

            Assert.True(I(10) == 10);
            Assert.True(I(5) == 5);
        }

        /// <summary>
        /// Derive the Bind combinator
        /// </summary>
        [Fact]
        public void DeriveBluebird()
        {
            var B = S(K(S))(K);
        }

        [Fact]
        public void DeriveCardinalBird()
        {
            //C = S(BBS)(KK) = S(S(KS)K(S(KS)K)S)(KK)

            var B = S(K(S))(K);

            var C = S(B(B)(S))(K(K));
        }
    }
}
