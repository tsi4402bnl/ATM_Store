using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TheUI
{
    class NameDays
    {
        public static string getNamedayNames()
        {
            string names = "";
            var reader = new StreamReader("Files/vardadienas.csv");
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var values = line.Split(';');
                DateTime calendarDay = Convert.ToDateTime(values[0]);
                DateTime now = DateTime.Today;

                //check if date is today
                if (calendarDay.Day == now.Day && calendarDay.Month == now.Month)
                {
                    names = values[1];
                    break;
                }
            }
            return names;
        }
    }
}
