using PL.Admin;
using PL.Volunteer;
using System.Windows;

namespace PL
{
    public partial class LobbyScreen : Window
    {
        public string IdField { get; set; } = "";
        public string passwordField { get; set; } = "";

        private static BlApi.IBl s_bl = BlApi.Factory.Get();

        public LobbyScreen()
        {
            InitializeComponent();
        }

        private void Login_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                string role = s_bl.Volunteer.Login(IdField, passwordField);
                if (role == "Admin")
                {
                    new AdminWindow().Show();
                    MainContent.Visibility = Visibility.Collapsed;
                    RoleSelection.Visibility = Visibility.Visible;
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
            MessageBox.Show("Admin selected!");
            new AdminWindow().Show();
            RoleSelection.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
        }

        private void Volunteer_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Volunteer selected!");
            new VolunteerLobbyWindow(int.Parse(IdField)).Show();
            RoleSelection.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
        }
    }
}
