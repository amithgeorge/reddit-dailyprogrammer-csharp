using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDailyProgrammer.Answers._205Easy
{
    public static class DateTimeHelpers
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<DateTime> Today = () => DateTime.Today;
    }

    public static class DateTimeExtensions
    {
        public static string FullMonth(this DateTime date)
        {
            return date.ToString("MMMM");
        }

        public static string DayWithSuffix(this DateTime date)
        {
            return string.Format("{0}{1}", date.Day, GetDaySuffix(date.Day));
        }

        private static string GetDaySuffix(int day)
        {
            // http://stackoverflow.com/a/9130114/30007

            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }

    public class HumanReadableDateRange
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        public HumanReadableDateRange(DateTime @from, DateTime to)
        {
            _from = @from;
            _to = to;

            if (_from > _to)
            {
                throw new ArgumentException("From date should be less than or equal to To date");
            }
        }

        public string ToString(string format = "MDY")
        {
            var dateRangeType = _supportedCases.First(x => x.Item2.Invoke(_from, _to)).Item1;
            if (dateRangeType == DateRangeType.Unknown)
            {
                var msg = string.Format("Couldn't handle input - {0} to {1}", _from.ToShortDateString(), _to.ToShortDateString());
                throw new ApplicationException(msg);
            }

            return GetStringifyStrategy(format).Stringify(_from, _to, dateRangeType);
        }

        private readonly List<Tuple<DateRangeType, Func<DateTime, DateTime, bool>>> _supportedCases =
            new List<Tuple<DateRangeType, Func<DateTime, DateTime, bool>>>
            {
                // order is important 
                //  - not current year before current year
                //  - same date before same month before same year
                
                Tuple.Create(DateRangeType.SameDate,
                             (Func<DateTime, DateTime, bool>) IsSameDate),
                Tuple.Create(DateRangeType.SameMonthNotCurrYear,
                             (Func<DateTime, DateTime, bool>) IsSameMonthNotCurrentYear),
                Tuple.Create(DateRangeType.SameMonth,
                             (Func<DateTime, DateTime, bool>) IsSameMonth),
                Tuple.Create(DateRangeType.SameYearNotCurrYear,
                             (Func<DateTime, DateTime, bool>) IsSameYearNotCurrentYear),
                Tuple.Create(DateRangeType.SameYear,
                             (Func<DateTime, DateTime, bool>) IsSameYear),
                Tuple.Create(DateRangeType.DiffYearLessNotCurrYear,
                             (Func<DateTime, DateTime, bool>) IsDiffYearsButLessThanAYearApartAndNotCurrentYear),
                Tuple.Create(DateRangeType.DiffYearLess,
                             (Func<DateTime, DateTime, bool>) IsDiffYearsButLessThanAYearApart),
                Tuple.Create(DateRangeType.ExactlyAYear,
                             (Func<DateTime, DateTime, bool>) IsExactlyAnYearApart),
                Tuple.Create(DateRangeType.MoreThanAYear,
                             (Func<DateTime, DateTime, bool>) IsMoreThanAYearApart),

                // should always be the last item
                Tuple.Create(DateRangeType.Unknown, (Func<DateTime, DateTime, bool>) ((d1, d2) => true))
            };

        private IStringifyStrategy GetStringifyStrategy(string format)
        {
            switch (format)
            {
                case "MDY":
                    return new MDY_Strategy();
                case "DMY":
                    return new DMY_Strategy();
                case "YMD":
                    return new YMD_Strategy();
                case "YDM":
                    return new YDM_Strategy();
                default:
                    throw new ArgumentException("Format not supported - " + format);
            }
        }

        private static bool IsMoreThanAYearApart(DateTime date1, DateTime date2)
        {
            return date1.AddYears(1) < date2;
        }

        private static bool IsExactlyAnYearApart(DateTime date1, DateTime date2)
        {
            return date1.AddYears(1).Equals(date2);
        }

        private static bool IsDiffYearsButLessThanAYearApart(DateTime date1, DateTime date2)
        {
            return date2 < date1.AddYears(1) && date1.Year < date2.Year && date1.Year == DateTimeHelpers.Today().Year;
        }

        private static bool IsDiffYearsButLessThanAYearApartAndNotCurrentYear(DateTime date1, DateTime date2)
        {
            return date2 < date1.AddYears(1) && date1.Year < date2.Year && date1.Year != DateTimeHelpers.Today().Year;
        }

        private static bool IsSameYearNotCurrentYear(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Year != DateTimeHelpers.Today().Year;
        }

        private static bool IsSameDate(DateTime date1, DateTime date2)
        {
            return date1.Equals(date2);
        }

        private static bool IsSameMonthNotCurrentYear(DateTime date1, DateTime date2)
        {
            return date1.Year != DateTimeHelpers.Today().Year &&
                   date1.Year == date2.Year && date1.Month == date2.Month;
        }

        private static bool IsSameYear(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Year == DateTimeHelpers.Today().Year;
        }

        private static bool IsSameMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Year == DateTimeHelpers.Today().Year;
        }

        enum DateRangeType
        {
            Unknown = 0,
            SameDate = 1,
            SameMonth = 2,
            SameYear = 3,
            DiffYearLess = 4,
            ExactlyAYear = 5,
            MoreThanAYear = 6,
            SameMonthNotCurrYear = 7,
            SameYearNotCurrYear = 8,
            DiffYearLessNotCurrYear = 9
        }

        private interface IStringifyStrategy
        {
            string Stringify(DateTime date1, DateTime date2, DateRangeType dateRangeType);
        }

        // ReSharper disable once InconsistentNaming
        private class MDY_Strategy : IStringifyStrategy
        {
            public string Stringify(DateTime date1, DateTime date2, DateRangeType dateRangeType)
            {
                switch (dateRangeType)
                {
                    case DateRangeType.SameDate:
                        return string.Format("{0} {1}, {2}", date1.FullMonth(), date1.DayWithSuffix(), date1.Year);
                    case DateRangeType.SameMonth:
                        return string.Format("{0} {1} - {2}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameMonthNotCurrYear:
                        return string.Format("{0} {1}, {2} - {3}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date1.Year,
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYear:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYearNotCurrYear:
                        return string.Format("{0} {1} - {2} {3}, {4}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.Year);
                    case DateRangeType.DiffYearLessNotCurrYear:
                        return string.Format("{0} {1}, {2} - {3} {4}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date1.Year,
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.DiffYearLess:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.ExactlyAYear:
                    //                    return string.Format("{0} {1}, {2} - {3} {4}, {5}",
                    //                                         date1.FullMonth(),
                    //                                         date1.DayWithSuffix(),
                    //                                         date1.Year,
                    //                                         date2.FullMonth(),
                    //                                         date2.DayWithSuffix(),
                    //                                         date2.Year);
                    case DateRangeType.MoreThanAYear:
                        return string.Format("{0} {1}, {2} - {3} {4}, {5}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date1.Year,
                                             date2.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.Year);
                    default:
                        throw new ApplicationException("Unknown date range type - " + dateRangeType);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private class DMY_Strategy : IStringifyStrategy
        {
            public string Stringify(DateTime date1, DateTime date2, DateRangeType dateRangeType)
            {
                switch (dateRangeType)
                {
                    case DateRangeType.SameDate:
                        return string.Format("{0} {1}, {2}", date1.DayWithSuffix(), date1.FullMonth(), date1.Year);
                    case DateRangeType.SameMonth:
                        return string.Format("{0} - {1} {2}",
                                             date1.DayWithSuffix(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.SameMonthNotCurrYear:
                        return string.Format("{0} {1}, {2} - {3}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date1.Year,
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYear:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.SameYearNotCurrYear:
                        return string.Format("{0} {1} - {2} {3}, {4}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.Year);
                    case DateRangeType.DiffYearLessNotCurrYear:
                        return string.Format("{0} {1}, {2} - {3} {4}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date1.Year,
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.DiffYearLess:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.ExactlyAYear:
                    case DateRangeType.MoreThanAYear:
                        return string.Format("{0} {1}, {2} - {3} {4}, {5}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date1.Year,
                                             date2.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.Year);
                    default:
                        throw new ApplicationException("Unknown date range type - " + dateRangeType);
                }
            }
        }


        // ReSharper disable once InconsistentNaming
        private class YDM_Strategy : IStringifyStrategy
        {
            public string Stringify(DateTime date1, DateTime date2, DateRangeType dateRangeType)
            {
                switch (dateRangeType)
                {
                    case DateRangeType.SameDate:
                        return string.Format("{0}, {1} {2}", date1.Year, date1.DayWithSuffix(), date1.FullMonth());
                    case DateRangeType.SameMonth:
                        return string.Format("{0} - {1} {2}",
                                             date1.DayWithSuffix(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.SameMonthNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3}",
                                             date1.Year,
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYear:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.SameYearNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3} {4}",
                                             date1.Year,
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.DiffYearLessNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3} {4}",
                                             date1.Year,
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.DiffYearLess:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    case DateRangeType.ExactlyAYear:
                    case DateRangeType.MoreThanAYear:
                        return string.Format("{0}, {1} {2} - {3}, {4} {5}",
                                             date1.Year,
                                             date1.DayWithSuffix(),
                                             date1.FullMonth(),
                                             date2.Year,
                                             date2.DayWithSuffix(),
                                             date2.FullMonth());
                    default:
                        throw new ApplicationException("Unknown date range type - " + dateRangeType);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private class YMD_Strategy : IStringifyStrategy
        {
            public string Stringify(DateTime date1, DateTime date2, DateRangeType dateRangeType)
            {
                switch (dateRangeType)
                {
                    case DateRangeType.SameDate:
                        return string.Format("{0}, {1} {2}", date1.Year, date1.FullMonth(), date1.DayWithSuffix());
                    case DateRangeType.SameMonth:
                        return string.Format("{0} {1} - {2}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameMonthNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3}",
                                             date1.Year,
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYear:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.SameYearNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3} {4}",
                                             date1.Year,
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.DiffYearLessNotCurrYear:
                        return string.Format("{0}, {1} {2} - {3} {4}",
                                             date1.Year,
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.DiffYearLess:
                        return string.Format("{0} {1} - {2} {3}",
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    case DateRangeType.ExactlyAYear:
                    case DateRangeType.MoreThanAYear:
                        return string.Format("{0}, {1} {2} - {3}, {4} {5}",
                                             date1.Year,
                                             date1.FullMonth(),
                                             date1.DayWithSuffix(),
                                             date2.Year,
                                             date2.FullMonth(),
                                             date2.DayWithSuffix());
                    default:
                        throw new ApplicationException("Unknown date range type - " + dateRangeType);
                }
            }
        }
    }
}
