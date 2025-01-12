using System.Windows;
namespace PL;

/// <summary>
/// Interaction logic for LobbyScreen.xaml
/// </summary>
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
            string role = s_bl.Volunteer.Login(IdField!, passwordField);
            if (role == "Volunteer")
            {
                // TODO: send to volunteer page
            }
            else if (role == "Admin")
            {

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
