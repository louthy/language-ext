using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableBackOrderedCollectionsTestSuite<F>
    where F : FoldableBack<F>
{
    const int ItemsCount = 100;
    readonly int[] Items;
    readonly int[] ItemsR;
    readonly int ItemsSum;
    readonly int EndItemsSum;
    readonly int EndItemCutOff;
    readonly Func<IEnumerable<int>, K<F, int>> Construct;

    FoldableBackOrderedCollectionsTestSuite(Func<IEnumerable<int>, K<F, int>> construct)
    {
        Items = new int[ItemsCount];
        var total = 0;
        for (var i = 0; i < ItemsCount; i++)
        {
            total += i;
            Items[i] = i;
        }

        var start    = ItemsCount - 1;
        var end      = start      - 10;
        var endTotal = 0;
        for (var i = ItemsCount - 1; i >= end; i--)
        {
            endTotal += i;
        }

        ItemsSum = total;
        EndItemCutOff = end;
        EndItemsSum = endTotal;
        Construct = construct;
        ItemsR = Items.AsIterable().Reverse().ToArray();
    }

    public static void RunAll(Func<IEnumerable<int>, K<F, int>> construct)
    {
        var suite = new FoldableBackOrderedCollectionsTestSuite<F>(construct);
        suite.FoldBackTest();
        suite.FoldBackNoneMTest();
        suite.FoldBackSomeMTest();
        suite.FoldBackWhileStateTest();
        suite.FoldBackWhileValueTest();
        suite.FoldBackMaybeTest();
        suite.FoldBackWhileMTest();
        suite.FoldBackUntilStateTest();
        suite.FoldBackUntilValueTest();
        suite.FoldBackUntilMTest();
        suite.PartitionTest();
        suite.LastNoneTest();
        suite.LastSomeTest();
        suite.FindBackFalseTest();
        suite.FindBackTrueTest();
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
        suite.ToIterableTest();
        suite.ToArrTest();
        suite.ToLstTest();
        suite.ToSeqTest();
    }

    void FoldBackTest()
    {
        var res = Construct(ItemsR).FoldBack((s, x) => s + x, 0);
        Assert.True(res == ItemsSum, $"{typeof(F).Name} | FoldBackTest failed");
    }

    void FoldBackNoneMTest()
    {
        var res = Construct(ItemsR).FoldBackM((s, x) => x == 4 ? None : Some(s + x), 0);
        Assert.True(res.As() == None, $"{typeof(F).Name} | FoldBackNoneMTest failed");
    }

    void FoldBackSomeMTest()
    {
        var res = Construct(ItemsR).FoldBackM((s, x) => Some(s + x), 0);
        Assert.True(res.As() == Some(ItemsSum), $"{typeof(F).Name} | FoldBackSomeMTest failed");
    }

    void FoldBackWhileStateTest()
    {
        var res    = Construct(ItemsR).FoldBackWhile((s, x) => s + x, s => s.State < EndItemsSum, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileStateTest failed");
    }

    void FoldBackWhileValueTest()
    {
        var res    = Construct(ItemsR).FoldBackWhile((s, x) => s + x, s => s.Value >= EndItemCutOff, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileValueTest failed");
    }

    void FoldBackMaybeTest()
    {
        var res = Construct(ItemsR).FoldBackMaybe((s, x) => x < EndItemCutOff ? None : Some(s + x), 0);

        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackMaybeTest failed");
    }

    void FoldBackWhileMTest()
    {
        var res    = Construct(ItemsR).FoldBackWhileM((s, x) => Some(s + x), x => x.Value >= EndItemCutOff, 0);

        Assert.True(res.As() == EndItemsSum, $"{typeof(F).Name} | FoldBackWhileTest failed");
    }

    void FoldBackUntilStateTest()
    {
        var res = Construct(ItemsR).FoldBackUntil((s, x) => s + x, s => s.State == EndItemsSum, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackUntilStateTest failed");
    }

    void FoldBackUntilValueTest()
    {
        var res    = Construct(ItemsR).FoldBackUntil((s, x) => s + x, s => s.Value < EndItemCutOff, 0);
        Assert.True(res == EndItemsSum, $"{typeof(F).Name} | FoldBackUntilValueTest failed");
    }

    void FoldBackUntilMTest()
    {
        var res = Construct(ItemsR).FoldBackUntilM((s, x) => Some(s + x), x => x.Value < EndItemCutOff, 0);

        Assert.True(res.As() == Some(EndItemsSum), $"{typeof(F).Name} | FoldBackUntilMTest failed");
    }

    void ToSeqTest()
    {
        var res = Construct(ItemsR).ToSeqBack();
        Assert.True(res == toSeq(Items), $"{typeof(F).Name} | ToSeqTest failed");
    }

    void ToLstTest()
    {
        var res = Construct(ItemsR).ToLstBack();
        Assert.True(res == toLst(Items), $"{typeof(F).Name} | ToLstTest failed");
    }

    void ToArrTest()
    {
        var res = Construct(ItemsR).ToArrBack();
        Assert.True(res == toArr(Items), $"{typeof(F).Name} | ToArrTest failed");
    }

    void ToIterableTest()
    {
        var res = Construct(ItemsR).ToIterableBack();
        Assert.True(res == toIterable(Items), $"{typeof(F).Name} | ToIterableTest failed");
    }

    void ExistsTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(ItemsR).ExistsBack(x => x == expect);
        Assert.True(res, $"{typeof(F).Name} | ExistsTrueTest failed");
    }

    void ExistsFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(ItemsR).ExistsBack(x => x == notExpect);
        Assert.False(res, $"{typeof(F).Name} | ExistsFalseTest failed");
    }

    void ExistsEmptyIsFalseTest()
    {
        var res = Construct([]).ExistsBack(x => x == 6);
        Assert.False(res, $"{typeof(F).Name} | ExistsEmptyIsFalseTest failed");
    }

    void ForAllTrueTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(ItemsR).ForAllBack(x => x < notExpect);
        Assert.True(res, $"{typeof(F).Name} | ForAllTrueTest failed");
    }

    void ForAllFalseTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(ItemsR).ForAllBack(x => x < expect);
        Assert.False(res, $"{typeof(F).Name} | ForAllFalseTest failed");
    }

    void ForAllEmptyIsTrueTest()
    {
        var res = Construct([]).ForAllBack(x => x < 5);
        Assert.True(res, $"{typeof(F).Name} | ForAllEmptyIsTrueTest failed");
    }

    void ContainsTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(ItemsR).ContainsBack(expect);
        Assert.True(res, $"{typeof(F).Name} | ContainsTrueTest failed");
    }

    void ContainsFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res = Construct(ItemsR).ContainsBack(notExpect);
        Assert.False(res, $"{typeof(F).Name} | ContainsFalseTest failed");
    }

    void ContainsEqTrueTest()
    {
        var expect = ItemsCount / 2;
        var res = Construct(ItemsR).ContainsBack<EqInt, F, int>(expect);
        Assert.True(res, $"{typeof(F).Name} | ContainsEqTrueTest failed");
    }

    void ContainsEqFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res = Construct(ItemsR).ContainsBack<EqInt, F, int>(notExpect);
        Assert.False(res, $"{typeof(F).Name} | ContainsEqFalseTest failed");
    }

    void FindBackTrueTest()
    {
        var expect = ItemsCount / 2;
        var res    = Construct(ItemsR).FindBack(x => x == expect);
        Assert.True(res == Some(expect), $"{typeof(F).Name} | FindBackTrueTest failed");
    }

    void FindBackFalseTest()
    {
        var notExpect = ItemsCount * 2;
        var res       = Construct(ItemsR).FindBack(x => x == notExpect);
        Assert.True(res == None, $"{typeof(F).Name} | FindBackFalseTest failed");
    }

    void LastSomeTest()
    {
        var res = Construct(ItemsR).Last;
        Assert.True(res == Some(ItemsCount - 1), $"{typeof(F).Name} | LastSomeTest failed");
    }

    void LastNoneTest()
    {
        var res = Construct([]).Last;
        Assert.True(res == None, $"{typeof(F).Name} | LastNoneTest failed");
    }

    void PartitionTest()
    {
        var res = Construct(ItemsR).PartitionBack(x => (x & 1) == 0);

        Assert.True(res.True  == toArr(Range(0, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest True failed");
        Assert.True(res.False == toArr(Range(1, ItemsCount / 2, 2)), $"{typeof(F).Name} | PartitionTest False failed");
    }
}
