using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{
    class SpriteHandler
    {
        static Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
        static Dictionary<string, Sprite> collidableDict = new Dictionary<string, Sprite>();

        static List<Sprite> sprites = new List<Sprite>();
        static List<Sprite> collidables = new List<Sprite>();

        static List<Sprite> trashed = new List<Sprite>();

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
            return sprites;
        }

        public static List<Sprite> GetTrashed()
        {
            return trashed;
        }

        public static List<Sprite> GetCollidables(int z)
        {
            return collidables.FindAll(x => x.GetZ() == z).ToList();
        }

        public static Sprite Get(string name)
        {
            return spriteDict[name];
        }

        static Sprite AddSprite(string name, Sprite s)
        {
            spriteDict.Add(name, s);
            UpdateAllSpriteList();
            if (s.CanCollide())
            {
                collidableDict.Add(name, s);
                UpdateCollidableList();
            }
            return s;

        }

        public static void TrashSprite(string name)
        {
            trashed.Add(Get(name));
            spriteDict.Remove(name);
            UpdateAllSpriteList();
            if (collidableDict.ContainsKey(name))
            {
                collidableDict.Remove(name);
                UpdateCollidableList();
            }
        }

        public static void UpdateAll()
        {
            UpdateAllSpriteList();
            UpdateCollidableList();
        }

        static void UpdateAllSpriteList()
        {
            sprites.Clear();
            foreach (string key in spriteDict.Keys)
                sprites.Add(spriteDict[key]);
            sprites = sprites.OrderBy(x => x.GetZ()).ToList();
        }

        static void UpdateCollidableList()
        {
            collidables.Clear();
            foreach (string key in collidableDict.Keys)
                collidables.Add(collidableDict[key]);
            collidables = collidables.OrderBy(x => x.GetZ()).ToList();
        }
    }

    class Sprite : Rect
    {
        string[] texture;

        ConsoleColor mainForeground = ConsoleColor.White;
        ConsoleColor mainBackground = ConsoleColor.Black;
        ConsoleColor[,] complexForeground = null;
        ConsoleColor[,] complexBackground = null; 

        Rect previousRect = new Rect(0, 0, 0, 0);
        string name;

        bool collide = true;
        Collision collision;

        // determines whether spaces can collide with
        // other sprites
        // if true, the collide will be a rectangle
        bool spaceCollide = true;

        bool test { set; get; } = true;

        bool trash = false;

        int z = 0;

        public Sprite(int x, int y, string[] texture)
            : base(x, y, 0, 0)
        {
            SetTexture(texture);
            previousRect = new Rect(x, y, w, h);
            collision = new Collision(this);
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

        public void SetName(string name)
        {
            this.name = name;
        }

        public void DrawTo(Window window)
        {
            window.DrawBlock(x, y, texture, mainForeground, mainBackground);
        }

        public void SetTexture(string[] texture)
        {
            this.texture = texture;
            SetSize(texture[0].Length > 0 ? texture[0].Length : 0, texture.Length);
        }

        public string[] GetTexture()
        {
            return texture;
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
            this.mainForeground = foreground;
            Redraw();
        }

        public void SetForeground(ConsoleColor[,] foreground)
        {
            if (foreground.GetLength(0) == h && foreground.GetLength(1) == w)
            {
                this.complexForeground = foreground;
            }
            else throw new BadBlockException();
            Redraw();
        }

        public void SetBackground(ConsoleColor background)
        {
            this.mainBackground = background;
            Redraw();
        }

        public void SetBackground(ConsoleColor[,] background)
        {
            if (background.GetLength(0) == h && background.GetLength(1) == w)
            {
                this.complexBackground = background;
            }
            else throw new BadBlockException();
            Redraw();
        }

        public void SetSpaceCollide(bool value)
        {
            spaceCollide = value;
        }

        public bool CanCollide()
        {
            return collide;
        }

        public void SetCollide(bool collide)
        {
            this.collide = collide;
        }

        public void Redraw()
        {
            Renderer.UpdateSprite(this);
        }

        public int GetZ()
        {
            return z;
        }

        public void SetZ(int z)
        {
            this.z = z;
            SpriteHandler.UpdateAll();
            Redraw();
        }

        public Rect GetPreviousRect()
        {
            return previousRect;
        }

        public void FillScreenRect(Rect r, char[,] texture, ConsoleColor[,] foreground, ConsoleColor[,] background)
        {
            if (r.x + r.w > x && r.y + r.h > y && r.x < x + w && r.y < y + h)
            {
                var offsetX = r.x - x;
                var offsetY = r.y - y;

                for (var j = 0; j < texture.GetLength(Util.HEIGHT); j++) // height
                {
                    for (var i = 0; i < texture.GetLength(Util.WIDTH); i++) // width
                    {
                        if (InBounds(r.x + i, r.y + j))
                        {
                            try
                            {
                                texture[j, i] = this.texture[offsetY + j][offsetX + i];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                texture[j, i] = '!';
                            }
                            foreground[j, i] = this.mainForeground;
                            background[j, i] = this.mainBackground;
                        }
                    }
                }
            }
        }

        public override void SetPosition(int x, int y)
        {
            if (previousRect != null) previousRect = new Rect(this);
            base.SetPosition(x, y);
            Redraw();
        }

        public override void SetSize(int w, int h)
        {
            if (previousRect != null) previousRect = new Rect(this);
            base.SetSize(w, h);
            Redraw();
        }

        public bool CollidingWith(Rect r)
        {
            return collide && CollidingSide(r) > 0;
        }

        public int CollidingSide(Rect r)
        {
            if (!collide || (r.x + r.w <= x || r.y + r.h <= y || r.x >= x + w || r.y >= y + h))
                return 0;

            var sides = 0;

            // top
            for (var i = 0; i < w; i++)
            {
                if (r.InBounds(x + i, y))
                {
                    sides |= 8;
                    break;
                }
            }

            // right
            for (var i = 0; i < h; i++)
            {
                if (r.InBounds(x + w - 1, y + i))
                {
                    sides |= 4;
                    break;
                }
            }

            // bottom
            for (var i = 0; i < w; i++)
            {
                if (r.InBounds(x + i, y + h - 1))
                {
                    sides |= 2;
                    break;
                }
            }

            // left
            for (var i = 0; i < h; i++)
            {
                if (r.InBounds(x, y + i))
                {
                    sides |= 1;
                    break;
                }
            }

            return sides > 0 ? sides : 15;
        }

        public Collision AnyCollidingSide()
        {
            collision.Reset();
            foreach (Sprite sprite in SpriteHandler.GetCollidables(z))
            {
                if (sprite == this) continue;
                var sides = CollidingSide(sprite);
                if (sides > 0)
                {
                    collision.AddSides(sides);
                    collision.AddSprite(sprite.GetName());
                }
            }
            return collision;
        }

        public Collision GetCollision()
        {
            return collision;
        }

        public void Trash()
        {
            trash = true;
        }

        public bool IsTrash()
        {
            return trash;
        }

        public string GetName()
        {
            return name;
        }

        public override string ToString()
        {
            return "S:" + name + base.ToString();
        }
    }

    class Collision
    {
        List<string> collidingSprites;
        int collidingSides = 0;

        Sprite owner;

        public Collision(Sprite owner)
        {
            this.owner = owner;
            collidingSprites = new List<string>();
        }

        public bool IsColliding()
        {
            return collidingSides > 0;
        }

        public bool IsColliding(string name)
        {
            return IsColliding() && collidingSprites.Contains(name);
        }

        public void Reset()
        {
            collidingSprites.Clear();
            collidingSides = 0;
        }

        public void AddSprite(string name)
        {
            collidingSprites.Add(name);
        }

        public void AddSides(int sides)
        {
            collidingSides |= sides;
        }

        public void KeepSides(int sides)
        {
            collidingSides &= sides;
        }

        public void CancelOpposing()
        {
            if (collidingSides == 15) return;

            if (Up() && Down()) KeepSides(5);
            if (Left() && Right()) KeepSides(10);
        }

        public bool Up()
        {
            return (collidingSides & 8) == 8;
        }
        
        public bool Right()
        {
            return (collidingSides & 4) == 4;
        }

        public bool Down()
        {
            return (collidingSides & 2) == 2;
        }

        public bool Left()
        {
            return (collidingSides & 1) == 1;
        }

        public string[] GetObjects()
        {
            return collidingSprites.ToArray();
        }

        public override string ToString()
        {
            if (IsColliding())
            {
                return (Up() ? "U" : "") + (Right() ? "R" : "") 
                    + (Down() ? "D" : "") + (Left() ? "L" : "")
                    + " : " + string.Join(",", collidingSprites.ToArray());
            }
            else
            {
                return "NONE";
            }
        }
    }
}
