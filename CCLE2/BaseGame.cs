using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CCLE.Game
{
    class BaseGame
    {
        protected Window Window;
        public Random Random = new Random();
        bool GameRunning = true;
        bool Restarting = false;
        int FPS = 60;
        int TPS = 60;

        protected BaseGame(string windowTitle, Window window)
        {
            Window = window;
            Window.SetTitle(windowTitle);
            Window.Init();
            Renderer.Init(Window);
        }

        public void SetFPS(int fps)
        {
            FPS = fps;
        }

        public void SetTPS(int tps)
        {
            TPS = tps;
        }

        public virtual void OnInit()
        {
            throw new NotImplementedException();
        }

        public virtual void OnExit()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            OnInit();
            Thread renderThread = new Thread(RenderLoop);
            renderThread.Start();
            while (GameRunning)
            {
                Update();
                Input();
                Sleep(1000.0 / TPS);
            }
            renderThread.Join();
            OnExit();

            Render();

            if (Restarting)
            {
                GameRunning = true;
                WaitForKey(Key.Space);
                Run();
            }
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void Input()
        {
            throw new NotImplementedException();
        }

        void RenderLoop()
        {
            while (GameRunning)
            {
                Render();
                Sleep(1000.0 / FPS);
            }
        }

        protected void Render()
        {
            Renderer.Draw();
        }

        public void Stop()
        {
            GameRunning = false;
        }

        public void Restart()
        {
            Restarting = true;
            Stop();
        }

        public void WaitForKey(Key key)
        {
            while (!Keyboard.IsKeyDown(key))
            {
                Sleep(1000.0 / TPS);
            }
        }

        public void Sleep(double ms)
        {
            Thread.Sleep((int) ms);
        }

        public void PlayNote(int note)
        {
            var actualNote = MusicBeeper.Tones.Note.A;
            switch (note)
            {
                case 2:
                    actualNote = MusicBeeper.Tones.Note.As;
                    break;
                case 3:
                    actualNote = MusicBeeper.Tones.Note.B;
                    break;
                case 4:
                    actualNote = MusicBeeper.Tones.Note.C;
                    break;
                case 5:
                    actualNote = MusicBeeper.Tones.Note.Cs;
                    break;
                case 6:
                    actualNote = MusicBeeper.Tones.Note.D;
                    break;
                case 7:
                    actualNote = MusicBeeper.Tones.Note.Ds;
                    break;
                case 8:
                    actualNote = MusicBeeper.Tones.Note.E;
                    break;
                case 9:
                    actualNote = MusicBeeper.Tones.Note.F;
                    break;
                case 10:
                    actualNote = MusicBeeper.Tones.Note.Fs;
                    break;
                case 11:
                    actualNote = MusicBeeper.Tones.Note.G;
                    break;
                case 12:
                    actualNote = MusicBeeper.Tones.Note.Gs;
                    break;
            }
            var lel = new Thread(() => MusicBeeper.Tones.PlayNote(actualNote));
            lel.Start();
        }
    }
}
