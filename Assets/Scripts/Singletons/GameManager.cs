﻿using UnityEngine;
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

	[SerializeField]
	bool isWaiting, isRunning, isGameOver, isFastFoward;

	public TileButton foodTile;

	public PathFinder animalActor, hunterActor;

	public System.Random rand = new System.Random();

	Task createBoardTask, setupBoardTask;

	public GameOverReason gameOverReason;

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

	[SerializeField]
	int maxPlayerBlocks;
	public int MaxPlayerBlocks
	{
		get
		{
			return maxPlayerBlocks;
		}
		set
		{
			maxPlayerBlocks = value;

			PlayerPrefs.SetInt(k_MAX_PLAYER_BLOCKS, maxPlayerBlocks);
		}
	}

	public int playerBlockedTilesCount;

	[SerializeField]
	bool doesHunterSeekHome;
	public bool DoesHunterSeeksHome
	{
		get
		{
			return doesHunterSeekHome;
		}
		set
		{
			doesHunterSeekHome = value;

			PlayerPrefs.SetInt(k_DOES_HUNTER_SEEK_HOME, System.Convert.ToInt32(doesHunterSeekHome));

			UpdateActorPaths();
		}
	}

	const string k_DOES_HUNTER_SEEK_HOME = "doesHunterSeekHome";
	const string k_MAX_PLAYER_BLOCKS = "maxPlayerBlocks";

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

		doesHunterSeekHome = System.Convert.ToBoolean(PlayerPrefs.GetInt(k_DOES_HUNTER_SEEK_HOME, System.Convert.ToInt32(DoesHunterSeeksHome)));

		maxPlayerBlocks = PlayerPrefs.GetInt(k_MAX_PLAYER_BLOCKS, maxPlayerBlocks);

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
		playerBlockedTilesCount = 0;
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
		hunterActor.Init(randomTile.tile, animalActor.GetTile(), foodTile.tile);

		TurnsTaken = 0;
        isFastFoward = false;
		isRunning = true;

		MenuManager.Instance.loadingSpinner.SetActive(false);
	}

	void Update()
	{
        if(isFastFoward && IsGameActive() && IsPlayerTurn())
        {
            ActorWent();
        }
		if(isRunning && isGameOver && hunterActor.isReady && animalActor.isReady)
		{
			if(animalActor.IsBlocked())
			{
				gameOverReason = GameOverReason.ANIMAL_STARVED;

				GameOver(false);
			}
			else
			{
				gameOverReason = GameOverReason.PLAYER_WON;

				GameOver(true);
			}
		}
	}

    public void ToggleFastForward()
    {
        isFastFoward = !isFastFoward;

        if(isFastFoward)
        {
            GameAnalytics.NewDesignEvent("playerFastFowardStart");
        }
        else
        {
            GameAnalytics.NewDesignEvent("playerFastFowardStop");
        }
    }

	public void PlayerPass()
	{
		if(IsPlayerTurn())
		{
			ActorWent();
		}

		GameAnalytics.NewDesignEvent("playerPassed");
	}

	public void ActorWent()
	{
		switch(turn)
		{
		case TurnActor.Player:
			++TurnsTaken;

			UpdateActorPaths();

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

	public void GoalReached(TurnActor actor)
	{
		if(actor == TurnActor.Hunter)
		{
			gameOverReason = GameOverReason.HUNTER_WON;

			GameOver(false);
		}
		else if(actor == TurnActor.Animal)
		{
			gameOverReason = GameOverReason.PLAYER_WON;

			GameOver(true);
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

        if(isFastFoward)
        {
            GameAnalytics.NewDesignEvent("gameOverFastForward");
        }

		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, BoardManager.Instance.boardSetupMethod.ToString(), gameOverReason.ToString(), TurnsTaken);
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

		isGameOver = false;

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

	public bool IsPlayerTurn()
	{
		return isRunning && !isGameOver && turn == TurnActor.Player;
	}

	public bool IsGameActive()
	{
		return isRunning && !isGameOver;
	}

	public bool CanPlayerBlockTile()
	{
		if(maxPlayerBlocks <= 0 || playerBlockedTilesCount < maxPlayerBlocks)
		{
			return true;
		}

		return false;
	}

	public void UpdateActorPaths()
	{
		if(hunterActor.isActiveAndEnabled)
			hunterActor.FindPath();

		if(animalActor.isActiveAndEnabled)
			animalActor.FindPath();
	}
}