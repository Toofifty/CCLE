using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{
    class MessageQueue
    {
        static Sprite background;
        static Sprite text;

        static string blankString = "";

        public static void Init(Rect rect)
        {
            background = SpriteHandler.NewSprite(rect.x, rect.y, rect.w, rect.h, Config.MESSAGE_QUEUE_BORDER);
            text = SpriteHandler.NewSprite(rect.x + 1, rect.y + 1, rect.w - 2, rect.h - 2, Config.MESSAGE_QUEUE_BACKGROUND);
            blankString = text.GetTexture()[0];

            background.SetBackground(ConsoleColor.DarkCyan);

            background.SetZ(99);
            text.SetZ(99);
        }

        public static void SetTitle(string title)
        {
            if (title.Length <= background.w - 2)
            {
                background.GetTexture()[0] = Config.MESSAGE_QUEUE_BORDER + Util.PadRight(title, Config.MESSAGE_QUEUE_BORDER, background.w - 1);
                text.Redraw();
            }
        }

        public static void Message(string message)
        {
            if (message.Length <= text.w)
            {
                var line = text.GetTexture();
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == blankString)
                    {
                        line[i] = Util.PadRight(message, Config.MESSAGE_QUEUE_BACKGROUND, line[0].Length);
                        text.Redraw();
                        return;
                    }
                }

                for (var i = 0; i < text.h - 1; i++)
                {
                    line[i] = line[i + 1];
                }
                line[line.Length - 1] = Util.PadRight(message, Config.MESSAGE_QUEUE_BACKGROUND, line[0].Length);
            }
        }
    }
}
