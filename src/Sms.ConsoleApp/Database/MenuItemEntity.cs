using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Database;

internal sealed class MenuItemEntity
{
    public string Id { get; private set; } = string.Empty;

    public string Article { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public bool IsWeighted { get; private set; }

    public string FullPath { get; private set; } = string.Empty;

    public string[] Barcodes { get; private set; } = [];

    public static MenuItemEntity From(MenuItem item)
    {
        var entity = new MenuItemEntity { Id = item.Id };
        entity.Update(item);

        return entity;
    }

    public void Update(MenuItem item)
    {
        Article = item.Article;
        Name = item.Name;
        Price = item.Price;
        IsWeighted = item.IsWeighted;
        FullPath = item.FullPath;
        Barcodes = item.Barcodes.ToArray();
    }
}