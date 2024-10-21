using System.Text.Json.Serialization;

namespace FoxEssDataAccess.Models
{
    public class GetTimeSegmentResponse : IFoxResponse
    {
        [JsonPropertyName("errno")]
        public int Errno { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("result")]
        public GetTimeSegmentResult? Result { get; set; }
    }

    public class GetTimeSegmentResult
    {
        [JsonPropertyName("enable")]
        public int Enable { get; set; }

        [JsonPropertyName("groups")]
        public List<GetTimeSegment>? Groups { get; set;}
    }

    public class GetTimeSegment
    {
        public int StartHour { get; set; }
        public int StartMinute { get; set; }

        public int EndHour { get; set; }
        public int EndMinute { get; set; }

        public string WorkMode { get; set; } = "SelfUse";

        public int MinSocOnGrid { get; set; } = 10;
        public int FdSoc { get; set; } = 10;
        public int FdPwr { get; set; } = 0;
    }
}
