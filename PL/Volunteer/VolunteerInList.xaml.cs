using BO;
using System.Windows;

namespace PL.Volunteer;


public partial class VolunteerInList : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    
    public IEnumerable<VolunteerInList> VolunteerList
    {
        get => (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty);
        set => SetValue(VolunteerListProperty, value);
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerListProperty",
            typeof(IEnumerable<VolunteerInList>),
            typeof(VolunteerInList),
            new PropertyMetadata(null));


    public static BO.CallType FilterByCallTypes { get; set; } = BO.CallType.Undefined;
    


    public VolunteerInList()
    {
        InitializeComponent();
    }

    private void OnFilterCallTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        IEnumerable<VolunteerInList> VolunteerList = (IEnumerable<VolunteerInList>)((FilterByCallTypes == BO.CallType.Undefined) ? s_bl.Volunteer.GetVolunteers(null, null)! : s_bl.Volunteer.GetVolunteers(null, BO.VolunteerInListField.TypeOfCall));
    }
}
