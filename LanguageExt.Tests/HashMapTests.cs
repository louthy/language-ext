//using LanguageExt.Trans;
using static LanguageExt.HashMap;
using Xunit;
using System;
using System.Linq;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests;

public class HashMapTests
{
    [Fact]
    public void HashMapGeneratorTest()
    {
        var m1 = HashMap<int, string>();
        m1 = add(m1, 100, "hello");
        Assert.True(m1.Count == 1 && containsKey(m1,100));
    }

    [Fact]
    public void MapGeneratorAndMatchTest()
    {
        var m2 = HashMap( (1, "a"),
                          (2, "b"),
                          (3, "c") );

        m2 = add(m2, 100, "world");

        var res = match(
            m2, 100,
            v  => v,
            () => "failed"
        );

        Assert.True(res == "world");
    }

    [Fact]
    public void MapSetTest()
    {
        var m1 = HashMap( (1, "a"),
                          (2, "b"),
                          (3, "c") );

        var m2 = setItem(m1, 1, "x");

        match( 
            m1, 1, 
            Some: v => Assert.True(v == "a"), 
            None: () => Assert.False(true) 
        );

        match(
            find(m2, 1),
            Some: v => Assert.True(v == "x"),
            None: () => Assert.False(true)
        );
    }

    [Fact]
    public void MapAddInOrderTest()
    {
        var m = HashMap((1, 1));
        m.Find(1).IfNone(() => failwith<int>("Broken"));

        m = HashMap((1, 1), (2, 2));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));

        m = HashMap((1, 1), (2, 2), (3, 3));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));

        m = HashMap((1, 1), (2, 2), (3, 3), (4, 4));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));

        m = HashMap((1, 1), (2, 2), (3, 3), (4, 4), (5, 5));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));
        m.Find(5).IfNone(() => failwith<int>("Broken"));
    }

    [Fact]
    public void MapAddInReverseOrderTest()
    {
        var m = HashMap((2, 2), (1, 1));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));

        m = HashMap((3, 3), (2, 2), (1, 1));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));

        m = HashMap((4, 4), (3, 3), (2, 2), (1, 1));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));

        m = HashMap((5, 5), (4, 4), (3, 3), (2, 2), (1, 1));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));
        m.Find(5).IfNone(() => failwith<int>("Broken"));
    }

    [Fact]
    public void MapAddInMixedOrderTest()
    {
        var m = HashMap((5, 5), (1, 1), (3, 3), (2, 2), (4, 4));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));
        m.Find(5).IfNone(() => failwith<int>("Broken"));

        m = HashMap((1, 1), (3, 3), (5, 5), (2, 2), (4, 4));
        m.Find(1).IfNone(() => failwith<int>("Broken"));
        m.Find(2).IfNone(() => failwith<int>("Broken"));
        m.Find(3).IfNone(() => failwith<int>("Broken"));
        m.Find(4).IfNone(() => failwith<int>("Broken"));
        m.Find(5).IfNone(() => failwith<int>("Broken"));
    }


    [Fact]
    public void MapRemoveTest()
    {
        var m = HashMap((1, "a"),
                        (2, "b"),
                        (3, "c"),
                        (4, "d"),
                        (5, "e"));

        m.Find(1).IfNone(() => failwith<string>("Broken 1"));
        m.Find(2).IfNone(() => failwith<string>("Broken 2"));
        m.Find(3).IfNone(() => failwith<string>("Broken 3"));
        m.Find(4).IfNone(() => failwith<string>("Broken 4"));
        m.Find(5).IfNone(() => failwith<string>("Broken 5"));

        Assert.True(m.Count == 5);

        m = remove(m,4);
        Assert.True(m.Count == 4);
        Assert.True(m.Find(4).IsNone);
        m.Find(1).IfNone(() => failwith<string>("Broken 1"));
        m.Find(2).IfNone(() => failwith<string>("Broken 2"));
        m.Find(3).IfNone(() => failwith<string>("Broken 3"));
        m.Find(5).IfNone(() => failwith<string>("Broken 5"));

        m = remove(m, 1);
        Assert.True(m.Count == 3);
        Assert.True(m.Find(1).IsNone);
        m.Find(2).IfNone(() => failwith<string>("Broken 2"));
        m.Find(3).IfNone(() => failwith<string>("Broken 3"));
        m.Find(5).IfNone(() => failwith<string>("Broken 5"));

        m = remove(m, 2);
        Assert.True(m.Count == 2);
        Assert.True(m.Find(2).IsNone);
        m.Find(3).IfNone(() => failwith<string>("Broken 3"));
        m.Find(5).IfNone(() => failwith<string>("Broken 5"));

        m = remove(m, 3);
        Assert.True(m.Count == 1);
        Assert.True(m.Find(3).IsNone);
        m.Find(5).IfNone(() => failwith<string>("Broken 5"));

        m = remove(m, 5);
        Assert.True(m.Count == 0);
        Assert.True(m.Find(5).IsNone);
    }

    [Fact]
    public void MassAddRemoveTest()
    {
        int max = 100000;

        var items = IterableExtensions.AsIterable(Range(1, max))
                                      .Map( _ => (Key: Guid.NewGuid(), Value: Guid.NewGuid()))
                                      .ToSeq();

        var m = HashMap<Guid, Guid>().AddRange(items);
        Assert.True(m.Count == max);

        foreach (var item in items)
        {
            Assert.True(m.ContainsKey(item.Key));
            m = m.Remove(item.Key);
            Assert.False(m.ContainsKey(item.Key));
            max--;
            Assert.True(m.Count == max);
        }
    }

    [Fact]
    public void HashMapSetTest()
    {
        var map  = HashMap<EqStringOrdinalIgnoreCase, string, int>(("one", 1), ("two",2), ("three", 3));
        var map2 = map.SetItem("One", -1);
        Assert.Equal(3, map2.Count);
        Assert.Equal(-1, map2["one"]);
        Assert.DoesNotContain("one", map2.Keys.AsEnumerable()); // make sure key got replaced, too
        Assert.Contains("One", map2.Keys.AsEnumerable());       // make sure key got replaced, too

        Assert.Throws<ArgumentException>(() => map.SetItem("four", identity));
    }

    [Fact]
    public void EqualsTest()
    {
        Assert.True(HashMap<int, int>().Equals(HashMap<int, int>()));
        Assert.False(HashMap((1, 2)).Equals(HashMap<int, int>()));
        Assert.False(HashMap<int, int>().Equals(HashMap((1, 2))));
        Assert.True(HashMap((1, 2)).Equals(HashMap((1, 2))));
        Assert.False(HashMap((1, 2), (3, 4)).Equals(HashMap((1, 2))));
        Assert.False(HashMap((1, 2)).Equals(HashMap((1, 2), (3, 4))));
        Assert.True(HashMap((1, 2), (3, 4)).Equals(HashMap((1, 2), (3, 4))));
        Assert.True(HashMap((3, 4), (1, 2)).Equals(HashMap((1, 2), (3, 4))));
        Assert.True(HashMap((3, 4), (1, 2)).Equals(HashMap((3, 4), (1, 2))));
    }
        
    [Fact]
    public void FetchBack()
    {
        var init = Seq(69, 1477);
        var rmv  = Seq(69);

        var map = toHashMap(init.Zip(Enumerable.Repeat(1, int.MaxValue)));

        Assert.True(map.ContainsKey(1477)); // false
        Assert.True(map.Find(1477).IsSome); // false


        var minus = map.RemoveRange(rmv);

        Assert.True(minus.Keys.Find(i => i == 1477).IsSome); // true
            
        Assert.True(minus.ContainsKey(1477)); // false
        Assert.True(minus.Find(1477).IsSome); // false

        var boom = minus[1477]; // throws
    }

    [Fact]
    public void HashMapRemoveTest()
    {
        var values = new[] { 1175691501, 613261927, 178639586, 745392133,
                               1071314707, 464997766, 746033505, 2055266377,
                               9321519, 2085595311 };

        var items = toHashMap(values.Zip(values));

        foreach(var value in values)
        {
            items = items.Remove(value);
            Assert.True(!items.Contains(value));
        }
    }

    [Fact]
    public void itemLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var map      = HashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
        var actual   = HashMap<int, string>.item(3).Get(map);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemLensGetShouldThrowExceptionForNonExistingValue()
    {
        Assert.Throws<ArgumentException>(() =>
                                         {
                                             var map    = HashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
                                             var actual = HashMap<int, string>.item(10).Get(map);
                                         });
    }

    [Fact]
    public void itemOrNoneLensGetShouldGetExistingValue()
    {
        var expected = "3";
        var map      = HashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
        var actual   = HashMap<int, string>.itemOrNone(3).Get(map);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
    {
        var expected = Option<string>.None;
        var map      = HashMap((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
        var actual   = HashMap<int, string>.itemOrNone(10).Get(map);

        Assert.Equal(expected, actual);
    }
}
