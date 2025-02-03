using BO;
using DO;
using PL.Call;
using PL.Sub_Windows;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Converters;
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for Volunteer.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    public VolunteerWindow(int id, BO.UserRole windowRole)
    {
        // If the ID is not zero, add an observer for updates related to this volunteer
        if (id != 0)
            s_bl.Volunteer.AddObserver(id, RefereshScreen);

        VolunteerId = id;
        UserRoleIndicator = windowRole;

        // Determine button text based on whether the volunteer is being added or updated
        ButtonText = id == 0
            ? "Add"
            : "Update";

        ButtonText += " Volunteer";

        RefereshScreen(); // Refresh the UI with updated data
        InitializeComponent(); // Initialize UI components
    }

    #region Regular Properties
    private static BlApi.IBl s_bl = BlApi.Factory.Get(); // Business logic instance
    public int VolunteerId { get; set; } // ID of the volunteer
    public string? CallStatus { get; set; } // Current call status

    private volatile DispatcherOperation? _observerOperation = null; // Used for UI thread-safe updates
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
    /// Controls the visibility of the password field to prevent unnecessary updates
    /// </summary>
    public bool PasswordVisibility
    {
        get => (bool)GetValue(PasswordVisibilityPropety);
        set => SetValue(PasswordVisibilityPropety, value);
    }

    /// <summary>
    /// The current volunteer object displayed in the UI
    /// </summary>
    public BO.Volunteer CurrentVolunteer
    {
        get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public BO.UserRole UserRoleIndicator
    {
        get => (BO.UserRole)GetValue(UserRoleIndicatorPropety);
        set => SetValue(UserRoleIndicatorPropety, value);
    }

    // Registering dependency properties for UI binding
    private static readonly DependencyProperty UserRoleIndicatorPropety =
        DependencyProperty.Register("UserRoleIndicator", typeof(BO.UserRole), typeof(VolunteerWindow));

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(null));

    public static readonly DependencyProperty PasswordVisibilityPropety =
        DependencyProperty.Register("PasswordVisibility", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(null));

    public string? NewPassword
    {
        get => (string?)GetValue(NewPasswordProperty);
        set => SetValue(NewPasswordProperty, value);
    }
    public static readonly DependencyProperty NewPasswordProperty =
        DependencyProperty.Register("NewPassword", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(null));

    public UserControl VolunteerDetailsUserControl
    {
        get => (UserControl)(GetValue(VolunteerDetailsUserControlPropperty));
        set => SetValue(VolunteerDetailsUserControlPropperty, value);
    }

    public static readonly DependencyProperty VolunteerDetailsUserControlPropperty =
        DependencyProperty.Register("VolunteerDetailsUserControl", typeof(UserControl), typeof(VolunteerWindow));

    public UserControl VolunteerMapDetailsUserControl
    {
        get => (UserControl)(GetValue(VolunteerMapDetailsUserControlPropperty));
        set => SetValue(VolunteerMapDetailsUserControlPropperty, value);
    }

    public static readonly DependencyProperty VolunteerMapDetailsUserControlPropperty =
        DependencyProperty.Register("VolunteerMapDetailsUserControl", typeof(UserControl), typeof(VolunteerWindow));
    #endregion

    #region Events
    #region Regular Events
    /// <summary>
    /// Handles the submit button click event to add or update a volunteer
    /// </summary>
    private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add Volunteer")
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Close the window after adding
            }
            else if (ButtonText == "Update Volunteer")
            {
                // Update password only if it's visible (user wants to change it)
                if (PasswordVisibility)
                    CurrentVolunteer.Password = NewPassword;

                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer, PasswordVisibility);
                MessageBox.Show("Volunteer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens the call in progress window if there is an active call
    /// </summary>
    private void OnShowCurrentCallInProgress(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer.CurrentCall != null)
            new CallInProgressWindow(CurrentVolunteer.Id).Show();
    }
    #endregion

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        // Add observers for volunteer and call updates
        s_bl.Volunteer.AddObserver(RefereshScreen);
        s_bl.Call.AddObserver(RefereshScreen);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        // Remove observers when the window is closed
        if (VolunteerId != 0)
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefereshScreen);
        s_bl.Volunteer.RemoveObserver(RefereshScreen);
        s_bl.Call.RemoveObserver(RefereshScreen);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Refreshes the UI with updated volunteer details
    /// </summary>
    private void RefereshScreen()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        {
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                // Retrieve volunteer details from the business logic
                CurrentVolunteer = (VolunteerId == 0)
                        ? new BO.Volunteer()
                        : s_bl.Volunteer.GetVolunteerDetails(VolunteerId);

                CallStatus = CurrentVolunteer.CurrentCall?.Status.ToString() ?? "";

                VolunteerDetailsUserControl = new VolunteerDetailsControl(CurrentVolunteer);

                // Display volunteer location on a map if valid coordinates exist
                if ((CurrentVolunteer.Latitude, CurrentVolunteer.Longitude) is not (null, null))
                {
                    List<(double, double)> listOfPoints = [((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!)];
                    VolunteerMapDetailsUserControl = new DisplayMapContent(TypeOfMap.Pin, CurrentVolunteer.RangeType, listOfPoints);
                }

                if (CurrentVolunteer is not null && CurrentVolunteer.Latitude is null && VolunteerId is not 0)
                    MessageBox.Show("NOTE: Your address is not valid, please change it");
            });
        }
    }
    #endregion


    private void OnActiveValueChanged(object sender, RoutedEventArgs e) => CurrentVolunteer!.IsActive = !CurrentVolunteer.IsActive;

    private void OnRoleChanged(object sender, RoutedEventArgs e) => CurrentVolunteer!.Role = CurrentVolunteer.Role == BO.UserRole.Admin ? BO.UserRole.Volunteer : BO.UserRole.Admin;

    private void OnPasswordVisibilityChanged(object sender, RoutedEventArgs e)
    {
        PasswordVisibility = !PasswordVisibility;
        if (PasswordVisibility)
        {
            NewPassword = CurrentVolunteer.Password;
        }
        else
        {
            NewPassword = null;
        }
    }
}
