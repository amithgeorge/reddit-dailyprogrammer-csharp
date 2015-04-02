using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDailyProgrammer.Answers._208Medium
{
    public class Gradient
    {
        private readonly IBandingStrategy _bandingStrategy;

        public Gradient(IGradientOptions options)
        {
            _bandingStrategy = GetBandingStrategy(options);
        }

        private IBandingStrategy GetBandingStrategy(IGradientOptions options)
        {
            switch (options.Type)
            {
                case GradientType.Radial:
                    return new RadialBandingStrategy((RadialGradientOptions)options);
                case GradientType.Linear:
                    return new LinearBandingStrategy((LinearGradientOptions)options);
                default:
                    throw new ArgumentException("Unknown gradient type - " + options.Type);
            }
        }

        public string GetBand(int x, int y)
        {
            return _bandingStrategy.GetBand(new Point(x, y));
        }
    }

    public enum GradientType
    {
        Unknown = 0,
        Radial = 1,
        Linear = 2
    }

    public interface IGradientOptions
    {
        GradientType Type { get; }
        List<string> Bands { get; set; }
    }

    public class LinearGradientOptions : IGradientOptions
    {
        public GradientType Type { get; private set; }
        public List<string> Bands { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }

        public LinearGradientOptions()
        {
            Type = GradientType.Linear;
            Bands = new List<string>();
        }
    }

    public class RadialGradientOptions : IGradientOptions
    {
        public GradientType Type { get; private set; }
        public Point Center { get; set; }
        public int Radius { get; set; }
        public List<string> Bands { get; set; }

        public RadialGradientOptions()
        {
            Type = GradientType.Radial;
            Bands = new List<string>();
        }
    }

    
    interface IBandingStrategy
    {
        string GetBand(Point point);
    }

    class LinearBandingStrategy : IBandingStrategy
    {
        private readonly LinearGradientOptions _options;
        private readonly List<BandLowerBound> _bandLowerBounds;
        private readonly Func<Point, Point> _intersectionFunc;
        private readonly int _distance;

        public LinearBandingStrategy(LinearGradientOptions options)
        {
            _options = options;
            _distance = (int)Math.Sqrt(_options.Start.DistanceSquared(_options.End));
            if (_options.Bands.Count > _distance)
            {
                throw new ArgumentException("Way too many bands!");
            }
            if (_options.Bands.Count < 2)
            {
                throw new ArgumentException("Need atleast two bands");
            }

            _intersectionFunc = GetIntersectionFunc(_options.Start, _options.End);
            _bandLowerBounds = ComputeBandLowerBounds();
        }

        private List<BandLowerBound> ComputeBandLowerBounds()
        {
            var limit = _distance / _options.Bands.Count;
            return Enumerable.Range(0, _distance)
                             .Select((value, index) => new { Value = value, Index = index })
                             .Where(i => i.Index % limit == 0)
                             .Select(i => i.Value)
                             .Zip(_options.Bands,
                                  (i, band) => new BandLowerBound
                                  {
                                      // store lower bound squared to help with easing distance calculations
                                      LowerBound = i * i,
                                      Band = band
                                  })
                             .OrderByDescending(i => i.LowerBound)
                             .ToList();
        }

        public string GetBand(Point point)
        {
            var intersection = _intersectionFunc(point);
            
            string gradientBand;
            if (PointIsOutsideGradientArea(intersection, out gradientBand))
            {
                return gradientBand;
            }

            var distanceSquared = _options.Start.DistanceSquared(intersection);
            return _bandLowerBounds.First(i => distanceSquared >= i.LowerBound).Band;
        }

        private bool PointIsOutsideGradientArea(Point intersection, out string gradientBand)
        {
            gradientBand = string.Empty;
            var distanceToStart = intersection.Distance(_options.Start);
            var distanceToEnd = intersection.Distance(_options.End);

            if (distanceToStart + distanceToEnd > _distance)
            {
                gradientBand = distanceToStart < distanceToEnd
                                   ? _bandLowerBounds.Last().Band
                                   : _bandLowerBounds.First().Band;

                return true;
            }

            return false;
        }


        private static Func<Point, Point> GetIntersectionFunc(Point start, Point end)
        {
            //  http://stackoverflow.com/a/12499474/30007

            var px = end.X - start.X;
            var py = end.Y - start.Y;
            // ReSharper disable once InconsistentNaming
            var dAB = px * px + py * py;

            return point =>
            {
                var u = ((point.X - start.X) * px + (point.Y - start.Y) * py) / (double)dAB;
                var xi = (int)(start.X + u * px);
                var yi = (int)(start.Y + u * py);
                return new Point(xi, yi);
            };
        }
    }

    class RadialBandingStrategy : IBandingStrategy
    {
        private readonly RadialGradientOptions _options;
        private readonly List<BandLowerBound> _bandLowerBounds;

        public RadialBandingStrategy(RadialGradientOptions options)
        {
            _options = options;
            if (_options.Bands.Count > _options.Radius)
            {
                throw new ArgumentException("Num of bands can't exceed radius");
            }
            if (_options.Bands.Count < 2)
            {
                throw new ArgumentException("Need atleast two bands");
            }

            _bandLowerBounds = ComputeBandLowerBounds();
        }

        public string GetBand(Point point)
        {
            var distanceSquared = _options.Center.DistanceSquared(point);
            return _bandLowerBounds.First(i => distanceSquared >= i.LowerBound).Band;
        }

        private List<BandLowerBound> ComputeBandLowerBounds()
        {
            var limit = _options.Radius / _options.Bands.Count;
            return Enumerable.Range(0, _options.Radius)
                             .Select((value, index) => new { Value = value, Index = index })
                             .Where(i => i.Index % limit == 0)
                             .Select(i => i.Value)
                             .Zip(_options.Bands,
                                  (i, band) => new BandLowerBound
                                  {
                                      // store lower bound squared to help with easing distance calculations
                                      LowerBound = i * i,
                                      Band = band
                                  })
                             .OrderByDescending(i => i.LowerBound)
                             .ToList();
        }

    }


    public class Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        private sealed class CoordsEqualityComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point x, Point y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null))
                {
                    return false;
                }
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return x.Y == y.Y && x.X == y.X;
            }

            public int GetHashCode(Point obj)
            {
                unchecked
                {
                    return (obj.Y * 397) ^ obj.X;
                }
            }
        }

        private static readonly IEqualityComparer<Point> ComparerInstance = new CoordsEqualityComparer();

        public static IEqualityComparer<Point> Comparer
        {
            get { return ComparerInstance; }
        }
    }

    public class BandLowerBound
    {
        public int LowerBound { get; set; }
        public string Band { get; set; }
    }
    
    public static class MathHelpers
    {
        public static int DistanceSquared(int x1, int y1, int x2, int y2)
        {
            return (x2 - x1) * (x2 - x1) +
                   (y2 - y1) * (y2 - y1);
        }

        public static int DistanceSquared(this Point p1, Point p2)
        {
            return DistanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static int Distance(this Point p1, Point p2)
        {
            return (int) Math.Sqrt(p1.DistanceSquared(p2));
        }
    }
}
