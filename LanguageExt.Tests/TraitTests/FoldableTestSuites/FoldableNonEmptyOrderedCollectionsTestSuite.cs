using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableNonEmptyOrderedCollectionsTestSuite<F>
    where F : Foldable<F>
{
    const int ItemsCount = 100;
    readonly int[] Items;
    readonly int ItemsSum;
    readonly int EndItemsSum;
    readonly int EndItemCutOff;
    readonly int ItemsProduct;
    readonly Func<IEnumerable<int>, K<F, int>> Construct;

    FoldableNonEmptyOrderedCollectionsTestSuite(Func<IEnumerable<int>, K<F, int>> construct)
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

        var start    = ItemsCount - 1;
        var end      = start      - 10;
        var endTotal = 0;
        for (var i = ItemsCount - 1; i >= end; i--)
        {
            endTotal += i;
        }

        ItemsSum = total;
        ItemsProduct = product;
        EndItemCutOff = end;
        EndItemsSum = endTotal;
        Construct = construct;
    }

    public static void RunAll(Func<IEnumerable<int>, K<F, int>> construct)
    {
        var suite = new FoldableNonEmptyOrderedCollectionsTestSuite<F>(construct);
        suite.FoldTest();
        suite.FoldBackTest();
        suite.FoldMNoneTest();
        suite.FoldBackNoneMTest();
        suite.FoldMSomeTest();
        suite.FoldBackSomeMTest();
        suite.FoldWhileStateTest();
        suite.FoldWhileValueTest();
        suite.FoldBackWhileStateTest();
        suite.FoldBackWhileValueTest();
        suite.FoldMaybeTest();
        suite.FoldBackMaybeTest();
        suite.FoldWhileMTest();
        suite.FoldBackWhileMTest();
        suite.FoldUntilStateTest();
        suite.FoldUntilValueTest();
        suite.FoldBackUntilStateTest();
        suite.FoldBackUntilValueTest();
        suite.FoldUntilMTest();
        suite.FoldBackUntilMTest();
        suite.PartitionTest();
        suite.AtTest();
        suite.AverageTest();
        suite.MaxOrdTest();
        suite.MaxTest();
        suite.MinOrdTest();
        suite.MinTest();
        suite.IterTest();
        suite.LastSomeTest();
        suite.HeadSomeTest();
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
        suite.ForAllFalseTest();
        suite.ForAllTrueTest();
        suite.ExistsFalseTest();
        suite.ExistsTrueTest();
        suite.CountTest();
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

    void FoldBackTest()
    {
        var res = Construct(Items).FoldBack((s, x) => s + x, 0);
        Assert.True(res == ItemsSum, $"{typeof(F).Name} | FoldBackTest failed");
    }

    void FoldMNoneTest()
    {
        var res = Construct(Items).FoldM((s, x) => x == 4 ? None : Some(s + x), 0);
        Assert.True(res.As() == None, $"{typeof(F).Name} | FoldMNoneTest failed");
    }

    void FoldBackNoneMTest()
    {
        var res = Construct(Items).FoldBackM((s, x) => x == 4 ? None : Some(s + x), 0);
        Assert.True(res.As() == None, $"{typeof(F).Name} | FoldBackNoneMTest failed");
    }

    void FoldMSomeTest()
    {
        var res = Construct(Items).FoldM((s, x) => Some(s + x), 0);
        Assert.True(res.As() == Some(ItemsSum), $"{typeof(F).Name} | FoldMSomeTest failed");
    }

    void FoldBackSomeMTest()
    {
        var res = Construct(Items).FoldBackM((s, x) => Some(s + x), 0);
        Assert.True(res.As() == Some(ItemsSum), $"{typeof(F).Name} | FoldBackSomeMTest failed");
    }

    void FoldWhileStateTest()
    {
        var res = Construct(Items).FoldWhile((s, x) => s + x, s => s.State < 3, 0);
        Assert.True(res == 3, $"{typeof(F).Name} | FoldWhileStateTest failed");
    }

    void FoldWhileValueTest()
    {
        var res = Construct(Items).FoldWhile((s, x) => s + x, s => s.Value < 4, 0);
        Assert.True(res == 6, $"{typeof(F).Name} | FoldWhileValueTest failed");
    }

    void FoldBackWhileStateTest()
    {
        var res    = Construct(Items).FoldBackWhile((s, x) => s + x, s => s.State < EndItemsSum, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileStateTest failed");
    }

    void FoldBackWhileValueTest()
    {
        var res    = Construct(Items).FoldBackWhile((s, x) => s + x, s => s.Value >= EndItemCutOff, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileValueTest failed");
    }

    void FoldMaybeTest()
    {
        var res = Construct(Items).FoldMaybe((s, x) => x == 4 ? None : Some(s + x), 0);

        Assert.True(res == 6, $"{typeof(F).Name} | FoldMaybeTest failed");
    }

    void FoldBackMaybeTest()
    {
        var res = Construct(Items).FoldBackMaybe((s, x) => x < EndItemCutOff ? None : Some(s + x), 0);

        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackMaybeTest failed");
    }

    void FoldWhileMTest()
    {
        var res = Construct(Items).FoldWhileM((s, x) => Some(s + x), x => x.Value < 4, 0);

        Assert.True(res.As() == Some(6), $"{typeof(F).Name} | FoldWhileMTest failed");
    }

    void FoldBackWhileMTest()
    {
        var res    = Construct(Items).FoldBackWhileM((s, x) => Some(s + x), x => x.Value >= EndItemCutOff, 0);

        Assert.True(res.As() == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileTest failed");
    }

    void FoldUntilStateTest()
    {
        var res = Construct(Items).FoldUntil((s, x) => s + x, s => s.State == 3, 0);
        Assert.True(res == 3, $"{typeof(F).Name} | FoldUntilStateTest failed");
    }

    void FoldUntilValueTest()
    {
        var res = Construct(Items).FoldUntil((s, x) => s + x, s => s.Value == 4, 0);
        Assert.True(res == 6, $"{typeof(F).Name} | FoldUntilValueTest failed");
    }

    void FoldBackUntilStateTest()
    {
        var res = Construct(Items).FoldBackUntil((s, x) => s + x, s => s.State == EndItemsSum, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackUntilStateTest failed");
    }

    void FoldBackUntilValueTest()
    {
        var res    = Construct(Items).FoldBackUntil((s, x) => s + x, s => s.Value < EndItemCutOff, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackUntilValueTest failed");
    }

    void FoldUntilMTest()
    {
        var res = Construct(Items).FoldUntilM((s, x) => Some(s + x), x => x.Value == 4, 0);
        Assert.True(res.As() == Some(6), $"{typeof(F).Name} | FoldUntilMTest failed");
    }

    void FoldBackUntilMTest()
    {
        var res = Construct(Items).FoldBackUntilM((s, x) => Some(s + x), x => x.Value < EndItemCutOff, 0);

        Assert.True(res.As() == Some(EndItemsSum), $"{typeof(F).Name} | FoldBackUntilMTest failed");
    }

    void ToSeqTest()
    {
        var res = Construct(Items).ToSeq();
        Assert.True(res == toSeq(Items), $"{typeof(F).Name} | ToSeqTest failed");
    }

    void ToLstTest()
    {
        var res = Construct(Items).ToLst();
        Assert.True(res == toList(Items), $"{typeof(F).Name} | ToLstTest failed");
    }

    void ToArrTest()
    {
        var res = Construct(Items).ToArr();
        Assert.True(res == toArray(Items), $"{typeof(F).Name} | ToArrTest failed");
    }

    void ToIterableTest()
    {
        var res = Construct(Items).ToIterable();
        Assert.True(res == toIterable(Items), $"{typeof(F).Name} | ToIterableTest failed");
    }

    void IsEmptyFalseTest()
    {
        var res = Construct(Items).IsEmpty;
        Assert.False(res, $"{typeof(F).Name} | IsEmptyFalseTest failed");
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
        Assert.True(res == Iterable(98, 99), $"{typeof(F).Name} | FindAllTest failed");
    }

    void FindAllBackTest()
    {
        var res = Construct(Items).FindAllBack(x => x > 97);
        Assert.True(res == Iterable(99, 98), $"{typeof(F).Name} | FindAllBackTest failed");
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

    void HeadSomeTest()
    {
        var res = Construct(Items).Head;
        Assert.True(res == Some(0), $"{typeof(F).Name} | HeadSomeTest failed");
    }

    void LastSomeTest()
    {
        var res = Construct(Items).Last;
        Assert.True(res == Some(ItemsCount - 1), $"{typeof(F).Name} | LastSomeTest failed");
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

    void AtTest()
    {
        var foldable = Construct(Items);
        var r0       = foldable.At(0);
        var r1       = foldable.At(1);
        var r2       = foldable.At(2);
        var r3       = foldable.At(3);
        var r4       = foldable.At(4);
        var r95      = foldable.At(95);
        var r96      = foldable.At(96);
        var r97      = foldable.At(97);
        var r98      = foldable.At(98);
        var r99      = foldable.At(99);
        var x100     = foldable.At(100);
        
        
        Assert.True(r0   == Some(0), $"{typeof(F).Name} | AtTest r0 failed");
        Assert.True(r1   == Some(1), $"{typeof(F).Name} | AtTest r1 failed");
        Assert.True(r2   == Some(2), $"{typeof(F).Name} | AtTest r2 failed");
        Assert.True(r3   == Some(3), $"{typeof(F).Name} | AtTest r3 failed");
        Assert.True(r4   == Some(4), $"{typeof(F).Name} | AtTest r4 failed");
        Assert.True(r95  == Some(95), $"{typeof(F).Name} | AtTest r95 failed");
        Assert.True(r96  == Some(96), $"{typeof(F).Name} | AtTest r96 failed");
        Assert.True(r97  == Some(97), $"{typeof(F).Name} | AtTest r97 failed");
        Assert.True(r98  == Some(98), $"{typeof(F).Name} | AtTest r98 failed");
        Assert.True(r99  == Some(99), $"{typeof(F).Name} | AtTest r99 failed");
        Assert.True(x100 == None, $"{typeof(F).Name} | AtTest x100 failed");
    }

    void PartitionTest()
    {
        var res = Construct(Items).Partition(x => (x & 1) == 0);

        Assert.True(res.True  == toSeq(Range(0, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest True failed");
        Assert.True(res.False == toSeq(Range(1, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest False failed");
    }
}
