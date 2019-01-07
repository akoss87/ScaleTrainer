using System;

namespace Music
{
    public readonly struct Pitch
    {
        const int ReferenceFrequency = 440;

        public static readonly Pitch Reference = new Pitch();

        const int OctaveOffset = 4;

        public static Pitch operator +(Pitch pitch, Interval interval)
        {
            var octave = pitch.Octave + Math.DivRem(interval.AbsoluteNumber, Interval.NumbersPerOctave, out int remainder);
            var naturalNote = (NaturalNote)((pitch._note.NaturalNote - NaturalNote.C) + remainder);

            if (naturalNote > NaturalNote.B)
            {
                naturalNote -= Note.NumberOfNaturalNotes;
                octave++;
            }
            else if (naturalNote < NaturalNote.C)
            {
                naturalNote += Note.NumberOfNaturalNotes;
                octave--;
            }

            var targetPitch = new Pitch(new Note(naturalNote), octave);
            var targetInterval = targetPitch - pitch;
            interval = interval - targetInterval;

            var semitonesDifference = interval.Semitones;
            if (semitonesDifference < (int)Accidental.DoubleFlat || semitonesDifference > (int)Accidental.DoubleSharp)
                throw new ArgumentOutOfRangeException(nameof(interval));

            return new Pitch(new Note(naturalNote, (Accidental)semitonesDifference), octave);
        }

        public static Pitch operator -(Pitch pitch, Interval interval)
        {
            return pitch + -interval;
        }

        public static Interval operator -(Pitch pitch1, Pitch pitch2)
        {
            int absoluteNumber = Note.NumberOfNaturalNotes * (pitch1.Octave - pitch2.Octave) +
                (pitch1.Note.NaturalNote - pitch2.Note.NaturalNote);

            var number = (IntervalNumber)Math.Abs(absoluteNumber % Interval.NumbersPerOctave);
            var quality = number.IsPerfect() ? IntervalQuality.Perfect : IntervalQuality.Major;

            var interval = new Interval(absoluteNumber, quality, 0);

            var semitones =
                interval.IsUpwardStep ?
                pitch1.SemitonesFromReference - pitch2.SemitonesFromReference - interval.Semitones :
                -pitch1.SemitonesFromReference + pitch2.SemitonesFromReference + interval.Semitones;

            if (semitones > 0)
                while (--semitones >= 0)
                    interval = interval.Augment();
            else if (semitones < 0)
                while (++semitones <= 0)
                    interval = interval.Diminish();

            return interval;
        }

        readonly Note _note;
        readonly int _semitonesFromReference;

        public Pitch(Note note, int octave)
        {
            _note = note;
            _semitonesFromReference = (octave - OctaveOffset) * Interval.SemitonesPerOctave + _note.AbsoluteNumber - 9;
        }

        public Note Note => _note;

        public int Octave => (_semitonesFromReference - _note.AbsoluteNumber + 9) / Interval.SemitonesPerOctave + OctaveOffset;

        public double Frequency => ReferenceFrequency * Math.Pow(2, _semitonesFromReference / 12.0);

        public int SemitonesFromReference => _semitonesFromReference;

        public override string ToString()
        {
            return Note.ToString() + Octave;
        }
    }
}
