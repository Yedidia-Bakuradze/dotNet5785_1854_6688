using PL.Admin;
using PL.Sub_Windows;
using PL.Volunteer;
using System.Windows;
namespace PL;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        //Loads the lobby window instead
        new VolunteerMapDetailsControl().Show();
        //new AdminWindow(332461854).Show();
        //new LobbyScreen().Show();
        //new VolunteerLobbyWindow(199089079).Show();
        this.Close();
    }
}