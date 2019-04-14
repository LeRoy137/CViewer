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

namespace CV
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitApp.InitApplication();
            listBoxMain.ItemsSource = Core.SelectedRecords;
        }

        private void menuExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes) // если да то закрываем приложение
                    Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!!!");
            }
        }

        private void menuSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsWindow windowSetting = new SettingsWindow();
                windowSetting.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
            finally
            {

            }
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AboutWindow aboutWindow = new AboutWindow();
                aboutWindow.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String template = txtSearch.Text.ToUpper();
                Core.SearchRecord(template);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
            finally
            {

            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddRecordWindow windowAddRecord = new AddRecordWindow();
                windowAddRecord.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
        }

        private void btnInteractiveMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Controls.ListBoxItem listItem = sender as System.Windows.Controls.ListBoxItem;
                AppRecord recordView = ((AppRecord)listItem.DataContext);

                RecordViewWindow windowRecordView = new RecordViewWindow(recordView);
                windowRecordView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
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
                AppRecord recordChange = ((AppRecord)btn.DataContext);
                ChangeRecordWindow changeRecordWindow = new ChangeRecordWindow(recordChange);
                changeRecordWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка");
            }
            finally
            {

            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
                AppRecord recordRemove = ((AppRecord)btn.DataContext);

                var result = System.Windows.MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление Записи", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes) // если да то закрываем приложение
                {
                    Core.RemoveRecord(recordRemove);
                    MessageBox.Show("Запись успешно удалена!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Ошибка!!!");
            }
        }
    }
}
