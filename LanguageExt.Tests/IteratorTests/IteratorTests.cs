using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExt.Tests.IteratorTests;

public class IteratorTests
{
    [Fact]
    public void Iterator_BasicBehavior_Test()
    {
        // Arrange
        var numbers = new List<int> { 1, 2, 3, 4 };
        var iterator = numbers.GetIterator();

        // Act and Assert
        Assert.False(iterator.IsEmpty);                             // Iterator should not be empty
        Assert.Equal(1, iterator.Head);                             // Verify the first element (Head)
        Assert.Equal(2, iterator.Tail.Head);                        // Navigate to the Tail and check the next Head
        Assert.Equal(3, iterator.Tail.Tail.Head);                   // Keep checking subsequent elements
        Assert.Equal(4, iterator.Tail.Tail.Tail.Head);
        Assert.True(iterator.Tail.Tail.Tail.Tail.IsEmpty);          // Check if the iterator ends correctly
    }

    [Fact]
    public async Task Iterator_ThreadSafety_Test()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 1000).ToList();
        var iterator = numbers.GetIterator();

        // Act
        var tasks = Enumerable.Range(0, 5).Select(
            _ =>
                Task.Run(() =>
                         {
                             var current = iterator;
                             while (!current.IsEmpty)
                             {
                                 ignore(current.Head);   // Access the head
                                 current = current.Tail; // Access the tail
                             }
                         })).ToArray();

        // Assert
        await Task.WhenAll(tasks); // Ensure all tasks complete without throwing exceptions
    }

    [Fact]
    public void Iterator_Nil_Test()
    {
        // Arrange
        var iterator = Iterator<int>.Nil.Default;

        // Act and Assert
        Assert.True(iterator.IsEmpty);                              // Nil should be empty
        Assert.Throws<InvalidOperationException>(() => iterator.Head); // Accessing the Head should throw
        Assert.Same(iterator, iterator.Tail);                       // The Tail of Nil should be itself
    }

    [Fact]
    public void Iterator_MultipleEnumerationPrevention_Test()
    {
        // Arrange
        var enumerator = "Lazy Evaluation".GetIterator();

        // Act
        var head1 = enumerator.Head;
        var tail1 = enumerator.Tail; // Forces evaluation

        // Attempt to re-access the same enumerator
        var head2 = enumerator.Head; // Should be the same value, from cache

        // Assert
        Assert.Equal(head1, head2); // Items should not be re-enumerated
    }

    [Fact]
    public void Iterator_EmptyEnumerable_Test()
    {
        // Arrange
        var iterator = Enumerable.Empty<int>().GetIterator();

        // Act and Assert
        Assert.True(iterator.IsEmpty);                              // Empty iterator should be empty
        Assert.Throws<InvalidOperationException>(() => iterator.Head);
        Assert.Equal(iterator, iterator.Tail);                       // Tail of an empty iterator should be itself
    }

    [Fact]
    public void Iterator_ThreadSafety_Test_WithContentCheck()
    {
        // Arrange
        var threads        = 5;
        var numbers        = Enumerable.Range(1, 1000).ToList();
        var iterator       = numbers.GetIterator();
        var resultingLists = new List<List<int>>();

        for (var i = 0; i < threads; i++)
        {
            resultingLists.Add(new List<int>());
        }

        // Act
        Parallel.ForEach(Enumerable.Range(0, 5), ix =>
        {
            var current = iterator;
            while (!current.IsEmpty)
            {
                lock (resultingLists[ix])
                {
                    resultingLists[ix].Add(current.Head); // Safely add the emitted value into the list
                }

                current = current.Tail;
            }
        });

        // Assert
        // The final list length should equal the source list's length (sequentially added values)
        foreach (var resultList in resultingLists)
        {
            Assert.True(Enumerable.SequenceEqual(numbers, resultList));
        }
    }
}
