using FoxEssDataAccess.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChargeController.Models
{
    public class Template : ObservableObject
    {
        private string m_name = string.Empty;

        [JsonPropertyName("name")]
        public string Name 
        { 
            get => m_name;
            set 
            {
                m_name = value;
                OnPropertyChanged(nameof(Name));
            } 
        }

        [JsonPropertyName("groups")]
        public List<SetTimeSegment>? Groups { get; set; }
    }
}
