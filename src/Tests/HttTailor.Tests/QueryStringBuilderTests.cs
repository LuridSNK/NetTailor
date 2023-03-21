using System.Text;
using FluentAssertions;
using HttTailor.Extensions;

namespace HttTailor.Tests;

public class QueryStringBuilderTests
{
    private readonly StringBuilder _stringBuilder = new(); 
    record SomeRequest
    {
        public SomeRequest(string value, int number, string[]? items = null)
        {
            Value = value;
            Number = number;
            Items = items;
        }

        public string Value { get; set; }
        public int Number  { get; set; }
        public string[]? Items { get; set; } = null;
    }
    
    [Fact]
    public void QueryStringBuilder_WhenGivenNullNamingPolicy_ShouldReturnCorrectUpperCamelCaseQueryString()
    {
        var r = new SomeRequest(
            "oneTwoTree", 
            321);
        
        var s = QueryStringBuilder.Build(_stringBuilder, new
        {
            StringValue = r.Value,
            NumericValue = r.Number
        }, null);
        
        _stringBuilder.Clear();
        
        Assert.Equal(
            expected: "?StringValue=oneTwoTree&NumericValue=321",
            actual: s);
    }
    
    [Fact]
    public void QueryStringBuilder_WhenGivenNullString_ShouldReturnEmptyQueryString()
    {
        var s = QueryStringBuilder.Build(_stringBuilder, null, null);
        _stringBuilder.Clear();
        Assert.Equal(
            expected: s, 
            actual: string.Empty);
    }
    
    [Fact]
    public void QueryStringBuilder_WhenGivenObjectWithArray_AndLowerSnakeCaseNamingPolicy_ShouldReturnCorrectLowerSnakeCaseQueryString()
    {
        var r = new SomeRequest(
            "oneTwoTree", 
            321123, 
            new[] { "item1", "item2", "item3" });
        
        var result = QueryStringBuilder.Build(_stringBuilder, new
        {
            StringValue = r.Value,
            NumericValue = r.Number,
            Collection = r.Items
        }, NamingPolicies.LowerSnakeCase);

        _stringBuilder.Clear();
        
        result.Should()
            .Be("?string_value=oneTwoTree&numeric_value=321123&collection=item1&collection=item2&collection=item3");
    }
    
    [Fact]
    public void QueryStringBuilder_WhenGivenAnIConvertibleValue_ShouldThrowInvalidOperationException()
    {
        FluentActions.Invoking(() => QueryStringBuilder.Build(_stringBuilder, "string"))
            .Should()
            .Throw<InvalidOperationException>();
        
        FluentActions.Invoking(() => QueryStringBuilder.Build(_stringBuilder, 123))
            .Should()
            .Throw<InvalidOperationException>();
        
        FluentActions.Invoking(() => QueryStringBuilder.Build(_stringBuilder, 'x'))
            .Should()
            .Throw<InvalidOperationException>();
    }
}
