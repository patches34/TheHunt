using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	[SerializeField]
	Point currentCoord, goalCoord;

	public float moveLerpTime, timer;
	Vector2 lerpStart, lerpEnd;

	[SerializeField]
	RectTransform rectTrans;

	[SerializeField]
	TurnActor actor;

	// Use this for initialization
	void Start ()
	{
		currentCoord = new Point(-1, -1);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void TakeTurn()
	{
		timer = 0;


	}

	#region Find Path
	public Path<Tile> FindPath(
		Tile start,
		Tile destination)
	{
		var closed = new HashSet<Tile>();
		var queue = new PriorityQueue<double, Path<Tile>>();
		queue.Enqueue(0, new Path<Tile>(start));

		while (!queue.IsEmpty)
		{
			var path = queue.Dequeue();

			if (closed.Contains(path.LastStep))
				continue;
			if (path.LastStep.Equals(destination))
				return path;

			closed.Add(path.LastStep);

			foreach (Tile n in path.LastStep.Neighbours)
			{
				double d = distance(path.LastStep, n);
				var newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n, destination), 
					newPath);
			}
		}

		return null;
	}

	double distance(Tile tile1, Tile tile2)
	{
		return 1;
	}

	double estimate(Tile tile, Tile destTile)
	{
		float dx = Mathf.Abs(destTile.X - tile.X);
		float dy = Mathf.Abs(destTile.Y - tile.Y);
		int z1 = -(tile.X + tile.Y);
		int z2 = -(destTile.X + destTile.Y);
		float dz = Mathf.Abs(z2 - z1);

		return Mathf.Max(dx, dy, dz);
	}
	#endregion

	public void MoveToCoord(Point newPos, bool withLerp)
	{
		if(withLerp)
		{
			lerpStart = currentCoord.ToVector2();

			currentCoord = newPos;

			timer = 0;
		}
		else
		{
			rectTrans.anchoredPosition = BoardManager.Instance.TileCoordToScreenSpace(newPos);
		}
	}
}
