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

    #region Regular Properties
    public int VolunteerId { get; set; }
    #endregion

    #region Dependency Properties
    public IEnumerable<BO.ClosedCallInList> ListOfCalls 
    {
        get => (IEnumerable<BO.ClosedCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty,value);
    }

    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(ClosedCallListWindow));

    public BO.CallType? FilterValue
    {
        get { return (BO.CallType?)GetValue(FilterValueProperty); }
        set { SetValue(FilterValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FilterValue.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FilterValueProperty =
        DependencyProperty.Register("FilterValue", typeof(BO.CallType?), typeof(ClosedCallListWindow));

    public BO.ClosedCallInListFields? SortField
    {
        get { return (BO.ClosedCallInListFields?)GetValue(SortFieldProperty); }
        set { SetValue(SortFieldProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SortField.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SortFieldProperty =
        DependencyProperty.Register("SortField", typeof(BO.ClosedCallInListFields?), typeof(ClosedCallListWindow));

    public BO.ClosedCallInList SelectedCall
    {
        get { return (BO.ClosedCallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedCall.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.ClosedCallInList), typeof(ClosedCallListWindow));
    #endregion

    #region Events

    private void OnFilterValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefereshList();

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
    private void OnFliterAndSort(object sender, RoutedEventArgs e) => RefereshList();

    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        FilterValue = null;
        SortField = null;
    }

    #endregion

    #region Methods
    private void RefereshList() => ListOfCalls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, FilterValue, SortField);
    #endregion
}
