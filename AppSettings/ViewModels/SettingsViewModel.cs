using ApplicationFramework;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AppSettings.ViewModels
{
    public class SettingsViewModel : VMBase
    {
        private readonly ISettings m_settings;
        private readonly IMainWindowViewModel m_mainWindowViewModel;
        private bool m_editActive;

        public SettingsViewModel(IMainWindowViewModel mainWindowViewModel, ISettings settings)
        {
            m_settings = settings;
            m_mainWindowViewModel = mainWindowViewModel;
            MainMenu = new RelayCommand<string>(Execute, CanExecute);
        }

        private bool CanExecute(string? obj)
        {
            switch (obj)
            {
                case "Cancel":
                case "Save":
                    return EditActive;


                case "Close":
                    return !EditActive;
            }
            return false;
        }

        private void Execute(string? obj)
        {
            switch (obj)
            {
                case "Cancel":
                    m_settings.Cancel();
                    EditActive = false;
                    OnPropertyChanged(nameof(DeviceSN));
                    OnPropertyChanged(nameof(APIKey));
                    break;

                case "Save":
                    m_settings.Save();
                    EditActive = false;
                    break;

                case "Close":
                    m_mainWindowViewModel.Close();
                    break;
            }
        }

        public string DeviceSN
        {
            get => m_settings.DeviceSN;
            set
            {
                m_settings.DeviceSN = value;
                EditActive = true;
            }
        }

        public string APIKey
        {
            get => m_settings.ApiKey;
            set
            {
                m_settings.ApiKey = value;
                EditActive = true;
            }
        }

        public RelayCommand<string> MainMenu { get; }
        public bool EditActive
        {
            get => m_editActive;
            set
            {
                m_editActive = value;
                MainMenu.NotifyCanExecuteChanged();
            }
        }

        public override string? Title => "FoxESS - Settings";
    }
}
