using LanguageExt;
using LanguageExt.TypeClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Records
{
    public static class JObjectTestEq
    {

        public static void Test()
        {
            JObject a = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Hello", B = "World" }));
            JObject b = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Hello", B = "World" }));
            JObject c = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { A = "Different", B = "World" }));

            var i = new TestEqJObject(a);
            var j = new TestEqJObject(b);
            var k = new TestEqJObject(c);

            var x = i == j;
            var y = i == k;
            var z = j == k;

            var h1 = i.GetHashCode();
            var h2 = j.GetHashCode();
            var h3 = k.GetHashCode();

            var l = i > j;
            var m = i < j;

        }
    }

    public class TestEqJObject : Record<TestEqJObject>
    {
        public readonly JObject Value;

        public TestEqJObject(JObject value) =>
            Value = value;
    }

    public struct EqJObject : Eq<JObject>
    {
        public bool Equals(JObject x, JObject y) =>
            JObject.DeepEquals(x, y);

        public int GetHashCode(JObject x) =>
            x.GetHashCode();

        public Task<bool> EqualsAsync(JObject x, JObject y) =>
            Equals(x, y).AsTask();

        public Task<int> GetHashCodeAsync(JObject x) =>
            GetHashCode(x).AsTask();
    }

    public struct OrdJObject : Ord<JObject>
    {
        public int Compare(JObject x, JObject y) =>
            x.GetHashCode().CompareTo(y.GetHashCode());

        public bool Equals(JObject x, JObject y) =>
            JObject.DeepEquals(x, y);

        public int GetHashCode(JObject x) =>
            x.GetHashCode();

        public Task<bool> EqualsAsync(JObject x, JObject y) =>
            Equals(x, y).AsTask();

        public Task<int> GetHashCodeAsync(JObject x) =>
            GetHashCode(x).AsTask();

        public Task<int> CompareAsync(JObject x, JObject y) =>
            Compare(x, y).AsTask();    
    }
}
