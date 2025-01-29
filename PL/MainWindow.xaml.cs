using PL.Admin;
using PL.Volunteer;
using System.Windows;
namespace PL;

/// <summary>
/// To login into the system here some credentials:
/// Id:332461854
/// Password: yedidiaYY12@!
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        //new LobbyScreen().Show();
        new AdminWindow(332461854).Show();
        new VolunteerLobbyWindow(332461854).Show();
        this.Close();
    }
}