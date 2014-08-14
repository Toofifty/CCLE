using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace CLEngine
{
    class Game
    {
        static Sprite player;
        static Sprite ball;
        static Sprite wallBottom;

        static float TPS = 10;
        static float IPS = 60;

        static volatile bool isRunning;
        static bool spacebarPressed = false;

        [STAThread]
        static void Main(string[] args)
        {
            bool continuing = true;
            while (continuing)
            {
                continuing = StartGame();
            }
        }

        static bool StartGame()
        {
            Engine.SetGameSize(160, 40);
            Engine.SetWindowTitle("CCLE Engine - ATARI Breakout");
            Engine.Init(Pixel.LoadTextLevel("basic"), 60);

            player = new Sprite(new char[15] {'/', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '\\'}, new Rect(36, 38, 15, 1), 2);
            ball = new Sprite('@', ConsoleColor.Red, new Rect(39, 17, 1, 1));
            ball.SetZ(2);
            Sprite wallTop = new Sprite(CharArray(159, '-'), new Rect(1, 0, 157, 1), 1);
            Sprite wallLeft = new Sprite(CharArray(39, '|'), new Rect(0, 1, 1, 38), 1);
            Sprite wallRight = new Sprite(CharArray(39, '|'), new Rect(158, 1, 1, 38), 1);
            wallBottom = new Sprite(CharArray(157, '-'), ConsoleColorArray(157, ConsoleColor.Gray), new Rect(1, 39, 157, 1), 1);

            player.AddCollider(wallLeft);
            player.AddCollider(wallRight);

            ball.AddCollider(player);
            ball.AddCollider(wallTop);
            ball.AddCollider(wallLeft);
            ball.AddCollider(wallRight);
            ball.AddCollider(wallBottom);

            CreateBlocks(ball);

            player.dirty = true;

            isRunning = true;
            Thread gameLoopThread = new Thread(GameLoop);
            gameLoopThread.Start();

            bool restart = AcceptInput();

            isRunning = false;
            Engine.StopRender();
            gameLoopThread.Join();

            return restart;
        }

        static void GameLoop()
        {
            string vdir = "UP";
            string hdir = "RIGHT";
            int speed = 2;
            int angle = 45;
            int actualAngle = 90 - angle;
            int xc;
            int yc;

            while (!spacebarPressed)
            {
                ball.SetPosition(player.GetPosition() + new Position(2, 0));
                Thread.Sleep((int)((1.0 / TPS) * 1000));
            }
            spacebarPressed = false;

            while (isRunning)
            {                
                double xcc = Math.Cos(actualAngle * Math.PI / 180) * speed;
                double ycc = Math.Sin(actualAngle * Math.PI / 180) * speed;
                if (xcc >= 0) xc = (int)Math.Ceiling(xcc);
                else xc = (int)Math.Floor(xcc);
                if (ycc >= 0) yc = (int)Math.Ceiling(ycc);
                else yc = (int)Math.Floor(ycc);

                Engine.QueueMessage(speed + "cos(" + actualAngle + ")=" + xc + " " + speed + "sin(" + actualAngle + ") = " + yc);

                if (!ball.MoveCollision(hdir, xc))
                {
                    angle *= -1;
                }

                if (!ball.MoveCollision(vdir, yc))
                {
                    if (ball.lastCollision == player)
                    {
                        Console.Beep(400, 100);
                        double distanceD = Position.Distance(ball.GetPosition(), player.GetPosition());
                        int distance = (int)Math.Ceiling(distanceD);
                        int distanceFromCentre = distance - 8;
                        if (distanceFromCentre > 3)
                        {
                            angle = 225 - angle;
                            angle = Math.Min(angle, 80);
                        }
                        else if (distanceFromCentre < -3)
                        {
                            angle = 135 - angle;
                            angle = Math.Max(angle, -80);
                        }
                        else
                        {
                            angle = 180 - angle;
                        }
                    }
                    else if (ball.lastCollision == wallBottom)
                    {
                        BaseSprite.ClearSprites();
                        Engine.renderer.NewScene(Pixel.LoadTextLevel("lose"));
                        Engine.renderer.Refresh();
                        break;
                    }
                    else
                    {
                        angle = 180 - angle;
                    }
                }

                if (angle > 180) angle -= 360;
                if (angle < -180) angle += 360;
                actualAngle = 90 - angle;
                if (actualAngle < 0) actualAngle += 360;
                if (actualAngle > 360) actualAngle -= 360;
                if (angle == 90 || angle == -90) angle += 10;
                Thread.Sleep((int)((1 / TPS) * 1000));
            }
        }

        static bool AcceptInput()
        {
            bool acceptingInput = true;
            while (acceptingInput)
            {
                try
                {
                    if (Keyboard.IsKeyDown(Key.Left))
                    {
                        player.MoveCollision("LEFT", 1);
                    }
                    if (Keyboard.IsKeyDown(Key.Right))
                    {
                        player.MoveCollision("RIGHT", 1);
                    }
                    if (Keyboard.IsKeyToggled(Key.Space))
                    {
                        spacebarPressed = true;
                    }
                    if (Keyboard.IsKeyDown(Key.Escape))
                    {
                        acceptingInput = false;
                        return false;
                    }
                    if (Keyboard.IsKeyDown(Key.Return))
                    {
                        acceptingInput = false;
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Thread.Sleep((int)((1.0 / IPS) * 1000));
            }
            return false;
        }

        static void CreateBlocks(Sprite ball)
        {

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

        static ConsoleColor[] ConsoleColorArray(int length, ConsoleColor c)
        {
            ConsoleColor[] arr;
            arr = new ConsoleColor[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = c;
            }
            return arr;
        }
    }
}
