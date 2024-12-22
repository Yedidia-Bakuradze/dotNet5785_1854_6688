using BO;
using System.Runtime.InteropServices;
using System.Windows;

namespace PL.Volunteer;


public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public static BO.CallType FilterByCallTypes { get; set; } = BO.CallType.Undefined;
    
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

    private void OnFilterCallTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        IEnumerable<VolunteerInList> VolunteerList = (FilterByCallTypes == BO.CallType.Undefined) ? s_bl.Volunteer.GetVolunteers(null, null)! : s_bl.Volunteer.GetVolunteers(null, BO.VolunteerInListField.TypeOfCall);
    }
    /// <summary>
    /// This method triggred when the user changed his selection of a call type and it filters the voluteers by the selected value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilterCallTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => VolunteerList = GetFilteredVolunteerInList();

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

}
