using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RedditDailyProgrammer.Answers._205Easy
{
    public class DateRangePrinterTests
    {
        [Theory]
        [InlineData("MDY", "January 1st, 2015")]
        [InlineData("DMY", "1st January, 2015")]
        [InlineData("YDM", "2015, 1st January")]
        [InlineData("YMD", "2015, January 1st")]
        public void Returns_date_if_from_to_are_same(string format, string expected)
        {
            var @from = new DateTime(2015, 01, 01);
            var to = new DateTime(2015, 01, 01);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "January 1st - 5th")]
        [InlineData("DMY", "1st - 5th January")]
        [InlineData("YDM", "1st - 5th January")]
        [InlineData("YMD", "January 1st - 5th")]
        public void Can_handle_dates_in_same_month_current_year(string format, string expected)
        {
            var @from = new DateTime(2015, 01, 01);
            var to = new DateTime(2015, 01, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "January 1st, 2016 - 5th")]
        [InlineData("DMY", "1st January, 2016 - 5th")]
        [InlineData("YDM", "2016, 1st January - 5th")]
        [InlineData("YMD", "2016, January 1st - 5th")]
        public void Can_handle_dates_in_same_month_not_current_year(string format, string expected)
        {
            var @from = new DateTime(2016, 01, 01);
            var to = new DateTime(2016, 01, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "January 1st - February 5th, 2016")]
        [InlineData("DMY", "1st January - 5th February, 2016")]
        [InlineData("YDM", "2016, 1st January - 5th February")]
        [InlineData("YMD", "2016, January 1st - February 5th")]
        public void Can_handle_dates_in_same_year_not_current_year(string format, string expected)
        {
            var @from = new DateTime(2016, 01, 01);
            var to = new DateTime(2016, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "January 1st - February 5th")]
        [InlineData("DMY", "1st January - 5th February")]
        [InlineData("YDM", "1st January - 5th February")]
        [InlineData("YMD", "January 1st - February 5th")]
        public void Can_handle_dates_in_same_year_for_current_year(string format, string expected)
        {
            var @from = new DateTime(2015, 01, 01);
            var to = new DateTime(2015, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st, 2016 - February 5th")]
        [InlineData("DMY", "1st March, 2016 - 5th February")]
        [InlineData("YDM", "2016, 1st March - 5th February")]
        [InlineData("YMD", "2016, March 1st - February 5th")]
        public void Can_handle_dates_in_diff_year_but_less_than_a_year_apart_not_current_year(string format, string expected)
        {
            var @from = new DateTime(2016, 03, 01);
            var to = new DateTime(2017, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st - February 5th")]
        [InlineData("DMY", "1st March - 5th February")]
        [InlineData("YDM", "1st March - 5th February")]
        [InlineData("YMD", "March 1st - February 5th")]
        public void Can_handle_dates_in_diff_year_but_less_than_a_year_apart_for_current_year(string format, string expected)
        {
            var @from = new DateTime(2015, 03, 01);
            var to = new DateTime(2016, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st, 2016 - March 1st, 2017")]
        [InlineData("DMY", "1st March, 2016 - 1st March, 2017")]
        [InlineData("YDM", "2016, 1st March - 2017, 1st March")]
        [InlineData("YMD", "2016, March 1st - 2017, March 1st")]
        public void Can_handle_dates_exactly_a_year_apart_not_current_year(string format, string expected)
        {
            var @from = new DateTime(2016, 03, 01);
            var to = @from.AddYears(1);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st, 2015 - March 1st, 2016")]
        [InlineData("DMY", "1st March, 2015 - 1st March, 2016")]
        [InlineData("YDM", "2015, 1st March - 2016, 1st March")]
        [InlineData("YMD", "2015, March 1st - 2016, March 1st")]
        public void Can_handle_dates_exactly_a_year_apart_for_current_year(string format, string expected)
        {
            var @from = new DateTime(2015, 03, 01);
            var to = @from.AddYears(1);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st, 2016 - February 5th, 2018")]
        [InlineData("DMY", "1st March, 2016 - 5th February, 2018")]
        [InlineData("YDM", "2016, 1st March - 2018, 5th February")]
        [InlineData("YMD", "2016, March 1st - 2018, February 5th")]
        public void Can_handle_dates_more_than_a_year_apart_not_current_year(string format, string expected)
        {
            var @from = new DateTime(2016, 03, 01);
            var to = new DateTime(2018, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString(format);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MDY", "March 1st, 2016 - February 5th, 2018")]
        [InlineData("DMY", "1st March, 2016 - 5th February, 2018")]
        [InlineData("YDM", "2016, 1st March - 2018, 5th February")]
        [InlineData("YMD", "2016, March 1st - 2018, February 5th")]
        public void Can_handle_dates_more_than_a_year_apart_for_current_year(string format, string expected)
        {
            var @from = new DateTime(2015, 03, 01);
            var to = new DateTime(2018, 02, 05);
            DateTimeHelpers.Today = () => new DateTime(2015, 03, 31);

            var result = new HumanReadableDateRange(@from, to).ToString();

            Assert.Equal("March 1st, 2015 - February 5th, 2018", result);
        }
    }

    
}
