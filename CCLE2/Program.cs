using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game
{
    class Game
    {
        static CCLE.Window window;

        static int TPS = 20;
        static int score = 0;

        static bool spacebarPressed = false;
        static bool gameRunning = true;

        public static Random random = new Random();

        static List<Key> keysDown = new List<Key>();

        static Paddle paddle;
        static Ball ball;

        [STAThread]
        static void Main(string[] args)
        {
            window = new CCLE.Window(200, 81);
            window.SetTitle("Test Window");
            window.Init();

            CCLE.Renderer.Init(window);

            var rand = new Random();
            
            while (gameRunning)
            {
                //MusicBeeper.Tones.PlaySong(ReadyUp());
                gameRunning = RunGame();
                TPS = 20;
                score = 0;
            }

        }

        static bool RunGame()
        {
            //var mq = new CCLE.MessageQueue(new CCLE.Rect(0, 70, window.w, 10));
            //mq.SetTitle("Log");

            CCLE.MessageQueue.Init(new CCLE.Rect(0, 70, window.w, 10));
            CCLE.MessageQueue.SetTitle("Log");

            CCLE.SpriteHandler.NewSprite("top-border", 0, 1, window.w, 1, ' ').SetBackground(ConsoleColor.White);
            CCLE.SpriteHandler.NewSprite("right-border", window.w - 1, 1, 1, window.h - 13, ' ').SetBackground(ConsoleColor.White);
            CCLE.SpriteHandler.NewSprite("left-border", 0, 1, 1, window.h - 13, ' ').SetBackground(ConsoleColor.White);
            CCLE.SpriteHandler.NewSprite("bottom-border", 0, 69, window.w, 1, ' ').SetBackground(ConsoleColor.White);

            paddle = new Paddle(window.w / 2 - 11, 67);
            ball = new Ball(window.w / 2, 66);

            gameRunning = true;
            Thread gl = new Thread(GameLoop);
            gl.Start();

            bool restart = InputLoop();

            gl.Join();

            Console.ReadKey();
            return restart;
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
                        paddle.MoveLeft();
                    }
                    if (Keyboard.IsKeyDown(Key.Right))
                    {
                        paddle.MoveRight();
                    }
                    if (Keyboard.IsKeyDown(Key.Space))
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
                Thread.Sleep((int)((1.0 / TPS) * 1000));
            }
            return false;
        }

        static void GameLoop()
        {
            CCLE.SpriteHandler.NewSprite("msg", window.w / 2 - 10, 20, new string[] { "Press SPACEBAR to begin" });
            CCLE.Renderer.Draw();

            while (!spacebarPressed)
            {
                Thread.Sleep((int)((1.0 / TPS) * 1000));
            }

            //CCLE.SpriteHandler.NewSprite(20, 20, 160, 20, '/');

            CCLE.SpriteHandler.TrashSprite("msg");
            CCLE.Renderer.Draw();

            ball.Release();

            while (gameRunning)
            {
                ball.Update();
                CCLE.Renderer.Draw();
            }
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

    class Paddle
    {
        CCLE.Sprite sprite;

        public Paddle(int x, int y)
        {
            sprite = CCLE.SpriteHandler.NewSprite("paddle", x, y, 
                new string[] { "  /=================\\  ", "/=====================\\" });
        }

        public void MoveLeft()
        {
            sprite.SetPosition(sprite.x - 1, sprite.y);
        }

        public void MoveRight()
        {
            sprite.SetPosition(sprite.x + 1, sprite.y);
        }
    }

    class Ball
    {
        CCLE.Sprite sprite;

        const double MIN_ANGLE = Math.PI / 12;
        const double MAX_ANGLE = 11 * Math.PI / 12;

        double angle;
        double speed = 0;

        CCLE.DVector pos;

        public Ball(int x, int y)
        {
            y--;
            sprite = CCLE.SpriteHandler.NewSprite("ball", x, y, 2, 2, '@');
            sprite.SetForeground(ConsoleColor.Red);
            pos = new CCLE.DVector(x, y);
        }

        public void Release()
        {
            angle = Game.random.NextDouble() * (MAX_ANGLE - MIN_ANGLE) + MIN_ANGLE;
            speed = 0.05;

            CCLE.MessageQueue.Message("" + angle);
        }

        public void Update()
        {
            double dx = Math.Cos(-angle) * speed;
            double dy = Math.Sin(-angle) * speed;

            speed *= 1.00005;

            pos.Add(dx, dy);

            sprite.SetPosition((int) pos.x, (int) pos.y);

            var col = sprite.AnyCollidingSide();
            if (col.IsColliding())
            {
                pos.Subtract(dx, dy);
                col.CancelOpposing();
                CCLE.MessageQueue.Message("Collision: " + col.ToString());

                if (col.Right() || col.Left())
                {
                    angle += Math.PI / 2;
                    angle *= -1;
                    angle -= Math.PI / 2;
                }

                if (col.Up() || col.Down())
                {
                    angle *= -1;
                }

                if (col.Down())
                {
                    if (col.IsColliding("bottom-border"))
                    {
                        // die
                    }
                    else if (col.IsColliding("paddle"))
                    {
                        // do things to angle
                    }
                }

                dx = Math.Cos(-angle) * speed;
                dy = Math.Sin(-angle) * speed;
                pos.Add(dx, dy);
            }

            sprite.SetPosition((int) pos.x, (int) pos.y);
        }
    }
}
