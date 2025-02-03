using PL.Admin;
using PL.Volunteer;
using System.Windows;

namespace PL
{
    public partial class LobbyScreen : Window
    {
        public string IdField { get; set; } = "";
        public bool IsAdminConnected { get; set; } = false;
        public string passwordField { get; set; } = "";
        private static BlApi.IBl s_bl = BlApi.Factory.Get();
        public Visibility IsRoleSelectionVisible { get; set; } = Visibility.Hidden;
        public int UserId { get; set; }
        public LobbyScreen()
        {
            InitializeComponent();
            DataContext = this;
        }
        private static int _connectedAdminId = 0 ; // משתנה סטטי לשמירת ה-ID של המנהל המחובר

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                string role = s_bl.Volunteer.Login(IdField, passwordField);
                int userId = int.Parse(IdField);

                if (role == "Admin")
                {
                    // בדיקה אם כבר יש מנהל מחובר
                    if (_connectedAdminId != 0)
                    {
                        throw new Exception("Admin is already connected. Only one admin can be connected at a time.");
                    }
                    else
                    {
                        // שמירת ה-ID של המנהל המחובר
                        _connectedAdminId = userId;
                        IsAdminConnected = true;
                        IsRoleSelectionVisible = Visibility.Visible;

                        // עדכון ה-UI
                        Dispatcher.Invoke(() => { DataContext = null; DataContext = this; });

                    }
                }
                else if (role == "Volunteer")
                {
                    MessageBox.Show("Welcome, Volunteer!");
                    new VolunteerLobbyWindow(userId).Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void Login_Button(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string role = s_bl.Volunteer.Login(IdField, passwordField);
        //        UserId = int.Parse(IdField);
        //        if(IsAdminConnected == true)
        //        {
        //            IsRoleSelectionVisible = Visibility.Hidden;
        //            Dispatcher.Invoke(() => { DataContext = null; DataContext = this; });
        //            throw new Exception("Admin is already connected");

        //        }
        //        if (role == "Admin" && IsAdminConnected == false)
        //        {
        //            IsAdminConnected = true;
        //            IsRoleSelectionVisible = Visibility.Visible;
        //            // Force UI to update (using Data Binding only)
        //            Dispatcher.Invoke(() => { DataContext = null; DataContext = this; });
        //        }
        //        else if (role == "Volunteer")
        //        {
        //            MessageBox.Show("Welcome, Volunteer!");
        //            new VolunteerLobbyWindow(int.Parse(IdField)).Show();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void Admin_Button_Click(object sender, RoutedEventArgs e)
        {
            new AdminWindow(_connectedAdminId).Show();
        }

        private void Volunteer_Button_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerLobbyWindow(_connectedAdminId).Show();
        }
    }
}
