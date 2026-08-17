using Autofac;
using Sms.ConsoleApp.Application;
using Sms.ConsoleApp.Composition;
using Sms.ConsoleApp.Configuration;
using Sms.ConsoleApp.Output;
using Sms.Shared.Configuration;

using var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

IContainer? container = null;
IConsoleOutput? output = null;

try
{
    var settings =
        JsonSettingsLoader.Load<ConsoleAppSettings>(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
    var builder = new ContainerBuilder();
    builder.RegisterModule(new ConsoleAppModule(settings));
    container = builder.Build();

    output = container.Resolve<IConsoleOutput>();

    return await container.Resolve<ConsoleApplication>().RunAsync(cancellation.Token);
}
catch (OperationCanceledException)
{
    WriteLine("Операция отменена.");

    return 1;
}
catch (Exception exception)
{
    WriteLine($"Ошибка: {exception.Message}");

    return 1;
}
finally
{
    if (container is not null)
    {
        await container.DisposeAsync();
    }
}

void WriteLine(string message)
{
    if (output is null)
    {
        Console.WriteLine(message);
    }
    else
    {
        output.WriteLine(message);
    }
}