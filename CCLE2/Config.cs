using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{
    class Config
    {
        public static ConsoleColor DEFAULT_FOREGROUND = ConsoleColor.White;
        public static ConsoleColor DEFAULT_BACKGROUND = ConsoleColor.Black;

        public static IVector DEFAULT_SIZE = new IVector(80, 20);
        public static IVector DEFAULT_POS = new IVector(0, 0);
        public static Rect DEFAULT_WINDOW = new Rect(DEFAULT_POS, DEFAULT_SIZE);

        public static char MESSAGE_QUEUE_BORDER = ' ';
        public static char MESSAGE_QUEUE_BACKGROUND = ' ';
    }
}
