using System.Numerics;
using LanguageExt.DataTypes.Recursive;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    public class RecursiveTests
    {
        [Fact]        
        public static void FibonacciTest()
        {
            var target = 100000;
            BigInteger r1 = RecursiveFibonacci(target);
            var r2 = IterativeFibonacci(target);
            Assert.Equal(r1,r2);            
        }

        [Fact]
        public static void FactorialTest()
        {
            var target = 100000;
            BigInteger r1 = RecursiveFactorial(target);
            BigInteger r2 = IterativeFactorial(target);
            Assert.Equal(r1, r2);
        }


        public static BigInteger RecursiveFactorial(BigInteger factorial) => RecursiveFactorial(factorial, 1);
        
        public static Recursive<BigInteger> RecursiveFactorial(BigInteger factorial, BigInteger product) =>
            factorial <= 1
                ? Return(product*factorial)
                : Recurse(RecursiveFactorial, factorial - 1, product * factorial);


        public static BigInteger IterativeFactorial(BigInteger goal)
        {
            BigInteger product = 1;
            for (BigInteger i = 1; i <= goal; ++i)
            {
                product *= i;                
            }
            return product*1;
        }

        public static BigInteger IterativeFibonacci(BigInteger goal)
        {            
            BigInteger i0 = 0;
            BigInteger i1 = 1;

            for (BigInteger count = 0; count < goal; ++count)
            {
                var temp = i0;
                i0 = i1;
                i1 = temp + i1;
            }
            return i0;
        }

        public static Recursive<BigInteger> RecursiveFibonacci(BigInteger goal) => RecursiveFibonacci(0, 1, 0, goal);

        public static Recursive<BigInteger> RecursiveFibonacci(BigInteger i0, BigInteger i1, BigInteger count, BigInteger goal) =>
            count >= goal                
                ? Return(i0)
                : Recurse(RecursiveFibonacci, i1, i0 + i1, count + 1, goal);
    }
}