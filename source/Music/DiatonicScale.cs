using System;
using System.Collections.Generic;

using static Music.Interval;

namespace Music
{
    public class DiatonicScale : Scale
    {
        public const int NumberOfDegrees = NumbersPerOctave;

        private static readonly Interval[] s_majorScaleIntervals = new Interval[NumberOfDegrees] { MajorSecond, MajorSecond, MinorSecond, MajorSecond, MajorSecond, MajorSecond, MinorSecond };

        public DiatonicScale(Note keyNote, DiatonicMode diatonicMode)
        {
            if (diatonicMode < DiatonicMode.Ionian || diatonicMode > DiatonicMode.Locrian)
                throw new ArgumentOutOfRangeException(nameof(diatonicMode));

            KeyNote = keyNote;
            DiatonicMode = diatonicMode;
        }

        public override int DegreeCount => NumberOfDegrees;

        public override string Name => $"{KeyNote} {DiatonicMode.GetCombinedName()}";

        public override Note KeyNote { get; }

        public DiatonicMode DiatonicMode { get; }

        public override IEnumerable<Interval> GetIntervalsAscending()
        {
            int intervalIndex = DiatonicMode - DiatonicMode.Ionian;
            for (; ; )
            {
                yield return s_majorScaleIntervals[intervalIndex++];

                if (intervalIndex >= DegreeCount)
                    intervalIndex -= DegreeCount;
            }
        }

        public override IEnumerable<Interval> GetIntervalsDescending()
        {
            int intervalIndex = DiatonicMode - DiatonicMode.Ionian - 1;

            for (; ; )
            {
                if (intervalIndex < 0)
                    intervalIndex += DegreeCount;

                yield return -s_majorScaleIntervals[intervalIndex--];
            }
        }

        private IEnumerable<Pitch> GetPitches(Func<IEnumerable<Interval>> getIntervals, int octave)
        {
            var pitch = new Pitch(KeyNote, octave);
            yield return pitch;

            foreach (var interval in getIntervals())
            {
                pitch += interval;
                yield return pitch;
            }
        }

        public override IEnumerable<Pitch> GetPitchesAscending(int octave = 4)
        {
            return GetPitches(GetIntervalsAscending, octave);
        }

        public override IEnumerable<Pitch> GetPitchesDescending(int octave = 4)
        {
            return GetPitches(GetIntervalsDescending, octave);
        }
    }
}
