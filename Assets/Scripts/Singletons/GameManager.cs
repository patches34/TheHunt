using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;


public enum TurnActor
{
	None,
	Player,
	Hunter,
	Animal
}

public class GameManager : Singleton<GameManager>
{
	[SerializeField]
	TurnActor turn;
	public TurnActor Turn
	{
		get
		{
			return this.turn;
		}
		private set
		{
			this.turn = value;
		}
	}

	public bool isWaiting = true, isRunning = false;
	public float compTurnWait;
	public float turnWaitTimer;

	public TurnActor GameOverState { get; private set; }

	public TileButton foodTile;

	public PathFinder animalActor, hunterActor;

	public bool HideBlockedTiles;

	[Range (1, 10)]
	public int actorMinDistance;

	#region Initialization
	// Use this for initialization
	protected GameManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
		BoardManager.Instance.CreateBoard();

		StartGame();
	}

	void StartGame()
	{
		turn = TurnActor.Player;
		isWaiting = true;

		SetupBoard();

		isRunning = true;
	}

	void SetupBoard()
	{
		List<Point> actorTiles = new List<Point>();

		foodTile = BoardManager.Instance.GetRandomTile();
		foodTile.SetIsInteractable(false);
		foodTile.SetState(TileState.Food);
		actorTiles.Add(foodTile.tile.Location);

		TileButton randomTile = BoardManager.Instance.GetRandomTile(actorTiles, actorMinDistance);
		randomTile.SetIsInteractable(false);
		animalActor.Init(randomTile.tile, foodTile.tile);
		actorTiles.Add(randomTile.tile.Location);

		randomTile= BoardManager.Instance.GetRandomTile(actorTiles, actorMinDistance);
		randomTile.SetIsInteractable(false);
		hunterActor.Init(randomTile.tile, animalActor.GetTile());
	}

	void Update()
	{
		
	}

	public void PlayerWent()
	{
		switch(turn)
		{
		case TurnActor.Player:
			turn = TurnActor.Hunter;
			break;
		case TurnActor.Hunter:
			turn = TurnActor.Animal;
			break;
		case TurnActor.Animal:
			hunterActor.SetGoalTile(animalActor.GetTile());

			turn = TurnActor.Player;
			break;
		}

		isWaiting = false;
	}

	public void GoalReached(TurnActor actor = TurnActor.None)
	{
		if(turn == TurnActor.Hunter || actor == TurnActor.Hunter)
		{
			GameOver(false);
		}
	}

	public void GoalBlocked(TurnActor actor)
	{
		switch(actor)
		{
		case TurnActor.Animal:
			GameOver(false);
			break;
		case TurnActor.Hunter:
			GameOver(true);
			break;
		}
	}

	public bool IsActorTurn(TurnActor actor)
	{
		if(isRunning)
		{
			return turn == actor;
		}
		else
		{
			return false;
		}
	}

	void GameOver(bool didPlayerWin)
	{
		if(didPlayerWin)
		{
			GameOverState = TurnActor.Player;
		}
		else
		{
			GameOverState = turn;
		}


		isRunning = false;
		turn = TurnActor.None;

		MenuManager.Instance.ShowMenu(MenuTypes.GameOver);
	}

	public void Restart()
	{
		GameOverState = TurnActor.None;

		BoardManager.Instance.Reset();

		foodTile.SetState(TileState.None);

		animalActor.Reset();
		hunterActor.Reset();

		turn = TurnActor.None;

		isRunning = false;

		StartGame();
	}

	public void Rebuild()
	{
		BoardManager.Instance.DestoryBoard();

		Start();
	}
}