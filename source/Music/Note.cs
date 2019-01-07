using System;

namespace Music
{
    public readonly struct Note
    {
        public const int NumberOfNaturalNotes = NaturalNote.B - NaturalNote.C + 1;

        private static readonly int[] s_naturalNoteToNumber = new int[7] { 0, 2, 4, 5, 7, 9, 11 };

        private static readonly string[] s_standardNames = new string[Interval.SemitonesPerOctave] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public static readonly Note C = new Note(NaturalNote.C);
        public static readonly Note CSharp = new Note(NaturalNote.C, Accidental.Sharp);
        public static readonly Note DFlat = new Note(NaturalNote.D, Accidental.Flat);
        public static readonly Note D = new Note(NaturalNote.D);
        public static readonly Note DSharp = new Note(NaturalNote.D, Accidental.Sharp);
        public static readonly Note EFlat = new Note(NaturalNote.E, Accidental.Flat);
        public static readonly Note E = new Note(NaturalNote.E);
        public static readonly Note F = new Note(NaturalNote.F);
        public static readonly Note FSharp = new Note(NaturalNote.F, Accidental.Sharp);
        public static readonly Note GFlat = new Note(NaturalNote.G, Accidental.Flat);
        public static readonly Note G = new Note(NaturalNote.G);
        public static readonly Note GSharp = new Note(NaturalNote.G, Accidental.Sharp);
        public static readonly Note AFlat = new Note(NaturalNote.A, Accidental.Flat);
        public static readonly Note A = new Note(NaturalNote.A);
        public static readonly Note ASharp = new Note(NaturalNote.A, Accidental.Sharp);
        public static readonly Note BFlat = new Note(NaturalNote.B, Accidental.Flat);
        public static readonly Note B = new Note(NaturalNote.B);

        private readonly NaturalNote _naturalNote;
        private readonly Accidental _accidental;

        public Note(NaturalNote naturalNote, Accidental accidental = Accidental.Natural)
        {
            if (naturalNote < NaturalNote.C || naturalNote > NaturalNote.B)
                throw new ArgumentOutOfRangeException(nameof(naturalNote));

            if (accidental < Accidental.DoubleFlat || accidental > Accidental.DoubleSharp)
                throw new ArgumentOutOfRangeException(nameof(naturalNote));

            _naturalNote = naturalNote;
            _accidental = accidental;
        }

        public int AbsoluteNumber => s_naturalNoteToNumber[_naturalNote - NaturalNote.C] + (_accidental - Accidental.Natural);

        public NaturalNote NaturalNote => _naturalNote;
        public Accidental Accidental => _accidental;

        public int Number
        {
            get
            {
                var number = AbsoluteNumber;
                if (number >= Interval.SemitonesPerOctave)
                    number -= Interval.SemitonesPerOctave;
                else if (number < 0)
                    number += Interval.SemitonesPerOctave;
                return number;
            }
        }

        public string StandardName => s_standardNames[Number];

        public Note Flatten()
        {
            Accidental accidental = Accidental;
            if (accidental > Accidental.DoubleFlat)
                return new Note(NaturalNote, --accidental);
            else
                throw new InvalidOperationException();
        }

        public Note Sharpen()
        {
            Accidental accidental = Accidental;
            if (accidental < Accidental.DoubleSharp)
                return new Note(NaturalNote, ++accidental);
            else
                throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return NaturalNote.ToString() + Accidental.GetSign();
        }
    }
}
