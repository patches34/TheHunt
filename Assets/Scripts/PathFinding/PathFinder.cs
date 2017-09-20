using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	[SerializeField]
	Tile currentTile, goalTile, nextTile;

	public float moveLerpTime, timer;
	Vector2 lerpStart, lerpEnd;

	[SerializeField]
	RectTransform rectTrans;

	[SerializeField]
	TurnActor actor;

	[SerializeField]
	Path<Tile> movePath;

	// Use this for initialization
	public void Init(Tile start, Tile goal)
	{
		rectTrans.anchoredPosition = BoardManager.Instance.TileCoordToScreenSpace(start.Location);
		currentTile = start;

		goalTile = goal;

		timer = -1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GameManager.Instance.turn == actor)
		{
			if(timer < 0)
			{
				
				TakeTurn();
			}
			else
			{
				timer += Time.deltaTime;

				rectTrans.anchoredPosition = Vector2.Lerp(lerpStart, lerpEnd, timer / moveLerpTime);

				if(timer >= moveLerpTime)
				{
					currentTile = nextTile;

					if(movePath.TotalCost == 1)
					{
						Debug.LogErrorFormat("{0} reached goal", GameManager.Instance.turn);
					}

					GameManager.Instance.PlayerWent();

					timer = -1;
				}
			}
		}
	}

	public void TakeTurn()
	{
		timer = 0;

		movePath = FindPath();

		if(movePath == null)
		{
			Debug.LogErrorFormat("No Path for {0}", GameManager.Instance.turn);
		}
		else if(movePath.TotalCost <= 0)
		{
			Debug.LogErrorFormat("{0} at goal", GameManager.Instance.turn);
		}
		else
		{
			nextTile = movePath.ElementAt((int)movePath.TotalCost - 1);

			lerpStart = BoardManager.Instance.TileCoordToScreenSpace(currentTile.Location);
			lerpEnd = BoardManager.Instance.TileCoordToScreenSpace(nextTile.Location);

			timer = 0;
		}
	}

	#region Find Path
	public Path<Tile> FindPath()
	{
		var closed = new HashSet<Tile>();
		var queue = new PriorityQueue<double, Path<Tile>>();
		queue.Enqueue(0, new Path<Tile>(currentTile));

		while (!queue.IsEmpty)
		{
			var path = queue.Dequeue();

			// if this adjacent square is already in the closed list ignore it
			if (closed.Contains(path.LastStep))
			{
				continue;
			}
			// if we added the destination to the closed list, we've found a path
			if (path.LastStep.Equals(goalTile))
			{
				return path;
			}

			closed.Add(path.LastStep);

			foreach (Tile n in path.LastStep.Neighbours)
			{
				double d = distance(path.LastStep, n);
				var newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n, goalTile), 
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

	public Tile GetTile()
	{
		return currentTile;
  	}

	public void SetGoalTile(Tile newGoal)
	{
		goalTile = newGoal;
	}
}
