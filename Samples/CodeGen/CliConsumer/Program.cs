using System;
using LanguageExt;

namespace CliConsumer
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine(new Person("asd", "asd") == new Person("asd", "asd"));
            Console.WriteLine(GetArea(new Rectangle(10, 20)));
            Console.ReadKey();
        }

        static double GetArea(Shape shape)
            => shape switch
            {
                Rectangle rec => rec.Length * rec.Width,
                Circle circle => 2 * Math.PI * circle.Radius,
                _ => throw new NotImplementedException()
            };
    }

    [Union]
    public interface Shape
    {
        Shape Rectangle(float width, float length);
        Shape Circle(float radius);
        Shape Prism(float width, float height);
    }

    [Record]
    public partial struct Person
    {
        public readonly string Forename;
        public readonly string Surname;
    }
}
