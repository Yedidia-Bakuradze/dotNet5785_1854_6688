using PL.Admin;
using PL.Volunteer;
using System.Windows;
namespace PL;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        //Loads the lobby window instead
        //new AdminWindow().Show();
        new LobbyScreen().Show();
        //new VolunteerLobbyWindow().Show();
        this.Close();
    }
}