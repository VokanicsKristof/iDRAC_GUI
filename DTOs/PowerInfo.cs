using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDRAC_IPMI.DTOs
{
    public class PowerInfo
    {
        public string StatisticName { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string Reading { get; set; }
        public string AvgPerDayConsumption { get; set; }
        public PowerInfo(string Name, string StartDate, string EndDate, string Reading)
        {
            StatisticName = Name;
            StartTime = StartDate;
            FinishTime = EndDate;
            this.Reading = Reading;

            AvgPerDayConsumption = "na";

            if (Name.ToLower().Contains("cumulative"))
            {
                DateTime start = NormailzedDate(StartTime);
                DateTime finish = NormailzedDate(FinishTime);
                double timeBetween = (finish - start).TotalDays;

                double.TryParse(Reading.Split(" ")[0], out double consumption);
                AvgPerDayConsumption = Math.Round((consumption / timeBetween), 4).ToString() + Reading.Split(" ")[1];
            }
        }

        private DateTime NormailzedDate(string Date)
        {
            string[] startDate = Date.Split(' ', StringSplitOptions.RemoveEmptyEntries); // Format: Fri Sep  5 13:22:41 2025
            int Month = MonthToNumber(startDate[1]);
            int Day = int.Parse(startDate[2]);
            string[] Time = startDate[3].Split(":");
            int Hour = int.Parse(Time[0]);
            int Minute = int.Parse(Time[1]);
            int Second = int.Parse(Time[2]);
            int Year = int.Parse(startDate[4]);
            return new DateTime(Year, Month, Day, Hour, Minute, Second);
        }

        private int MonthToNumber(string Month) 
        {
            return Array.IndexOf(["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], Month) + 1;
        }
    }
}
