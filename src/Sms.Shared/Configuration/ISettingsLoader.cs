namespace Sms.Shared.Configuration;

public interface ISettingsLoader
{
    T Load<T>(string path);
}