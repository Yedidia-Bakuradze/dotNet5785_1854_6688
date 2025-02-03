using System.Windows;
using System.Windows.Threading;

namespace PL.Call;

public partial class CallInProgressWindow : Window
{
    // Constructor to initialize the window with a volunteer ID and load the screen
    public CallInProgressWindow(int volunteerId)
    {
        VolunteerId = volunteerId;
        LoadScreen(); // Load the screen to display the current call in progress
    }

    #region Regular Properties
    // Static reference to the business logic layer (BL)
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    // Dispatcher operation for handling UI updates on the UI thread
    private volatile DispatcherOperation? _observerOperation = null; // stage 7

    // Volunteer ID for identifying the volunteer whose call is in progress
    public int VolunteerId { get; set; }
    #endregion

    #region Dependency Property
    // Current call in progress (bindable property)
    public BO.CallInProgress CurrentCallInProgress
    {
        get => (BO.CallInProgress)GetValue(CurrentCallInProgressProperty);
        set => SetValue(CurrentCallInProgressProperty, value);
    }

    // DependencyProperty for CurrentCallInProgress
    public static readonly DependencyProperty CurrentCallInProgressProperty
        = DependencyProperty.Register("CurrentCallInProgress", typeof(BO.CallInProgress), typeof(CallInProgressWindow), new PropertyMetadata(null));

    #endregion

    #region Events
    // Event handler to add the observer when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Volunteer.AddObserver(LoadScreen);

    // Event handler to remove the observer when the window is closed
    private void Window_Closed(object sender, EventArgs e) => s_bl.Volunteer.RemoveObserver(LoadScreen);
    #endregion

    #region Methods
    // Method to load the screen with the current call details
    private void LoadScreen()
    {
        // Check if the operation has completed or hasn't started
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            // Start a new operation to load the current call in progress
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    // Get the volunteer details and check if there is a call in progress
                    CurrentCallInProgress = s_bl.Volunteer.GetVolunteerDetails(VolunteerId).CurrentCall
                        ?? throw new Exception($"PL: The volunteer {VolunteerId} no longer has a call in progress. Existing Screen");
                }
                catch (Exception ex)
                {
                    // Show error message if something goes wrong (e.g., no current call in progress)
                    MessageBox.Show(ex.Message);
                    Close(); // Close the window if no call is found
                }
            });
    }
    #endregion
}
