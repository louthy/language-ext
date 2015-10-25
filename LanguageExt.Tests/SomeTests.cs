using Xunit;
using System;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.SomeHelp;

namespace LanguageExtTests
{
    
    public class SomeTests
    {
        [Fact]
        public void CtorTests()
        {
            // should fail when null
            Assert.Throws<ValueIsNullException>(() => new Some<string>(null));

            var x = new Some<string>("ctor should not fail");
            Assert.Equal(x.Value, "ctor should not fail");
        }

        [Fact]
        public void ValueTests()
        {
            // should fail when not initialised
            Assert.Throws<SomeNotInitialisedException>(() => new Some<string>().Value);

            var x = new Some<string>("value should be correct").Value;
            Assert.Equal("value should be correct", x);
        }

        [Fact]
        public void ImplicitValueToSomeTests()
        {
            // should fail when not null
            Assert.Throws<ValueIsNullException>(() => ImplicitValueToSome<string>(null));

            var some = ImplicitValueToSome("value to some");
            Assert.Equal("value to some", some.Value);
        }

        [Fact]
        public void ImplicitSomeToValueTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => ImplicitSomeToValue(new Some<string>()));

            var v = ImplicitSomeToValue(new Some<string>("some to value"));
            Assert.Equal("some to value", v);
        }

        [Fact]
        public void ImplicitSomeToOptionTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => ImplicitSomeToOption(new Some<string>()));

            var opt = ImplicitSomeToOption(new Some<string>("some to option"));
            match(opt,
                Some: v => Assert.Equal("some to option", v),
                None: () => failwith<string>("no value")
                );
        }

        [Fact]
        public void OverrideTests()
        {
            var uninitialisedSome = new Some<string>();
            var initialisedSome = new Some<string>("abc");

            // none of these are allowed to fail per MS rules
            Assert.Equal("(uninitialised)", uninitialisedSome.ToString());
            Assert.Equal(false, uninitialisedSome.Equals(uninitialisedSome));
            Assert.Equal(0, uninitialisedSome.GetHashCode());

            Assert.Equal("abc", initialisedSome.ToString());
            Assert.Equal(true, initialisedSome.Equals("abc"));
            Assert.Equal("abc".GetHashCode(), initialisedSome.GetHashCode());
        }

        [Fact]
        public void IsSomeTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => new Some<string>().IsSome);

            // should be true when filled with value
            Assert.True(new Some<string>("abc").IsSome);
        }

        [Fact]
        public void IsNoneTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => new Some<string>().IsNone);

            // should be false when filled with value
            Assert.False(new Some<string>("abc").IsNone);
        }

        [Fact]
        public void MatchUntypedTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => new Some<string>().MatchUntyped(Some: v => true, None: () => false));

            // should call Some when filled with value
            Assert.True(new Some<string>("abc").MatchUntyped(Some: v => true, None: () => false));
        }

        [Fact]
        public void GetUnderlyingTypeTests()
        {
            // should fail when uninitialised
            Assert.Throws<SomeNotInitialisedException>(() => new Some<string>().GetUnderlyingType());

            // should return type when filled with value
            Assert.Equal(typeof(string), new Some<string>("abc").GetUnderlyingType());
        }

        [Fact]
        public void SomeExtensionTests()
        {
            // should fail when null
            Assert.Throws<ValueIsNullException>(() => SomeExt.ToSome<string>(null));

            // should create Some when valid
            Assert.Equal("abc", SomeExt.ToSome<string>("abc").Value);
        }

        [Fact]
        public void SomeCreateTests()
        {
            // should fail when null
            Assert.Throws<ValueIsNullException>(() => LanguageExt.Some.Create((string)null));

            // should create Some when valid
            Assert.Equal("abc", LanguageExt.Some.Create("abc").Value);
        }



        Option<T> ImplicitSomeToOption<T>(Some<T> some)
        {
            return some;
        }

        Some<T> ImplicitValueToSome<T>(T value)
        {
            return value;
        }

        T ImplicitSomeToValue<T>(Some<T> some)
        {
            return some;
        }
    }
}
