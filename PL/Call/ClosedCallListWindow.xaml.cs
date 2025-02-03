using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call;

public partial class ClosedCallListWindow : Window
{
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Initialize the business logic layer
    public ClosedCallListWindow(int volunteerId)
    {
        VolunteerId = volunteerId; // Set the volunteer ID for this window
        RefereshList(); // Refresh the list of closed calls when the window is initialized
        InitializeComponent(); // Initialize the UI components
    }

    #region Regular Properties
    public int VolunteerId { get; set; } // Property to store the volunteer ID
    private volatile DispatcherOperation? _observerOperation = null; // Used to track the dispatcher operation for asynchronous tasks
    public BO.Volunteer? CurrentVolunteer { get; set; } // Property to store the current volunteer details

    #endregion

    #region Dependency Properties
    // Dependency property for ListOfCalls, used for data binding
    public IEnumerable<BO.ClosedCallInList> ListOfCalls
    {
        get => (IEnumerable<BO.ClosedCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }

    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(ClosedCallListWindow));

    // Dependency property for MapView, used for displaying a map
    public UserControl MapView
    {
        get { return (UserControl)GetValue(MapViewProperty); }
        set { SetValue(MapViewProperty, value); }
    }

    public static readonly DependencyProperty MapViewProperty =
        DependencyProperty.Register("MapView", typeof(UserControl), typeof(ClosedCallListWindow));

    // Dependency property for FilterValue, used to filter the calls
    public BO.CallType? FilterValue
    {
        get { return (BO.CallType?)GetValue(FilterValueProperty); }
        set { SetValue(FilterValueProperty, value); }
    }

    public static readonly DependencyProperty FilterValueProperty =
        DependencyProperty.Register("FilterValue", typeof(BO.CallType?), typeof(ClosedCallListWindow));

    // Dependency property for SortField, used to sort the list of calls
    public BO.ClosedCallInListFields? SortField
    {
        get { return (BO.ClosedCallInListFields?)GetValue(SortFieldProperty); }
        set { SetValue(SortFieldProperty, value); }
    }

    public static readonly DependencyProperty SortFieldProperty =
        DependencyProperty.Register("SortField", typeof(BO.ClosedCallInListFields?), typeof(ClosedCallListWindow));

    // Dependency property for SelectedCall, used to store the selected call in the list
    public BO.ClosedCallInList SelectedCall
    {
        get { return (BO.ClosedCallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }

    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.ClosedCallInList), typeof(ClosedCallListWindow));
    #endregion

    #region Events
    // Event handler when the window is closed, removes observer for updates
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefereshList);

    // Event handler when the window is loaded, adds observer for updates
    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefereshList);

    // Event handler when the filter value is changed, refreshes the list
    private void OnFilterValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefereshList();

    // Event handler for filtering and sorting actions, refreshes the list
    private void OnFliterAndSort(object sender, RoutedEventArgs e) => RefereshList();

    // Event handler for resetting the filter and sort parameters
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        FilterValue = null; // Clear filter value
        SortField = null; // Clear sort field
    }
    #endregion

    #region Methods
    // Method to refresh the list of closed calls
    private void RefereshList()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    // Get the details of the current volunteer
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions by showing an error message and closing the window
                    MessageBox.Show(ex.Message);
                    Close();
                }
                // Get the list of closed calls for the volunteer with the selected filter and sort options
                ListOfCalls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, FilterValue, SortField);

                // Convert the list of calls into coordinates for displaying on the map
                List<(double, double)> listOfCordinates = s_bl.Call.ConvertClosedCallsIntoCordinates(ListOfCalls).ToList();

                // If the volunteer's address is available, add it to the list of coordinates
                if (CurrentVolunteer?.FullCurrentAddress is not null)
                {
                    listOfCordinates.Insert(0, ((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
                    MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfCordinates);
                }
                else
                {
                    // If the address is not available, display the map with just the call coordinates
                    MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfCordinates);
                }
            });
    }
    #endregion

}
