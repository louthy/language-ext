using System;
using Xunit;

namespace LanguageExt.Tests.SeqTypes;

public class Seq_Lazy_Tests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void BeginIndexTests(int index)
    {
        Index ix    = index; 
        var   seq   = toSeq(Range(0, 100));
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void EndIndexTests(int index)
    {
        index++;
        var ix    = Index.FromEnd(index); 
        var seq   = toSeq(Range(0, 100));
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void PreConcatBeginIndexTests(int index)
    {
        Index ix  = index; 
        var   pre = toSeq(Range(0, 50));
        var   seq = pre + toSeq(Range(50, 50));
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void PreConcatEndIndexTests(int index)
    {
        index++;
        var ix  = Index.FromEnd(index); 
        var pre = toSeq(Range(0, 50));
        var seq = pre + toSeq(Range(50, 50));
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }    
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void PostConcatBeginIndexTests(int index)
    {
        Index ix   = index; 
        var   post = toSeq(Range(50, 50));
        var   seq  = toSeq(Range(0, 50)) + post;
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
    [InlineData(89)]
    [InlineData(99)]
    public void PostConcatEndIndexTests(int index)
    {
        index++;
        var ix   = Index.FromEnd(index); 
        var post = toSeq(Range(50, 50));
        var seq  = toSeq(Range(0, 50)) + post;
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }     
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void MemoisedBeginIndexTests(int index, int load)
    {
        Index ix    = index; 
        var   seq   = toSeq(Range(0, 100));
        Enum(seq, load);
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void MemoisedEndIndexTests(int index, int load)
    {
        index++;
        var ix    = Index.FromEnd(index); 
        var seq   = toSeq(Range(0, 100));
        Enum(seq, load);
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void PreConcatMemoisedBeginIndexTests(int index, int load)
    {
        Index ix  = index; 
        var   pre = toSeq(Range(0, 50));
        var   seq = pre + toSeq(Range(50, 50));
        Enum(seq, load);
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void PreConcatMemoisedEndIndexTests(int index, int load)
    {
        index++;
        var ix  = Index.FromEnd(index); 
        var pre = toSeq(Range(0, 50));
        var seq = pre + toSeq(Range(50, 50));
        Enum(seq, load);
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }    
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void PostConcatMemoisedBeginIndexTests(int index, int load)
    {
        Index ix   = index; 
        var   post = toSeq(Range(50, 50));
        var   seq  = toSeq(Range(0, 50)) + post;
        Enum(seq, load);
        var   value = seq[ix];
        Assert.True(value == index);
    }
    
    [Theory]
    [InlineData(0, 50)]
    [InlineData(1, 99)]
    [InlineData(2, 89)]
    [InlineData(3, 55)]
    [InlineData(5, 34)]
    [InlineData(8, 21)]
    [InlineData(13, 13)]
    [InlineData(21, 8)]
    [InlineData(34, 5)]
    [InlineData(55, 3)]
    [InlineData(89, 2)]
    [InlineData(99, 1)]
    public void PostConcatMemoisedEndIndexTests(int index, int load)
    {
        index++;
        var ix   = Index.FromEnd(index); 
        var post = toSeq(Range(50, 50));
        var seq  = toSeq(Range(0, 50)) + post;
        Enum(seq, load);
        var value = seq[ix];
        Assert.True(value == 100 - index);
    }    

    static void Enum(Seq<int> seq, int amount)
    {
        foreach(var x in seq)
        {
            amount--;
            if (amount == 0) return;
        }
    }
}
