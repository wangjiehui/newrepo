using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace group_project
{
    /// <summary>
    /// Interaction logic for AddAppointmentWindow.xaml
    /// </summary>
    public partial class AddAppointmentWindow : Window
    {
        private static string filePath = "./appointments.json";
        private static string[] time = { "09:00 AM", "10:00 AM", "11:00 AM", "13:00 PM", "14:00 PM", "15:00 PM", "16:00 PM" };
        private static string[] inOffice = { "Y", "N"};
        private Appointment originalAppointment { get; set; }
        public AddAppointmentWindow()
        {
            InitializeComponent();
            //Set default selected date as today
            datePicker.Text = "";
            datePicker.SelectedDate = DateTime.Today;
            //Set display date limit
            datePicker.DisplayDateStart = DateTime.Today;
            originalAppointment = null;
            timeCbox.ItemsSource = time;
            inOfficeCbox.ItemsSource = inOffice;
        }

        public AddAppointmentWindow(Appointment SeletedItem)
        {
            InitializeComponent();
            datePicker.DisplayDateStart = DateTime.Today;
            originalAppointment = SeletedItem;
            DataContext = originalAppointment;
            timeCbox.ItemsSource = time;
            inOfficeCbox.ItemsSource = inOffice;
            // Change button content
            confirmBtn.Content = "Save";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsFields = DataValidation();
            if (errorsFields.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                errorsFields.ForEach(e => sb.Append(e.ToString() + "\n"));
                MessageBox.Show($"Please fill in the required fields below:\n{sb.ToString()}");
                return;
            }           
            if (originalAppointment == null)
            {
                //It's a add action
                Appointment newAppointment = new Appointment();
                newAppointment.Date = datePicker.Text;
                newAppointment.Time = timeCbox.Text;
                newAppointment.InOffice = inOfficeCbox.Text;
                newAppointment.ClientName = cName.Text;
                newAppointment.Species = species.Text;
                newAppointment.Address = address.Text;
                newAppointment.Type = type.Text;
                newAppointment.PatientName = pName.Text;
                newAppointment.Note = note.Text;
                MainWindow mainWindow = Owner as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Appointments.Add(newAppointment);
                    //Save data
                    WriteIntoFile();
                }
                Close();
            }
            else
            {
                //It's a edit action
                MainWindow mainWindow = Owner as MainWindow;
                if (mainWindow != null)
                {
                    //Before add, remove the original element first
                    mainWindow.Appointments.Remove(originalAppointment);
                }
                originalAppointment.Date = datePicker.Text;
                originalAppointment.Time = timeCbox.Text;
                originalAppointment.InOffice = inOfficeCbox.Text;
                originalAppointment.ClientName = cName.Text;
                originalAppointment.Species = species.Text;
                originalAppointment.Address = address.Text;
                originalAppointment.Type = type.Text;
                originalAppointment.PatientName = pName.Text;
                originalAppointment.Note = note.Text;
                
                if (mainWindow != null)
                {
                    mainWindow.Appointments.Add(originalAppointment);
                    //Save data
                    WriteIntoFile();
                }
                Close();
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WriteIntoFile()
        {
            MainWindow mainWindow = Owner as MainWindow;
            if (mainWindow != null)
            {
                //Clean all data of the file
                File.WriteAllText(filePath, "");
                //Rewrite data into the file
                string json = JsonSerializer.Serialize(mainWindow.Appointments);
                File.WriteAllText(filePath, json);
            }
            
        }
        private void inOfficeCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(null != inOfficeCbox.SelectedItem)
            {
                string content = inOfficeCbox.SelectedItem.ToString();
                if (content.Equals("Y"))
                {
                    address.IsEnabled = false;
                    address.Text = "";
                }
                if (content.Equals("N"))
                {
                    address.IsEnabled = true;
                }
            }            
        }

        private List<string> DataValidation()
        {
            List<string> errorsFields = new List<string>();
            if (String.IsNullOrEmpty(datePicker.Text))
            {
                datePicker.BorderBrush = Brushes.Red;
                errorsFields.Add("Date");
            }
            if (String.IsNullOrEmpty(timeCbox.Text))
            {
                timeCbox.BorderBrush = Brushes.Red;
                errorsFields.Add("Time");
            }
            if (String.IsNullOrEmpty(inOfficeCbox.Text))
            {
                inOfficeCbox.BorderBrush = Brushes.Red;
                errorsFields.Add("Time");
            }
            if (String.IsNullOrEmpty(cName.Text))
            {
                cName.BorderBrush = Brushes.Red;
                errorsFields.Add("Client Name");
            }
            if (String.IsNullOrEmpty(species.Text))
            {
                species.BorderBrush = Brushes.Red;
                errorsFields.Add("Patient Species");
            }
            if (inOfficeCbox.Text.Equals("N") && String.IsNullOrEmpty(address.Text))
            {
                address.BorderBrush = Brushes.Red;
                errorsFields.Add("Appointment Address");
            }
            if (String.IsNullOrEmpty(type.Text))
            {
                type.BorderBrush = Brushes.Red;
                errorsFields.Add("Appointment Type");
            }
            if (String.IsNullOrEmpty(pName.Text))
            {
                pName.BorderBrush = Brushes.Red;
                errorsFields.Add("Patient Name");
            }
            if (String.IsNullOrEmpty(note.Text))
            {
                note.BorderBrush = Brushes.Red;
                errorsFields.Add("Note");
            }
            return errorsFields;            
        }
    }
}
