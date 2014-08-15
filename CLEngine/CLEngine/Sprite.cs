using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLEngine
{

    /// <summary>
    /// Create a sprite of one or more pixels
    /// </summary>
    class Sprite
    {
        static List<Sprite> globalSprites = new List<Sprite>();

        public List<Pixel> pixels = new List<Pixel>();
        public List<Pixel> colliders = new List<Pixel>();

        public Sprite lastCollision = null;

        public int[] dimensions;

        public bool dirty = true;

        public string tag = "null";

        public Sprite(char[] syms, ConsoleColor[] colors, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    pixels.Add(new Pixel(syms[i * rect.w + j], new Position(j + rect.topLeft.x, i + rect.topLeft.y), colors[i * rect.w + j]));
                }
            }

            foreach (Pixel p in pixels)
            {
                p.SetZ(depth);
                p.owner = this;
            }

            dimensions = new int[2]{rect.h, rect.w};
            AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public Sprite(char[] syms, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    pixels.Add(new Pixel(syms[i * rect.w + j], new Position(j + rect.topLeft.x, i + rect.topLeft.y)));
                }
            }

            foreach (Pixel p in pixels)
            {
                p.SetZ(1);
                p.owner = this;
            }

            dimensions = new int[2] { rect.h, rect.w };
            AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public Sprite(char sym, ConsoleColor c, Rect rect)
        {
            Pixel p = new Pixel(sym, rect.topLeft, c);
            p.SetZ(2);
            pixels.Add(p);
            dimensions = new int[2] { rect.h, rect.w };
            AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public bool MoveCollision(string dir, int am)
        {
            Position temp = null;
            foreach (Pixel p in pixels)
            {
                switch (dir)
                {
                    case "UP":
                        temp = p.cartesianPosition.TempMoveY(-am);
                        break;
                    case "DOWN":
                        temp = p.cartesianPosition.TempMoveY(am);
                        break;
                    case "LEFT":
                        temp = p.cartesianPosition.TempMoveX(-am);
                        break;
                    case "RIGHT":
                        temp = p.cartesianPosition.TempMoveX(am);
                        break;
                    default:
                        return false;
                }

                foreach (Pixel cs in colliders)
                {
                    if (temp.Equals(cs.cartesianPosition))
                    {
                        lastCollision = cs.owner;
                        return false;
                    }
                }
            }

            Move(dir, am);
            return true;
        }

        public void Move(string dir, int am)
        {
            foreach (Pixel p in pixels)
            {
                p.Move(dir, am);
            }
            dirty = true;
        }

        public void SetPosition(Position pos)
        {
            for (int i = 0; i < dimensions[1]; i++)
            {
                for (int j = 0; j < dimensions[0]; j++)
                {
                    pixels[i * dimensions[0] + j].cartesianPosition.SetX(pos.x + j);
                    pixels[i * dimensions[0] + j].cartesianPosition.SetY(pos.y + i);
                }
            }
            dirty = true;
        }

        public bool SetCollision(Position pos)
        {
            foreach (Pixel cs in colliders)
            {
                if (pos.Equals(cs.cartesianPosition))
                {
                    lastCollision = cs.owner;
                    return false;
                }
            }
            SetPosition(pos);
            return true;
        }

        public void SetZ(int z)
        {
            foreach (Pixel p in pixels)
            {
                p.SetZ(z);
            }
            dirty = true;
        }

        public Position GetPosition()
        {
            return pixels[0].cartesianPosition;
        }

        public void AddCollider(Sprite s)
        {
            foreach (Pixel p in s.pixels)
            {
                colliders.Add(p);
            }
        }

        public void RemoveCollider(Sprite s)
        {
            foreach (Pixel p in s.pixels)
            {
                colliders.Remove(p);
            }
        }
        
        public static void AddSprite(Sprite spr)
        {
            globalSprites.Add(spr);
        }

        public static void RemoveSprite(Sprite spr)
        {
            globalSprites.Remove(spr);
            foreach (Pixel p in spr.pixels)
            {
                Engine.renderer.MarkDirty(p.cartesianPosition);
            }
        }

        public static void ClearSprites()
        {
            List<Sprite> temp = new List<Sprite>();
            foreach (Sprite s in globalSprites)
            {
                temp.Add(s);
            }
            foreach (Sprite s in temp)
            {
                RemoveSprite(s);
            }
        }

        public static List<Sprite> GetSprites()
        {
            return globalSprites;
        }
    }
}
