using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Music;

using static Music.Interval;

namespace ScaleTrainer
{
    public class Guitar
    {
        public const int DefaultNumberOfFrets = 24;

        public static readonly Pitch DefaultTuningPitch = new Pitch(Note.E, 2);

        public static readonly IReadOnlyList<Interval> DefaultStringIntervals = new ReadOnlyCollection<Interval>(new Interval[] 
        {
            PerfectFourth,
            PerfectFourth,
            PerfectFourth,
            MajorThird,
            PerfectFourth
        });

        private readonly (ChromaticScale Scale, int Octave)[] _strings;

        public Guitar() : this(DefaultTuningPitch, DefaultStringIntervals, DefaultNumberOfFrets) { }

        public Guitar(Pitch tuningPitch, IEnumerable<Interval> stringIntervals, int numberOfFrets)
        {
            if (stringIntervals == null)
                throw new ArgumentNullException(nameof(stringIntervals));

            StringIntervals = stringIntervals.ToArray();
            if (StringIntervals.Count <= 0)
                throw new ArgumentException(null, nameof(stringIntervals));

            TuningPitch = tuningPitch;
            NumberOfFrets = numberOfFrets;

            _strings = new (ChromaticScale, int)[NumberOfStrings];
            _strings[0] = (new ChromaticScale(tuningPitch.Note), tuningPitch.Octave);
            for (var i = 0; i < StringIntervals.Count; i++)
            {
                tuningPitch += StringIntervals[i];
                _strings[i + 1] = (new ChromaticScale(tuningPitch.Note), tuningPitch.Octave);
            }
        }

        public Pitch GetPitch(int stringNumber, int fretNumber)
        {
            if (stringNumber < 0 || NumberOfStrings <= stringNumber)
                throw new ArgumentOutOfRangeException(nameof(stringNumber));

            if (fretNumber < 0 || NumberOfFrets < fretNumber)
                throw new ArgumentOutOfRangeException(nameof(fretNumber));

            var @string = _strings[stringNumber];
            return @string.Scale.GetPitchesAscending(@string.Octave).Skip(fretNumber).First();
        }

        public Pitch TuningPitch { get; }
        public IReadOnlyList<Interval> StringIntervals { get; }
        public int NumberOfStrings => StringIntervals.Count + 1;
        public int NumberOfFrets { get; }
    }
}
