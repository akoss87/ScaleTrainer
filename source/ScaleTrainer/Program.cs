using System;
using System.Text;
using Music;

using static Music.DiatonicMode;
using static Music.DiatonicScale;
using static Music.Interval;

namespace ScaleTrainer
{
    class Program
    {
        // scaletrainer print-Circle ionian
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if ("print-circle".Equals(args[0], StringComparison.OrdinalIgnoreCase))
                {
                    DiatonicMode diatonicMode;

                    if (args.Length > 1)
                        diatonicMode = (DiatonicMode)Enum.Parse(typeof(DiatonicMode), args[1], ignoreCase: true);
                    else
                        diatonicMode = Major;

                    PrintDiatonicScales(diatonicMode);
                }
            }
        }

        private static void PrintDiatonicScales(DiatonicMode diatonicMode)
        {
            var notes = CreateScaleArray();

            GetNotes(notes, Note.GFlat, Major);

            var keyNote = notes[diatonicMode - Ionian];

            var title = $"Circle of {diatonicMode.GetName()} scales";
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length));
            Console.WriteLine();

            var sb = new StringBuilder();
            for (var i = -SemitonesPerOctave / 2; i <= SemitonesPerOctave / 2; i++)
            {
                GetNotes(notes, keyNote, diatonicMode);

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
    }
}
