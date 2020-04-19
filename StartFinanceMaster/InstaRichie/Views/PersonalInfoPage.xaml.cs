// **************************************************************************
//Start Finance - An to manage your personal finances.

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    { 
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();

           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
             // Resets the UI if the user left during an edit
            AddButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
            PersonalInfoList.Visibility = Visibility.Visible;

            ClearFeilds();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            PersonalInfoList.ItemsSource = query.ToList();
        }

        //Add the new personal info to the database
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstName.Text.ToString() == "" || LastName.Text.ToString() == "" || Gender.Text.ToString() == "" || Email.Text.ToString() == "" || PhoneNo.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("All Feilds must be entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    DateTime date = DateOfBirth.Date.Value.DateTime;
                    
                    // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        DateOfBirth = date,
                        Gender = Gender.Text,
                        Email = Email.Text,
                        PhoneNo = PhoneNo.Text
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the data or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("SQLite contraints violated", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                }

            }
        }

        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Entry will delete all personal information for this account", "Important");
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
                // checks if data is null else inserts
                try
                {
                    int personal_Id = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE PersonalID='" + personal_Id + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private async void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int personal_Id = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                PersonalInfo infoObject = new PersonalInfo();
                infoObject = conn.Find<PersonalInfo>(personal_Id);
                FirstName.Text = infoObject.FirstName;
                LastName.Text = infoObject.LastName;
                DateOfBirth.Date = infoObject.DateOfBirth;
                Gender.Text = infoObject.Gender;
                Email.Text = infoObject.Email;
                PhoneNo.Text = infoObject.PhoneNo;
                //switched out the add new entry button for the save editted button
                AddButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
                // Hides the sql List so that the selection can't be changed while edditing
                PersonalInfoList.Visibility = Visibility.Collapsed;
            }
            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to Edit", "Oops..!");
                await ClearDialog.ShowAsync();
            }
        }

        //saves the edited info over the existing info
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstName.Text.ToString() == "" || LastName.Text.ToString() == "" || Gender.Text.ToString() == "" || Email.Text.ToString() == "" || PhoneNo.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("All Feilds must be entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    DateTime date = DateOfBirth.Date.Value.DateTime;
                    int personal_Id = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                    //Deletes the existing entry
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE PersonalID='" + personal_Id + "'");
                    Results();

                    // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        PersonalID = personal_Id,
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        DateOfBirth = date,
                        Gender = Gender.Text,
                        Email = Email.Text,
                        PhoneNo = PhoneNo.Text
                    });
                    // Resets the UI when edit is complete
                    AddButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Collapsed;
                    PersonalInfoList.Visibility = Visibility.Visible;

                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the data or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("SQLite contraints violated", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                }

            }
        }

        public void ClearFeilds()
        {
            FirstName.Text = "";
            LastName.Text = "";
            Gender.Text = "";
            Email.Text = "";
            PhoneNo.Text = "";
        }
    }
}