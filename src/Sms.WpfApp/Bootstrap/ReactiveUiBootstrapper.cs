using ReactiveUI.Builder;

namespace Sms.WpfApp.Bootstrap;

public static class ReactiveUiBootstrapper
{
    private static readonly Lazy<bool> Initialization = new(() =>
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
        return true;
    });

    public static void Initialize() => _ = Initialization.Value;
}
