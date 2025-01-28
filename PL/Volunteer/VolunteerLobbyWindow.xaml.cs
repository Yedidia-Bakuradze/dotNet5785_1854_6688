using PL.Call;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Volunteer;
public partial class VolunteerLobbyWindow : Window
{
    private readonly static BlApi.IBl s_bl = BlApi.Factory.Get();
    public VolunteerLobbyWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        try
        {
            RefershWindowDetails();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            this.Close();
        }
        InitializeComponent();
    }

    #region Dependecy Propeties
    public BO.Volunteer CurrentVolunteer 
    {
        get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty,value);
    }

    private static readonly DependencyProperty CurrentVolunteerProperty
        = DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerLobbyWindow));



    public UserControl CallDetailsContent
    {
        get { return (UserControl)GetValue(CallDetailsContentProperty); }
        set { SetValue(CallDetailsContentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallDetailsContent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallDetailsContentProperty =
        DependencyProperty.Register("CallDetailsContent", typeof(UserControl), typeof(VolunteerLobbyWindow));


    public UserControl RouteMap
    {
        get { return (UserControl)GetValue(RouteMapProperty); }
        set { SetValue(RouteMapProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RouteMap.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RouteMapProperty =
        DependencyProperty.Register("RouteMap", typeof(UserControl), typeof(VolunteerLobbyWindow));

    public string DescriptionText
    {
        get => (string)GetValue(DescriptionTextProperty);
        set => SetValue(DescriptionTextProperty, value);
    }

    private static readonly DependencyProperty DescriptionTextProperty
        = DependencyProperty.Register("DescriptionText", typeof(string), typeof(VolunteerLobbyWindow));

    public string WarrningSelectCallText
    {
        get => (string)GetValue(WarrningSelectCallTextProperty);
        set => SetValue(WarrningSelectCallTextProperty, value);
    }

    private static readonly DependencyProperty WarrningSelectCallTextProperty
        = DependencyProperty.Register("WarrningSelectCallText", typeof(string), typeof(VolunteerLobbyWindow));

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    private static readonly DependencyProperty HeaderTextProperty
        = DependencyProperty.Register("HeaderText", typeof(string), typeof(VolunteerLobbyWindow));



    public string CallDetails
    {
        get { return (string)GetValue(CallDetailsProperty); }
        set { SetValue(CallDetailsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallDetails.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallDetailsProperty =
        DependencyProperty.Register("CallDetails", typeof(string), typeof(VolunteerLobbyWindow));


    #endregion

    #region Regular Propeties
    public int VolunteerId { get; set; }
    private volatile DispatcherOperation? _observerOperation = null;
    #endregion

    #region Events
    private void OnSelectCall(object sender, RoutedEventArgs e) => new OpenCallListWindow(VolunteerId).Show();
    private void OnCancelCall(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.CancelAssignement(VolunteerId,CurrentVolunteer.CurrentCall!.CallId);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
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
    private void OnShowSettingsWindow(object sender, RoutedEventArgs e) => new VolunteerWindow(VolunteerId, BO.UserRole.Admin).Show();
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(Observer);
        s_bl.Call.RemoveObserver(Observer);
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(Observer);
        s_bl.Call.AddObserver(Observer);
    }
    private void OnShowHistory(object sender, RoutedEventArgs e) => new ClosedCallListWindow(VolunteerId).Show();
    #endregion

    #region Methods
    private void Observer() => RefershWindowDetails();
    private async Task RefershWindowDetails()
    {
        var count = await Task.Run(()=> s_bl.Call.GetOpenCallsForVolunteer(VolunteerId, null, null).Count());

        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        {
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                HeaderText = $"Welcome {CurrentVolunteer.FullName}";
                if(CurrentVolunteer.CurrentCall is not null)
                {

                    CallDetails = $"Route: {CurrentVolunteer.RangeType}\nETA: 31 Minutes";
                    List<(double, double)> listOfCorinates = new();
                    if(CurrentVolunteer.FullName is not null)
                        listOfCorinates.Add(((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
                    var call = s_bl.Call.GetDetielsOfCall(CurrentVolunteer.CurrentCall.CallId);
                    listOfCorinates.Add((call.Latitude,call.Longitude));
                    RouteMap = new DisplayMapContent(TypeOfMap.Route,CurrentVolunteer.RangeType,listOfCorinates);
                    CallDetailsContent = new CallDetailsControl(call);
                }
                else
                {
                    RouteMap = null;
                    CallDetailsContent = null;
                    WarrningSelectCallText = CurrentVolunteer.IsActive ? "" : "Only online volunteers can take calls   Please activate this user in the settings to select a call";
                    DescriptionText = $"Currently there are {count} calls open Would you like to take one?";
                }
            });
        }
    }

    #endregion

}
