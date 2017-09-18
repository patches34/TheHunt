using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;


public enum TurnActor
{
	Player,
	Hunter,
	Animal
}

public class GameManager : Singleton<GameManager>
{
	public TurnActor turn;// {get; private set;}
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
		BoardManager.Instance.CreateBoard();

		animalActor.gameObject.SetActive(true);
		hunterActor.gameObject.SetActive(true);
	}

	void Update()
	{
		if(turn != TurnActor.Player)
		{
			turnWaitTimer += Time.deltaTime;

			if(turnWaitTimer >= compTurnWait)
			{
				turnWaitTimer = 0;

				switch(turn)
				{
				case TurnActor.Animal:
					turn = TurnActor.Player;
					break;
				case TurnActor.Hunter:
					turn = TurnActor.Animal;
					break;
				}
			}
		}
	}

	public void PlayerWent()
	{
		Debug.Log("Player went");
		turn = TurnActor.Hunter;
	}
}