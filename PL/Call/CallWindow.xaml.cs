using System;
using System.Windows;

namespace PL.Call;

public partial class CallWindow : Window
{
    const string AddMode = "Add Call";
    const string UpdateMode = "Update Call";
    public CallWindow(int callId)
    {
        if(callId == -1)
        {
            CurrentCall = new BO.Call();
            ButtonText = AddMode;
        }
        else
        {
            CurrentCall = s_bl.Call.GetDetielsOfCall(callId);
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
    #endregion

    #region Regular Propeties
    private static BlApi.IBl s_bl = BlApi.Factory.Get();
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
            }
            else
                s_bl.Call.UpdateCall(CurrentCall);
            MessageBox.Show($@"Call Has been {ButtonText} Succecfully");
            Close();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }


    }
    #endregion

    #region Method
    #endregion

}
