using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{
    class Messages
    {
        static Sprite BackSprite;
        static Sprite TextSprite;

        static string BlankString;

        public static void Init(Rect rect)
        {
            BackSprite = SpriteHandler.NewSprite(rect.X, rect.Y, rect.W, rect.H, Config.MESSAGE_QUEUE_BORDER);
            TextSprite = SpriteHandler.NewSprite(rect.X + 1, rect.Y + 1, rect.W - 2, rect.H - 2, Config.MESSAGE_QUEUE_BACKGROUND);
            BlankString = TextSprite.GetTexture()[0];

            BackSprite.SetBackground(ConsoleColor.DarkCyan);

            BackSprite.SetZ(99);
            TextSprite.SetZ(99);
        }

        public static void Trash()
        {
            SpriteHandler.TrashSprite(BackSprite.GetName());
            SpriteHandler.TrashSprite(TextSprite.GetName());
        }

        public static void SetTitle(string title)
        {
            if (title.Length <= BackSprite.W - 2)
            {
                BackSprite.GetTexture()[0] = Config.MESSAGE_QUEUE_BORDER + Util.PadRight(title, Config.MESSAGE_QUEUE_BORDER, BackSprite.W - 1);
                TextSprite.Redraw();
            }
        }

        public static void Log(string message)
        {
            if (message.Length <= TextSprite.W)
            {
                var line = TextSprite.GetTexture();
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == BlankString)
                    {
                        line[i] = Util.PadRight(message, Config.MESSAGE_QUEUE_BACKGROUND, line[0].Length);
                        TextSprite.Redraw();
                        return;
                    }
                }

                for (var i = 0; i < TextSprite.H - 1; i++)
                {
                    line[i] = line[i + 1];
                }
                line[line.Length - 1] = Util.PadRight(message, Config.MESSAGE_QUEUE_BACKGROUND, line[0].Length);
            }
        }
    }
}
