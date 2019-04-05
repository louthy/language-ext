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

    public class Person : Record<Person>
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

        public Person With(string Name = null, string Surname = null, Map<int, Appt>? Appts = null) =>
            new Person(Name ?? this.Name, Surname ?? this.Surname, Appts ?? this.Appts);

        public static readonly Lens<Person, string> name = Lens<Person, string>.New(
            Get: a => a.Name,
            Set: a => s => s.With(Name: a));

        public static readonly Lens<Person, string> surname = Lens<Person, string>.New(
            Get: a => a.Surname,
            Set: a => s => s.With(Surname: a));

        public static readonly Lens<Person, Map<int, Appt>> appts = Lens<Person, Map<int, Appt>>.New(
            Get: a => a.Appts,
            Set: a => s => s.With(Appts: a));
    }

    public class Appt : Record<Appt>
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

        public Appt With(int? Id = null, DateTime? StartDate = null, ApptState? State = null) =>
            new Appt(Id ?? this.Id, StartDate ?? this.StartDate, State ?? this.State);

        public static readonly Lens<Appt, ApptState> state = Lens<Appt, ApptState>.New(
            Get: a => a.State,
            Set: a => s => s.With(State: a));

        public static readonly Lens<Appt, DateTime> startDate = Lens<Appt, DateTime>.New(
            Get: a => a.StartDate,
            Set: a => s => s.With(StartDate: a));

        public static readonly Lens<Appt, int> id = Lens<Appt, int>.New(
            Get: a => a.Id,
            Set: a => s => s.With(Id: a));
    }

    public enum ApptState
    {
        NotArrived,
        Arrived,
        DNA,
        Cancelled
    }

    public class Car : Record<Car>
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

        public Car With(string Make = null, string Model = null, int? Mileage = null) =>
            new Car(Make ?? this.Make, Model ?? this.Model, Mileage ?? this.Mileage);

        public static readonly Lens<Car, int> mileage =
            Lens<Car, int>.New(
                Get: car => car.Mileage,
                Set: v => car => car.With(Mileage: v));
    }

    public class Editor : Record<Editor>
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

        public Editor With(string Name = null, int? Salary = null, Car Car = null) =>
            new Editor(Name ?? this.Name, Salary ?? this.Salary, Car ?? this.Car);

        public static readonly Lens<Editor, Car> car =
            Lens<Editor, Car>.New(
                Get: editor => editor.Car,
                Set: v => editor => editor.With(Car: v));

        public static readonly Lens<Editor, int> salary =
            Lens<Editor, int>.New(
                Get: editor => editor.Salary,
                Set: v => editor => editor.With(Salary: v));
    }

    public class Book : Record<Book>
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

        public Book With(string Name = null, string Author = null, Editor Editor = null) =>
            new Book(Name ?? this.Name, Author ?? this.Author, Editor ?? this.Editor);

        public static readonly Lens<Book, Editor> editor =
            Lens<Book, Editor>.New(
                Get: book => book.Editor,
                Set: v => book => book.With(Editor: v));
    }
}

