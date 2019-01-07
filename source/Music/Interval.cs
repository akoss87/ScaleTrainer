using System;
using System.Text;

namespace Music
{
    public readonly struct Interval
    {
        public const int NumbersPerOctave = IntervalNumber.Octave - IntervalNumber.Unison;
        public const int SemitonesPerOctave = 12;

        private static readonly int[] s_numberToSemitones = new int[NumbersPerOctave] { 0, 2, 4, 5, 7, 9, 11 };

        public static readonly Interval PerfectUnison = new Interval(IntervalNumber.Unison, IntervalQuality.Perfect);
        public static readonly Interval MinorSecond = new Interval(IntervalNumber.Second, IntervalQuality.Minor);
        public static readonly Interval MajorSecond = new Interval(IntervalNumber.Second, IntervalQuality.Major);
        public static readonly Interval MinorThird = new Interval(IntervalNumber.Third, IntervalQuality.Minor);
        public static readonly Interval MajorThird = new Interval(IntervalNumber.Third, IntervalQuality.Major);
        public static readonly Interval PerfectFourth = new Interval(IntervalNumber.Fourth, IntervalQuality.Perfect);
        public static readonly Interval AugmentedFourth = new Interval(IntervalNumber.Fourth, IntervalQuality.Augmented, 1);
        public static readonly Interval DiminishedFifth = new Interval(IntervalNumber.Fifth, IntervalQuality.Diminished, 1);
        public static readonly Interval PerfectFifth = new Interval(IntervalNumber.Fifth, IntervalQuality.Perfect);
        public static readonly Interval MinorSixth = new Interval(IntervalNumber.Sixth, IntervalQuality.Minor);
        public static readonly Interval MajorSixth = new Interval(IntervalNumber.Sixth, IntervalQuality.Major);
        public static readonly Interval MinorSeventh = new Interval(IntervalNumber.Seventh, IntervalQuality.Minor);
        public static readonly Interval MajorSeventh = new Interval(IntervalNumber.Seventh, IntervalQuality.Major);
        public static readonly Interval PerfectOctave = new Interval(IntervalNumber.Octave, IntervalQuality.Perfect);

        private static IntervalNumber GetNumber(int absoluteNumber)
        {
            return (IntervalNumber)Math.Abs(absoluteNumber % NumbersPerOctave);
        }

        private static int GetSemitones(IntervalNumber number, IntervalQuality quality, sbyte multiplicity)
        {
            int semitones = s_numberToSemitones[number - IntervalNumber.Unison];

            if (quality == IntervalQuality.Minor || quality == IntervalQuality.Diminished && !number.IsPerfect())
                semitones--;

            if (multiplicity > 0)
                if (quality == IntervalQuality.Augmented)
                    semitones += multiplicity;
                else
                    semitones -= multiplicity;

            return semitones;
        }

        public static Interval operator +(Interval interval)
        {
            return interval;
        }

        public static Interval operator +(Interval interval1, Interval interval2)
        {
            int absoluteNumber = interval1._absoluteNumber + interval2._absoluteNumber;

            int semitones = (interval1.Semitones + interval2.Semitones) % SemitonesPerOctave;

            IntervalNumber number = (IntervalNumber)Math.Abs(absoluteNumber % NumbersPerOctave);

            IntervalQuality quality;
            if (number.IsPerfect())
            {
                quality = IntervalQuality.Perfect;
                semitones -= GetSemitones(number, quality, 0);
            }
            else
            {
                quality = IntervalQuality.Major;
                semitones -= GetSemitones(number, quality, 0);
                if (semitones < 0)
                {
                    quality = IntervalQuality.Minor;
                    semitones++;
                }
            }

            if (semitones > 0)
                quality = IntervalQuality.Augmented;
            else if (semitones < 0)
                quality = IntervalQuality.Diminished;

            return new Interval(absoluteNumber, quality, (sbyte)Math.Abs(semitones));
        }

        public static Interval operator -(Interval interval)
        {
            return new Interval(-interval._absoluteNumber, interval._quality, interval._multiplicity);
        }

        public static Interval operator -(Interval interval1, Interval interval2)
        {
            return interval1 + -interval2;
        }

        public static Interval operator *(Interval interval, int factor)
        {
            Interval product = PerfectUnison;

            for (int i = Math.Abs(factor); i > 0; i--)
                product += interval;

            return factor >= 0 ? product : -product;
        }

        public static Interval operator *(int factor, Interval interval)
        {
            return interval * factor;
        }

        readonly int _absoluteNumber;
        readonly IntervalQuality _quality;
        readonly sbyte _multiplicity;

        public Interval(int absoluteNumber, IntervalQuality quality, sbyte multiplicity = 0)
        {
            var number = GetNumber(absoluteNumber);

            if (number.IsPerfect())
            {
                if (quality == IntervalQuality.Minor || quality == IntervalQuality.Major)
                    throw new ArgumentException(null, nameof(quality));
            }
            else
            {
                if (quality == IntervalQuality.Perfect)
                    throw new ArgumentException(null, nameof(quality));
            }

            if (quality < IntervalQuality.Diminished || quality > IntervalQuality.Augmented)
                throw new ArgumentOutOfRangeException(nameof(quality));

            if (quality == IntervalQuality.Diminished || quality == IntervalQuality.Augmented)
            {
                if (multiplicity <= 0)
                    throw new ArgumentOutOfRangeException(nameof(multiplicity));
            }
            else
            {
                if (multiplicity != 0)
                    throw new ArgumentOutOfRangeException(nameof(multiplicity));
            }

            _absoluteNumber = absoluteNumber;
            _quality = quality;
            _multiplicity = multiplicity;
        }

        public Interval(IntervalNumber number, IntervalQuality quality, sbyte multiplicity = 0)
            : this(IntervalNumber.Unison <= number && number <= IntervalNumber.Octave ? number - IntervalNumber.Unison : throw new ArgumentOutOfRangeException(nameof(number)),
                quality, multiplicity) { }

        public int AbsoluteNumber => _absoluteNumber;

        public bool IsUpwardStep => _absoluteNumber >= 0;

        public int Octaves => _absoluteNumber / NumbersPerOctave;

        public IntervalNumber Number => GetNumber(_absoluteNumber);

        public IntervalQuality Quality => _quality;

        public int Multiplicity => _multiplicity;

        public int Semitones
        {
            get
            {
                int octaves = Math.DivRem(_absoluteNumber, NumbersPerOctave, out int remainder);

                int semitones =
                    remainder >= 0 ?
                    GetSemitones((IntervalNumber)remainder, _quality, _multiplicity) :
                    -GetSemitones((IntervalNumber)(-remainder), _quality, _multiplicity);

                return octaves * SemitonesPerOctave + semitones;
            }
        }

        public Interval Augment()
        {
            IntervalQuality quality = _quality;
            sbyte multiplicity = _multiplicity;

            if (quality == IntervalQuality.Diminished)
            {
                multiplicity--;
                if (multiplicity == 0)
                    quality = Number.IsPerfect() ? IntervalQuality.Perfect : IntervalQuality.Minor;
            }
            else if (quality == IntervalQuality.Perfect || quality == IntervalQuality.Major)
            {
                multiplicity++;
                quality = IntervalQuality.Augmented;
            }
            else if (quality == IntervalQuality.Minor)
                quality = IntervalQuality.Major;
            else
                multiplicity++;

            return new Interval(_absoluteNumber, quality, multiplicity);
        }

        public Interval Diminish()
        {
            IntervalQuality quality = _quality;
            sbyte multiplicity = _multiplicity;

            if (quality == IntervalQuality.Augmented)
            {
                multiplicity--;
                if (multiplicity == 0)
                    quality = Number.IsPerfect() ? IntervalQuality.Perfect : IntervalQuality.Major;
            }
            else if (quality == IntervalQuality.Perfect || quality == IntervalQuality.Minor)
            {
                multiplicity++;
                quality = IntervalQuality.Diminished;
            }
            else if (quality == IntervalQuality.Major)
                quality = IntervalQuality.Minor;
            else
                multiplicity++;

            return new Interval(_absoluteNumber, quality, multiplicity);
        }

        public Interval Invert()
        {
            return (_absoluteNumber >= 0 ? PerfectOctave : -PerfectOctave) - this;
        }

        public override string ToString()
        {
            int octaves = Math.Abs(Math.DivRem(_absoluteNumber, NumbersPerOctave, out int remainder));
            remainder = Math.Abs(remainder);

            var sb = new StringBuilder();

            if (octaves > 0)
            {
                if (remainder == 0 && _quality != IntervalQuality.Perfect)
                    octaves--;

                if (octaves > 1)
                {
                    sb.Append(octaves).Append('x');

                    if (remainder != 0 || _quality != IntervalQuality.Perfect)
                        sb.Append(IntervalQuality.Perfect.GetSign(0)).Append(IntervalNumber.Octave - IntervalNumber.Unison + 1).Append('+');
                }

                if (remainder == 0)
                    sb.Append(_quality.GetSign(_multiplicity)).Append(IntervalNumber.Octave - IntervalNumber.Unison + 1);
            }

            if (remainder > 0 || octaves == 0)
                sb.Append(_quality.GetSign(_multiplicity)).Append((IntervalNumber)remainder - IntervalNumber.Unison + 1);

            if (_absoluteNumber > 0)
                sb.Append(" [U]");
            else if (_absoluteNumber < 0)
                sb.Append(" [D]");

            return sb.ToString();
        }
    }
}
