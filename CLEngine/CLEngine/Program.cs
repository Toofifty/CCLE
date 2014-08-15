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
    /// <summary>
    /// Main Game and Game Loop
    /// </summary>
    class Game
    {
        static Sprite player;
        static Sprite ball;
        static Sprite wallBottom;

        static float TPS = 20;
        static float IPS = 80;
        static int score = 0;

        static volatile bool isRunning;
        static bool spacebarPressed = false;

        // Must be on STA to not be bugged by VS
        [STAThread]
        static void Main(string[] args)
        {
            bool continuing = true;
            while (continuing)
            {
                // StartGame will return bool to restart (true)
                // or exit (false)
                continuing = StartGame();
                Console.Clear();
                Console.Beep(2000, 100);
                spacebarPressed = false;
                TPS = 20;
                score = 0;
            }
        }

        static bool StartGame()
        {
            // Initial game properties
            Engine.SetGameSize(320, 80);
            Engine.SetWindowTitle("CCLE Engine - ATARI Breakout");
            Engine.Init(Pixel.LoadTextLevel("basic"), 60);

            // Initiate sprites
            player = new Sprite(new char[19] { '/', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '=', '\\' }, new Rect(150, 78, 19, 1), 2);
            ball = new Sprite('@', ConsoleColor.Red, new Rect(150, 78, 1, 1));
            ball.SetZ(2);
            Sprite wallTop = new Sprite(Tools.CharArray(316, '-'), new Rect(1, 0, 316, 1), 1);
            Sprite wallLeft = new Sprite(Tools.CharArray(78, '|'), new Rect(0, 1, 1, 78), 1);
            Sprite wallRight = new Sprite(Tools.CharArray(78, '|'), new Rect(317, 1, 1, 78), 1);
            wallBottom = new Sprite(Tools.CharArray(317, '-'), Tools.ConsoleColorArray(317, ConsoleColor.Gray), new Rect(1, 79, 316, 1), 1);

            // Tag the player sprite for further use
            player.tag = "player";

            // Add player (paddle) colliders
            player.AddCollider(wallLeft);
            player.AddCollider(wallRight);

            // Add ball colliders
            ball.AddCollider(player);
            ball.AddCollider(wallTop);
            ball.AddCollider(wallLeft);
            ball.AddCollider(wallRight);
            ball.AddCollider(wallBottom);

            CreateBlocks(ball);

            // Update player sprite
            player.dirty = true;

            // Start a game loop
            isRunning = true;
            Thread gameLoopThread = new Thread(GameLoop);
            gameLoopThread.Start();

            bool restart = InputLoop();

            // End game and renderer loops
            isRunning = false;
            Engine.StopRender();
            gameLoopThread.Join();

            return restart;
        }

        static void CreateBlocks(Sprite ball)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 45; i++)
                {
                    Thread.Sleep(10);
                    ConsoleColor c;
                    switch (new Random().Next(0, 5))
                    {
                        case 0:
                            c = ConsoleColor.Blue;
                            break;
                        case 1:
                            c = ConsoleColor.Red;
                            break;
                        case 2:
                            c = ConsoleColor.Yellow;
                            break;
                        case 3:
                            c = ConsoleColor.Cyan;
                            break;
                        case 4:
                            c = ConsoleColor.Magenta;
                            break;
                        default:
                            c = ConsoleColor.White;
                            break;
                    }
                    Sprite tBlock = new Sprite(Tools.CharArray(10, '$'), Tools.ConsoleColorArray(10, c), new Rect(2 + i * 7, 2 + j*4, 5, 2), 1);
                    tBlock.tag = "block";
                    ball.AddCollider(tBlock);
                }
            }
        }
        
        static void GameLoop()
        {
            string vdir = "UP";
            string hdir = "RIGHT";
            int speed = 1;
            int angle = 45;
            int actualAngle = 90 - angle;
            int xc;
            int yc;

            ball.SetPosition(player.GetPosition() + new Position(2, 0));

            // Wait to release the ball
            while (!spacebarPressed)
            {
                ball.SetPosition(player.GetPosition() + new Position(2, 0));
                Thread.Sleep((int)((1.0 / TPS) * 1000));
                //Engine.QueueMessage("Waiting for spacebar!");
            }
            spacebarPressed = false;

            double[] ballPos = new double[2]{ball.GetPosition().x, ball.GetPosition().y};

            // Loop
            while (isRunning)
            {                
                double xcc = Math.Cos(actualAngle * Math.PI / 180) * speed;
                double ycc = Math.Sin(actualAngle * Math.PI / 180) * speed;
                if (xcc >= 0) xc = (int)Math.Ceiling(xcc);
                else xc = (int)Math.Floor(xcc);
                if (ycc >= 0) yc = (int)Math.Ceiling(ycc);
                else yc = (int)Math.Floor(ycc);

                //Engine.QueueMessage(speed + "cos(" + actualAngle + ")=" + xc + " " + speed + "sin(" + actualAngle + ") = " + yc);

                if (!ball.MoveCollision(hdir, xc))
                {
                    if (ball.lastCollision.tag == "block")
                    {
                        Sprite.RemoveSprite(ball.lastCollision);
                        ball.RemoveCollider(ball.lastCollision);
                        Console.Beep((int)Math.Max(Math.Pow(2, (score / 225) * 15), 37), 100);
                        TPS *= 1.005f;
                        IPS = 4 * TPS;
                        score += 1;
                    }
                    angle *= -1;
                }

                if (!ball.MoveCollision(vdir, yc))
                {

                    if (ball.lastCollision.tag == "block")
                    {
                        Sprite.RemoveSprite(ball.lastCollision);
                        ball.RemoveCollider(ball.lastCollision);
                        Console.Beep((int)Math.Max(Math.Pow(2, (score / 450) * 15), 37), 100);
                        TPS *= 1.005f;
                        IPS = 4 * TPS;
                        score += 1;
                    }

                    if (ball.lastCollision == player)
                    {
                        Console.Beep(400, 100);
                        double distanceD = Position.Distance(ball.GetPosition(), player.GetPosition());
                        int distance = (int)Math.Ceiling(distanceD);
                        int distanceFromCentre = distance - 10;
                        if (distanceFromCentre > 1)
                        {
                            angle -= 45;
                        }
                        else if (distanceFromCentre < -1)
                        {
                            angle += 45;
                        }
                        angle = 180 - angle;
                    }
                    else if (ball.lastCollision == wallBottom)
                    {
                        Sprite.ClearSprites();
                        Engine.renderer.NewScene(Pixel.LoadTextLevel("lose"));
                        Engine.renderer.Refresh();
                        Console.WriteLine("  SCORE: " + score + " FINAL SPEED: " + TPS);
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
                Engine.QueueMessage("TPS: " + TPS + "ANGLE" + angle);
                Thread.Sleep((int)((1 / TPS) * 1000));
            }
        }

        static bool InputLoop()
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
    }
}
