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





    public VolunteerInList()
    {
        InitializeComponent();
    }
    

}
