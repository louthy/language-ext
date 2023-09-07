using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class PrismTests
    {
        private static readonly Prism<Job, int> jobWorkerCarMileage = prism(Job.worker.ToPrism(), Worker.car.ToPrism(), Car.mileage.ToPrism());

        [Fact]
        public void PrismShouldGetSome()
        {
            var expected = Some(20000);
            
            var car = new Car("Maserati", "Ghibli", 20000);
            var worker = new Worker("Joe Bloggs", 50000, car);
            var job = new Job("Programmer", "Write code and tests.", worker);

            var actual = jobWorkerCarMileage.Get(job);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PrismShouldGetNone()
        {
            var expected = Option<int>.None;
            
            var worker = new Worker("Joe Bloggs", 50000, Option<Car>.None);
            var job = new Job("Programmer", "Write code and tests.", worker);

            var actual = jobWorkerCarMileage.Get(job);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PrismShouldSetWhenOptionalPartOfChainIsSome()
        {
            var expectedCar = new Car("Maserati", "Ghibli", 25000);
            var expectedWorker = new Worker("Joe Bloggs", 50000, expectedCar);
            var expected = new Job("Programmer", "Write code and tests.", expectedWorker);
            
            var car = new Car("Maserati", "Ghibli", 20000);
            var worker = new Worker("Joe Bloggs", 50000, car);
            var job = new Job("Programmer", "Write code and tests.", worker);
            
            var actual = jobWorkerCarMileage.Set(25000, job);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PrismShouldNotSetWhenOptionalPartOfChainIsNone()
        {
            var expectedWorker = new Worker("Joe Bloggs", 50000, Option<Car>.None);
            var expected = new Job("Programmer", "Write code and tests.", expectedWorker);

            var worker = new Worker("Joe Bloggs", 50000, Option<Car>.None);
            var job = new Job("Programmer", "Write code and tests.", worker);

            var actual = jobWorkerCarMileage.Set(25000, job);

            Assert.Equal(expected, actual);
        }

    }
    
    [Record]
    public partial class Worker
    {
        public readonly string Name;
        public readonly int Salary;
        public readonly Option<Car> Car;
    }

    [Record]
    public partial class Job
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Worker Worker;
    }
}

