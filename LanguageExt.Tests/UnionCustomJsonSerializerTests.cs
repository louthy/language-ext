using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LanguageExt.Tests
{
    [Union]
    [MyUnion]
    public abstract partial class UnionTestClass
    {
        public abstract UnionTestClass UnionA();
        public abstract UnionTestClass UnionB(int i);
        public abstract UnionTestClass UnionC(int i);
        public abstract UnionTestClass UnionD(int i, int j);
    }
        
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MyUnionAttribute : Attribute
    {
    }
        
    public partial class UnionCustomJsonSerializerTests
    {
        public class UnionJsonReadConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string typeName = null;
                JObject typeValueToken = null;
                if (reader.TokenType != JsonToken.StartObject) throw new Exception();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndObject) break;
                    if (reader.TokenType != JsonToken.PropertyName) throw new Exception();
                    if ((string) reader.Value == "Type")
                    {
                        if (!reader.Read()) throw new Exception();
                        if (typeName != null) throw new Exception();
                        if (reader.TokenType != JsonToken.String) throw new Exception();
                        typeName = (string) reader.Value;
                    }
                    else if ((string) reader.Value == "Value")
                    {
                        if (typeValueToken != null) throw new Exception();
                        if (!reader.Read()) throw new Exception();
                        if (reader.TokenType != JsonToken.StartObject) throw new Exception();
                        typeValueToken = JObject.Load(reader);
                    }
                }

                var type = objectType.Assembly.GetTypes().Where(_ => objectType.IsAssignableFrom(_) && _.Name == typeName).Single();
                var result = typeValueToken.ToObject(type, serializer);
                return result;
            }

            public override bool CanWrite => false;
            public override bool CanConvert(Type objectType) => objectType.IsClass && objectType.GetCustomAttribute<MyUnionAttribute>() != null;
        }
         
        public class UnionJsonWriteConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var t = value.GetType();
                writer.WriteStartObject();
                writer.WritePropertyName("Type");
                writer.WriteValue(t.Name);
                writer.WritePropertyName("Value");
                writer.WriteStartObject();
                foreach (var fieldInfo in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    writer.WritePropertyName(fieldInfo.Name);
                    serializer.Serialize(writer, fieldInfo.GetValue(value));
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
                new NotImplementedException();

            public override bool CanRead => false;

            public override bool CanConvert(Type objectType) => objectType.IsClass && objectType.BaseType?.BaseType?.GetCustomAttribute<MyUnionAttribute>() != null;
        }
        
        [Fact(Skip = "No feature written that supports this yet")]
        public void UnionFromJson()
        {
            var json = @"{""Type"":""B"",""Value"":{""I"":42}}";
            var x = JsonConvert.DeserializeObject<UnionTestClass>(json, new UnionJsonReadConverter()) as UnionB;
            Assert.Equal(42, x?.I);
        }
        
        [Fact(Skip = "No feature written that supports this yet")]
        public void UnionToJson()
        {
            var x = JsonConvert.SerializeObject(UnionTestClassCon.UnionB(42), new UnionJsonWriteConverter());
            Assert.Equal(@"{""Type"":""B"",""Value"":{""I"":42}}", x);
        }
    }
}
