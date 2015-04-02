using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RedditDailyProgrammer.Answers._206Easy
{
    public class RecurrenceFormulaTests
    {
        [Fact]
        public void Can_compute_for_input1()
        {
            const string recFrmStr = "*3 +2 *2";
            var expected = new List<int> {0, 4, 28, 172, 1036, 6220, 37324, 223948};

            var nTerms = new RecurrenceRelation<int>(recFrmStr, 0).GetNTerms(7);
            Enumerable.Range(0, expected.Count)
                .ToList()
                .ForEach(i => Assert.True(nTerms[i] == expected[i]));
        }

        [Fact]
        public void Can_compute_for_input2()
        {
            const string recFrmStr = "*2 +1";
            var expected = new List<int> { 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047 };
            var nTerms = new RecurrenceRelation<int>(recFrmStr, 1).GetNTerms(10);
            Enumerable.Range(0, expected.Count)
                .ToList()
                .ForEach(i => Assert.True(nTerms[i] == expected[i]));
        }

        [Fact]
        public void Can_compute_for_input3()
        {
            const string recFrmStr = "*-2";
            var expected = new List<int> { 1, -2, 4, -8, 16, -32, 64, -128, 256 };
            var nTerms = new RecurrenceRelation<int>(recFrmStr, 1).GetNTerms(8);
            Enumerable.Range(0, expected.Count)
                .ToList()
                .ForEach(i => Assert.True(nTerms[i] == expected[i]));
        }

        [Fact]
        public void Can_compute_for_input4()
        {
            const string recFrmStr = "+2 *3 -5";
            var expected = new List<int> { 0, 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524};
            var nTerms = new RecurrenceRelation<int>(recFrmStr, 0).GetNTerms(10);
            Enumerable.Range(0, expected.Count)
                .ToList()
                .ForEach(i => Assert.True(nTerms[i] == expected[i]));
        }

        [Fact]
        public void Can_compute_when_n_is_0()
        {
            const string recFrmStr = "+2 *3 -5";
            var expected = new List<int> { 0, 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524 };
            var nTerms = new RecurrenceRelation<int>(recFrmStr, 0).GetNTerms(0);

            Assert.Equal(1, nTerms.Count);
            Assert.Equal(0, nTerms.First());
        }

        [Fact]
        public void Can_compute_when_n_is_1()
        {
            const string recFrmStr = "+2 *3 -5";
            var expected = new List<int> { 0, 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524 };
            var nTerms = new RecurrenceRelation<int>(recFrmStr, 0).GetNTerms(1);

            Assert.Equal(2, nTerms.Count);
            Assert.Equal(1, nTerms[1]);
        }
    }
}
