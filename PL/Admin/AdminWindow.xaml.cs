using System.Windows;

namespace PL.Admin;

public partial class AdminWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public AdminWindow()
    {
        InitializeComponent();
    }






    #region Dependency Propeties
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

    //Bottom buttons' text dependecy variables
    public string ExpieredBtnText {
        get => (string)GetValue(ExpieredBtnTextProperty); 
        set => SetValue(ExpieredBtnTextProperty,value);
    }
    public static readonly DependencyProperty ExpieredBtnTextProperty = 
        DependencyProperty.Register("ExpieredBtnText",typeof(string),typeof(AdminWindow));
    
    public string OpenInRiskBtnText {
        get => (string)GetValue(OpenInRiskBtnTextProperty); 
        set => SetValue(OpenInRiskBtnTextProperty,value);
    }
    public static readonly DependencyProperty OpenInRiskBtnTextProperty = 
        DependencyProperty.Register("OpenInRiskBtnText",typeof(string),typeof(AdminWindow));
    
    public string OpenBtnText {
        get => (string)GetValue(OpenBtnTextProperty); 
        set => SetValue(OpenBtnTextProperty,value);
    }
    public static readonly DependencyProperty OpenBtnTextProperty = 
        DependencyProperty.Register("OpenBtnText",typeof(string),typeof(AdminWindow));
    
    public string InProgressBtnText {
        get => (string)GetValue(InProgressBtnTextProperty); 
        set => SetValue(InProgressBtnTextProperty,value);
    }
    public static readonly DependencyProperty InProgressBtnTextProperty = 
        DependencyProperty.Register("InProgressBtnText",typeof(string),typeof(AdminWindow));
    
    public string InProgressInRiskBtnText {
        get => (string)GetValue(InProgressInRiskBtnTextProperty); 
        set => SetValue(InProgressInRiskBtnTextProperty,value);
    }
    public static readonly DependencyProperty InProgressInRiskBtnTextProperty = 
        DependencyProperty.Register("InProgressInRiskBtnText",typeof(string),typeof(AdminWindow));
    
    public string ClosedBtnText {
        get => (string)GetValue(ClosedBtnTextProperty); 
        set => SetValue(ClosedBtnTextProperty,value);
    }
    public static readonly DependencyProperty ClosedBtnTextProperty = 
        DependencyProperty.Register("ClosedBtnText", typeof(string), typeof(AdminWindow));


    #endregion


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

    #region Event Methods

    #region Clock Update Methods
    private void OnClockForwardOneSecondUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Second);
    private void OnClockForwardOneMinuteUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Minute);
    private void OnClockForwardOneHourUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Hour);
    private void OnClockForwardOneDayUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Day);
    private void OnClockForwardOneYearUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.UpdateClock(BO.TimeUnit.Year);

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
        MessageBoxResult result = MessageBox.Show("Do you want to initialize the system?", "System Initialize", MessageBoxButton.YesNo);
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
    
    /// <summary>
    /// This method is trigged when the user resets the system clock
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClockResetAction(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("implement OnClockResetAction");
    }
    #endregion

    #endregion
}