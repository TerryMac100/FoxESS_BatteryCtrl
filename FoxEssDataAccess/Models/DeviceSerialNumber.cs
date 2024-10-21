using ApplicationFramework;
using System.Text.Json.Serialization;

namespace FoxEssDataAccess.Models
{
    public class DeviceSerialNumber
    {
        private readonly ISettings m_settings;

        internal DeviceSerialNumber(ISettings settings)
        {
            m_settings = settings;
        }

        [JsonPropertyName("deviceSN")]
        public string DeviceSN => m_settings.DeviceSN;
    }
}
