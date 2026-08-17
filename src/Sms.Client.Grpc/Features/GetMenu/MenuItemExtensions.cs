using Sms.Shared.Sms;

namespace Sms.Client.Grpc.Features.GetMenu;

internal static class MenuItemExtensions
{
    internal static MenuItem ToMenuItem(this Sms.Test.MenuItem item) =>
        new(
            item.Id,
            item.Article,
            item.Name,
            (decimal)item.Price,
            item.IsWeighted,
            item.FullPath,
            item.Barcodes.ToArray());
}
