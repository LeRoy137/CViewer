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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для ImageFrame.xaml
    /// </summary>
    public partial class ImageFrame : UserControl, IDisposable
    {
        int id; // id элемента
        String imagePath;   // изображение 
        Action<ImageFrame> _closeImage;
        BitmapImage _image = new BitmapImage();

        public String ImagePath
        {
            get { return imagePath; }
            private set { imagePath = value; }
        }

        public int ID
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
            }
        }

        public ImageFrame()
        {
            InitializeComponent();
        }

        public ImageFrame(int id, String imagePath, Action<ImageFrame> closeImage) : this()
        {

            ID = id;
            ImagePath = imagePath;
            SetImage(imagePath);
            _closeImage = closeImage;
        }


        /// <summary>
        /// метод установки изображения
        /// </summary>
        /// <param name="imagePath">путь к изображению</param>
        private void SetImage(String imagePath)
        {

            var stream = File.OpenRead(imagePath);
            _image.BeginInit();
            _image.CacheOption = BitmapCacheOption.OnLoad;
            _image.StreamSource = stream;
            _image.EndInit();
            stream.Close();
            image.Source = _image;
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageViewWindow windowImageView = new ImageViewWindow(imagePath);
            windowImageView.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ImageFrame _imageFrame = this;
            _closeImage(_imageFrame);
        }

        public void Dispose()
        {
           
            image.Source = null;
        }
    }
}
