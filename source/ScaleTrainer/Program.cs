using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Music;

using static Music.DiatonicMode;
using static Music.Interval;

namespace ScaleTrainer
{
    class Program
    {
        private enum Pattern { SingleString, TwoStringsA, TwoStringsB, ThreeStrings };

        private const int numberOfPatterns = Pattern.ThreeStrings - Pattern.SingleString + 1;

        private class PracticeStats
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public int NumberOfQuestions => NumberOfCorrectAnswers + NumberOfWrongAnswers;
            public int NumberOfCorrectAnswers { get; set; }
            public int NumberOfWrongAnswers { get; set; }

            public override string ToString()
            {
                return $"You answered {NumberOfQuestions} question(s) in {Stopwatch.Elapsed:hh':'mm':'ss}. Accuracy: {(NumberOfQuestions != 0 ? (NumberOfCorrectAnswers / (double)NumberOfQuestions).ToString("P1") : "n/a")}";
            }
        }

        private static volatile PracticeStats s_practiceStats;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var command = args[0].ToLowerInvariant();

                if (command == "help" || command == "--help" || command == "-h" || command == "/h" || command == "-?" || command == "/?")
                {
                    PrintUsage();
                    return;
                }

                // scaletrainer print-circle ionian
                if (command == "print-circle")
                {
                    DiatonicMode diatonicMode;
                    if (args.Length > 1)
                    {
                        if (!Enum.TryParse(args[1], ignoreCase: true, result: out diatonicMode))
                        {
                            PrintUsage("Unknown mode.");
                            return;
                        }
                    }
                    else
                        diatonicMode = Major;

                    PrintDiatonicScales(diatonicMode);
                }
                else
                    PrintUsage("Unknown command.");
            }
            else
                Practice();
        }

        private static void PrintDiatonicScales(DiatonicMode diatonicMode)
        {
            var scale = new DiatonicScale(Note.GFlat, Major);
            var notes = scale.GetPitchesAscending().Select(pitch => pitch.Note).Take(scale.DegreeCount + 1).ToArray();

            var keyNote = notes[diatonicMode - Ionian];

            var title = $"Circle of {diatonicMode.GetCombinedName()} scales";
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length));
            Console.WriteLine();

            var sb = new StringBuilder();
            for (var i = -SemitonesPerOctave / 2; i <= SemitonesPerOctave / 2; i++)
            {
                scale = new DiatonicScale(keyNote, diatonicMode);
                notes = scale.GetPitchesAscending().Select(pitch => pitch.Note).Take(scale.DegreeCount + 1).ToArray();

                sb.Clear();
                if (i < 0)
                    sb.AppendFormat("[{0}b] ", -i);
                else if (i > 0)
                    sb.AppendFormat("[{0}#] ", i);
                else
                    sb.Append(' ', 5);

                string note = notes[0].ToString();
                sb.Append(note);
                for (var j = 1; j < notes.Length; j++)
                {
                    sb.Append(',').Append(' ', 3 - note.Length);
                    note = notes[j].ToString();
                    sb.Append(note);
                }

                Console.WriteLine(sb.ToString());

                keyNote = (new Pitch(keyNote, 4) + PerfectFifth).Note;
            }
        }

        private static void Practice()
        {
            s_practiceStats = new PracticeStats();
            Console.CancelKeyPress += (s, e) => PrintPracticeStats();

            var guitar = new Guitar();
            var random = new Random();

            for (; ; )
            {
                var mode = (DiatonicMode)random.Next(DiatonicScale.NumberOfDegrees);
                var scale = new DiatonicScale(Note.C, mode);

                var pattern = (Pattern)random.Next(numberOfPatterns);
                int[][] fretNumbers = CreateFretNumbers(pattern);

                int stringNumber = random.Next(guitar.NumberOfStrings - fretNumbers.Length + 1);
                int maxFretNumber = FillFretNumbers(fretNumbers, guitar, stringNumber, scale);

                int questionStringIndex;
                int questionFretIndex;
                if (random.Next(10) != 0)
                {
                    questionStringIndex = random.Next(fretNumbers.Length);
                    questionFretIndex = random.Next(fretNumbers[questionStringIndex].Length);
                }
                else
                    questionStringIndex = questionFretIndex = -1;

                int position = random.Next(guitar.NumberOfFrets - maxFretNumber + 1);

                PrintExcercise(guitar, stringNumber, fretNumbers, maxFretNumber, questionStringIndex, questionFretIndex, position);
                Console.WriteLine();

                Console.Write(questionStringIndex >= 0 ? "Enter scale degree: " : "Enter scale name: ");
                Console.WriteLine($"({s_practiceStats.Stopwatch.Elapsed:hh':'mm':'ss})");
                for (; ; )
                {
                    var answer = Console.ReadLine()?.ToLowerInvariant();

                    if (answer == null || answer == "q" || answer == "quit")
                    {
                        PrintPracticeStats();
                        return;
                    }

                    bool isCorrectAnswer;
                    if (questionStringIndex >= 0)
                    {
                        DiatonicMode degree = mode;
                        for (var i = 0; i < fretNumbers.Length; i++)
                            for (var j = 0; j < fretNumbers[i].Length; j++)
                                if (i < questionStringIndex || j < questionFretIndex)
                                {
                                    degree++;
                                    if (degree - Ionian >= DiatonicScale.NumberOfDegrees)
                                        degree -= DiatonicScale.NumberOfDegrees;
                                }
                                else
                                    goto EndOfLoop;
                        EndOfLoop:

                        isCorrectAnswer = EvaluateDegreeAnswer(degree, answer);
                    }
                    else
                        isCorrectAnswer = EvaluateModeAnswer(mode, guitar, stringNumber, position, fretNumbers, answer);

                    if (isCorrectAnswer)
                    {
                        s_practiceStats.NumberOfCorrectAnswers++;
                        Console.WriteLine("Correct!");
                        break;
                    }
                    else
                    {
                        s_practiceStats.NumberOfWrongAnswers++;
                        Console.WriteLine("Wrong!");
                    }
                }

                Console.WriteLine();
            }
        }

        private static void PrintExcercise(Guitar guitar, int stringNumber, int[][] fretNumbers, int maxFretNumber, int questionStringIndex, int questionFretIndex, int position)
        {
            var questionStringNumber = stringNumber + questionStringIndex;
            var sb = new StringBuilder();
            var stringFrets = new bool[maxFretNumber + 1];
            for (var i = guitar.NumberOfStrings - 1; i >= 0; i--)
            {
                sb.Clear();
                Array.Clear(stringFrets, 0, stringFrets.Length);

                int fretNumberIndex = i - stringNumber;
                int[] stringFretNumbers;
                if (0 <= fretNumberIndex && fretNumberIndex < fretNumbers.Length)
                {
                    stringFretNumbers = fretNumbers[fretNumberIndex];
                    for (var j = 0; j < stringFretNumbers.Length; j++)
                        stringFrets[stringFretNumbers[j]] = true;
                }
                else
                    stringFretNumbers = null;

                sb.Append(guitar.GetPitch(i, 0).Note.ToString().PadRight(3)).Append('-').Append('|');
                for (var j = 0; j < stringFrets.Length; j++)
                {
                    sb.Append('-');
                    if (!stringFrets[j])
                        sb.Append('-');
                    else if (i == questionStringNumber && j == stringFretNumbers[questionFretIndex])
                        sb.Append('?');
                    else
                        sb.Append('O');
                    sb.Append('-').Append('|');
                }

                sb.Append('-');
                Console.WriteLine(sb.ToString());
            }

            Console.WriteLine(new string(' ', 5) + position.ToString().PadLeft(2));
        }

        private static int[][] CreateFretNumbers(Pattern pattern)
        {
            switch (pattern)
            {
                case Pattern.SingleString:
                    return new int[][] { new int[7] };
                case Pattern.TwoStringsA:
                    return new int[][] { new int[3], new int[4] };
                case Pattern.TwoStringsB:
                    return new int[][] { new int[4], new int[3] };
                case Pattern.ThreeStrings:
                    return new int[][] { new int[3], new int[3], new int[3] };
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int FillFretNumbers(int[][] fretNumbers, Guitar guitar, int stringNumber, DiatonicScale scale)
        {
            int fretNumber = 0;
            int minFretNumber = fretNumber;
            int maxFretNumber = fretNumber;
            using (IEnumerator<Interval> intervalEnumerator = scale.GetIntervalsAscending().GetEnumerator())
            {
                for (var i = 0; i < fretNumbers.Length; i++)
                {
                    for (var j = 0; j < fretNumbers[i].Length; j++)
                    {
                        fretNumbers[i][j] = fretNumber;
                        if (fretNumber > maxFretNumber)
                            maxFretNumber = fretNumber;

                        intervalEnumerator.MoveNext();
                        fretNumber += intervalEnumerator.Current.Semitones;
                    }

                    if (stringNumber < guitar.StringIntervals.Count)
                    {
                        fretNumber -= guitar.StringIntervals[stringNumber].Semitones;
                        if (fretNumber < minFretNumber)
                            minFretNumber = fretNumber;
                    }

                    stringNumber++;
                }
            }

            if (minFretNumber < 0)
            {
                ShiftFretNumbers(fretNumbers, -minFretNumber);
                maxFretNumber -= minFretNumber;
            }

            return maxFretNumber;
        }

        private static void ShiftFretNumbers(int[][] fretNumbers, int minFretNumber)
        {
            for (var i = 0; i < fretNumbers.Length; i++)
                for (var j = 0; j < fretNumbers[i].Length; j++)
                    fretNumbers[i][j] += minFretNumber;
        }

        private static bool EvaluateDegreeAnswer(DiatonicMode degree, string answer)
        {
            switch (degree)
            {
                case Ionian: return answer == "d" || answer == "do";
                case Dorian: return answer == "r" || answer == "re";
                case Phrygian: return answer == "m" || answer == "mi";
                case Lydian: return answer == "f" || answer == "fa";
                case Mixolydian: return answer == "s" || answer == "sol";
                case Aeolian: return answer == "l" || answer == "la";
                case Locrian: return answer == "t" || answer == "ti";
                default: throw new InvalidOperationException();
            }
        }

        private static bool EvaluateModeAnswer(DiatonicMode mode, Guitar guitar, int stringNumber, int position, int[][] fretNumbers, string answer)
        {
            var pitch = guitar.GetPitch(stringNumber, position + fretNumbers[0][0]);

            var combinations =
                from keyNote in new[] { pitch.Note, pitch.Note.Invert() }.Distinct()
                from modeName in new[] { mode.GetName(), mode.GetAlternativeName() }.Distinct()
                select $"{keyNote.ToString().ToLowerInvariant()} {modeName.ToLowerInvariant()}";

            return combinations.Contains(answer);
        }

        private static void PrintPracticeStats()
        {
            Console.WriteLine();
            Console.WriteLine(s_practiceStats);
            Console.WriteLine();
        }

        private static void PrintUsage(string errorMessage = null)
        {
            if (errorMessage != null)
            {
                Console.WriteLine(errorMessage);
                Console.WriteLine();
            }

            Console.WriteLine("Usage:");
            Console.WriteLine("");
            Console.WriteLine("help (-h): Help");
            Console.WriteLine("print-circle <mode>: Prints the circle of fifths for the specified mode (Major if omitted).");
        }
    }
}
