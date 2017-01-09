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
        public int X { get; set; }
        public int Y { get; set; }

        public int W { get { return X; } set { X = value; } }
        public int H { get { return Y; } set { Y = value; } }

        public IVector(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        // TODO
        // More things
    }

    class DVector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double W { get { return X; } set { X = value; } }
        public double H { get { return Y; } set { Y = value; } }

        public DVector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public DVector Add(double x, double y)
        {
            this.X += x;
            this.Y += y;
            return this;
        }

        public DVector Subtract(double x, double y)
        {
            return this.Add(-x, -y);
        }

        public DVector Add(DVector other)
        {
            return this.Add(other.X, other.Y);
        }

        public DVector Subtract(DVector other)
        {
            return this.Add(-other.X, -other.Y);
        }

        public static DVector operator +(DVector v1, DVector v2)
        {
            return new DVector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static DVector operator -(DVector v1, DVector v2)
        {
            return new DVector(v1.X - v2.X, v1.Y - v2.Y);
        }
    }

    class Rect
    {
        public int X;
        public int Y;
        public int W;
        public int H;

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
            SetRect(r.X, r.Y, r.W, r.H);
        }

        public void SetRect(int x, int y, int w, int h)
        {
            SetPosition(x, y);
            SetSize(w, h);
        }

        public void CopyPosition(Rect r)
        {
            SetPosition(r.X, r.Y);
        }

        public void SetPosition(IVector v)
        {
            SetPosition(v.X, v.Y);
        }

        public virtual void SetPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void CopySize(Rect r)
        {
            SetSize(r.W, r.H);
        }

        public void SetSize(IVector v)
        {
            SetSize(v.W, v.H);
        }

        public virtual void SetSize(int w, int h)
        {
            this.W = w;
            this.H = h;
        }

        public bool InBounds(int x, int y)
        {
            return x >= this.X && y >= this.Y && x < this.X + W && y < this.Y + H;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + W + ", " + H + ")";
        }

    }
}
