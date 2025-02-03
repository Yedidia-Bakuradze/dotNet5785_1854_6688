using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Converters;
using System.Windows.Threading;

namespace PL.Call;

public partial class CallWindow : Window
{
    // Constants for Add and Update modes
    const string AddMode = "Add Call";
    const string UpdateMode = "Update Call";

    // Dispatcher operation for handling UI updates on the UI thread
    private volatile DispatcherOperation? _observerOperation = null; // stage 7

    // Constructor to initialize the window with a call ID and handle Add or Update mode
    public CallWindow(int callId)
    {
        CallId = callId;
        if (callId == -1)
        {
            CurrentCall = new BO.Call(); // New call for adding a call

            ButtonText = AddMode; // Button text will be "Add Call"
            CurrentTime = DateTime.Now.ToString(); // Set the current time as the call time
        }
        else
        {
            Referesh();
            CurrentTime = CurrentCall.CallStartTime.ToString();
            ButtonText = UpdateMode;
        }
        InitializeComponent(); // Initialize the components
    }

    #region Dependency Properties

    // Dependency property for the current call being worked on
    public BO.Call CurrentCall
    {
        get => (BO.Call)GetValue(CurrentCallPropety);
        set => SetValue(CurrentCallPropety, value);
    }

    // Dependency property for the button text (Add/Update mode)
    private static readonly DependencyProperty CurrentCallPropety =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    // Dependency property for the button text
    private static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow));

    // Dependency property for call details controller (UserControl)
    public UserControl CallDetailsControler
    {
        get => (UserControl)GetValue(CallDetailsControlerProperty);
        set => SetValue(CallDetailsControlerProperty, value);
    }

    // Dependency property for the call details controller
    private static readonly DependencyProperty CallDetailsControlerProperty =
        DependencyProperty.Register("CallDetailsControler", typeof(UserControl), typeof(CallWindow));

    // Dependency property for the call map details controller (UserControl)
    public UserControl CallMapDetailsControler
    {
        get => (UserControl)GetValue(CallMapDetailsControlerProperty);
        set => SetValue(CallMapDetailsControlerProperty, value);
    }

    // Dependency property for the call map details controller
    private static readonly DependencyProperty CallMapDetailsControlerProperty =
        DependencyProperty.Register("CallMapDetailsControler", typeof(UserControl), typeof(CallWindow));

    // Dependency property for the current time of the call
    public string CurrentTime
    {
        get => (string)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    // Dependency property for current time
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(string), typeof(CallWindow));

    #endregion

    #region Regular Properties

    // Static reference to the business logic layer (BL)
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    // Property to store the call ID
    public int CallId { get; set; }

    // Property to store the time set for the call
    public string HourSet { get; set; } = "";

    #endregion

    #region Events

    // Event handler for when the submit button is clicked
    private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            // If in Add mode, add a new call
            if (ButtonText == AddMode)
            {
                // Attempt to parse the time for the deadline of the call
                if (!TimeSpan.TryParse(HourSet, out TimeSpan time))
                    if (HourSet == "")
                        time = TimeSpan.Zero;
                    else
                        throw new Exception($"Unable to cast {HourSet} into time span");

                // Add the parsed time to the deadline of the call
                CurrentCall.CallDeadLine += time;

                // Add the call using the business logic layer
                s_bl.Call.RemoveObserver(Observer);
                s_bl.Call.AddCall(CurrentCall);
                Close(); // Close the window after adding the call
            }
            else
                // Update the existing call using the business logic layer
                s_bl.Call.UpdateCall(CurrentCall);

            // Show a success message
            MessageBox.Show($@"Call Has been {ButtonText} Successfully");
        }
        catch (Exception ex)
        {
            // Show an error message if an exception occurs
            MessageBox.Show(ex.Message);
        }
    }

    // Event handler for when the window is closed
    private void Window_Closed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(Observer);

    // Event handler for when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(Observer);

    #endregion

    #region Methods

    // Observer method to refresh the call details when the state changes
    private void Observer() => Referesh();

    // Method to refresh the call details displayed on the window
    private void Referesh()
    {
        try
        {
            // Get the call details using the call ID
            CurrentCall = s_bl.Call.GetDetielsOfCall(CallId);
        }
        catch (Exception ex)
        {
            // Show an error message if an exception occurs
            MessageBox.Show(ex.Message);
            if(_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() => Close());
        }

        // If the operation is completed or hasn't started, refresh the UI
        if ((_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed))
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                // Check if the call's latitude is null and display a warning
                if (CurrentCall is not null && CurrentCall.Latitude is null && CallId is not -1)
                    MessageBox.Show("NOTE: That your address is not valid, please change it");

                // If the call's latitude is not null, show the call details and map
                if (CurrentCall is not null && CurrentCall.Latitude is not null)
                {
                    CallDetailsControler = new CallDetailsControl(CurrentCall);

                    // Create a list of points to display on the map (latitude, longitude)
                    List<(double, double)> listOfPoints = [((double)CurrentCall.Latitude!, (double)CurrentCall.Longitude!)];

                    // Set the map details controller to display the map
                    CallMapDetailsControler = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfPoints);
                }
            });
    }

    #endregion
}
