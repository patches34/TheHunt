using System;
using UnityEngine;

[Serializable]
public struct Point
{
    public int X, Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return String.Format("({0}, {1})", X, Y);
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
}
