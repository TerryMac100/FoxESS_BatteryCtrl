using ApplicationFramework;

namespace ChargeController
{
    public class Settings : ISettings
    {
        public Settings()
        {
            ApiKey = Properties.Settings.Default.ApiKey;
            DeviceSN = Properties.Settings.Default.DeviceSN;
        }

        public string ApiKey { get; set; }

        public string DeviceSN { get; set; }

        public void Save()
        {
            Properties.Settings.Default.ApiKey = ApiKey;
            Properties.Settings.Default.DeviceSN = DeviceSN;
            Properties.Settings.Default.Save();
        }

        public void Cancel()
        {
            ApiKey = Properties.Settings.Default.ApiKey;
            DeviceSN = Properties.Settings.Default.DeviceSN;
        }
    }
}
