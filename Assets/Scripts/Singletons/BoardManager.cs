using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
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

    public int spawnPoints, spawnDistance;
    Dictionary<int, List<Point>> spawnGroups;

	public int tileSize, tileSpacing;

	[SerializeField]
	GameObject tilePrefab;

	Dictionary<Point, TileButton> tiles;

	// Use this for initialization
	protected BoardManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}

	public void CreateBoard()
	{
		Stopwatch timer = new Stopwatch();
		long boardTime, generateTime, neighboursTime, selectTime;

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
				GameObject newTile = Instantiate(tilePrefab, transform);
				newTile.transform.SetAsFirstSibling();

				TileButton tileBtn = newTile.GetComponent<TileButton>();
				//	Set tile coordinate position
				tileBtn.tile = new Tile(column - row / 2, row);
				newTile.GetComponent<RectTransform>().anchoredPosition = TileCoordToScreenSpace(tileBtn.tile.Location);

				tiles.Add(tileBtn.tile.Location, tileBtn);

                newTile.SetActive(false);
			}
		}
		generateTime = timer.ElapsedMilliseconds;
        #endregion

        #region Set neighbours
        foreach (TileButton t in tiles.Values)
		{
			t.tile.FindNeighbours(tiles, boardSize, true);
		}
		neighboursTime = timer.ElapsedMilliseconds;
        #endregion

        #region Select spawn points
		spawnGroups = new Dictionary<int, List<Point>>();
        List<Point> spawns = new List<Point>();
		List<int> activeSpawns = new List<int>();
        for(int i = 0; i < spawnPoints; ++i)
        {
            Point spawn = GetRandomBoardPoint(spawns, spawnDistance);

			tiles[spawn].gameObject.SetActive(true);
			spawns.Add(spawn);

			spawnGroups.Add(i, new List<Point> {spawn});
			activeSpawns.Add(i);
        }
		selectTime = timer.ElapsedMilliseconds;
        #endregion

		#region Spread from spawns
		//	each spawn group grows until it hits another group
		int currentSpawnIndex = 0;
		List<int> doneSpawnGroups = new List<int>();
		do
		{
			if(!doneSpawnGroups.Contains(currentSpawnIndex))
			{
				//	Pick random tile from group
				int tileIndex = Random.Range(0, spawnGroups[currentSpawnIndex].Count);
				Point randomTile = spawnGroups[currentSpawnIndex][tileIndex];

				//	Pick random neighbor tile
				Point neighbour = tiles[randomTile].tile.GetRandomNeighbour();

				tiles[neighbour].gameObject.SetActive(true);
				spawnGroups[currentSpawnIndex].Add(neighbour);

				//	Check if hit another spawn group
				for(int i = 0; i < spawnGroups.Count; ++i)
				{
					if(i == currentSpawnIndex)
						continue;
					else if(spawnGroups[i].Contains(neighbour))
					{
						doneSpawnGroups.Add(currentSpawnIndex);
					}
				}
			}

			//	Move to next spawn group
			++currentSpawnIndex;
			//	Check if have to reset the index counter
			if(currentSpawnIndex >= activeSpawns.Count)
			{
				currentSpawnIndex = 0;
			}
		}while(doneSpawnGroups.Count < spawnGroups.Count);
		#endregion

		timer.Stop();

		UnityEngine.Debug.LogFormat("Total Time: {0:F}\nSpread Time: {1:F}", 
			timer.ElapsedMilliseconds / 1000f, 
			(timer.ElapsedMilliseconds - selectTime) / 1000f);
    }

    public void Reset()
	{
		foreach(TileButton t in tiles.Values)
		{
			t.SetState(TileState.None);
		}
	}

	public void DestoryBoard()
	{
		foreach(TileButton t in tiles.Values)
		{
			Destroy(t.gameObject);
		}
	}

	public TileButton GetRandomTile(List<Point> usedTiles = null, int minDistance = 1)
	{
		Point randoPoint = new Point();

		do
		{
			randoPoint.Y = Random.Range(0, boardSize.Y);
			randoPoint.X = Random.Range(0, boardSize.X - (randoPoint.Y % 2));
			randoPoint.X -= (randoPoint.Y / 2);
			randoPoint.Z = -(randoPoint.X + randoPoint.Y);

			if(tiles[randoPoint].tile.Passable)
			{
				if(usedTiles == null)
				{
					break;
				}
				else
				{
					bool flag = true;
					foreach(Point p in usedTiles)
					{
						if(randoPoint.Distance(p) <= minDistance)
						{
							flag = false;
							break;
						}
						//Debug.LogFormat("P1 {0}\tP2 {1}\nDistance = {2}", randoPoint, p, randoPoint.Distance(p));
					}

					if(flag)
					{
						break;
					}
				}
			}
		}while(true);

		return tiles[randoPoint];
	}

    Point GetRandomBoardPoint(List<Point> usedTiles = null, int minDistance = 1)
    {
        Point randoPoint = new Point();
        int maxTries = 10, tries = 0;

        do
        {
            ++tries;
            randoPoint.Y = Random.Range(0, boardSize.Y);
            randoPoint.X = Random.Range(0, boardSize.X - (randoPoint.Y % 2));
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
			UnityEngine.Debug.LogWarning("GetRandomBoardPoint hit max tries");

        return randoPoint;
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
}