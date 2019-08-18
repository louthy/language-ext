using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LanguageExt.Tests
{
    public class WithTests
    {
        // Test that the generated With() only supplies valid properties to the Record constructor.
        [Fact]
        public void WithIsGeneratedForValidProperties()
        {
            var p1 = new PropertyFields("");
            var p2 = p1.With(Prop1: "p");
            Assert.True(p2.Prop1 == "p");
        }
    }

    [With]
    public partial class PropertyFields : Record<PropertyFields>
    {
        public string Prop1 { get; }  // Valid record field
        public string Prop2 { get; set; } // Invalid - set accessor not allowed
        public string Prop3 { get => ""; } // Invalid - Expression body not allowed
        public string Prop4 { get { return ""; } } // Invalid - Body not allowed
        public string Prop5 { get; } = ""; // Invalid - Initializer not allowed

        // Prop1 is the only valid record field that we'll need to cater for in our constructor.
        public PropertyFields(string prop1)
        {
            Prop1 = prop1;
        }
    }

}
