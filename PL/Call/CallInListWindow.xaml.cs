using System.Windows;

namespace PL.Call;

public partial class CallInListWindow : Window
{
    public CallInListWindow() => InitializeComponent();

    #region Regular Propeties
    public BO.CallInProgress? SelectedCall { get; set; }
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
    #endregion

    #region Methods
    #endregion

    private void OnShowCallEntityWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private void OnDeleteCurrentAssignment(object sender, RoutedEventArgs e)
    {

    }

    private void OnDeleteCall(object sender, RoutedEventArgs e)
    {

    }

    private void OnWindowClosed(object sender, EventArgs e)
    {

    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {

    }

    private void OnFilterSet(object sender, RoutedEventArgs e)
    {

    }

    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void OnAddCall(object sender, RoutedEventArgs e)
    {

    }
}
