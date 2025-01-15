using System.Windows;

namespace PL.Call;

public partial class CallInListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public CallInListWindow(BO.CallStatus? requstedCallByStatus = null,int userId = 332461854) {
        RequestedSpecialMode = requstedCallByStatus;
        UserId = userId;
        InitializeComponent();
        RefreshList();
    }

    #region Regular Propeties
    public BO.CallInList? SelectedCall { get; set; }
    public int UserId { get; set; }
    public BO.CallStatus? RequestedSpecialMode{ get; set; }
    public string? FilterByValue { get; set; }
    public BO.CallInListFields? FilterByField { get; set; }
    public BO.CallInListFields? SortByField { get; set; }
    #endregion

    #region Dependecy Propeties
    public IEnumerable<BO.CallInList> ListOfCalls
    {
        get => (IEnumerable<BO.CallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty,value);
    }

    private static DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow),new PropertyMetadata(null));


    #endregion

    #region Events
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();

    private void OnDeleteCurrentAssignment(object sender, RoutedEventArgs e)
    {
        try
        {
            if (SelectedCall is null)
                throw new Exception("PL: The selected call is null");
            s_bl.Call.CancelAssignement(UserId,SelectedCall!.CallId);
        }
        catch(Exception ex)
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
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefreshList);

    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefreshList);

    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();

    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();

    private void OnAddCall(object sender, RoutedEventArgs e) => new CallWindow(-1).Show();

    #endregion

    #region Methods
    private void RefreshList() => ListOfCalls
        = s_bl.Call
        .GetListOfCalls(
            FilterByField,
            FilterByValue,
            SortByField,
            RequestedSpecialMode is null
                ? null
                : s_bl.Call.GetListOfCalls(BO.CallInListFields.Status, RequestedSpecialMode!,null)
            );
    #endregion

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {

    }
}
