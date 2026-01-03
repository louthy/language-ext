using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableUnorderedCollectionsTestSuite<F>
    where F : Foldable<F>
{
    const int ItemsCount = 100;
    readonly int[] Items;
    readonly int ItemsSum;
    readonly int ItemsProduct;
    readonly Func<IEnumerable<int>, K<F, int>> Construct;

    FoldableUnorderedCollectionsTestSuite(Func<IEnumerable<int>, K<F, int>> construct)
    {
        Items = new int[ItemsCount];
        var total = 0;
        var product  = 1;
        for (var i = 0; i < ItemsCount; i++)
        {
            total += i;
            Items[i] = i;
            product *= i;
        }

        ItemsSum = total;
        ItemsProduct = product;
        Construct = construct;
    }

    public static void RunAll(Func<IEnumerable<int>, K<F, int>> construct)
    {
        var suite = new FoldableUnorderedCollectionsTestSuite<F>(construct);
        suite.FoldTest();
        suite.FoldMNoneTest();
        suite.FoldMSomeTest();
        suite.PartitionTest();
        suite.AverageTest();
        suite.MaxOrdTest();
        suite.MaxTest();
        suite.MinOrdTest();
        suite.MinTest();
        suite.IterTest();
        suite.ProductTest();
        suite.SumTest();
        suite.FindAllBackTest();
        suite.FindAllTest();
        suite.FindBackFalseTest();
        suite.FindBackTrueTest();
        suite.FindFalseTest();
        suite.FindTrueTest();
        suite.ContainsEqFalseTest();
        suite.ContainsEqTrueTest();
        suite.ContainsFalseTest();
        suite.ContainsTrueTest();
        suite.ForAllEmptyIsTrueTest();
        suite.ForAllFalseTest();
        suite.ForAllTrueTest();
        suite.ExistsEmptyIsFalseTest();
        suite.ExistsFalseTest();
        suite.ExistsTrueTest();
        suite.CountTest();
        suite.IsEmptyTrueTest();
        suite.IsEmptyFalseTest();
        suite.ToIterableTest();
        suite.ToArrTest();
        suite.ToLstTest();
        suite.ToSeqTest();
    }

    void FoldTest()
    {
        var res = Construct(Items).Fold((s, x) => s + x, 0);
        Assert.True(res == ItemsSum, $"{typeof(F).Name} | FoldTest failed");
    }
    
    void FoldMNoneTest()
    {
        var res = Construct(Items).FoldM((s, x) => x == 4 ? None : Some(s + x), 0);
        Assert.True(res.As() == None, $"{typeof(F).Name} | FoldMNoneTest failed");
    }

    void FoldMSomeTest()
    {
        var res = Construct(Items).FoldM((s, x) => Some(s + x), 0);
        Assert.True(res.As() == Some(ItemsSum), $"{typeof(F).Name} | FoldMSomeTest failed");
    }

    void ToSeqTest()
    {
        var res = Construct(Items).ToSeq().OrderBy(x => x).AsIterable().ToSeq();
        Assert.True(res == toSeq(Items), $"{typeof(F).Name} | ToSeqTest failed");
    }

    void ToLstTest()
    {
        var res = Construct(Items).ToLst().OrderBy(x => x).AsIterable().ToLst();
        Assert.True(res == toList(Items), $"{typeof(F).Name} | ToLstTest failed");
    }

    void ToArrTest()
    {
        var res = Construct(Items).ToArr().OrderBy(x => x).AsIterable().ToArr();
        Assert.True(res == toArray(Items), $"{typeof(F).Name} | ToArrTest failed");
    }

    void ToIterableTest()
    {
        var res = Construct(Items).ToIterable().OrderBy(x => x).AsIterable();
        Assert.True(res == toIterable(Items), $"{typeof(F).Name} | ToIterableTest failed");
    }

    void IsEmptyFalseTest()
    {
        var res = Construct(Items).IsEmpty;
        Assert.False(res, $"{typeof(F).Name} | IsEmptyFalseTest failed");
    }

    void IsEmptyTrueTest()
    {
        var res = Construct([]).IsEmpty;
        Assert.True(res, $"{typeof(F).Name} | IsEmptyTrueTest failed");
    }

    void CountTest()
    {
        var res = Construct(Items).Count;
        Assert.True(res == ItemsCount, $"{typeof(F).Name} | CountTest failed");
    }

    void ExistsTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(Items).Exists(x => x == expect);
        Assert.True(res, $"{typeof(F).Name} | ExistsTrueTest failed");
    }

    void ExistsFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(Items).Exists(x => x == notExpect);
        Assert.False(res, $"{typeof(F).Name} | ExistsFalseTest failed");
    }

    void ExistsEmptyIsFalseTest()
    {
        var res = Construct([]).Exists(x => x == 6);
        Assert.False(res, $"{typeof(F).Name} | ExistsEmptyIsFalseTest failed");
    }

    void ForAllTrueTest()
    {
        var notExpect = ItemsCount * 2;
        var res = Construct(Items).ForAll(x => x < notExpect);
        Assert.True(res, $"{typeof(F).Name} | ForAllTrueTest failed");
    }

    void ForAllFalseTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(Items).ForAll(x => x < expect);
        Assert.False(res, $"{typeof(F).Name} | ForAllFalseTest failed");
    }

    void ForAllEmptyIsTrueTest()
    {
        var res = Construct([]).ForAll(x => x < 5);
        Assert.True(res, $"{typeof(F).Name} | ForAllEmptyIsTrueTest failed");
    }

    void ContainsTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(Items).Contains(expect);
        Assert.True(res, $"{typeof(F).Name} | ContainsTrueTest failed");
    }

    void ContainsFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res = Construct(Items).Contains(notExpect);
        Assert.False(res, $"{typeof(F).Name} | ContainsFalseTest failed");
    }

    void ContainsEqTrueTest()
    {
        var expect = ItemsCount / 2;
        var res = Construct(Items).Contains<EqInt, F, int>(expect);
        Assert.True(res, $"{typeof(F).Name} | ContainsEqTrueTest failed");
    }

    void ContainsEqFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res = Construct(Items).Contains<EqInt, F, int>(notExpect);
        Assert.False(res, $"{typeof(F).Name} | ContainsEqFalseTest failed");
    }

    void FindTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(Items).Find(x => x == expect);
        Assert.True(res == Some(expect), $"{typeof(F).Name} | FindTrueTest failed");
    }

    void FindFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(Items).Find(x => x == notExpect);
        Assert.True(res == None, $"{typeof(F).Name} | FindFalseTest failed");
    }

    void FindBackTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(Items).FindBack(x => x == expect);
        Assert.True(res == Some(expect), $"{typeof(F).Name} | FindBackTrueTest failed");
    }

    void FindBackFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(Items).FindBack(x => x == notExpect);
        Assert.True(res == None, $"{typeof(F).Name} | FindBackFalseTest failed");
    }

    void FindAllTest()
    {
        var res = Construct(Items).FindAll(x => x > 97);
        Assert.True(res == Iterable(99, 98) || res == Iterable(98, 99), $"{typeof(F).Name} | FindAllTest failed");
    }

    void FindAllBackTest()
    {
        var res = Construct(Items).FindAllBack(x => x > 97);
        Assert.True(res == Iterable(99, 98) || res == Iterable(98, 99) , $"{typeof(F).Name} | FindAllBackTest failed");
    }

    void SumTest()
    {
        var res = Construct(Items).Sum();
        Assert.True(res == ItemsSum, $"{typeof(F).Name} | SumTest failed");
    }

    void ProductTest()
    {
        var res = Construct(Items).Product();
        Assert.True(res == ItemsProduct, $"{typeof(F).Name} | ProductTest failed");
    }

    void IterTest()
    {
        var atom = Atom(0);
        var comp = Construct(Items).IterM(x => atom.SwapIO(v => v + x));
        ignore(comp.Run());
        Assert.True(atom.Value == ItemsSum, $"{typeof(F).Name} | IterTest failed");
    }

    void MinTest()
    {
        var res = Construct(Items).Min();
        Assert.True(res == Some(0), $"{typeof(F).Name} | MinTest failed");
    }

    void MinOrdTest()
    {
        var res = Construct(Items).Min<OrdInt, F, int>();
        Assert.True(res == 0, $"{typeof(F).Name} | MinOrdTest failed");
    }

    void MaxTest()
    {
        var res = Construct(Items).Max();
        Assert.True(res == Some(99), $"{typeof(F).Name} | MaxTest failed");
    }

    void MaxOrdTest()
    {
        var res = Construct(Items).Max<OrdInt, F, int>();
        Assert.True(res == 99, $"{typeof(F).Name} | MaxOrdTest failed");
    }

    void AverageTest()
    {
        var res = Construct(Items).Average();
        
        // Subtract 1 because 0 is in our list
        Assert.True(res == ItemsCount / 2 - 1, $"{typeof(F).Name} | AverageTest failed");
    }

    void PartitionTest()
    {
        var res = Construct(Items).Partition(x => (x & 1) == 0);

        Assert.True(res.True.OrderBy(x => x).AsIterable()  == toIterable(Range(0, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest True failed");
        Assert.True(res.False.OrderBy(x => x).AsIterable() == toIterable(Range(1, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest False failed");
    }
}
