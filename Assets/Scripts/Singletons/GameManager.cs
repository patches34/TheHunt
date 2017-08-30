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
	public TurnState turn {get; private set;}

	#region Initialization
	// Use this for initialization
	protected GameManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion
}