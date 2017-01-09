using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCLE.Game
{
    class Game
    {
        [STAThread]
        static void Main(string[] args)
        {
            new BreakoutGame(160, 80).Run();
        }

        static string ReadyUp()
        {
            string song = string.Empty;

            song += "E-5-4,B-4-8,C-5-8,D-5-4";

            return song;
        }

        //static string Tetris()
        //{
        //    string song = string.Empty;

        //    song += "E-5-4,B-4-8,C-5-8,D-5-4,C-5-8,B-4-8,";
        //    song += "A-4-4,A-4-8,C-5-8,E-5-4,D-5-8,C-5-8,";
        //    song += "B-4-4.5,C-5-8,D-5-4,E-5-4,";
        //    song += "C-5-4,A-4-4,A-4-8,A-4-8,B-4-8,C-5-8,";

        //    song += "D-5-4.5,F-5-8,A-5-4,G-5-8,F-5-8,";
        //    song += "E-5-4.5,C-5-8,E-5-4,D-5-8,C-5-8,";
        //    song += "B-4-4,B-4-8,C-5-8,D-5-4,E-5-4,";
        //    song += "C-5-4,A-4-4,A-4-4,P-4,";

        //    song += "E-5-2,C-5-2,D-5-2,B-4-2,C-5-2,A-4-2,";
        //    song += "GS-4-2,B-4-4,P-4,E-5-2,C-5-2,D-5-2,B-4-2,";
        //    song += "C-5-4,E-5-4,A-5-2,GS-5-2";

        //    return song;
        //}

        //static string Mario()
        //{
        //    string song = string.Empty;

        //    //song += "D-7-2,F-6-2,C-7-2,F-6-2,b-6-2,A-6-4,b-6-2,C-7-4,b-6-2,A-6-2,P-4,A-6-4,";
        //    //song += "e-7-4,G-6-4,A-6-4,e-7-4,G-6-4,A-6-4,P-4";

        //    song += "F-4-8,F-4-8,F-4-8,G-4-1,D-5-1,C-5-8,B-4-8,A-4-8,G-5-1,D-5-2,C-5-8,B-4-8,A-4-8,G-5-1,D-5-2,";
        //    song += "C-5-8,B-4-8,C-5-8,A-4-1,P-4,";
        //    song += "F-4-8,F-4-8,F-4-8,G-4-1,D-5-1,C-5-8,B-4-8,A-4-8,G-5-1,D-5-2,C-5-8,B-4-8,A-4-8,G-5-1,D-5-2,";
        //    song += "C-5-8,B-4-8,C-5-8,A-4-1,P-4,";
        //    song += "D-4-4,D-4-16,E-4-2,E-4-8,C-5-8,B-4-8,A-4-8,G-4-8,G-4-8,A-4-16,B-4-16,A-4-4,E-4-8,F-4-4,";
        //    song += "D-4-4,D-4-16,E-4-2,E-4-8,C-5-8,B-4-8,A-4-8,G-4-8,D-5-4,A-4-2";

        //    return song;
        //}
    }
}
