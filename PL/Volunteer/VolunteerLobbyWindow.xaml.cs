using PL.Call;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Volunteer;
public partial class VolunteerLobbyWindow : Window
{
    private readonly static BlApi.IBl s_bl = BlApi.Factory.Get();

    // Constructor initializes the window with a given volunteer ID
    public VolunteerLobbyWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        Observer(); // Start observing updates
        InitializeComponent();
    }

    #region Dependency Properties

    // Holds the current volunteer's data
    public BO.Volunteer CurrentVolunteer
    {
        get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    private static readonly DependencyProperty CurrentVolunteerProperty
        = DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerLobbyWindow));

    // UserControl for displaying call details
    public UserControl CallDetailsContent
    {
        get { return (UserControl)GetValue(CallDetailsContentProperty); }
        set { SetValue(CallDetailsContentProperty, value); }
    }

    public static readonly DependencyProperty CallDetailsContentProperty =
        DependencyProperty.Register("CallDetailsContent", typeof(UserControl), typeof(VolunteerLobbyWindow));

    // UserControl for displaying the route map
    public UserControl RouteMap
    {
        get { return (UserControl)GetValue(RouteMapProperty); }
        set { SetValue(RouteMapProperty, value); }
    }

    public static readonly DependencyProperty RouteMapProperty =
        DependencyProperty.Register("RouteMap", typeof(UserControl), typeof(VolunteerLobbyWindow));

    // Holds the main description text
    public string DescriptionText
    {
        get => (string)GetValue(DescriptionTextProperty);
        set => SetValue(DescriptionTextProperty, value);
    }

    private static readonly DependencyProperty DescriptionTextProperty
        = DependencyProperty.Register("DescriptionText", typeof(string), typeof(VolunteerLobbyWindow));

    // Holds the warning text for selecting a call
    public string WarrningSelectCallText
    {
        get => (string)GetValue(WarrningSelectCallTextProperty);
        set => SetValue(WarrningSelectCallTextProperty, value);
    }

    private static readonly DependencyProperty WarrningSelectCallTextProperty
        = DependencyProperty.Register("WarrningSelectCallText", typeof(string), typeof(VolunteerLobbyWindow));

    // Header text displayed in the window
    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    private static readonly DependencyProperty HeaderTextProperty
        = DependencyProperty.Register("HeaderText", typeof(string), typeof(VolunteerLobbyWindow));

    // Holds details about the current call
    public string CallDetails
    {
        get { return (string)GetValue(CallDetailsProperty); }
        set { SetValue(CallDetailsProperty, value); }
    }

    public static readonly DependencyProperty CallDetailsProperty =
        DependencyProperty.Register("CallDetails", typeof(string), typeof(VolunteerLobbyWindow));

    #endregion

    #region Regular Properties
    public int VolunteerId { get; set; }
    private volatile DispatcherOperation? _observerOperation = null;
    #endregion

    #region Events

    // Opens a window to select a call
    private void OnSelectCall(object sender, RoutedEventArgs e) => new OpenCallListWindow(VolunteerId).Show();

    // Cancels the current assigned call
    private void OnCancelCall(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.CancelAssignement(VolunteerId, CurrentVolunteer.CurrentCall!.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    // Marks the current call as finished
    private void OnFinishCall(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.FinishAssignement(VolunteerId, CurrentVolunteer.CurrentCall!.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    // Opens the settings window
    private void OnShowSettingsWindow(object sender, RoutedEventArgs e) => new VolunteerWindow(VolunteerId, BO.UserRole.Admin).Show();

    // Removes observers when the window is closed
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(Observer);
        s_bl.Call.RemoveObserver(Observer);
    }

    // Adds observers when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(Observer);
        s_bl.Call.AddObserver(Observer);
    }

    // Opens a window displaying call history
    private void OnShowHistory(object sender, RoutedEventArgs e) => new ClosedCallListWindow(VolunteerId).Show();

    #endregion

    #region Methods

    private void Observer() => _ = RefershWindowDetails();

    // Refreshes window details asynchronously
    private async Task RefershWindowDetails()
    {
        try
        {
            var count = await Task.Run(() => {
                try
                {
                    return s_bl.Call.GetOpenCallsForVolunteer(VolunteerId, null, null).Count();
                }
                catch (Exception)
                {
                    return -1;
                }
            });

            if (count == -1) // Error handling if the volunteer is not found
            {
                MessageBox.Show($"The Volunteer is not found {VolunteerId}");
                Close();
                return;
            }

            // Ensures only one dispatcher operation is running at a time
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Close();
                    }

                    HeaderText = $"Welcome {CurrentVolunteer.FullName}";
                    if (CurrentVolunteer.CurrentCall is not null)
                    {
                        CallDetails = $"Route: {CurrentVolunteer.RangeType}\nETA: 31 Minutes";
                        List<(double, double)> listOfCorinates = new();
                        if (CurrentVolunteer.FullName is not null)
                            listOfCorinates.Add(((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
                        var call = s_bl.Call.GetDetielsOfCall(CurrentVolunteer.CurrentCall.CallId);
                        listOfCorinates.Add(((double)call.Latitude!, (double)call.Longitude!));
                        RouteMap = new DisplayMapContent(TypeOfMap.Route, CurrentVolunteer.RangeType, listOfCorinates);
                        CallDetailsContent = new CallDetailsControl(call);
                    }
                    else
                    {
                        RouteMap = null;
                        CallDetailsContent = null;
                        WarrningSelectCallText = CurrentVolunteer.IsActive ? "" : "Only online volunteers can take calls. Please activate this user in the settings to select a call.";
                        DescriptionText = $"Currently there are {count} calls open. Would you like to take one?";
                    }
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Close();
        }
    }
    #endregion
}
