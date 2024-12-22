using System.Windows;

namespace PL;


public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
    #region Clock Update Methods
    private void btnAddOneSecond_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateClock(BO.TimeUnit.Second);
    }
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateClock(BO.TimeUnit.Minute);
    }
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateClock(BO.TimeUnit.Hour);
    }
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateClock(BO.TimeUnit.Day);
    }
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateClock(BO.TimeUnit.Year);
    }
    private void clockObserver()
    {
        CurrentTime = s_bl.Admin.GetClock();
    }
    private void configObserver()
    {
        CurrentRiskRange = s_bl.Admin.GetRiskRange();

    }   
    #endregion
    public TimeSpan CurrentRiskRange
    {
        get => (TimeSpan)GetValue(CurrentRiskRangeProperty);
        set => SetValue(CurrentRiskRangeProperty, value);
    }
    public static readonly DependencyProperty CurrentRiskRangeProperty = DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow));
    public MainWindow()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Triggered when user clicks the update risk range button and it updates the risk range variable with the current modified value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClick_RiskRangeUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.SetRiskRange(CurrentRiskRange);
    /// <summary>
    /// </summary>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock();

        CurrentRiskRange = s_bl.Admin.GetRiskRange();

        s_bl.Admin.AddClockObserver(clockObserver);

        s_bl.Admin.AddConfigObserver(configObserver);
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        MessageBox.Show("The window is closed");
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    /// <summary>
    /// This method is triggered when the user clicks on the Show List of Volunteer in List and it opens a new window which shows the volunteer is list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShowListOfVolunteerInList(object sender, RoutedEventArgs e) => new Volunteer.VolunteerListWindow().Show();

    /// <summary>
    /// This method initializes the databse only if the user has clicked on the Yes button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSystemInitialize(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Do you want to initialize the system?", "System Initialize",MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
            return;
        s_bl.Admin.DbInit();
    }

    /// <summary>
    /// This methdo reset the database and the system only if user click on the Yes button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSyetmReset(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Do you want to reset the system?", "System Reset", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
            return;
        s_bl.Admin.DbReset();
    }
}