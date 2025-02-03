using BO;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Converters;
using System.Windows.Threading;

namespace PL.Call;

public partial class OpenCallListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get(); // Static instance of the BL API
    public OpenCallListWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        ListOfCalls = null; // Initialize the list of calls to null
        _ = RefreshList(); // Refresh the list of open calls asynchronously
        InitializeComponent();
    }

    #region Regular Propeties
    public int VolunteerId { get; set; } // Property to store the volunteer ID
    private volatile DispatcherOperation? _observerOperation = null; // Used to manage the refresh operation status

    #endregion

    #region Dependecy Propeties
    // Property for sorting the calls
    public BO.OpenCallFields? SortByField
    {
        get { return (BO.OpenCallFields?)GetValue(SortByFieldProperty); }
        set { SetValue(SortByFieldProperty, value); }
    }
    public static readonly DependencyProperty SortByFieldProperty =
        DependencyProperty.Register("SortByField", typeof(BO.OpenCallFields?), typeof(OpenCallListWindow));

    // Property for filtering the calls by type
    public BO.CallType? FilterByValue
    {
        get { return (BO.CallType?)GetValue(FilterByValueProperty); }
        set { SetValue(FilterByValueProperty, value); }
    }
    public static readonly DependencyProperty FilterByValueProperty =
        DependencyProperty.Register("FilterByValue", typeof(BO.CallType?), typeof(OpenCallListWindow));

    // Property for filtering calls by requested special mode (status)
    public BO.CallStatus? RequestedSpecialMode
    {
        get { return (BO.CallStatus?)GetValue(RequestedSpecialModeProperty); }
        set { SetValue(RequestedSpecialModeProperty, value); }
    }
    public static readonly DependencyProperty RequestedSpecialModeProperty =
        DependencyProperty.Register("RequestedSpecialMode", typeof(BO.CallStatus?), typeof(OpenCallListWindow));

    // Property for selecting a specific call from the list
    public BO.OpenCallInList? SelectedCall
    {
        get { return (BO.OpenCallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }
    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(OpenCallListWindow));

    // Property to hold the list of open calls
    public IEnumerable<BO.OpenCallInList> ListOfCalls
    {
        get => (IEnumerable<BO.OpenCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }
    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallListWindow), new PropertyMetadata(null));

    // Property to display the map for open calls
    public UserControl OpenCallsMap
    {
        get { return (UserControl)GetValue(OpenCallsMapProperty); }
        set { SetValue(OpenCallsMapProperty, value); }
    }
    public static readonly DependencyProperty OpenCallsMapProperty =
        DependencyProperty.Register("OpenCallsMap", typeof(UserControl), typeof(OpenCallListWindow));
    #endregion

    #region Events
    // Event to show the call window when a call is clicked
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();

    // Event handler for when the window is closed, removing observer for call updates
    private void OnWindowClosed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer); // Add observer to volunteer updates
    }

    // Event handler for when the window is loaded, adding observers for call and volunteer updates
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer);
    }

    // Event handler for when filter parameters are set, refreshes the call list
    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();

    // Event handler for when sorting changes, refreshes the call list
    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();

    // Event handler for when a selection is changed in the list, opens the selected call window
    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (SelectedCall is not null)
            new OpenCallWindow(SelectedCall!.CallId, VolunteerId).Show();
    }

    // Event handler for selecting a call to do, updates the call status and closes the window
    private void OnSelectCallTodo(object sender, RoutedEventArgs e)
    {
        s_bl.Call.SelectCallToDo(VolunteerId, SelectedCall!.CallId);
        Close();
    }

    // Event handler for applying filter and sort parameters, refreshes the call list
    private void OnFliterAndSort(object sender, RoutedEventArgs e) => RefreshList();

    // Event handler for resetting filter and sort parameters to null and refreshing the call list
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        FilterByValue = null;
        SortByField = null;
        RefreshList();
    }
    #endregion

    #region Methods
    // Observer method to refresh the list of calls
    private void Observer() => _ = RefreshList();

    // Method to asynchronously refresh the list of open calls
    private async Task RefreshList()
    {
        var _volunteerId = VolunteerId;
        var _filterByValue = FilterByValue;
        var _sortByField = SortByField;

        // Retrieve the list of open calls for the volunteer based on the filters and sort field
        IEnumerable<OpenCallInList> calls = await Task.Run(() => s_bl.Call.GetOpenCallsForVolunteer(_volunteerId, _filterByValue, _sortByField));

        // Convert the open calls into coordinates for map display
        var _listOfCordinates = await Task.Run(() => s_bl.Call.ConvertOpenCallsToCordinates(calls).ToList());

        // Refresh the UI with the new data
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var volunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                    ListOfCalls = calls; // Set the list of open calls

                    List<(double, double)> listOfCordinates = _listOfCordinates;

                    // Check if the volunteer has a valid location and display the map accordingly
                    if ((volunteer.Latitude, volunteer.Longitude) is not (null, null))
                    {
                        listOfCordinates.Insert(0, ((double)volunteer.Latitude!, (double)volunteer.Longitude!));
                        if (listOfCordinates.Count != 0)
                            OpenCallsMap = new DisplayMapContent(TypeOfMap.MultipleTypeOfRoutes, volunteer.RangeType, listOfCordinates);
                        else
                            OpenCallsMap = new DisplayMapContent(TypeOfMap.Pin, volunteer.RangeType, listOfCordinates);
                    }
                    else
                        OpenCallsMap = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.WalkingDistance, listOfCordinates);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Show error if any exception occurs
                    Close(); // Close the window on error
                }
            });

    }
    #endregion
}
