using System.ComponentModel;
using System.Reflection;

namespace Music
{
    public enum DiatonicMode : sbyte
    {
        [Description("Ionian (Major)")]
        Ionian,
        [Description("Dorian")]
        Dorian,
        [Description("Phrygian")]
        Phrygian,
        [Description("Lydian")]
        Lydian,
        [Description("Mixolydian")]
        Mixolydian,
        [Description("Aeolian (Minor)")]
        Aeolian,
        [Description("Locrian")]
        Locrian,
        
        Major = Ionian,
        Minor = Aeolian
    }

    public static class DiatonicModeExtensions
    {
        public static string GetName(this DiatonicMode mode)
        {
            FieldInfo field = typeof(DiatonicMode).GetField(mode.ToString());
            return field.GetCustomAttribute<DescriptionAttribute>().Description;
        }
    }
}
