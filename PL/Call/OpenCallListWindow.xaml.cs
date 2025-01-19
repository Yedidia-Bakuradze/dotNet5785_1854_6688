using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Converters;

namespace PL.Call;

public partial class OpenCallListWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public OpenCallListWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        RefreshList();
        InitializeComponent();
    }

    #region Regular Propeties
    public int VolunteerId { get; set; }
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
        s_bl.Call.RemoveObserver(RefreshList);
        s_bl.Volunteer.AddObserver(RefreshList);
    }
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(RefreshList);
        s_bl.Volunteer.AddObserver(RefreshList);
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
    }

    #endregion

    #region Methods
    private void RefreshList()
    {
        var volunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
        ListOfCalls = s_bl.Call.GetOpenCallsForVolunteer(VolunteerId,FilterByValue,SortByField);
        List<(double, double)> listOfCordinates = s_bl.Call
            .ConvertOpenCallsToCordinates(ListOfCalls)
            .ToList<(double,double)>();

        if (volunteer.FullCurrentAddress is not null)
        {
            listOfCordinates.Insert(0, ((double)volunteer.Latitude!, (double)volunteer.Longitude!));
            if (listOfCordinates.Count() != 0)
                OpenCallsMap = new DisplayMapContent(TypeOfMap.Route, volunteer.RangeType,listOfCordinates);
            else
                OpenCallsMap = new DisplayMapContent(TypeOfMap.Pin, volunteer.RangeType, listOfCordinates);
        }
        else
            OpenCallsMap = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.WalkingDistance, listOfCordinates);
    }
    #endregion
}
