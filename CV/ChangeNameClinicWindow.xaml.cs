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
    /// Логика взаимодействия для ChangeNameClinicWindow.xaml
    /// </summary>
    public partial class ChangeNameClinicWindow : Window
    {
        Clinic _clinic;

        public ChangeNameClinicWindow()
        {
            InitializeComponent();
        }

        public ChangeNameClinicWindow(Clinic clinic) : this()
        {
            _clinic = clinic;
            txtClinicName.Text = _clinic.Name;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String clinicName = txtClinicName.Text;
                if (String.IsNullOrWhiteSpace(clinicName) || String.IsNullOrEmpty(clinicName))
                    throw new Exception("Введите корректное наименование клиники!!!");


                _clinic.Name = clinicName;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка");
            }
            finally
            {

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка");
            }
        }
    }
}
