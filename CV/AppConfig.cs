using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace CV
{
    /// <summary>
    /// Класс конфигурации приложения
    /// </summary>
    static class AppConfig
    {
        public static String FileConfig { get; private set; } = "config.xml"; // путь до файла конфигурации
        public static Config ApplicationConfig { get; set; } = new Config(); // конфиг файл

        static AppConfig()
        {
            if (!File.Exists(FileConfig))
                CreteCongifFile();
            else
                ParseConfigFile();
        }

        /// <summary>
        /// Метод создания файла конфигурации и сериализации
        /// </summary>
        private static void CreteCongifFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            using (FileStream fs = new FileStream(FileConfig, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, ApplicationConfig);
            }
        }

        /// <summary>
        /// Метод парсер файла с настройками
        /// </summary>
        private static void ParseConfigFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            using (FileStream fs = new FileStream(FileConfig, FileMode.Open))
            {
                ApplicationConfig = (Config)serializer.Deserialize(fs);
            }
        }

        /// <summary>
        /// Метод сохранения настроек
        /// </summary>
        /// <param name="appConf">новый файл настроек для приложения</param>
        public static void SaveSetting(Config appConf)
        {
            ApplicationConfig = appConf;
            FileInfo appFile = new FileInfo(FileConfig);
            appFile.Delete();
            CreteCongifFile();
        }
    }
}
