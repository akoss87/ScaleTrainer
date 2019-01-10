using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Music
{
    public abstract class Scale
    {
        public abstract int DegreeCount { get; }

        public abstract IEnumerable<Interval> GetIntervalsAscending();
        public abstract IEnumerable<Interval> GetIntervalsDescending();

        public abstract IEnumerable<Pitch> GetPitchesAscending(int octave = 4);
        public abstract IEnumerable<Pitch> GetPitchesDescending(int octave = 4);

        public abstract Note KeyNote { get; }

        public abstract string Name { get; }

        public override string ToString()
        {
            return $"{Name} ({string.Join(", ", GetPitchesAscending().Select(pitch => pitch.Note).Take(DegreeCount + 1))})";
        }
    }
}
