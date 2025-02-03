using PL.Admin;
using PL.Volunteer;
using System.Windows;

namespace PL
{
    public partial class LobbyScreen : Window
    {
        // Property to store the user ID input
        public string IdField { get; set; } = "";

        // Boolean flag indicating if an admin is connected
        public bool IsAdminConnected { get; set; } = false;

        // Property to store the user password input
        public string passwordField { get; set; } = "";

        // Static instance of the business logic layer
        private static BlApi.IBl s_bl = BlApi.Factory.Get();

        // Controls the visibility of role selection UI element
        public Visibility IsRoleSelectionVisible { get; set; } = Visibility.Hidden;

        // Stores the user ID after login
        public int UserId { get; set; }

        public LobbyScreen()
        {
            InitializeComponent(); // Initializes the WPF components
            DataContext = this; // Sets the data context for data binding
        }

        // Static variable to keep track of the connected admin's ID
        private static int _connectedAdminId = 0;

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                // Attempt to log in and get the user's role
                string role = s_bl.Volunteer.Login(IdField, passwordField);
                int userId = int.Parse(IdField);

                if (role == "Admin")
                {
                    // Check if an admin is already connected
                    if (_connectedAdminId != 0)
                    {
                        throw new Exception("Admin is already connected. Only one admin can be connected at a time.");
                    }
                    else
                    {
                        // Store the connected admin's ID
                        _connectedAdminId = userId;
                        IsAdminConnected = true;
                        IsRoleSelectionVisible = Visibility.Visible;

                        // Update the UI by refreshing the DataContext
                        Dispatcher.Invoke(() => { DataContext = null; DataContext = this; });
                    }
                }
                else if (role == "Volunteer")
                {
                    MessageBox.Show("Welcome, Volunteer!");
                    // Open the volunteer's lobby window with the user ID
                    new VolunteerLobbyWindow(userId).Show();
                }
            }
            catch (Exception ex)
            {
                // Display any error message
                MessageBox.Show(ex.Message);
            }
        }

        private void Admin_Button_Click(object sender, RoutedEventArgs e)
        {
            // Open the admin window with the stored admin ID
            new AdminWindow(_connectedAdminId).Show();
        }

        private void Volunteer_Button_Click(object sender, RoutedEventArgs e)
        {
            // Open the volunteer lobby window with the stored admin ID
            new VolunteerLobbyWindow(_connectedAdminId).Show();
        }
    }
}
