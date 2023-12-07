using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

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

    int spawnPoints, spawnDistance;
	[SerializeField]
	int startBlockedTiles;
	public int StartBlockedTiles
	{
		get
		{
			return startBlockedTiles;
		}
		set
		{
			startBlockedTiles = value;

			PlayerPrefs.SetInt(k_BLOCKED_TILES, startBlockedTiles);
		}
	}

	[SerializeField]
	int startBlockedTilesMinSpacing;
	public int StartBlockedTilesMinSpacing
	{
		get
		{
			return startBlockedTilesMinSpacing;
		}
		set
		{
			startBlockedTilesMinSpacing = value;

			PlayerPrefs.SetInt(k_BLOCKED_TILES_SPACING, startBlockedTilesMinSpacing);
		}
	}

	public int blockedTiles;

    Dictionary<int, List<Point>> spawnGroups;

	[SerializeField]
	int tileSize, tileSpacing, maxTries;

	[SerializeField]
	GameObject tilePrefab;

	Dictionary<Point, TileButton> tiles;

	List<Point> actorPoints = new List<Point>();
	public List<Point> AcotrPoints
	{
		get
		{
			return actorPoints;
		}
  	}

	[SerializeField]
	Transform boardTransform;

	[SerializeField]
	bool showAiPath;
	public bool ShowAiPath
	{
		get
		{
			return showAiPath;
		}
		set
		{
			showAiPath = value;

			PlayerPrefs.SetInt(k_SHOW_AI_PATH, System.Convert.ToInt32(showAiPath));
		}
	}

	const string k_BOARD_SIZE_X = "boardSizeX";
	const string k_BOARD_SIZE_Y = "boardSizeY";
	const string k_BLOCKED_TILES = "blockedTiles";
	const string k_BLOCKED_TILES_SPACING = "blockedTilesSpacing";
	const string k_SHOW_AI_PATH = "showAiPath";

	// Use this for initialization
	protected BoardManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}

	void Awake()
	{
		boardSize.X = PlayerPrefs.GetInt(k_BOARD_SIZE_X, boardSize.X);
		boardSize.Y = PlayerPrefs.GetInt(k_BOARD_SIZE_Y, boardSize.Y);

		startBlockedTiles = PlayerPrefs.GetInt(k_BLOCKED_TILES, startBlockedTiles);
		startBlockedTilesMinSpacing = PlayerPrefs.GetInt(k_BLOCKED_TILES_SPACING, startBlockedTilesMinSpacing);

		showAiPath = System.Convert.ToBoolean(PlayerPrefs.GetInt(k_SHOW_AI_PATH, System.Convert.ToInt32(showAiPath)));
	}

	public void CreateBoard()
	{
		Stopwatch timer = new Stopwatch();
		long boardTime, generateTime, neighboursTime;

		timer.Start();

		#region Resize game board
		Vector2 boardRect = new Vector2();
		boardRect.x = ((tileSize + tileSpacing) * boardSize.X - tileSpacing) / 2f;
		boardRect.y = (tileSize * boardSize.Y) / 2f;

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

				tileBtn.ShowActorPath(ShowAiPath);

                newTile.SetActive(false);
			}
		}

		generateTime = timer.ElapsedMilliseconds;
        #endregion

        #region Set neighbours
        foreach (TileButton t in tiles.Values)
		{
			t.tile.FindNeighbors(tiles, boardSize);
		}
		neighboursTime = timer.ElapsedMilliseconds;
        #endregion

		timer.Stop();

		//GameAnalytics.SettingsGA.SetCustomArea(string.Format("X:{0}_Y:{1}", BoardSize.X, BoardSize.Y));
		//GameAnalytics.NewDesignEvent("BoardCreation", (float)System.Math.Round(timer.Elapsed.TotalSeconds, 4));
    }

	public void SetupBoard()
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        actorPoints = new List<Point>();

        BasicSetup();

		//GameAnalytics.NewDesignEvent("BoardSetup", (float)System.Math.Round(timer.Elapsed.TotalSeconds, 4));
    }

    public void Reset()
	{
		foreach(TileButton t in tiles.Values)
		{
            t.gameObject.SetActive(false);
		}
	}

    public void RestartBoard()
    {
        foreach (TileButton t in tiles.Values)
        {
            t.SetIsInteractable(true);

            if (t.isActiveAndEnabled)
            {
                t.SetState(TileState.None);
            }
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

            if (tiles[randoPoint].tile.Reachable)
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
		if(!tiles.ContainsKey(tileCoord))
			UnityEngine.Debug.LogError(tileCoord);
		tiles[tileCoord].SetAsPathNodeFor(actor, isOn);
	}

	public void SetBoardSize(int width = 0, int height = 0)
	{
		if(width > 0)
		{
			boardSize.X = width;

			PlayerPrefs.SetInt(k_BOARD_SIZE_X, width);
		}

		if(height > 0)
		{
			boardSize.Y = height;

			PlayerPrefs.SetInt(k_BOARD_SIZE_Y, height);
		}
  	}

    public TileButton GetTileButtonByPoint(Point location)
    {
        return tiles[location];
    }
    
	void BasicSetup()
    {
		#region Reset board
        foreach(TileButton t in tiles.Values)
        {
            t.gameObject.SetActive(true);
        }
		#endregion

		#region Block tiles
		List<Point> blockedPoints = new List<Point>();
		for(int i = 0; i < startBlockedTiles; ++i)
		{
			Point p = GetRandomBoardPoint(blockedPoints, startBlockedTilesMinSpacing, true);

			if(p.IsNull())
			{
				//GameAnalytics.NewDesignEvent("BlockedTiles", blockedPoints.Count);
				break;
			}
			else
			{
				blockedPoints.Add(p);
			}
		}

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

	public void ShowActorPaths(bool isVisible)
	{
		ShowAiPath = isVisible;

		foreach(TileButton tile in tiles.Values)
		{
			tile.ShowActorPath(ShowAiPath);
		}
	}
}