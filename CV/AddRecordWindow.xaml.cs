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
using Microsoft.Win32;

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для AddRecordWindow.xaml
    /// </summary>
    public partial class AddRecordWindow : Window
    {
        private List<ImageFrame> Frames { get; set; } = new List<ImageFrame>();

        public AddRecordWindow()
        {
            InitializeComponent();
            SetInitParametrs();
        }

        private void btnAddImages_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
                openFileDialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
                openFileDialog.ShowDialog();

                string[] imagesPath = openFileDialog.FileNames; // пути к файлам

                if (imagesPath.Length + Frames.Count > AppConfig.ApplicationConfig.MaxImageLoad)
                {
                    MessageBox.Show($"Максимальное число для загрузки изображение - {AppConfig.ApplicationConfig.MaxImageLoad}");
                    return;
                }

                for (int i = 0; i < imagesPath.Length; i++)
                {
                    ImageFrame imageFrame = new ImageFrame(i, imagesPath[i], RemoveFrame);
                    listBoxPanel.Children.Add(imageFrame);
                    Frames.Add(imageFrame);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<String> imagesPath = new List<string>();

                foreach (var frame in Frames)
                    imagesPath.Add(frame.ImagePath);

                Int32 id = Int32.Parse(txtNumber.Text);           // id записи
                String firstName = txtFirstName.Text;             // фамилия
                String secondName = txtSecondName.Text;           // имя
                String lastName = txtLastName.Text;               // отчество
                DateTime? dateBorn = datePickerBorn.SelectedDate; // дата рождения, может быть null
                String clinicName = cmbClinics.Text;              // наименование клиники
                String diagnosis = txtDiagnosis.Text;
                String description = (new TextRange(docInformation.ContentStart, docInformation.ContentEnd)).Text;

                if (Core.IsExistID(id))
                    throw new Exception("Запись с таким номером уже есть!!!");

                Record rec = new Record(id, firstName, secondName, lastName, dateBorn, clinicName, diagnosis, description);
                Core.AddRecord(rec, imagesPath);

                MessageBox.Show("Запись успешно создана!!!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
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
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int widthChangeOrientation = 750;
            try
            {
                if (this.Width < widthChangeOrientation)
                {
                    stackInputsName.Orientation = Orientation.Vertical;
                    stackPanelClinic.Orientation = Orientation.Vertical;
                }
                else
                {
                    stackInputsName.Orientation = Orientation.Horizontal;
                    stackPanelClinic.Orientation = Orientation.Horizontal;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
            }
        }

        // делегат удаления из контейнера
        private void RemoveFrame(ImageFrame imageFrame)
        {
            Frames.Remove(imageFrame);
            listBoxPanel.Children.Remove(imageFrame);
        }

        private void SetInitParametrs()
        {
            try
            {
                AddComboBoxClinicNames();
                SetNextId();
            }
            catch(Exception ex)
            {

            }
        }

        private void AddComboBoxClinicNames()
        {
            try
            {
                foreach (var clinic in AppConfig.ApplicationConfig.ClinicNames)
                    cmbClinics.Items.Add(clinic);

                if (AppConfig.ApplicationConfig.ClinicNames.Count != 0)
                    cmbClinics.Text = AppConfig.ApplicationConfig.ClinicNames[0];
                else
                    MessageBox.Show("Отсутствуют наименования клиник в конфигурационном файле!!!");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        private void SetNextId()
        {
            txtNumber.Text = Core.NextId.ToString();
        }

    }
}
