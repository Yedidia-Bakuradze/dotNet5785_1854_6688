using System.Windows;
using System.Windows.Threading;

namespace PL.Call;

public partial class CallInListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public CallInListWindow(int userId, BO.CallStatus? requstedCallByStatus = null)
    {
        RequestedSpecialMode = requstedCallByStatus;
        UserId = userId;
        RefreshList();
        InitializeComponent();
    }

    #region Regular Propeties
    public int UserId { get; set; }
    public BO.CallStatus? RequestedSpecialMode { get; set; }
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    #endregion
    #region Dependecy Propeties
    public BO.CallInList? SelectedCall
    {
        get => (BO.CallInList?)GetValue(SelectedCallProperty);
        set => SetValue(SelectedCallProperty, value);
    }

    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.CallInList), typeof(CallInListWindow), new PropertyMetadata(null));

    public string? FilterByValue
    {
        get => (string?)GetValue(FilterByValueProperty);
        set => SetValue(FilterByValueProperty, value);
    }

    public static readonly DependencyProperty FilterByValueProperty =
        DependencyProperty.Register("FilterByValue", typeof(string), typeof(CallInListWindow), new PropertyMetadata(null));

    public BO.CallInListFields? FilterByField
    {
        get => (BO.CallInListFields?)GetValue(FilterByFieldProperty);
        set => SetValue(FilterByFieldProperty, value);
    }

    public static readonly DependencyProperty FilterByFieldProperty =
        DependencyProperty.Register("FilterByField", typeof(BO.CallInListFields?), typeof(CallInListWindow), new PropertyMetadata(null));

    public BO.CallInListFields? SortByField
    {
        get => (BO.CallInListFields?)GetValue(SortByFieldProperty);
        set => SetValue(SortByFieldProperty, value);
    }

    public static readonly DependencyProperty SortByFieldProperty =
        DependencyProperty.Register("SortByField", typeof(BO.CallInListFields?), typeof(CallInListWindow), new PropertyMetadata(null));

    public IEnumerable<BO.CallInList> ListOfCalls
    {
        get => (IEnumerable<BO.CallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }

    private static DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));


    #endregion

    #region Events
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();

    private void OnDeleteCurrentAssignment(object sender, RoutedEventArgs e)
    {
        try
        {
            if (SelectedCall is null)
                throw new Exception("PL: The selected call is null");
            s_bl.Call.CancleCallSendEmailAsync(SelectedCall);
            s_bl.Call.CancelAssignement(UserId, SelectedCall!.CallId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void OnDeleteCall(object sender, RoutedEventArgs e)
    {
        try
        {
            if (SelectedCall is null)
                throw new Exception("PL: The selected call is null");
            s_bl.Call.DeleteCallRequest(SelectedCall.CallId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefreshList);

    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefreshList);

    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();

    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();

    private void OnAddCall(object sender, RoutedEventArgs e) => new CallWindow(-1).Show();
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        SortByField = null;
        FilterByField = null;
        FilterByValue = null;
        SelectedCall = null;
        RefreshList();
    }
    private void OnApplyFilterAndSort(object sender, RoutedEventArgs e) => RefreshList();

    #endregion

    #region Methods
    private void RefreshList()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        {
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                ListOfCalls = s_bl.Call.GetListOfCalls(
                    FilterByField,
                    FilterByValue,
                    SortByField,
                    RequestedSpecialMode is null
                        ? null
                        : s_bl.Call.GetListOfCalls(BO.CallInListFields.Status, RequestedSpecialMode!, null));
            } );
        }
    }
    #endregion
}