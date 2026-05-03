using Xunit;
using System;
using System.Linq;

namespace LanguageExt.Tests;

public class ArrayTests
{
    [Fact]
    public void ConsTest1()
    {
        var test = 1.Cons(2.Cons(3.Cons(4.Cons(Lst.empty<int>()))));

        var array = test.ToArray();

        Assert.True(array[0] == 1);
        Assert.True(array[1] == 2);
        Assert.True(array[2] == 3);
        Assert.True(array[3] == 4);
    }

    [Fact]
    public void ListConstruct()
    {
        var test = Arr(1, 2, 3, 4, 5);

        var array = test.ToArray();

        Assert.True(array[0] == 1);
        Assert.True(array[1] == 2);
        Assert.True(array[2] == 3);
        Assert.True(array[3] == 4);
        Assert.True(array[4] == 5);
    }

    [Fact]
    public void MapTestFluent()
    {
        var res = Arr(1, 2, 3, 4, 5)
                 .Map(x => x * 10)
                 .Filter(x => x > 20)
                 .Fold((x, s) => s + x, 0);

        Assert.True(res == 120);
    }

    [Fact]
    public void ReduceTestFluent()
    {
        var res = Arr(1, 2, 3, 4, 5)
                 .Map(x => x * 10)
                 .Filter(x => x > 20)
                 .Reduce((x, s) => s + x);

        Assert.True(res == 120);
    }

    [Fact]
    public void ReverseListTest1()
    {
        var list = Arr(1, 2, 3, 4, 5);
        var rev  = list.Reverse();

        Assert.True(rev[0] == 5);
        Assert.True(rev[4] == 1);
    }

    [Fact]
    public void ReverseListTest2()
    {
        var list = Arr(1, 2, 3, 4, 5);
        var rev  = list.Reverse();

        Assert.True(rev.IndexOf(1) == 4, "Should have been 4, actually is: " + rev.IndexOf(1));
        Assert.True(rev.IndexOf(5) == 0, "Should have been 0, actually is: " + rev.IndexOf(5));
    }

    [Fact]
    public void ReverseListTest3()
    {
        var list = Arr(1, 1, 2, 2, 2);
        var rev  = list.Reverse();

        Assert.True(rev.IndexOfBack(1) == 0, "Should have been 0, actually is: " + rev.IndexOfBack(1));
        Assert.True(rev.IndexOfBack(2) == 2, "Should have been 2, actually is: " + rev.IndexOfBack(2));
    }

    [Fact]
    public void OpEqualTest()
    {
        var goodOnes = Arr(
            (Lst(1, 2, 3), Lst(1, 2, 3)),
            (Lst<int>.Empty, Lst<int>.Empty)
        );
        var badOnes = Arr(
            (Lst(1, 2, 3), Lst(1, 2, 4)),
            (Lst(1, 2, 3), Lst<int>.Empty)
        );

        goodOnes.Iter(t => t.Iter((fst, snd) =>
                                  {
                                      Assert.True(fst  == snd, $"'{fst}' == '{snd}'");
                                      Assert.False(fst != snd, $"'{fst}' != '{snd}'");
                                  }));

        badOnes.Iter(t => t.Iter((fst, snd) =>
                                 {
                                     Assert.True(fst  != snd, $"'{fst}' != '{snd}'");
                                     Assert.False(fst == snd, $"'{fst}' == '{snd}'");
                                 }));
    }


    [Fact]
    public void ArrShouldNotStackOverflowOnEquals()
    {
        var arr = default(Arr<Arr<double>>);
        Assert.True(arr.Equals(arr));
    }

    [Fact]
    public void EqualsTest()
    {
        Assert.False(Arr(1, 2, 3).Equals(Arr<int>()));
        Assert.False(Arr<int>().Equals(Arr(1, 2, 3)));
        Assert.True(Arr<int>().Equals(Arr<int>()));
        Assert.True(Arr(1).Equals(Arr(1)));
        Assert.True(Arr(1, 2).Equals(Arr(1, 2)));
        Assert.False(Arr(1, 2).Equals(Arr(1, 2, 3)));
        Assert.False(Arr(1, 2, 3).Equals(Arr(1, 2)));
    }


    [Fact]
    public void itemLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var array    = Arr("0", "1", "2", "3", "4", "5");
        var actual   = Arr<string>.item(3).Get(array);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemLensGetShouldThrowExceptionForNonExistingValue()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
                                                {
                                                    var array  = Arr("0", "1", "2", "3", "4", "5");
                                                    var actual = Arr<string>.item(10).Get(array);
                                                });
    }

    [Fact]
    public void itemOrNoneLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var array    = Arr("0", "1", "2", "3", "4", "5");
        var actual   = Arr<string>.itemOrNone(3).Get(array);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
    {
        var expected = Option<string>.None;
        var array    = Arr("0", "1", "2", "3", "4", "5");
        var actual   = Arr<string>.itemOrNone(10).Get(array);

        Assert.Equal(expected, actual);
    }
}
