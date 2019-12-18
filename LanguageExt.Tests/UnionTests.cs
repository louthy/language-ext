using Newtonsoft.Json;
using Xunit;

namespace LanguageExt.Tests
{
    public partial class UnionTests
    {
        [Union]
        public abstract partial class LightControl
        {
            public abstract LightControl OnOff(bool enabled);
            public abstract LightControl Dimmer(int value);
        }

        [Fact]
        public void FromJson()
        {
            var json = @"{""Value"": 100}";
            var x = JsonConvert.DeserializeObject<Dimmer>(json);
            Assert.Equal(100, x.Value);
        }
    }
}
