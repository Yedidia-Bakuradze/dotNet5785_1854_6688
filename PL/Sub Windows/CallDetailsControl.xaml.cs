using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.Sub_Windows;

public partial class CallDetailsControl : UserControl
{
    public CallDetailsControl(BO.Call call)
    {
        CurrentCall = call;
        Count = call.MyAssignments?.Count() ?? 0;
        InitializeComponent();
    }

    public BO.Call? CurrentCall { get; set; }
    public int Count { get; set; }
}
