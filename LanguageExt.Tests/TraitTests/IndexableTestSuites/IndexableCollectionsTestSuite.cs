using System;
using System.Collections.Generic;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class IndexableOrderedCollectionsTestSuite<F>
    where F : Indexable<F, Index>
{
    const int ItemsCount = 100;
    readonly int[] Items;
    readonly int ItemsSum;
    readonly int ItemsProduct;
    readonly Func<IEnumerable<int>, K<F, int>> Construct;

    IndexableOrderedCollectionsTestSuite(Func<IEnumerable<int>, K<F, int>> construct)
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
        var suite = new IndexableOrderedCollectionsTestSuite<F>(construct);
        suite.AtTest();
    }

    void AtTest()
    {
        var indexable = Construct(Items);
        var r0        = indexable.At((Index)0);
        var r1        = indexable.At((Index)1);
        var r2        = indexable.At((Index)2);
        var r3        = indexable.At((Index)3);
        var r4        = indexable.At((Index)4);
        var r95       = indexable.At((Index)95);
        var r96       = indexable.At((Index)96);
        var r97       = indexable.At((Index)97);
        var r98       = indexable.At((Index)98);
        var r99       = indexable.At((Index)99);
        var x100      = indexable.At((Index)100);
        
        
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
}
