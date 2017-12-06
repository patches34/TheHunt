using System;
using UnityEngine;

[Serializable]
public struct Point
{
    public int X, Y, Z;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
		Z = -(x + y);
    }

	public int Distance(Point b)
	{
		return Math.Max(Math.Abs(X - b.X), Math.Max(Math.Abs(Z - b.Z), Math.Abs(Y - b.Y)));
	}

    public override string ToString()
    {
		return String.Format("({0}, {1}, {2})", X, Y, Z);
    }

	public static bool operator ==(Point a, Point b)
	{
		return a.X == b.X && a.Y == b.Y;
	}

	public static bool operator !=(Point a, Point b)
	{
		return a.X != b.X || a.Y != b.Y;
	}

	public static implicit operator Vector2(Point p)
	{
		return new Vector2(p.X, p.Y);
	}

	public bool IsNull()
	{
		return X < 0 && Y < 0;
	}
}
