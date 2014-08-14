using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLEngine
{
    /// <summary>
    /// Basic (1x1) sprite.
    /// </summary>
    class BaseSprite
    {
        static List<Sprite> spriteList = new List<Sprite>();

        public List<BaseSprite> colliders = new List<BaseSprite>();
        public Position oldPosition = new Position(0, 0);
        public Position position = new Position(0, 0);
        public Pixel symbol;

        public Sprite owner;

        public BaseSprite(char sym, Position pos, Sprite o)
        {
            position = pos;
            symbol = new Pixel(sym, position, ConsoleColor.Cyan, ConsoleColor.Black);
            owner = o;
        }

        public BaseSprite(char sym, Position pos, ConsoleColor f, Sprite o)
        {
            position = pos;
            symbol = new Pixel(sym, position, f, ConsoleColor.Black);
            owner = o;
        }

        public BaseSprite(char sym, int x, int y, bool addSprite, ConsoleColor foreGround, Sprite o)
        {
            position.SetX(x);
            position.SetY(y);
            symbol = new Pixel(sym, position, foreGround, ConsoleColor.Black);
            owner = o;
        }

        public BaseSprite(char sym, int x, int y, bool addSprite, Sprite o)
        {
            position.SetX(x);
            position.SetY(y);
            symbol = new Pixel(sym, position, ConsoleColor.Cyan, ConsoleColor.Black);
            owner = o;
        }

        public bool MoveCollision(string dir, int am)
        {
            Position temp = null;
            switch (dir)
            {
                case "UP":
                    temp = position.TempMoveY(-am);
                    break;
                case "DOWN":
                    temp = position.TempMoveY(am);
                    break;
                case "LEFT":
                    temp = position.TempMoveX(-am);
                    break;
                case "RIGHT":
                    temp = position.TempMoveX(am);
                    break;
                default:
                    return false;
            }
            foreach (BaseSprite s in colliders)
            {
                if (temp.Equals(s.position))
                {
                    return false;
                }
            }

            Move(dir, am);
            return true;
        }

        public void Move(string dir, int am)
        {
            Engine.renderer.MarkDirty(position.Clone());
            switch (dir)
            {
                case "UP":
                    position.MoveY(-am);
                    break;
                case "DOWN":
                    position.MoveY(am);
                    break;
                case "LEFT":
                    position.MoveX(-am);
                    break;
                case "RIGHT":
                    position.MoveX(am);
                    break;
                default:
                    break;
            }
        }

        public static bool CheckColliding(BaseSprite spr1, BaseSprite spr2)
        {
            return Position.AreEqual(spr1.position, spr2.position);
        }

        public static bool CheckColliding(Sprite spr1, BaseSprite spr2)
        {
            foreach (BaseSprite s in spr1.sprites)
            {
                if (CheckColliding(s, spr2)) return true;
            }
            return false;
        }

        public static bool CheckColliding(BaseSprite spr1, Sprite spr2)
        {
            return CheckColliding(spr2, spr1);
        }

        public static bool CheckColliding(Sprite spr1, Sprite spr2)
        {
            foreach (BaseSprite s1 in spr1.sprites)
            {
                foreach (BaseSprite s2 in spr2.sprites)
                {
                    if (CheckColliding(s1, s2)) return true;
                }
            }
            return false;
        }

        public static void AddSprite(Sprite spr)
        {
            spriteList.Add(spr);
        }

        public static void RemoveSprite(Sprite spr)
        {
            spriteList.Remove(spr);
        }

        public static void ClearSprites()
        {
            List<Sprite> temp = new List<Sprite>();
            foreach (Sprite s in spriteList)
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
            return spriteList;
        }

        public void AddCollider(Sprite s)
        {
            foreach (BaseSprite bs in s.sprites)
            {
                colliders.Add(bs);
            }
        }
    }

    /// <summary>
    /// Create a sprite of one or more pixels
    /// </summary>
    class Sprite
    {
        public List<BaseSprite> sprites = new List<BaseSprite>();
        public List<BaseSprite> colliders = new List<BaseSprite>();

        public Sprite lastCollision = null;

        public int[] dimensions;

        public bool dirty = true;

        public Sprite(char[] syms, ConsoleColor[] colors, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    sprites.Add(new BaseSprite(syms[i * rect.w + j], j + rect.topLeft.x, i + rect.topLeft.y, false, colors[i * rect.w + j], this));
                }
            }

            foreach (BaseSprite s in sprites)
            {
                s.symbol.SetZ(1);
            }

            dimensions = new int[2]{rect.h, rect.w};
            BaseSprite.AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public Sprite(char[] syms, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    sprites.Add(new BaseSprite(syms[i * rect.w + j], j + rect.topLeft.x, i + rect.topLeft.y, false, this));
                }
            }

            foreach (BaseSprite s in sprites)
            {
                s.symbol.SetZ(1);
            }

            dimensions = new int[2] { rect.h, rect.w };
            BaseSprite.AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public Sprite(char sym, ConsoleColor c, Rect rect)
        {
            BaseSprite bs = new BaseSprite(sym, rect.topLeft, c, this);
            bs.symbol.SetZ(2);
            sprites.Add(bs);
            dimensions = new int[2] { rect.h, rect.w };
            BaseSprite.AddSprite(this);
            Engine.renderer.AddSprite(this);
        }

        public bool MoveCollision(string dir, int am)
        {
            Position temp = null;
            foreach (BaseSprite s in sprites)
            {
                switch (dir)
                {
                    case "UP":
                        temp = s.position.TempMoveY(-am);
                        break;
                    case "DOWN":
                        temp = s.position.TempMoveY(am);
                        break;
                    case "LEFT":
                        temp = s.position.TempMoveX(-am);
                        break;
                    case "RIGHT":
                        temp = s.position.TempMoveX(am);
                        break;
                    default:
                        return false;
                }

                foreach (BaseSprite cs in colliders)
                {
                    if (temp.Equals(cs.position))
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
            foreach (BaseSprite s in sprites)
            {
                s.Move(dir, am);
            }
            dirty = true;
        }

        public void SetPosition(Position pos)
        {
            for (int i = 0; i < dimensions[1]; i++)
            {
                for (int j = 0; j < dimensions[0]; j++)
                {
                    sprites[i * dimensions[0] + j].position.SetX(pos.x + j);
                    sprites[i * dimensions[0] + j].position.SetY(pos.y + i);
                }
            }
            dirty = true;
        }

        public void SetZ(int z)
        {
            foreach (BaseSprite bs in sprites)
            {
                bs.symbol.SetZ(z);
            }
            dirty = true;
        }

        public Position GetPosition()
        {
            return sprites[0].position;
        }

        public void AddCollider(Sprite s)
        {
            foreach (BaseSprite bs in s.sprites)
            {
                colliders.Add(bs);
            }
        }
    }
}
