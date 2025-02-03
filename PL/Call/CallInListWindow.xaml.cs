using System.Windows;
using System.Windows.Threading;

namespace PL.Call;

public partial class CallInListWindow : Window
{
    // Static reference to the business logic layer (BL)
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    // Constructor to initialize the window with user ID and optional filter by call status
    public CallInListWindow(int userId, BO.CallStatus? requstedCallByStatus = null)
    {
        RequestedSpecialMode = requstedCallByStatus;
        UserId = userId;
        RefreshList(); // Refresh the list on initialization
        InitializeComponent();
    }

    #region Regular Properties
    // User ID for filtering
    public int UserId { get; set; }

    // Requested call status filter (optional)
    public BO.CallStatus? RequestedSpecialMode { get; set; }

    // Dispatcher operation for handling UI updates on the UI thread
    private volatile DispatcherOperation? _observerOperation = null; // stage 7
    #endregion

    #region Dependency Properties
    // Selected call from the list (bindable property)
    public BO.CallInList? SelectedCall
    {
        get => (BO.CallInList?)GetValue(SelectedCallProperty);
        set => SetValue(SelectedCallProperty, value);
    }

    // DependencyProperty for SelectedCall
    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.CallInList), typeof(CallInListWindow), new PropertyMetadata(null));

    // Filter by value (bindable property)
    public string? FilterByValue
    {
        get => (string?)GetValue(FilterByValueProperty);
        set => SetValue(FilterByValueProperty, value);
    }

    // DependencyProperty for FilterByValue
    public static readonly DependencyProperty FilterByValueProperty =
        DependencyProperty.Register("FilterByValue", typeof(string), typeof(CallInListWindow), new PropertyMetadata(null));

    // Filter by field (bindable property)
    public BO.CallInListFields? FilterByField
    {
        get => (BO.CallInListFields?)GetValue(FilterByFieldProperty);
        set => SetValue(FilterByFieldProperty, value);
    }

    // DependencyProperty for FilterByField
    public static readonly DependencyProperty FilterByFieldProperty =
        DependencyProperty.Register("FilterByField", typeof(BO.CallInListFields?), typeof(CallInListWindow), new PropertyMetadata(null));

    // Sort by field (bindable property)
    public BO.CallInListFields? SortByField
    {
        get => (BO.CallInListFields?)GetValue(SortByFieldProperty);
        set => SetValue(SortByFieldProperty, value);
    }

    // DependencyProperty for SortByField
    public static readonly DependencyProperty SortByFieldProperty =
        DependencyProperty.Register("SortByField", typeof(BO.CallInListFields?), typeof(CallInListWindow), new PropertyMetadata(null));

    // List of calls (bindable property)
    public IEnumerable<BO.CallInList> ListOfCalls
    {
        get => (IEnumerable<BO.CallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }

    // DependencyProperty for ListOfCalls
    private static DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

    #endregion

    #region Events
    // Event handler to show the call details window when a call is selected
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();

    // Event handler to delete the current assignment
    private void OnDeleteCurrentAssignment(object sender, RoutedEventArgs e)
    {
        try
        {
            // Check if a call is selected
            if (SelectedCall is null)
                throw new Exception("PL: The selected call is null");

            // Cancel the call and send email
            s_bl.Call.CancleCallSendEmailAsync(SelectedCall);

            // Cancel the assignment if the selected call has an ID
            if (SelectedCall?.Id is not null)
                s_bl.Call.CancelAssignement(UserId, (int)SelectedCall.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message); // Show error message if something goes wrong
        }
    }

    // Event handler to delete a call request
    private void OnDeleteCall(object sender, RoutedEventArgs e)
    {
        try
        {
            // Check if a call is selected
            if (SelectedCall is null)
                throw new Exception("PL: The selected call is null");

            // Delete the selected call request
            s_bl.Call.DeleteCallRequest(SelectedCall.CallId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message); // Show error message if something goes wrong
        }
    }

    // Event handler to remove the observer when the window is closed
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefreshList);

    // Event handler to add the observer when the window is loaded
    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefreshList);

    // Event handler to apply the filter settings
    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();

    // Event handler to handle sorting changes
    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();

    // Event handler to add a new call
    private void OnAddCall(object sender, RoutedEventArgs e) => new CallWindow(-1).Show();

    // Event handler to reset the filter and sort parameters
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        SortByField = null;
        FilterByField = null;
        FilterByValue = null;
        SelectedCall = null;
        RefreshList(); // Refresh the list after resetting
    }

    // Event handler to apply the filter and sorting settings
    private void OnApplyFilterAndSort(object sender, RoutedEventArgs e) => RefreshList();

    #endregion

    #region Methods
    // Method to refresh the list of calls based on current filter and sorting settings
    private void RefreshList()
    {
        // Check if the operation has completed or hasn't started
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        {
            // Start a new operation to refresh the list
            _observerOperation = Dispatcher.BeginInvoke(async () =>
            {
                var _FilterByField = FilterByField;
                var _FilterByValue = FilterByValue;
                var _SortByField = SortByField;
                var _RequestedSpecialMode = RequestedSpecialMode is null
                        ? null
                        : s_bl.Call.GetListOfCalls(BO.CallInListFields.Status, RequestedSpecialMode!, null);

                // Get the list of calls based on the filter, sorting, and special mode
                var calls = await Task.Run(() => s_bl.Call.GetListOfCalls(
                    _FilterByField,
                    _FilterByValue,
                    _SortByField,
                    _RequestedSpecialMode is null
                        ? null
                        : s_bl.Call.GetListOfCalls(BO.CallInListFields.Status, RequestedSpecialMode!, null)
                    ));

                // Update the list of calls in the UI
                ListOfCalls = calls;
            });
        }
    }
    #endregion
}
