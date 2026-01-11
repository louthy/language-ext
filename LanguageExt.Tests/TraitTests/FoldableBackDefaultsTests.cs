using System;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableBackDefaultsTests
{
    [Fact]
    public static void FoldBackTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBack((s, x) => s + x, 0);
        Assert.True(res == 15);
    }
        
    [Fact]
    public static void FoldBackNoneMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackM((s, x) => x == 4 ? None : Some(s + x), 0);
        
        Assert.True(res.As() == None);
    }
        
    [Fact]
    public static void FoldBackSomeMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackM((s, x) => Some(s + x), 0);
        
        Assert.True(res.As() == Some(15));
    }
    
    [Fact]
    public static void FoldBackWhileStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackWhile((s, x) => s - x, s => s.State > 3, 15);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldBackWhileValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackWhile((s, x) => s + x, s => s.Value > 3, 0);
        Assert.True(res == 9);
    }
    
    [Fact]
    public static void FoldBackMaybeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackMaybe((s, x) => x == 2 ? None : Some(s + x), 0);
        
        Assert.True(res == 12);
    }

    [Fact]
    public static void FoldBackWhileMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackWhileM((s, x) => Some(s - x), x => x.Value > 3, 15);
        
        Assert.True(res.As() == Some(6));
    }
    
    [Fact]
    public static void FoldBackUntilStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackUntil((s, x) => s - x, s => s.State == 3, 15);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldBackUntilValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackUntil((s, x) => s + x, s => s.Value == 3, 0);
        Assert.True(res == 9);
    }
    
    [Fact]
    public static void FoldBackUntilMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackUntilM((s, x) => Some(s - x), x => x.Value == 3, 15);
        
        Assert.True(res.As() == Some(6));
    }

    [Fact]
    public static void ToSeqTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToSeqBack();
        Assert.True(res == Seq(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToLstTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToLstBack();
        Assert.True(res == Lst(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToArrTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToArrBack();
        Assert.True(res == Array(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToIterableTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToIterableBack();
        Assert.True(res == Iterable(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ExistsTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ExistsBack(x => x == 3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ExistsFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ExistsBack(x => x == 6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ExistsEmptyIsFalseTest()
    {
        var res = FList.New<int>().ExistsBack(x => x == 6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ForAllTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ForAllBack(x => x < 6);
        Assert.True(res);
    }
    
    [Fact]
    public static void ForAllFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ForAllBack(x => x < 3);
        Assert.False(res);
    }
    
    [Fact]
    public static void ForAllEmptyIsTrueTest()
    {
        var res = FList.New<int>().ForAllBack(x => x < 5);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ContainsBack(3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ContainsBack(6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ContainsEqTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ContainsBack<EqInt, FList, int>(3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsEqFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ContainsBack<EqInt, FList, int>(6);
        Assert.False(res);
    }
        
    [Fact]
    public static void FindBackTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FindBack(x => x == 3);
        Assert.True(res == Some(3));
    }
    
    [Fact]
    public static void FindBackFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FindBack(x => x == 6);
        Assert.True(res == None);
    }
        
    [Fact]
    public static void FindAllBackTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FindAllBack(x => x > 3);
        Assert.True(res == Iterable(5, 4));
    }
        
    [Fact]
    public static void LastSomeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Last;
        Assert.True(res == Some(5));
    }
        
    [Fact]
    public static void LastNoneTest()
    {
        var res = FList.New<int>().Last;
        Assert.True(res == None);
    }
        
    [Fact]
    public static void AtTest()
    {
        var foldable = FList.New(1, 2, 3, 4, 5);
        var r0       = foldable.AtBack(0);
        var r1       = foldable.AtBack(1);
        var r2       = foldable.AtBack(2);
        var r3       = foldable.AtBack(3);
        var r4       = foldable.AtBack(4);
        var x5       = foldable.AtBack(5);
        
        Assert.True(r0 == Some(5));
        Assert.True(r1 == Some(4));
        Assert.True(r2 == Some(3));
        Assert.True(r3 == Some(2));
        Assert.True(r4 == Some(1));
        Assert.True(x5 == None);
    }
        
    [Fact]
    public static void PartitionTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).PartitionBack(x => (x & 1) == 0);
        
        Assert.True(res.True == [2, 4]);
        Assert.True(res.False == [1, 3, 5]);
    }
}
