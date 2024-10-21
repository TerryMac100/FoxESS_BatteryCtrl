using ApplicationFramework;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChargeController.ViewModels
{
    public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
    {
        public MainWindowViewModel()
        {
        }

        private string? m_title;
        public string? Title
        {
            get => m_title;
            set
            {
                m_title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public void AddChildView(UserControl view)
        {
            childControls.Add(view);
            OnPropertyChanged(nameof(RootView));
            UpdateTitle();
        }

        public void Close()
        {
            if (childControls.Count > 1)
            {
                childControls.Remove(childControls.Last());
                OnPropertyChanged(nameof(RootView));
                UpdateTitle();
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        private void UpdateTitle()
        {
            var currentVM = RootView.DataContext as VMBase;

            if (currentVM != null && currentVM.Title != null)
            {
                Title = currentVM.Title;
            }
        }

        public UserControl RootView => childControls.Last();

        private List<UserControl> childControls { get; set; } = new List<UserControl>();

        public void SetBusy(string message)
        {
            StatusColour = Colors.Orange.ToString();
            StatusMessage = message;
        }
        public void ClearBusy()
        {
            StatusColour = Colors.Green.ToString();
            StatusMessage = "OK";
        }

        public void SetErrorMessage(string message)
        {
            StatusColour = Colors.Red.ToString();
            StatusMessage = message;
        }

        private string m_statusColour = Colors.Green.ToString();
        public string? StatusColour
        {
            get => m_statusColour;
            set
            {
                m_statusColour = value;
                OnPropertyChanged(nameof(StatusColour));
            }
        }

        private string m_statusMessage = "OK";
        public string? StatusMessage
        {
            get { return m_statusMessage; }
            set 
            { 
                m_statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
    }
}
