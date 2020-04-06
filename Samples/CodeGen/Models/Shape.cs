using System;
using LanguageExt;

namespace Models
{
    [Union]
    public interface Shape
    {
        Shape Rectangle(float width, float length);
        Shape Circle(float radius);
        Shape Prism(float width, float height);
    }

    public class UseShapeLibrary
    {
        public double GetArea(Shape shape)
            => shape switch
            {
                Rectangle rec => rec.Length * rec.Width,
                Circle circle => 2 * Math.PI * circle.Radius,
                _ => throw new NotImplementedException()
            };
    }
}
