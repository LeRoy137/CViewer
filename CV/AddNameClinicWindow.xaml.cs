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

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для AddNameClinicWindow.xaml
    /// </summary>
    public partial class AddNameClinicWindow : Window
    {
        AddClinic _addClinic;

        public AddNameClinicWindow()
        {
            InitializeComponent();
        }

        public AddNameClinicWindow(AddClinic addClinic) : this()
        {
            _addClinic = addClinic;
            _addClinic.IsAdd = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String clinicName = txtClinicName.Text;

                if (String.IsNullOrWhiteSpace(clinicName))
                    throw new Exception("Название клиники не может быть пустым");

                _addClinic.IsAdd = true;
                _addClinic.Clinic = new Clinic(clinicName);
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _addClinic.IsAdd = false;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
