using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call;

public partial class OpenCallWindow : Window
{
    // Constructor that initializes the CallId and VolunteerId, and reloads the screen with necessary data
    public OpenCallWindow(int callId, int volunteerId)
    {
        CallId = callId;
        VolunteerId = volunteerId;
        ReloadScreen();
        InitializeComponent();
    }

    #region Propeties
    // The BlApi object for accessing the business logic layer
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Properties to store the CallId and VolunteerId
    public int CallId { get; set; }
    public int VolunteerId { get; set; }

    // A volatile property for the dispatcher operation to handle UI updates in a thread-safe manner
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    #endregion

    #region Events
    // Event handler that adds observers when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer);
    }

    // Event handler that removes observers when the window is closed
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(Observer);
        s_bl.Volunteer.RemoveObserver(Observer);
    }
    #endregion

    #region Dependecy Propeties
    // Dependency property to store the details of the current call
    public BO.Call CurrentCall
    {
        get => (BO.Call)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty, value);
    }
    private static readonly DependencyProperty CurrentCallProperty
        = DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(OpenCallWindow));

    // Dependency property to store the details of the current volunteer
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(OpenCallWindow));

    // Dependency property for displaying the map view
    public UserControl MapView
    {
        get { return (UserControl)GetValue(MapViewProperty); }
        set { SetValue(MapViewProperty, value); }
    }
    public static readonly DependencyProperty MapViewProperty =
        DependencyProperty.Register("MapView", typeof(UserControl), typeof(OpenCallWindow));

    #endregion

    #region Methods
    // Observer method to refresh the screen when data changes
    private void Observer() => _ = ReloadScreen();

    // Method to reload the screen with updated details for the call and volunteer
    private async Task ReloadScreen()
    {
        try
        {
            // Fetch the call and volunteer details asynchronously
            CurrentCall = await Task.Run(() => s_bl.Call.GetDetielsOfCall(CallId));
            CurrentVolunteer = await Task.Run(() => s_bl.Volunteer.GetVolunteerDetails(VolunteerId));
        }
        catch (Exception ex)
        {
            // Display error message if an exception occurs
            MessageBox.Show(ex.Message);
            Close();
        }

        // If no latitude and longitude for the current call, return early
        if ((CurrentCall.Latitude, CurrentCall.Longitude) is (null, null))
            return;

        // Create a list of coordinates for the call's location
        List<(double, double)> listOfCordinates = [((double)CurrentCall.Latitude!, (double)CurrentCall.Longitude!)];

        // If the volunteer has a location, include it in the map's coordinates list
        if ((CurrentVolunteer.Latitude, CurrentVolunteer.Longitude) is not (null, null))
        {
            listOfCordinates.Insert(0, ((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
            // Create a map view showing multiple routes
            MapView = new DisplayMapContent(TypeOfMap.MultipleTypeOfRoutes, BO.TypeOfRange.WalkingDistance, listOfCordinates);
        }
        else
        {
            // Create a map view showing only the call location
            MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.WalkingDistance, listOfCordinates);
        }
    }
    #endregion
}
