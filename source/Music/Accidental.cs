namespace Music
{
    public enum Accidental : sbyte
    {
        DoubleFlat = -2,
        Flat,
        Natural,
        Sharp,
        DoubleSharp,
    }

    public static class AccidentalExtensions
    {
        private static readonly string[] s_accidentalSigns = new string[Accidental.DoubleSharp - Accidental.DoubleFlat + 1] { "bb", "b", string.Empty, "#", "##" };

        public static string GetSign(this Accidental accidental)
        {
            return s_accidentalSigns[accidental - Accidental.DoubleFlat];
        }
    }
}
