using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        public static DateTime DateTimeNow = DateTime.Now;
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        // gets a list of appointment from the database
        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointment>();
            var query = conn.Table<Appointment>();
            AppointmentList.ItemsSource = query.ToList();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void add_aptmn_btn_click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            try
            {
                EventStartTime.Time.ToString();
                // checks if the Event Name and Event Location are Empty
                if (EventName.Text.ToString() == "" || Location.Text == "")
                {
                    MessageDialog dialog = new MessageDialog("Event Name and Event Location must be ", "Oops..!");
                    await dialog.ShowAsync();
                    isValid = false;
                }

                // checks if event date has not been entered and if the date selected is less than the current date
                if (EventDate.Date == null || EventDate.Date < DateTimeNow.Date)
                {
                    MessageDialog dialog = new MessageDialog("Please select a valid date.\nRemember, the Event Date cannot be less than the current date.", "Oops..!");
                    await dialog.ShowAsync();
                    isValid = false;
                }

                
                // checks if the event date and the current date match
                if (EventDate.Date == DateTimeNow.Date)
                {
                    // checks if the start time 
                    if (EventStartTime.Time < DateTimeNow.TimeOfDay)
                    {
                        MessageDialog dialog = new MessageDialog("The time selected is not valid. Start time has to be greater than current time ", "Oops..!");
                        await dialog.ShowAsync();
                        isValid = false;
                    }
                }

                // checks if Start Time is less than End Time
                if (EventStartTime.Time > EventEndTime.Time)
                {
                    MessageDialog dialog = new MessageDialog("The Start time must be less than the End time. Please change the time.", "Oops..!");
                    await dialog.ShowAsync();
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                MessageDialog infoBox = new MessageDialog("Sorry Something Went Wrong. Try Again. " + ex);
                await infoBox.ShowAsync();
            }            


            if (isValid == true)
            {
                // converting CaldendarDatePicker value into DateTime object to modify dsiplaying format
                var date = EventDate.Date;
                DateTime event_date = date.Value.DateTime;

                // converting TimpePicker values into  DateTime objects to modify displaying format into AM/PM
                DateTime eventStart_time = DateTime.Today.Add(EventStartTime.Time);
                DateTime eventEnd_time = DateTime.Today.Add(EventStartTime.Time);

                conn.Insert(new Appointment()
                {
                    AptmtName = EventName.Text,
                    AptmtDesc = EventDescription.Text,
                    Location = Location.Text,
                    AptmtDate = event_date.ToString("dd.mm.yyyy"),
                    StartTime = eventStart_time.ToString("hh:mm tt"),
                    EndTime = eventEnd_time.ToString("hh:mm tt"),
                });

                Results();
                MessageDialog info = new MessageDialog("Appointment was successfully added.");
                await info.ShowAsync();
            }
        }

        private async void delete_aptmn_btn_click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure to delete this Appointment", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                try
                {
                    int aptmtID = ((Appointment)AppointmentList.SelectedItem).ID;
                    var querydel = conn.Query<Appointment>("DELETE FROM Appointment WHERE ID='" + aptmtID + "'");
                    Results();

                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }

        }
    }
}
