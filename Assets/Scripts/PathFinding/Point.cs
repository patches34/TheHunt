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

	public Vector2 ToVector2()
	{
		return new Vector2(X, Y);
	}
}
