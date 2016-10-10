using LanguageExt;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
	public class UnionTests
	{
		[Fact]
		public void UnionTest1()
		{
			Union<string, int> x = "Test";

			object value = null;

			x.Match()
				(a => value = a)
				(b => value = b);

			Assert.IsType<string>(value);
		}

		[Fact]
		public void UnionTest2()
		{
			Union<string, int> x = 100;

			object value = null;

			x.Match()
				(v => value = v)
				(v => value = v);

			Assert.IsType<int>(value);
		}

		[Fact]
		public void UnionTest3()
		{
			Union<string, int> x = "Test";

			string value = x.Match<string, int, string>()
				(a => a)
				(b => b.ToString());

			Assert.Equal("Test", value);
		}

		[Fact]
		public void UnionTest4()
		{
			var x = new Union<string, int>(100);

			string value = x.Match<string, int, string>()
				(a => a)
				(b => b.ToString());

			Assert.Equal("100", value);
		}

		[Fact]
		public void UnionTest5()
		{
			var x = new Union<string, int>(100);

			string value = x.Match<string, int, string>()
				(a => a)
				(b => b == 100 ? "Keeping It 100." : "Tea?");

			Assert.Equal("Keeping It 100.", value);
		}

		[Fact]
		public void UnionTest6()
		{
			string value = AOrB(true)
				.Match<string, int, string>()
				(a => a)
				(b => b == 100 ? "Keeping It 100." : "Tea?");

			Assert.Equal("Keeping It 100.", value);
		}

		[Fact]
		public void UnionTest7()
		{
			string value = AOrB(false)
				.Match<string, int, string>()
				(a => a)
				(b => b == 100 ? "Keeping It 100." : "Tea?");

			Assert.Equal("test", value);
		}

		[Fact]
		public void UnionTest8()
		{
			var x = Prelude.ToErrorUnion<int, UnauthorizedAccessException>(() => AOrError(false));
			string value = x
				.Match<int, UnauthorizedAccessException, string>()
				(a => a.ToString())
				(b => "test");

			Assert.Equal("test", value);
		}

		private Union<string, int> AOrB(bool isA)
		{
			if (isA)
			{
				return 100;
			}
			else
			{
				return "test";
			}
		}

		private int AOrError(bool isA)
		{
			if (isA)
			{
				return 100;
			}
			else
			{
				throw new System.UnauthorizedAccessException();
			}
		}
	}
}