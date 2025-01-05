using PL.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for LobbyScreen.xaml
/// </summary>
public partial class LobbyScreen : Window
{
    public string IdField { get; set; }
    public string passwordField { get; set; }

    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    public LobbyScreen()
    {

        InitializeComponent();

    }

    private void Login_Button(object sender, RoutedEventArgs e)
    {
        try
        {
            string role = s_bl.Volunteer.Login(IdField!, passwordField);
            if (role == "Volunteer")
            {
                // TODO: send to volunteer page
            }
            else if (role == "Admin")
            {
                MessageBoxResult result = MessageBox.Show("Do you want to go to the admin page?", "Choose Page", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    new AdminWindow().Show();
                    this.Close();
                }
                else if (result == MessageBoxResult.No)
                {
                    // TODO: send to volunteer page
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


}
