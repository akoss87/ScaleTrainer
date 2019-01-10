using System;

namespace Music
{
    public readonly struct Note
    {
        public const int NumberOfNaturalNotes = NaturalNote.B - NaturalNote.C + 1;

        private static readonly int[] s_naturalNoteToNumber = new int[7] { 0, 2, 4, 5, 7, 9, 11 };

        private static readonly string[] s_chromaticNames = new string[Interval.SemitonesPerOctave] { "C", "C#/Db", "D", "D#/Eb", "E", "F", "F#/Gb", "G", "G#/Ab", "A", "A#/Bb", "B" };

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

        public static int GetSemitonesBetween(NaturalNote naturalNote1, NaturalNote naturalNote2)
        {
            var number1 = s_naturalNoteToNumber[naturalNote1 - NaturalNote.C];
            var number2 = s_naturalNoteToNumber[naturalNote2 - NaturalNote.C];
            if (number1 > number2)
                number1 -= Interval.SemitonesPerOctave;
            return number2 - number1;
        }

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

        public string ChromaticName => s_chromaticNames[Number];

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

        public Note Invert()
        {
            if (Accidental == Accidental.Natural)
                return this;

            NaturalNote newNaturalNote;
            Accidental invertedAccidental;
            NaturalNote naturalNote = _naturalNote;
            if (Accidental > Accidental.Natural)
            {
                int semitonesDifference = Accidental - Accidental.Natural;
                for (; ; )
                {
                    newNaturalNote = naturalNote + 1;
                    if (newNaturalNote - NaturalNote.C >= NumberOfNaturalNotes)
                        newNaturalNote -= NumberOfNaturalNotes;

                    semitonesDifference -= GetSemitonesBetween(naturalNote, newNaturalNote);

                    if (semitonesDifference < Accidental.DoubleFlat - Accidental.Natural)
                        throw new InvalidOperationException();
                    else if (semitonesDifference < Accidental.Natural - Accidental.Natural)
                    {
                        invertedAccidental = (Accidental)semitonesDifference;
                        break;
                    }

                    naturalNote = newNaturalNote;
                }
            }
            else
            {
                int semitonesDifference = Accidental - Accidental.Natural;
                for (; ; )
                {
                    newNaturalNote = naturalNote - 1;
                    if (newNaturalNote - NaturalNote.C < 0)
                        newNaturalNote += NumberOfNaturalNotes;

                    semitonesDifference += Interval.SemitonesPerOctave - GetSemitonesBetween(naturalNote, newNaturalNote);

                    if (semitonesDifference > Accidental.DoubleSharp - Accidental.Natural)
                        throw new InvalidOperationException();
                    else if (semitonesDifference > Accidental.Natural - Accidental.Natural)
                    {
                        invertedAccidental = (Accidental)semitonesDifference;
                        break;
                    }

                    naturalNote = newNaturalNote;
                }
            }

            return new Note(newNaturalNote, invertedAccidental);
        }

        public override string ToString()
        {
            return NaturalNote.ToString() + Accidental.GetSign();
        }
    }
}
