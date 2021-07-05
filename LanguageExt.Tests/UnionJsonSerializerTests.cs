using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace LanguageExt.Tests
{
    [Union]
    public abstract partial class LightControl
    {
        public abstract LightControl OnOff(bool enabled);
        public abstract LightControl Dimmer(int value);
    }

    public partial class UnionJsonSerializerTests
    { 
        [Fact]
        public void UnionInstanceFromJson()
        {
            var json = @"{""Value"":100,""Tag"":2}";
            var x = JsonConvert.DeserializeObject<Dimmer>(json);
            Assert.Equal(100, x.Value);
        }
        
        [Fact(Skip="not yet supported")]
        public void UnionInstanceToJson()
        {
            var json = JsonConvert.SerializeObject(LightControlCon.Dimmer(100));
            Assert.Equal(@"{""Value"":100,""Tag"":2}", json);
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
