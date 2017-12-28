using System;
using System.Collections.Generic;
using System.Globalization;

namespace Zhuang.Commons.Utils
{
    public class WeekModel
    {
        public int Year { get; set; }

        public int Week { get; set; }

        public DateTime FirstDate
        {
            get
            {
                return DateUtil.GetFirstDateOfWeek(Week, Year);
            }
        }

        public DateTime LastDate {
            get
            {
                return DateUtil.GetLastDateOfWeek(Week, Year);
            }
        }

    }

    public static class DateUtil
    {
        public static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public static Calendar GetCalendar()
        {

            return new GregorianCalendar();

        }

        public static int GetWeekOfYear(DateTime dt)
        {
            var calendar = GetCalendar();

            int weekOfYear = calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            return weekOfYear;
        }

        public static int GetCurrentWeek()
        {
            return GetWeekByIndex(0);
        }

        public static int GetNextWeek()
        {
            return GetWeekByIndex(1);
        }

        public static DateTime GetToday()
        {
            return DateTime.Parse(GetNow().ToString("yyyy-MM-dd"));
        }

        public static DateTime GetTomorrow()
        {
            return DateTime.Parse(GetNow().AddDays(1).ToString("yyyy-MM-dd"));
        }

        public static int GetPreviousWeek()
        {
            return GetWeekByIndex(-1);
        }

        public static int GetWeekByIndex(int index)
        {
            var dt = GetDtByWeekIndex(index);

            return GetWeekOfYear(dt);
        }

        public static int GetYearByWeekIndex(int index)
        {
            var dt = GetDtByWeekIndex(index);
            return dt.Year;
        }

        public static DateTime GetDtByWeekIndex(int index)
        {
            var dtNow = GetNow();
            var calendar = GetCalendar();

            var result = dtNow;
            var dtLast = dtNow;
            var dtFirst = dtNow;
            
            if (index < 0)
            {
                dtLast = GetLastDateOfWeek(dtLast);
                var isDiffYear = false;

                for (int i = 0; i < Math.Abs(index); i++)
                {
                    var dtTemp = calendar.AddWeeks(dtLast, -1);
                    if (isDiffYear)
                    {
                        dtTemp = GetLastDateOfWeek(dtTemp);
                        isDiffYear = false;
                    }
                    if (dtTemp.Year == dtLast.Year)
                    {
                        dtLast = dtTemp;
                    }
                    else
                    {
                        dtLast = DateTime.Parse(string.Format("{0}-01-01",dtLast.Year)).AddDays(-1);
                        isDiffYear = true;
                    }
                }

                result = dtLast;
            }
            else if (index > 0)
            {
                dtFirst = GetFirstDateOfWeek(dtFirst);
                var isDiffYear = false;

                for (int i = 0; i < Math.Abs(index); i++)
                {
                    
                    var dtTemp = calendar.AddWeeks(dtFirst, 1);

                    if (isDiffYear)
                    {
                        dtTemp =GetFirstDateOfWeek(dtTemp);
                        isDiffYear = false;
                    }

                    if (dtTemp.Year == dtFirst.Year)
                    {
                        dtFirst = dtTemp;
                    }
                    else
                    {
                        dtFirst = DateTime.Parse(string.Format("{0}-01-01", dtTemp.Year));
                        isDiffYear = true;
                    }
                }

                result = dtFirst;
            }

            return result;
            //var dt = calendar.AddWeeks(dtNow, index);

            //return dt;
        }

        public static IList<DateTime> GetDatesOfWeek(int week)
        {
            return GetDatesOfWeek(week, GetNow().Year);
        }

        public static IList<DateTime> GetDatesOfWeek(int week, int year)
        {
            IList<DateTime> lsReuslt = new List<DateTime>();

            DateTime dtFirstDayOfYear = DateTime.Parse(string.Format("{0}-01-01", year));

            var totalDays = GetCalendar().GetDaysInYear(year);

            for (int i = 0; i < totalDays; i++)
            {
                DateTime tempDt = dtFirstDayOfYear.AddDays(i);
                if (GetWeekOfYear(tempDt) == week)
                {
                    lsReuslt.Add(tempDt);
                }
            }
            
            return lsReuslt;

        }

        public static DateTime GetFirstDateOfWeek(int week)
        {
            return GetFirstDateOfWeek(week, GetNow().Year);
        }

        public static DateTime GetFirstDateOfWeek(int week, int year)
        {
            var dates = GetDatesOfWeek(week, year);

            return dates[0];
        }

        public static DateTime GetFirstDateOfWeek(DateTime dt)
        {
            var year = dt.Year;
            var week = GetWeekOfYear(dt);
            return GetFirstDateOfWeek(week, year);
        }
        
        public static DateTime GetLastDateOfWeek(int week)
        {
            return GetLastDateOfWeek(week, GetNow().Year);
        }

        public static DateTime GetLastDateOfWeek(int week, int year)
        {
            var dates = GetDatesOfWeek(week, year);
            return dates[dates.Count-1];
        }

        public static DateTime GetLastDateOfWeek(DateTime dt)
        {
            var year = dt.Year;
            var week = GetWeekOfYear(dt);
            return GetLastDateOfWeek(week, year);
        }
        
        public static int GetWeekIndex(int week,int year)
        {
            int index = 0;

            var dtFirstDay = GetFirstDateOfWeek(week, year);

            var dtFirstDay4Now = GetFirstDateOfWeek(GetCurrentWeek(), GetNow().Year);

            if (dtFirstDay == dtFirstDay4Now)
            {
                index = 0;
            }
            else if (dtFirstDay > dtFirstDay4Now)
            {
                var tempDtFirstDay = dtFirstDay4Now;
                while (tempDtFirstDay != dtFirstDay)
                {
                    index = index + 1;

                    tempDtFirstDay = GetFirstDateOfWeek( GetWeekByIndex(index), GetYearByWeekIndex(index));
                }
            }
            else
            {
                var tempDtFirstDay = dtFirstDay4Now;
                while (tempDtFirstDay != dtFirstDay)
                {
                    index = index - 1;

                    tempDtFirstDay = GetFirstDateOfWeek(GetWeekByIndex(index), GetYearByWeekIndex(index));
                }
            }

            return index;
        }
        
        public static int GetWeekCount(DateTime startDate, DateTime endDate)
        {
            return GetWeekList(startDate, endDate).Count;
        }

        public static IList<WeekModel> GetWeekList(DateTime startDate, DateTime endDate)
        {
            var result = new List<WeekModel>();

            if (GetFirstDateOfWeek(startDate) == startDate)
            {
                result.Add(new WeekModel() { Year = startDate.Year, Week = GetWeekOfYear(startDate) });
            }

            var tempDate = startDate;
            var preWeek = GetWeekOfYear(tempDate);


            while (tempDate <= endDate)
            {
                var tempWeek = GetWeekOfYear(tempDate);
                if (tempWeek != preWeek)
                {
                    result.Add(new WeekModel() { Year = tempDate.Year, Week = GetWeekOfYear(tempDate) });
                }

                tempDate = tempDate.AddDays(1);
                preWeek = tempWeek;

            }

            return result;
        }

        public static IList<DateTime> GetDateList(DateTime startDate, DateTime endDate)
        {
            IList<DateTime> result = new List<DateTime>();

            while (startDate <= endDate)
            {
                result.Add(startDate);
                startDate = startDate.AddDays(1);
            }

            return result;
        }

        public static DateTime GetFirstDayOfMonth(int year, int month)
        {
            return DateTime.Parse(string.Format("{0}-{1}", year.ToString(), month < 10 ? "0" + month.ToString() : month.ToString()));
        }

        public static DateTime GetLastDayOfMonth(int year, int month)
        {
            var firstDay = GetFirstDayOfMonth(year,month);

            var nextMonthFirstDay = firstDay.AddMonths(1);

            return nextMonthFirstDay.AddDays(-1);
        }

        public static string GetDayOfWeekName(DateTime dt)
        {
            string result = string.Empty;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = "日";
                    break;
                case DayOfWeek.Monday:
                    result = "一";
                    break;
                case DayOfWeek.Tuesday:
                    result = "二";
                    break;
                case DayOfWeek.Wednesday:
                    result = "三";
                    break;
                case DayOfWeek.Thursday:
                    result = "四";
                    break;
                case DayOfWeek.Friday:
                    result = "五";
                    break;
                case DayOfWeek.Saturday:
                    result = "六";
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
