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
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Swap(m => m.Add("biz", 99));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(("biz", Change<int>.Added(99))), state.Changes);
        }

        [Fact]
        public void SwapKeyInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SwapKey("foo", i => i + 1);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(("foo", Change<int>.Mapped(3, 4))),
                state.Changes);
        }

        [Fact]
        public void SwapKeyOptionalInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SwapKey("foo", i => None);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(("foo", Change<int>.Removed(3))),
                state.Changes);
        }

        [Fact]
        public void FilterInPlaceInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FilterInPlace(i => i % 2 == 0);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Removed(7))),
                state.Changes);
        }

        [Fact]
        public void FilterInPlaceWithKeyInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FilterInPlace((k, i) => k[0] == 'b' && i % 2 == 0);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Removed(7))),
                state.Changes);
        }

        [Fact]
        public void MapInPlaceInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.MapInPlace(i => i * 3);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 9)),
                    ("bar", Change<int>.Mapped(42, 126)),
                    ("biz", Change<int>.Mapped(7, 21))),
                state.Changes);
        }

        [Fact]
        public void AddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Add("biz", 7);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void TryAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TryAdd("biz", 7);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void AddOrUpdateInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddOrUpdate("biz", 7);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void AddRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void TryAddRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TryAddRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void AddOrUpdateRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.AddOrUpdateRange(Seq(("biz", 7), ("baz", 9)));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void RemoveInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Remove("bar");

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("bar", Change<int>.Removed(42))),
                state.Changes);
        }

        [Fact]
        public void RemoveRangeInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.RemoveRange(Seq("bar", "biz"));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("bar", Change<int>.Removed(42)),
                    ("biz", Change<int>.Removed(7))),
                state.Changes);
        }

        [Fact]
        public void FindOrAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrAdd("biz", () => 7);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void FindOrAddConstantInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrAdd("biz", 7);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void FindOrMaybeAddInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrMaybeAdd("biz", () => Some(7));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void FindOrMaybeAddConstantInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.FindOrMaybeAdd("biz", Some(7));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7))),
                state.Changes);
        }

        [Fact]
        public void SetItemsInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SetItems(Seq(("foo", 80), ("bar", 17)));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 80)),
                    ("bar", Change<int>.Mapped(42, 17))),
                state.Changes);
        }

        [Fact]
        public void TrySetItemsInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItems(Seq(("foo", 80), ("bar", 17), ("biz", 33)));

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 80)),
                    ("bar", Change<int>.Mapped(42, 17))),
                state.Changes);
        }

        [Fact]
        public void SetItemFuncInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SetItem("foo", i => i * 2);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 6))),
                state.Changes);
        }

        [Fact]
        public void TrySetItemInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItem("foo", 80);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 80))),
                state.Changes);
        }

        [Fact]
        public void TrySetItemFuncInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.TrySetItem("foo", i => i * 2);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 6))),
                state.Changes);
        }

        [Fact]
        public void ClearInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Clear();

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("bar", Change<int>.Removed(42))),
                state.Changes);
        }

        [Fact]
        public void AppendInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var toAppend = HashMap<TString, string, int>(("foo", 7), ("biz", 7), ("baz", 9));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Append(toAppend);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(initialValue.Append(toAppend), hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void AppendAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toAppend = AtomHashMap<TString, string, int>(("foo", 7), ("biz", 7), ("baz", 9));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Append(toAppend);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Append(toAppend.ToHashMap()),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void SubtractInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toSubtract = HashMap<TString, string, int>(
                ("foo", 7), ("biz", 7), ("baz", 9));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Subtract(toSubtract);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Subtract(toSubtract),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Removed(7)),
                    ("baz", Change<int>.Removed(9))),
                state.Changes);
        }

        [Fact]
        public void SubtractAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toSubtract = AtomHashMap<TString, string, int>(
                ("foo", 7), ("biz", 7), ("baz", 9));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Subtract(toSubtract);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Subtract(toSubtract.ToHashMap()),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Removed(7)),
                    ("baz", Change<int>.Removed(9))),
                state.Changes);
        }

        [Fact]
        public void IntersectInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toIntersect = HashMap(
                ("foo", 7), ("biz", 7), ("baz", 9), ("bin", 0));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Intersect(toIntersect);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Intersect(toIntersect),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("bar", Change<int>.Removed(42))),
                state.Changes);
        }

        [Fact]
        public void IntersectAtomInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toIntersect = AtomHashMap<TString, string, int>(
                ("foo", 7), ("biz", 7), ("baz", 9), ("bin", 0));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Intersect(toIntersect);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Intersect(toIntersect.ToHashMap()),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("bar", Change<int>.Removed(42))),
                state.Changes);
        }

        [Fact]
        public void ExceptInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toExcept = HashMap(
                ("foo", 7), ("biz", 7), ("baz", 9), ("bin", 0));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Except(toExcept);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Except(toExcept),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Removed(7)),
                    ("baz", Change<int>.Removed(9))),
                state.Changes);
        }

        [Fact]
        public void ExceptKeysInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42), ("biz", 7), ("baz", 9));
            var toExcept = Seq("biz", "baz");
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Except(toExcept);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Except(toExcept),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Removed(7)),
                    ("baz", Change<int>.Removed(9))),
                state.Changes);
        }

        [Fact]
        public void SymmetricExceptInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(
                ("foo", 3), ("bar", 42));
            var toExcept = AtomHashMap<TString, string, int>(
                ("foo", 3), ("biz", 7), ("baz", 9));
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.SymmetricExcept(toExcept);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.SymmetricExcept(toExcept),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Removed(3)),
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }

        [Fact]
        public void UnionInvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toUnion = AtomHashMap<TString, string, int>(("foo", 7), ("biz", 7), ("baz", 9));
            
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Union(toUnion);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Union(toUnion),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }
        
        [Fact]
        public void Union_TakeRight_InvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toUnion = AtomHashMap<TString, string, int>(("foo", 7), ("biz", 7), ("baz", 9));
            
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Union(toUnion, Merge: (_, _, r) => r);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Union(toUnion, Merge: (_, _, r) => r),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("foo", Change<int>.Mapped(3, 7)),
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }
        
        [Fact]
        public void Union_TakeLeft_InvokesChange()
        {
            var hashMap = AtomHashMap<TString, string, int>(("foo", 3), ("bar", 42));
            var toUnion = AtomHashMap<TString, string, int>(("foo", 7), ("biz", 7), ("baz", 9));
            
            var initialValue = hashMap.ToHashMap();
            HashMapPatch<TString, string, int> state = default;
            hashMap.Change += v => state = v;

            hashMap.Union(toUnion, Merge: (_, l, _) => l);

            Assert.Equal(initialValue, state.From);
            Assert.Equal(hashMap.ToHashMap(), state.To);
            Assert.Equal(
                initialValue.Union(toUnion, Merge: (_, l, _) => l),
                hashMap.ToHashMap());
            Assert.Equal(
                HashMap(
                    ("biz", Change<int>.Added(7)),
                    ("baz", Change<int>.Added(9))),
                state.Changes);
        }
    }
}
