using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;


public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    public TimeSpan CurrentRiskRange
    {
        get => (TimeSpan)GetValue(CurrentRiskRangeProperty);
        set => SetValue(CurrentRiskRangeProperty, value);
    }

    public static readonly DependencyProperty CurrentRiskRangeProperty = DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow));
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Triggered when user clicks the update risk range button and it updates the risk range variable with the current modified value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClick_RiskRangeUpdate(object sender, RoutedEventArgs e) => s_bl.Admin.SetRiskRange(CurrentRiskRange);
}