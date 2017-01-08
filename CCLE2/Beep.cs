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
using System.IO;
using System.Media;

namespace MusicBeeper
{
    public struct BeepData
    {
        public double Frequency;
        public double Duration;
    }

    //------------------------------------------------------------------------------------------------

    public class Beep
    {
        /// <summary>
        /// Plays the sound
        /// </summary>
        /// <param name="data">Struct containing the frequency and duration</param>
        public static void Play(BeepData data)
        {
            BeepBeep(1000, data.Frequency, data.Duration);
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Plays the sound
        /// </summary>
        /// <param name="frequency">Hertz value</param>
        /// <param name="duration">How long the sound is played</param>
        public static void Play(double frequency, double duration)
        {
            BeepBeep(1000, frequency, duration);
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a wav of the sound in memory and plays it through the computer speaker
        /// </summary>
        /// <remarks>
        /// Copied from:
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/18fe83f0-5658-4bcf-bafc-2e02e187eb80
        /// </remarks>
        /// <param name="Amplitude">Amplitude of the sine wave</param>
        /// <param name="Frequency">Frequency of the sine wave</param>
        /// <param name="Duration">Duration of the sound</param>
        private static void BeepBeep(double Amplitude, double Frequency, double Duration)
        {
            Duration += Duration * 0.1;

            double Amp = ((Amplitude * (System.Math.Pow(2, 15))) / 1000) - 1;
            double DeltaFT = 2 * Math.PI * Frequency / 44100.0;

            int Samples = (int)(441.0 * Duration / 10.0);
            int Bytes = Samples * sizeof(int);
            int[] Hdr = { 0X46464952, 36 + Bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, Bytes };

            using (MemoryStream MS = new MemoryStream(44 + Bytes))
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    for (int I = 0; I < Hdr.Length; I++)
                    {
                        BW.Write(Hdr[I]);
                    }
                    for (int T = 0; T < Samples; T++)
                    {
                        short Sample = System.Convert.ToInt16(Amp * Math.Sin(DeltaFT * T));
                        BW.Write(Sample);
                        BW.Write(Sample);
                    }

                    BW.Flush();
                    MS.Seek(0, SeekOrigin.Begin);
                    using (SoundPlayer SP = new SoundPlayer(MS))
                    {
                        SP.PlaySync();
                    }
                }
            }

            System.Threading.Thread.Sleep(20);
        }
    }
}