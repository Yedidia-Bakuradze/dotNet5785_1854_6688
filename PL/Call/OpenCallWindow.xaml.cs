using System.Windows;

namespace PL.Call;

public partial class OpenCallWindow : Window
{
    private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public OpenCallWindow(int callId)
    {
        try
        {
            CurrentCall = s_bl.Call.GetDetielsOfCall(callId);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            this.Close();
        }
        InitializeComponent();
    }


    public BO.Call CurrentCall 
    {
        get => (BO.Call)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty,value);
    }

    private static readonly DependencyProperty CurrentCallProperty
        = DependencyProperty.Register("CurrentCall", typeof(BO.Call),typeof(OpenCallListWindow));
}
