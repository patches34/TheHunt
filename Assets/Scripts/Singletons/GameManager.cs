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

	public Tile foodTile;

	public PathFinder animalActor, hunterActor;

	#region Initialization
	// Use this for initialization
	protected GameManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
		turn = TurnActor.Player;
		isWaiting = true;

		BoardManager.Instance.CreateBoard();

		#region Place starting pieces
		TileButton randomTile = BoardManager.Instance.GetRandomTile();
		randomTile.SetIsInteractable(false);
		randomTile.SetState(TileState.Food);
		foodTile = randomTile.tile;

		randomTile= BoardManager.Instance.GetRandomTile();
		randomTile.SetIsInteractable(false);
		animalActor.Init(randomTile.tile, foodTile);

		randomTile= BoardManager.Instance.GetRandomTile();
		randomTile.SetIsInteractable(false);
		hunterActor.Init(randomTile.tile, animalActor.GetTile());
		#endregion

		animalActor.gameObject.SetActive(true);
		hunterActor.gameObject.SetActive(true);

		isRunning = true;
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
			Debug.LogError("Game Over\nHunter Won");
			isRunning = false;
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
}