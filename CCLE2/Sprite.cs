using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{
    class SpriteHandler
    {
        static Dictionary<string, Sprite> SpriteDict = new Dictionary<string, Sprite>();
        static Dictionary<string, Sprite> CollidableDict = new Dictionary<string, Sprite>();

        static List<Sprite> Sprites = new List<Sprite>();
        static List<Sprite> Collidables = new List<Sprite>();

        static List<Sprite> Trashed = new List<Sprite>();

        public static Sprite NewSprite(string name, int x, int y, int w, int h, char ch)
        {
            var sprite = Sprite.SingleTextureSprite(x, y, w, h, ch);
            sprite.SetName(name);
            return AddSprite(name, sprite);
        }

        public static Sprite NewSprite(int x, int y, int w, int h, char ch)
        {
            return NewSprite(Guid.NewGuid().ToString(), x, y, w, h, ch);
        }

        public static Sprite NewSprite(string name, int x, int y, string[] texture)
        {
            var sprite = new Sprite(x, y, texture);
            sprite.SetName(name);
            return AddSprite(name, sprite);
        }

        public static Sprite NewSprite(int x, int y, string[] texture)
        {
            return NewSprite(Guid.NewGuid().ToString(), x, y, texture);
        }

        public static List<Sprite> GetSprites()
        {
            return Sprites;
        }

        public static List<Sprite> GetTrashed()
        {
            return Trashed;
        }

        public static List<Sprite> GetCollidables(int z)
        {
            return Collidables.FindAll(x => x.GetZ() == z).ToList();
        }

        public static Sprite Get(string name)
        {
            return SpriteDict[name];
        }

        static Sprite AddSprite(string name, Sprite s)
        {
            SpriteDict.Add(name, s);
            UpdateAllSpriteList();
            if (s.CanCollide())
            {
                CollidableDict.Add(name, s);
                UpdateCollidableList();
            }
            return s;

        }

        public static void TrashSprite(string name)
        {
            Trashed.Add(Get(name));
            SpriteDict.Remove(name);
            UpdateAllSpriteList();
            if (CollidableDict.ContainsKey(name))
            {
                CollidableDict.Remove(name);
                UpdateCollidableList();
            }
        }

        static void TrashSpriteLite(string name)
        {
            Trashed.Add(Get(name));
            SpriteDict.Remove(name);
            if (CollidableDict.ContainsKey(name))
            {
                CollidableDict.Remove(name);
            }
        }

        public static void UpdateAll()
        {
            UpdateAllSpriteList();
            UpdateCollidableList();
        }

        static void UpdateAllSpriteList()
        {
            Sprites.Clear();
            foreach (string key in SpriteDict.Keys)
                Sprites.Add(SpriteDict[key]);
            Sprites = Sprites.OrderBy(x => x.GetZ()).ToList();
        }

        static void UpdateCollidableList()
        {
            Collidables.Clear();
            foreach (string key in CollidableDict.Keys)
                Collidables.Add(CollidableDict[key]);
            Collidables = Collidables.OrderBy(x => x.GetZ()).ToList();
        }

        public static void ClearAll()
        {
            foreach (string key in SpriteDict.Keys.ToArray())
            {
                TrashSpriteLite(key);
            }
            UpdateAll();
        }
    }

    class Sprite : Rect
    {
        string[] Texture;

        ConsoleColor Foreground = ConsoleColor.White;
        ConsoleColor Background = ConsoleColor.Black;
        ConsoleColor[,] ComplexForeground = null;
        ConsoleColor[,] ComplexBackground = null; 

        Rect PreviousRect = new Rect(0, 0, 0, 0);
        string Name;
        string Tag;

        bool Collide = true;
        Collision Collision;

        // determines whether spaces can collide with
        // other sprites
        // if true, the collide will be a rectangle
        bool SpaceCollide = true;

        int Z = 0;

        public Sprite(int x, int y, string[] texture)
            : base(x, y, 0, 0)
        {
            SetTexture(texture);
            PreviousRect = new Rect(x, y, W, H);
            Collision = new Collision(this);
            Renderer.UpdateSprite(this);
        }

        public static Sprite SingleTextureSprite(int x, int y, int w, int h, char ch)
        {
            var texture = new List<string>();
            for (var j = 0; j < h; j++)
            {
                texture.Add("");
                for (var i = 0; i < w; i++)
                {
                    texture[j] += ch;
                }
            }
            return new Sprite(x, y, texture.ToArray());
        }

        public void DrawTo(Window window)
        {
            window.DrawBlock(X, Y, Texture, Foreground, Background);
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public string GetName()
        {
            return Name;
        }

        public void SetTag(string tag)
        {
            Tag = tag;
        }

        public string GetTag()
        {
            return Tag;
        }

        public void SetTexture(string[] texture)
        {
            Texture = texture;
            SetSize(texture[0].Length > 0 ? texture[0].Length : 0, texture.Length);
        }

        public string[] GetTexture()
        {
            return Texture;
        }

        public void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            SetForeground(foreground);
            SetBackground(background);
        }

        public void SetColors(ConsoleColor[,] foreground, ConsoleColor[,] background)
        {
            SetForeground(foreground);
            SetBackground(background);
        }

        public void SetForeground(ConsoleColor foreground)
        {
            Foreground = foreground;
            Redraw();
        }

        public void SetForeground(ConsoleColor[,] foreground)
        {
            if (foreground.GetLength(0) == H && foreground.GetLength(1) == W)
            {
                ComplexForeground = foreground;
            }
            else throw new BadBlockException();
            Redraw();
        }

        public void SetBackground(ConsoleColor background)
        {
            Background = background;
            Redraw();
        }

        public void SetBackground(ConsoleColor[,] background)
        {
            if (background.GetLength(0) == H && background.GetLength(1) == W)
            {
                ComplexBackground = background;
            }
            else throw new BadBlockException();
            Redraw();
        }

        public void SetSpaceCollide(bool value)
        {
            SpaceCollide = value;
        }

        public void SetCollide(bool collide)
        {
            Collide = collide;
        }

        public bool CanCollide()
        {
            return Collide;
        }

        public void Redraw()
        {
            Renderer.UpdateSprite(this);
        }

        public void SetZ(int z)
        {
            this.Z = z;
            SpriteHandler.UpdateAll();
            Redraw();
        }

        public int GetZ()
        {
            return Z;
        }
        
        public Rect GetPreviousRect()
        {
            return PreviousRect;
        }

        public override void SetPosition(int x, int y)
        {
            if (PreviousRect != null) PreviousRect = new Rect(this);
            base.SetPosition(x, y);
            Redraw();
        }

        public override void SetSize(int w, int h)
        {
            if (PreviousRect != null) PreviousRect = new Rect(this);
            base.SetSize(w, h);
            Redraw();
        }

        public void FillScreenRect(Rect r, char[,] texture, ConsoleColor[,] foreground, ConsoleColor[,] background)
        {
            if (r.X + r.W > X && r.Y + r.H > Y && r.X < X + W && r.Y < Y + H)
            {
                var offsetX = r.X - X;
                var offsetY = r.Y - Y;

                for (var j = 0; j < texture.GetLength(Util.HEIGHT); j++) // height
                {
                    for (var i = 0; i < texture.GetLength(Util.WIDTH); i++) // width
                    {
                        if (InBounds(r.X + i, r.Y + j))
                        {
                            try
                            {
                                texture[j, i] = this.Texture[offsetY + j][offsetX + i];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                texture[j, i] = '!';
                            }
                            foreground[j, i] = this.Foreground;
                            background[j, i] = this.Background;
                        }
                    }
                }
            }
        }

        public int CollidingSide(Rect r)
        {
            if (!Collide || (r.X + r.W <= X || r.Y + r.H <= Y || r.X >= X + W || r.Y >= Y + H))
                return 0;

            var sides = 0;

            // top
            for (var i = 0; i < W; i++)
            {
                if (r.InBounds(X + i, Y))
                {
                    sides |= 8;
                    break;
                }
            }

            // right
            for (var i = 0; i < H; i++)
            {
                if (r.InBounds(X + W - 1, Y + i))
                {
                    sides |= 4;
                    break;
                }
            }

            // bottom
            for (var i = 0; i < W; i++)
            {
                if (r.InBounds(X + i, Y + H - 1))
                {
                    sides |= 2;
                    break;
                }
            }

            // left
            for (var i = 0; i < H; i++)
            {
                if (r.InBounds(X, Y + i))
                {
                    sides |= 1;
                    break;
                }
            }

            return sides > 0 ? sides : 15;
        }

        public Collision AnyCollidingSide()
        {
            Collision.Reset();
            foreach (Sprite sprite in SpriteHandler.GetCollidables(Z))
            {
                if (sprite == this) continue;
                var sides = CollidingSide(sprite);
                if (sides > 0)
                {
                    Collision.AddSides(sides);
                    Collision.AddSprite(sprite.GetName());
                }
            }
            return Collision;
        }

        public override string ToString()
        {
            return "S:" + Name + base.ToString();
        }
    }

    class Collision
    {
        List<string> Sprites;
        int Sides = 0;

        Sprite owner;

        public Collision(Sprite owner)
        {
            this.owner = owner;
            Sprites = new List<string>();
        }

        public bool IsColliding()
        {
            return Sides > 0;
        }

        public bool IsColliding(string name)
        {
            return IsColliding() && Sprites.Contains(name);
        }

        public void Reset()
        {
            Sprites.Clear();
            Sides = 0;
        }

        public void AddSprite(string name)
        {
            Sprites.Add(name);
        }

        public void AddSides(int sides)
        {
            Sides |= sides;
        }

        public void KeepSides(int sides)
        {
            Sides &= sides;
        }

        public void CancelOpposing()
        {
            if (Sides == 15) return;

            if (Up() && Down()) KeepSides(5);
            if (Left() && Right()) KeepSides(10);
        }

        public bool Up()
        {
            return (Sides & 8) == 8;
        }
        
        public bool Right()
        {
            return (Sides & 4) == 4;
        }

        public bool Down()
        {
            return (Sides & 2) == 2;
        }

        public bool Left()
        {
            return (Sides & 1) == 1;
        }

        public string[] GetSpriteList()
        {
            return Sprites.ToArray();
        }

        public override string ToString()
        {
            if (IsColliding())
            {
                return (Up() ? "U" : "") + (Right() ? "R" : "") 
                    + (Down() ? "D" : "") + (Left() ? "L" : "")
                    + " : " + string.Join(",", Sprites.ToArray());
            }
            else
            {
                return "NONE";
            }
        }
    }
}
