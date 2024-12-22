using System.Windows;

namespace PL.Volunteer;


public partial class VolunteerListWindow : Window
{
    public VolunteerListWindow()
    {
        InitializeComponent();
    }

    #region Regular Propeties
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public static BO.CallType FilterByCallTypes { get; set; } = BO.CallType.Undefined;
    public BO.VolunteerInList? SelectedVolunteer { get; set; }
    #endregion

    #region Dependency Properties
    /// <summary>
    /// The property which is the source of the volunteers in the window
    /// </summary>
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
        set => SetValue(VolunteerListProperty, value);
    }
    
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList",
            typeof(IEnumerable<BO.VolunteerInList>),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));

    #endregion

    #region Observers
    /// <summary>
    /// This method sets the current list of volunteers to get the filtered / unfiltered volunteers by the select call type
    /// </summary>
    private void VolunteerListObserver() => RefereshVolunteerList();
    #endregion

    #region Events

    #region Regular Events
    /// <summary>
    /// This method triggred when the user changed his selection of a call type and it filters the voluteers by the selected value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilterCallTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefereshVolunteerList();
    
    /// <summary>
    /// This method opens the Volunteer's window and show its details for editing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDoubleTappedVolunteerInList(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedVolunteer is not null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    /// <summary>
    /// This method opens the Volunteer's window and show the fields which the user can enter value to create a new Volunteer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAddVolunteer(object sender, RoutedEventArgs e) => new VolunteerWindow().Show();
    #endregion

    #region Window Evenets
    /// <summary>
    /// Triggered when the window is activated and registers the refereshes the volunteer list referesher to the observers in VolunteerManager
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWindowLoaded(object sender, RoutedEventArgs e) => s_bl.Volunteer.AddObserver(VolunteerListObserver);
    
    /// <summary>
    /// This method is triggered when the user closes the window and it removes the referesher from the VolunteerManager observer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWindowClosed(object sender, EventArgs e) => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
    #endregion

    #endregion

    #region Other Methods
    /// <summary>
    /// This method requests from the BL layer list of volunteers by the past parameters
    /// </summary>
    private void RefereshVolunteerList()
        => VolunteerList = (FilterByCallTypes == BO.CallType.Undefined)
        ? s_bl.Volunteer.GetVolunteers(null, null)!
        : s_bl.Volunteer.GetVolunteers(null, BO.VolunteerInListField.TypeOfCall);
    #endregion
}
