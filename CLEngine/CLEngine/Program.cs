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
        static LargeSprite obstacle;

        static void Main(string[] args)
        {
            Engine.level = Pixel.LoadTextLevel("basic");
            obstacle = new LargeSprite(new char[9] { '#', '#', '#', '#', '#', '#', '#', '#', '#' }, new ConsoleColor[9] { ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, ConsoleColor.Red, }, new Rect(20, 10, 3, 3), 1);
            player = new LargeSprite(new char[4] { '1', '2', '3', '4' }, new Rect(10, 10, 2, 2), 1);
            Engine.activeScene = (Pixel[])Engine.level.Clone();
            Engine.renderer = new Renderer(Engine.level, 60);
            Engine.UpdateSpriteInScene(player);
            Engine.UpdateSpriteInScene(obstacle);
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
                        player.Move("UP", 1);
                    }
                    if (keyPress == ConsoleKey.DownArrow)
                    {
                        player.Move("DOWN", 1);
                    }
                    if (keyPress == ConsoleKey.LeftArrow)
                    {
                        player.Move("LEFT", 1);
                    }
                    if (keyPress == ConsoleKey.RightArrow)
                    {
                        player.Move("RIGHT", 1);
                    }
                    if (keyPress == ConsoleKey.Escape)
                    {
                        acceptingInput = false;
                    }
                    if (Sprite.CheckColliding(player, obstacle))
                    {
                        foreach (Sprite s in player.sprites)
                        {
                            s.symbol.bColor = ConsoleColor.Red;
                        }
                    }
                    else
                    {
                        foreach (Sprite s in player.sprites)
                        {
                            s.symbol.bColor = ConsoleColor.Black;
                        }
                    }
                    Engine.UpdateAllSprites();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        
    }
}
