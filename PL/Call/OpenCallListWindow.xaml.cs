using System.Windows;

namespace PL.Call;

public partial class OpenCallListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public OpenCallListWindow(int userId)
    {
        UserId = userId;
        RefreshList();
        InitializeComponent();
    }

    #region Regular Propeties
    public BO.OpenCallInList? SelectedCall { get; set; }
    public int UserId { get; set; }
    public BO.CallStatus? RequestedSpecialMode { get; set; }
    public BO.CallType? FilterByValue { get; set; }
    public BO.OpenCallFields? SortByField { get; set; }
    #endregion

    #region Dependecy Propeties
    public IEnumerable<BO.OpenCallInList> ListOfCalls
    {
        get => (IEnumerable<BO.OpenCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }

    private static DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallListWindow), new PropertyMetadata(null));


    #endregion

    #region Events
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefreshList);
    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefreshList);
    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();
    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();

    #endregion

    #region Methods
    private void RefreshList() => ListOfCalls
        = s_bl.Call
        .GetOpenCallsForVolunteer(
            UserId,
            FilterByValue,
            SortByField
            );
    #endregion

    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => MessageBox.Show($" Vale: {SelectedCall}\nnew OpenCallWindow().Show()");

    private void OnSelectCallTodo(object sender, RoutedEventArgs e)
    {
        s_bl.Call.SelectCallToDo(UserId, SelectedCall!.CallId);
    }
}
