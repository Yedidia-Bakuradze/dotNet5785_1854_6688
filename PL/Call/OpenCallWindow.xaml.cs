using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call;

public partial class OpenCallWindow : Window
{
    public OpenCallWindow(int callId,int volunteerId)
    {
        CallId = callId;
        VolunteerId = volunteerId;
        ReloadScreen();
        InitializeComponent();
    }

    #region Propeties
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    #endregion

    #region Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(Observer);
        s_bl.Volunteer.AddObserver(Observer);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(Observer);
        s_bl.Volunteer.RemoveObserver(Observer);
    }
    #endregion

    #region Dependecy Propeties
    public BO.Call CurrentCall 
    {
        get => (BO.Call)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty,value);
    }
    private static readonly DependencyProperty CurrentCallProperty
        = DependencyProperty.Register("CurrentCall", typeof(BO.Call),typeof(OpenCallWindow));

    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(OpenCallWindow));
    
    public UserControl MapView
    {
        get { return (UserControl)GetValue(MapViewProperty);}
        set { SetValue(MapViewProperty, value); }
    }
    public static readonly DependencyProperty MapViewProperty =
        DependencyProperty.Register("MapView", typeof(UserControl), typeof(OpenCallWindow));

    #endregion

    #region Methods
    private void Observer() => _ = ReloadScreen();
    private async Task ReloadScreen()
    {
        try
        {
            CurrentCall = await Task.Run(()=> s_bl.Call.GetDetielsOfCall(CallId));
            CurrentVolunteer = await Task.Run(() => s_bl.Volunteer.GetVolunteerDetails(VolunteerId));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Close();
        }


        if((CurrentCall.Latitude, CurrentCall.Longitude) is (null,null))
            return;
        
        List<(double, double)> listOfCordinates = [((double)CurrentCall.Latitude!, (double)CurrentCall.Longitude!)];

        if ((CurrentVolunteer.Latitude,CurrentVolunteer.Longitude) is not (null,null))
        {
            listOfCordinates.Insert(0, ((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
            MapView = new DisplayMapContent(TypeOfMap.MultipleTypeOfRoutes, BO.TypeOfRange.WalkingDistance, listOfCordinates);
        }
        else
        {
            MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.WalkingDistance, listOfCordinates);
        }


    }
    #endregion

}
