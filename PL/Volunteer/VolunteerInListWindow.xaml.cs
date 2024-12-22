using BO;
using System.Runtime.InteropServices;
using System.Windows;

namespace PL.Volunteer;


public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public static BO.CallType FilterByCallTypes { get; set; } = BO.CallType.Undefined;

    private BO.Volunteer? _selectedVolunteer;
    public BO.Volunteer? SelectedVolunteer {
        get => _selectedVolunteer;
        set => _selectedVolunteer = (BO.Volunteer?)value;
    }

    /// <summary>
    /// This method sets the current list of volunteers to get the filtered / unfiltered volunteers by the select call type
    /// </summary>
    private void VolunteerListObserver() => VolunteerList = GetFilteredVolunteerInList();

    /// <summary>
    /// The property which is the source of the volunteers in the window
    /// </summary>
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
        set => SetValue(VolunteerListProperty, value);
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerListProperty",
            typeof(IEnumerable<BO.VolunteerInList>),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));


    


    public VolunteerListWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// This method triggred when the user changed his selection of a call type and it filters the voluteers by the selected value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilterCallTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)=>
        VolunteerList = (FilterByCallTypes == BO.CallType.Undefined)
        ? s_bl.Volunteer.GetVolunteers(null, null)!
        : s_bl.Volunteer.GetVolunteers(null, BO.VolunteerInListField.TypeOfCall);

    /// <summary>
    /// Returna filtered volunteers by the select call type
    /// </summary>
    /// <returns></returns>
    private IEnumerable<BO.VolunteerInList> GetFilteredVolunteerInList() => (IEnumerable<BO.VolunteerInList>)((FilterByCallTypes == BO.CallType.Undefined)
            ? s_bl.Volunteer.GetVolunteers(null, null)!
            : s_bl.Volunteer.GetVolunteers(null, BO.VolunteerInListField.TypeOfCall));
    /// <summary>
    /// Triggered when the window is activated and registers the refereshes the volunteer list referesher to the observers in VolunteerManager
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Volunteer.AddObserver(VolunteerListObserver);
    
    /// <summary>
    /// This method is triggered when the user closes the window and it removes the referesher from the VolunteerManager observer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);

    private void OnDoubleTappedVolunteerInList(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedVolunteer is not null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    private void OnAddVolunteer(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }
}
