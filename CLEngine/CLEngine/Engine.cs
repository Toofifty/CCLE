using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLEngine
{
    class Engine
    {
        public static Renderer renderer;
        public static Pixel[] level;
        public static Pixel[] activeScene;
        static List<string> consoleQueue = new List<string>();

        public static TimeSpan DrawScene(Pixel[] scene)
        {
            DateTime initialTime = DateTime.Now;
            Console.SetCursorPosition(0, 1);
            foreach (Pixel o in scene)
            {
                if (Console.BackgroundColor != o.bColor) Console.BackgroundColor = o.bColor;
                if (Console.ForegroundColor != o.fColor) Console.ForegroundColor = o.fColor;
                Console.Write(o.ch);
            }
            return (DateTime.Now - initialTime);
        }

        public static TimeSpan UpdatePixel(Pixel[] scene, int x, int y)
        {
            DateTime initialTime = DateTime.Now;

            Console.SetCursorPosition(x, y + 1);
            Pixel current = scene[Position.PositionToPixel(x, y)];
            if (current.ch == '.')
            {
                int result = new Random().Next(5);
                switch (result)
                {
                    case 0:
                        current.fColor = ConsoleColor.Yellow;
                        break;
                    case 1:
                        current.fColor = ConsoleColor.Green;
                        break;
                    case 2:
                        current.fColor = ConsoleColor.Magenta;
                        break;
                    case 3:
                        current.fColor = ConsoleColor.Red;
                        break;
                    case 4:
                        current.fColor = ConsoleColor.Black;
                        break;
                }
            }
            if (Console.BackgroundColor != current.bColor) Console.BackgroundColor = current.bColor;
            if (Console.ForegroundColor != current.fColor) Console.ForegroundColor = current.fColor;
            Console.Write(current.ch);
            Console.SetCursorPosition(13, 0);

            return (DateTime.Now - initialTime);
        }

        public static void CleanDirtyPixel(Sprite s)
        {
            int oldOrdinate = s.oldPosition.ToPixel();
            activeScene[oldOrdinate] = level[oldOrdinate];
            renderer.UpdateScene(activeScene, s.oldPosition);
            if (s.position.Equals(new Position(1, 1)))
            {
                QueueMessage("Suspect: " + s.symbol.ch.ToString());
            }
        }

        public static void UpdateNewPixel(Sprite s)
        {
            int ordinate = s.position.ToPixel();
            if (s.symbol.z >= activeScene[ordinate].z)
            {
                activeScene[ordinate] = s.symbol;
                renderer.UpdateScene(activeScene, s.position);
                if (s.position.Equals(new Position(1, 1)))
                {
                    QueueMessage("Suspect: " + s.symbol.ch.ToString());
                }
            }
        }

        public static void UpdateSpriteInScene(Sprite s)
        {
            CleanDirtyPixel(s);
            UpdateNewPixel(s);
        }

        public static void UpdateSpriteInScene(LargeSprite s)
        {
            foreach (Sprite ss in s.sprites)
            {
                CleanDirtyPixel(ss);
            }
            foreach (Sprite ss in s.sprites)
            {
                UpdateNewPixel(ss);
            }
        }

        public static void WriteConsoleQueue()
        {
            Console.SetCursorPosition(0, 23);
            Console.Write("                   ");
            Console.SetCursorPosition(0, 23);

            foreach (string s in consoleQueue)
            {
                Console.Write(s);
            }
            consoleQueue.Clear();
        }

        public static void QueueMessage(string message)
        {
            consoleQueue.Add(message);
        }

        public static void QueuePixelUpdate(Pixel[] scene, Position pos)
        {
            renderer.UpdateScene(scene, pos);
        }

        public static void UpdateAllSprites()
        {
            foreach (Sprite s in Sprite.GetSprites())
            {
                UpdateSpriteInScene(s);
            }

            foreach (LargeSprite ls in Sprite.GetLargeSprites())
            {
                UpdateSpriteInScene(ls);
            }
        }
    }

    /// <summary>
    /// Information for one character on screen.
    /// </summary>
    class Pixel
    {
        public char ch;
        public ConsoleColor fColor;
        public ConsoleColor bColor;
        public int pixelPosition;
        public Position cartesianPosition;
        public int z;

        public Pixel(char c, int pP)
        {
            pixelPosition = pP;
            cartesianPosition = PixelToPosition(pP);
            ch = c;
            fColor = ConsoleColor.White;
            bColor = ConsoleColor.Black;
            z = 0;
        }

        public Pixel(char c, int pP, ConsoleColor foreground, ConsoleColor background)
        {
            pixelPosition = pP;
            cartesianPosition = PixelToPosition(pP);
            ch = c;
            fColor = foreground;
            bColor = background;
            z = 0;
        }

        public Pixel(char c, Position p)
        {
            pixelPosition = p.ToPixel();
            cartesianPosition = p;
            ch = c;
            fColor = ConsoleColor.White;
            bColor = ConsoleColor.Black;
            z = 0;
        }

        public Pixel(char c, Position p, ConsoleColor foreground, ConsoleColor background)
        {
            pixelPosition = p.ToPixel();
            cartesianPosition = p;
            ch = c;
            fColor = foreground;
            bColor = background;
            z = 0;
        }

        public void SetZ(int nz)
        {
            z = nz;
        }

        public static Position PixelToPosition(Pixel o)
        {
            return PixelToPosition(o.pixelPosition);
        }

        public static Position PixelToPosition(int oP)
        {
            return new Position(oP % 79, (int)(oP / 79));
        }

        public static Pixel[] CharArrayToPixelArray(char[] inArray)
        {

            List<Pixel> pList = new List<Pixel>();
            int count = 0;
            foreach (char c in inArray)
            {
                pList.Add(new Pixel(c, count));
                count++;
            }

            Pixel[] outArray = pList.ToArray();
            return outArray;
        }

        public static Pixel[] LoadTextLevel(string levelFile)
        {
            string filePath = Environment.CurrentDirectory + @"\..\..\..\..\Levels\" + levelFile + ".cll";
            try
            {
                return CharArrayToPixelArray(System.IO.File.ReadAllText(filePath).ToCharArray());
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to load file from path: " + filePath);
                Thread.Sleep(10000);
                Environment.Exit(0);
                return null;
            }
        }
    }

    /// <summary>
    /// Main rendering loop.
    /// </summary>
    class Renderer
    {
        public volatile bool isRunning;
        public volatile bool hasUpdate;
        public volatile Pixel[] charScene;
        public float frameRate;

        public List<Position> dirtyPositions = new List<Position>();

        public Renderer(Pixel[] l, float r)
        {
            charScene = l;
            frameRate = r;
            hasUpdate = true;
            isRunning = true;
        }

        public void RenderScene()
        {
            DateTime tempTime = DateTime.Now;
            DateTime startTime = DateTime.Now;
            int frameCount = 1;
            List<float> CycleTimesList = new List<float>();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("CCLE v0.0.1a");
            Engine.DrawScene(charScene);

            while (isRunning)
            {
                float CycleTime = (DateTime.Now - tempTime).Milliseconds;
                if (dirtyPositions.Count > 0)
                {
                    int c = 0;
                    do
                    {
                        Position dp = dirtyPositions[c];
                        Engine.UpdatePixel(charScene, dp.x, dp.y);
                        dirtyPositions.RemoveAt(c);
                        c++;
                    }
                    while (c < dirtyPositions.Count());

                    try
                    {
                        CycleTime = (DateTime.Now - tempTime).Milliseconds;
                        CycleTimesList.Add(CycleTime);

                        float AvCycleTime = CycleTimesList.Average();

                        Console.SetCursorPosition(0, 21);

                        Console.Write("Averages over " + frameCount.ToString() + " frames: ");
                        Console.Write("Cycle time: " + AvCycleTime.ToString("n2") + "ms ");
                        Console.WriteLine("| Est.FPS: " + (1000.0 / AvCycleTime).ToString("n2"));
                        Console.Write("Last frame: ");
                        Console.Write("Cycle time: " + CycleTime.ToString("n2") + "ms ");
                        Console.WriteLine("| True FPS: " + (frameCount / (DateTime.Now - startTime).Seconds).ToString("n2"));

                        frameCount += 1;
                    }
                    catch (DivideByZeroException)
                    {
                        continue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Engine.WriteConsoleQueue();
                    Console.SetCursorPosition(12, 0);
                    hasUpdate = false;
                }
                tempTime = DateTime.Now;
            }
        }

        public void UpdateScene(Pixel[] newScene)
        {
            charScene = newScene;
            hasUpdate = true;
        }

        public void UpdateScene(Pixel[] newScene, int x, int y)
        {
            charScene = newScene;
            dirtyPositions.Add(new Position(x, y));
            hasUpdate = true;
        }

        public void UpdateScene(Pixel[] newScene, Position pos)
        {
            //Engine.QueueMessage(pos.ToString() + ",");
            charScene = newScene;
            dirtyPositions.Add(pos);
            hasUpdate = true;
        }

        public void Stop()
        {
            isRunning = false;
        }
    }

    /// <summary>
    /// Represents a position on screen.
    /// </summary>
    class Position
    {
        public int x;
        public int y;

        public int[] xBounds = new int[2] { 1, 77 };
        public int[] yBounds = new int[2] { 1, 18 };

        public Position(int nx, int ny)
        {
            x = nx;
            y = ny;
            CheckBounds();
        }

        public void MoveX(int dx)
        {
            x += dx;
            CheckBounds();
        }

        public void MoveY(int dy)
        {
            y += dy;
            CheckBounds();
        }

        public void SetX(int nx)
        {
            x = nx;
            CheckBounds();
        }

        public void SetY(int ny)
        {
            y = ny;
            CheckBounds();
        }

        public void CheckBounds()
        {
            if (y < yBounds[0]) y = yBounds[0];
            if (y > yBounds[1]) y = yBounds[1];
            if (x < xBounds[0]) x = xBounds[0];
            if (x > xBounds[1]) x = xBounds[1];
        }

        public int ToPixel()
        {
            return y * 81 + x;
        }

        public static int PositionToPixel(int x, int y)
        {
            return y * 81 + x;
        }

        public Position Clone()
        {
            return new Position(x, y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public bool Equals(Position pos)
        {
            return AreEqual(this, pos);
        }

        public static bool AreEqual(Position pos1, Position pos2)
        {
            return (pos1.x == pos2.x && pos1.y == pos2.y);
        }
    }

    /// <summary>
    /// Represents position and dimensions of a rectangle.
    /// </summary>
    class Rect
    {
        public Position topLeft;
        public int w;
        public int h;

        public Rect(int x, int y, int width, int height)
        {
            topLeft = new Position(x, y);
            w = width;
            h = height;

        }
    }
}
