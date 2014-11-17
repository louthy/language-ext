using NUnit.Framework;
using LanguageExt.Prelude;

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

            var ac1 = fun( () => { } );
            var ac2 = fun( (int a) => { ignore(() => 123 + a); } );
            var ac3 = fun( (int a, int b) => { ignore(() => 123 + a + b); } );
            var ac4 = fun( (int a, int b, int c) => { ignore(() => 123 + a + b + c); });
            var ac5 = fun( (int a, int b, int c, int d) => { ignore(() => 123 + a + b + c + d); } );
            var ac6 = fun( (int a, int b, int c, int d, int e) => { ignore(() => 123 + a + b + c + d + e); });
            var ac7 = fun( (int a, int b, int c, int d, int e, int f) => { ignore(() => 123 + a + b + c + d + e + f); });
            var ac8 = fun( (int a, int b, int c, int d, int e, int f, int g) => { ignore(() => 123 + a + b + c + d + e + f + g); });
        }
    }
}
