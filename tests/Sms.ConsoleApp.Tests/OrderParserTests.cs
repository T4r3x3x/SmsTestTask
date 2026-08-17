using System.Globalization;
using Sms.ConsoleApp.Features.Orders;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Tests;

public sealed class OrderParserTests
{
    private static readonly IReadOnlyDictionary<string, MenuItem> Menu =
        new[]
        {
            new MenuItem("1", "A1", "Первое", 100, false, "Блюда", []),
            new MenuItem("2", "A2", "Второе", 200, true, "Блюда", [])
        }.ToDictionary(item => item.Id);

    [Fact]
    public void TryParse_ParsesWholeAndFractionalQuantities()
    {
        var success = OrderParser.TryParse(
            "1:1;2:0.408",
            Menu,
            out var items,
            out var error,
            CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal(1m, items.ElementAt(0).Quantity);
        Assert.Equal(0.408m, items.ElementAt(1).Quantity);
    }

    [Fact]
    public void TryParse_AcceptsCurrentCultureDecimalSeparator()
    {
        var success = OrderParser.TryParse(
            "2:0,408",
            Menu,
            out var items,
            out _,
            CultureInfo.GetCultureInfo("ru-RU"));

        Assert.True(success);
        Assert.Equal(0.408m, Assert.Single(items).Quantity);
    }

    [Fact]
    public void TryParse_RejectsUnknownCode()
    {
        var success = OrderParser.TryParse("3:1", Menu, out var items, out var error);

        Assert.False(success);
        Assert.Empty(items);
        Assert.Contains("не найдено", error);
    }

    [Theory]
    [InlineData("1:0")]
    [InlineData("1:-1")]
    [InlineData("1:text")]
    public void TryParse_RejectsInvalidQuantity(string input)
    {
        var success = OrderParser.TryParse(input, Menu, out _, out var error);

        Assert.False(success);
        Assert.Contains("больше нуля", error);
    }

    [Fact]
    public void TryParse_RejectsDuplicateCode()
    {
        var success = OrderParser.TryParse("1:1;1:2", Menu, out _, out var error);

        Assert.False(success);
        Assert.Contains("несколько раз", error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(";")]
    [InlineData("1")]
    public void TryParse_RejectsEmptyOrMalformedInput(string input)
    {
        var success = OrderParser.TryParse(input, Menu, out _, out var error);

        Assert.False(success);
        Assert.False(string.IsNullOrEmpty(error));
    }
}
