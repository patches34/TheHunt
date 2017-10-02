﻿using UnityEngine;
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
	}

	public TileButton GetRandomTile()
	{
		Point randoPoint = new Point();

		do
		{
			randoPoint.Y = Random.Range(0, boardSize.Y);
			randoPoint.X = Random.Range(0, boardSize.X - (randoPoint.Y % 2));
			randoPoint.X -= (randoPoint.Y / 2);
		}while(!tiles[randoPoint].tile.Passable);

		return tiles[randoPoint];
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
}