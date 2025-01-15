using PL.Admin;
using PL.Volunteer;
using System.Windows;

namespace PL
{
    public partial class LobbyScreen : Window
    {
        public string IdField { get; set; } = "";
        public string passwordField { get; set; } = "";
        public Visibility IsRoleSelectionVisible { get; set; } = Visibility.Collapsed;

        private static BlApi.IBl s_bl = BlApi.Factory.Get();
        public int UserId { get; set; }
        public LobbyScreen()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                string role = s_bl.Volunteer.Login(IdField, passwordField);
                UserId = int.Parse(IdField);
                if (role == "Admin")
                {
                    IsRoleSelectionVisible = Visibility.Visible;
                    // Force UI to update (using Data Binding only)
                    Dispatcher.Invoke(() => { DataContext = null; DataContext = this; });
                }
                else if (role == "Volunteer")
                {
                    MessageBox.Show("Welcome, Volunteer!");
                    new VolunteerLobbyWindow(int.Parse(IdField)).Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Admin_Button_Click(object sender, RoutedEventArgs e)
        {
            new AdminWindow(UserId).Show();
        }

        private void Volunteer_Button_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerLobbyWindow(int.Parse(IdField)).Show();
        }
    }
}
