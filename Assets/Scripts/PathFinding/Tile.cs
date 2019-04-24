using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class handles all the work a node needs to do for pathfinding.
/// </summary>
/// <remarks>
/// This class knows if its reachable and all of its neighbors.
/// </remarks>
[Serializable]
public class Tile: GridObject, IHasNeighbors<Tile>
{
    /// <summary>
    /// The reachable field represents whether or not this block can be reached.
    /// </summary>
    [SerializeField]
    bool reachable;

    /// <summary>
    /// Gets or sets if a tile is reachable.
    /// </summary>
    public bool Reachable
    {
        get
        {
            return reachable;
        }
        set
        {
            reachable = value;
        }
    }

    /// <summary>
    /// Gets of sets all the neighbors of this tile.
    /// </summary>
    List<Tile> neighbors;

    /// <summary>
    /// The neighborOffests field represents a list of movements to get to each of this tiles neighbor's from this tile.
    /// </summary>
    List<Point> neighborOffsets = new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };

    /// <summary>
    /// Gets all the reachable neighbors of this tile.
    /// </summary>
    public IEnumerable<Tile> ReachableNeighbors
    {
        get
        {
            return neighbors.Where(o => o.reachable);
        }
    }

    /// <summary>
    /// Initializes a new instance of the Tile class with an x and y position.
    /// </summary>
    /// <param name="x">The x position of this tile</param>
    /// <param name="y">The y position of this tile</param>
    public Tile(int x, int y)
        : base(x, y)
    {
        reachable = false;
    }

    /// <summary>
    /// Determines all the neighbors for this tile.
    /// </summary>
    /// <remarks>
    /// Should be called after the game board has been created.
    /// </remarks>
    /// <param name="Board">A collection of all the tiles in the game board</param>
    /// <param name="BoardSize">The dementions of the game board</param>
	public void FindNeighbors(Dictionary<Point, TileButton> Board, Point BoardSize)
    {
        //  Intializes the neighbors collection
        neighbors = new List<Tile>();

        //  Loop through all the possible positions of neighbors of this tile
        foreach (Point point in neighborOffsets)
        {
            int neighborX = Location.X + point.X;
            int neighborY = Location.Y + point.Y;
            int xOffset = neighborY / 2;

            //  Check if there is a tile at this position
            if (Board.ContainsKey(new Point(neighborX, neighborY)))
            {
                neighbors.Add(Board[new Point(neighborX, neighborY)].tile);
            }
        }
    }
}
