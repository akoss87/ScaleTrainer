using System;
using System.Collections.Generic;
using System.Text;

using static Music.Interval;

namespace Music
{
    public static class DiatonicScale
    {
        public const int NumberOfDegrees = NumbersPerOctave;

        private static readonly Interval[] s_majorScaleIntervals = new Interval[NumberOfDegrees] { MajorSecond, MajorSecond, MinorSecond, MajorSecond, MajorSecond, MajorSecond, MinorSecond };

        public static Note[] CreateScaleArray()
        {
            return new Note[NumberOfDegrees + 1];
        }

        public static void GetNotes(Note[] notes, Note keyNote, DiatonicMode diatonicMode)
        {
            if (notes == null)
                throw new ArgumentNullException(nameof(notes));

            if (notes.Length != NumberOfDegrees + 1)
                throw new ArgumentException(null, nameof(notes));

            if (diatonicMode < DiatonicMode.Ionian || diatonicMode > DiatonicMode.Locrian)
                throw new ArgumentOutOfRangeException(nameof(diatonicMode));

            var pitch = new Pitch(keyNote, 4);
            int intervalIndex = diatonicMode - DiatonicMode.Ionian;
            for (int degree = 0; degree <= NumberOfDegrees; degree++)
            {
                notes[degree] = pitch.Note;

                pitch += s_majorScaleIntervals[intervalIndex];

                intervalIndex++;
                if (intervalIndex >= NumberOfDegrees)
                    intervalIndex -= NumberOfDegrees;
            }
        }
    }
}
