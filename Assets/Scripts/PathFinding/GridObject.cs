using System;

[Serializable]
public abstract class GridObject
{
    Point location;

    public Point Location
    {
        get
        {
            return location;
        }
    }

    public GridObject(Point location)
    {
        this.location = location;
    }

    public GridObject(int x, int y)
        : this(new Point(x, y))
    {
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", location.X, location.Y);
    }
}
