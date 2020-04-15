using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {

        bool isEditing;
        int editingContactID;

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            conn.CreateTable<ContactDetails>();
            var query = conn.Table<ContactDetails>();
            ContactDetailsListView.ItemsSource = query.ToList();
        }

        private async void AddContactDetail_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text == "" || LastName.Text == "" || CompanyName.Text == "" || MobilePhone.Text == "")
            {
                MessageDialog dialog = new MessageDialog("All fields must have values!", "Oops..!");
                await dialog.ShowAsync();
            }
            else
            {
                if (!isEditing)
                {
                    conn.CreateTable<ContactDetails>();
                    conn.Insert(new ContactDetails
                    {
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        CompanyName = CompanyName.Text,
                        MobilePhone = MobilePhone.Text
                    });

                    Results();
                } else
                {
                    conn.CreateTable<WishList>();
                    var query1 = conn.Table<WishList>();
                    var query3 = conn.Query<WishList>("UPDATE ContactDetails SET FirstName='" + FirstName.Text + 
                        "', LastName='" + LastName.Text + "', CompanyName='" + CompanyName.Text + 
                        "', MobilePhone='" + MobilePhone.Text + "' WHERE ContactID ='" + editingContactID + "'");
                    ContactDetailsListView.ItemsSource = query1.ToList();

                    //Resetting buttons displaying data
                    Page_Loaded(sender, e);
                }
            }
        }

        private async void EditContactDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ContactDetails)ContactDetailsListView.SelectedItem).FirstName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    if (!isEditing)
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Label = "Update Contact Detail";
                        isEditing = true;
                    }

                    var selected = (ContactDetails)ContactDetailsListView.SelectedItem;

                    editingContactID = selected.ContactID;

                    FirstName.Text = selected.FirstName;
                    LastName.Text = selected.LastName;
                    CompanyName.Text = selected.CompanyName;
                    MobilePhone.Text = selected.MobilePhone;
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void DeleteContactDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection = ((ContactDetails)ContactDetailsListView.SelectedItem).ContactID;
                if (AccSelection == 0)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<WishList>();
                    var query1 = conn.Table<WishList>();
                    var query3 = conn.Query<WishList>("DELETE FROM ContactDetails WHERE ContactID ='" + AccSelection + "'");
                    ContactDetailsListView.ItemsSource = query1.ToList();

                    Results();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            isEditing = false;
            editingContactID = -1;

            AddButton.Label = "Add Contact Detail";
            EditButton.Visibility = Visibility.Visible;
            Results();
        }
    }
}
