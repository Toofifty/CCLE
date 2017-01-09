using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCLE.Game
{
    class BreakoutGame : BaseGame
    {
        Paddle Paddle;
        Ball Ball;

        int Score;

        private static BreakoutGame _instance;

        public BreakoutGame(int w, int h)
            : base("Breakout v0.2", new Window(w, h + 1))
        {
            _instance = this;
        }

        public static BreakoutGame Instance()
        {
            return _instance;
        }

        public override void OnInit()
        {
            var w = Window.W;
            var h = Window.H;

            Score = 0;
            Messages.Init(new Rect(0, 70, Window.W, 10));

            SpriteHandler.NewSprite(   "top-border",     0,  1, w,      1, ' ')
                .SetBackground(ConsoleColor.White);
            SpriteHandler.NewSprite( "right-border", w - 1,  1, 1, h - 13, ' ')
                .SetBackground(ConsoleColor.White);
            SpriteHandler.NewSprite(  "left-border",     0,  1, 1, h - 13, ' ')
                .SetBackground(ConsoleColor.White);
            SpriteHandler.NewSprite("bottom-border",     0, 69, w,      1, ' ')
                .SetBackground(ConsoleColor.White);

            Paddle = new Paddle(w / 2 - 11, 67);
            Ball = new Ball(w / 2, 66);
            Render();

            CreateBlocks();

            MusicBeeper.Tones.PlaySong(ReadyUp());
            Render();

            SpriteHandler.NewSprite("msg", w / 2 - 10, 20, new string[] { "Press SPACEBAR to begin" });
            Render();

            WaitForKey(Key.Space);

            SpriteHandler.TrashSprite("msg");
            Render();

            Ball.Release();
        }

        static string ReadyUp()
        {
            string song = string.Empty;

            song += "E-5-4,B-4-8,C-5-8,D-5-4";

            return song;
        }

        public override void OnExit()
        {
            SpriteHandler.ClearAll();
        }

        public override void Update()
        {
            Ball.Update();
        }

        public override void Input()
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {
                Paddle.MoveLeft();
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                Paddle.MoveRight();
            }
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                Stop();
            }
            if (Keyboard.IsKeyDown(Key.Return))
            {
                Restart();
            }
        }

        void CreateBlocks()
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 22; i++)
                {
                    ConsoleColor c;
                    switch (Random.Next(0, 5))
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
                    var s = SpriteHandler.NewSprite(4 + i * 7, 3 + j * 4, 5, 2, '$');
                    s.SetColors(c, ConsoleColor.Black);
                    s.SetTag("block");
                    Render();
                }
            }
        }
    }



    class Paddle
    {
        Sprite LeftPart;
        Sprite MiddlePart;
        Sprite RightPart;
        
        public Paddle(int x, int y)
        {
            LeftPart = SpriteHandler.NewSprite("paddle-left", x, y,
                new string[] { "  /====", "/======" });
            MiddlePart = SpriteHandler.NewSprite("paddle-middle", x + 8, y,
                new string[] { "=======", "=======" });
            RightPart = SpriteHandler.NewSprite("paddle-right", x + 16, y,
                new string[] { "====\\  ", "======\\" });
        }

        void Move(int dir)
        {
            LeftPart.SetPosition(LeftPart.X + dir, LeftPart.Y);
            MiddlePart.SetPosition(LeftPart.X + dir + 8, LeftPart.Y);
            RightPart.SetPosition(LeftPart.X + dir + 16, LeftPart.Y);
        }

        public void MoveLeft()
        {
            Move(-1);
        }

        public void MoveRight()
        {
            Move(1);
        }
    }

    class Ball
    {
        Sprite Sprite;

        const double MIN_ANGLE = Math.PI / 12;
        const double MAX_ANGLE = 11 * Math.PI / 12;

        double Angle;
        double Speed = 0;

        DVector Pos;

        public Ball(int x, int y)
        {
            y--;
            Sprite = SpriteHandler.NewSprite("ball", x, y, new string[] { "/\\", "\\/" });
            Sprite.SetForeground(ConsoleColor.Red);
            Pos = new DVector(x, y);
        }

        public void Release()
        {
            Angle = BreakoutGame.Instance().Random.NextDouble() * (MAX_ANGLE - MIN_ANGLE) + MIN_ANGLE;
            Speed = 0.25;

            Messages.Log("Take off angle: " + Angle * 180 / Math.PI);
        }

        public void Update()
        {
            double dx = Math.Cos(-Angle) * Speed;
            double dy = Math.Sin(-Angle) * Speed;

            Speed *= 1.0005;

            Pos.Add(dx, dy);

            Sprite.SetPosition((int)Pos.X, (int)Pos.Y);

            var col = Sprite.AnyCollidingSide();
            if (col.IsColliding())
            {
                Pos.Subtract(dx, dy);
                col.CancelOpposing();
                Messages.Log("Collision: " + col.ToString());

                BreakoutGame.Instance().PlayNote((int) (BreakoutGame.Instance().Random.NextDouble() * 3) + 6);

                if (col.Right() || col.Left())
                {
                    Angle += Math.PI / 2;
                    Angle *= -1;
                    Angle -= Math.PI / 2;
                }

                if (col.Up() || col.Down())
                {
                    Angle *= -1;
                }

                if (col.Down())
                {
                    if (col.IsColliding("bottom-border"))
                    {
                        // die
                    }
                    else if (col.IsColliding("paddle-left"))
                    {
                        Angle += Math.PI / 6;
                    }
                    else if (col.IsColliding("paddle-right"))
                    {
                        Angle -= Math.PI / 6;
                    }
                }

                foreach (string name in col.GetSpriteList())
                {
                    var s = SpriteHandler.Get(name);
                    if (s.GetTag() == "block")
                    {
                        //s.SetColors(ConsoleColor.DarkGray, ConsoleColor.Black);
                        //s.SetCollide(false);
                        //SpriteHandler.UpdateAll();
                        SpriteHandler.TrashSprite(name);
                        Speed *= 1.05;
                    }
                }

                dx = Math.Cos(-Angle) * Speed;
                dy = Math.Sin(-Angle) * Speed;
                Pos.Add(dx, dy);
            }

            Messages.Log(dx + ", " + dy);

            Sprite.SetPosition((int)Pos.X, (int)Pos.Y);
        }
    }
}
