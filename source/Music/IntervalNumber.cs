namespace Music
{
    public enum IntervalNumber : sbyte
    {
        Unison,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh,
        Octave,
    }

    public static class IntervalNumberExtensions
    {
        public static bool IsPerfect(this IntervalNumber number)
        {
            return number == IntervalNumber.Unison || number == IntervalNumber.Fourth || number == IntervalNumber.Fifth || number == IntervalNumber.Octave;
        }
    }
}
