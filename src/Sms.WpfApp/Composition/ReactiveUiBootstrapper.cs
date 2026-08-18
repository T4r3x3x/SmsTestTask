using ReactiveUI.Builder;

namespace Sms.WpfApp.Composition;

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
