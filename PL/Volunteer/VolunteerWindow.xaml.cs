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
    /// Interaction logic for Volunteer.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonTextProperty",
            typeof(string),
            typeof(VolunteerWindow),
            new PropertyMetadata(null));


        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0
                ? "Add"
                : "Update";

            ButtonText += " Volunteer";
            InitializeComponent();
        }

        private void OnSubmitBtnClick(object sender, RoutedEventArgs e)
        {
            //TODO: Make the action either create or update
        }
    }
}
