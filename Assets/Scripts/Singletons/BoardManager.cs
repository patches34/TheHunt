using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;
using System.Diagnostics;
using CielaSpike;

public enum BoardSetupMethods
{
    BASIC,
    SPREAD_POINT
}

public class BoardManager : Singleton<BoardManager>
{
	[SerializeField]
	Point boardSize;
	public Point BoardSize
	{
		get
		{
			return boardSize;
		}
	}

    public int spawnPoints, spawnDistance, blockPoints, blockDistance, maxTries;
    Dictionary<int, List<Point>> spawnGroups;

	public int tileSize, tileSpacing;

	[SerializeField]
	GameObject tilePrefab;

	Dictionary<Point, TileButton> tiles;

    public BoardSetupMethods boardSetupMethod;

	List<Point> actorPoints = new List<Point>();
	public List<Point> AcotrPoints
	{
		get
		{
			return actorPoints;
		}
  	}

	Task boardSetupTask;
	public Task BoardSetupTask
	{
		get
		{
			return boardSetupTask;
		}
	}

	[SerializeField]
	Transform boardTransform;

	// Use this for initialization
	protected BoardManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}

	public IEnumerator CreateBoard()
	{
		Stopwatch timer = new Stopwatch();
		long boardTime, generateTime, neighboursTime;

		timer.Start();

		#region Resize game board
		Vector2 boardRect = new Vector2();
		boardRect.x = ((tileSize + tileSpacing) * boardSize.X - tileSpacing) / 2f;
		boardRect.y = (tileSize * boardSize.Y) / 2f;

		yield return Ninja.JumpToUnity;
		MenuManager.Instance.ResizeGameBoard(boardRect);
		boardTime = timer.ElapsedMilliseconds;
		#endregion

		#region Generate tiles
		tiles = new Dictionary<Point, TileButton>();

		for(int row = 0; row < boardSize.Y; ++row)
		{
			for(int column = 0; column < boardSize.X - (row % 2); ++column)
			{
				GameObject newTile = Instantiate(tilePrefab, boardTransform);
				newTile.transform.SetAsFirstSibling();

				TileButton tileBtn = newTile.GetComponent<TileButton>();
				//	Set tile coordinate position
				tileBtn.tile = new Tile(column - row / 2, row);
				newTile.GetComponent<RectTransform>().anchoredPosition = TileCoordToScreenSpace(tileBtn.tile.Location);

				tiles.Add(tileBtn.tile.Location, tileBtn);

                newTile.SetActive(false);
			}
		}

		yield return Ninja.JumpBack;
		generateTime = timer.ElapsedMilliseconds;
        #endregion

        #region Set neighbours
        foreach (TileButton t in tiles.Values)
		{
			t.tile.FindNeighbours(tiles, boardSize, true);
		}
		neighboursTime = timer.ElapsedMilliseconds;
        #endregion

		timer.Stop();

		UnityEngine.Debug.LogFormat("Create Board Time: {0}", 
			timer.ElapsedMilliseconds / 1000f);
    }

	public IEnumerator SetupBoard()
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        actorPoints = new List<Point>();
       
		yield return Ninja.JumpToUnity;
        switch(boardSetupMethod)
        {
      	case BoardSetupMethods.BASIC:
			this.StartCoroutineAsync(BasicSetup(), out boardSetupTask);
            break;
        case BoardSetupMethods.SPREAD_POINT:
			this.StartCoroutineAsync(SpreadPointMethod(), out boardSetupTask);
            break;
        }

		yield return StartCoroutine(boardSetupTask.Wait());
		yield return Ninja.JumpBack;

        UnityEngine.Debug.LogFormat("Setup Board Time: {0}",
            timer.ElapsedMilliseconds / 1000f);

		yield return null;
    }

    public void Reset()
	{
		foreach(TileButton t in tiles.Values)
		{
            t.gameObject.SetActive(false);
		}
	}

	public void DestoryBoard()
	{
		foreach(TileButton t in tiles.Values)
		{
			Destroy(t.gameObject);
		}
	}

	Point GetRandomBoardPoint(List<Point> usedTiles = null, int minDistance = 1, bool allowNull = false)
    {
        Point randoPoint = new Point();
        int tries = 0;

        do
        {
            ++tries;
			randoPoint.Y = GameManager.Instance.rand.Next(boardSize.Y);
			randoPoint.X = GameManager.Instance.rand.Next(boardSize.X - (randoPoint.Y % 2));
            randoPoint.X -= (randoPoint.Y / 2);
            randoPoint.Z = -(randoPoint.X + randoPoint.Y);

            if (tiles[randoPoint].tile.Passable)
            {
                if (usedTiles == null)
                {
                    break;
                }
                else
                {
                    bool flag = true;
                    foreach (Point p in usedTiles)
                    {
                        if (randoPoint.Distance(p) <= minDistance)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        break;
                    }
                }
            }
        } while (tries <= maxTries);

		if(tries >= maxTries)
		{
			UnityEngine.Debug.LogWarningFormat("GetRandomBoardPoint hit max tries. Point: {0}", randoPoint);

			if(allowNull)
				return new Point(-1, -1);
		}

        return randoPoint;
    }

	Point GetRandomBoardPointAtY(int y)
	{
		List<Point> activeTiles = new List<Point>();

		for(int i = 0; i < boardSize.X - (y % 2); ++i)
		{
			Point p = new Point(i - y / 2, y);
			if(tiles[p].gameObject.activeInHierarchy)
			{
				activeTiles.Add(p);
			}
		}

		return activeTiles[GameManager.Instance.rand.Next(activeTiles.Count)];
	}

	public Vector2 TileCoordToScreenSpace(Point tileCoord)
	{
		Vector2 screenPosition = Vector2.zero;

		//	X
		screenPosition.x = (tileCoord.X + tileCoord.Y / 2) * (tileSize + tileSpacing) + (tileSize / 2f);
		if(tileCoord.Y % 2 != 0)
		{
			screenPosition.x += (tileSize + tileSpacing) / 2f;
		}

		//	Y
		screenPosition.y = tileCoord.Y * tileSize + (tileSize / 2f);

		return screenPosition;
	}

	public void SetTileInteractable(Point tileCoord, bool isInteractable)
	{
		tiles[tileCoord].SetIsInteractable(isInteractable);
	}

	public void SetPathNodeForActor(Point tileCoord, TurnActor actor, bool isOn = true)
	{
		tiles[tileCoord].SetAsPathNodeFor(actor, isOn);
	}

	public void SetBoardSize(int width = 0, int height = 0)
	{
		if(width > 0)
		{
			boardSize.X = width;
		}

		if(height > 0)
		{
			boardSize.Y = height;
		}
	}

    public TileButton GetTileButtonByPoint(Point location)
    {
        return tiles[location];
    }

    #region Board Setup Methods
	IEnumerator BasicSetup()
    {
		#region Reset board
		yield return Ninja.JumpToUnity;
        foreach(TileButton t in tiles.Values)
        {
            t.gameObject.SetActive(true);
        }
		yield return Ninja.JumpBack;
		#endregion

		#region Block tiles
		List<Point> blockedPoints = new List<Point>();
		for(int i = 0; i < blockPoints; ++i)
		{
			Point p = GetRandomBoardPoint(blockedPoints, blockDistance, true);

			if(p.IsNull())
			{
				UnityEngine.Debug.Log(blockedPoints.Count);
				break;
			}
			else
			{
				blockedPoints.Add(p);
			}
		}

		yield return Ninja.JumpToUnity;
		foreach(Point t in blockedPoints)
		{
			tiles[t].gameObject.SetActive(false);
		}

		#endregion

        #region File actor starting tiles
        actorPoints = new List<Point>();

		//	food
		actorPoints.Add(GetRandomBoardPointAtY(boardSize.Y - 1));
		//	Animal
		actorPoints.Add(GetRandomBoardPointAtY(0));

		//	Hunter
		actorPoints.Add(GetRandomBoardPointAtY(boardSize.Y / 2));
        #endregion
    }

	IEnumerator SpreadPointMethod()
    {
        #region Select spawn points
        spawnGroups = new Dictionary<int, List<Point>>();
        List<Point> spawns = new List<Point>();
        List<int> activeSpawns = new List<int>();
        for (int i = 0; i < spawnPoints; ++i)
        {
            Point spawn = GetRandomBoardPoint(spawns, spawnDistance);

            spawns.Add(spawn);

            spawnGroups.Add(i, new List<Point> { spawn });
            activeSpawns.Add(i);
        }
        #endregion

        #region Spread from spawns
        //	each spawn group grows until it hits another group
        int currentSpawnIndex = 0;
        List<int> doneSpawnGroups = new List<int>();
        do
        {
            if (!doneSpawnGroups.Contains(currentSpawnIndex))
            {
                //	Pick random tile from group
				int tileIndex = GameManager.Instance.rand.Next(spawnGroups[currentSpawnIndex].Count);
                Point randomTile = spawnGroups[currentSpawnIndex][tileIndex];

                //	Pick random neighbor tile
                Point neighbour = tiles[randomTile].tile.GetRandomNeighbour();

				if(!spawnGroups.ContainsKey(currentSpawnIndex))
					continue;
				
               	spawnGroups[currentSpawnIndex].Add(neighbour);

                //	Check if hit another spawn group
                for (int i = 0; i < spawnGroups.Count; ++i)
                {
                    if (i == currentSpawnIndex)
                        continue;
                    else if (spawnGroups[i].Contains(neighbour))
                    {
                        doneSpawnGroups.Add(currentSpawnIndex);
                    }
                }
            }

            //	Move to next spawn group
            ++currentSpawnIndex;
            //	Check if have to reset the index counter
            if (currentSpawnIndex >= activeSpawns.Count)
            {
                currentSpawnIndex = 0;
            }
        } while (doneSpawnGroups.Count < spawnGroups.Count);
        #endregion

		#region Activevate Tiles
		yield return Ninja.JumpToUnity;
		for(int i = 0; i < spawnPoints; ++i)
		{
			foreach(Point tile in spawnGroups[i])
			{
				tiles[tile].gameObject.SetActive(true);
			}
			yield return null;
		}
		yield return Ninja.JumpBack;
		#endregion

		actorPoints = new List<Point> { spawnGroups[0][0], spawnGroups[1][0], spawnGroups[2][0] };
    }
    #endregion
}