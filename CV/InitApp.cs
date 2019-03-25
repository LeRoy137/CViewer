using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CV
{
    class InitApp
    {
        public static void InitApplication()
        {
            CheckDirectories();
            LoadRecords();
        }


        private static void CheckDirectories()
        {
            DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

            String recordsFolder = AppConfig.ApplicationConfig.RecordsFolder;

            if (!Directory.Exists(recordsFolder)) // проверка наличия папки с записями
                currentDir.CreateSubdirectory(recordsFolder);

            String logsFolder = AppConfig.ApplicationConfig.LogsFolder;

            if (!Directory.Exists(logsFolder))
               currentDir.CreateSubdirectory(logsFolder);

        }

        private static void LoadRecords()
        {
            Core.LoadRecords();
        }
    }
}
