using Xunit;
using System.Linq;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests;

public class OptionTTests
{
    [Fact]
    public void TestOptionT2()
    {
        var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

        var newlist = list.KindT<Lst, Option, Option<int>, int>()
                          .BindT(x => x % 2 == 0 ? Some(x) : None)
                          .AsT<Lst, Option, Option<int>, int>().As();

        Assert.True(newlist == List(None, Some(2), None, Some(4), None));
    }

    [Fact]
    public void TestOptionT3()
    {
        var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

        var result = list.KindT<Lst, Option, Option<int>, int>()
                         .SumT();

        Assert.True(result == 15);
    }

    [Fact]
    public void TestOptionT4()
    {
        var list = List(Some(1), Some(2), Some(3), Some(4), Some(5));

        var result = list.KindT<Lst, Option, Option<int>, int>()
                         .CountT();

        Assert.True(result == 5);
    }

    [Fact]
    public void WrappedListTest()
    {
        var opt  = Some(List(1, 2, 3, 4, 5));
        var res  = opt.KindT<Option, Lst, Lst<int>, int>().FoldT(0, (s, v) => s + v);
        var mopt = opt.KindT<Option, Lst, Lst<int>, int>().MapT(x => x * 2);
        var mres = mopt.FoldT(0, (s, v) => s + v);

        Assert.True(res  == 15, "Expected 15, but got " + res);
        Assert.True(mres == 30, "Expected 30, but got " + mres);
        Assert.True(opt.KindT<Option, Lst, Lst<int>, int>().CountT() == 5,
                    "opt != 5, (" + opt.KindT<Option, Lst, Lst<int>, int>().CountT() + ")");
        Assert.True(mopt.CountT() == 5, "mopt != 5, (" + mopt.CountT() + ")");

        opt = None;
        res = opt.KindT<Option, Lst, Lst<int>, int>().FoldT(0, (s, v) => s + v);

        Assert.True(res == 0, "res != 0, got " + res);
        Assert.True(opt.KindT<Option, Lst, Lst<int>, int>().CountT() == 0,
                    "opt.Count() != 0, got " + opt.KindT<Option, Lst, Lst<int>, int>().CountT());
    }

    //[Fact]
    //public void WrappedMapTest()
    //{
    //    var opt = Some(Map(Tuple(1, "A"), Tuple(2, "B"), Tuple(3, "C"), Tuple(4, "D"), Tuple(5, "E")));
    //    var res = opt.FoldT("", (s, v) => s + v);
    //    var mopt = opt.MapT(x => x.ToLower());
    //    var mres = mopt.FoldT("", (s, v) => s + v);

    //    Assert.True(res == "ABCDE");
    //    Assert.True(opt.CountT() == 5);
    //    Assert.True(mopt.CountT() == 5);

    //    match(mopt,
    //        Some: x =>
    //        {
    //            Assert.True(x[1] == "a");
    //            Assert.True(x[2] == "b");
    //            Assert.True(x[3] == "c");
    //            Assert.True(x[4] == "d");
    //            Assert.True(x[5] == "e");
    //        },
    //        None: () => Assert.False(true)
    //    );
    //}

    [Fact]
    public void WrappedListLinqTest()
    {
        var opt = Some(List(1, 2, 3, 4, 5));

        var res = from x in opt.ToIterable()
                  from y in x
                  select y * 2;

        var total = res.Sum();

        Assert.True(total == 30);
    }

    [Fact]
    public void WrappedMapLinqTest()
    {
        var opt = Some(Map((1, "A"), (2, "B"), (3, "C"), (4, "D"), (5, "E")));

        var res = from x in opt.ToIterable()
                  from y in x.AsIterable()
                  select y.Value.ToLower();

        var fd = res.AsIterable().Fold("", (s, x) => s + x);

        Assert.True(fd == "abcde");
    }

    [Fact]
    public void WrappedOptionOptionLinqTest()
    {
        var opt = Some(Some(Some(100)));

        var res = from x in opt
                  from y in x
                  from z in y
                  select z * 2;

        Assert.True(equals<EqInt, int>(res, Some(200)));

        opt = Some(Some<Option<int>>(None));

        var res2 = from x in opt
                   from y in x
                   from z in y
                   select z * 2;

        Assert.False(equals<EqInt, int>(res, Some(0)));
    }

    [Fact]
    public void WrappedOptionLinqTest()
    {
        var opt = Some(Some(100));

        var res = from x in opt
                  from y in x
                  select y * 2;

        Assert.True(res.Map(x => x == 200).IfNone(false));

        opt = Some<Option<int>>(None);

        res = from x in opt
              from y in x
              select y * 2;

        var bopt = res.Map(x => x == 1);

        Assert.True(bopt.IfNone(true));
    }

    //[Fact]
    //public void WrappedEitherLinqTest()
    //{
    //    var opt = Some(Right<string, int>(100));

    //    var res = from x in opt
    //              from y in x
    //              select y * 2;

    //    Assert.True(res.LiftT() == 200);

    //    opt = Some(Left<string, int>("left"));

    //    res = from x in opt
    //          from y in x
    //          select y * 2;

    //    Assert.True(res.LiftT() == 0);
    //}


    [Fact]
    public void WrappedListOfOptionsTest2()
    {
        var opt = List(Some(1), Some(2), Some(3), Some(4), Some(5));

        opt = opt.KindT<Lst, Option, Option<int>, int>()
                 .FoldT(Lst<Option<int>>.Empty, (xs, x) => x > 2 ? xs.Add(x) : xs);
              // .FilterT(x => x > 2);   <-- TODO: Above was this -- should we restore a filter operation? Can we even?

        Assert.True(opt.Count() == 3, "Count should be 3, is: "                   + opt.Count());
        Assert.True(equals<TInt, int>(opt[0], Some(3)), "opt[0] != Some(3), Is: " + opt[0]);
        Assert.True(equals<TInt, int>(opt[1], Some(4)), "opt[1] != Some(4), Is: " + opt[1]);
        Assert.True(equals<TInt, int>(opt[2], Some(5)), "opt[2] != Some(5), Is: " + opt[2]);
    }
}
