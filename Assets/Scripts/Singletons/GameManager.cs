using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;
using CielaSpike;

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

	[SerializeField]
	bool isWaiting, isRunning, isGameOver;
	public float compTurnWait;
	public float turnWaitTimer;

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

	public int actorMinDistance;

	public int randomBlockedTiles;

	public System.Random rand = new System.Random();

	Task createBoardTask, setupBoardTask;

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
		hunterActor.Init(randomTile.tile, animalActor.GetTile(), foodTile.tile);

		isRunning = true;

		MenuManager.Instance.loadingSpinner.SetActive(false);
	}

	void Update()
	{
		if(isGameOver && hunterActor.isReady && animalActor.isReady)
		{
			if(animalActor.IsBlocked())
			{
				GameOver(false);
			}
			else
			{
				GameOver(true);
			}
		}
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

	public void BlockTile(Point tile)
	{
		hunterActor.CheckForPathBlocked(tile);
		animalActor.CheckForPathBlocked(tile);
	}

	public void GoalReached(TurnActor actor = TurnActor.None)
	{
		if(turn == TurnActor.Hunter || actor == TurnActor.Hunter)
		{
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
	}

	public void Restart()
	{
		CancelTasks();

		BoardManager.Instance.Reset();

		if(foodTile != null)
        	foodTile.gameObject.SetActive(false);

		animalActor.Reset();
		hunterActor.Reset();

		StartGame();
	}

	public void Rebuild()
	{
		BoardManager.Instance.DestoryBoard();

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