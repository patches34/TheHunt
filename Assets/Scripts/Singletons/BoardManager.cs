using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;

public class BoardManager : Singleton<BoardManager>
{
	[SerializeField]
	Point boardSize;
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
			}
		}
		#endregion

		foreach(TileButton t in tiles.Values)
		{
			t.tile.FindNeighbours(tiles, boardSize, true);
		}

		/*#region Place starting pieces
		TileButton randomTile = GetRandomTile();
		randomTile.SetIsPassible(false);
		randomTile.SetState(TileState.Food);
		GameManager.Instance.foodTile = randomTile.tile;

		do
		{
			randomTile= GetRandomTile();
		}while(randomTile.GetState() != TileState.None);
		randomTile.SetIsPassible(false);
		GameManager.Instance.animalActor.MoveToCoord(randomTile.tile.Location, false);

		do
		{
			randomTile= GetRandomTile();
		}while(randomTile.GetState() != TileState.None);
		randomTile.SetIsPassible(false);
		GameManager.Instance.hunterActor.MoveToCoord(randomTile.tile.Location, false);
		#endregion*/
	}

	TileButton GetRandomTile()
	{
		int y = Random.Range(0, boardSize.Y);
		int x = Random.Range(0, boardSize.X);
		x -= y % 2;

		Debug.LogFormat("X: {0}\tY: {1}", x, y);
		return tiles[new Point(x, y)];
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
}