using PL.Sub_Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Converters;
using System.Windows.Threading;
namespace PL.Call;

public partial class CallWindow : Window
{
    const string AddMode = "Add Call";
    const string UpdateMode = "Update Call";
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    public CallWindow(int callId)
    {
        CallId = callId;
        if(callId == -1)
        {
            CurrentCall = new BO.Call();

            ButtonText = AddMode;
            CurrentTime = DateTime.Now.ToString();
        }
        else
        {
            Referesh();
            CurrentTime = CurrentCall.CallStartTime.ToString();
            ButtonText = UpdateMode;
        }
        InitializeComponent();
    }
    #region Dependecy Propeties
    public BO.Call CurrentCall
    {
        get => (BO.Call)GetValue(CurrentCallPropety);
        set => SetValue(CurrentCallPropety,value);
    }

    private static readonly DependencyProperty CurrentCallPropety =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));


    public string ButtonText
    { 
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty,value);
    }
    private static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",typeof(string),typeof(CallWindow));


    public UserControl CallDetailsControler 
    {
        get => (UserControl)GetValue(CallDetailsControlerProperty);
        set => SetValue(CallDetailsControlerProperty,value);
    }
    private static readonly DependencyProperty CallDetailsControlerProperty =
        DependencyProperty.Register("CallDetailsControler", typeof(UserControl), typeof(CallWindow));
    public UserControl CallMapDetailsControler
    {
        get => (UserControl)GetValue(CallMapDetailsControlerProperty);
        set => SetValue(CallMapDetailsControlerProperty, value);
    }
    private static readonly DependencyProperty CallMapDetailsControlerProperty =
        DependencyProperty.Register("CallMapDetailsControler", typeof(UserControl), typeof(CallWindow));
    public string CurrentTime
    {
        get => (string)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(string), typeof(CallWindow));
    #endregion

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    public int CallId { get;set; }
    public string HourSet { get; set; } = "";
    #endregion

    #region Events
    private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == AddMode)
            {
                

                if (!TimeSpan.TryParse(HourSet, out TimeSpan time))
                    if (HourSet == "")
                        time = TimeSpan.Zero;
                    else
                        throw new Exception($"Unable to case {HourSet} into time span");

                CurrentCall.CallDeadLine += time;
                s_bl.Call.AddCall(CurrentCall);
                Close();
            }
            else
                s_bl.Call.UpdateCall(CurrentCall);
            MessageBox.Show($@"Call Has been {ButtonText} Succecfully");
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }


    }
    private void Window_Closed(object sender, EventArgs e) => s_bl.Call.RemoveObserver(Observer);
    private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Call.AddObserver(Observer);
    #endregion

    #region Method
    private void Observer() => Referesh();
    private void Referesh()
    {
        try
        {
            CurrentCall = s_bl.Call.GetDetielsOfCall(CallId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            if(_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() => Close());
        }

        if ((_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed))
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                if (CurrentCall is not null && CurrentCall.Latitude is null && CallId is not -1)
                    MessageBox.Show("NOTE: That your address is not valid, please change it");
                if(CurrentCall is not null && CurrentCall.Latitude is not null)
                {
                    CallDetailsControler = new CallDetailsControl(CurrentCall);
                    List<(double, double)> listOfPoints = [((double)CurrentCall.Latitude!, (double)CurrentCall.Longitude!)];
                    CallMapDetailsControler = new DisplayMapContent(TypeOfMap.Pin, BO.TypeOfRange.AirDistance, listOfPoints);
                }
            });
        
    }
    #endregion
}
