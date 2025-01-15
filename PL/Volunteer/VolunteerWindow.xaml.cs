using BO;
using PL.Call;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for Volunteer.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    public VolunteerWindow(int id, BO.UserRole windowRole)
    {
        VolunteerId = id;
        UserRoleIndicator = windowRole;
        ButtonText = id == 0
            ? "Add"
            : "Update";

        ButtonText += " Volunteer";
        RefereshScreen();
        InitializeComponent();
    }

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public int VolunteerId { get; set; }
    public string? CallStatus { get; set; }
    #endregion

    #region Dependency Properties

    /// <summary>
    /// The submit button which is shown on the UI as Add or Update Volunteer
    /// </summary>
    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// This propety managers whether we show the user the password field or not, to prevemt unnessery update
    /// </summary>
    public bool PasswordVisibility
    {
        get => (bool)GetValue(PasswordVisibilityPropety);
        set => SetValue(PasswordVisibilityPropety, value);
    }

    /// <summary>
    /// The current volunteer which is shown on the UI
    /// </summary>
    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public BO.UserRole UserRoleIndicator 
    {
        get => (BO.UserRole)GetValue(UserRoleIndicatorPropety);
        set => SetValue(UserRoleIndicatorPropety,value);
    }

    private static readonly DependencyProperty UserRoleIndicatorPropety =
        DependencyProperty.Register("UserRoleIndicator", typeof(BO.UserRole), typeof(VolunteerWindow));

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",typeof(string),typeof(VolunteerWindow),new PropertyMetadata(null));

    public static readonly DependencyProperty PasswordVisibilityPropety =
        DependencyProperty.Register("PasswordVisibility", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(null));


    public UserControl VolunteerDetailsUserControl
    {  
        get => (UserControl)(GetValue(VolunteerDetailsUserControlPropperty));
        set => SetValue(VolunteerDetailsUserControlPropperty,value);
    }

    private static readonly DependencyProperty VolunteerDetailsUserControlPropperty =
        DependencyProperty.Register("VolunteerDetailsUserControl", typeof(UserControl), typeof(VolunteerWindow));
    public UserControl VolunteerMapDetailsUserControl
    {
        get => (UserControl)(GetValue(VolunteerMapDetailsUserControlPropperty));
        set => SetValue(VolunteerMapDetailsUserControlPropperty, value);
    }

    private static readonly DependencyProperty VolunteerMapDetailsUserControlPropperty =
        DependencyProperty.Register("VolunteerMapDetailsUserControl", typeof(UserControl), typeof(VolunteerWindow));
    #endregion

    #region Events
    #region Regular Events
    /// <summary>
    /// This method request to update or create new volunteer entity based on the parameters given by the user in the UI
    /// If it faileds, the system would pop up a message box telling whats wrong
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add Volunteer")
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ButtonText == "Update Volunteer")
            {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer,PasswordVisibility);
                MessageBox.Show("Volunteer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary>
    /// This method is invoked when the user wants to open the call in progress window to watch its details
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShowCurrentCallInProgress(object sender, RoutedEventArgs e)
    {
        if(CurrentVolunteer?.CurrentCall != null)
            new CallInProgressWindow(CurrentVolunteer.CurrentCall).Show();
    }
    #endregion

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(RefereshScreen);
        s_bl.Call.AddObserver(RefereshScreen);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(RefereshScreen);
        s_bl.Call.RemoveObserver(RefereshScreen);
    }
    #endregion
    #region Methods
    private void RefereshScreen()
    {
        CurrentVolunteer = (VolunteerId == 0)
                ? new BO.Volunteer()
                : s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
        CallStatus = CurrentVolunteer.CurrentCall?.Status.ToString() ?? "";

        VolunteerDetailsUserControl = new VolunteerDetailsControl(CurrentVolunteer);
        //VolunteerMapDetailsUserControl = new VolunteerMapDetailsUserControl(CurrentVolunteer);
    }
    #endregion
}
