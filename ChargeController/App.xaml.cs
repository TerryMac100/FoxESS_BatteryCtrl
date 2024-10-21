using ApplicationFramework;
using ChargeController.ViewModels;
using ChargeController.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ChargeController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            var list = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(type => type.IsClass)
                .Where(type => (type.Name.EndsWith("ViewModel") && type.Namespace != null && type.Namespace.EndsWith("ViewModels")) ||
                               (type.Name.EndsWith("View") && type.Namespace != null && type.Namespace.EndsWith("Views")));

            var sevices = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
                    services.AddSingleton<MainWindow, MainWindow>();
                    services.AddSingleton<ISettings, Settings>();

                    foreach (var t in list)
                        services.AddTransient(t);

                    services = FoxEssDataAccess.Register.RegisterItems(services);
                    services = AppSettings.Register.RegisterItems(services);

                });

            AppHost = sevices.Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            var mainViewModel = AppHost.Services.GetRequiredService<IMainWindowViewModel>();

            mainViewModel.AddChildView(AppHost.Services.GetRequiredService<StartView>());

            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
