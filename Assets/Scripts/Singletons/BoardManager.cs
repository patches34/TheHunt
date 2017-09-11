using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;

public class BoardManager : Singleton<BoardManager>
{
	[SerializeField]
	int boardWidth, boardHeight, tileSize, tileSpacing;

	[SerializeField]
	GameObject tilePrefab;

	List<List<TileButton>> tiles;

	// Use this for initialization
	protected BoardManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}

	public void CreateBoard()
	{
		tiles = new List<List<TileButton>>();

		for(int row = 0; row < boardHeight; ++row)
		{
			List<TileButton> rowTiles = new List<TileButton>();

			for(int column = 0; column < boardWidth - (row % 2); ++column)
			{
				Vector3 pos = Vector3.zero;
				if(row % 2 == 0)
				{
					pos.x = column * (tileSize + tileSpacing) + (tileSize / 2f);
				}
				else
				{
					pos.x = column * (tileSize + tileSpacing) + (tileSize / 2f) + ((tileSize + tileSpacing) / 2f);
				}
				pos.y = row * tileSize + (tileSize / 2f);

				GameObject newTile = Instantiate(tilePrefab, transform);

				newTile.GetComponent<RectTransform>().anchoredPosition = pos;

				rowTiles.Add(newTile.GetComponent<TileButton>());
			}

			tiles.Add(rowTiles);
		}

		TileButton randomTile = GetRandomTile();
		randomTile.SetState(TileState.Animal);

		int i = 0;
		do
		{
			++i;
			randomTile= GetRandomTile();
		}while(randomTile.GetState() != TileState.None);
		randomTile.SetState(TileState.Hunter);

		do
		{
			++i;
			randomTile= GetRandomTile();
		}while(randomTile.GetState() != TileState.None);
		randomTile.SetState(TileState.Food);

		Debug.Log(i);
	}

	TileButton GetRandomTile()
	{
		int y = Random.Range(0, boardHeight);
		int x = Random.Range(0, boardWidth - (y % 2));

		return tiles[y][x];
	}
}