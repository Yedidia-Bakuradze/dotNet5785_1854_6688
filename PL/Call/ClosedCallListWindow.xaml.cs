using System.Windows;

namespace PL.Call;

public partial class ClosedCallListWindow : Window
{
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public ClosedCallListWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        RefereshList();
        InitializeComponent();
    }

    public int VolunteerId { get; set; }

    public IEnumerable<BO.ClosedCallInList> ListOfCalls 
    {
        get => (IEnumerable<BO.ClosedCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty,value);
    }

    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(ClosedCallListWindow));

    public BO.CallType? FilterValue { get; set; }
    public BO.ClosedCallInListFields? SortFiled { get; set; }

    public BO.ClosedCallInList? SelectedCall { get; set; }

    private void RefereshList() => ListOfCalls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, FilterValue, SortFiled);

    private void OnFilterValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        RefereshList();
    }

    private void OnSortingValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        RefereshList();

    }

    private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        MessageBox.Show("No usage");
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        s_bl.Call.AddObserver(RefereshList);
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.RemoveObserver(RefereshList);
    }
}
