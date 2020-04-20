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
    public sealed partial class AppointmenDetailsPage : Page
    {
        public static DateTime DateTimeNow = DateTime.Now;
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmenDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int ID = AppointmentPage.NavID;
            Appointment aptmtObj = new Appointment();
            aptmtObj = conn.Find<Appointment>(ID);
            EventName.Text = aptmtObj.AptmtName;
            EventDescription.Text = aptmtObj.AptmtDesc;
            Location.Text = aptmtObj.AptmtDesc;
            DateTime evDate = Convert.ToDateTime(aptmtObj.AptmtDate);
            EventDate.Date = evDate;
            EventStartTime.Time = DateTime.Parse(aptmtObj.StartTime).TimeOfDay;
            EventEndTime.Time = DateTime.Parse(aptmtObj.EndTime).TimeOfDay;
        }

        private async void save_aptmt_btn_Clcik(object sender, RoutedEventArgs e)
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
                DateTime eventEnd_time = DateTime.Today.Add(EventEndTime.Time);
                Appointment aptmObjSave = new Appointment();
                aptmObjSave = conn.Get<Appointment>(AppointmentPage.NavID);
                aptmObjSave.AptmtName = EventName.Text;
                aptmObjSave.AptmtDesc = EventDescription.Text;
                aptmObjSave.Location = Location.Text;
                aptmObjSave.AptmtDate = event_date.ToString("dd.MM.yyyy");
                aptmObjSave.StartTime = eventStart_time.ToString("hh:mm tt");
                aptmObjSave.EndTime = eventEnd_time.ToString("hh:mm tt");
                conn.Update(aptmObjSave);
                
                MessageDialog info = new MessageDialog("Appointment is successfully updated.");
                await info.ShowAsync();
                Frame.Navigate(typeof(AppointmentPage));
            }
        }
    }
}
