using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoogRemind
{
    /// <summary>
    /// Interaction logic for AddReminderForm.xaml
    /// </summary>
    public partial class AddReminderForm : Window
    {
        public static AddReminderForm Instance;

        public AddReminderForm()
        {
            Instance = this;
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(HoursTextBox.Text, out _))
            {
                MessageBox.Show("Your reminder time is retarded, fix it");
                return;
            }

            Reminder reminder = new Reminder(NameTextBox.Text, HoursTextBox.Text);
            MainWindow.Instance.Reminders.Add(reminder);

            this.Close();
        }
    }
}
