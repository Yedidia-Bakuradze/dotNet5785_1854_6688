using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    public BO.Volunteer? CurrentVolunteer { get; set; }

    #endregion

    #region Dependency Properties
    public IEnumerable<BO.ClosedCallInList> ListOfCalls 
    {
        get => (IEnumerable<BO.ClosedCallInList>)GetValue(ListOfCallsProperty);
        set => SetValue(ListOfCallsProperty,value);
    }

    private static readonly DependencyProperty ListOfCallsProperty =
        DependencyProperty.Register("ListOfCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(ClosedCallListWindow));

    public UserControl MapView
    {
        get { return (UserControl)GetValue(MapViewProperty); }
        set { SetValue(MapViewProperty, value); }
    }

    public static readonly DependencyProperty MapViewProperty =
        DependencyProperty.Register("MapView", typeof(UserControl), typeof(ClosedCallListWindow));

    public BO.CallType? FilterValue
    {
        get { return (BO.CallType?)GetValue(FilterValueProperty); }
        set { SetValue(FilterValueProperty, value); }
    }

    public static readonly DependencyProperty FilterValueProperty =
        DependencyProperty.Register("FilterValue", typeof(BO.CallType?), typeof(ClosedCallListWindow));

    public BO.ClosedCallInListFields? SortField
    {
        get { return (BO.ClosedCallInListFields?)GetValue(SortFieldProperty); }
        set { SetValue(SortFieldProperty, value); }
    }

    public static readonly DependencyProperty SortFieldProperty =
        DependencyProperty.Register("SortField", typeof(BO.ClosedCallInListFields?), typeof(ClosedCallListWindow));

    public BO.ClosedCallInList SelectedCall
    {
        get { return (BO.ClosedCallInList)GetValue(SelectedCallProperty); }
        set { SetValue(SelectedCallProperty, value); }
    }

    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(BO.ClosedCallInList), typeof(ClosedCallListWindow));
    #endregion

    #region Events
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(RefereshList);
    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(RefereshList);
    private void OnFilterValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefereshList();
    private void OnFliterAndSort(object sender, RoutedEventArgs e) => RefereshList();
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        FilterValue = null;
        SortField = null;
    }
    #endregion

    #region Methods
    private void RefereshList()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
                ListOfCalls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, FilterValue, SortField);
                List<(double, double)> listOfCordinates = s_bl.Call.ConvertClosedCallsIntoCordinates(ListOfCalls).ToList();
                if (CurrentVolunteer?.FullCurrentAddress is not null)
                {
                    listOfCordinates.Insert(0, ((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
                    MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfCordinates);
                }
                else
                {
                    MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfCordinates);
                }
            });

    }
    #endregion

}
