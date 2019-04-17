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

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для RecordViewWindow.xaml
    /// </summary>
    public partial class RecordViewWindow : Window
    {
        AppRecord _appRecord;
        private List<ImageFrame> Frames { get; set; } = new List<ImageFrame>();


        public RecordViewWindow()
        {
            InitializeComponent();
        }

        public RecordViewWindow(AppRecord record) : this()
        {
            _appRecord = record;
            SetRecordValues(record);
            SetRecordImages(record);
            AddClinicsNamesComboBox();
            Title = $"Просмотр записи: {record.Information}";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
            finally
            {

            }
        }

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
            MessageBox.Show("В режиме просмотра удаление невозможно!!!", "Предупреждение");
        }

        private void AddClinicsNamesComboBox()
        {
            try
            {
                String recClinicName = _appRecord.Rec.ClinicName;
                cmbClinics.Items.Add(recClinicName);
                cmbClinics.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Frames = null;
                listBoxPanel.Children.Clear();

                foreach (var frame in Frames)
                    frame.Dispose();

                GC.Collect(2,GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
            catch(Exception ex)
            {

            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Core.ClearMemory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чистке памяти!");
            }
        }
    }
}
