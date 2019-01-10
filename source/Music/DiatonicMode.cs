using System;

namespace Music
{
    public enum DiatonicMode : sbyte
    {
        Ionian,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian,
        Locrian,

        Major = Ionian,
        Minor = Aeolian
    }

    public static class DiatonicModeExtensions
    {
        public static string GetCombinedName(this DiatonicMode mode)
        {
            var name = mode.GetName();
            var alternativeName = mode.GetAlternativeName();
            return name != alternativeName ? $"{name} ({alternativeName})" : name;
        }

        public static string GetName(this DiatonicMode mode)
        {
            switch (mode)
            {
                case DiatonicMode.Ionian: return "Ionian";
                case DiatonicMode.Dorian: return "Dorian";
                case DiatonicMode.Phrygian: return "Phrygian";
                case DiatonicMode.Lydian: return "Lydian";
                case DiatonicMode.Mixolydian: return "Mixolydian";
                case DiatonicMode.Aeolian: return "Aeolian";
                case DiatonicMode.Locrian: return "Locrian";
                default: throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public static string GetAlternativeName(this DiatonicMode mode)
        {
            switch (mode)
            {
                case DiatonicMode.Major: return "Major";
                case DiatonicMode.Minor: return "Minor";
                default: return mode.GetName();
            }
        }
    }
}