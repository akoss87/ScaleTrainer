using System;

namespace Music
{
    public enum IntervalQuality : sbyte
    {
        Diminished = -2,
        Minor,
        Perfect,
        Major,
        Augmented,
    }

    public static class IntervalQualityExtensions
    {
        private static readonly char[] s_qualitySigns = new char[IntervalQuality.Augmented - IntervalQuality.Diminished + 1] { 'd', 'm', 'P', 'M', 'A' };

        public static string GetSign(this IntervalQuality quality, int multiciplity = 0)
        {
            return new string(s_qualitySigns[quality - IntervalQuality.Diminished], Math.Max(multiciplity, 1));
        }
    }
}
