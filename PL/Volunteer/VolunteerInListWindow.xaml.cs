using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer;


public partial class VolunteerListWindow : Window
{
    public VolunteerListWindow()
    {
        InitializeComponent();
        VolunteerListObserver();
    }

    #region Regular Propeties
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null;
    #endregion

    #region Dependency Properties
    public BO.VolunteerInListField? SortByField
    {
        get => (BO.VolunteerInListField?)GetValue(SortByFieldProperty);
        set => SetValue(SortByFieldProperty, value);
    }
    public static readonly DependencyProperty SortByFieldProperty =
        DependencyProperty.Register("SortByField",
            typeof(BO.VolunteerInListField?),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));
    public BO.VolunteerInListField? FilterByField
    {
        get => (BO.VolunteerInListField?)GetValue(FilterByFieldProperty);
        set => SetValue(FilterByFieldProperty, value);
    }
    public static readonly DependencyProperty FilterByFieldProperty =
        DependencyProperty.Register("FilterByField",
            typeof(BO.VolunteerInListField?),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));
    public string? FilterByValue
    {
        get => (string?)GetValue(FilterByValueProperty);
        set => SetValue(FilterByValueProperty, value);
    }
    public static readonly DependencyProperty FilterByValueProperty =
        DependencyProperty.Register("FilterByValue",
            typeof(string),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));
    public BO.VolunteerInList? SelectedVolunteer
    {
        get => (BO.VolunteerInList?)GetValue(SelectedVolunteerProperty);
        set => SetValue(SelectedVolunteerProperty, value);
    }
    public static readonly DependencyProperty SelectedVolunteerProperty =
        DependencyProperty.Register("SelectedVolunteer",
            typeof(BO.VolunteerInList),
            typeof(VolunteerListWindow),
            new PropertyMetadata(null));
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

    /// <summary>
    /// This method opens the Volunteer's window and show its details for editing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDoubleTappedVolunteerInList(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedVolunteer is not null)
            new VolunteerWindow(SelectedVolunteer.Id, BO.UserRole.Admin).Show();
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
    private void OnResetParameters(object sender, RoutedEventArgs e)
    {
        SortByField = null;
        FilterByField = null;
        FilterByValue = null;
        SelectedVolunteer = null;
        RefereshVolunteerList();
    }
    private void OnApplyFiltersAndSort(object sender, RoutedEventArgs e) => RefereshVolunteerList();
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
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        {
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    VolunteerList = s_bl.Volunteer.GetFilteredVolunteers(FilterByField, FilterByValue, SortByField);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }
    }
    #endregion

}
