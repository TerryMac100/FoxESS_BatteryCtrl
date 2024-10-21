using ApplicationFramework;
using AppSettings.Views;
using ChargeController.Models;
using ChargeController.Views;
using FoxEssDataAccess;
using FoxEssDataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

namespace ChargeController.ViewModels
{
    public class StartViewModel : VMBase  //, IDataErrorInfo
    {
        private readonly IServiceProvider m_service;
        private readonly IMainWindowViewModel m_mainWindowViewModel;

        private readonly FoxESSService m_foxESSService;
        private readonly ISettings m_settings;
        private bool m_editActive;

        public StartViewModel(IServiceProvider service, IMainWindowViewModel mainWindowViewModel, FoxESSService foxESSService, ISettings settings)
        {
            m_service = service;
            m_mainWindowViewModel = mainWindowViewModel;
            m_foxESSService = foxESSService;
            m_settings = settings;

            MainMenu = new RelayCommand<string>(Execute, CanExecute);

            m_selfUseMinSOC = Properties.Settings.Default.SelfUseMinSOC;
            m_forceDischargeMinSOC = Properties.Settings.Default.ForceDischargeMinSOC;
            m_forceDischargePower = Properties.Settings.Default.ForceDischargePower;

            var templates = JsonConvert.DeserializeObject<ObservableCollection<Template>>(Properties.Settings.Default.TemplateSchedules);

            if (templates != null )
            {
                Templates = templates;
                if (Templates.Count >= Properties.Settings.Default.SelectedTemplate)
                    SelectedTemplate = Templates[Properties.Settings.Default.SelectedTemplate];
            }

            //var schedule = JsonConvert.DeserializeObject<SetSchedule>(Properties.Settings.Default.ScheduleSettings);
            //var schedule = SelectedTemplate;

            //foreach (var period in Schedule)
            //    period.SelfUse = true;

            if (SelectedTemplate != null && SelectedTemplate.Groups != null)
                foreach (var period in SelectedTemplate.Groups)
                    SetScheduleDisplay(period);
        }

        private ObservableCollection<Template> m_templates;
        public ObservableCollection<Template> Templates 
        {
            get 
            { 
                if (m_templates == null)
                {
                    m_templates = new ObservableCollection<Template>();
                    m_templates.Add(new Template()
                    {
                        Name = "Default",
                        Groups = new List<SetTimeSegment>()
                    });

                    SelectedTemplate = m_templates.First();
                }

                return m_templates;
            }
            set
            {
                m_templates = value;
            }         
        }

        private Template? m_selectedTemplate;

        public Template? SelectedTemplate 
        { 
            get => m_selectedTemplate;
            set 
            {

                if (value != null)
                {
                    if (m_selectedTemplate != null)
                    {
                        var schedule = GetScheduleObject();
                        m_selectedTemplate.Groups = schedule.Groups;
                    }

                    m_selectedTemplate = value;

                    if (SelectedTemplate != null && SelectedTemplate.Groups != null)
                        foreach (var period in SelectedTemplate.Groups)
                            SetScheduleDisplay(period);
                }

                OnPropertyChanged(nameof(SelectedTemplate));
                OnPropertyChanged(nameof(Templates));
            } 
        }

        private string m_selectedTemplateText;
        public string SelectedTemplateText
        {
            get
            {
                if (SelectedTemplate != null)
                    return SelectedTemplate.Name;

                return m_selectedTemplateText;
            } 
            set 
            {
                m_selectedTemplateText = value;
                if (SelectedTemplate != null)
                {
                    SelectedTemplate.Name = value;
                    OnPropertyChanged(nameof(Templates));
                    OnPropertyChanged(nameof(SelectedTemplate));
                }
            }
        }

        private void SetScheduleDisplay(GetTimeSegment period)
        {
            var startIndex = period.StartHour * 2;
            if (period.StartMinute >= 30)
                startIndex += 1;

            var endIndex = period.EndHour * 2;
            if (period.EndMinute > 30)
                endIndex += 1;

            for (int i = startIndex; i <= endIndex; i++)
                switch (period.WorkMode)
                {
                    case "ForceCharge":
                        //Schedule[i].SelfUse = false;
                        Schedule[i].Charge = true;
                        break;

                    case "Backup":
                        //Schedule[i].SelfUse = false;
                        Schedule[i].Backup = true;
                        break;

                    case "ForceDischarge":
                        //Schedule[i].SelfUse = false;
                        Schedule[i].Discharge = true;
                        break;

                    case "Feedin":
                        //Schedule[i].SelfUse = false;
                        Schedule[i].FeedIn = true;
                        break;

                    case "SelfUse":
                        Schedule[i].SelfUse = true;
                        break;

                }
        }

        private ChargeSwitchViewModel[] m_schedule;

        public ChargeSwitchViewModel[] Schedule
        {
            get
            {
                if (m_schedule == null)
                {
                    m_schedule = new ChargeSwitchViewModel[48];

                    for (int x = 0; x < 48; x++)
                        m_schedule[x] = new ChargeSwitchViewModel(x);
                }

                return m_schedule;
            }
        }

        private int m_selfUseMinSOC;

        public int SelfUseMinSOC
        {
            get { return m_selfUseMinSOC; }
            set { m_selfUseMinSOC = value; }
        }

        private int m_forceDischargeMinSOC;
        public int ForceDischargeMinSOC
        {
            get { return m_forceDischargeMinSOC; }
            set { m_forceDischargeMinSOC = value; }
        }

        private int m_forceDischargePower;
        public int ForceDischargePower
        {
            get { return m_forceDischargePower; }
            set { m_forceDischargePower = value; }
        }

        private bool CanExecute(string? obj)
        {
            switch (obj)
            {
                case "AddTemplate":
                case "Settings":
                case "SetSchedule":
                case "GetSchedule":
                case "Close":
                    return true;

                case "DeleteTemplate":
                    return Templates.Count > 1;
            }
            return false;
        }

        private void Execute(string? obj)
        {
            switch (obj)
            {
                case "GetSchedule":
                    GetSchedule();
                    break;

                case "SetSchedule":
                    SetSchedule();
                    break;

                case "Settings":
                    m_mainWindowViewModel.AddChildView(m_service.GetRequiredService<SettingsView>());
                    break;

                case "Close":
                    SaveSettings();
                    m_mainWindowViewModel.Close();
                    break;

                case "AddTemplate":
                    m_templates.Add(new Template()
                    {
                        Name = $"Template {m_templates.Count}",
                        Groups = new List<SetTimeSegment>()
                    });

                    SelectedTemplate = m_templates.Last();
                    MainMenu.NotifyCanExecuteChanged();
                    break;

                case "DeleteTemplate":
                    if (SelectedTemplate  != null)
                        Templates.Remove(SelectedTemplate);

                    SelectedTemplate = Templates.First();
                    MainMenu.NotifyCanExecuteChanged();
                    break;
            }
        }

        private void SaveSettings()
        {
            if (SelectedTemplate != null)
            {
                // Update the selected template it may have been changed by user
                var schedule = GetScheduleObject();
                SelectedTemplate.Groups = schedule.Groups;
            }

            Properties.Settings.Default.TemplateSchedules = JsonConvert.SerializeObject(Templates);
            if (SelectedTemplate != null && Templates.Contains(SelectedTemplate))
                Properties.Settings.Default.SelectedTemplate = Templates.IndexOf(SelectedTemplate);

            Properties.Settings.Default.SelfUseMinSOC = SelfUseMinSOC;
            Properties.Settings.Default.ForceDischargeMinSOC = ForceDischargeMinSOC;
            Properties.Settings.Default.ForceDischargePower = ForceDischargePower;

            Properties.Settings.Default.Save();
            EditActive = false;
        }

        private async void GetSchedule()
        {
            var timeSegments = await m_foxESSService.GetSchedule();

            if (timeSegments == null || timeSegments.Result == null || timeSegments.Result.Groups == null)
                return;
            
            var schedule = timeSegments.Result.Groups;

            foreach (var period in Schedule)
                period.SelfUse = true;

            if (schedule != null)
                foreach (var period in schedule)
                {
                    SetScheduleDisplay(period);
                    switch (period.WorkMode)
                    {
                        case "Selfuse":
                            SelfUseMinSOC = period.MinSocOnGrid;
                            
                            break;
                    }
                }
        }

        private void SetSchedule()
        {
            var schedule = GetScheduleObject();
            var resp = m_foxESSService.SetSchedule(schedule);
        }

        private SetSchedule GetScheduleObject()
        {
            var schedule = new SetSchedule(m_settings.DeviceSN);

            // Schedule index
            int s = 0;

            // 30 minute increment
            int i = 0;

            schedule.Groups = new List<SetTimeSegment>();
            while (i < Schedule.Length && s < 8)
            {
                if (Schedule[i].Charge)
                {
                    schedule.Groups.Add(new SetTimeSegment());
                    schedule.Groups[s].WorkMode = "ForceCharge";
                    schedule.Groups[s].Enable = 1;
                    schedule.Groups[s].MinSocOnGrid = SelfUseMinSOC;
                    schedule.Groups[s].FdSoc = SelfUseMinSOC;
                    schedule.Groups[s].FdPwr = 0;

                    schedule.Groups[s].StartHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].StartMinute = 30;

                    while (i < Schedule.Length - 1 && Schedule[i + 1].Charge)
                        i++;

                    schedule.Groups[s].EndHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].EndMinute = 59;
                    else
                        schedule.Groups[s].EndMinute = 29;

                    s++;
                }
                else if (Schedule[i].Backup)
                {
                    schedule.Groups.Add(new SetTimeSegment());
                    schedule.Groups[s].WorkMode = "Backup";
                    schedule.Groups[s].Enable = 1;
                    schedule.Groups[s].MinSocOnGrid = SelfUseMinSOC;
                    schedule.Groups[s].FdSoc = SelfUseMinSOC;
                    schedule.Groups[s].FdPwr = 0;

                    schedule.Groups[s].StartHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].StartMinute = 30;

                    while (i < Schedule.Length - 1 && Schedule[i + 1].Backup)
                        i++;

                    schedule.Groups[s].EndHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].EndMinute = 59;
                    else
                        schedule.Groups[s].EndMinute = 29;

                    s++;
                }
                else if (Schedule[i].Discharge)
                {
                    schedule.Groups.Add(new SetTimeSegment());
                    schedule.Groups[s].WorkMode = "ForceDischarge";
                    schedule.Groups[s].Enable = 1;

                    schedule.Groups[s].MinSocOnGrid = SelfUseMinSOC;
                    schedule.Groups[s].FdSoc = ForceDischargeMinSOC;
                    schedule.Groups[s].FdPwr = ForceDischargePower;

                    schedule.Groups[s].StartHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].StartMinute = 30;

                    while (i < Schedule.Length - 1 && Schedule[i + 1].Discharge)
                        i++;

                    schedule.Groups[s].EndHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].EndMinute = 59;
                    else
                        schedule.Groups[s].EndMinute = 29;

                    s++;
                }
                else if (Schedule[i].FeedIn)
                {
                    schedule.Groups.Add(new SetTimeSegment());
                    schedule.Groups[s].WorkMode = "Feedin";
                    schedule.Groups[s].Enable = 1;

                    schedule.Groups[s].MinSocOnGrid = SelfUseMinSOC;
                    schedule.Groups[s].FdSoc = ForceDischargeMinSOC;
                    schedule.Groups[s].FdPwr = ForceDischargePower;

                    schedule.Groups[s].StartHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].StartMinute = 30;

                    while (i < Schedule.Length - 1 && Schedule[i + 1].FeedIn)
                        i++;

                    schedule.Groups[s].EndHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].EndMinute = 59;
                    else
                        schedule.Groups[s].EndMinute = 29;

                    s++;
                }
                else if (Schedule[i].SelfUse)
                {
                    schedule.Groups.Add(new SetTimeSegment());
                    schedule.Groups[s].WorkMode = "SelfUse";
                    schedule.Groups[s].Enable = 1;

                    schedule.Groups[s].MinSocOnGrid = SelfUseMinSOC;
                    schedule.Groups[s].FdSoc = SelfUseMinSOC;
                    schedule.Groups[s].FdPwr = 0;

                    schedule.Groups[s].StartHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].StartMinute = 30;

                    while (i < Schedule.Length - 1 && Schedule[i + 1].SelfUse)
                        i++;

                    schedule.Groups[s].EndHour = i >> 1;
                    if ((i % 2) == 1)
                        schedule.Groups[s].EndMinute = 59;
                    else
                        schedule.Groups[s].EndMinute = 29;

                    s++;
                }

                i++;
            }

            return schedule;
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

        public override string Title => "FoxESS - Battery Controller";
    }
}
