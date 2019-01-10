using System;
using System.Collections.Generic;

using static Music.Interval;

namespace Music
{
    public class ChromaticScale : Scale
    {
        public const int NumberOfDegrees = SemitonesPerOctave;

        private static readonly Note[] s_notesAscending = new Note[SemitonesPerOctave] 
        {
            Note.C, Note.CSharp, Note.D, Note.DSharp, Note.E, Note.F, Note.FSharp,Note.G, Note.GSharp, Note.A, Note.ASharp, Note.B,
        };

        private static readonly Note[] s_notesDescending = new Note[SemitonesPerOctave]
        {
            Note.C, Note.DFlat, Note.D, Note.EFlat, Note.E, Note.F, Note.GFlat,Note.G, Note.AFlat, Note.A, Note.BFlat, Note.B,
        };

        private readonly int _keyNoteIndex;

        public ChromaticScale(Note keyNote)
        {
            _keyNoteIndex = keyNote.Number;
        }

        public override int DegreeCount => NumberOfDegrees;

        public override string Name => $"{KeyNote.ChromaticName} Chromatic";

        public override Note KeyNote => s_notesAscending[_keyNoteIndex];

        private IEnumerable<Interval> GetIntervals(Func<int, IEnumerable<Pitch>> getPitches)
        {
            using (IEnumerator<Pitch> pitchEnumerator = getPitches(0).GetEnumerator())
            {
                pitchEnumerator.MoveNext();
                var pitch = pitchEnumerator.Current;

                for (; ; )
                {
                    pitchEnumerator.MoveNext();
                    var nextPitch = pitchEnumerator.Current;

                    yield return nextPitch - pitch;

                    pitch = nextPitch;
                }
            }
        }

        public override IEnumerable<Interval> GetIntervalsAscending()
        {
            return GetIntervals(GetPitchesAscending);
        }

        public override IEnumerable<Interval> GetIntervalsDescending()
        {
            return GetIntervals(GetPitchesDescending);
        }

        public override IEnumerable<Pitch> GetPitchesAscending(int octave = 4)
        {
            var index = _keyNoteIndex;
            for (; ; )
            {
                yield return new Pitch(s_notesAscending[index], octave);

                index++;
                if (index >= SemitonesPerOctave)
                {
                    index = 0;
                    octave++;
                }
            }
        }

        public override IEnumerable<Pitch> GetPitchesDescending(int octave = 4)
        {
            var index = _keyNoteIndex;
            for (; ; )
            {
                yield return new Pitch(s_notesDescending[index], octave);

                index--;
                if (index < 0)
                {
                    index = SemitonesPerOctave - 1;
                    octave--;
                }
            }
        }
    }
}
