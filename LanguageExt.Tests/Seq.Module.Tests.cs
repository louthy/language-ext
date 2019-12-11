using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class SeqModuleTests
    {
        [Fact]
        public void EmptyTest()
        {
            Assert.True(Seq.empty<int>().IsEmpty);
        }

        [Fact]
        public void CreateTest()
        {
            Assert.True(Seq.create<int>().IsEmpty);

            var seq = Seq.create(1, 2, 3, 4, 5);

            Assert.True(seq.Head == 1);
            Assert.True(seq.Tail.Head == 2);
            Assert.True(seq.Tail.Tail.Head == 3);
            Assert.True(seq.Tail.Tail.Tail.Head == 4);
            Assert.True(seq.Tail.Tail.Tail.Tail.Head == 5);

            var seqr = Seq.createRange(new[] { 1, 2, 3, 4, 5 });

            Assert.True(seqr.Head == 1);
            Assert.True(seqr.Tail.Head == 2);
            Assert.True(seqr.Tail.Tail.Head == 3);
            Assert.True(seqr.Tail.Tail.Tail.Head == 4);
            Assert.True(seqr.Tail.Tail.Tail.Tail.Head == 5);
        }

        [Fact]
        public void InitTest()
        {
            int counter = 0;
            Func<int, int> run = x => { counter++; return x; };

            var seq = Seq.generate(5, x => run((x + 1) * 2));

            var fst = seq.Take(1).Head;

            Assert.True(counter == 1);
            Assert.True(fst == 2);

            var snd = seq.Skip(1).Take(1).Head;

            Assert.True(counter == 2);
            Assert.True(snd == 4);

            var thd = seq.Skip(2).Take(1).Head;

            Assert.True(counter == 3);
            Assert.True(thd == 6);

            var fth = seq.Skip(3).Take(1).Head;

            Assert.True(counter == 4);
            Assert.True(fth == 8);

            var fit = seq.Skip(4).Take(1).Head;

            Assert.True(counter == 5);
            Assert.True(fit == 10);

            var sum = seq.Sum();

            Assert.True(counter == 5);
            Assert.True(sum == 30);
        }

        [Fact]
        public void EquivalentOfTheInitTestWithIEnumerable()
        {
            int counter = 0;
            Func<int, int> run = x => { counter++; return x; };

            var seq = List.generate(5, x => run((x + 1) * 2));

            var fst = seq.Take(1).First();

            Assert.True(counter == 1);
            Assert.True(fst == 2);

            var snd = seq.Skip(1).Take(1).First();

            // Assert.True(counter == 2);  equals 3 by this point
            Assert.True(snd == 4);

            var thd = seq.Skip(2).Take(1).First();

            // Assert.True(counter == 3);   equals 6 by this point
            Assert.True(thd == 6);

            var fth = seq.Skip(3).Take(1).First();

            // Assert.True(counter == 4);   equals 10 by this point (double what the InitTest needs!)
            Assert.True(fth == 8);

            var fit = seq.Skip(4).Take(1).First();

            //Assert.True(counter == 5);    equals 15 by this point (treble what the InitTest needs!)
            Assert.True(fit == 10);

            var sum = seq.Sum();

            // Assert.True(counter == 5);   equals 20 by this point(four times what the InitTest needs!!!)
            Assert.True(sum == 30);
        }

        [Fact]
        public void TailsTestIterative()
        {
            var seq = Seq(1, 2, 3, 4, 5);

            var seqs = Seq.tails(seq);

            Assert.True(seqs.Head == Seq(1, 2, 3, 4, 5));
            Assert.True(seqs.Tail.Head == Seq(2, 3, 4, 5));
            Assert.True(seqs.Tail.Tail.Head == Seq(3, 4, 5));
            Assert.True(seqs.Tail.Tail.Tail.Head == Seq(4, 5));
            Assert.True(seqs.Tail.Tail.Tail.Tail.Head == Seq.create(5));
            Assert.True(seqs.Tail.Tail.Tail.Tail.Tail.IsEmpty);
        }

        [Fact]
        public void TailsTestRecursive()
        {
            var seq = Seq(1, 2, 3, 4, 5);

            var seqs = Seq.tailsr(seq);

            Assert.True(seqs.Head == Seq(1, 2, 3, 4, 5));
            Assert.True(seqs.Tail.Head == Seq(2, 3, 4, 5));
            Assert.True(seqs.Tail.Tail.Head == Seq(3, 4, 5));
            Assert.True(seqs.Tail.Tail.Tail.Head == Seq(4, 5));
            Assert.True(seqs.Tail.Tail.Tail.Tail.Head == Seq.create(5));
            Assert.True(seqs.Tail.Tail.Tail.Tail.Tail.IsEmpty);
        }

        [Fact]
        public void CountTests()
        {
            int counter = 0;
            Func<int, int> run = x => { counter++; return x; };

            var seq1 = Seq.generate(5, x => run((x + 1) * 2));
            var seq2 = 1.Cons(seq1);

            var cnt1 = seq1.Count;
            var cnt2 = seq2.Count;

            Assert.True(counter == 5);
            Assert.True(cnt1 == 5);
            Assert.True(cnt2 == 6);

            var seq3 = Seq.generate(5, x => run((x + 1) * 2));
            var seq4 = 1.Cons(seq3);

            var cnt3 = seq4.Count;
            var cnt4 = seq3.Count;

            Assert.True(counter == 10);
            Assert.True(cnt4 == 5);
            Assert.True(cnt3 == 6);
        }

        /// <summary>
        /// Runs 1000 tasks that sums the same lazy Seq to
        /// make sure we get the same result as a synchronous
        /// sum.
        /// </summary>
        [Fact]
        public void ParallelTests()
        {
            var sum = Range(1, 10000).Sum();

            var seq = Seq(Range(1, 10000));

            var tasks = new List<Task<int>>();
            foreach(var x in Range(1, 1000))
            {
                tasks.Add(Task.Run(() => seq.Sum()));
            }

            Task.WaitAll(tasks.ToArray());

            var results = tasks.Map(t => t.Result).ToArray();

            seq.Iter((i, x) => Assert.True(x != 0, $"Invalid value in the sequence at index {i}"));

            foreach (var task in tasks)
            {
                Assert.True(task.Result == sum, $"Result is {task.Result}, should be: {sum}");
            }
        }
    }
}
