using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace CV
{
    static class Core
    {
        static int _nextId = 0;
        public static ObservableCollection<AppRecord> SelectedRecords = new ObservableCollection<AppRecord>(); // текущая коллекция по поиску, если поиск пуст то отображает все
        public static ObservableCollection<AppRecord> Records { get; set; } = new ObservableCollection<AppRecord>(); // коллекция записей

        // новое значение id
        public static Int32 NextId
        {
            get { _nextId++; return _nextId; }
        }


        /// <summary>
        /// проверка существует ли запись с указанным id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsExistID(Int32 id)
        {
            foreach (var appRec in Records)
                if (appRec.Rec.Id == id)
                    return true;
            return false;
        }

        /// <summary>
        /// метод добавления записи
        /// </summary>
        /// <param name="record">объект записи</param>
        /// <param name="imagesPath">пути изображений</param>
        public static void AddRecord(Record record, List<String> imagesPath)
        {
            String folderName = $"{record.Id} - {record.ShortFIO}"; // наименование папки
            DirectoryInfo recordsDir = new DirectoryInfo(AppConfig.ApplicationConfig.RecordsFolder); // прикрепляемся к папке с записями
            DirectoryInfo recordDir = recordsDir.CreateSubdirectory(folderName); // создаем каталог для записи
            DirectoryInfo recordImages = recordDir.CreateSubdirectory("Images"); // создаем каталог для изображений

            // копируем изображения
            foreach (var imagePath in imagesPath)
            {
                String imageName = Path.GetFileName(imagePath);
                String copyImage = Path.Combine(recordImages.FullName, imageName);
                File.Copy(imagePath, copyImage);
            }

            String recordXml = "record.xml";
            String recordFile = Path.Combine(recordDir.FullName, recordXml);
            SaveRecord(record,recordFile);

            AppRecord appRecord = new AppRecord(recordDir.FullName, record);

            Records.Add(appRecord);
            SelectedRecords.Add(appRecord);
        }

        /// <summary>
        /// Запись в файл объект записи
        /// </summary>
        /// <param name="recordFile"></param>
        /// <param name="record"></param>
        public static void SaveRecord(Record record, String recordFile)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Record));

            using (FileStream fs = new FileStream(recordFile, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, record);
            }
        }

        /// <summary>
        /// Метод удаления записи по id
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveRecord(Int32 id)
        {

            GC.Collect(0, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            foreach (var appRec in Records)
            {
                if (appRec.Rec.Id == id)
                {
                    RemoveRecord(appRec);
                    return;
                }
            }
            throw new Exception("Удаление по id не сработало, так как не нашлось записи с таким id!!!");
        }

        /// <summary>
        /// Удаление по ссылке
        /// </summary>
        /// <param name="appRecord"></param>
        public static void RemoveRecord(AppRecord appRecord)
        {
            Directory.Delete(appRecord.RecordPath, true);
            Records.Remove(appRecord);
            SelectedRecords.Remove(appRecord);
        }

        /// <summary>
        /// Изменение записи
        /// </summary>
        /// <param name="oldAppRecord"></param>
        /// <param name="newRecord"></param>
        /// <param name="newImages"></param>
        public static void ChangeRecord(AppRecord oldAppRecord, Record newRecord, List<String> newImages)
        {
            DirectoryInfo recDir = new DirectoryInfo(oldAppRecord.RecordPath);
            var recFiles = recDir.GetFiles("record.xml");

            if (recFiles.Length != 1)
                throw new Exception($"В данном каталоге {oldAppRecord.RecordPath} отсутствует файл с записью или их несколько!!!");

            var recFile = recFiles.First();
            recFile.Delete(); // удаляем

            String newRecPath = Path.Combine(oldAppRecord.RecordPath, "record.xml");
            SaveRecord(newRecord, newRecPath); // записываем новый файл

            // теперь уизображения

            String imagesPath = Path.Combine(recDir.FullName, "Images");
            DirectoryInfo imagesDir = new DirectoryInfo(imagesPath);

            var oldImages = imagesDir.GetFiles();

            // удаляем лишние
            foreach (var oldImage in oldImages)
            {
                if (!newImages.Contains(oldImage.FullName))
                    oldImage.Delete();
            }

            // копируем новые
            foreach (var newImage in newImages)
            {
                bool isExist = false;

                foreach (var oldImage in oldImages)
                {
                    if (newImage == oldImage.FullName)
                    {
                        isExist = true;
                        break;
                    }
                }

                // если не существует
                if (!isExist)
                {
                    String imageName = Path.GetFileName(newImage);
                    String copyImage = Path.Combine(imagesDir.FullName, imageName);
                    File.Copy(newImage, copyImage);
                }

            }

            String newRecordShortFIO = newRecord.ShortFIO;
            if (newRecordShortFIO != oldAppRecord.Rec.ShortFIO) // т.е. придется менять название каталога
            {
                String newPath = Path.Combine(AppConfig.ApplicationConfig.RecordsFolder, $"{newRecord.Id} - {newRecordShortFIO}");
                String oldPath = oldAppRecord.RecordPath;
                Directory.Move(oldPath, newPath); // переименуем
                oldAppRecord.RecordPath = newPath; // теперь ссылка на новую переименованную папку
            }

            oldAppRecord.Rec = newRecord;
        }

        /// <summary>
        /// поиск записей по шаблону
        /// </summary>
        /// <param name="template"></param>
        public static void SearchRecord(String template)
        {
            SelectedRecords.Clear();
            if (String.IsNullOrEmpty(template)||String.IsNullOrWhiteSpace(template))
            {
                foreach (var rec in Records)
                    SelectedRecords.Add(rec);
            }
            else
            {
                foreach (var rec in Records)
                {
                    if (IsTemplate(rec, template))
                        SelectedRecords.Add(rec);
                }
            }
        }

        /// <summary>
        /// Проверка соответствия записи шаблону
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private static bool IsTemplate(AppRecord rec, String template)
        {
            if (rec.Rec.Diagnosis.ToUpper().Contains(template))
                return true;
            if (rec.Rec.ClinicName.ToUpper().Contains(template))
                return true;
            if (rec.Rec.FIO.ToUpper().Contains(template))
                return true;
            return false;
        }



        /// <summary>
        /// метод загрузки записей
        /// </summary>
        public static void LoadRecords()
        {
            String recordsFolder = AppConfig.ApplicationConfig.RecordsFolder;
            DirectoryInfo recordsDir = new DirectoryInfo(recordsFolder);

            foreach (var dir in recordsDir.GetDirectories())
            {
                String recordPath = dir.FullName; // получаем путь папки записи
                Record rec = GetRecordByDir(recordPath);

                if (rec == null)
                    continue;

                AppRecord appRecord = new AppRecord(recordPath, rec);
                Records.Add(appRecord);
                SelectedRecords.Add(appRecord);
            }


            if (Records.Count!=0)
                _nextId = Records.Max(rec => rec.Rec.Id)+1; ;
        }

        /// <summary>
        /// Получение записи в директории
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static Record GetRecordByDir(String dirPath)
        {
            DirectoryInfo dirRecord = new DirectoryInfo(dirPath);
            var recordFiles = dirRecord.GetFiles("record.xml");

            if (recordFiles.Count() == 0)
                return null;

            FileInfo recordFile = dirRecord.GetFiles("record.xml").First();
            Record record = GetRecordByPath(recordFile.FullName);
            return record;
        }

        /// <summary>
        /// Десериализация записи по пути файла
        /// </summary>
        /// <param name="recordPath">путь файла с записью</param>
        /// <returns></returns>
        public static Record GetRecordByPath(String recordPath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Record));
            Record record = new Record();
            using (FileStream fs = new FileStream(recordPath, FileMode.Open))
            {
                record = (Record)xmlSerializer.Deserialize(fs);
            }
            return record;
        }
    }



    /// <summary>
    /// Класс отображения записей на пути их расположений, с этим классом и работает приложение
    /// </summary>
    public class AppRecord : INotifyPropertyChanged
    {
        Record _record;

        public String RecordPath { get; set; } // путь до папки с записью
        public Record Rec
        {
            get { return _record; }
            set
            {
                _record = value;
                OnPropertyChanged("Information");
            }
        }

        public String Information
        {
            get { return $"{_record.ShortFIO} \t({_record.Diagnosis}) \t: {_record.ClinicName} \t- {_record.DateCreated.ToShortDateString()}"; }
        }

        public AppRecord()
        {

        }

        public AppRecord(String recordPath, Record rec)
        {
            RecordPath = recordPath;
            Rec = rec;
        }


        public override string ToString()
        {
            return Rec.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
