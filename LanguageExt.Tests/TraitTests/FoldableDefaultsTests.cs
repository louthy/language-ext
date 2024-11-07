using System;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableDefaultsTests
{
    [Fact]
    public static void FoldTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Fold(0, (s, x) => s + x);
        Assert.True(res == 15);
    }
    
    [Fact]
    public static void FoldBackTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBack(0, (s, x) => s + x);
        Assert.True(res == 15);
    }
        
    [Fact]
    public static void FoldMNoneTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldM(0, (s, x) => x == 4 ? None : Some(s + x));
        
        Assert.True(res.As() == None);
    }
        
    [Fact]
    public static void FoldBackNoneMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackM(0, (s, x) => x == 4 ? None : Some(s + x));
        
        Assert.True(res.As() == None);
    }
        
    [Fact]
    public static void FoldMSomeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldM(0, (s, x) => Some(s + x));
        
        Assert.True(res.As() == Some(15));
    }
        
    [Fact]
    public static void FoldBackSomeMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackM(0, (s, x) => Some(s + x));
        
        Assert.True(res.As() == Some(15));
    }

    [Fact]
    public static void FoldWhileStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldWhile(0, (s, x) => s + x, s => s.State < 3);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldWhileValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldWhile(0, (s, x) => s + x, s => s.Value < 4);
        Assert.True(res == 6);
    }
    
    [Fact]
    public static void FoldBackWhileStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackWhile(15, (s, x) => s - x, s => s.State > 3);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldBackWhileValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackWhile(0, (s, x) => s + x, s => s.Value > 3);
        Assert.True(res == 9);
    }
    
    [Fact]
    public static void FoldMaybeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldMaybe(0, (s, x) => x == 4 ? None : Some(s + x));
        
        Assert.True(res == 6);
    }
    
    [Fact]
    public static void FoldBackMaybeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackMaybe(0, (s, x) => x == 2 ? None : Some(s + x));
        
        Assert.True(res == 12);
    }
    
    [Fact]
    public static void FoldWhileMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldWhileM(0, (s, x) => Some(s + x), x => x < 4);
        
        Assert.True(res.As() == Some(6));
    }
    
    [Fact]
    public static void FoldBackWhileMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackWhileM(15, (s, x) => Some(s - x), x => x > 3);
        
        Assert.True(res.As() == Some(6));
    }
    
    [Fact]
    public static void FoldUntilStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldUntil(0, (s, x) => s + x, s => s.State == 3);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldUntilValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldUntil(0, (s, x) => s + x, s => s.Value == 4);
        Assert.True(res == 6);
    }
    
    [Fact]
    public static void FoldBackUntilStateTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackUntil(15, (s, x) => s - x, s => s.State == 3);
        Assert.True(res == 3);
    }
    
    [Fact]
    public static void FoldBackUntilValueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FoldBackUntil(0, (s, x) => s + x, s => s.Value == 3);
        Assert.True(res == 9);
    }
    
    [Fact]
    public static void FoldUntilMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldUntilM(0, (s, x) => Some(s + x), x => x == 4);
        
        Assert.True(res.As() == Some(6));
    }
    
    [Fact]
    public static void FoldBackUntilMTest()
    {
        var res = FList.New(1, 2, 3, 4, 5)
                       .FoldBackUntilM(15, (s, x) => Some(s - x), x => x == 3);
        
        Assert.True(res.As() == Some(6));
    }

    [Fact]
    public static void ToSeqTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToSeq();
        Assert.True(res == Seq(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToLstTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToLst();
        Assert.True(res == List(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToArrTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToArr();
        Assert.True(res == Array(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void ToIterableTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ToIterable();
        Assert.True(res == Iterable(1, 2, 3, 4, 5));
    }

    [Fact]
    public static void IsEmptyFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).IsEmpty();
        Assert.False(res);
    }

    [Fact]
    public static void IsEmptyTrueTest()
    {
        var res = FList.New<int>().IsEmpty();
        Assert.True(res);
    }

    [Fact]
    public static void CountTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Count();
        Assert.True(res == 5);
    }
    
    [Fact]
    public static void ExistsTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Exists(x => x == 3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ExistsFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Exists(x => x == 6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ExistsEmptyIsFalseTest()
    {
        var res = FList.New<int>().Exists(x => x == 6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ForAllTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ForAll(x => x < 6);
        Assert.True(res);
    }
    
    [Fact]
    public static void ForAllFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).ForAll(x => x < 3);
        Assert.False(res);
    }
    
    [Fact]
    public static void ForAllEmptyIsTrueTest()
    {
        var res = FList.New<int>().ForAll(x => x < 5);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Contains(3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Contains(6);
        Assert.False(res);
    }
    
    [Fact]
    public static void ContainsEqTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Contains<EqInt, FList, int>(3);
        Assert.True(res);
    }
    
    [Fact]
    public static void ContainsEqFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Contains<EqInt, FList, int>(6);
        Assert.False(res);
    }
        
    [Fact]
    public static void FindTrueTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Find(x => x == 3);
        Assert.True(res == Some(3));
    }
    
    [Fact]
    public static void FindFalseTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Find(x => x == 6);
        Assert.True(res == None);
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
    public static void FindAllTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FindAll(x => x > 3);
        Assert.True(res == Seq(4, 5));
    }
        
    [Fact]
    public static void FindAllBackTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).FindAllBack(x => x > 3);
        Assert.True(res == Seq(5, 4));
    }
        
    [Fact]
    public static void SumTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Sum();
        Assert.True(res == 15);
    }
        
    [Fact]
    public static void ProductTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Product();
        Assert.True(res == 120);
    }
        
    [Fact]
    public static void HeadSomeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Head();
        Assert.True(res == Some(1));
    }
        
    [Fact]
    public static void HeadNoneTest()
    {
        var res = FList.New<int>().Head();
        Assert.True(res == None);
    }
        
    [Fact]
    public static void LastSomeTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Last();
        Assert.True(res == Some(5));
    }
        
    [Fact]
    public static void LastNoneTest()
    {
        var res = FList.New<int>().Last();
        Assert.True(res == None);
    }
            
    [Fact]
    public static void IterTest()
    {
        var atom = Atom(0);
        var comp = FList.New(1, 2, 3, 4, 5)
                        .Iter(x => atom.SwapIO(v => v + x));
        
        ignore(comp.Run());
        
        Assert.True(atom.Value == 15);
    }
        
    [Fact]
    public static void MinTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Min();
        Assert.True(res == Some(1));
    }
        
    [Fact]
    public static void MinOrdTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Min<OrdInt, FList, int>();
        Assert.True(res == 1);
    }
        
    [Fact]
    public static void MaxTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Max();
        Assert.True(res == Some(5));
    }
        
    [Fact]
    public static void MaxOrdTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Max<OrdInt, FList, int>();
        Assert.True(res == 5);
    }
        
    [Fact]
    public static void AverageTest()
    {
        var res = FList.New(1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0).Average();
        Assert.True(Math.Abs(res - 5.5) < 0.0000001);
    }
        
    [Fact]
    public static void AtTest()
    {
        var foldable = FList.New(1, 2, 3, 4, 5);
        var r0       = foldable.At(0);
        var r1       = foldable.At(1);
        var r2       = foldable.At(2);
        var r3       = foldable.At(3);
        var r4       = foldable.At(4);
        var x5       = foldable.At(5);
        
        Assert.True(r0 == Some(1));
        Assert.True(r1 == Some(2));
        Assert.True(r2 == Some(3));
        Assert.True(r3 == Some(4));
        Assert.True(r4 == Some(5));
        Assert.True(x5 == None);
    }
        
    [Fact]
    public static void PartitionTest()
    {
        var res = FList.New(1, 2, 3, 4, 5).Partition(x => (x & 1) == 0);
        
        Assert.True(res.True == Seq(2, 4));
        Assert.True(res.False == Seq(1, 3, 5));
    }
}

public record FList<A>(A[] Values) : K<FList, A>;

public static class FListExtensions
{
    public static FList<A> As<A>(this K<FList, A> self) =>
        (FList<A>)self;
}

public class FList : Foldable<FList>
{
    public static FList<A> New<A>(params A[] values) =>
        new (values);
    
    public static S FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state,
        K<FList, A> ta)
    {
        var values = ta.As().Values;
        for(var i = 0; i < values.Length; i++)
        {
            var v = values[i];
            if (!predicate((state, v)))
            {
                return state; 
            }
            state = f(v)(state);
        }
        return state;
    }

    public static S FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<FList, A> ta)
    {
        var values = ta.As().Values;
        for(var i = values.Length - 1; i >= 0; i--)
        {
            var v = values[i];
            if (!predicate((state, v)))
            {
                return state; 
            }
            state = f(state)(v);
        }
        return state;
    }
}
