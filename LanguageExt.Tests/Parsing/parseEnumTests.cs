using Xunit;

namespace LanguageExt.Tests.Parsing
{
    public class parseEnumTests : AbstractParseTTests<FooBarEnum>
    {
        protected override Option<FooBarEnum> ParseT(string value) => Prelude.parseEnum<FooBarEnum>(value);

        [Fact]
        public void parseEnum_ValidStringFromFoo_SomeFoo() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(FooBarEnum.Foo);

        [Fact]
        public void parseEnum_ValidStringFromBar_SomeBar() =>
            ParseT_ValidStringFromGiven_SomeAsGiven(FooBarEnum.Bar);

    }

    public class parseEnumIgnoreCaseTests : AbstractParseTTests<FooBarEnum>
    {
        protected override Option<FooBarEnum> ParseT(string value) => Prelude.parseEnumIgnoreCase<FooBarEnum>(value);

        [Fact]
        public void parseEnum_ValidStringFromFoo_SomeFoo() =>
            ParseT_ValidStringFromGivenToLower_SomeAsGiven(FooBarEnum.Foo);

        [Fact]
        public void parseEnum_ValidStringFromBar_SomeBar() =>
            ParseT_ValidStringFromGivenToLower_SomeAsGiven(FooBarEnum.Bar);

    }

    public enum FooBarEnum
    {
        Foo,
        Bar
    }
}
