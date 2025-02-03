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
    // The Start point of the program
    public MainWindow()
    {
        new LobbyScreen().Show();
        //new AdminWindow(332461854).Show();//We used that for reduce the time for login each time
        //new VolunteerLobbyWindow(332461854).Show(); //Same as above
        this.Close();
    }
}