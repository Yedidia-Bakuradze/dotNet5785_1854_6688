using System.Windows;

namespace PL.Call;

public partial class CallWindow : Window
{

    public CallWindow(int callId)
    {
        CurrentCall = s_bl.Call.GetDetielsOfCall(callId);
        InitializeComponent();
    }
    #region Dependecy Propeties
    public BO.Call CurrentCall
    {
        get => (BO.Call)GetValue(CurrentCallPropety);
        set => SetValue(CurrentCallPropety,value);
    }

    private static DependencyProperty CurrentCallPropety =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));
    #endregion

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();

    #endregion

    #region Events
    #endregion

    #region Method
    #endregion

    private void OnSubmitBtnClick(object sender, RoutedEventArgs e) => MessageBox.Show("Unimplemened");
}
