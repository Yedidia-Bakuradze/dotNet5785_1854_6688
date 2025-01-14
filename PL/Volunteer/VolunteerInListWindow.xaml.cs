using System.Windows;

namespace PL.Volunteer;


public partial class VolunteerListWindow : Window
{
    public VolunteerListWindow()
    {
        InitializeComponent();
        RefereshVolunteerList();
    }

    #region Regular Propeties
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public static BO.VolunteerInListField? SortByField { get; set; }
    public static BO.VolunteerInListField? FilterByField{ get; set; }
    public static string? FilterByValue{ get; set; }
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
    /// This method triggred when the user changed his selection of parameter to sort by the volunteers
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSortingValueChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RefereshVolunteerList();
    
    /// <summary>
    /// This method triggred when the user changed his selection of filters to filter the volunteers
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFilterValueChanged(object sender, RoutedEventArgs e) => RefereshVolunteerList();

    /// <summary>
    /// This method opens the Volunteer's window and show its details for editing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDoubleTappedVolunteerInList(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedVolunteer is not null)
            new VolunteerWindow(SelectedVolunteer.Id,BO.UserRole.Admin).Show();
    }

    /// <summary>
    /// This method opens the Volunteer's window and show the fields which the user can enter value to create a new Volunteer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAddVolunteer(object sender, RoutedEventArgs e) => new VolunteerWindow(0, BO.UserRole.Admin).Show();

    /// <summary>
    /// This method is invoked when the user requests to remove a volunteer from the list
    /// The method would ask his final premission and try to delete the volunteer
    /// If the volunteer is now allowed to be delete, the system will show a message box which tells the user the reason for not being able to delete the volunteer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDeleteVolunteerInList(object sender, RoutedEventArgs e)
    {
        MessageBoxResult answer = MessageBox.Show(
$@"
Are you sure that you want to delete user number: {SelectedVolunteer?.Id} with the following values
- Id: {SelectedVolunteer?.Id}
- FullName: {SelectedVolunteer?.FullName}
- IsActive: {SelectedVolunteer?.IsActive}
- TotalCallsDoneByVolunteer: {SelectedVolunteer?.TotalCallsDoneByVolunteer}
- TotalCallsCancelByVolunteer: {SelectedVolunteer?.TotalCallsCancelByVolunteer}
- TotalCallsExpiredByVoluntee: {SelectedVolunteer?.TotalCallsExpiredByVolunteer}
- CallId: {SelectedVolunteer?.CallId}
- TypeOfCall: {SelectedVolunteer?.TypeOfCall}
",
$"Delete User: {SelectedVolunteer?.Id} Request", MessageBoxButton.YesNo);
        if (answer == MessageBoxResult.Yes)
        {
            try
            {
                s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer!.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
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
    {
        try
        {
            VolunteerList = s_bl.Volunteer.GetFilteredVolunteers(FilterByField, FilterByValue, SortByField);
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }
    #endregion

}
