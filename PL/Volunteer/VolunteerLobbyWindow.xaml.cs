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
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerLobbyWindow.xaml
    /// </summary>
    public partial class VolunteerLobbyWindow : Window
    {
        private readonly static BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerLobbyWindow(int volunteerId = 332461854)
        {
            VolunteerId = volunteerId;
            try
            {
                RefershWindowDetails();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
            InitializeComponent();
        }

        #region Dependecy Propeties
        public BO.Volunteer CurrentVolunteer 
        {
            get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty,value);
        }

        private static readonly DependencyProperty CurrentVolunteerProperty
            = DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerLobbyWindow));
        
        public string DescriptionText
        {
            get => (string)GetValue(DescriptionTextProperty);
            set => SetValue(DescriptionTextProperty, value);
        }

        private static readonly DependencyProperty DescriptionTextProperty
            = DependencyProperty.Register("DescriptionText", typeof(string), typeof(VolunteerLobbyWindow));

        public string WarrningSelectCallText
        {
            get => (string)GetValue(WarrningSelectCallTextProperty);
            set => SetValue(WarrningSelectCallTextProperty, value);
        }

        private static readonly DependencyProperty WarrningSelectCallTextProperty
            = DependencyProperty.Register("WarrningSelectCallText", typeof(string), typeof(VolunteerLobbyWindow));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        private static readonly DependencyProperty HeaderTextProperty
            = DependencyProperty.Register("HeaderText", typeof(string), typeof(VolunteerLobbyWindow));

        #endregion

        #region Regular Propeties
        public int VolunteerId { get; set; }
        #endregion

        #region Events
        private void OnSelectCall(object sender, RoutedEventArgs e)
        {

        }

        private void OnCancelCall(object sender, RoutedEventArgs e)
        {

        }

        private void OnFinishCall(object sender, RoutedEventArgs e)
        {

        }

        private void OnShowSettingsWindow(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e) => s_bl.Volunteer.RemoveObserver(RefershWindowDetails);

        private void Window_Loaded(object sender, RoutedEventArgs e) => s_bl.Volunteer.AddObserver(RefershWindowDetails);
        #endregion

        #region Methods
        private void RefershWindowDetails()
        {
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
            WarrningSelectCallText = CurrentVolunteer.IsActive ? "" : "Only online volunteers can take calls   Please activate this user in the settings to select a call";
            HeaderText = $"Welcome {CurrentVolunteer.FullName}";
            DescriptionText = $"Currently there are {s_bl.Call.GetOpenCallsForVolunteer(VolunteerId, null, null).Count()} calls open Would you like to take one?";
        }
        #endregion



    }
}
