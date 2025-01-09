using PL.Admin;
using System.Windows;
namespace PL;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        //Loads the lobby window instead
        new AdminWindow().Show();
        this.Close();
    }
}