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
    class Sprite
    {
        static List<Sprite> spriteList = new List<Sprite>();
        static List<LargeSprite> largeSpriteList = new List<LargeSprite>();

        public List<Sprite> colliders = new List<Sprite>();
        public Position oldPosition = new Position(0, 0);
        public Position position = new Position(0, 0);
        public Pixel symbol;

        public bool hasFrameUpdate = true;

        public Sprite(char sym, Position pos)
        {
            position = pos;
            symbol = new Pixel(sym, position, ConsoleColor.Cyan, ConsoleColor.Black);
            AddSprite(this);
        }

        public Sprite(char sym, int x, int y)
        {
            position.SetX(x);
            position.SetY(y);
            symbol = new Pixel(sym, position, ConsoleColor.Cyan, ConsoleColor.Black);
            AddSprite(this);
        }

        public Sprite(char sym, int x, int y, bool addSprite, ConsoleColor foreGround)
        {
            position.SetX(x);
            position.SetY(y);
            symbol = new Pixel(sym, position, foreGround, ConsoleColor.Black);
            if (addSprite) AddSprite(this);
        }

        public Sprite(char sym, int x, int y, bool addSprite)
        {
            position.SetX(x);
            position.SetY(y);
            symbol = new Pixel(sym, position, ConsoleColor.Cyan, ConsoleColor.Black);
            if (addSprite) AddSprite(this);
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
            foreach (Sprite s in colliders)
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
            oldPosition = position.Clone();
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
            hasFrameUpdate = true;
        }

        public static bool CheckColliding(Sprite spr1, Sprite spr2)
        {
            return Position.AreEqual(spr1.position, spr2.position);
        }

        public static bool CheckColliding(LargeSprite spr1, Sprite spr2)
        {
            foreach (Sprite s in spr1.sprites)
            {
                if (CheckColliding(s, spr2)) return true;
            }
            return false;
        }

        public static bool CheckColliding(Sprite spr1, LargeSprite spr2)
        {
            return CheckColliding(spr2, spr1);
        }

        public static bool CheckColliding(LargeSprite spr1, LargeSprite spr2)
        {
            foreach (Sprite s1 in spr1.sprites)
            {
                foreach (Sprite s2 in spr2.sprites)
                {
                    if (CheckColliding(s1, s2)) return true;
                }
            }
            return false;
        }

        public static void AddSprite(Sprite spr)
        {
            spriteList.Add(spr);
            Engine.QueueMessage(spriteList.ToString());
        }

        public static void RemoveSprite(Sprite spr)
        {
            spriteList.Remove(spr);
        }

        public static List<Sprite> GetSprites()
        {
            return spriteList;
        }

        public static void AddLargeSprite(LargeSprite spr)
        {
            largeSpriteList.Add(spr);
        }

        public static void RemoveLargeSprite(LargeSprite spr)
        {
            largeSpriteList.Remove(spr);
        }

        public static List<LargeSprite> GetLargeSprites()
        {
            return largeSpriteList;
        }

        public void AddCollider(Sprite s)
        {
            colliders.Add(s);
        }

        public void AddLargeCollider(LargeSprite hadron)
        {
            foreach (Sprite s in hadron.sprites)
            {
                colliders.Add(s);
            }
        }
    }

    /// <summary>
    /// Compendium of sprites to create larger ones
    /// </summary>
    class LargeSprite
    {
        public List<Sprite> sprites = new List<Sprite>();
        public List<Sprite> colliders = new List<Sprite>();

        public bool hasFrameUpdate = true;

        public LargeSprite(char[] syms, ConsoleColor[] colors, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    sprites.Add(new Sprite(syms[i * rect.w + j], j + rect.topLeft.x, i + rect.topLeft.y, false, colors[i * rect.w + j]));
                }
            }

            foreach (Sprite s in sprites)
            {
                s.symbol.SetZ(1);
            }

            Sprite.AddLargeSprite(this);
        }

        public LargeSprite(char[] syms, Rect rect, int depth)
        {
            for (int i = 0; i < rect.h; i++)
            {
                for (int j = 0; j < rect.w; j++)
                {
                    sprites.Add(new Sprite(syms[i * rect.w + j], j + rect.topLeft.x, i + rect.topLeft.y, false));
                }
            }

            foreach (Sprite s in sprites)
            {
                s.symbol.SetZ(1);
            }

            Sprite.AddLargeSprite(this);
        }

        public void GenerateColliders()
        {

        }

        public bool MoveCollision(string dir, int am)
        {
            DateTime tempTime = DateTime.Now;
            Position temp = null;
            foreach (Sprite s in sprites)
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

                foreach (Sprite cs in colliders)
                {
                    if (temp.Equals(cs.position))
                    {
                        return false;
                    }
                }
            }

            Move(dir, am);
            Engine.QueueMessage("Collision time: " + (DateTime.Now - tempTime));
            return true;
        }

        public void Move(string dir, int am)
        {
            foreach (Sprite s in sprites)
            {
                s.Move(dir, am);
            }
            hasFrameUpdate = true;
        }

        public void AddCollider(Sprite s)
        {
            colliders.Add(s);
        }

        public void AddLargeCollider(LargeSprite hadron)
        {
            foreach (Sprite s in hadron.sprites)
            {
                colliders.Add(s);
            }
        }
    }
}
