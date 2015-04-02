using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RedditDailyProgrammer.Common;

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

        public LinearBandingStrategy(LinearGradientOptions options)
        {
            _options = options;
            if (_options.Bands.Count > _options.Start.Distance(_options.End))
            {
                throw new ArgumentException("Way too many bands!");
            }
            if (_options.Bands.Count < 2)
            {
                throw new ArgumentException("Need atleast two bands");
            }
        }

        public string GetBand(Point point)
        {
            var multiplier = MathHelpers.GetVectorProjectionMultiplier(point, _options.End, _options.Start);
            return _options.Bands[Math.Max(0, Math.Min((int) (multiplier*_options.Bands.Count),
                                                       _options.Bands.Count - 1))];
        }
    }

    class RadialBandingStrategy : IBandingStrategy
    {
        private readonly RadialGradientOptions _options;

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
        }

        public string GetBand(Point point)
        {
            var multiplier = _options.Center.Distance(point)/(double) _options.Radius;
            return _options.Bands[Math.Max(0, Math.Min((int) (multiplier*_options.Bands.Count), _options.Bands.Count - 1))];
        }
    }
}
