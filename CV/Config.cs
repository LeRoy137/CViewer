using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV
{
    [Serializable]
    public class Config
    {
        int _maxImageLoad = 10;

        public Int32 MaxImageLoad
        {
            get { return _maxImageLoad; }
            set
            {
                if (value <= 0 || value >= 15)
                    throw new Exception("Максимальное число изображений для загрузки в пределах от 1 до 15");

                _maxImageLoad = value;
            }
        }

        public String RecordsFolder { get; set; } = "Records"; // папка с записями
        public String LogsFolder { get; set; } = "Logs"; // папка с логами

        public List<String> ClinicNames { get; set; } = new List<string>(); // наименования клиник

        // конструктор по умолчанию для сериализации в Xml
        public Config()
        {

        }

        public Config(int maxImageLoad, String recordsFolder, String logsFolder, List<String> clinicNames)
        {
            MaxImageLoad = maxImageLoad;
            RecordsFolder = recordsFolder;
            LogsFolder = logsFolder;
            ClinicNames = clinicNames;
        }
    }
}
