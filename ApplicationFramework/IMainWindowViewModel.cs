using System.Windows.Controls;

namespace ApplicationFramework
{
    public interface IMainWindowViewModel
    {
        void AddChildView(UserControl view);

        void Close();

        void SetBusy(string message);

        void ClearBusy();

        void SetErrorMessage(string message);
    }
}
