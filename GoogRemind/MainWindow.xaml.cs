﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Timers;
using Path = System.IO.Path;
using System.Media;

namespace GoogRemind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Reminder> Reminders;
        public AddReminderForm AddReminderForm;
        public static MainWindow Instance;
        public Stopwatch Stopwatch;

        static string AppDataString = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string SavePath = Path.Combine(AppDataString, "GoogRemind", "UserReminders.json");

        public MainWindow()
        {
            Stopwatch = Stopwatch.StartNew();
            InitializeComponent();
            Instance = this;

            // load reminders
            Reminders = new ObservableCollection<Reminder>();

            RemindersListView.ItemsSource = Reminders;

            string jsonReminders = "";
            try
            {
                jsonReminders = File.ReadAllText(SavePath);
            }
            catch (DirectoryNotFoundException ex)
            {
                Directory.CreateDirectory(Path.Combine(AppDataString, "GoogRemind"));
            }
            catch (FileNotFoundException ex)
            {
                var stream = File.Create(SavePath);
                stream.Close();
                jsonReminders = File.ReadAllText(SavePath);
            }

            if (jsonReminders.Length > 0)
            {
                IEnumerable<Reminder> loadedReminders = (IEnumerable<Reminder>)JsonSerializer.Deserialize(jsonReminders, typeof(IEnumerable<Reminder>));

                foreach (Reminder reminder in loadedReminders)
                {
                    reminder.Triggered = false;
                    Reminders.Add(reminder);
                }
            }

            DataContext = this;

            // set up small task to run every minute checking all reminders
            // hope you didn't make a million fucking reminders, this shit aint optimal
            Timer timer = new Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(OnElapsed);
            timer.AutoReset = true;
            timer.Start();

        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            foreach(Reminder reminder in Reminders)
            {
                if (reminder.TimeSpan < Stopwatch.Elapsed && !reminder.Triggered)
                {
                    SystemSounds.Beep.Play();
                    reminder.Triggered = true;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ReminderWindow reminderWindow = new ReminderWindow(reminder.ReminderName);
                        reminderWindow.ShowDialog();
                    });
                }
            }
        }

        private void SaveReminders()
        {
            File.WriteAllText(SavePath, JsonSerializer.Serialize(Reminders));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddReminderForm = new AddReminderForm();
            AddReminderForm.ShowDialog();
            SaveReminders();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Reminder selected = (Reminder)RemindersListView.SelectedItem;
            Reminders.Remove(selected);
            SaveReminders();
        }

        // Minimize to system tray when application is closed.
        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            e.Cancel = true;

            this.Hide();

            base.OnClosing(e);
        }

        private void notifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            return;
        }

        private void MarkTriggeredButton_Click(object sender, RoutedEventArgs e)
        {
            Reminder selected = (Reminder)RemindersListView.SelectedItem;
            Reminders.First(it => it.ReminderName == selected.ReminderName).Triggered = true;
        }
    }
}
