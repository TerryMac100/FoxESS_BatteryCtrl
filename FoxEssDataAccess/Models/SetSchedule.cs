using System.Text.Json.Serialization;

namespace FoxEssDataAccess.Models
{
    public class SetScheduleResponse : IFoxResponse
    {
        [JsonPropertyName("errno")]
        public int Errno { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }
    }

    public class SetSchedule
    {
        public SetSchedule()
        {
            DeviceSN = string.Empty;
        }

        public SetSchedule(string deviceSN)
        {
            DeviceSN = deviceSN;
        }

        [JsonPropertyName("deviceSN")]
        public string DeviceSN { get; set; }

        [JsonPropertyName("groups")]
        public List<SetTimeSegment>? Groups { get; set; }
    }

    public class SetTimeSegment : GetTimeSegment
    {
        public int Enable { get; set; } = 0;
    }
}
