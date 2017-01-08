using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLE
{

    class Util
    {
        public static int WIDTH = 1;
        public static int HEIGHT = 0;

        public static T[] GetRow<T>(T[,] matrix, int row)
        {
            var columns = matrix.GetLength(1);
            var array = new T[columns];
            for (int i = 0; i < columns; i++)
                array[i] = matrix[row, i];
            return array;
        }

        public static string PadRight(string text, char padder, int num)
        {
            var padding = "";
            for (var i = 0; i < num - text.Length; i++)
            {
                padding += padder;
            }
            return text + padding;
        }
    }

    class IVector
    {
        public int x { get; set; }
        public int y { get; set; }

        public int w { get { return x; } set { x = value; } }
        public int h { get { return y; } set { y = value; } }

        public IVector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // TODO
        // More things
    }

    class DVector
    {
        public double x { get; set; }
        public double y { get; set; }

        public double w { get { return x; } set { x = value; } }
        public double h { get { return y; } set { y = value; } }

        public DVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public DVector Add(double x, double y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        public DVector Subtract(double x, double y)
        {
            return this.Add(-x, -y);
        }

        public DVector Add(DVector other)
        {
            return this.Add(other.x, other.y);
        }

        public DVector Subtract(DVector other)
        {
            return this.Add(-other.x, -other.y);
        }

        public static DVector operator +(DVector v1, DVector v2)
        {
            return new DVector(v1.x + v2.x, v1.y + v2.y);
        }

        public static DVector operator -(DVector v1, DVector v2)
        {
            return new DVector(v1.x - v2.x, v1.y - v2.y);
        }
    }

    class Rect
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public Rect(IVector pos, IVector size)
        {
            SetPosition(pos);
            SetSize(size);
        }

        public Rect(Rect r)
        {
            CopyRect(r);
        }

        public Rect(int x, int y, int w, int h)
        {
            SetRect(x, y, w, h);
        }

        public void CopyRect(Rect r)
        {
            SetRect(r.x, r.y, r.w, r.h);
        }

        public void SetRect(int x, int y, int w, int h)
        {
            SetPosition(x, y);
            SetSize(w, h);
        }

        public void CopyPosition(Rect r)
        {
            SetPosition(r.x, r.y);
        }

        public void SetPosition(IVector v)
        {
            SetPosition(v.x, v.y);
        }

        public virtual void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void CopySize(Rect r)
        {
            SetSize(r.w, r.h);
        }

        public void SetSize(IVector v)
        {
            SetSize(v.w, v.h);
        }

        public virtual void SetSize(int w, int h)
        {
            this.w = w;
            this.h = h;
        }

        public bool InBounds(int x, int y)
        {
            return x >= this.x && y >= this.y && x < this.x + w && y < this.y + h;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + w + ", " + h + ")";
        }

    }
}
