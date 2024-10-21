using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace ChargeController.ViewModels
{
    public class ChargeSwitchViewModel : ObservableObject
    {
        private readonly int m_index;

        public ChargeSwitchViewModel(int index)
        {
            m_index = index;
        }

        public bool Charge
        {
            get => (m_chargeState & ChargeStates.Charge) != 0;
            set
            {
                if (value)
                {
                    m_chargeState = ChargeStates.Charge;
                    UpdateAll();
                }
            }
        }
        public bool Backup
        {
            get => (m_chargeState & ChargeStates.Backup) != 0;
            set
            {
                if (value)
                {
                    m_chargeState = ChargeStates.Backup;
                    UpdateAll();
                }
            }
        }        
        public bool SelfUse
        {
            get => (m_chargeState & ChargeStates.SelfUse) != 0;
            set
            {
                if (value)
                {
                    m_chargeState = ChargeStates.SelfUse;
                    UpdateAll();
                }
            }
        }       
        public bool Discharge
        {
            get => (m_chargeState & ChargeStates.Discharge) != 0;
            set
            {
                if (value)
                {
                    m_chargeState = ChargeStates.Discharge;
                    UpdateAll();
                }
            }
        }
        public bool FeedIn
        {
            get => (m_chargeState & ChargeStates.FeedIn) != 0;
            set
            {
                if (value)
                {
                    m_chargeState = ChargeStates.FeedIn;
                    UpdateAll();
                }
            }
        }

        public ChargeStates m_chargeState = ChargeStates.SelfUse;

        private void UpdateAll()
        {
            OnPropertyChanged(nameof(Discharge));
            OnPropertyChanged(nameof(Charge));
            OnPropertyChanged(nameof(Backup));
            OnPropertyChanged(nameof(SelfUse));
            OnPropertyChanged(nameof(FeedIn));
            
            OnPropertyChanged(nameof(ToolTip));

            ValueChanged?.Invoke(this, null);
        }

        public event PropertyChangedEventHandler? ValueChanged;

        private string GetState()
        {
            if (Backup)
                return "\nBackup\n";

            if (Charge)
                return "\nForce Charge\n";

            if (Discharge)
                return "\nForce Discharge\n";

            if (FeedIn)
                return "\nFeed In\n";

            return "\nSelf Use\n";
        }

        public string ToolTip
        {
            get
            {
                string toolTip = (m_index / 2).ToString("D2");

                if (m_index % 2 == 0)
                    toolTip += ":00";
                else
                    toolTip += ":30";

                toolTip += GetState();

                toolTip += ((m_index + 1) / 2).ToString("D2");

                if ((m_index + 1) % 2 == 0)
                    toolTip += ":00";
                else
                    toolTip += ":30";

                return toolTip;
            }
        }

        public string TickLegend
        {
            get
            {
                if (m_index % 6 == 0)
                    return (m_index / 2).ToString("D2");
                return
                    string.Empty;
            }
        }

        public int TickLength
        {
            get
            {
                if (m_index % 6 == 0)
                    return 30;
                else
                    return 15;
            }
        }


        public enum ChargeStates
        {
            Charge = 1,
            Backup = 2,
            SelfUse = 4,
            FeedIn = 8,
            Discharge = 16
        }
    }
}
