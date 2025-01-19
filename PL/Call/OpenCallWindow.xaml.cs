using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call;

public partial class OpenCallWindow : Window
{
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public OpenCallWindow(int callId,int volunteerId)
    {
        CallId = callId;
        VolunteerId = volunteerId;
        ReloadScreen();
        InitializeComponent();
    }

    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public BO.Call CurrentCall 
    {
        get => (BO.Call)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty,value);
    }

    private static readonly DependencyProperty CurrentCallProperty
        = DependencyProperty.Register("CurrentCall", typeof(BO.Call),typeof(OpenCallListWindow));



    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentVolunteer.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(OpenCallListWindow));


    public UserControl MapView
    {
        get { return (UserControl)GetValue(MapViewProperty);}
        set { SetValue(MapViewProperty, value); }
    }

    public static readonly DependencyProperty MapViewProperty =
        DependencyProperty.Register("MapView", typeof(UserControl), typeof(OpenCallListWindow));

    
    private void ReloadScreen()
    {
        try
        {
            CurrentCall = s_bl.Call.GetDetielsOfCall(CallId);
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
            List<(double, double)> listOfCordinates = new();
            listOfCordinates.Add((CurrentCall.Latitude,CurrentCall.Longitude));
            if(CurrentVolunteer.FullCurrentAddress is not null)
            {
                listOfCordinates.Insert(0, ((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!));
                MapView = new DisplayMapContent(TypeOfMap.MultipleTypeOfRoutes,BO.TypeOfRange.WalkingDistance,listOfCordinates);
            }
            else
            {
                MapView = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.WalkingDistance,listOfCordinates);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            this.Close();
        }
    }
}
