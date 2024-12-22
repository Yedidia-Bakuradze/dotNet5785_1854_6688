using System.Windows;

namespace PL;


public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MainWindow()
    {
        InitializeComponent();
    }

    #region Dependency Variables and registration
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
    public TimeSpan CurrentRiskRange
    {
        get => (TimeSpan)GetValue(CurrentRiskRangeProperty);
        set => SetValue(CurrentRiskRangeProperty, value);
    }
    public static readonly DependencyProperty CurrentRiskRangeProperty
        = DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow));

    #endregion
    
    #region Clock & Config Update Methods
    private void OnClockForwardOneSecondUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Second);
    private void OnClockForwardOneMinuteUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Minute);
    private void OnClockForwardOneHourUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Hour);
    private void OnClockForwardOneDayUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Day);
    private void OnClockForwardOneYearUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Year);
    
    /// <summary>
    /// Triggered when user clicks the update risk range button and it updates the risk range variable with the current modified value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRiskRagneUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.SetRiskRange(CurrentRiskRange);

    /// <summary>
    /// This method is invoked when the clock is changed
    /// </summary>
    private void clockObserver()
    {
        CurrentTime = s_bl.Admin.GetClock();
    }
    
    /// <summary>
    /// This method is invoked when the configuration variables are changed
    /// </summary>
    private void configObserver()
    {
        CurrentRiskRange = s_bl.Admin.GetRiskRange();

    }
    #endregion

    #region Window Startup & Closing
    /// <summary>
    /// This method is invoked just when the Main Window is loaded
    /// It registeres the config variables' modification methods to be invoked when the clock and RiskRagne are modified
    /// </summary>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        //Show the configuration variables' values
        CurrentTime = s_bl.Admin.GetClock();
        CurrentRiskRange = s_bl.Admin.GetRiskRange();

        //Adds the methods to invoke when the clock is changed
        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);
    }

    /// <summary>
    /// This method is invoked just when the Main Window is closed by the user
    /// It un-registeres the config variables' modification methods, and it shows a little message on the screen indicating that the use has closed the screent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWindowClosed(object sender, EventArgs e)
    {
        MessageBox.Show("The window is closed");
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }
    #endregion

    #region Other Events Methods
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
    #endregion
}