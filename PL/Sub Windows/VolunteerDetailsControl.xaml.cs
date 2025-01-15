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

public partial class VolunteerDetailsControl : UserControl
{
    public VolunteerDetailsControl(BO.Volunteer volunteer)
    {
        CurrentVolunteer = volunteer;
        IsActive = volunteer.IsActive ? "Yes" : "No";
        InitializeComponent();
    }

    public BO.Volunteer? CurrentVolunteer { get; set; }
    public string Password { get; set; } = "******************";
    public string IsActive { get; set; }
}
