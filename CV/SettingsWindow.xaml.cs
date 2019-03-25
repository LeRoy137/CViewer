using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.IO;

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        String recDir = String.Empty;
        String logsDir = String.Empty;
        ObservableCollection<Clinic> clinicNames = new ObservableCollection<Clinic>();

        public SettingsWindow()
        {
            InitializeComponent();
            InitFieldAndBinding();
        }

        private void btnAddClinic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddClinic addClinic = new AddClinic();

                AddNameClinicWindow windowaAddClinicName = new AddNameClinicWindow(addClinic);
                windowaAddClinicName.ShowDialog();

                if (addClinic.IsAdd)
                    clinicNames.Add(addClinic.Clinic);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Произошла ошибка!!!");
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
                System.Windows.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
            finally
            {

            }
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 maxCountLoadImage = Int32.Parse(txtMaxCountImages.Text);
                recDir = txtRecordsFolder.Text;
                logsDir = txtLogsFolder.Text;

                List<String> clinics = new List<string>();

                foreach (var cn in clinicNames)
                    clinics.Add(cn.Name);

                Config config = new Config(maxCountLoadImage, AppConfig.ApplicationConfig.RecordsFolder, AppConfig.ApplicationConfig.LogsFolder, clinics);
                AppConfig.SaveSetting(config);

                System.Windows.MessageBox.Show("Настройки успешно изменены");
                this.Close();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
            finally
            {

            }
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
                Clinic clinic = (Clinic)btn.DataContext;

                ChangeNameClinicWindow windowChangeClinic = new ChangeNameClinicWindow(clinic);
                windowChangeClinic.ShowDialog();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Произошла ошибка!!!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = System.Windows.MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление названия клиники", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes) // если да то закрываем приложение
                {
                    System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
                    clinicNames.Remove((Clinic)btn.DataContext);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Произошла ошибка!!!");
            }
        }


        private void InitFieldAndBinding()
        {
            DirectoryInfo diRec = new DirectoryInfo(AppConfig.ApplicationConfig.RecordsFolder);
            DirectoryInfo diLogs = new DirectoryInfo(AppConfig.ApplicationConfig.LogsFolder);
            txtRecordsFolder.Text = diRec.FullName;
            txtLogsFolder.Text = diLogs.FullName;
            txtMaxCountImages.Text = AppConfig.ApplicationConfig.MaxImageLoad.ToString();
            listClinicNames.ItemsSource = clinicNames;

            LoadClinicNames();
        }

        private void LoadClinicNames()
        {
            foreach (var clinicName in AppConfig.ApplicationConfig.ClinicNames)
                clinicNames.Add(new Clinic(clinicName));
        }
    }


    public class AddClinic
    {
        public bool IsAdd { get; set; }
        public Clinic Clinic { get; set; }

        public AddClinic()
        {

        }

        public AddClinic(Clinic clinic)
        {
            Clinic = clinic;
        }
    }


    public class Clinic : INotifyPropertyChanged
    {
        private String _name;

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public Clinic()
        {

        }

        public Clinic(String name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString() => Name;
    }
}
