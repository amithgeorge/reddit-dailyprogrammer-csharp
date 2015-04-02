using System.Collections.Generic;

namespace RedditDailyProgrammer.Common
{
    public class Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point TranslateTo(Point origin)
        {
            return new Point(X - origin.X, Y - origin.Y);
        }

        public int DotProduct(Point other)
        {
            return X*other.X + Y*other.Y;
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
}