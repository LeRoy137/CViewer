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
    /// Логика взаимодействия для ImageViewWindow.xaml
    /// </summary>
    public partial class ImageViewWindow : Window
    {
        public ImageViewWindow()
        {
            InitializeComponent();
        }

        public ImageViewWindow(String imagePath) : this()
        {
            SetImage(imagePath);
            String fileName = System.IO.Path.GetFileName(imagePath);
            this.Title = fileName;
        }

        /// <summary>
        /// метод установки изображения
        /// </summary>
        /// <param name="imagePath">путь к изображению</param>
        private void SetImage(String imagePath)
        {
            imgElement.Source = new BitmapImage(new Uri(imagePath));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Core.ClearMemory();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка при чистке памяти!");
            }
        }
    }
}
