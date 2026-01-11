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
        // ReSharper disable once PossibleMultipleEnumeration
        FoldableNonEmptyOrderedCollectionsTestSuite<IterableNE>.RunAll(xs => IterableNE.create(xs.Take(1).Single(), xs.Skip(1)));

    [Fact]
    public static void StckTests() =>
        FoldableOrderedCollectionsTestSuite<Stck>.RunAll(Stck.createRange);

    [Fact]
    public static void SeqTests() =>
        FoldableOrderedCollectionsTestSuite<Seq>.RunAll(xs => Seq.createRange(xs));

    [Fact]
    public static void LstTests() =>
        FoldableOrderedCollectionsTestSuite<Lst>.RunAll(xs => Lst.createRange(xs));

    [Fact]
    public static void SetTests() =>
        FoldableOrderedCollectionsTestSuite<Set>.RunAll(xs => Set.createRange(xs));

    [Fact]
    public static void MapTests() =>
        // ReSharper disable once PossibleMultipleEnumeration
        FoldableOrderedCollectionsTestSuite<Map<int>>.RunAll(xs => Map.createRange(xs.Zip(xs)));
    
    [Fact]
    public static void HashMapTests() =>
        // ReSharper disable once PossibleMultipleEnumeration
        FoldableUnorderedCollectionsTestSuite<HashMap<int>>.RunAll(xs => HashMap.createRange(xs.Zip(xs)));
    
    [Fact]
    public static void HashSetTests() =>
        FoldableUnorderedCollectionsTestSuite<HashSet>.RunAll(xs => HashSet.createRange(xs));
}
