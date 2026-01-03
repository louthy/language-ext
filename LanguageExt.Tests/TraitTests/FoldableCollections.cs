using System.Linq;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class FoldableCollections
{
    [Fact]
    public static void ArrTests() =>
        FoldableOrderedCollectionsTestSuite<Arr>.RunAll(xs => Arr.createRange(xs));

    [Fact]
    public static void IterableTests() =>
        FoldableOrderedCollectionsTestSuite<Iterable>.RunAll(Iterable.createRange);

    [Fact]
    public static void IterableNETests() =>
        FoldableNonEmptyOrderedCollectionsTestSuite<IterableNE>.RunAll(xs => (IterableNE<int>)IterableNE.createRange(xs));

    [Fact]
    public static void SeqTests() =>
        FoldableOrderedCollectionsTestSuite<Seq>.RunAll(xs => Seq.createRange(xs));

    [Fact]
    public static void LstTests() =>
        FoldableOrderedCollectionsTestSuite<Seq>.RunAll(xs => Seq.createRange(xs));

    [Fact]
    public static void SetTests() =>
        FoldableOrderedCollectionsTestSuite<Set>.RunAll(xs => Set.createRange(xs));

    [Fact]
    public static void MapTests() =>
        FoldableOrderedCollectionsTestSuite<Map<int>>.RunAll(xs => Map.createRange(xs.Zip(xs)));
    
    [Fact]
    public static void HashMapTests() =>
        FoldableUnorderedCollectionsTestSuite<HashMap<int>>.RunAll(xs => HashMap.createRange(xs.Zip(xs)));
    
    [Fact]
    public static void HashSetTests() =>
        FoldableUnorderedCollectionsTestSuite<HashSet>.RunAll(xs => HashSet.createRange(xs));
}
