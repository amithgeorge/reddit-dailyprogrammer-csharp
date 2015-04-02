using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RedditDailyProgrammer.Common;
using Xunit;

namespace RedditDailyProgrammer.Answers._208Medium
{
    public class GradientTests
    {
        [Theory]
        [InlineData("x", 10, 10)] // lower bound
        [InlineData("x", 15, 15)] // inside
        [InlineData("x", 10, 19)] // upper bounds
        [InlineData("x", 19, 10)]
        public void Can_return_color_for_inner_most_point_in_radial_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new RadialGradientOptions
                             {
                                 Center = new Point(10, 10),
                                 Radius = 20,
                                 Bands = new List<string> {"x", "X"}
                             });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("X", 20, 10)] // lower bounds
        [InlineData("X", 10, 20)] 
        [InlineData("X", 10, 39)] // upper bounds
        [InlineData("X", 39, 10)]
        [InlineData("X", 25, 25)] // inside
        public void Can_return_color_for_outermost_point_in_radial_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new RadialGradientOptions
                             {
                                 Center = new Point(10, 10),
                                 Radius = 20,
                                 Bands = new List<string> {"x", "X"}
                             });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("X", 45, 45)]
        public void Can_return_color_for_point_beyond_radius_in_radial_gradient(string expected, int x, int y )
        {
            var gradient =
                new Gradient(new RadialGradientOptions
                {
                    Center = new Point(10, 10),
                    Radius = 20,
                    Bands = new List<string> { "x", "X" }
                });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("x", 10, 15)] // lower bounds - 2nd band
        [InlineData("x", 10, 19)]
        [InlineData("x", 19, 10)] // upper bounds - 2nd band
        [InlineData("x", 15, 10)]
        [InlineData("X", 10, 24)] // upper bounds - 3rd band
        [InlineData("X", 24, 10)]
        [InlineData("X", 20, 10)] // lower bounds - 3rd band
        [InlineData("X", 10, 20)]
        public void Can_return_color_for_inbetween_point_in_radial_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new RadialGradientOptions
                             {
                                 Center =  new Point(10, 10),
                                 Radius = 20,
                                 Bands = new List<string> {" ", "x", "X", "@"}
                             });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }


        [Theory]
        [InlineData("x", 10, 10)] // lower
        [InlineData("x", 19, 19)] // upper
        [InlineData("x", 15, 15)] // inner
        public void Can_return_color_for_innermost_point_in_linear_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                             {
                                 Start = new Point(10, 10),
                                 End = new Point(30, 30),
                                 Bands = new List<string> {"x", "X"}
                             });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("X", 20, 20)] // lower
        [InlineData("X", 25, 25)] // inner
        [InlineData("X", 29, 29)] // upper
        [InlineData("X", 30, 30)]
        public void Can_return_color_for_outermost_point_in_linear_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                {
                    Start = new Point(10, 10),
                    End = new Point(30, 30),
                    Bands = new List<string> { "x", "X" }
                });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("X", 35, 35)]
        public void Can_return_color_for_point_beyond_linear_gradients_end(string expected, int x, int y )
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                {
                    Start = new Point(10, 10),
                    End = new Point(30, 30),
                    Bands = new List<string> { "x", "X" }
                });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData(" ", 0, 0)]
        [InlineData(" ", -1, -1)]
        [InlineData(" ", -100, -100)]
        public void For_point_before_linear_gradients_start_return_earliest_band(string expected, int x, int y )
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                {
                    Start = new Point(10, 10),
                    End = new Point(30, 30),
                    Bands = new List<string> { " ", "x", "X", "@" }
                });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("@", 40, 40)]
        [InlineData("@", 41, 41)]
        [InlineData("@", 100, 100)]
        public void For_point_beyond_linear_gradients_end_return_latest_band(string expected, int x, int y )
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                {
                    Start = new Point(10, 10),
                    End = new Point(30, 30),
                    Bands = new List<string> { " ", "x", "X", "@" }
                });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }

        [Theory]
        [InlineData("x", 15, 15)] // lower 2nd band
        [InlineData("x", 19, 19)] // upper 2nd band
        [InlineData("X", 20, 20)] // lower 3rd band
        [InlineData("X", 24, 24)] // upper 3rd band
        [InlineData("@", 25, 25)] // lower 4th band
        public void Can_return_color_for_inbetween_point_in_linear_gradient(string expected, int x, int y)
        {
            var gradient =
                new Gradient(new LinearGradientOptions
                             {
                                 Start = new Point(10, 10),
                                 End = new Point(30, 30),
                                 Bands = new List<string> {" ", "x", "X", "@"}
                             });

            Assert.Equal(expected, gradient.GetBand(x, y));
        }


        public static void Main()
        {
            var inputs = new List<string>
                         {
                             @"40 30
 .,:;xX&@
radial 20 15 20",
                             @"60 30
 '""^+$
linear 30 30 0 0",
                             @"40 40
aaabcccdeeefggg
radial -10 20 60"
                         };

            inputs.ForEach(ParseInput);
        }

        private static void ParseInput(string input)
        {
            using (var reader = new StringReader(input))
            {
// ReSharper disable once PossibleNullReferenceException
                var line1Parts = reader.ReadLine().Split(' ');
                var maxCols = Int32.Parse(line1Parts[0]);
                var maxRows = Int32.Parse(line1Parts[1]);
                var options = ParseGradientOptions(reader.ReadLine(), reader.ReadLine());

                var gradient = new Gradient(options);

                var builder = new StringBuilder();
                foreach (var y in Enumerable.Range(0, maxRows))
                {
                    foreach (var x in Enumerable.Range(0, maxCols))
                    {
                        builder.Append(gradient.GetBand(x, y));
                    }
                    builder.AppendLine();
                }

                Console.WriteLine(input);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(builder.ToString());
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static IGradientOptions ParseGradientOptions(string bandsLine, string gradientTypeLine)
        {
            var bands = bandsLine.Select(c => c.ToString()).ToList();

            var lineParts = gradientTypeLine.Split(' ');
            IGradientOptions options;
            switch (lineParts[0])
            {
                case "linear":
                    var startX = Int32.Parse(lineParts[1]);
                    var startY = Int32.Parse(lineParts[2]);
                    var endX = Int32.Parse(lineParts[3]);
                    var endY = Int32.Parse(lineParts[4]);
                    options = new LinearGradientOptions
                              {
                                  Bands = bands,
                                  Start = new Point(startX, startY),
                                  End = new Point(endX, endY)
                              };
                    break;
                case "radial":
                    var centerX = Int32.Parse(lineParts[1]);
                    var centerY = Int32.Parse(lineParts[2]);
                    var radius = Int32.Parse(lineParts[3]);
                    options = new RadialGradientOptions
                              {
                                  Bands = bands,
                                  Center = new Point(centerX, centerY),
                                  Radius = radius
                              };
                    break;
                default:
                    throw new ArgumentException("Unsupported gradient - " + lineParts[0]);
            }
            return options;
        }
    }
}
