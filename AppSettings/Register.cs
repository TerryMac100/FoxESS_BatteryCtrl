using AppSettings.ViewModels;
using AppSettings.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AppSettings
{
    public class Register
    {
        public static IServiceCollection RegisterItems(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<SettingsViewModel, SettingsViewModel>();
            serviceCollection.AddSingleton<SettingsView, SettingsView>();

            return serviceCollection;
        }
    }
}
