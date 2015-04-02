using System;

namespace RedditDailyProgrammer.Common
{
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

        public static double GetVectorProjectionMultiplier(Point p1, Point p2, Point origin = default(Point))
        {
            if (origin == default(Point))
            {
                origin = new Point(0, 0);
            }

            var a = p1.TranslateTo(origin);
            var b = p2.TranslateTo(origin);

            return a.DotProduct(b)/(double)b.DotProduct(b);
        }
    }
}