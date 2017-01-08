/*
Basic tones music player program
Copyright 2012 Alex Tam
http://inphamousdevelopment.wordpress.com/

Video demo: http://youtu.be/s-6B8H8Ieuk

If this code is used to make something cool, please make a video response.

 * * *

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MusicBeeper
{
    /*
     * http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/18fe83f0-5658-4bcf-bafc-2e02e187eb80
     * Octave 0 1 2 3 4 5 6 7

	Note

	 C 16 33 65 131 262 523 1046 2093

	 C# 17 35 69 139 277 554 1109 2217

	 D 18 37 73 147 294 587 1175 2349

	 D# 19 39 78 155 311 622 1244 2489

	 E 21 41 82 165 330 659 1328 2637

	 F 22 44 87 175 349 698 1397 2794

	 F# 23 46 92 185 370 740 1480 2960

	 G 24 49 98 196 392 784 1568 3136

	 G# 26 52 104 208 415 831 1661 3322

	 A 27 55 110 220 440 880 1760 3520

	 A# 29 58 116 233 466 932 1865 3729

	 B 31 62 123 245 494 988 1975 3951

     */

    public class Tones
    {
        public enum Octave
        {
            DoublePedal,
            Pedal,
            Deep,
            Low,
            Middle,
            Tenor,
            High,
            DoubleHigh,
            Size
        };

        public enum Duration
        {
            Whole = 1000,
            Half = 500,
            Third = 333,
            Quarter = 250,
            Eighth = 125,
            Sixteenth = 62
        };

        public enum Note
        {
            C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B, Size
        };

        public static int[] Frequency = {
                                            16,17,18,19,21,22,23,24,26,27,29,31,
                                            33,35,37,39,41,44,46,49,52,55,58,62,
                                            65,69,73,78,82,87,92,98,104,110,116,123,
                                            131,139,147,155,165,175,185,196,208,220,233,245,
                                            262,277,294,311,330,349,370,392,415,440,466,494,
                                            523,554,587,622,659,698,740,784,831,880,932,988,
                                            1046,1109,1175,1244,1328,1397,1480,1568,1661,1760,1865,1975,
                                            2093,2217,2349,2489,2637,2794,2960,3136,3322,3520,3729,3951
                                        };

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Play the song sent as a string
        /// </summary>
        /// <param name="song">Song in the format note-octave-duration</param>
        public static void PlaySong(string song)
        {
            string[] tokens = song.Split(',');

            int freq, dur;
            foreach (string token in tokens)
            {
                Parse(token, out freq, out dur);
                Beep.Play(freq, dur);
            }
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Play a single note in the default middle octave and quarter duration
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        public static void PlayNote(Note note)
        {
            PlayNote(note, Octave.Middle, Duration.Quarter);
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Play a single note with the specified note and octave
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        /// <param name="oct">See octave enum</param>
        public static void PlayNote(Note note, Octave oct)
        {
            PlayNote(note, oct, Duration.Quarter);
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Play a single note with the specified note and duration with middle octave
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        /// <param name="dur">See duration enum</param>
        public static void PlayNote(Note note, Duration dur)
        {
            PlayNote(note, Octave.Middle, dur);
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Play a single note
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        /// <param name="oct">See octave enum</param>
        /// <param name="dur">See duration enum</param>
        public static void PlayNote(Note note, Octave oct, Duration dur)
        {
            int freq = GetFreq(note, oct);
            Beep.Play(freq, (int)dur);
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Translates the note and octave to a frequency in hertz
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        /// <param name="oct">See octave enum</param>
        /// <returns>Hertz value of the note</returns>
        public static int GetFreq(Note note, Octave oct)
        {
            return GetFreq((int)note, (int)oct);
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Translates the note and octave to a frequency in hertz
        /// </summary>
        /// <param name="note">C,D,E,F,G,A,B</param>
        /// <param name="oct">Integer value matching octave enum</param>
        /// <returns>Hertz value of the note</returns>
        public static int GetFreq(Note note, int oct)
        {
            if (oct < 0 || oct >= (int)Octave.Size)
                throw new ArgumentOutOfRangeException("oct", "Octave value exceeds range.");

            return GetFreq((int)note, oct);
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Translates the note and octave to a frequency in hertz
        /// </summary>
        /// <param name="note">Integer value matching note enum</param>
        /// <param name="oct">Integer value matching octave enum</param>
        /// <returns>Hertz value of the note</returns>
        private static int GetFreq(int note, int oct)
        {
            return Frequency[(int)Note.Size * oct + note];
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the string of note
        /// </summary>
        /// <param name="line">String containing note</param>
        /// <param name="freq">Return the hertz of the note</param>
        /// <param name="dur">Return the duration of the note</param>
        public static void Parse(string line, out int freq, out int dur)
        {
            // Default values
            freq = 0;
            dur = (int)Duration.Quarter;
            int octIdx = (int)Octave.Middle;

            // 3 param : note-octave-duration
            // 2 param : note-duration
            // 1 param : note

            string[] tokens = line.Split('-');

            int noteIdx = ParseNote(tokens.First().ToUpper().Trim());

            if (tokens.Length >= 3)
            {
                octIdx = ParseOctive(tokens[1]);
            }

            if (tokens.Length >= 2)
            {
                dur = ParseDuration(tokens.Last());
            }

            if (tokens.First().ToUpper().Trim() == "P")
            {
                freq = 22000;
            }
            else
            {
                freq = GetFreq(noteIdx, octIdx);
            }
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Gets the duration of the note
        /// </summary>
        /// <param name="strValue">String containing note</param>
        /// <returns>Duration in milliseconds</returns>
        private static int ParseDuration(string strValue)
        {
            float step;
            int milliseconds;

            if (!float.TryParse(strValue, out step))
                step = (float)Duration.Whole;

            switch ((int)step)
            {
                case 1:
                    milliseconds = (int)Duration.Whole;
                    break;
                case 2:
                    milliseconds = (int)Duration.Half;
                    break;
                case 4:
                    milliseconds = (int)Duration.Quarter;
                    break;
                case 8:
                    milliseconds = (int)Duration.Eighth;
                    break;
                case 16:
                    milliseconds = (int)Duration.Sixteenth;
                    break;
                default:
                    milliseconds = (int)Duration.Whole;
                    break;
            }

            if (step - (int)step > 0.0)
                milliseconds += (milliseconds / 2);

            return milliseconds;
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Gets the octave of the note
        /// </summary>
        /// <param name="strValue">String containing the note</param>
        /// <returns></returns>
        private static int ParseOctive(string strValue)
        {
            int octIdx;

            if (!int.TryParse(strValue, out octIdx))
                octIdx = (int)Octave.Middle;

            return octIdx;
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Gets the enum integer value of the note
        /// </summary>
        /// <param name="strValue">The string containing the note</param>
        /// <returns>Enumeration index value of the note</returns>
        private static int ParseNote(string strValue)
        {
            int noteIdx;

            switch (strValue)
            {
                case "C":
                    noteIdx = (int)Note.C;
                    break;
                case "CS":
                    noteIdx = (int)Note.Cs;
                    break;
                case "D":
                    noteIdx = (int)Note.D;
                    break;
                case "DS":
                    noteIdx = (int)Note.Ds;
                    break;
                case "E":
                    noteIdx = (int)Note.E;
                    break;
                case "F":
                    noteIdx = (int)Note.F;
                    break;
                case "FS":
                    noteIdx = (int)Note.Fs;
                    break;
                case "G":
                    noteIdx = (int)Note.G;
                    break;
                case "GS":
                    noteIdx = (int)Note.Gs;
                    break;
                case "A":
                    noteIdx = (int)Note.A;
                    break;
                case "AS":
                    noteIdx = (int)Note.As;
                    break;
                case "B":
                    noteIdx = (int)Note.B;
                    break;
                default:
                    noteIdx = (int)Note.C;
                    break;
            }

            return noteIdx;
        }
    }
}
