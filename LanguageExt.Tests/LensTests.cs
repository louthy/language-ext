using System;
using LanguageExt;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class LensTests
    {
        [Fact]
        public void LensMutate()
        {
            var car = new Car("Maserati", "Ghibli", 20000);
            var editor = new Editor("Joe Bloggs", 50000, car);
            var book = new Book("Editors of the World", "Sarah Bloggs", editor);

            var bookEditorCarMileage = lens(Book.editor, Editor.car, Car.mileage);
            var mileage = bookEditorCarMileage.Get(book);
            var book2 = bookEditorCarMileage.Set(25000, book);

            Assert.True(book2.Editor.Car.Mileage == 25000);
        }

        [Fact]
        public void CollectionsMutate()
        {
            var person = new Person("Paul", "Louth", Map(
                (1, new Appt(1, DateTime.Parse("1/1/2010"), ApptState.NotArrived)),
                (2, new Appt(2, DateTime.Parse("2/1/2010"), ApptState.NotArrived)),
                (3, new Appt(3, DateTime.Parse("3/1/2010"), ApptState.NotArrived))));

            Lens<Person, ApptState> arrive(int id) => 
                lens(Person.appts, Map<int, Appt>.item(id), Appt.state);

            var person2 = arrive(2).Set(ApptState.Arrived, person);

            Assert.True(person2.Appts[2].State == ApptState.Arrived);
        }
    }

    [WithLens]
    public partial class Person : Record<Person>
    {
        public readonly string Name;
        public readonly string Surname;
        public readonly Map<int, Appt> Appts;

        public Person(string name, string surname, Map<int, Appt> appts)
        {
            Name = name;
            Surname = surname;
            Appts = appts;
        }
    }

    [WithLens]
    public partial class Appt : Record<Appt>
    {
        public readonly int Id;
        public readonly DateTime StartDate;
        public readonly ApptState State;

        public Appt(int id, DateTime startDate, ApptState state)
        {
            Id = id;
            StartDate = startDate;
            State = state;
        }
    }

    public enum ApptState
    {
        NotArrived,
        Arrived,
        DNA,
        Cancelled
    }

    [WithLens]
    public partial class Car : Record<Car>
    {
        public readonly string Make;
        public readonly string Model;
        public readonly int Mileage;

        public Car(string make, string model, int mileage)
        {
            Make = make;
            Model = model;
            Mileage = mileage;
        }
    }

    [WithLens]
    public partial class Editor : Record<Editor>
    {
        public readonly string Name;
        public readonly int Salary;
        public readonly Car Car;

        public Editor(string name, int salary, Car car)
        {
            Name = name;
            Salary = salary;
            Car = car;
        }
    }

    [WithLens]
    public partial class Book : Record<Book>
    {
        public readonly string Name;
        public readonly string Author;
        public readonly Editor Editor;

        public Book(string name, string author, Editor editor)
        {
            Name = name;
            Author = author;
            Editor = editor;
        }
    }
}

