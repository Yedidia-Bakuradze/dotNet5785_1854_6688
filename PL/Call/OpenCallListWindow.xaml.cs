using BO;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Converters;
using System.Windows.Threading;

namespace PL.Call;

public partial class OpenCallListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public OpenCallListWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        ListOfCalls = null;
        _ = RefreshList();
        InitializeComponent();
    }

    #region Regular Propeties
    public int VolunteerId { get; set; }
    private volatile DispatcherOperation? _observerOperation = null; //stage 7

    #endregion

    #region Dependecy Propeties
    public BO.OpenCallFields? SortByField
    {
        get { return (BO.OpenCallFields?)GetValue(SortByFieldProperty); }
        set { SetValue(SortByFieldProperty, value); }
    }
    public static readonly DependencyProperty SortByFieldProperty =
        DependencyProperty.Register("SortByField", typeof(BO.OpenCallFields?), typeof(OpenCallListWindow));

    public BO.CallType? FilterByValue
    {
        get { return (BO.CallType?)GetValue(FilterByValueProperty); }
        set { SetValue(FilterByValueProperty, value); }
    }
    public static readonly DependencyProperty FilterByValueProperty =
        DependencyProperty.Register("FilterByValue", typeof(BO.CallType?), typeof(OpenCallListWindow));

    public BO.CallStatus? RequestedSpecialMode
    {
        get { return (BO.CallStatus?)GetValue(RequestedSpecialModeProperty); }
        set { SetValue(RequestedSpecialModeProperty, value); }
    }
    public static readonly DependencyProperty RequestedSpecialModeProperty =
        DependencyProperty.Register("RequestedSpecialMode", typeof(BO.CallStatus?), typeof(OpenCallListWindow));

    public BO.OpenCallInList? SelectedCall
    {
        get { return (BO.OpenCallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }
    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(OpenCallListWindow));

    public IEnumerable<BO.OpenCallInList> ListOfCalls
    {
        get => (IEnumerable<BO.OpenCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty, value);
    }
    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallListWindow), new PropertyMetadata(null));

    public UserControl OpenCallsMap
    {
        get { return (UserControl)GetValue(OpenCallsMapProperty); }
        set { SetValue(OpenCallsMapProperty, value); }
    }
    public static readonly DependencyProperty OpenCallsMapProperty =
        DependencyProperty.Register("OpenCallsMap", typeof(UserControl), typeof(OpenCallListWindow));
    #endregion

    #region Events
    private void OnShowCallWindow(object sender, System.Windows.Input.MouseButtonEventArgs e) => new CallWindow(SelectedCall!.CallId).Show();
    private void OnWindowClosed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer);
    }
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer);
    }
    private void OnFilterSet(object sender, RoutedEventArgs e) => RefreshList();
    private void OnSortingChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefreshList();
    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if(SelectedCall is not null)
            new OpenCallWindow(SelectedCall!.CallId,VolunteerId).Show();
    }
    private void OnSelectCallTodo(object sender, RoutedEventArgs e)
    {
        s_bl.Call.SelectCallToDo(VolunteerId, SelectedCall!.CallId);
        Close();
    }
    private void OnFliterAndSort(object sender, RoutedEventArgs e) => RefreshList();
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        FilterByValue = null;
        SortByField = null;
        RefreshList();
    }

    #endregion

    #region Methods
    private void Observer() => _ = RefreshList();
    private async Task RefreshList()
    {
        var _volunteerId = VolunteerId;
        var _filterByValue = FilterByValue;
        var _sortByField = SortByField;

        IEnumerable<OpenCallInList> calls = await Task.Run(()=> s_bl.Call.GetOpenCallsForVolunteer(_volunteerId, _filterByValue, _sortByField));
        var _listOfCordinates = await Task.Run(() => s_bl.Call.ConvertOpenCallsToCordinates(calls).ToList());

        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var volunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                    ListOfCalls = calls;
                    List<(double, double)> listOfCordinates = _listOfCordinates;

                    if ((volunteer.Latitude, volunteer.Longitude) is not (null,null))
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
                    MessageBox.Show(ex.Message);
                    Close();
                }
            });
        
    }
    #endregion
}
