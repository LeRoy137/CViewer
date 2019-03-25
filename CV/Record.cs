using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV
{
    [Serializable]
    public class Record
    {
        public Int32 Id { get; set; } // id записи

        public String FirstName { get; set; } // фамилия
        public String SecondName { get; set; } // имя
        public String LastName { get; set; } // отчество
        public DateTime? DateBorn { get; set; } // дата рождения
        public String ClinicName { get; set; } // наименование клиники за которой закреплена запись
        public String Diagnosis { get; set; } // диагноз
        public String Description { get; set; } // дополнительная информация по записи
        public DateTime DateCreated { get; set; } // дата создания записи

        public String FIO => FirstName + " " + SecondName + " " + LastName + " "; // полное фио
        public String ShortFIO
        {
            get
            {
                String shortFIO = "";
                shortFIO += FirstName + " ";
                shortFIO += (SecondName != "") ? SecondName[0] + ". " : "";
                shortFIO += (LastName != "") ? LastName[0] + ". " : "";
                return shortFIO;
            }
        }

        // конструктор по умолчанию для сериализации
        public Record()
        {

        }

        public Record(Int32 id, String firstName, String secondName, String lastName, DateTime? dateBorn, String clinicName, String diagnosis, String description)
        {
            Id = id;
            FirstName = firstName;
            SecondName = secondName;
            LastName = lastName;
            DateBorn = dateBorn;
            ClinicName = clinicName;
            Diagnosis = diagnosis;
            Description = description;
            DateCreated = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{ShortFIO}: {Diagnosis} - {DateCreated.ToShortDateString()}";
        }

    }
}
