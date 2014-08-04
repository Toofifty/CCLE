using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLEngine
{
    class Game
    {
        static LargeSprite player;

        static void Main(string[] args)
        {
            Console.Title = "CCLE Engine";
            Console.CursorVisible = false;
            Engine.level = Pixel.LoadTextLevel("basic");
            LargeSprite obstacle = new LargeSprite(CharArray(4, '+'), new Rect(20, 10, 2, 2), 1);
            player = new LargeSprite(CharArray(4, (char)4), new Rect(10, 10, 2, 2), 2);
            LargeSprite wallTop = new LargeSprite(CharArray(79, '-'), new Rect(1, 0, 77, 1), 1);
            LargeSprite wallLeft = new LargeSprite(CharArray(19, '|'), new Rect(0, 1, 1, 18), 1);
            LargeSprite wallRight = new LargeSprite(CharArray(19, '|'), new Rect(79, 1, 1, 18), 1);
            LargeSprite wallBottom = new LargeSprite(CharArray(77, '-'), new Rect(1, 19, 77, 1), 1);

            player.AddLargeCollider(obstacle);
            player.AddLargeCollider(wallTop);
            player.AddLargeCollider(wallLeft);
            player.AddLargeCollider(wallRight);
            player.AddLargeCollider(wallBottom);

            Engine.activeScene = (Pixel[])Engine.level.Clone();
            Engine.renderer = new Renderer(Engine.level, 60);
            Engine.UpdateAllSprites();
            Thread rendererThread = new Thread(Engine.renderer.RenderScene);
            rendererThread.Start();

            AcceptInput();

            Engine.renderer.Stop();
            rendererThread.Join();
        }

        static void AcceptInput()
        {
            bool acceptingInput = true;
            while (acceptingInput)
            {
                try
                {
                    ConsoleKey keyPress = Console.ReadKey(true).Key;
                    if (keyPress == ConsoleKey.UpArrow)
                    {
                        if (!player.MoveCollision("UP", 1)) Console.Beep(2000, 10);
                    }
                    if (keyPress == ConsoleKey.DownArrow)
                    {
                        player.MoveCollision("DOWN", 1);
                    }
                    if (keyPress == ConsoleKey.LeftArrow)
                    {
                        player.MoveCollision("LEFT", 1);
                    }
                    if (keyPress == ConsoleKey.RightArrow)
                    {
                        player.MoveCollision("RIGHT", 1);
                    }
                    if (keyPress == ConsoleKey.Escape)
                    {
                        acceptingInput = false;
                    }

                    if (keyPress == ConsoleKey.D)
                    {                        
                        Engine.renderer.debugMode = !Engine.renderer.debugMode;
                        Engine.renderer.Refresh();
                    }
                    Engine.UpdateAllSprites();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        static char[] CharArray(int length, char ch)
        {
            char[] arr;
            arr = new char[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = ch;
            }
            return arr;
        }
    }
}
