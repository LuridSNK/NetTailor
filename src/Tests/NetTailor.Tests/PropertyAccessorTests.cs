using FluentAssertions;
using NetTailor.Extensions;

namespace NetTailor.Tests;

public class PropertyAccessorTests
{
    [Fact]
    void PropertyAccessorFactory_WhenGivenAnAnonymousObject_ShouldReturnExpectedValues_BothTimes_EvenWhenObj2_IsCastToObject()
    {
        IEnumerable<(string, object)> expected1 = new[]
        {
            ("Id", (object)333),
            ("Name", "SomeNameOfAnonObject1"),
            ("IsTrue", true)
        };
        
        IEnumerable<(string, object)> expected2 = new[]
        {
            ("OtherId", (object)222),
            ("OtherName", "SomeNameOfAnonObject2"),
            ("IsFalse", false)
        };
        
        var obj1 = new {Id = 333, Name = "SomeNameOfAnonObject1", IsTrue = true};
        object obj2 = new {OtherId = 222, OtherName = "SomeNameOfAnonObject2", IsFalse = false};
        var accessors1 = PropertyAccessorFactory.GetPropertyAccessors(obj1);
        var accessors2 = PropertyAccessorFactory.GetPropertyAccessors(obj2);
        IEnumerable<(string, object)> results1 = accessors1.Select(accessor => (accessor.PropertyName, accessor.Getter(obj1))).ToArray();
        IEnumerable<(string, object)> results2 = accessors2.Select(accessor => (accessor.PropertyName, accessor.Getter(obj2))).ToArray();
        
        results1.Should().BeEquivalentTo(expected1);
        results2.Should().BeEquivalentTo(expected2);
    }
    
    [Fact]
    void PropertyAccessorFactory_WhenGivenANullPropertyValue_ShouldReturnAsExpected()
    {
        #pragma warning disable CS8619
        IEnumerable<(string, object)> expected = new[]
        {
            ("Id", (object)333),
            ("Name", null),
            ("IsTrue", true)
        };
        #pragma warning restore CS8619
        
        #pragma warning disable CS8600
        var obj = new
        {
            Id = 333,
            Name = (string)null,
            IsTrue = true
        };
        #pragma warning restore CS8600

        var accessors = PropertyAccessorFactory.GetPropertyAccessors(obj);
        IEnumerable<(string, object)> results = accessors.Select(accessor => (accessor.PropertyName, accessor.Getter(obj)));
        results.Should().BeEquivalentTo(expected);
    }
}