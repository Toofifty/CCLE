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
        public static int gameWidth;
        public static int gameHeight;

        public static string title = "CCLE";
        public static string version = "v0.0.2a";

        public static Renderer renderer;
        static Thread rendererThread;
        public static Pixel[] level;
        static List<string> consoleQueue = new List<string>();

        public static void WriteConsoleQueue()
        {
            Console.SetCursorPosition(0, gameHeight + 2);
            Console.ResetColor();

            if (consoleQueue.Count() > 0)
            {
                int c = consoleQueue.Count() - 1;
                do
                {
                    try
                    {
                        string s = consoleQueue[c];
                        Console.WriteLine(s + "                  ");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    c--;
                }
                while (c >= 0);

                while (consoleQueue.Count() > 6)
                {
                    consoleQueue.RemoveAt(0);
                }
            }
        }

        public static void QueueMessage(string message)
        {
            consoleQueue.Add(message);

            while (consoleQueue.Count() > 6)
            {
                consoleQueue.RemoveAt(0);
            }
        }

        public static void SetGameSize(int width, int height)
        {
            Console.SetWindowSize(width, height + 11);
            Console.SetBufferSize(width, height + 11);
            gameHeight = height;
            gameWidth = width;
        }

        public static void SetWindowTitle(string title)
        {
            Console.Title = title;
        }

        public static void Init()
        {
            Console.CursorVisible = false;
        }

        public static void Init(Pixel[] level)
        {
            Console.CursorVisible = false;
            renderer = new Renderer(level, 60);
            rendererThread = new Thread(renderer.RenderScene);
            rendererThread.Start();
        }

        public static void Init(Pixel[] level, int fps)
        {
            Console.CursorVisible = false;
            renderer = new Renderer(level, fps);
            rendererThread = new Thread(renderer.RenderScene);
            rendererThread.Start();
        }

        public static void StopRender()
        {
            renderer.Stop();
            rendererThread.Join();
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

        public bool IsEqualTo(Pixel pixel)
        {
            return (ch == pixel.ch && fColor == pixel.fColor && bColor == pixel.bColor && z == pixel.z);
        }

        public static Position PixelToPosition(Pixel o)
        {
            return PixelToPosition(o.pixelPosition);
        }

        public static Position PixelToPosition(int oP)
        {
            return new Position(oP % (Engine.gameWidth), (int)(oP / (Engine.gameWidth)));
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
        public volatile bool debugMode = false;
        volatile Pixel[] background;
        float frameRate;

        List<Position> dirtyPixels = new List<Position>();
        List<Sprite> renderSprites = new List<Sprite>();

        /// <summary>
        /// Initiate a new rendering loop.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        public Renderer(Pixel[] l, float r)
        {
            background = l;
            frameRate = r;
            isRunning = true;
        }

        /// <summary>
        /// Reset one pixel to the default state (as in the scene pixel array)
        /// </summary>
        /// <param name="pixel"></param>
        public void ClearPixel(Position pixel)
        {
            Console.SetCursorPosition(pixel.x, pixel.y + 1);
            Console.ResetColor();
            //Console.Write("-");
            Console.Write(background[pixel.ToPixel()].ch);
        }

        /// <summary>
        /// Update one sprite on the screen
        /// </summary>
        /// <param name="sprite"></param>
        public void DrawSprite(Sprite sprite)
        {
            foreach (BaseSprite bs in sprite.sprites)
            {
                if (Console.ForegroundColor != bs.symbol.fColor) Console.ForegroundColor = bs.symbol.fColor;
                if (Console.BackgroundColor != bs.symbol.bColor) Console.BackgroundColor = bs.symbol.bColor;
                Console.SetCursorPosition(bs.position.x, bs.position.y + 1);
                Console.Write(bs.symbol.ch);
            }
            sprite.dirty = false;
        }

        /// <summary>
        /// Draw a pixel array onto the screen
        /// </summary>
        /// <param name="scene"></param>
        public TimeSpan DrawScene(Pixel[] scene)
        {
            DateTime initialTime = DateTime.Now;
            Console.SetCursorPosition(0, 1);
            foreach (Pixel p in scene)
            {
                if (Console.BackgroundColor != p.bColor) Console.BackgroundColor = p.bColor;
                if (Console.ForegroundColor != p.fColor) Console.ForegroundColor = p.fColor;
                Console.Write(p.ch);
            }
            return (DateTime.Now - initialTime);
        }

        /// <summary>
        /// Main render loop, must be called on a new thread.
        /// </summary>
        public void RenderScene()
        {
            DateTime startTime = DateTime.Now;
            DateTime tempTime = DateTime.Now;
            int frameCount = 1;
            float CycleTime;
            List<float> CycleTimesList = new List<float>();
            List<Position> cleanPixels = new List<Position>();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(Engine.title + " " + Engine.version);
            DrawScene(background);

            while (isRunning)
            {
                // Clean dirty pixels
                int i = dirtyPixels.Count();
                for (int n = 0; n < i; n++)
                {
                    Position pixel = dirtyPixels[n];
                    ClearPixel(pixel);
                    cleanPixels.Add(pixel);
                }

                foreach (Position p in cleanPixels)
                {
                    dirtyPixels.Remove(p);
                }
                cleanPixels.Clear();

                // Draw dirty sprites
                foreach (Sprite sprite in renderSprites)
                {
                    if (sprite.dirty)
                    {
                        DrawSprite(sprite);
                    }
                }

                CycleTime = (DateTime.Now - tempTime).Milliseconds;

                // Write debug
                if (debugMode)
                {
                    CycleTimesList.Add(CycleTime);
                    float AvCycleTime = CycleTimesList.Average();
                    WriteDebugInfo(frameCount, AvCycleTime, CycleTime, startTime);
                    tempTime = DateTime.Now;
                }
                
                // Add to frame count
                frameCount += 1;
                Engine.WriteConsoleQueue();
                Thread.Sleep(10);
            }
        }

        void WriteDebugInfo(int frameCount, float AvCycleTime, float CycleTime, DateTime startTime)
        {
            try
            {
                Console.SetCursorPosition(0, 21);
                Console.ResetColor();
                Console.Write("Averages over " + frameCount + " frames: ");
                Console.Write("Cycle time: " + AvCycleTime.ToString("n2") + "ms ");
                Console.WriteLine("| Est.FPS: " + (1000.0 / AvCycleTime).ToString("n2"));
                Console.Write("Last frame: ");
                Console.Write("Cycle time: " + CycleTime.ToString("n2") + "ms ");
                Console.WriteLine("| True FPS: " + (frameCount / (DateTime.Now - startTime).Seconds).ToString("n2"));
            }
            catch (DivideByZeroException) { }
        }

        /// <summary>
        /// Refresh the screen and redraw the scene and sprites
        /// </summary>
        public void Refresh()
        {
            isRunning = false;
            Console.ResetColor();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(Engine.title + " " + Engine.version);
            DrawScene(background);
            Console.Beep(3767, 10);
            isRunning = true;
        }

        /// <summary>
        /// Mark a pixel as dirty and update it next frame.
        /// </summary>
        /// <param name="pos"></param>
        public void MarkDirty(Position pos)
        {
            dirtyPixels.Add(pos);
        }

        /// <summary>
        /// Mark a rectangle as dirty and update it next frame.
        /// </summary>
        /// <param name="rect"></param>
        public void MarkDirty(Rect rect)
        {
            foreach (Position p in rect.Positions())
            {
                dirtyPixels.Add(p);
            }
        }

        /// <summary>
        /// Add a sprite to the render queue.
        /// </summary>
        /// <param name="sprite"></param>
        public void AddSprite(Sprite sprite)
        {
            renderSprites.Add(sprite);
        }

        /// <summary>
        /// Remove a sprite from the render queue.
        /// </summary>
        /// <param name="sprite"></param>
        public void RemoveSprite(Sprite sprite)
        {
            renderSprites.Remove(sprite);
        }

        /// <summary>
        /// Stop the rendering loop in order to safely join the thread.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Set a new scene to be rendered.
        /// </summary>
        /// <param name="scene"></param>
        public void NewScene(Pixel[] scene)
        {
            background = scene;
        }
    }

    /// <summary>
    /// Represents a position on screen.
    /// </summary>
    class Position
    {
        public int x;
        public int y;

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

        public Position TempMoveX(int dx)
        {
            return new Position(x + dx, y);
        }

        public Position TempMoveY(int dy)
        {
            return new Position(x, y + dy);
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
            if (y < 0) y = 0;
            if (x < 0) x = 0;
            if (y > Engine.gameHeight - 1) y = Engine.gameHeight - 1;
            if (x > Engine.gameWidth - 2) x = Engine.gameWidth - 2;
        }

        public int ToPixel()
        {
            return y * (Engine.gameWidth + 1) + x;
        }

        public static int PositionToPixel(int x, int y)
        {
            return y * (Engine.gameWidth + 1) + x;
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

        static public Position operator +(Position p1, Position p2)
        {
            return new Position(p1.x + p2.x, p1.y + p2.y);
        }

        static public Position operator -(Position p1, Position p2)
        {
            return new Position(p1.x - p2.x, p1.y - p2.y);
        }

        static public double Distance(Position p1, Position p2)
        {
            //return (float)Math.Sqrt(((p1.x-p2.x)^2) + ((p1.y-p2.y)^2));
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2d) + Math.Pow(p1.y - p2.y, 2d));
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

        public Position[] Positions()
        {
            Position[] o = new Position[w * h];

            for (int i = 0; i < h; i++ )
            {
                for (int j = 0; j < w; j++)
                {
                    o[i * h + w] = new Position(topLeft.x + j, topLeft.y + i);
                }
            }
            return o;
        }
    }
}
