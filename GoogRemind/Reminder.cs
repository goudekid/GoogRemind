using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Text.Json;

namespace GoogRemind
{
    [Serializable]
    public class Reminder
    {
        public double TimeSpanAfterStartupToShowReminder { get; set; }

        public TimeSpan TimeSpan { get 
            {
                return TimeSpan.FromHours(TimeSpanAfterStartupToShowReminder);
            } 
        }
        public string ReminderName { get; set; }
        public bool Triggered { get; internal set; }

        public Reminder()
        {

        }

        public Reminder(string ReminderName, string ReminderTime)
        {
            this.ReminderName = ReminderName;

            if (!double.TryParse(ReminderTime, out double ReminderTimeDouble))
            {
                MessageBox.Show("Your reminder time is retarded, fix it");
                return;
            }

            this.TimeSpanAfterStartupToShowReminder = ReminderTimeDouble;

            this.Triggered = false;
        }
    }
}
