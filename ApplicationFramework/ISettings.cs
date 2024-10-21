namespace ApplicationFramework
{
    public interface ISettings
    {
        string ApiKey { get; set; }
        string DeviceSN { get; set; }

        void Cancel();
        void Save();
    }
}

