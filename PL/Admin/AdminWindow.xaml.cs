using BO;
using System.Windows;

namespace PL.Admin;

public partial class AdminWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public AdminWindow()
    {
        InitializeComponent();
        UpdateAllBottomButtonTexts();
    }

    #region Dependency Propeties
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    private static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminWindow));

    public TimeSpan CurrentRiskRange
    {
        get => (TimeSpan)GetValue(CurrentRiskRangeProperty);
        set => SetValue(CurrentRiskRangeProperty, value);
    }
    private static readonly DependencyProperty CurrentRiskRangeProperty
        = DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(AdminWindow));

    //Bottom buttons' text dependecy variables
    public string ExpieredBtnText
    {
        get => (string)GetValue(ExpieredBtnTextProperty);
        set => SetValue(ExpieredBtnTextProperty, value);
    }
    private static readonly DependencyProperty ExpieredBtnTextProperty =
        DependencyProperty.Register("ExpieredBtnText", typeof(string), typeof(AdminWindow));

    public string OpenInRiskBtnText
    {
        get => (string)GetValue(OpenInRiskBtnTextProperty);
        set => SetValue(OpenInRiskBtnTextProperty, value);
    }
    private static readonly DependencyProperty OpenInRiskBtnTextProperty =
        DependencyProperty.Register("OpenInRiskBtnText", typeof(string), typeof(AdminWindow));

    public string OpenBtnText
    {
        get => (string)GetValue(OpenBtnTextProperty);
        set => SetValue(OpenBtnTextProperty, value);
    }
    private static readonly DependencyProperty OpenBtnTextProperty =
        DependencyProperty.Register("OpenBtnText", typeof(string), typeof(AdminWindow));

    public string InProgressBtnText
    {
        get => (string)GetValue(InProgressBtnTextProperty);
        set => SetValue(InProgressBtnTextProperty, value);
    }
    private static readonly DependencyProperty InProgressBtnTextProperty =
        DependencyProperty.Register("InProgressBtnText", typeof(string), typeof(AdminWindow));

    public string InProgressInRiskBtnText
    {
        get => (string)GetValue(InProgressInRiskBtnTextProperty);
        set => SetValue(InProgressInRiskBtnTextProperty, value);
    }
    private static readonly DependencyProperty InProgressInRiskBtnTextProperty =
        DependencyProperty.Register("InProgressInRiskBtnText", typeof(string), typeof(AdminWindow));

    public string ClosedBtnText
    {
        get => (string)GetValue(ClosedBtnTextProperty);
        set => SetValue(ClosedBtnTextProperty, value);
    }
    private static readonly DependencyProperty ClosedBtnTextProperty =
        DependencyProperty.Register("ClosedBtnText", typeof(string), typeof(AdminWindow));

    public OperationSubScreenMode CurrentOperationSelected
    {
        get => (OperationSubScreenMode)GetValue(CurrentOperationSelectedProperty);
        set => SetValue(CurrentOperationSelectedProperty, value);
    }
    private static readonly DependencyProperty CurrentOperationSelectedProperty
        = DependencyProperty.Register("CurrentOperationSelected", typeof(OperationSubScreenMode), typeof(AdminWindow));

    #endregion

    #region Observers
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

    #region Event

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
        s_bl.Admin.DbReset();
    }
    #endregion

    #endregion

    #region Other Methods

    /// <summary>
    /// Initializes the bottom button's texts including the count of number of assisiated calls
    /// </summary>
    private void UpdateAllBottomButtonTexts()
    {
        ExpieredBtnText = $"Expiered\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.Expiered.GetHashCode()]}";
        OpenBtnText = $"Open\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.Open.GetHashCode()]}";
        OpenInRiskBtnText = $"Risky & Open\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.OpenAndRisky.GetHashCode()]}";
        ClosedBtnText = $"Closed\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.Closed.GetHashCode()]}";
        InProgressBtnText = $"In Progress\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.InProgress.GetHashCode()]}";
        InProgressInRiskBtnText = $"Risky &\nIn Progress\nCount: {s_bl.Call.GetTotalCallsByStatus()[CallStatus.InProgressAndRisky.GetHashCode()  ]}";
    }

    /// <summary>
    /// This method is triggered when the user requests to pop up the Clock managment view, it would replace the old view with the current
    /// Or would close the screen if the same button been pressed twis
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShowClockManagerClick(object sender, RoutedEventArgs e) =>
        CurrentOperationSelected = (CurrentOperationSelected == OperationSubScreenMode.ClockManager)
            ? OperationSubScreenMode.Closed
            : OperationSubScreenMode.ClockManager;

    /// <summary>
    /// This method is triggered when the user requests to pop up the Risk Range managment view, it would replace the old view with the current
    /// Or would close the screen if the same button been pressed twis
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShowRiskRangeManagerClick(object sender, RoutedEventArgs e)
       => CurrentOperationSelected = (CurrentOperationSelected == OperationSubScreenMode.RiskRangeManager)
            ? OperationSubScreenMode.Closed
            : OperationSubScreenMode.RiskRangeManager;

    /// <summary>
    /// This method is triggered when the user requests to pop up the Syetm Actions managment view, it would replace the old view with the current
    /// Or would close the screen if the same button been pressed twis
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShowActionManagerClick(object sender, RoutedEventArgs e)
        => CurrentOperationSelected = (CurrentOperationSelected == OperationSubScreenMode.ActionManger)
            ? OperationSubScreenMode.Closed
            : OperationSubScreenMode.ActionManger;

    #endregion
    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are expiered
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnExpieredCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.Expiered);
    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are open
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>private void OnOpenInRiskCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.OpenAndRisky);
    private void OnOpenCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.Open);

    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are open and risky
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnOpenInRiskCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.OpenAndRisky);
    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are in progress and risky
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnInProgressInRiskCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.InProgressAndRisky);

    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are in progress 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnInProgressCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.InProgress);

    /// <summary>
    /// This method is invoked when the user wants to pop up a screen filled with calls that are closed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClosedCallsRequestClick(object sender, RoutedEventArgs e) => ShowListOfCalls(CallStatus.Closed);

    private void ShowListOfCalls(CallStatus? callStatus) => MessageBox.Show(callStatus.ToString()??"No Filter");//TODO: new CallInListWindow(callStatus).Show();
}