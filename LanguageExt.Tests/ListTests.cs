using Xunit;
using System;
using System.Linq;
using static LanguageExt.Lst;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public class ListTests
{
    [Fact]
    public void ConsTest1()
    {
        var test = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons(empty<int>())))));

        var array = test.ToArray();

        Assert.True(array[0] == 1);
        Assert.True(array[1] == 2);
        Assert.True(array[2] == 3);
        Assert.True(array[3] == 4);
        Assert.True(array[4] == 5);
    }

    [Fact]
    public void ListConstruct()
    {
        var test = Lst(1, 2, 3, 4, 5);

        var array = test.ToArray();

        Assert.True(array[0] == 1);
        Assert.True(array[1] == 2);
        Assert.True(array[2] == 3);
        Assert.True(array[3] == 4);
        Assert.True(array[4] == 5);
    }

    [Fact]
    public void MapTest()
    {
        // Generates 10,20,30,40,50
        var input   = Lst(1, 2, 3, 4, 5);
        var output1 = +map(x => x * 10, input);

        // Generates 30,40,50
        var output2 = MonoidK.filter(output1, x => x > 20);

        // Generates 120
        var output3 = Foldable.fold((x, s) => s + x, 0, output2);

        Assert.True(output3 == 120);
    }

    [Fact]
    public void MapTestFluent()
    {
        var res = Lst(1, 2, 3, 4, 5)
                 .Map(x => x * 10)
                 .Filter(x => x > 20)
                 .Fold((x, s) => s + x, 0);

        Assert.True(res == 120);
    }

    [Fact]
    public void RangeTest1()
    {
        var r = Range(0, 10).AsIterable();
        for (var i = 0; i < 10; i++)
        {
            Assert.True(r.First() == i);
            r = r.Skip(1);
        }
    }

    [Fact]
    public void RangeTest2()
    {
        var r = Range(0, 100, 10).AsIterable();
        for (var i = 0; i < 10; i+=10)
        {
            Assert.True(r.First() == i);
            r = r.Skip(1);
        }
    }

    [Fact]
    public void RangeTest4()
    {
        var r = Range('a', 'f');
        Assert.True(string.Join("", r) == "abcdef");
    }

    [Fact]
    public void RangeTest5()
    {
        var r = Range('f', 'a');
        Assert.True(string.Join("", r) == "fedcba");
    }

    [Fact]
    public void RepeatTest()
    {
        var r = Iterable.repeat("Hello", 10);

        foreach (var item in r)
        {
            Assert.True(item == "Hello");
        }
    }


    [Fact]
    public void GenerateTest()
    {
        var r = Iterable.generate(10, i => "Hello " + i );

        for (int i = 0; i < 10; i++)
        {
            Assert.True(r.First() == "Hello " +i);
            r = r.Skip(1);
        }
    }

    [Fact]
    public void UnfoldTest()
    {
        var test = Lst(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181);

        var fibs = Iterable.take(Iterable.unfold((0, 1), tup => map(tup, (a, b) => Some((a, (b, a + b))))), 20);

        Assert.True( test.SequenceEqual(fibs) );
    }

    [Fact]
    public void UnfoldSingleTest()
    {
        var e = new Exception("Outer", new Exception("Inner"));

        var list = Iterable.unfold(e, state => Optional(state.InnerException));

        var res = list.ToList();

        Assert.True(res[0].Message == "Outer" && res[1].Message == "Inner");
    }

    [Fact]
    public void ReverseListTest1()
    {
        var list = Lst(1, 2, 3, 4, 5);
        var rev  = list.Reverse();

        Assert.True(rev[0] == 5);
        Assert.True(rev[4] == 1);
    }

    [Fact]
    public void ReverseListTest2()
    {
        var list = Lst(1, 2, 3, 4, 5);
        var rev  = list.Reverse();

        Assert.True(rev.IndexOf(1) == 4, "Should have been 4, actually is: " + rev.IndexOf(1));
        Assert.True(rev.IndexOf(5) == 0, "Should have been 0, actually is: " + rev.IndexOf(5));
    }

    [Fact]
    public void ReverseListTest3()
    {
        var list = Lst(1, 1, 2, 2, 2);
        var rev  = list.Reverse();

        Assert.True(rev.LastIndexOf(1) == 4, "Should have been 4, actually is: " + rev.LastIndexOf(1));
        Assert.True(rev.LastIndexOf(2) == 2, "Should have been 2, actually is: " + rev.LastIndexOf(5));
    }

    [Fact]
    public void OpEqualTest()
    {
        var goodOnes = Lst((Lst(1, 2, 3), Lst(1, 2, 3)),
                           (Lst<int>.Empty, Lst<int>.Empty));
        
        var badOnes = Lst((Lst(1, 2, 3), Lst(1, 2, 4)),
                          (Lst(1, 2, 3), Lst<int>.Empty));

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
    public void IterSimpleTest()
    {
        var embeddedSideEffectResult = 0;
        var expression = from dummy in Some(unit).ToIterable()
                         from i in Lst(2, 3, 5)
                         let _ = fun(() => embeddedSideEffectResult += i)()
                         select i;

        Assert.Equal(0, embeddedSideEffectResult);

        var sideEffectByAction = 0;

        expression.AsIterable().Iter(i => sideEffectByAction += i * i);
        Assert.Equal(2     + 3     + 5, embeddedSideEffectResult);
        Assert.Equal(2 * 2 + 3 * 3 + 5 * 5, sideEffectByAction);
    }

    [Fact]
    public void IterPositionalTest()
    {
        var embeddedSideEffectResult = 0;
        var expression = from dummy in Some(unit).ToIterable()
                         from i in Lst(2, 3, 5)
                         let _ = fun(() => embeddedSideEffectResult += i)()
                         select i;

        Assert.Equal(0, embeddedSideEffectResult);

        var sideEffectByAction = 0L;

        expression.AsIterable().Iter((i, pos) => sideEffectByAction += i * pos);
        Assert.Equal(2     + 3     + 5, embeddedSideEffectResult);
        Assert.Equal(2 * 0 + 3 * 1 + 5 * 2, sideEffectByAction);
    }

    [Fact]
    public void SkipLastTest3()
    {
        var list = Lst(1, 2, 3, 4, 5);

        var skipped = list.SkipLast(2).AsIterable().ToLst();

        Assert.True(skipped == Lst(1, 2, 3));
    }

    [Fact]
    public void SkipLastTest4()
    {
        var list = Lst<int>();

        var skipped = list.SkipLast(2).AsIterable().ToLst();

        Assert.True(skipped == list);
    }

    [Fact]
    public void SetItemTest()
    {
        var lint = new Lst<int>();
        lint = lint.Insert(0, 0).Insert(1, 1).Insert(2, 2).Insert(3, 3);

        Assert.True(lint[0] == 0);
        Assert.True(lint[1] == 1);
        Assert.True(lint[2] == 2);
        Assert.True(lint[3] == 3);

        lint = lint.SetItem(2, 500);

        Assert.True(lint[0] == 0);
        Assert.True(lint[1] == 1);
        Assert.True(lint[2] == 500);
        Assert.True(lint[3] == 3);
    }

    [Fact]
    public void RemoveAllTest()
    {
        var test = Lst(1, 2, 3, 4, 5);
        Assert.True(test.RemoveAll(x => x % 2 == 0) == Lst(1, 3, 5));
    }

    [Fact]
    public void RemoveAtInsertTest()
    {
        Lst<int> lint = new Lst<int>();
        lint = lint.Insert(0, 0).Insert(1, 1).Insert(2, 2).Insert(3, 3);

        Assert.True(lint[0] == 0);
        Assert.True(lint[1] == 1);
        Assert.True(lint[2] == 2);
        Assert.True(lint[3] == 3);

        lint = lint.RemoveAt(2);

        Assert.True(lint[0] == 0);
        Assert.True(lint[1] == 1);
        Assert.True(lint[2] == 3);

        lint = lint.Insert(2, 500);

        Assert.True(lint[0] == 0);
        Assert.True(lint[1] == 1);
        Assert.True(lint[2] == 500);
        Assert.True(lint[3] == 3);
    }

    [Fact]
    public void RemoveRange()
    {
        var list = Lst(1, 2, 3, 4);

        Assert.Equal(list.RemoveRange(2, 2), Lst(1, 2));
        Assert.Throws<IndexOutOfRangeException>(() => list.RemoveRange(2, 3));
    }

    [Fact]
    public void SetItemManyTest()
    {
        var range = Range(0, 100).ToLst();
        for (var i = 0; i < 100; i++)
        {
            range = range.SetItem(i, i * 2);
            Assert.True(range[i] == i  * 2);
            for(var b = 0; b < i; b++)
            {
                Assert.True(range[b] == b * 2);
            }
            for (var a = i + 1; a < 100; a++)
            {
                Assert.True(range[a] == a);
            }
        }
    }

    [Fact]
    public void RemoveAtInsertManyTest()
    {
        var range = Range(0, 100).ToLst();
        for (var i = 0; i < 100; i++)
        {
            range = range.RemoveAt(i);
            Assert.True(range.Count == 99);
            range = range.Insert(i, i * 2);
            Assert.True(range[i] == i * 2);
            for (var b = 0; b < i; b++)
            {
                Assert.True(range[b] == b * 2);
            }
            for (var a = i + 1; a < 100; a++)
            {
                Assert.True(range[a] == a);
            }
        }
    }

    [Fact]
    public void EqualsTest()
    {
        Assert.False(Lst(1, 2, 3).Equals(Lst<int>()));
        Assert.False(Lst<int>().Equals(Lst(1, 2, 3)));
        Assert.True(Lst<int>().Equals(Lst<int>()));
        Assert.True(Lst(1).Equals(Lst(1)));
        Assert.True(Lst(1, 2).Equals(Lst(1, 2)));
        Assert.False(Lst(1, 2).Equals(Lst(1, 2, 3)));
        Assert.False(Lst(1, 2, 3).Equals(Lst(1, 2)));
    }

    [Fact]
    public void ListShouldRemoveByReference()
    {
        var o0 = new object();
        var o1 = new object();
        var o2 = new object();
        var l  = Lst(o0, o1);
        l = l.Remove(o2);
        Assert.Equal(2, l.Count);
        l = l.Remove(o0);
        Assert.Equal(1, l.Count);
        l = l.Remove(o1);
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void ListShouldRemoveByReferenceForReverseLists()
    {
        var o0 = new Object();
        var o1 = new Object();
        var o2 = new Object();
        var l  = Lst(o0, o1).Reverse();
        l = l.Remove(o2);
        Assert.Equal(2, l.Count);
        l = l.Remove(o0);
        Assert.Equal(1, l.Count);
        l = l.Remove(o1);
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void itemLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var list     = Lst("0","1", "2", "3", "4", "5");
        var actual   = Lst<string>.item(3).Get(list);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemLensGetShouldThrowExceptionForNonExistingValue()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
                                                {
                                                    var list   = Lst("0", "1", "2", "3", "4", "5");
                                                    var actual = Lst<string>.item(10).Get(list);
                                                });
    }

    [Fact]
    public void itemOrNoneLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var list     = Lst("0", "1", "2", "3", "4", "5");
        var actual   = Lst<string>.itemOrNone(3).Get(list);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
    {
        var expected = Option<string>.None;
        var list     = Lst("0", "1", "2", "3", "4", "5");
        var actual   = Lst<string>.itemOrNone(10).Get(list);

        Assert.Equal(expected, actual);
    }
}
