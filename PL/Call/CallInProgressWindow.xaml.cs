using System.Windows;
namespace PL.Call;

public partial class CallInProgressWindow : Window
{
    public CallInProgressWindow(BO.CallInProgress callInProgress)
    {
        CurrentCallInProgress = callInProgress;
        InitializeComponent();
    }

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
    #endregion

    #region Dependency Property
    public BO.CallInProgress CurrentCallInProgress
    {
        get => (BO.CallInProgress)GetValue(CurrentCallInProgressProperty);
        set => SetValue(CurrentCallInProgressProperty, value);
    }

    public static readonly DependencyProperty CurrentCallInProgressProperty
        = DependencyProperty.Register("CurrentCallInProgress", typeof(BO.CallInProgress), typeof(CallInProgressWindow), new PropertyMetadata(null));
    #endregion

}
