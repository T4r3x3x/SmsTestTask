using System.Globalization;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Features.Orders;

public static class OrderParser
{
    public static bool TryParse(
        string? input,
        IReadOnlyDictionary<string, MenuItem> menu,
        out IReadOnlyCollection<OrderItem> items,
        out string? error,
        CultureInfo? culture = null)
    {
        var result = new List<OrderItem>();
        var codes = new HashSet<string>(StringComparer.Ordinal);
        culture ??= CultureInfo.CurrentCulture;

        foreach (var segment in (input ?? string.Empty)
                     .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = segment.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
            {
                return Fail("Используйте формат Код:Количество.", out items, out error);
            }

            var code = parts[0];
            if (!menu.ContainsKey(code))
            {
                return Fail($"Блюдо с кодом '{code}' не найдено.", out items, out error);
            }

            if (!codes.Add(code))
            {
                return Fail($"Код '{code}' указан несколько раз.", out items, out error);
            }

            if (!TryParseQuantity(parts[1], culture, out var quantity) || quantity <= 0)
            {
                return Fail($"Количество для кода '{code}' должно быть больше нуля.", out items, out error);
            }

            result.Add(new OrderItem(code, quantity));
        }

        if (result.Count == 0)
        {
            return Fail("Добавьте хотя бы одну позицию.", out items, out error);
        }

        items = result;
        error = null;
        return true;
    }

    private static bool TryParseQuantity(string value, CultureInfo culture, out decimal quantity) =>
        decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out quantity) ||
        decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, culture, out quantity);

    private static bool Fail(
        string message,
        out IReadOnlyCollection<OrderItem> items,
        out string? error)
    {
        items = [];
        error = message;
        return false;
    }
}
