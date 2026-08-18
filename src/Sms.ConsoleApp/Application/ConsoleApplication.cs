using Sms.ConsoleApp.Database;
using Sms.ConsoleApp.Features.Orders;
using Sms.ConsoleApp.Output;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Application;

public sealed class ConsoleApplication(
    ISmsClient client,
    IMenuRepository repository,
    IConsoleOutput output)
{
    public async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        output.Log("Инициализация базы данных...");
        await repository.InitializeAsync(cancellationToken);

        output.Log("Получение меню...");
        IReadOnlyList<MenuItem> menu;
        try
        {
            menu = await client.GetMenuAsync(cancellationToken);
        }
        catch (SmsApiException exception)
        {
            output.WriteLine(exception.ErrorMessage);
            return 1;
        }

        await repository.UpsertAsync(menu, cancellationToken);
        foreach (var item in menu)
        {
            output.WriteLine($"{item.Name} - {item.Id} ({item.Article}) - {item.Price:0.##}");
        }

        var menuById = menu.ToDictionary(item => item.Id, StringComparer.Ordinal);
        IReadOnlyCollection<OrderItem> items;
        while (!OrderParser.TryParse(
                   output.ReadLine("Введите заказ: Код1:Количество1;Код2:Количество2;..."),
                   menuById,
                   out items,
                   out var error))
        {
            output.WriteLine(error!);
        }

        try
        {
            await client.SendOrderAsync(
                new Order(Guid.NewGuid().ToString(), items),
                cancellationToken);
            output.WriteLine("УСПЕХ");
            return 0;
        }
        catch (SmsApiException exception)
        {
            output.WriteLine(exception.ErrorMessage);
            return 1;
        }
    }
}
