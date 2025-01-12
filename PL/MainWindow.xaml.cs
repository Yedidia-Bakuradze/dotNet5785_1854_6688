using PL.Volunteer;
using System.Windows;
namespace PL;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        //Loads the lobby window instead
        new VolunteerListWindow().Show();
        this.Close();
    }
}