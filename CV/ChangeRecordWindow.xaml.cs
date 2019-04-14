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
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для ChangeRecordWindow.xaml
    /// </summary>
    public partial class ChangeRecordWindow : Window
    {
        private List<ImageFrame> Frames { get; set; } = new List<ImageFrame>();
        AppRecord _appRecord;

        public ChangeRecordWindow()
        {
            InitializeComponent();
        }

        public ChangeRecordWindow(AppRecord appRecord) : this()
        {
            _appRecord = appRecord;
            AddClinicsNamesComboBox();
            SetRecordValues(_appRecord);
            SetRecordImages(_appRecord);
        }

        private void AddClinicsNamesComboBox()
        {
            try
            {
                String recClinicName = _appRecord.Rec.ClinicName;
                if (!AppConfig.ApplicationConfig.ClinicNames.Contains(recClinicName))
                    cmbClinics.Items.Add(recClinicName);

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

        /// <summary>
        /// Метод установки исходных значений записи
        /// </summary>
        /// <param name="appRecord"></param>
        private void SetRecordValues(AppRecord appRecord)
        {
            txtFirstName.Text = appRecord.Rec.FirstName;   // фамилия
            txtSecondName.Text = appRecord.Rec.SecondName; // имя
            txtLastName.Text = appRecord.Rec.LastName;     // отчество
            txtNumber.Text = appRecord.Rec.Id.ToString();  // id
            cmbClinics.Text = appRecord.Rec.ClinicName;    // наименование клиники
            txtDiagnosis.Text = appRecord.Rec.Diagnosis;   // диагноз
            datePickerBorn.SelectedDate = appRecord.Rec.DateBorn;

            docInformation.Blocks.Add(new Paragraph(new Run(appRecord.Rec.Description)));  // дополнительная информация
        }

        /// <summary>
        /// метод установки изображений
        /// </summary>
        /// <param name="appRecord"></param>
        private void SetRecordImages(AppRecord appRecord)
        {
            DirectoryInfo recordDir = new DirectoryInfo(appRecord.RecordPath);
            String pathImages = System.IO.Path.Combine(recordDir.FullName, "Images");

            // *.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png" - фильтр
            var files = Directory.GetFiles(pathImages, "*.*", SearchOption.AllDirectories);

            List<string> imageFiles = new List<string>();
            foreach (string filename in files)
            {
                if (Regex.IsMatch(filename.ToLower(), @"(.jpg|.png|.bmp|.jpeg)$"))
                    imageFiles.Add(filename);
            }

            foreach (var imageFile in imageFiles)
            {
                ImageFrame imageFrame = new ImageFrame(Frames.Count, imageFile, RemoveFrame);
                Frames.Add(imageFrame);
                listBoxPanel.Children.Add(imageFrame);
            }
        }

        // делегат удаления из контейнера
        private void RemoveFrame(ImageFrame imageFrame)
        {
            var result = MessageBox.Show("Удалить изображение?", "Удаление!", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) // если да то удаляем
            {
                Frames.Remove(imageFrame);
                listBoxPanel.Children.Remove(imageFrame);
            }
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


        /// <summary>
        /// Кнопка отмены
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
                this.Close();
            }
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
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

                Record newRecord = new Record(id, firstName, secondName, lastName, dateBorn, clinicName, diagnosis, description);
                Core.ChangeRecord(_appRecord, newRecord, imagesPath); // меняем запись
                MessageBox.Show("Запись успешно изменена!");
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


    }
}
