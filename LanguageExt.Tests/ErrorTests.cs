using System;
using System.Runtime.Serialization;
using Xunit;
using LanguageExt.Common;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public class ErrorTests
{
    [Fact]
    public void EqualityTest()
    {
        var ea = Error.New(100, "Hello");
        var eb = Error.New(100, "Hello");
        
        Assert.True(ea == eb);
    }
    
    [Fact]
    public void NonEqualityTest()
    {
        var ea = Error.New(100, "Hello");
        var eb = Error.New(100, "World");
        
        Assert.False(ea == eb);
    }
    
    [Fact]
    public void IsTest()
    {
        var ea = Error.New(100, "Hello");
        var eb = Error.New(100, "World");
        
        Assert.True(ea.Is(ea));
        Assert.True(eb.Is(eb));
        Assert.True(ea.Is(eb));
        Assert.True(eb.Is(ea));
    }
        
    [Fact]
    public void IsExpectedTest()
    {
        var ea = Error.New(100, "Hello");

        Assert.True(ea.IsExpected);
        Assert.True(
            ea switch
            {
                var ex when ex.IsExpected => true,    
                var ex => false    
            });
    }

    [Fact]
    public void ExpectedSerialisationTest()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
        var ea = Error.New(123, "Err");
        var json = JsonConvert.SerializeObject(ea, settings);
        var eb = JsonConvert.DeserializeObject<Error>(json, settings);
        Assert.True(eb.IsExpected);
        Assert.False(eb.IsExceptional);
        Assert.True(ea == eb);
        Assert.True(ea.Is(eb));
        Assert.True(eb.Is(ea));
    }
    
    [Fact]
    public void ExceptionalSerialisationTest()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
        var ea = Error.New(new Exception("Hello, World"));
        var json = JsonConvert.SerializeObject(ea, settings);
        var eb = JsonConvert.DeserializeObject<Error>(json, settings);
        Assert.False(eb.IsExpected);
        Assert.True(eb.IsExceptional);
        Assert.True(ea.Is(eb));
        Assert.True(eb.Is(ea));
    }

    public record BespokeError([property: DataMember] bool MyData) : 
        Expected("Expected something bespoke", 100, None); 
    
    [Fact]
    public void BespokeExpectedSerialisationTest()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
        var ea = new BespokeError(true);
        var json = JsonConvert.SerializeObject(ea, settings);
        var eb = JsonConvert.DeserializeObject<Error>(json, settings);
        Assert.True(eb.IsExpected);
        Assert.False(eb.IsExceptional);
        Assert.True(ea == eb);
        Assert.True(ea.Is(eb));
        Assert.True(eb.Is(ea));
    }
    
    
    [Fact]
    public void ManyExpectedSerialisationTest()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
        var ea = Error.New(123, "Err 1");
        var eb = Error.New(345, "Err 2");
        var ec = Error.New(678, "Err 3");
        
        var json = JsonConvert.SerializeObject(ea + eb + ec, settings);
        
        var es = JsonConvert.DeserializeObject<Error>(json, settings);
        
        Assert.False(es.IsEmpty);
        Assert.True(es.Count == 3);
        Assert.True(es.IsExpected);
        Assert.False(eb.IsExceptional);
        
        Assert.True(es == ea + eb + ec);
        Assert.True(es.Is(ea + eb + ec));
        Assert.True((ea + eb + ec).Is(es));
    }
}
