using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CielaSpike;

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
	Path<Tile> movePath, lastPath;

	public bool isReady;

	[SerializeField]
	int pathIndex;

	// Use this for initialization
	public void Init(Tile start, Tile goal)
	{
		rectTrans.anchoredPosition = BoardManager.Instance.TileCoordToScreenSpace(start.Location);
		currentTile = start;

		timer = -1;

		pathIndex = 0;

		gameObject.SetActive(true);

		SetGoalTile(goal);
	}

	public void Reset()
	{
		timer = -1;

		goalTile = null;
        
		currentTile = null;

		gameObject.SetActive(false);

		movePath = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GameManager.Instance.IsActorTurn(actor) && isReady && movePath != null)
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
					BoardManager.Instance.SetTileInteractable(currentTile.Location, true);

					currentTile = nextTile;

					timer = -1;

					if(movePath.TotalCost - pathIndex <= 0)
					{
						Debug.LogFormat("{0} Goal reached: {1}", actor, goalTile.Location);
						GameManager.Instance.GoalReached(actor);
					}
					else
					{
						GameManager.Instance.ActorWent();
					}
				}
			}
		}
	}

	public void TakeTurn()
	{
		++pathIndex;
		nextTile = movePath.ElementAt((int)movePath.TotalCost - pathIndex);

		BoardManager.Instance.SetTileInteractable(nextTile.Location, false);

		lerpStart = BoardManager.Instance.TileCoordToScreenSpace(currentTile.Location);
		lerpEnd = BoardManager.Instance.TileCoordToScreenSpace(nextTile.Location);

		timer = 0;
	}

	public void CheckForPathBlocked(Point newBlocked)
	{
		isReady = false;

		foreach(Tile t in movePath)
		{
			if(t.Location.Equals(newBlocked))
			{
				this.StartCoroutineAsync(FindPath());

				return;
			}
		}

		isReady = true;
	}

	#region Find Path
	IEnumerator FindPath()
	{
		isReady = false;

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
				yield return Ninja.JumpToUnity;
				UpdatePathNodes(path);
				yield return Ninja.JumpBack;

				break;
			}

			closed.Add(path.LastStep);

			foreach (Tile n in path.LastStep.Neighbours)
			{
				double d = distance(path.LastStep, n);
				var newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n, goalTile), newPath);
			}
		}

		if(queue.IsEmpty)
		{
			Debug.LogFormat("No Path for {0}", actor);
			movePath = null;
			GameManager.Instance.GoalBlocked();
		}
		else if(movePath.TotalCost <= 0)
		{
			Debug.LogFormat("{0} at goal", actor);
		}
		else
		{
			pathIndex = 0;
		}

		isReady = true;
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

	public bool IsBlocked()
	{
		return movePath == null;
	}
	#endregion

	public Tile GetTile()
	{
		return currentTile;
  	}

	public void SetGoalTile(Tile newGoal)
	{
		goalTile = newGoal;

		if(goalTile.Location == currentTile.Location)
		{
			GameManager.Instance.GoalReached(TurnActor.Hunter);
		}
		else
		{
			this.StartCoroutineAsync(FindPath());
		}
	}

	void UpdatePathNodes(Path<Tile> newPath)
	{
		if(movePath != null)
		{
			foreach(Tile t in movePath)
			{
				BoardManager.Instance.SetPathNodeForActor(t.Location, actor, false);
			}
		}

		foreach(Tile t in newPath)
		{
			if(!t.Location.Equals(currentTile.Location) && !t.Location.Equals(goalTile.Location))
				BoardManager.Instance.SetPathNodeForActor(t.Location, actor);
		}

		movePath = newPath;
	}
}
