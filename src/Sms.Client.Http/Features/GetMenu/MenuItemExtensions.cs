using Sms.Client.Http.Client;
using Sms.Shared.Sms;

namespace Sms.Client.Http.Features.GetMenu;

internal static class MenuItemExtensions
{
    internal static MenuItem ToMenuItem(this MenuItemDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            throw new SmsProtocolException("Menu item does not contain Id.");
        }

        return new MenuItem(
            item.Id,
            item.Article ?? string.Empty,
            item.Name ?? string.Empty,
            item.Price,
            item.IsWeighted,
            item.FullPath ?? string.Empty,
            item.Barcodes ?? []);
    }
}
