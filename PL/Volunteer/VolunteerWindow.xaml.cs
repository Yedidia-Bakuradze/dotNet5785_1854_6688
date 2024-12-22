using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for Volunteer.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

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
        DependencyProperty.Register("ButtonTextProperty",
        typeof(string),
        typeof(VolunteerWindow),
        new PropertyMetadata(null));
    #endregion

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

    private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add")
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ButtonText == "Update")
            {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
