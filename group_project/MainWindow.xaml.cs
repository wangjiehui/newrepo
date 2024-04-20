using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text;
using System.Text.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace group_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //File Path
        private static string filePath = "./appointments.json";
        public ObservableCollection<Appointment> Appointments { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //If file not exists, create a new file
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                Appointments = new ObservableCollection<Appointment>();
            }
            else
            {
                //If file exists, fetch data from the JSON file
                string jsonString = File.ReadAllText(filePath);
                if (String.IsNullOrEmpty(jsonString))
                {
                    Appointments = new ObservableCollection<Appointment>();
                }
                else
                {
                    Appointments = JsonSerializer.Deserialize<ObservableCollection<Appointment>>(jsonString);
                }
                appointmentListView.ItemsSource = Appointments;
            }
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            AddAppointmentWindow addWindow = new AddAppointmentWindow();
            addWindow.Owner = this;
            addWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //Get ojb from dataContext
            Appointment selectedItem = (sender as FrameworkElement).DataContext as Appointment;
            if (selectedItem != null)
            {
                //Pass data to the new window
                AddAppointmentWindow addAppointmentWindow = new AddAppointmentWindow(selectedItem);
                addAppointmentWindow.Owner = this;
                addAppointmentWindow.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Get ojb from dataContext
            Appointment selectedItem = (sender as FrameworkElement).DataContext as Appointment;
            //Delete the selected item
            Appointments.Remove(selectedItem);
            //Clean all data of the file
            File.WriteAllText(filePath, "");
            //Rewrite data into the file
            string json = JsonSerializer.Serialize(Appointments);
            File.WriteAllText(filePath, json);

        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            //Get ojb from dataContext
            Appointment selectedItem = (sender as FrameworkElement).DataContext as Appointment;
            if (selectedItem != null)
            {
                //Pass data to the new window
                AppointmentDetailWindow detailsWindow = new AppointmentDetailWindow(selectedItem);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }
    }
}