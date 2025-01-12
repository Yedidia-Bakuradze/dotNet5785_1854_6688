using PL.Call;
using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for Volunteer.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    public VolunteerWindow(int id = 0)
    {
        ButtonText = id == 0
            ? "Add"
            : "Update";

        ButtonText += " Volunteer";
        InitializeComponent();

        //Getting the volunteer / creating a new one
        try
        {
            CurrentVolunteer = (id == 0)
                ? new BO.Volunteer()
                : s_bl.Volunteer.GetVolunteerDetails(id);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
    }

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
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

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",typeof(string),typeof(VolunteerWindow),new PropertyMetadata(null));

    public static readonly DependencyProperty PasswordVisibilityPropety =
        DependencyProperty.Register("PasswordVisibility", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(null));
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

    #endregion

}
