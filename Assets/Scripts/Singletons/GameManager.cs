using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CielaSpike;
using GameAnalyticsSDK;

public enum TurnActor
{
	None,
	Player,
	Hunter,
	Animal
}

public enum GameOverReason
{
	PLAYER_WON,
	HUNTER_WON,
	ANIMAL_STARVED,
	FORFEIT
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

	[SerializeField]
	bool isWaiting, isRunning, isGameOver;

	public TurnActor GameOverState {
		get
		{
			if(animalActor.IsBlocked())
				return TurnActor.Animal;
			else if(hunterActor.IsBlocked())
				return TurnActor.Player;
			else
				return TurnActor.Hunter;
		}
	}

	public TileButton foodTile;

	public PathFinder animalActor, hunterActor;

	public System.Random rand = new System.Random();

	Task createBoardTask, setupBoardTask;

	public GameOverReason reason;

	[SerializeField]
	int turnsTaken;
	public int TurnsTaken
	{
		get
		{
			return turnsTaken;
		}
		private set
		{
			turnsTaken = value;
		}
	}

	#region Initialization
	// Use this for initialization
	protected GameManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
		isWaiting = true;
		isRunning = false;
		isGameOver = false;

		MenuManager.Instance.loadingSpinner.SetActive(true);
		StartCoroutine(CreateGame());
	}

	IEnumerator CreateGame()
	{
		this.StartCoroutineAsync(BoardManager.Instance.CreateBoard(), out createBoardTask);

		yield return StartCoroutine(createBoardTask.Wait());

		StartGame();
	}

	void StartGame()
	{
		MenuManager.Instance.loadingSpinner.SetActive(true);
		turn = TurnActor.Player;
		isWaiting = true;

		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, BoardManager.Instance.boardSetupMethod.ToString());

		StartCoroutine(SetupBoard());
	}

	IEnumerator SetupBoard()
	{
		this.StartCoroutineAsync(BoardManager.Instance.SetupBoard(), out setupBoardTask);

		yield return StartCoroutine(setupBoardTask.Wait());

		List<Point> actorTiles = BoardManager.Instance.AcotrPoints;

        foodTile = BoardManager.Instance.GetTileButtonByPoint(actorTiles[0]);
		foodTile.SetIsInteractable(false);
		foodTile.SetState(TileState.Food);

		TileButton randomTile = BoardManager.Instance.GetTileButtonByPoint(actorTiles[1]);
        randomTile.SetIsInteractable(false);
		animalActor.Init(randomTile.tile, foodTile.tile);

		randomTile = BoardManager.Instance.GetTileButtonByPoint(actorTiles[2]);
        randomTile.SetIsInteractable(false);
		hunterActor.Init(randomTile.tile, animalActor.GetTile());

		TurnsTaken = 0;
		isRunning = true;

		MenuManager.Instance.loadingSpinner.SetActive(false);
	}

	void Update()
	{
		if(isRunning && isGameOver && hunterActor.isReady && animalActor.isReady)
		{
			if(animalActor.IsBlocked())
			{
				reason = GameOverReason.ANIMAL_STARVED;

				GameOver(false);
			}
			else
			{
				reason = GameOverReason.PLAYER_WON;

				GameOver(true);
			}
		}
	}

	public void PlayerWent()
	{
		switch(turn)
		{
		case TurnActor.Player:
			++TurnsTaken;

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

	public void BlockTile(Point tile)
	{
		hunterActor.CheckForPathBlocked(tile);
		animalActor.CheckForPathBlocked(tile);
	}

	public void GoalReached(TurnActor actor = TurnActor.None)
	{
		if(turn == TurnActor.Hunter || actor == TurnActor.Hunter)
		{
			reason = GameOverReason.HUNTER_WON;

			GameOver(false);
		}
	}

	public void GoalBlocked()
	{
		isGameOver = true;
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
		isRunning = false;
		turn = TurnActor.None;

		MenuManager.Instance.ShowMenu(MenuTypes.GameOver);

		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, BoardManager.Instance.boardSetupMethod.ToString(), reason.ToString(), TurnsTaken);
	}

	public void Restart()
	{
		CancelTasks();

		BoardManager.Instance.Reset();

		if(foodTile != null)
        	foodTile.gameObject.SetActive(false);

		animalActor.Reset();
		hunterActor.Reset();

		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, BoardManager.Instance.boardSetupMethod.ToString(), TurnsTaken);

		StartGame();
	}

	public void Rebuild()
	{
		CancelTasks();

		BoardManager.Instance.DestoryBoard();

		if(foodTile != null)
			foodTile.gameObject.SetActive(false);

		animalActor.Reset();
		hunterActor.Reset();

		Start();
	}

	void CancelTasks()
	{
		if(createBoardTask.State == TaskState.Running)
			createBoardTask.Cancel();

		if(setupBoardTask.State == TaskState.Running)
		{
			if(BoardManager.Instance.BoardSetupTask != null &&
				BoardManager.Instance.BoardSetupTask.State == TaskState.Running)
			{
				BoardManager.Instance.BoardSetupTask.Cancel();
			}

			setupBoardTask.Cancel();
		}

		StopAllCoroutines();
	}
}