using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCLE
{
    class Renderer
    {
        static Window window;

        static List<Rect> dirty = new List<Rect>();

        public static void Init(Window w)
        {
            window = w;
        }

        public static void FullRender()
        {
            Console.Clear();
            foreach (Sprite sprite in SpriteHandler.GetSprites())
            {
                sprite.DrawTo(window);
            }
        }

        static void UpdateRect(Rect rect)
        {
            char[,] blockTexture = new char[rect.h, rect.w];
            ConsoleColor[,] blockBackground = new ConsoleColor[rect.h, rect.w];
            ConsoleColor[,] blockForeground = new ConsoleColor[rect.h, rect.w];
            
            foreach (Sprite sprite in SpriteHandler.GetSprites())
            {
                sprite.FillScreenRect(rect, blockTexture, blockBackground, blockForeground);
            }

            window.DrawColoredChars(rect.x, rect.y, blockTexture, blockBackground, blockForeground);
        }

        public static void UpdateSprite(Sprite sprite)
        {
            dirty.Add(sprite);
            dirty.Add(sprite.GetPreviousRect());
        }

        public static void Draw()
        {
            var done = new List<Rect>();
            foreach (Rect r in dirty.ToArray())
            {
                UpdateRect(r);
                done.Add(r);
            }

            foreach (Rect r in done)
            {
                dirty.Remove(r);
            }


            foreach (Sprite sprite in SpriteHandler.GetTrashed())
            {
                UpdateRect(sprite.GetPreviousRect());
                UpdateRect(sprite);
            }
            window.SetCursorPosition(0, 0);
        }
    }

    /// <summary>
    /// All handling of the Console comes through here
    /// </summary>
    class Window : Rect
    {
        public string title;

        public Window()
            : base(Config.DEFAULT_WINDOW)
        {
        }

        public Window(int w, int h)
            : base(Config.DEFAULT_POS, new IVector(w, h))
        {
        }

        public void Init()
        {
            Console.CursorVisible = false;
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            // Console.SetWindowPosition(x, y);
        }

        public override void SetSize(int w, int h)
        {
            base.SetSize(w, h);
            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w, h);
        }

        public void SetCursorPosition(IVector pos)
        {
            SetCursorPosition(pos.x, pos.y);
        }

        public void SetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }

        public void SetTitle(string title)
        {
            this.title = title;
            Console.Title = title;
        }

        public void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public void DrawString(int x, int y, string str, ConsoleColor foreground, ConsoleColor background)
        {
            SetColors(foreground, background);
            DrawString(x, y, str);
        }

        public void DrawString(int x, int y, string str)
        {
            try
            {
                SetCursorPosition(x, y);
                Write(str);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public void DrawBlock(int x, int y, string[] block, ConsoleColor foreground, ConsoleColor background)
        {
            SetColors(foreground, background);
            DrawBlock(x, y, block);
        }

        public void DrawBlockDefault(int x, int y, string[] block)
        {
            DrawBlock(x, y, block, ConsoleColor.White, ConsoleColor.Black);
        }

        public void DrawBlock(int x, int y, string[] block)
        {
            foreach (string str in block)
            {
                DrawString(x, y++, str);
            }
        }

        public void Write(Object str)
        {
            if (Console.CursorTop == h && Console.CursorLeft == w)
                return;
            Console.Write(str);
        }

        public void DrawColoredChars(int x, int y, char[,] block, ConsoleColor[,] foreground, ConsoleColor[,] background)
        {
            for (var j = 0; j < block.GetLength(Util.HEIGHT); j++)
            {
                try
                {
                    SetCursorPosition(x, y + j);
                    for (var i = 0; i < block.GetLength(Util.WIDTH); i++)
                    {
                        Console.ForegroundColor = foreground[j, i];
                        Console.BackgroundColor = background[j, i];
                        Write(block[j, i]);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                }
            }
        }

        public void Beep(int freq, int duration)
        {
            Console.Beep(freq, duration);
        }

        public override string ToString()
        {
            return "Window" + base.ToString();
        }
    }
}
