using System.Windows;
namespace PL.Call;

public partial class CallInProgressWindow : Window
{
    public CallInProgressWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        LoadScreen();
    }

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    public int VolunteerId { get; set; }

    #endregion

    #region Dependency Property
    public BO.CallInProgress CurrentCallInProgress
    {
        get => (BO.CallInProgress)GetValue(CurrentCallInProgressProperty);
        set => SetValue(CurrentCallInProgressProperty, value);
    }

    public static readonly DependencyProperty CurrentCallInProgressProperty
        = DependencyProperty.Register("CurrentCallInProgress", typeof(BO.CallInProgress), typeof(CallInProgressWindow), new PropertyMetadata(null));


    #endregion

    #region Events
    private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Volunteer.AddObserver(LoadScreen);

    private void Window_Closed(object sender, EventArgs e) => s_bl.Volunteer.RemoveObserver(LoadScreen);
    #endregion

    #region Methods
    private void LoadScreen()
    {
        try
        {
            CurrentCallInProgress = s_bl.Volunteer.GetVolunteerDetails(VolunteerId).CurrentCall
                ?? throw new Exception($"PL: The volunteer {VolunteerId} no longer has a call in progress. Existing Screen");
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            Close();
        }
    }
    #endregion
}
