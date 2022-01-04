using static LanguageExt.Prelude;
using Xunit;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class AtomHashMapEqTests
    {
        [Fact]
        public void SwapInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Swap(m => m.Add("biz", 99));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SwapKeyInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SwapKey("foo", i => i + 1);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SwapKeyOptionalInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SwapKey("foo", i => None);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FilterInPlaceInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FilterInPlace(i => i % 2 == 0);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FilterInPlaceWithKeyInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FilterInPlace((k, i) => k[0] == 'b' && i % 2 == 0);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void MapInPlaceInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.MapInPlace(i => i * 3);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Add("biz", 7);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void TryAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TryAdd("biz", 7);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AddOrUpdateInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddOrUpdate("biz", 7);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AddRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void TryAddRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TryAddRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AddOrUpdateRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddOrUpdateRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void RemoveInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Remove("bar");

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void RemoveRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.RemoveRange(Seq("bar", "biz"));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FindOrAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrAdd("biz", () => 7);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FindOrAddConstantInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrAdd("biz", 7);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FindOrMaybeAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrMaybeAdd("biz", () => Some(7));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void FindOrMaybeAddConstantInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrMaybeAdd("biz", Some(7));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SetItemsInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SetItems(Seq(("foo", 80), ("bar", 17)));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void TrySetItemsInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItems(Seq(("foo", 80), ("bar", 17)));

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SetItemFuncInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SetItem("foo", i => i * 2);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void TrySetItemInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItem("foo", 80);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void TrySetItemFuncInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            HashMap<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItem("foo", i => i * 2);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void ClearInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Clear();

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AppendInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toAppend = HashMap(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Append(toAppend);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void AppendAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toAppend = AtomHashMap<TString, string, int>(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Append(toAppend);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SubtractInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toSubtract = HashMap<TString, string, int>(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Subtract(toSubtract);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SubtractAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toSubtract = HashMap<TString, string, int>(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Subtract(toSubtract);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void IntersectInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toIntersect = HashMap(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Intersect(toIntersect);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void IntersectAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toIntersect = HashMap(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Intersect(toIntersect);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void ExceptInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toExcept = HashMap(("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Except(toExcept);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void ExceptKeysInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toExcept = Seq("biz", "baz");
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Except(toExcept);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void SymmetricExceptInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toExcept = AtomHashMap<TString, string, int>(("foo", 3), ("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.SymmetricExcept(toExcept);

            Assert.Equal(hashMap.ToHashMap(), state);
        }

        [Fact]
        public void UnionInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toUnion = AtomHashMap<TString, string, int>(("foo", 3), ("biz", 7), ("baz", 9));
            var state = hashMap.ToHashMap();
            hashMap.Change += v => state = v;

            hashMap.Union(toUnion);

            Assert.Equal(hashMap.ToHashMap(), state);
        }
    }
}
