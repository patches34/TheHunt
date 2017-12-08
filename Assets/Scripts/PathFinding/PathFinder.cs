using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CielaSpike;

public class PathFinder : MonoBehaviour
{
	[SerializeField]
	Tile currentTile, goalTile, subGoalTile, nextTile;

	public float moveLerpTime, timer;
	Vector2 lerpStart, lerpEnd;

	[SerializeField]
	RectTransform rectTrans;

	[SerializeField]
	TurnActor actor;

	[SerializeField]
	Path<Tile> movePath;

	public bool isReady;

	// Use this for initialization
	public void Init(Tile start, Tile goal, Tile subGoal = null)
	{
		rectTrans.anchoredPosition = BoardManager.Instance.TileCoordToScreenSpace(start.Location);
		currentTile = start;

		timer = -1;

		gameObject.SetActive(true);

		subGoalTile = subGoal;

		SetGoalTile(goal);
	}

	public void Reset()
	{
		timer = -1;

		goalTile = null;
        
		currentTile = null;

		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GameManager.Instance.IsActorTurn(actor) && isReady)
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

					if(movePath.TotalCost <= 1)
					{
						Debug.Log("Goal reached" + goalTile.Location);
						GameManager.Instance.GoalReached();
					}
					else
					{
						GameManager.Instance.PlayerWent();

						this.StartCoroutineAsync(FindPath());
					}
				}
			}
		}
	}

	public void TakeTurn()
	{
		if(currentTile.Location != goalTile.Location)
		{
			Debug.LogFormat("{0} path cost: {1}", actor, movePath.TotalCost);
			BoardManager.Instance.SetTileInteractable(nextTile.Location, false);

			lerpStart = BoardManager.Instance.TileCoordToScreenSpace(currentTile.Location);
			lerpEnd = BoardManager.Instance.TileCoordToScreenSpace(nextTile.Location);

			timer = 0;
		}
		else
		{
			GameManager.Instance.PlayerWent();
		}
	}

	public void CheckForPathBlocked(Point newBlocked)
	{
		foreach(Tile t in movePath)
		{
			if(t.Location.Equals(newBlocked))
			{
				Debug.LogFormat("{0} find new path", actor);
				this.StartCoroutineAsync(FindPath());
			}
		}
	}

	#region Find Path
	IEnumerator FindPath()
	{
		isReady = false;
		movePath = null;

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
				movePath = path;

				nextTile = movePath.ElementAt((int)movePath.TotalCost - 1);

				break;
			}

			closed.Add(path.LastStep);

			foreach (Tile t in path.LastStep.Neighbours)
			{
				double d = distance(path.LastStep, t);
				var newPath = path.AddStep(t, d);

				double costVaule = newPath.TotalCost + estimate(t, goalTile);
				if(subGoalTile != null)
				{
					costVaule += estimate(t, subGoalTile);
				}
				queue.Enqueue(costVaule, newPath);
			}
		}

		if(movePath == null)
		{
			Debug.LogFormat("No Path for {0}", GameManager.Instance.Turn);
			GameManager.Instance.GoalBlocked();
		}
		else if(movePath.TotalCost <= 0)
		{
			Debug.LogFormat("{0} at goal", GameManager.Instance.Turn);
		}

		isReady = true;

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
}
