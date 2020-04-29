using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace LanguageExt.Tests
{
    public partial class UnionJsonSerializerTests
    { 
        [Union]
        public abstract partial class LightControl
        {
            public abstract LightControl OnOff(bool enabled);
            public abstract LightControl Dimmer(int value);
        }

        [Fact]
        public void UnionInstanceFromJson()
        {
            var json = @"{""Value"":100}";
            var x = JsonConvert.DeserializeObject<Dimmer>(json);
            Assert.Equal(100, x.Value);
        }
        
        [Fact]
        public void UnionInstanceToJson()
        {
            Assert.Equal(@"{""Value"":100}", JsonConvert.SerializeObject(LightControlCon.Dimmer(100)));
        }        
        
        [Fact]
        public void UnionRoundTrip()
        {
            Assert.Equal(LightControlCon.OnOff(true), JsonConvert.DeserializeObject<OnOff>(JsonConvert.SerializeObject(LightControlCon.OnOff(true))));
            Assert.Equal(LightControlCon.OnOff(false), JsonConvert.DeserializeObject<OnOff>(JsonConvert.SerializeObject(LightControlCon.OnOff(false))));
            Assert.Equal(LightControlCon.Dimmer(10), JsonConvert.DeserializeObject<Dimmer>(JsonConvert.SerializeObject(LightControlCon.Dimmer(10))));
            Assert.Equal(LightControlCon.Dimmer(90), JsonConvert.DeserializeObject<Dimmer>(JsonConvert.SerializeObject(LightControlCon.Dimmer(90))));
            
        }
    }
}
