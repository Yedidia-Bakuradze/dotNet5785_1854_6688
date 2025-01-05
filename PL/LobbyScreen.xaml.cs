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
        s_bl.Volunteer.Login(IdField, passwordField);
        InitializeComponent();
    }

    private void Login_Button(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(IdField);
    }


}
