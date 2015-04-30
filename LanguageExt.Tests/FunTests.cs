using System;
using NUnit.Framework;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class FunTests
    {
        [Test] public void LambdaInferTests()
        {
            var fn1 = fun( () => 123 );
            var fn2 = fun( (int a) => 123 + a );
            var fn3 = fun( (int a, int b) => 123 + a + b );
            var fn4 = fun( (int a, int b, int c) => 123 + a + b + c);
            var fn5 = fun( (int a, int b, int c, int d) => 123 + a + b + c + d);
            var fn6 = fun( (int a, int b, int c, int d, int e) => 123 + a + b + c + d + e);
            var fn7 = fun( (int a, int b, int c, int d, int e, int f) => 123 + a + b + c + d + e + f);
            var fn8 = fun( (int a, int b, int c, int d, int e, int f, int g) => 123 + a + b + c + d + e + f + g);

            var fnac1 = fun( () => { } );
            var fnac2 = fun( (int a) => Console.WriteLine(123 + a) );
            var fnac3 = fun( (int a, int b) => Console.WriteLine(123 + a + b));
            var fnac4 = fun( (int a, int b, int c) => Console.WriteLine(123 + a + b + c));
            var fnac5 = fun( (int a, int b, int c, int d) => Console.WriteLine(123 + a + b + c + d));
            var fnac6 = fun( (int a, int b, int c, int d, int e) => Console.WriteLine(123 + a + b + c + d + e));
            var fnac7 = fun( (int a, int b, int c, int d, int e, int f) => Console.WriteLine(123 + a + b + c + d + e + f));
            var fnac8 = fun( (int a, int b, int c, int d, int e, int f, int g) => Console.WriteLine(123 + a + b + c + d + e + f + g));

            var ac1 = act(() => { });
            var ac2 = act((int a) => Console.WriteLine(123 + a));
            var ac3 = act((int a, int b) => Console.WriteLine(123 + a + b));
            var ac4 = act((int a, int b, int c) => Console.WriteLine(123 + a + b + c));
            var ac5 = act((int a, int b, int c, int d) => Console.WriteLine(123 + a + b + c + d));
            var ac6 = act((int a, int b, int c, int d, int e) => Console.WriteLine(123 + a + b + c + d + e));
            var ac7 = act((int a, int b, int c, int d, int e, int f) => Console.WriteLine(123 + a + b + c + d + e + f));
            var ac8 = act((int a, int b, int c, int d, int e, int f, int g) => Console.WriteLine(123 + a + b + c + d + e + f + g));
        }
    }
}
