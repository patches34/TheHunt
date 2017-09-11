using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;


public enum TurnState
{
	Player,
	Hunter,
	Animal
}

public class GameManager : Singleton<GameManager>
{
	public TurnState turn;// {get; private set;}
	public float compTurnWait;
	public float turnWaitTimer;

	public TileButton hunterTile, animalTile, foodTile;

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
	}

	void Update()
	{
		if(turn != TurnState.Player)
		{
			turnWaitTimer += Time.deltaTime;

			if(turnWaitTimer >= compTurnWait)
			{
				turnWaitTimer = 0;

				switch(turn)
				{
				case TurnState.Animal:
					turn = TurnState.Player;
					break;
				case TurnState.Hunter:
					turn = TurnState.Animal;
					break;
				}
			}
		}
	}

	public void PlayerWent()
	{
		Debug.Log("Player went");
		turn = TurnState.Hunter;
	}
}