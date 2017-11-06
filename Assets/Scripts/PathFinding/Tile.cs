using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Tile: GridObject, IHasNeighbours<Tile>
{
    public bool Passable;

    public Tile(int x, int y)
        : base(x, y)
    {
		Passable = false;
    }

    public IEnumerable<Tile> AllNeighbours { get; set; }
    public IEnumerable<Tile> Neighbours
    {
        get { return AllNeighbours.Where(o => o.Passable); }
    }

	public void FindNeighbours(Dictionary<Point, TileButton> Board,
		Point BoardSize, bool EqualLineLengths)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Point point in NeighbourShift)
        {
            int neighbourX = X + point.X;
            int neighbourY = Y + point.Y;
            int xOffset = neighbourY / 2;

            if (neighbourY % 2 != 0 && !EqualLineLengths &&
                neighbourX + xOffset == BoardSize.X - 1)
			{
                continue;
			}

            if (neighbourX >= 0 - xOffset &&
				neighbourX < BoardSize.X - xOffset &&
                neighbourY >= 0 &&
				neighbourY < BoardSize.Y)
			{
				if(Board.ContainsKey(new Point(neighbourX, neighbourY)))
				{
					neighbours.Add(Board[new Point(neighbourX, neighbourY)].tile);
				}
			}
        }

        AllNeighbours = neighbours;
    }

    public static List<Point> NeighbourShift
    {
        get
        {
            return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };
        }
    }

	public Point GetRandomNeighbour()
	{
		int index = UnityEngine.Random.Range(0, AllNeighbours.Count());

		return AllNeighbours.ElementAt<Tile>(index).Location;
	}
}
